using System.Collections.Generic;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Unity.Mono;
using HarmonyLib;
using TMPro;
using UnifromCheat_REPO.Funs;
using UnifromCheat_REPO.Patches;
using UnifromCheat_REPO.Utils; 
using UnifromCheat_REPO.WallHack;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnifromCheat_REPO.GUIMenuSkin;
using static UnifromCheat_REPO.Utils.FireboxConsole;
using Object = UnityEngine.Object;

namespace UnifromCheat_REPO
{
    [BepInPlugin("ru.morkamo.unifrom", "Unifrom", "4.1.0")]
    public partial class Core : BaseUnityPlugin
    {
        public static Core Instance;
        public Harmony harmony;
        public ItemsWallHack ItemsWallHack;
        public CosmeticBoxesWallHack CosmeticBoxesWallHack;
        public EnemiesWallHack EnemiesWallHack;
        public PlayersWallHack PlayersWallHack;
        public HintsController HintsController;
        public Noclip Noclip;
        public MiscFunctions MiscFunctions;
        public static GameObject unifromCanvasObject;
        private static Camera[] renderDistanceCameras = new Camera[8];

        public static List<GameObject> UnifromHints = new List<GameObject>();

        private void Start()
        {
            Instance = this;
            
            ItemsWallHack = gameObject.AddComponent<ItemsWallHack>();
            CosmeticBoxesWallHack = gameObject.AddComponent<CosmeticBoxesWallHack>();
            EnemiesWallHack = gameObject.AddComponent<EnemiesWallHack>();
            PlayersWallHack = gameObject.AddComponent<PlayersWallHack>();
            HintsController = gameObject.AddComponent<HintsController>();
            Noclip = gameObject.AddComponent<Noclip>();
            MiscFunctions = gameObject.AddComponent<MiscFunctions>();
            
            harmony = new Harmony("ru.morkamo.unifromPatches");
            harmony.PatchAll();
            
            HintsController.CreateHint($"Unifrom {cheatVersion} - by Morkamo", "UnifromBadge", 338, 242, 8,
                new Color(HC_R, HC_G, HC_B, HC_A));
            
            UnifromHints.Add(GameObject.Find("UnifromBadge"));
            
            HintsController.CreateHint("Noclip - OFF", "NoclipText", -450, 242, 8,
                new Color(HC_R, HC_G, HC_B, HC_A));
            
            HintsController.CreateHint($"Initializing", "initMenuHint", -35, -230, 13,
                new Color(HC_R, HC_G, HC_B, HC_A));
            
            UnifromHints.Add(GameObject.Find("NoclipText"));
            
            FireLog($"\n--------------------------\n     " +
                    $"[CHEAT-INJECTED]\n Welcome to Unifrom {cheatVersion}" +
                    $"\n--------------------------\n");
            
            unifromCanvasObject = new GameObject("UnifromMenuCanvas");
            var canvas = unifromCanvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 9999;
            unifromCanvasObject.AddComponent<CanvasScaler>();
            unifromCanvasObject.AddComponent<GraphicRaycaster>();
            Object.DontDestroyOnLoad(unifromCanvasObject);
            
            FireLog("[Snow] Canvas created: " + unifromCanvasObject);
            
            var snowObj = new GameObject("Snowfall");
            snowObj.transform.SetParent(unifromCanvasObject.transform);
            var snow = snowObj.AddComponent<SnowfallUI>();
            snow.InitSnowAnimation(unifromCanvasObject.GetComponent<RectTransform>());
            Object.DontDestroyOnLoad(snowObj);
            
            ToggleMenuWithDelay();
        }

        private void Update()
        {
            UpdateMenuAnimation();
            GameController.UpdateGameplay();

            if (Keyboard.current.insertKey.wasPressedThisFrame || Keyboard.current.rightAltKey.wasPressedThisFrame
                || Keyboard.current.f11Key.wasPressedThisFrame)
                ToggleMenu();
        }

        private void LateUpdate()
        {
            ApplyRenderDistance();
        }

        private static void ApplyRenderDistance()
        {
            int cameraCount = Camera.allCamerasCount;
            if (cameraCount <= 0)
            {
                ApplyRenderDistance(Camera.main);
                return;
            }

            if (renderDistanceCameras.Length < cameraCount)
                renderDistanceCameras = new Camera[cameraCount];

            int count = Camera.GetAllCameras(renderDistanceCameras);
            for (int i = 0; i < count; i++)
            {
                Camera camera = renderDistanceCameras[i];
                ApplyRenderDistance(camera);
                renderDistanceCameras[i] = null;
            }
        }

        internal static void ApplyRenderDistance(Camera camera)
        {
            if (camera == null)
                return;

            float farClip = Mathf.Clamp(wallHackCameraFarClipPlane, 1f, 10000f);
            camera.farClipPlane = farClip;
            camera.layerCullSpherical = false;
            camera.useOcclusionCulling = false;
        }

