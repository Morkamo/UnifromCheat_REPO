using System;
using System.IO;
using TMPro;
using UnifromCheat_REPO.Funs;
using UnifromCheat_REPO.Utils;
using UnifromCheat_REPO.WallHack;
using UnityEngine;
using static UnifromCheat_REPO.GUIMenuSkin;
using static UnifromCheat_REPO.TooltipsLanguages;
using static UnifromCheat_REPO.Utils.FireboxConsole;

namespace UnifromCheat_REPO;

public partial class Core
{
    private void DrawHostOnlyToggle(string label, ref bool value, Color? onColor = null, string tooltip = null, string animationKey = null)
    {
        bool before = value;
        Color activeColor = value && HostOnlyGuard.ShouldShowUnavailableStatus()
            ? new Color(1f, 0.22f, 0.18f, 1f)
            : onColor ?? Color.green;

        DrawToggle(label, ref value, activeColor, tooltip, animationKey);
        if (value && !before && HostOnlyGuard.ShouldShowUnavailableStatus())
            HostOnlyGuard.CanUseHostOnly(true, label);
    }

    private void GUIMenuInit(int id)
    {
        float baseWidth = 1920;
        float baseHeight = 1080;
        float maxWidth = baseWidth;
        float maxHeight = baseHeight;

        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(maxWidth - 20), GUILayout.Height(maxHeight - 20));

        GUILayout.BeginHorizontal();

