using System.Collections.Generic;
using HarmonyLib;
using TMPro;
using UnifromCheat_REPO.Patches;
using UnifromCheat_REPO.Utils;
using UnifromCheat_REPO.WallHack;
using UnityEngine;

namespace UnifromCheat_REPO
{
    public class Core : MonoBehaviour
    {
        public static Core Instance;
        public Harmony harmony;
        public ItemsWallHack ItemsWallHack;
        public EnemiesWallHack EnemiesWallHack;
        public HintsController HintsController;
        public FullBrightMode FullBrightMode;
        public Noclip Noclip;
        
        public static List<GameObject> UnifromHints = new List<GameObject>();
        
        #region Params
    
        // Menu
        private Rect RectMenu;
        public float dpiScaling = Screen.dpi / getScale();
        private Vector2 scrollPosition = Vector2.zero;
        
        private byte MenuID = 1;
        internal bool MenuState = true;
        internal float MC_R = 1;
        internal float MC_G = 0.6f;
        internal float MC_B = 0.1f;
        internal float MC_A = 1;
        
        private bool advancedMenuSettings;
        private bool isColorSync = true;
        private bool isHideAllHints;
        internal static bool isGodModeEnabled;
        internal static bool isInfiniteSprint;

        internal static bool isSpeedHackEnabled;
        internal float walkSpeed = 2;
        internal float sprintSpeed = 5;
        internal float crouchSpeed = 1;

        internal static bool isCustomJumpForceEnabled;
        internal static float jumpForce = 17;

        internal static bool isWallHackEnabled = true;
        internal static bool isItemWallHackEnabled = true;
        internal static bool item_color;
        
        internal static float IC_R = 1f;
        internal static float IC_G = 0.8f;
        internal static float IC_B = 0.3f;
        internal static float IC_A = 1;
        
        internal static bool showItemName;
        internal static bool showItemPrice;
        internal static bool sortByPrice;
        internal static float sortFromPrice = 100;
        internal static float sortToPrice = 100000;
        internal static float itemTextSize = 5;

        internal static bool isEnemyWallHackEnabled = true;
        internal static bool enemy_color;
        
        internal static float EC_R = 1f;
        internal static float EC_G;
        internal static float EC_B;
        internal static float EC_A = 1f;

        internal static bool showEnemyName;
        internal static bool showEnemyHealth;
        internal static float enemyTextSize = 5;

        internal static bool isNoclipEnabled;
        internal static float noclipSpeed = 5f;
        
        internal static bool isMiscellaneousEnabled = true;
        internal static bool isFullBrightEnabled;
        internal static bool isOneShotModeEnabled;
        internal static bool isSuperStrengthEnabled;
        internal static bool isFragilityDisabled;
        internal static bool isColliderDisabledOnGrab ;

        #endregion

        #region vars

        internal string cheatVersion = "1.0.0"; 

        #endregion
        
        private static int getScale()
        {
            int width = Screen.width;
            int height = Screen.height;

            if (width <= 2000 && height <= 1300) // 1080p и ниже
                return 80;
            else if (width <= 3000 && height <= 2000) // 1440p
                return 100;
            else if (width <= 4000 && height <= 2500) // 4K
                return 160;
            else
                return 200;
        }

        private void Start()
        {
            Instance = this;
            
            ItemsWallHack = gameObject.AddComponent<ItemsWallHack>();
            EnemiesWallHack = gameObject.AddComponent<EnemiesWallHack>();
            HintsController = gameObject.AddComponent<HintsController>();
            FullBrightMode = gameObject.AddComponent<FullBrightMode>();
            Noclip = gameObject.AddComponent<Noclip>();
            
            harmony = new Harmony("ru.morkamo.unifromPatches");
            harmony.PatchAll();
            
            HintsController.CreateHint($"Unifrom {cheatVersion} - by Morkamo", "UnifromBadge", 338, 242, 8,
                new Color(MC_R, MC_G, MC_B, MC_A));
            
            UnifromHints.Add(GameObject.Find("UnifromBadge"));
            
            HintsController.CreateHint("Noclip - OFF", "NoclipText", -450, 242, 8,
                new Color(Core.Instance.MC_R, Core.Instance.MC_G, Core.Instance.MC_B, Core.Instance.MC_A));
            
            UnifromHints.Add(GameObject.Find("NoclipText"));
            
            Debug.LogWarning($"\n--------------------------\n     " +
                             $"[CHEAT-INJECTED]\n Welcome to Unifrom {cheatVersion}" +
                             $"\n--------------------------\n");
        }

