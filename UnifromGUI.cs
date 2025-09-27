using TMPro;
using UnifromCheat_REPO.Utils;
using UnityEngine;
using static UnifromCheat_REPO.GUIMenuSkin;
using static UnifromCheat_REPO.TooltipsLanguages;

namespace UnifromCheat_REPO;

public partial class Core
{
    private void GUIMenuInit(int id)
    {
        float baseWidth = 1920;
        float baseHeight = 1080;
        float maxWidth = baseWidth;
        float maxHeight = baseHeight;

        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(maxWidth - 20), GUILayout.Height(maxHeight - 20));

        GUILayout.BeginHorizontal();

        MenuSettingsTab();
        PlayerSettingsTab();
        WallHackSettingsTab();
        MiscSettingsTab();
        HostOnlyFunctions();
        ConfigSettingsTab();

        GUILayout.EndHorizontal();
        GUILayout.EndScrollView();
        
        if (dragWindow) GUI.DragWindow();
        
        Rect hintRect = new Rect((baseWidth / 2f) - 200, baseHeight - 90, 300, 50);
        GUILayout.BeginArea(hintRect, GUIMenuSkin.windowStyle);
        
        GUIStyle centeredLabel = new GUIStyle(GUIMenuSkin.labelStyle)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 12, 
            wordWrap = true
        };

        GUILayout.Label($"<b>{Get("PressLabelInfo")}\n</b>" +
                        $"<b>INSERT - {Get("insertInfo")}</b>\n",
            centeredLabel);

        GUILayout.EndArea();
        GUITooltip.Draw();
    }
    
    private void MenuSettingsTab()
    {
        GUILayout.BeginVertical(windowStyle, GUILayout.Width(320));
        
        if (GUILayout.Button("<b>MENU SETTINGS</b>", buttonStyle)) 
            advancedMenuSettings = !advancedMenuSettings;

        if (advancedMenuSettings)
        {
            GUILayout.BeginVertical(windowStyle);
            
            GUILayout.Label("Hints color:", labelStyle);
            DrawSlider("R", ref HC_R, 0f, 1f, 1f);
            DrawSlider("G", ref HC_G, 0f, 1f, 1f);
            DrawSlider("B", ref HC_B, 0f, 1f, 1f);
            DrawSlider("A", ref HC_A, 0f, 1f, 0.7f);
            
            /*GUILayout.BeginVertical(windowStyle);
            
            DrawSlider("Menu opacity", ref menuOpacity, 0.1f, 1f, 0.7f, Get("MenuOpacity"));

            GUILayout.EndVertical();
            
            GUILayout.BeginVertical(windowStyle);
            
            DrawLabel("Hints color:", Color.white, Get("hintsColor"));
            DrawSlider("R", ref MC_R, 0f, 1f, 0.18f);
            DrawSlider("G", ref MC_G, 0f, 1f, 0.18f);
            DrawSlider("B", ref MC_B, 0f, 1f, 0.18f);
            
            GUILayout.EndVertical();*/

            DrawToggle("Hide all hints", ref HideAllHints, Color.green, Get("hideAllHints"));

            if (!HideAllHints)
            {
                foreach (var hint in UnifromHints)
                {
                    hint.GetComponent<TextMeshProUGUI>().color = new Color(HC_R, HC_G, HC_B, HC_A);
                    hint.SetActive(true);
                }
            }
            else
            {
                foreach (var hint in UnifromHints)
                    hint.gameObject.SetActive(false);
            }
            
            DrawToggle("Hide all tooltips", ref HideAllTooltips, Color.green, Get("hideAllTooltips"));
            DrawToggle("Drag window", ref dragWindow, Color.green, Get("dragWindow"));
            
            GUILayout.Space(5);
            
            if (GUILayout.Button("<b>TOOLTIPS LANGUAGE</b>", buttonStyle)) 
                tooltipsLanguage = !tooltipsLanguage;

            if (tooltipsLanguage)
            {
                GUILayout.BeginVertical(windowStyle);
                DrawRadio("English", 0, ref lg_state, Color.white);
                DrawRadio("Russian", 1, ref lg_state, Color.white);
                DrawRadio("Ukrainian", 2, ref lg_state, Color.white);
                DrawRadio("Chinese", 3, ref lg_state, Color.white);
                GUILayout.Space(5);
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndVertical();
    }

    private void PlayerSettingsTab()
    {
        GUILayout.BeginVertical(windowStyle, GUILayout.Width(150));
        if (GUILayout.Button("<b>PLAYER</b>", buttonStyle)) PlayerTab = !PlayerTab;

        if (PlayerTab)
        {
            GUILayout.BeginVertical(windowStyle);
            
            DrawToggle("God mode", ref isGodModeEnabled, Color.green, Get("godMode"));
            DrawToggle("Infinite sprint", ref isInfiniteSprint, Color.green, Get("infSprint"));
            DrawToggle("Speed hack", ref isSpeedHackEnabled, Color.green, Get("speedHack"));

            if (isSpeedHackEnabled)
            {
                GUILayout.Space(10);
                GUILayout.BeginVertical(windowStyle);
                DrawSlider("Walk speed", ref walkSpeed, 0, 30, 2, Get("walkSpeedHack"));
                DrawSlider("Sprint speed", ref sprintSpeed, 0, 30, 5, Get("sprintSpeedHack"));
                DrawSlider("Crouch speed", ref crouchSpeed, 0, 30, 1, Get("crouchSpeedHack"));
                GUILayout.EndVertical();
            }

            DrawToggle("Jump force", ref isCustomJumpForceEnabled, Color.green, Get("jumpForce"));
            if (isCustomJumpForceEnabled)
            {
                GUILayout.Space(10);
                GUILayout.BeginVertical(windowStyle);
                DrawSlider("Jump force", ref jumpForce, 0, 100, 17);
                GUILayout.EndVertical();
            }

            DrawToggle("Custom FOV", ref isCustomFovEnabled, Color.green, Get("customfov"));
            if (isCustomFovEnabled)
            {
                GUILayout.Space(5);
                GUILayout.BeginVertical(windowStyle);
                DrawSlider("FOV", ref fovValue, 1, 360, 80);
                GUILayout.EndVertical();
            }
            
            bool old = isRGBPlayerEnabled;
            
            DrawToggle("RGB Player color", ref isRGBPlayerEnabled, Color.green, Get("rgbPlayer"));
            if (old != isRGBPlayerEnabled)
            {
                if (isRGBPlayerEnabled)
                    RGBPlayer.StartCycle();
                else
                    RGBPlayer.StopCycle();
            }
            if (isRGBPlayerEnabled)
            {
                GUILayout.Space(5);
                GUILayout.BeginVertical(windowStyle);

                float timing = RGBupdateInterval;
                DrawSlider("Update interval", ref timing, 1, 10000, 100, Get("rgbPlayerUpdateInterval"));
                RGBupdateInterval = (ushort)timing;

                GUILayout.EndVertical();
            }
            
            
            DrawToggle("Flashlight settings", ref isFlashlightSettingsEnabled, Color.green, Get("flashlightSettings"));
            var flashlight = PlayerController.instance.playerAvatarScript.flashlightController.spotlight;
            if (isFlashlightSettingsEnabled)
            {
                GUILayout.Space(5);
                GUILayout.BeginVertical(windowStyle);

                DrawToggle("Shadows", ref isFlashlightShadowsEnabled, Color.green);
                flashlight.shadows = isFlashlightShadowsEnabled ? LightShadows.Hard : LightShadows.None;
                GUILayout.Space(5);

                DrawSlider("Range", ref flashlightSpotAngle, 0, 360, 60);
                flashlight.spotAngle = flashlightSpotAngle;

                DrawSlider("Intensity", ref flashlightRange, 0, 100, 25);
                flashlight.range = flashlightRange;

                GUILayout.BeginVertical(windowStyle);

                DrawSlider("R", ref FLC_R, 0f, 1f, 1f);
                DrawSlider("G", ref FLC_G, 0f, 1f, 0.674f);
                DrawSlider("B", ref FLC_B, 0f, 1f, 0.382f);

                flashlight.color = new Color(FLC_R, FLC_G, FLC_B);
                GUILayout.EndVertical();
                
                GUILayout.EndVertical();
            }
            else if (prevFlashlightState)
            {
                flashlight.shadows = LightShadows.Hard;
                flashlight.spotAngle = 60f;
                flashlight.range = 25f;
                flashlight.color = new Color(1f, 0.674f, 0.382f, 1f);
            }
            
            prevFlashlightState = isFlashlightSettingsEnabled;
            
            GUILayout.Space(5);
            GUILayout.EndVertical();
        }
        
        GUILayout.EndVertical();
    }

    private void WallHackSettingsTab()
    {
        GUILayout.BeginVertical(windowStyle, GUILayout.Width(250));
        if (GUILayout.Button("<b>WALLHACK</b>", buttonStyle)) WallHackTab = !WallHackTab;

        if (WallHackTab)
        {
            GUILayout.BeginVertical(windowStyle);
            
            DrawToggle("Show items", ref isItemWallHackEnabled, Color.green, Get("showItemsWH"));
            if (isItemWallHackEnabled)
            {
                GUILayout.Space(10);
                
                if (GUILayout.Button("<b>ITEMS COLOR</b>", buttonStyle)) item_color = !item_color;
                if (item_color)
                {
                    GUILayout.BeginVertical(windowStyle);
                    
                    DrawSlider("R", ref IC_R, 0f, 1f, 1f);
                    DrawSlider("G", ref IC_G, 0f, 1f, 0.8f);
                    DrawSlider("B", ref IC_B, 0f, 1f, 0.3f);
                    DrawSlider("A", ref IC_A, 0f, 1f, 1f);
                    
                    GUILayout.EndVertical();
                }
                
                DrawToggle("Show money bags", ref showSurplusValuable, Color.green, Get("showMoneyBagsWH"));
                GUILayout.Space(10);
                
                if (GUILayout.Button("<b>MONEY BAGS COLOR</b>", buttonStyle)) moneyBagsColor = !moneyBagsColor;
                if (moneyBagsColor)
                {
                    GUILayout.BeginVertical(windowStyle);
                    
                    DrawSlider("R", ref SPC_R, 0f, 1f, 1f);
                    DrawSlider("G", ref SPC_G, 0f, 1f, 0f);
                    DrawSlider("B", ref SPC_B, 0f, 1f, 1f);
                    DrawSlider("A", ref SPC_A, 0f, 1f, 0.8f);
                    
                    GUILayout.EndVertical();
                }
                
                bool newValue = showExtractionPoints;
                
                DrawToggle("Show extraction points", ref newValue, Color.green, Get("showExPointsWH"));
                
                if (newValue != showExtractionPoints)
                    showExtractionPoints = newValue;

                if (showExtractionPoints)
                {
                    GUILayout.Space(5);
                    GUILayout.BeginVertical(windowStyle);
                    
                    DrawSlider("R", ref EPC_R, 0f, 1f, 1f);
                    DrawSlider("G", ref EPC_G, 0f, 1f, 1f);
                    DrawSlider("B", ref EPC_B, 0f, 1f, 1f);
                    DrawSlider("A", ref EPC_A, 0f, 1f, 0.1f);
                    
                    GUILayout.EndVertical();
                }

                DrawToggle("Show name", ref showItemName, Color.green, Get("showItemNameWH"));
                DrawToggle("Show price", ref showItemPrice, Color.green, Get("showItemPriceWH"));
                DrawToggle("Sort by price", ref sortByPrice, Color.green, Get("sortByPriceWH"));
                
                GUILayout.Space(5);
                GUILayout.BeginVertical(windowStyle);
                
                if (sortByPrice)
                {
                    DrawSlider("Sort from", ref sortFromPrice, 0, 100000, 0);
                    DrawSlider("Sort to", ref sortToPrice, 0, 100000, 100000);
                    if (sortToPrice < sortFromPrice) sortToPrice = sortFromPrice;
                }

                GUILayout.Space(5);
                DrawSlider("Text size", ref itemTextSize, 0, 10, 3);
                GUILayout.EndVertical();
            }

            DrawToggle("Show enemies", ref isEnemyWallHackEnabled, Color.green, Get("showEnemiesWH"));
            if (isEnemyWallHackEnabled)
            {
                DrawToggle("Show name", ref showEnemyName, Color.green, Get("showEnemyNameWH"));
                DrawToggle("Show health", ref showEnemyHealth, Color.green, Get("showEnemyHealthWH"));
                
                GUILayout.Space(10);

                if (GUILayout.Button("<b>ENEMIES COLOR</b>", buttonStyle)) enemy_color = !enemy_color;
                GUILayout.BeginVertical(windowStyle);
                
                if (enemy_color)
                {
                    DrawSlider("R", ref EC_R, 0f, 1f, 1f);
                    DrawSlider("G", ref EC_G, 0f, 1f, 0f);
                    DrawSlider("B", ref EC_B, 0f, 1f, 0f);
                    DrawSlider("A", ref EC_A, 0f, 1f, 0.8f);
                }
                
                GUILayout.Space(5);
                DrawSlider("Text size", ref enemyTextSize, 0, 10, 3);
                GUILayout.EndVertical();
            }
            
            DrawToggle("Show player", ref isPlayerWallHackEnabled, Color.green, Get("showPlayersWH"));
            if (isPlayerWallHackEnabled)
            {
                DrawToggle("Show name", ref showPlayerName, Color.green, Get("showPlayerNameWH"));
                DrawToggle("Show health", ref showPlayerHealth, Color.green, Get("showPlayerHealthWH"));
                DrawToggle("Show glow", ref isShowPlayerGlow, Color.green, Get("showPlayerGlowWH"));

                GUILayout.Space(5);

                if (GUILayout.Button("<b>PLAYERS COLOR</b>", buttonStyle)) player_glow_color = !player_glow_color;

                if (player_glow_color)
                {
                    GUILayout.BeginVertical(windowStyle);
                    DrawSlider("R", ref PC_R, 0f, 1f, 0f);
                    DrawSlider("G", ref PC_G, 0f, 1f, 1f);
                    DrawSlider("B", ref PC_B, 0f, 1f, 1f);
                    DrawSlider("A", ref PC_A, 0f, 1f, 0.8f);
                    GUILayout.EndVertical();
                }

                DrawToggle("Show dead heads", ref isShowPlayerDeadHead, Color.green, Get("showDeadHeadsWH"));
                
                GUILayout.Space(5);
                
                if (GUILayout.Button("<b>DEAD HEADS COLOR</b>", buttonStyle)) player_deadHead_color = !player_deadHead_color;

                if (player_deadHead_color)
                {
                    GUILayout.BeginVertical(windowStyle);
                    GUILayout.Space(5);
                    
                    DrawSlider("R", ref PCDH_R, 0f, 1f, 1f);
                    DrawSlider("G", ref PCDH_G, 0f, 1f, 0f);
                    DrawSlider("B", ref PCDH_B, 0f, 1f, 0.5f);
                    DrawSlider("A", ref PCDH_A, 0f, 1f, 0.8f);
                    
                    GUILayout.EndVertical();
                }
            }
            
            GUILayout.BeginVertical(windowStyle);
            GUILayout.Space(5);
            
            DrawSlider("Text size", ref playerTextSize, 0, 10, 3);
            
            GUILayout.EndVertical();
            GUILayout.EndVertical();
        }
        GUILayout.EndVertical();
    }

    private void MiscSettingsTab()
    {
        GUILayout.BeginVertical(windowStyle, GUILayout.Width(300));
        if (GUILayout.Button("<b>MISC</b>", buttonStyle)) MiscTab = !MiscTab;
        
        if (MiscTab)
        {
            GUILayout.BeginVertical(windowStyle);
            
            DrawToggle("Noclip", ref isNoclipEnabled, Color.green, Get("noclip"));
            if (isNoclipEnabled)
            {
                GUILayout.Space(5);
                GUILayout.BeginVertical(windowStyle);
                GUILayout.Label("Bind: [<b>ALT</b>]", labelStyle);
                DrawSlider("Noclip speed", ref noclipSpeed, 1, 20, 5, Get("noclipSpeed"));
                GUILayout.EndVertical();
            }

            /*DrawToggle("Damage multiplier <color=yellow>(Not work)</color>", ref DamageMultiplier, Color.green);
            if (DamageMultiplier)
            {
                GUILayout.Space(5);
                GUILayout.BeginVertical(windowStyle);
                DrawSlider("Value", ref dMValue, 1, 100, 1);
                GUILayout.EndVertical();
            }

            DrawToggle("Anti tumble <color=yellow>(Not work)</color>", ref isAntiTumbleEnabled, Color.green);*/
            
            DrawToggle("Fullbright \b <color=yellow>\n[Test! Sometimes need crouch for disable]</color>", ref _isFullbrightEnabled, Color.green, Get("fullbright"));
            if (isFullbrightEnabled)
            {
                GUILayout.Space(5);
                GUILayout.BeginVertical(windowStyle);
                DrawSlider("R", ref FB_R, 0f, 1f, 1f);
                DrawSlider("G", ref FB_G, 0f, 1f, 1f);
                DrawSlider("B", ref FB_B, 0f, 1f, 1f);
                GUILayout.EndVertical();
            }
            
            DrawToggle("Multi-jumps", ref multiJumps, Color.green, Get("multiJumps"));

            GUILayout.Space(5);
            GUILayout.EndVertical();
        }
        GUILayout.EndVertical();
    }

    private void HostOnlyFunctions()
    {
        GUILayout.BeginVertical(windowStyle, GUILayout.Width(235));
        
        if (GUILayout.Button("<b>HOST ONLY FUNCTIONS</b>", buttonStyle)) 
            HostFunctsTab = !HostFunctsTab;
        
        if (HostFunctsTab)
        {
            GUILayout.BeginVertical(windowStyle);
            
            DrawToggle("Valuables teleporter", ref valuablesTeleporter, Color.green, Get("valuablesTp"));
            if (valuablesTeleporter)
            {
                GUILayout.Space(5);
                GUILayout.BeginVertical(windowStyle);
                if (GUILayout.Button("Teleport", buttonStyle))
                {
                    MiscFunctions.Instance.TeleportValuables(vt_state, vtm_one_any, vtm_kinematic);
                }
                
                GUILayout.EndVertical();

                GUILayout.BeginVertical(windowStyle);
                DrawLabel("Modes: ", Color.white);
                
                DrawToggle("Kinematic", ref vtm_kinematic, Color.yellow, Get("valuablesKinematic"));
                if (vtm_kinematic)
                {
                    GUILayout.Space(5);
                    GUILayout.BeginVertical(windowStyle);
                    DrawToggle("Permanent freeze", ref vtm_permanentFreeze, Color.green, Get("valuablesFreeze"));
                    GUILayout.Space(5);
                    if (!vtm_permanentFreeze)
                    {
                        DrawSlider("Disable interval", ref vtm_kinematicDisableInterval, 0f, 20f, 3f);
                        GUILayout.Space(5);
                    }
                    else
                    {
                        DrawToggle("Disable on touch", ref vtm_disableKinematicOnTouch, Color.green, Get("valuablesDisableTouch"));
                        GUILayout.Space(5);
                    }
                    
                    GUILayout.EndVertical();
                }
                
                DrawToggle("One any", ref vtm_one_any, Color.yellow, Get("valuablesOneAny"));
                DrawToggle("Teleport on touch", ref vtm_teleportOnTouch, Color.yellow, Get("valuablesTpOnTouch"));

                GUILayout.Space(5);
                GUILayout.EndVertical();

                GUILayout.BeginVertical(windowStyle);
                DrawRadio("To player", 0, ref vt_state, Color.cyan);
                DrawRadio("Into extract point", 1, ref vt_state, Color.cyan);
                DrawRadio("Into nearest cart", 2, ref vt_state, Color.cyan);
                GUILayout.Space(5);
                GUILayout.EndVertical();
            }
            
            DrawToggle("Enemies teleporter", ref enemiesTeleporter, Color.green, Get("enemiesTp"));
            if (enemiesTeleporter)
            {
                GUILayout.Space(5);
                GUILayout.BeginVertical(windowStyle);
                if (GUILayout.Button("Teleport", buttonStyle))
                {
                    MiscFunctions.Instance.TeleportEnemies(em_state, em_one_any, em_kinematic);
                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical(windowStyle);
                DrawLabel("Modes: ", Color.white);

                DrawToggle("Kinematic", ref em_kinematic, Color.yellow, Get("enemiesKinematic"));
                if (em_kinematic)
                {
                    GUILayout.Space(5);
                    GUILayout.BeginVertical(windowStyle);
                    DrawToggle("Permanent freeze movement", ref em_permanentFreeze, Color.green, Get("enemiesFreeze"));
                    GUILayout.Space(5);
                    GUILayout.EndVertical();
                }

                DrawToggle("One any", ref em_one_any, Color.yellow, Get("enemiesOneAny"));
                DrawToggle("Teleport on touch", ref em_teleportOnTouch, Color.yellow, Get("enemiesTpOnTouch"));
                GUILayout.Space(5);
                GUILayout.EndVertical();

                GUILayout.BeginVertical(windowStyle);
                DrawRadio("To player", 0, ref em_state, Color.cyan);
                DrawRadio("Into extract point", 1, ref em_state, Color.cyan);
                DrawRadio("Into void", 2, ref em_state, Color.cyan);
                GUILayout.Space(5);
                GUILayout.EndVertical();
            }
            
            /*DrawToggle("Players teleporter", ref playersTeleporter, Color.green);
            if (playersTeleporter)
            {
                GUILayout.Space(5);
                GUILayout.BeginVertical(windowStyle);
                
                if (GUILayout.Button("<b>GO TO ANY</b>", buttonStyle)) PlayersTeleporter.GoToAny(); 
                if (GUILayout.Button("<b>ANY TO YOU</b>", buttonStyle)) PlayersTeleporter.AnyToYou();
                if (GUILayout.Button("<b>ALL TO YOU</b>", buttonStyle)) PlayersTeleporter.AllToYou(); 

                GUILayout.Space(5);
                GUILayout.EndVertical();
            }*/
            
            DrawToggle("No fragility", ref isFragilityDisabled, Color.green, Get("fragilityOff"));
            DrawToggle("Ghost items", ref isGhostItemsMode, Color.green, Get("ghostItems"));
            DrawToggle("Lite items", ref isLiteItemsModeEnabled, Color.green, Get("liteItems"));
            DrawToggle("One-shot kills", ref isOneShotModeEnabled, Color.green, Get("oneShotKill"));

            GUILayout.Space(5);

            if (GUILayout.Button("<b>KILL ENEMIES</b>", buttonStyle))
            {
                foreach (var enemy in EnemyDirector.instance.enemiesSpawned)
                {
                    var hp = enemy.GetComponentInChildren<EnemyHealth>();
                    
                    if (hp == null) 
                        continue;

                    hp.Hurt(int.MaxValue, Vector3.up);
                }
            }

            /*if (GUILayout.Button("<b>SOULS SPAWNER</b>", buttonStyle))
                soulsSpawner = !soulsSpawner;

            if (soulsSpawner)
            {
                GUILayout.Space(5);
                GUILayout.BeginVertical(windowStyle);
            
                DrawSliderInt("Quantity", ref soulsNumber, 1, 10, 1);
                DrawSliderInt("Price", ref priceOneSoul, 1, 100000, 5000);
                
                if (GUILayout.Button("<b>SPAWN</b>", buttonStyle)) { }
            
                GUILayout.EndVertical();
            }*/

            GUILayout.EndVertical();
        }
        
        GUILayout.EndVertical();
    }
    
    private void ConfigSettingsTab()
    {
        GUILayout.BeginVertical(windowStyle, GUILayout.Width(150));
        
        if (GUILayout.Button("Configs", buttonStyle)) 
            ConfigTab = !ConfigTab;
        
        if (ConfigTab)
        {
            GUILayout.BeginVertical(windowStyle);
            
            if (GUILayout.Button("Save config", buttonStyle))
                SaveConfig();
            
            if (GUILayout.Button("Load config", buttonStyle))
                LoadConfig();

            if (GUILayout.Button("Reset config", buttonStyle))
                ResetConfig();
            
            GUILayout.EndVertical();
        }
        GUILayout.EndVertical();
    }
}