        DrawAnimatedMenuTile(0, MenuSettingsTab);
        DrawAnimatedMenuTile(1, PlayerSettingsTab);
        DrawAnimatedMenuTile(2, WallHackSettingsTab);
        DrawAnimatedMenuTile(3, MiscSettingsTab);
        DrawAnimatedMenuTile(4, HostOnlyFunctions);
        DrawAnimatedMenuTile(5, ConfigSettingsTab);

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
                        $"<b>INSERT/R-ALT/F11 - {Get("insertInfo")}</b>\n",
            centeredLabel);

        GUILayout.EndArea();
        GUITooltip.Draw();
    }

    private void DrawAnimatedMenuTile(int index, Action drawTile)
    {
        float progress = GetTileAnimationProgress(index);
        Color previousColor = GUI.color;
        Matrix4x4 previousMatrix = GUI.matrix;

        float offsetY = -30f * (1f - progress);
        float scale = Mathf.Lerp(0.985f, 1f, progress);
        GUI.color = new Color(previousColor.r, previousColor.g, previousColor.b, previousColor.a * progress);
        GUI.matrix = previousMatrix * Matrix4x4.TRS(new Vector3(0f, offsetY, 0f), Quaternion.identity, new Vector3(1f, scale, 1f));

        drawTile();
        Rect tileRect = GUILayoutUtility.GetLastRect();
        DrawAnimatedSectionFrame(tileRect, Time.unscaledTime, progress, index);

        GUI.matrix = previousMatrix;
        GUI.color = previousColor;
    }

    private float GetTileAnimationProgress(int index)
    {
        float stagger = index * 0.075f;
        float duration = 1f - (MenuAnimatedTileCount - 1) * 0.075f;
        float normalized = Mathf.Clamp01((menuAnimationProgress - stagger) / duration);
        return normalized * normalized * (3f - 2f * normalized);
    }

    private bool BeginAnimatedFoldout(string key, bool expanded, float slideOffset = -18f)
    {
        if (!foldoutAnimations.TryGetValue(key, out float progress))
            progress = 0f;

        if (Event.current.type == EventType.Layout)
        {
            float target = expanded ? 1f : 0f;
            progress = Mathf.MoveTowards(progress, target, Time.unscaledDeltaTime * 12f);
            foldoutAnimations[key] = progress;
        }

        if (!expanded && progress <= 0.001f)
            return false;

        float eased = progress * progress * (3f - 2f * progress);
        Matrix4x4 previousMatrix = GUI.matrix;
        Color previousColor = GUI.color;
        foldoutMatrices.Push(previousMatrix);
        foldoutColors.Push(previousColor);
        foldoutEnabledStates.Push(GUI.enabled);

        float offsetY = slideOffset * (1f - eased);
        GUI.matrix = previousMatrix * Matrix4x4.TRS(new Vector3(0f, offsetY, 0f), Quaternion.identity, Vector3.one);
        GUI.color = new Color(previousColor.r, previousColor.g, previousColor.b, previousColor.a * eased);
        if (!expanded)
            GUI.enabled = false;
        return true;
    }

    private void EndAnimatedFoldout()
    {
        if (foldoutMatrices.Count > 0)
            GUI.matrix = foldoutMatrices.Pop();
        if (foldoutColors.Count > 0)
            GUI.color = foldoutColors.Pop();
        if (foldoutEnabledStates.Count > 0)
            GUI.enabled = foldoutEnabledStates.Pop();
    }
    
    private void MenuSettingsTab()
    {
        GUILayout.BeginVertical(windowStyle, GUILayout.Width(320));
        
        if (GUILayout.Button("<b>MENU SETTINGS</b>", buttonStyle)) 
            advancedMenuSettings = !advancedMenuSettings;

        if (BeginAnimatedFoldout("menu.settings", advancedMenuSettings))
        {
            GUILayout.BeginVertical(windowStyle);
            
            DrawLabel("Hints color:", Color.white);
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
            
            if (GUILayout.Button("<b>CUSTOM CURSOR</b>", buttonStyle)) customCursor = !customCursor;
            if (BeginAnimatedFoldout("menu.settings.cursor", customCursor))
            {
                GUILayout.BeginVertical(windowStyle);
                
                GUILayout.Space(5);
                GUILayout.BeginVertical(windowStyle);
                DrawLabel(Get("getCursorOffsetLabel"), Color.grey);
                DrawSlider("Cursor X offset", ref cursorImageOffsetX, -64, 64, -5);
                DrawSlider("Cursor Y offset", ref cursorImageOffsetY, -64, 64, -15);
                GUILayout.EndVertical();

                DrawToggle("Custom source", ref customCursorSource, Color.yellow, Get("customCursorSource"));
                
                if (BeginAnimatedFoldout("menu.settings.cursor.source", customCursorSource, -10f))
                {
                    GUILayout.Space(5);
                    GUILayout.BeginVertical(windowStyle);
                    DrawLabel("Recommended files: PNG-32x32", Color.grey);
                    DrawTextField("Source path:", ref cursorSourcePath, 80, 250);
                    
                    GUILayout.Space(5);
                    
                    if (GUILayout.Button("<b>UPDATE</b>", buttonStyle))
                    {
                        if (!string.IsNullOrEmpty(cursorSourcePath) && File.Exists(cursorSourcePath))
                        {
                            try
                            {
                                byte[] data = File.ReadAllBytes(cursorSourcePath);
                                Texture2D newCursor = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                                if (newCursor.LoadImage(data))
                                {
                                    CursorTexture = newCursor;
                                    Vector2 hotspot = new Vector2(
                                        CursorTexture.width / 2f + cursorImageOffsetX,
                                        CursorTexture.height / 2f + cursorImageOffsetY
                                    );
                                    Cursor.SetCursor(CursorTexture, hotspot, CursorMode.Auto);
                                    FireLog("[UC] Custom cursor loaded from: " + cursorSourcePath);
                                }
                            }
                            catch (Exception ex)
                            {
                                FireLog("[UC] Failed to load cursor from path: " + ex.Message);
                            }
                        }
                    }
                    
                    if (GUILayout.Button("<b>RESET</b>", buttonStyle))
                    {
                        CursorTexture = ResourceLoader.LoadTexture("UnifromCheat_REPO.Assets.unifrom_cursor.png");
                        if (CursorTexture != null)
                        {
                            Vector2 hotspot = new Vector2(CursorTexture.width / 2f, CursorTexture.height / 2f);
                            Cursor.SetCursor(CursorTexture, hotspot, CursorMode.Auto);
                        }
                        FireLog("[UC] Cursor reset to default snowflake");
                    }

                    GUILayout.EndVertical();
                    EndAnimatedFoldout();
                }
                GUILayout.EndVertical();
                EndAnimatedFoldout();
            }


            if (GUILayout.Button("<b>PROCEDURAL SNOWFALL</b>", buttonStyle))  proceduralSnowfall = !proceduralSnowfall;
            if (BeginAnimatedFoldout("menu.settings.snowfall", proceduralSnowfall))
            {
                DrawToggle("Enable snowfall", ref enableProceduralSnowfall, Color.green, Get("enableProceduralSnowfall"));
                if (BeginAnimatedFoldout("menu.settings.snowfall.enabled", enableProceduralSnowfall, -10f))
                {
                    GUILayout.Space(5);
                    GUILayout.BeginVertical(windowStyle);
                    DrawToggle("Only in menu", ref ps_onlyInMenu, Color.green, Get("ps_onlyInMenu"));
                    GUILayout.Space(5);
                    DrawSlider("Fall speed", ref ps_fallSpeed, 0, 1000, 50, Get("ps_fallSpeed"));
                    DrawSlider("Spawn interval", ref ps_spawnInterval, 0, 5, 0.2f, Get("ps_spawnInterval"));
                    DrawSlider("Scale", ref ps_scale, 0, 100, 3f, Get("ps_scale"));
                    DrawToggle("Dynamic scale", ref ps_dynamicScale, Color.green, Get("ps_dynamicScale"));
                    if (BeginAnimatedFoldout("menu.settings.snowfall.scaleRange", ps_dynamicScale, -8f))
                    {
                        GUILayout.Space(5);
                        DrawSlider("From", ref ps_scaleRangeA, 0, 100, 1.5f, Get("ps_scaleRangeA"));
                        DrawSlider("To", ref ps_scaleRangeB, 0, 100, 5f, Get("ps_scaleRangeB"));
                        if (ps_scaleRangeB < ps_scaleRangeA) ps_scaleRangeB = ps_scaleRangeA;
                        EndAnimatedFoldout();
                    }

                    DrawToggle("Custom flake source", ref pss_customSource, Color.yellow, Get("pss_customSource"));
                    if (BeginAnimatedFoldout("menu.settings.snowfall.source", pss_customSource, -10f))
                    {
                        GUILayout.Space(5);
                        GUILayout.BeginVertical(windowStyle);
                        DrawLabel("Recommended files: PNG-32x32", Color.grey);
                        DrawTextField("Source path:", ref pss_sourcePath, 80, 250);
                        
                        GUILayout.Space(5);
                        
                        if (GUILayout.Button("<b>UPDATE</b>", buttonStyle))
                        {
                            if (!string.IsNullOrEmpty(pss_sourcePath) && File.Exists(pss_sourcePath))
                            {
                                try
                                {
                                    byte[] data = File.ReadAllBytes(pss_sourcePath);
                                    Texture2D newFlake = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                                    if (newFlake.LoadImage(data))
                                    {
                                        pss_flakeTexture = newFlake;
                                        SnowfallUI.Instance.RefreshFlakeTexture();
                                        FireLog("[PSS] Custom snowflake loaded from: " + pss_sourcePath);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    FireLog("[PSS] Failed to load snowflake: " + ex.Message);
                                }
                            }
                        }

                        if (GUILayout.Button("<b>RESET</b>", buttonStyle))
                        {
                            pss_flakeTexture = ResourceLoader.LoadTexture("UnifromCheat_REPO.Assets.snowflake.png");
                            SnowfallUI.Instance.RefreshFlakeTexture();
                            FireLog("[PSS] Snowflake texture reset to default");
                        }
                        
                        GUILayout.EndVertical();
                        EndAnimatedFoldout();
                    }

                    DrawToggle("Spin", ref pss_spin, Color.green);
                    if (BeginAnimatedFoldout("menu.settings.snowfall.spin", pss_spin, -10f))
                    {
                        GUILayout.Space(5);
                        GUILayout.BeginVertical(windowStyle);
                        GUILayout.BeginVertical(windowStyle);
                        DrawLabel("Spin side: ", Color.white);
                        DrawRadio("To left side", 0, ref pss_side, Color.cyan);
                        DrawRadio("To right side", 1, ref pss_side, Color.cyan);
                        GUILayout.EndVertical();
                        GUILayout.Space(5);
                        DrawToggle("Dynamic select side", ref pss_dynamicSelectSide, Color.yellow);
                        DrawToggle("Dynamic rotate offset", ref pss_dynamicRotateOffset, Color.yellow);
                        DrawSlider("Spin speed", ref pss_spinSpeed, 0, 1000, 60f);

                        GUILayout.EndVertical();
                        EndAnimatedFoldout();
                    }
                    GUILayout.EndVertical();
                    EndAnimatedFoldout();
                }
                EndAnimatedFoldout();
            }

            if (GUILayout.Button("<b>TOOLTIPS LANGUAGE</b>", buttonStyle)) 
                tooltipsLanguage = !tooltipsLanguage;

            if (BeginAnimatedFoldout("menu.settings.language", tooltipsLanguage))
            {
                GUILayout.BeginVertical(windowStyle);
                DrawRadio("English", 0, ref lg_state, Color.white);
                DrawRadio("Russian", 1, ref lg_state, Color.white);
                DrawRadio("Ukrainian", 2, ref lg_state, Color.white);
                DrawRadio("Chinese", 3, ref lg_state, Color.white);
                GUILayout.Space(5);
                GUILayout.EndVertical();
                EndAnimatedFoldout();
            }
            GUILayout.EndVertical();
            EndAnimatedFoldout();
        }
        GUILayout.EndVertical();
    }

    private void PlayerSettingsTab()
    {
        GUILayout.BeginVertical(windowStyle, GUILayout.Width(150));
        if (GUILayout.Button("<b>PLAYER</b>", buttonStyle)) PlayerTab = !PlayerTab;

        if (BeginAnimatedFoldout("menu.player", PlayerTab))
        {
            GUILayout.BeginVertical(windowStyle);
            
            DrawToggle("God mode", ref isGodModeEnabled, Color.green, Get("godMode"));
            DrawToggle("Infinite sprint", ref isInfiniteSprint, Color.green, Get("infSprint"));
            DrawToggle("Infinite head energy", ref isInfiniteHeadEnergy, Color.green, Get("infHeadEnergy"));
            DrawToggle("Freecam", ref isFreecamEnabled, Color.green, Get("freecam"));
            if (BeginAnimatedFoldout("menu.player.freecam", isFreecamEnabled, -10f))
            {
                GUILayout.Space(5);
                GUILayout.BeginVertical(windowStyle);
                DrawKeybindField("Bind", ref freecamBind, "F6", "freecam");
                GUILayout.EndVertical();
                EndAnimatedFoldout();
            }
            DrawToggle("Speed hack", ref isSpeedHackEnabled, Color.green, Get("speedHack"));

            if (BeginAnimatedFoldout("menu.player.speed", isSpeedHackEnabled, -10f))
            {
                GUILayout.Space(10);
                GUILayout.BeginVertical(windowStyle);
                DrawSlider("Walk speed", ref walkSpeed, 0, 30, 2, Get("walkSpeedHack"));
                DrawSlider("Sprint speed", ref sprintSpeed, 0, 30, 5, Get("sprintSpeedHack"));
                DrawSlider("Crouch speed", ref crouchSpeed, 0, 30, 1, Get("crouchSpeedHack"));
                GUILayout.EndVertical();
                EndAnimatedFoldout();
            }

            DrawToggle("Jump force", ref isCustomJumpForceEnabled, Color.green, Get("jumpForce"));
            if (BeginAnimatedFoldout("menu.player.jump", isCustomJumpForceEnabled, -10f))
            {
                GUILayout.Space(10);
                GUILayout.BeginVertical(windowStyle);
                DrawSlider("Jump force", ref jumpForce, 0, 100, 17);
                GUILayout.EndVertical();
                EndAnimatedFoldout();
            }

            DrawToggle("Custom FOV", ref isCustomFovEnabled, Color.green, Get("customfov"));
            if (BeginAnimatedFoldout("menu.player.fov", isCustomFovEnabled, -10f))
            {
                GUILayout.Space(5);
                GUILayout.BeginVertical(windowStyle);
                DrawSlider("FOV", ref fovValue, 1, 360, 80);
                GUILayout.EndVertical();
                EndAnimatedFoldout();
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
            if (BeginAnimatedFoldout("menu.player.rgb", isRGBPlayerEnabled, -10f))
            {
                GUILayout.Space(5);
                GUILayout.BeginVertical(windowStyle);

                float timing = RGBupdateInterval;
                DrawSlider("Update interval", ref timing, 1, 10000, 200, Get("rgbPlayerUpdateInterval"));
                RGBupdateInterval = (ushort)timing;

                GUILayout.EndVertical();
                EndAnimatedFoldout();
            }
            
            /*DrawToggle("Disable camera shake", ref disableCameraShake, Color.green, Get("disableCameraShake"));*/
            
            /*DrawToggle("Nickname animator", ref nicknameAnimator, Color.green, Get(""));*/

            bool oldNoPostProcessing = isNoPostProcessingEnabled;
            DrawToggle("No Post-Processing", ref isNoPostProcessingEnabled, Color.green, Get("noPostProcessing"));
            if (oldNoPostProcessing != isNoPostProcessingEnabled)
                NoPostProcessingManager.SetEnabled(isNoPostProcessingEnabled);
            
            DrawToggle("Dead voice", ref isDeadVoiceEnabled, Color.green, Get("deadVoice"));
            DrawToggle("Tumble Bypass", ref isTumbleBypassEnabled, Color.green, Get("tumbleBypass"));
            
            DrawToggle("Flashlight settings", ref isFlashlightSettingsEnabled, Color.green, Get("flashlightSettings"));
            var flashlight = PlayerController.instance.playerAvatarScript.flashlightController.spotlight;
            if (BeginAnimatedFoldout("menu.player.flashlight", isFlashlightSettingsEnabled, -10f))
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
                EndAnimatedFoldout();
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
            EndAnimatedFoldout();
        }
        
        GUILayout.EndVertical();
    }

    private void WallHackSettingsTab()
    {
        GUILayout.BeginVertical(windowStyle, GUILayout.Width(325));
        if (GUILayout.Button("<b>WALLHACK</b>", buttonStyle)) WallHackTab = !WallHackTab;

        if (BeginAnimatedFoldout("menu.wallhack", WallHackTab))
        {
            GUILayout.BeginVertical(windowStyle);
            
            if (GUILayout.Button("<b>RENDER SETTINGS</b>", buttonStyle)) isRenderSettingsOpened = !isRenderSettingsOpened;
            if (BeginAnimatedFoldout("wh.rendersettings.enabled", isRenderSettingsOpened, -10f))
            {
                GUILayout.BeginVertical(windowStyle);
                
                DrawLabel("Render distance:", Color.white);
                wallHackCameraFarClipPlane = ValidateRenderDistance(wallHackCameraFarClipPlane);
                DrawSlider("Distance", ref wallHackCameraFarClipPlane, MinRenderDistance, MaxRenderDistance, DefaultRenderDistance, Get("renderDistance"));
                wallHackCameraFarClipPlane = ValidateRenderDistance(wallHackCameraFarClipPlane);
                
                GUILayout.EndVertical();
            }
            
            DrawToggle("Show items", ref isItemsWallHackEnabled, Color.green, Get("showItemsWH"));
            if (BeginAnimatedFoldout("wh.items.enabled", isItemsWallHackEnabled, -10f))
            {
                GUILayout.Space(5);
                GUILayout.BeginVertical(windowStyle);
                
                if (GUILayout.Button("<b>ITEM GLOW COLOR</b>", buttonStyle)) item_glow_color = !item_glow_color;
                if (BeginAnimatedFoldout("wh.item.glow", item_glow_color))
                {
                    GUILayout.BeginVertical(windowStyle);
                    
                    DrawLabel("Glow color:", Color.white);
                    DrawSlider("R", ref IC_R, 0f, 1f, 1f);
                    DrawSlider("G", ref IC_G, 0f, 1f, 0.8f);
                    DrawSlider("B", ref IC_B, 0f, 1f, 0.3f);
                    DrawSlider("A", ref IC_A, 0f, 1f, 1f);
                    
                    GUILayout.EndVertical();
                    EndAnimatedFoldout();
                }
                
                if (GUILayout.Button("<b>ITEM TEXT COLOR</b>", buttonStyle)) item_text_color = !item_text_color;
                if (BeginAnimatedFoldout("wh.item.text", item_text_color))
                {
                    GUILayout.BeginVertical(windowStyle);
                    
                    DrawToggle("Sync with glow", ref iwh_syncTextColorWithGlow, Color.yellow, Get("syncTextColorWithGlow"), "wh.item.text.syncGlow");
                    
                    DrawLabel("Text color:", Color.white);
                    DrawSlider("R", ref TIC_R, 0f, 1f, 1f);
                    DrawSlider("G", ref TIC_G, 0f, 1f, 0.8f);
                    DrawSlider("B", ref TIC_B, 0f, 1f, 0.3f);
                    DrawSlider("A", ref TIC_A, 0f, 1f, 1f);
                    
                    DrawSlider("Text size", ref itemTextSize, 0, 10, 3);
                    
                    GUILayout.EndVertical();
                    EndAnimatedFoldout();
                }
                
                GUILayout.EndVertical();
                
                GUILayout.BeginVertical(windowStyle);
                
                DrawToggle("Show money bags", ref showSurplusValuable, Color.green, Get("showMoneyBagsWH"));
                GUILayout.Space(5);
                
                if (GUILayout.Button("<b>MONEY BAGS COLOR</b>", buttonStyle)) moneyBagsColor = !moneyBagsColor;
                if (BeginAnimatedFoldout("wh.money.color", moneyBagsColor))
                {
                    GUILayout.BeginVertical(windowStyle);
                    
                    DrawSlider("R", ref SPC_R, 0f, 1f, 1f);
                    DrawSlider("G", ref SPC_G, 0f, 1f, 0f);
                    DrawSlider("B", ref SPC_B, 0f, 1f, 1f);
                    DrawSlider("A", ref SPC_A, 0f, 1f, 0.8f);
                    
                    GUILayout.EndVertical();
                    EndAnimatedFoldout();
                }
                
                bool newValue = showExtractionPoints;
                
                DrawToggle("Show extraction points", ref newValue, Color.green, Get("showExPointsWH"));
                
                if (newValue != showExtractionPoints)
                    showExtractionPoints = newValue;
                
                GUILayout.Space(5);
                
                if (GUILayout.Button("<b>EXTRACTION POINTS COLOR</b>", buttonStyle)) extractionPointsColor = !extractionPointsColor;
                
                if (BeginAnimatedFoldout("wh.extraction.color", extractionPointsColor))
                {
                    GUILayout.BeginVertical(windowStyle);
                    
                    DrawSlider("R", ref EPC_R, 0f, 1f, 1f);
                    DrawSlider("G", ref EPC_G, 0f, 1f, 1f);
                    DrawSlider("B", ref EPC_B, 0f, 1f, 1f);
                    DrawSlider("A", ref EPC_A, 0f, 1f, 0.1f);
                    
                    GUILayout.EndVertical();
                    EndAnimatedFoldout();
                }

                DrawToggle("Show name", ref showItemName, Color.green, Get("showItemNameWH"), "wh.item.showName");
                DrawToggle("Show price", ref showItemPrice, Color.green, Get("showItemPriceWH"), "wh.item.showPrice");
                DrawToggle("Sort by price", ref sortByPrice, Color.green, Get("sortByPriceWH"), "wh.item.sortByPrice");
                
                GUILayout.Space(5);

                if (BeginAnimatedFoldout("wh.item.sortPrice", sortByPrice, -8f))
                {
                    GUILayout.BeginVertical(windowStyle);
                    DrawSlider("Sort from", ref sortFromPrice, 0, 100000, 0);
                    DrawSlider("Sort to", ref sortToPrice, 0, 100000, 100000);
                    if (sortToPrice < sortFromPrice) sortToPrice = sortFromPrice;
                    GUILayout.EndVertical();
                    EndAnimatedFoldout();
                }
                GUILayout.EndVertical();
                EndAnimatedFoldout();
            }

            DrawToggle("Show cosmetic boxes", ref isCosmeticBoxesWallHackEnabled, Color.green, Get("showCosmeticBoxesWH"));
            if (BeginAnimatedFoldout("wh.cosmetic.enabled", isCosmeticBoxesWallHackEnabled, -10f))
            {
                GUILayout.Space(5);
                GUILayout.BeginVertical(windowStyle);

                if (GUILayout.Button("<b>COSMETIC BOX GLOW COLOR</b>", buttonStyle))
                    cosmetic_box_glow_color = !cosmetic_box_glow_color;
                if (BeginAnimatedFoldout("wh.cosmetic.glow", cosmetic_box_glow_color))
                {
                    GUILayout.BeginVertical(windowStyle);

                    DrawLabel("Glow color:", Color.white);
                    DrawSlider("R", ref CBC_R, 0f, 1f, 1f);
                    DrawSlider("G", ref CBC_G, 0f, 1f, 0f);
                    DrawSlider("B", ref CBC_B, 0f, 1f, 1f);
                    DrawSlider("A", ref CBC_A, 0f, 1f, 1f);

                    GUILayout.EndVertical();
                    EndAnimatedFoldout();
                }

                if (GUILayout.Button("<b>COSMETIC BOX TEXT COLOR</b>", buttonStyle))
                    cosmetic_box_text_color = !cosmetic_box_text_color;
                if (BeginAnimatedFoldout("wh.cosmetic.text", cosmetic_box_text_color))
                {
                    GUILayout.BeginVertical(windowStyle);

                    DrawToggle("Show rarity text", ref showCosmeticBoxRarity, Color.green, Get("showCosmeticBoxRarity"), "wh.cosmetic.showRarityText");
                    DrawToggle("Sync with glow", ref cosmeticBoxTextSyncWithGlow, Color.yellow,
                        Get("syncTextColorWithGlow"), "wh.cosmetic.text.syncGlow");

                    if (BeginAnimatedFoldout("wh.cosmetic.text.manual", !cosmeticBoxTextSyncWithGlow, -8f))
                    {
                        GUILayout.Space(5);
                        DrawLabel("Text color:", Color.white);
                        DrawSlider("R", ref CBTC_R, 0f, 1f, 1f);
                        DrawSlider("G", ref CBTC_G, 0f, 1f, 0f);
                        DrawSlider("B", ref CBTC_B, 0f, 1f, 1f);
                        DrawSlider("A", ref CBTC_A, 0f, 1f, 1f);
                        EndAnimatedFoldout();
                    }
                    if (cosmeticBoxTextSyncWithGlow)
                    {
                        DrawSlider("Text alpha", ref CBTC_A, 0f, 1f, 1f);
                    }

                    DrawSlider("Text size", ref cosmeticBoxTextSize, 0, 10, 3);

                    if (BeginAnimatedFoldout("wh.cosmetic.rarity.enabled", showCosmeticBoxRarity, -8f))
                    {
                        GUILayout.Space(5);
                        if (GUILayout.Button("<b>RARITY TEXT COLOR</b>", buttonStyle))
                            cosmetic_box_rarity_text_color = !cosmetic_box_rarity_text_color;

                        if (BeginAnimatedFoldout("wh.cosmetic.rarity", cosmetic_box_rarity_text_color))
                        {
                            GUILayout.BeginVertical(windowStyle);
                            DrawLabel("Rarity text color:", Color.white);
                            DrawSlider("R", ref CBRTC_R, 0f, 1f, 1f);
                            DrawSlider("G", ref CBRTC_G, 0f, 1f, 0f);
                            DrawSlider("B", ref CBRTC_B, 0f, 1f, 1f);
                            DrawSlider("A", ref CBRTC_A, 0f, 1f, 1f);
                            GUILayout.EndVertical();
                            EndAnimatedFoldout();
                        }
                        EndAnimatedFoldout();
                    }

                    GUILayout.EndVertical();
                    EndAnimatedFoldout();
                }

                GUILayout.EndVertical();
                EndAnimatedFoldout();
            }

            DrawToggle("Show enemies", ref isEnemyWallHackEnabled, Color.green, Get("showEnemiesWH"));
            if (BeginAnimatedFoldout("wh.enemy.enabled", isEnemyWallHackEnabled, -10f))
            {
                DrawToggle("Show name", ref showEnemyName, Color.green, Get("showEnemyNameWH"), "wh.enemy.showName");
                DrawToggle("Show health", ref showEnemyHealth, Color.green, Get("showEnemyHealthWH"), "wh.enemy.showHealth");
                
                DrawToggle("Show glow", ref showEnemyGlow, Color.green, Get("showEnemyGlowWH"), "wh.enemy.showGlow");
                
                GUILayout.Space(5);
                GUILayout.BeginVertical(windowStyle);

                if (GUILayout.Button("<b>ENEMY GLOW COLOR</b>", buttonStyle))
                    enemy_glow_color = !enemy_glow_color;
                if (BeginAnimatedFoldout("wh.enemy.glow", enemy_glow_color))
                {
                    GUILayout.BeginVertical(windowStyle);
                    DrawLabel("Glow color:", Color.white);
                    DrawSlider("R", ref EC_R, 0f, 1f, 1f);
                    DrawSlider("G", ref EC_G, 0f, 1f, 0f);
                    DrawSlider("B", ref EC_B, 0f, 1f, 0f);
                    DrawSlider("A", ref EC_A, 0f, 1f, 0.8f);
                    GUILayout.EndVertical();
                    EndAnimatedFoldout();
                }

                if (GUILayout.Button("<b>ENEMY TEXT COLOR</b>", buttonStyle))
                    enemy_text_color = !enemy_text_color;
                if (BeginAnimatedFoldout("wh.enemy.text", enemy_text_color))
                {
                    GUILayout.BeginVertical(windowStyle);
                    DrawToggle("Sync with glow", ref ewh_syncTextColorWithGlow, Color.yellow,
                        Get("syncTextColorWithGlow"), "wh.enemy.text.syncGlow");

                    GUILayout.Space(5);

                    DrawLabel("Text color:", Color.white);
                    DrawSlider("R", ref TEC_R, 0f, 1f, 1f);
                    DrawSlider("G", ref TEC_G, 0f, 1f, 0f);
                    DrawSlider("B", ref TEC_B, 0f, 1f, 0f);
                    DrawSlider("A", ref TEC_A, 0f, 1f, 0.8f);

                    DrawSlider("Text size", ref enemyTextSize, 0, 10, 3);
                    GUILayout.EndVertical();
                    EndAnimatedFoldout();
                }
                GUILayout.EndVertical();
                EndAnimatedFoldout();
            }
            
            DrawToggle("Show player", ref isPlayerWallHackEnabled, Color.green, Get("showPlayersWH"));
            if (BeginAnimatedFoldout("wh.player.enabled", isPlayerWallHackEnabled, -10f))
            {
                DrawToggle("Show name", ref showPlayerName, Color.green, Get("showPlayerNameWH"), "wh.player.showName");
                DrawToggle("Show health", ref showPlayerHealth, Color.green, Get("showPlayerHealthWH"), "wh.player.showHealth");
                DrawToggle("Show glow", ref isShowPlayerGlow, Color.green, Get("showPlayerGlowWH"), "wh.player.showGlow");

                GUILayout.Space(5);

                GUILayout.BeginVertical(windowStyle);
                
                if (GUILayout.Button("<b>PLAYER GLOW COLOR</b>", buttonStyle)) player_glow_color = !player_glow_color;
                if (BeginAnimatedFoldout("wh.player.glow", player_glow_color))
                {
                    GUILayout.BeginVertical(windowStyle);
                    
                    DrawLabel("Glow color:", Color.white);
                    
                    DrawSlider("R", ref PC_R, 0f, 1f, 0f);
                    DrawSlider("G", ref PC_G, 0f, 1f, 1f);
                    DrawSlider("B", ref PC_B, 0f, 1f, 1f);
                    DrawSlider("A", ref PC_A, 0f, 1f, 0.8f);
                    GUILayout.EndVertical();
                    EndAnimatedFoldout();
                }
                
                if (GUILayout.Button("<b>PLAYER TEXT COLOR</b>", buttonStyle)) player_text_color = !player_text_color;
                if (BeginAnimatedFoldout("wh.player.text", player_text_color))
                {
                    GUILayout.BeginVertical(windowStyle);
                    
                    DrawToggle("Sync with glow", ref pwh_syncTextColorWithGlow, Color.yellow,
                        Get("syncTextColorWithGlow"), "wh.player.text.syncGlow");
                    
                    GUILayout.Space(5);
                    
                    DrawLabel("Text color:", Color.white);
                    
                    DrawSlider("R", ref PTC_R, 0f, 1f, 0f);
                    DrawSlider("G", ref PTC_G, 0f, 1f, 1f);
                    DrawSlider("B", ref PTC_B, 0f, 1f, 1f);
                    DrawSlider("A", ref PTC_A, 0f, 1f, 0.8f);
                    
                    DrawSlider("Text size", ref playerTextSize, 0, 10, 3);
                    
                    GUILayout.EndVertical();
                    EndAnimatedFoldout();
                }
                
                GUILayout.EndVertical();

                DrawToggle("Show dead heads", ref isShowPlayerDeadHead, Color.green, Get("showDeadHeadsWH"), "wh.player.showDeadHeads");
                
                GUILayout.Space(5);
                
                if (GUILayout.Button("<b>DEAD HEADS COLOR</b>", buttonStyle)) player_deadHead_color = !player_deadHead_color;

                if (BeginAnimatedFoldout("wh.player.deadhead", player_deadHead_color))
                {
                    GUILayout.BeginVertical(windowStyle);
                    GUILayout.Space(5);
                    
                    DrawSlider("R", ref PCDH_R, 0f, 1f, 1f);
                    DrawSlider("G", ref PCDH_G, 0f, 1f, 0f);
                    DrawSlider("B", ref PCDH_B, 0f, 1f, 0.5f);
                    DrawSlider("A", ref PCDH_A, 0f, 1f, 0.8f);
                    
                    GUILayout.EndVertical();
                    EndAnimatedFoldout();
                }
                EndAnimatedFoldout();
            }
            GUILayout.EndVertical();
            EndAnimatedFoldout();
        }
        GUILayout.EndVertical();
    }

    private void MiscSettingsTab()
    {
        GUILayout.BeginVertical(windowStyle, GUILayout.Width(300));
        if (GUILayout.Button("<b>MISC</b>", buttonStyle)) MiscTab = !MiscTab;
        
        if (BeginAnimatedFoldout("menu.misc", MiscTab))
        {
            GUILayout.BeginVertical(windowStyle);
            
            DrawToggle("Noclip", ref isNoclipEnabled, Color.green, Get("noclip"));
            if (BeginAnimatedFoldout("menu.misc.noclip", isNoclipEnabled, -10f))
            {
                GUILayout.Space(5);
                GUILayout.BeginVertical(windowStyle);
                DrawKeybindField("Bind", ref noclipBind, "LeftAlt", "noclip");
                DrawSlider("Noclip speed", ref noclipSpeed, 1, 20, 5, Get("noclipSpeed"));
                GUILayout.EndVertical();
                EndAnimatedFoldout();
            }

            DrawToggle("Hide Me", ref isHideMeEnabled, Color.green, Get("hideMe"));
            if (BeginAnimatedFoldout("menu.misc.hideMe", isHideMeEnabled, -10f))
            {
                GUILayout.Space(5);
                GUILayout.BeginVertical(windowStyle);
                DrawKeybindField("Bind", ref hideMeBind, "F9", "hideMe");
                GUILayout.EndVertical();
                EndAnimatedFoldout();
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
            
            DrawToggle("Fullbright", ref _isFullbrightEnabled, Color.green, Get("fullbright"));
            if (BeginAnimatedFoldout("menu.misc.fullbright", isFullbrightEnabled, -10f))
            {
                GUILayout.Space(5);
                GUILayout.BeginVertical(windowStyle);
                DrawSlider("R", ref FB_R, 0f, 1f, 1f);
                DrawSlider("G", ref FB_G, 0f, 1f, 1f);
                DrawSlider("B", ref FB_B, 0f, 1f, 1f);
                GUILayout.EndVertical();
                EndAnimatedFoldout();
            }
            
            DrawToggle("Multi-jumps", ref multiJumps, Color.green, Get("multiJumps"));
            DrawToggle("No Token HUD", ref isNoTokenHudEnabled, Color.green, Get("noTokenHud"));

            GUILayout.Space(8);
            if (GUILayout.Button("<b>UNLOAD CHEAT</b>", buttonStyle, GUILayout.Height(32)))
                OpenUnloadConfirmation();

            Rect unloadRect = GUILayoutUtility.GetLastRect();
            if (!string.IsNullOrWhiteSpace(Get("unloadCheat")) && !HideAllTooltips)
                GUITooltip.Show(Get("unloadCheat"), unloadRect);

            GUILayout.Space(5);
            GUILayout.EndVertical();
            EndAnimatedFoldout();
        }
        GUILayout.EndVertical();
    }

    private void HostOnlyFunctions()
    {
        GUILayout.BeginVertical(windowStyle, GUILayout.Width(235));
        
        if (GUILayout.Button("<b>HOST ONLY FUNCTIONS</b>", buttonStyle)) 
            HostFunctsTab = !HostFunctsTab;
        
        if (BeginAnimatedFoldout("menu.host", HostFunctsTab))
        {
            GUILayout.BeginVertical(windowStyle);
            
            DrawHostOnlyToggle("Protected session", ref isProtectedSession, Color.green, Get("protectedSession"));
            DrawHostOnlyToggle("Infinity ammo", ref isInfiniteAmmo, Color.green, Get("infAmmo"));
            DrawHostOnlyToggle("Died cockroach", ref isDiedCockroachEnabled, Color.green, Get("diedCockroach"));
            if (BeginAnimatedFoldout("menu.host.diedCockroach", isDiedCockroachEnabled, -10f))
            {
                GUILayout.Space(5);
                GUILayout.BeginVertical(windowStyle);
                DrawSlider("Force", ref diedCockroachForce, 0.25f, 12f, 3f, Get("diedCockroachForce"));
                GUILayout.EndVertical();
                EndAnimatedFoldout();
            }
            
            DrawHostOnlyToggle("Valuables teleporter", ref valuablesTeleporter, Color.green, Get("valuablesTp"));
            if (BeginAnimatedFoldout("menu.host.valuables", valuablesTeleporter, -10f))
            {
                GUILayout.Space(5);
                GUILayout.BeginVertical(windowStyle);
                if (GUILayout.Button("Teleport", buttonStyle))
                {
                    if (HostOnlyGuard.CanUseHostOnly(true, "Valuables teleporter"))
                        MiscFunctions.Instance.TeleportValuables(vt_state, vtm_one_any, vtm_kinematic);
                }
                
                GUILayout.EndVertical();

                GUILayout.BeginVertical(windowStyle);
                DrawLabel("Modes: ", Color.white);
                
                DrawToggle("Kinematic", ref vtm_kinematic, Color.yellow, Get("valuablesKinematic"));
                if (BeginAnimatedFoldout("menu.host.valuables.kinematic", vtm_kinematic, -8f))
                {
                    GUILayout.Space(5);
                    GUILayout.BeginVertical(windowStyle);
                    DrawToggle("Permanent freeze", ref vtm_permanentFreeze, Color.green, Get("valuablesFreeze"));
                    GUILayout.Space(5);
                    if (BeginAnimatedFoldout("menu.host.valuables.freeze.timer", !vtm_permanentFreeze, -6f))
                    {
                        DrawSlider("Disable interval", ref vtm_kinematicDisableInterval, 0f, 20f, 3f);
                        GUILayout.Space(5);
                        EndAnimatedFoldout();
                    }
                    if (vtm_permanentFreeze)
                    {
                        DrawToggle("Disable on touch", ref vtm_disableKinematicOnTouch, Color.green, Get("valuablesDisableTouch"));
                        GUILayout.Space(5);
                    }
                    
                    GUILayout.EndVertical();
                    EndAnimatedFoldout();
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
                EndAnimatedFoldout();
            }
            
            DrawHostOnlyToggle("Enemies teleporter", ref enemiesTeleporter, Color.green, Get("enemiesTp"));
            if (BeginAnimatedFoldout("menu.host.enemies", enemiesTeleporter, -10f))
            {
                GUILayout.Space(5);
                GUILayout.BeginVertical(windowStyle);
                if (GUILayout.Button("Teleport", buttonStyle))
                {
                    if (HostOnlyGuard.CanUseHostOnly(true, "Enemies teleporter"))
                        MiscFunctions.Instance.TeleportEnemies(em_state, em_one_any, em_kinematic);
                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical(windowStyle);
                DrawLabel("Modes: ", Color.white);

                DrawToggle("Kinematic", ref em_kinematic, Color.yellow, Get("enemiesKinematic"));
                if (BeginAnimatedFoldout("menu.host.enemies.kinematic", em_kinematic, -8f))
                {
                    GUILayout.Space(5);
                    GUILayout.BeginVertical(windowStyle);
                    DrawToggle("Permanent freeze movement", ref em_permanentFreeze, Color.green, Get("enemiesFreeze"));
                    GUILayout.Space(5);
                    GUILayout.EndVertical();
                    EndAnimatedFoldout();
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
                EndAnimatedFoldout();
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
            
            DrawHostOnlyToggle("No fragility", ref isFragilityDisabled, Color.green, Get("fragilityOff"));
            DrawHostOnlyToggle("Ghost items", ref isGhostItemsMode, Color.green, Get("ghostItems"));
            DrawHostOnlyToggle("Lite items", ref isLiteItemsModeEnabled, Color.green, Get("liteItems"));
            DrawHostOnlyToggle("Peaceful enemies", ref isPeacefulEnemiesEnabled, Color.green, Get("peacefulEnemies"));
            DrawHostOnlyToggle("One-shot kills", ref isOneShotModeEnabled, Color.green, Get("oneShotKill"));

            GUILayout.Space(5);

            if (GUILayout.Button("<b>KILL ENEMIES</b>", buttonStyle))
            {
                if (HostOnlyGuard.CanUseHostOnly(true, "Kill enemies"))
                {
                    foreach (var enemy in EnemyDirector.instance.enemiesSpawned)
                    {
                        var hp = enemy.GetComponentInChildren<EnemyHealth>();
                    
                        if (hp == null) 
                            continue;

                        hp.Hurt(int.MaxValue, Vector3.up);
                    }
                } 
            }
            
            if (GUILayout.Button("<b>OBJECT SPAWNER</b>", buttonStyle))
            {
                objectSpawnerWindowOpen = !objectSpawnerWindowOpen;
            }

            if (GUILayout.Button("<b>GAME CONTROLLER</b>", buttonStyle))
            {
                gameControllerWindowOpen = !gameControllerWindowOpen;
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
            EndAnimatedFoldout();
        }
        
        GUILayout.EndVertical();
    }
    
    private void ConfigSettingsTab()
    {
        GUILayout.BeginVertical(windowStyle, GUILayout.Width(150));
        
        if (GUILayout.Button("Configs", buttonStyle)) 
            ConfigTab = !ConfigTab;
        
        if (BeginAnimatedFoldout("menu.config", ConfigTab))
        {
            GUILayout.BeginVertical(windowStyle);
            
            if (GUILayout.Button("Save config", buttonStyle))
                SaveConfig();
            
            if (GUILayout.Button("Load config", buttonStyle))
                LoadConfig();

            if (GUILayout.Button("Reset config", buttonStyle))
                ResetConfig();
            
            GUILayout.EndVertical();
            EndAnimatedFoldout();
        }
        GUILayout.EndVertical();
    }
}