        private void UpdateMenuAnimation()
        {
            float target = MenuState ? 1f : 0f;
            menuAnimationProgress = Mathf.MoveTowards(
                menuAnimationProgress,
                target,
                Time.unscaledDeltaTime * MenuAnimationSpeed
            );
        }
        
        internal void ToggleMenu()
        {
            if (!IsUnifromReady)
                return;
            
            MenuState = !MenuState;
            if (!MenuState)
                CloseGameControllerConfirmation();

            if (ps_onlyInMenu)
                unifromCanvasObject.SetActive(MenuState);
            else
                unifromCanvasObject.SetActive(enableProceduralSnowfall);

            if (MenuState)
            {
                if (CursorTexture == null)
                {
                    CursorTexture = ResourceLoader.LoadTexture("UnifromCheat_REPO.Assets.unifrom_cursor.png");
                    CursorTexture.filterMode = FilterMode.Point; 
                    CursorTexture.wrapMode = TextureWrapMode.Clamp;
                }

                if (CursorTexture != null)
                {
                    Vector2 hotspot = new Vector2(
                        CursorTexture.width / 2f + Core.cursorImageOffsetX,
                        CursorTexture.height / 2f + Core.cursorImageOffsetY
                    );

                    Cursor.SetCursor(CursorTexture, hotspot, CursorMode.Auto);
                    enableCustomCursor = true;
                }

                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
            else
            {
                if (enableCustomCursor)
                {
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    enableCustomCursor = false;
                }

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private async void ToggleMenuWithDelay()
        {
            InitTextAnimation();
            await Task.Delay(5000);
            IsUnifromReady = true;
            await Task.Delay(250);
            ToggleMenu();
        }

        private async void InitTextAnimation()
        {
            GameObject hintGameObject = UnifromHints.Find(o => o.name == "initMenuHint");
            TextMeshProUGUI hint = hintGameObject.GetComponent<TextMeshProUGUI>();
            byte dotsQuantity = 0;

            while (!IsUnifromReady)
            {
                if (dotsQuantity == 3)
                {
                    hint.text = hint.text.Replace("...", "");
                    dotsQuantity = 0;
                }
                
                hint.text += ".";
                dotsQuantity++;
                
                await Task.Delay(250);
            }

            UnifromHints.Remove(hintGameObject);
            Object.Destroy(hintGameObject);
        }
        
        public void OnGUI()
        {
            bool shouldDrawMenu = MenuState || menuAnimationProgress > 0.001f;
            bool shouldDrawMessages = activeMessages.Count > 0;
            if (!shouldDrawMenu && !shouldDrawMessages)
                return;

            if (shouldDrawMenu && PlayerController.instance != null)
                PlayerController.instance.OverrideLookSpeed(0, 0, 0.1f);

            Matrix4x4 originalMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(dpiScaling, dpiScaling, 1f));

            if (GUIMenuSkin.menuSkin == null)
                InitSkin();

            GUI.skin = GUIMenuSkin.menuSkin;
            GUI.contentColor = Color.white;
            GUI.backgroundColor = Color.white;
            GUI.color = Color.white;

            if (!shouldDrawMenu)
            {
                DrawMessages();
                GUI.matrix = originalMatrix;
                return;
            }

            if (!isMenuInitialized)
            {
                float defaultWidth = 2560;
                float defaultHeight = 1440;
                RectMenu = new Rect(20, 20, defaultWidth, defaultHeight);
                isMenuInitialized = true;
            }

            GUI.backgroundColor = new Color(HC_R, HC_G, HC_B, HC_A);
            GUI.color = new Color(HC_R, HC_G, HC_B, HC_A);

            DrawMenuBackdrop();
            Color menuColor = GUI.color;
            Matrix4x4 menuMatrix = GUI.matrix;
            float menuEased = Mathf.SmoothStep(0f, 1f, menuAnimationProgress);
            float menuOffsetY = -14f * (1f - menuEased);
            GUI.color = new Color(menuColor.r, menuColor.g, menuColor.b, menuColor.a * menuEased);
            GUI.matrix = menuMatrix * Matrix4x4.TRS(
                new Vector3(0f, menuOffsetY, 0f),
                Quaternion.identity,
                new Vector3(1f, Mathf.Lerp(0.99f, 1f, menuEased), 1f)
            );
            RectMenu = GUILayout.Window(1, RectMenu, GUIMenuInit, string.Empty);
            GUI.matrix = menuMatrix;
            GUI.color = menuColor;

            DrawObjectSpawnerWindow();
            DrawGameControllerWindow();
            DrawMessages();

            GUI.matrix = originalMatrix;
        }

        private void DrawMenuBackdrop()
        {
            if (menuAnimationProgress <= 0f)
                return;

            Color previousColor = GUI.color;
            float alpha = Mathf.SmoothStep(0f, 1f, menuAnimationProgress) * 0.18f;
            GUI.color = new Color(0.02f, 0.025f, 0.03f, alpha);
            GUI.DrawTexture(new Rect(0f, 0f, Screen.width / dpiScaling, Screen.height / dpiScaling), Texture2D.whiteTexture);
            GUI.color = previousColor;
        }
    }
}
