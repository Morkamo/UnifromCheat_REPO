using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Unity.Mono;
using HarmonyLib;
using UnifromCheat_REPO.Patches;
using UnifromCheat_REPO.Utils;
using UnifromCheat_REPO.WallHack;
using UnityEngine;
using static UnifromCheat_REPO.GUIMenuSkin;
using static UnifromCheat_REPO.Utils.FireboxConsole;

namespace UnifromCheat_REPO
{
    /*[BepInPlugin("ru.morkamo.unifrom", "Unifrom", "3.0.0")]*/
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
            
            UnifromHints.Add(GameObject.Find("NoclipText"));
            
            FireLog($"--------------------------\n     " +
                    $"[CHEAT-INJECTED]\n Welcome to Unifrom {cheatVersion}" +
                    $"\n--------------------------\n");
        }

        private void Update()
        {
            if (Camera.main != null) Camera.main.farClipPlane = 10000;
            
            /*if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.End))
                Loader.Unload();*/
        }

        public void OnGUI()
        {
            if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Insert))
            {
                MenuState = !MenuState;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = true;

                return;
            }

            if (!MenuState) 
                return;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
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

            RectMenu = GUILayout.Window(1, RectMenu, GUIMenuInit, String.Empty);
            GUI.matrix = originalMatrix;
        }
    }
}