        private void Update()
        {
            if (Camera.main != null) 
                Camera.main.farClipPlane = 500;
        }

        public void OnGUI()
        {
            if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.Insert || Event.current.keyCode == KeyCode.End || 
                    Event.current.keyCode == KeyCode.Home)
                {
                    MenuState = !MenuState;
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    return;
                }
            }
            
            if (MenuState)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;

                Matrix4x4 originalMatrix = GUI.matrix;

                GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(dpiScaling, dpiScaling, 1f));
                
                RectMenu.x = Mathf.Clamp(RectMenu.x, 0, Screen.width / dpiScaling - RectMenu.width);
                RectMenu.y = Mathf.Clamp(RectMenu.y, 0, Screen.height / dpiScaling - RectMenu.height);
                
                GUI.backgroundColor = new Color(MC_R, MC_G, MC_B, MC_A);
                GUI.color = new Color(MC_R, MC_G, MC_B, MC_A);

                RectMenu = GUILayout.Window(MenuID, RectMenu, GUIMenu, $"Unifrom {cheatVersion}");

                GUI.matrix = originalMatrix;
            }
        }
        
        public void GUIMenu(int id)
        {
            #region General

            float maxWidth = 155f * dpiScaling;
            float maxHeight = 300f * dpiScaling;

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(maxWidth - 20), GUILayout.Height(maxHeight - 20));

            GUILayout.BeginVertical("box");

            if (GUILayout.Button("<b>MENU SETTINGS</b>"))
            {
                advancedMenuSettings = !advancedMenuSettings;
            }

            if (advancedMenuSettings)
            {
                GUILayout.Label("Menu color:");
                GUILayout.BeginVertical("box");

                // R
                GUILayout.Label("R: " + MC_R.ToString("F1"));
                MC_R = GUILayout.HorizontalSlider(MC_R, 0, 1, GUILayout.Width(100));

                // G
                GUILayout.Label("G: " + MC_G.ToString("F1"));
                MC_G = GUILayout.HorizontalSlider(MC_G, 0, 1, GUILayout.Width(100));

                // B
                GUILayout.Label("B: " + MC_B.ToString("F1"));
                MC_B = GUILayout.HorizontalSlider(MC_B, 0, 1, GUILayout.Width(100));

                // A
                GUILayout.Label("A: " + MC_A.ToString("F1"));
                MC_A = GUILayout.HorizontalSlider(MC_A, 0, 1, GUILayout.Width(100));
                if (GUILayout.Button("<b>Default</b>"))
                {
                    MC_R = 1;
                    MC_G = 0.6f;
                    MC_B = 0.1f;
                    MC_A = 1;
                }

                GUILayout.EndVertical();
                
                /*GUILayout.Label("Scaling: " + Scaling.ToString("F0"));
                Scaling = GUILayout.HorizontalSlider(Scaling, 80, 250, GUILayout.Width(100));*/
                
                isHideAllHints = GUILayout.Toggle(isHideAllHints, "Hide all hints");
                if (isHideAllHints)
                {
                    foreach (var hint in UnifromHints)
                    {
                        hint.gameObject.SetActive(false);
                    }
                }
                else
                {
                    foreach (var hint in UnifromHints)
                    {
                        hint.gameObject.SetActive(true);
                    }
                    
                    isColorSync = GUILayout.Toggle(isColorSync, "Sync color hints");
                    if (isColorSync)
                    {
                        foreach (var hint in UnifromHints)
                            hint.GetComponent<TextMeshProUGUI>().color = new Color(MC_R, MC_G, MC_B, MC_A);
                    }
                    else
                    {
                        foreach (var hint in UnifromHints)
                            hint.GetComponent<TextMeshProUGUI>().color = Color.white;
                    }
                }
            }

            #endregion
            
            GUILayout.EndVertical();

            isGodModeEnabled = GUILayout.Toggle(isGodModeEnabled, "God mode");

            isInfiniteSprint = GUILayout.Toggle(isInfiniteSprint, "Infinite sprint");
            
            isSpeedHackEnabled = GUILayout.Toggle(isSpeedHackEnabled, "Speed hack");
            if (isSpeedHackEnabled)
            {
                GUILayout.BeginVertical("box");
                    
                GUILayout.Label($"Walk speed: " + walkSpeed.ToString("F1"));
                walkSpeed = GUILayout.HorizontalSlider(walkSpeed, 0, 30);
                if (GUILayout.Button("<b>Default</b>"))
                    walkSpeed = 2;
                
                GUILayout.Label($"Sprint speed: " + sprintSpeed.ToString("F1"));
                sprintSpeed = GUILayout.HorizontalSlider(sprintSpeed, 0, 30);
                if (GUILayout.Button("<b>Default</b>"))
                    sprintSpeed = 5;
                
                GUILayout.Label($"Crouch speed: " + crouchSpeed.ToString("F1"));
                crouchSpeed = GUILayout.HorizontalSlider(crouchSpeed, 0, 30);
                if (GUILayout.Button("<b>Default</b>"))
                    crouchSpeed = 1;
                    
                GUILayout.EndVertical();
            }

            isCustomJumpForceEnabled = GUILayout.Toggle(isCustomJumpForceEnabled, "Jump force");
            if (isCustomJumpForceEnabled)
            {
                GUILayout.BeginVertical("box");
                
                GUILayout.Label($"Jump force: " + jumpForce.ToString("F1"));
                jumpForce = GUILayout.HorizontalSlider(jumpForce, 0, 100);
                if (GUILayout.Button("<b><b>Default</b></b>"))
                    jumpForce = 17;

                GUILayout.EndVertical();
            }

            isWallHackEnabled = GUILayout.Toggle(isWallHackEnabled, "Wall hack");
            if (isWallHackEnabled)
            {
                GUILayout.BeginVertical("box");
                isItemWallHackEnabled = GUILayout.Toggle(isItemWallHackEnabled, "Show items");
                if (isItemWallHackEnabled)
                {
                    GUILayout.BeginVertical("box");
                    if (GUILayout.Button("<b>ITEMS COLOR</b>"))
                    {
                        item_color = !item_color;
                    }
                    if (item_color)
                    {
                        GUILayout.Label("R: " + IC_R.ToString("F1"));
                        IC_R = GUILayout.HorizontalSlider(IC_R, 0, 1, GUILayout.Width(100));

                        // G
                        GUILayout.Label("G: " + IC_G.ToString("F1"));
                        IC_G = GUILayout.HorizontalSlider(IC_G, 0, 1, GUILayout.Width(100));

                        // B
                        GUILayout.Label("B: " + IC_B.ToString("F1"));
                        IC_B = GUILayout.HorizontalSlider(IC_B, 0, 1, GUILayout.Width(100));

                        // A
                        GUILayout.Label("A: " + IC_A.ToString("F1"));
                        IC_A = GUILayout.HorizontalSlider(IC_A, 0, 1, GUILayout.Width(100));
                        
                        if (GUILayout.Button("<b><b>Default</b></b>"))
                        {
                            IC_R = 1;
                            IC_G = 0.8f;
                            IC_B = 0.3f;
                            IC_A = 1;
                        }
                    }
                    GUILayout.EndVertical();

                    showItemName = GUILayout.Toggle(showItemName, "Show name");
                    showItemPrice = GUILayout.Toggle(showItemPrice, "Show price");
                    sortByPrice = GUILayout.Toggle(sortByPrice, "Sort by price");
                    if (sortByPrice)
                    {
                        GUILayout.BeginVertical("box");
                        
                        GUILayout.Label($"Sort from: " + sortFromPrice.ToString("F0"));
                        sortFromPrice = GUILayout.HorizontalSlider(sortFromPrice, 0, 100000);
                        
                        GUILayout.Label($"Sort to: " + sortToPrice.ToString("F0"));
                        sortToPrice = GUILayout.HorizontalSlider(sortToPrice, 0, 100000);
                        
                        if (sortToPrice < sortFromPrice)
                        {
                            sortToPrice = sortFromPrice;
                        }
                        
                        if (GUILayout.Button("<b>Default</b>"))
                        {
                            sortFromPrice = 0;
                            sortToPrice = 100000;
                        }
                        GUILayout.EndVertical();
                    }
                    
                    GUILayout.BeginVertical("box");
                    
                    GUILayout.Label($"Text size: " + itemTextSize.ToString("F1"));
                    itemTextSize = GUILayout.HorizontalSlider(itemTextSize, 0, 10);
                    if (GUILayout.Button("<b>Default</b>"))
                        itemTextSize = 5;
                    
                    GUILayout.EndVertical();
                }

                isEnemyWallHackEnabled = GUILayout.Toggle(isEnemyWallHackEnabled, "Show enemies");
                if (isEnemyWallHackEnabled)
                {
                    GUILayout.BeginVertical("box");
                    showEnemyName = GUILayout.Toggle(showEnemyName, "Show name");
                    showEnemyHealth = GUILayout.Toggle(showEnemyHealth, "Show health");
                    
                    GUILayout.BeginVertical("box");
                    if (GUILayout.Button("<b>ENEMIES COLOR</b>"))
                    {
                        enemy_color = !enemy_color;
                    }
                    if (enemy_color)
                    {
                        GUILayout.Label("R: " + EC_R.ToString("F1"));
                        EC_R = GUILayout.HorizontalSlider(EC_R, 0, 1, GUILayout.Width(100));

                        // G
                        GUILayout.Label("G: " + EC_G.ToString("F1"));
                        EC_G = GUILayout.HorizontalSlider(EC_G, 0, 1, GUILayout.Width(100));

                        // B
                        GUILayout.Label("B: " + EC_B.ToString("F1"));
                        EC_B = GUILayout.HorizontalSlider(EC_B, 0, 1, GUILayout.Width(100));

                        // A
                        GUILayout.Label("A: " + EC_A.ToString("F1"));
                        EC_A = GUILayout.HorizontalSlider(EC_A, 0, 1, GUILayout.Width(100));
                        
                        if (GUILayout.Button("<b>Default</b>"))
                        {
                            EC_R = 1;
                            EC_G = 0f;
                            EC_B = 0f;
                            EC_A = 1;
                        }
                    }
                    GUILayout.EndVertical();
                    
                    GUILayout.Label("Text size: " + enemyTextSize.ToString("F1"));
                    enemyTextSize = GUILayout.HorizontalSlider(enemyTextSize, 0, 10, GUILayout.Width(100));
                    if (GUILayout.Button("<b>Default</b>"))
                    {
                        enemyTextSize = 5;
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndVertical();
            }

            isNoclipEnabled = GUILayout.Toggle(isNoclipEnabled, "Noclip (Lite)");
            if (isNoclipEnabled)
            {
                GUILayout.BeginVertical("box");
                GUILayout.Label("Bind: [<b>ALT</b>]");
                
                GUILayout.BeginVertical("box");
                GUILayout.Label($"Noclip speed: " + noclipSpeed.ToString("F1"));
                noclipSpeed = GUILayout.HorizontalSlider(noclipSpeed, 1, 20);
                if (GUILayout.Button("<b>Default</b>"))
                    noclipSpeed = 5;
                GUILayout.EndVertical();
                
                GUILayout.EndVertical();
            }

            isMiscellaneousEnabled = GUILayout.Toggle(isMiscellaneousEnabled, "Misc");
            if (isMiscellaneousEnabled)
            {
                GUILayout.BeginVertical("box");

                isOneShotModeEnabled = GUILayout.Toggle(isOneShotModeEnabled, "One tap kill mode \n<b>(host only & for all players)\n(body only)</b>");
                if (isOneShotModeEnabled)
                {
                    GUILayout.BeginVertical("box");
                    GUILayout.Label("Only work if you use player body");
                    GUILayout.EndVertical();
                }
                
                isSuperStrengthEnabled = GUILayout.Toggle(isSuperStrengthEnabled, "Super strength \n<b>(host only & for all players)</b>");
                isFragilityDisabled = GUILayout.Toggle(isFragilityDisabled, "Disable item fragility \n<b>(host only & for all players)</b>");
                isColliderDisabledOnGrab = GUILayout.Toggle(isColliderDisabledOnGrab, "Disable item collider on grab \n<b>(host only & for all players)</b>");
                
                if (GUILayout.Button("<b>FULL BRIGHT</b>\n<size=10%>Not work correctly when rolling and crouching</size>"))
                {
                    isFullBrightEnabled = !isFullBrightEnabled;
                    PlayerController.instance.SetCrawl();
                }
                
                if (GUILayout.Button("<b>DESPAWN ENEMIES</b>\n<size=10%>Host only</size>"))
                {
                    foreach (var enemy in EnemyDirector.instance.enemiesSpawned)
                        enemy.Despawn();
                }
                
                GUILayout.EndVertical();
            }
            
            GUILayout.EndScrollView();
            GUI.DragWindow();
        }
    }
}