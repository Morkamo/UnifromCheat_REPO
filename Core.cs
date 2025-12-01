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
    /*[BepInPlugin("ru.morkamo.unifrom", "Unifrom", "3.1.2")]*/
    public partial class Core : MonoBehaviour
    {
        public static Core Instance;
        public Harmony harmony;
        public ItemsWallHack ItemsWallHack;
        public EnemiesWallHack EnemiesWallHack;
        public PlayersWallHack PlayersWallHack;
        public HintsController HintsController;
        public Noclip Noclip;
        public MiscFunctions MiscFunctions;
        public static GameObject unifromCanvasObject;

        public static List<GameObject> UnifromHints = new List<GameObject>();

        private void Start()
        {
            Instance = this;
            
            ItemsWallHack = gameObject.AddComponent<ItemsWallHack>();
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
            if (Camera.main != null) Camera.main.farClipPlane = 10000;

            if (Keyboard.current.insertKey.wasPressedThisFrame || Keyboard.current.rightAltKey.wasPressedThisFrame
                || Keyboard.current.f11Key.wasPressedThisFrame)
                ToggleMenu();
        }
        
        internal void ToggleMenu()
        {
            if (!IsUnifromReady)
                return;
            
            MenuState = !MenuState;

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
            if (!MenuState)
                return;
            
            PlayerController.instance.OverrideLookSpeed(0, 0, 0.1f);

            Matrix4x4 originalMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(dpiScaling, dpiScaling, 1f));

            if (!isMenuInitialized)
            {
                float defaultWidth = 2560;
                float defaultHeight = 1440;
                RectMenu = new Rect(20, 20, defaultWidth, defaultHeight);
                isMenuInitialized = true;
            }

            GUI.backgroundColor = new Color(HC_R, HC_G, HC_B, HC_A);
            GUI.color = new Color(HC_R, HC_G, HC_B, HC_A);

            if (GUIMenuSkin.menuSkin == null)
                InitSkin();

            GUI.skin = GUIMenuSkin.menuSkin;
            GUI.contentColor = Color.white;
            GUI.backgroundColor = Color.white;
            GUI.color = Color.white;

            RectMenu = GUILayout.Window(1, RectMenu, GUIMenuInit, string.Empty);

            GUI.matrix = originalMatrix;
        }
    }
}