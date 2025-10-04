using System;
using System.IO;
using UnifromCheat_REPO.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace UnifromCheat_REPO.Utils
{
    [Serializable]
    public class ConfigSystem
    {
        // ===== Menu =====
        public float HC_R, HC_G, HC_B, HC_A;
        public bool HideAllHints;
        public bool HideAllTooltips;
        public float menuOpacity;
        public bool tooltipsLanguage;
        public int lg_state;
        public bool dragWindow;

        // ===== Player =====
        public bool isGodModeEnabled;
        public bool isInfiniteSprint;
        public bool isSpeedHackEnabled;
        public float walkSpeed;
        public float sprintSpeed;
        public float crouchSpeed;
        public bool isCustomJumpForceEnabled;
        public float jumpForce;

        public bool isCustomFovEnabled;
        public float fovValue;

        public bool isRGBPlayerEnabled;
        public ushort RGBupdateInterval;

        // ===== Flashlight =====
        public bool isFlashlightSettingsEnabled;
        public bool isFlashlightShadowsEnabled;
        public float flashlightRange;
        public float flashlightSpotAngle;
        public float FLC_R, FLC_G, FLC_B;

        // ===== Item WallHack =====
        public bool isItemWallHackEnabled;
        [FormerlySerializedAs("item_glow_color")] [FormerlySerializedAs("item_color")] public bool items_glow_color;
        public bool item_text_color;
        public bool iwh_syncTextColorWithGlow;
        public float IC_R, IC_G, IC_B, IC_A;
        public float TIC_R, TIC_G, TIC_B, TIC_A;
        public bool showItemName;
        public bool showItemPrice;
        public bool sortByPrice;
        public float sortFromPrice;
        public float sortToPrice;
        public float itemTextSize;

        public bool showExtractionPoints;
        public float EPC_R, EPC_G, EPC_B, EPC_A;

        public bool showSurplusValuable;
        public bool moneyBagsColor;
        public float SPC_R, SPC_G, SPC_B, SPC_A;

        // ===== Enemy WallHack =====
        public bool isEnemyWallHackEnabled;
        public bool enemy_glow_color;
        public bool enemy_text_color;
        public bool ewh_syncTextColorWithGlow;
        public float EC_R, EC_G, EC_B, EC_A;
        public float TEC_R, TEC_G, TEC_B, TEC_A;
        public bool showEnemyName;
        public bool showEnemyHealth;
        public float enemyTextSize;

        // ===== Player WallHack =====
        public bool isPlayerWallHackEnabled;
        public bool showPlayerName;
        public bool showPlayerHealth;
        public bool isShowPlayerGlow;
        public bool player_glow_color;
        public bool player_text_color;
        public bool pwh_syncTextColorWithGlow;
        public float PC_R, PC_G, PC_B, PC_A;
        public float PTC_R, PTC_G, PTC_B, PTC_A;
        public bool isShowPlayerDeadHead;
        public bool player_deadHead_color;
        public float PCDH_R, PCDH_G, PCDH_B, PCDH_A;
        public float playerTextSize;

        // ===== Snowfall =====
        public bool proceduralSnowfall;
        public bool enableProceduralSnowfall;
        public bool ps_onlyInMenu;
        public bool pss_spin;
        public bool pss_dynamicRotateOffset;
        public bool pss_dynamicSelectSide;
        public int pss_side;
        public float pss_spinSpeed;
        public bool pss_customSource;
        public string pss_sourcePath;
        public float ps_fallSpeed;
        public float ps_scale;
        public float ps_spawnInterval;
        public bool ps_dynamicScale;
        public float ps_scaleRangeA;
        public float ps_scaleRangeB;

        // ===== Cursor =====
        public bool customCursor;
        public bool enableCustomCursor;
        public float cursorImageOffsetX;
        public float cursorImageOffsetY;
        public bool customCursorSource;
        public string cursorSourcePath;

        // ===== Misc =====
        public bool isNoclipEnabled;
        public float noclipSpeed;

        public bool isFullbrightEnabled;
        public float FB_R, FB_G, FB_B;

        public bool isOneShotModeEnabled;
        public bool isSuperStrengthEnabled;
        public bool isFragilityDisabled;
        public bool isColliderDisabledOnGrab;
        public bool multiJumps;

        // ===== Teleporters: Valuables =====
        public bool valuablesTeleporter;
        public int vt_state;
        public bool vtm_kinematic; 
        public bool vtm_permanentFreeze;
        public bool vtm_teleportOnTouch;
        public bool vtm_disableKinematicOnTouch;
        public float vtm_kinematicDisableInterval;
        public float vtm_teleportYOffset;
        public bool vtm_one_any;

        // ===== Teleporters: Enemies =====
        public bool enemiesTeleporter;
        public int em_state;
        public bool em_kinematic;
        public bool em_permanentFreeze;
        public bool em_teleportOnTouch;
        public float em_teleportYOffset;
        public bool em_one_any;
    }
}

namespace UnifromCheat_REPO
{
    public partial class Core
    {
        private string cfgPath => Path.Combine(Application.persistentDataPath, "unifrom_config.json");

        public void SaveConfig()
        {
            ConfigSystem cfg = new ConfigSystem()
            {
                // ===== Menu =====
                HC_R = HC_R, HC_G = HC_G, HC_B = HC_B, HC_A = HC_A,
                HideAllHints = HideAllHints,
                HideAllTooltips = HideAllTooltips,
                tooltipsLanguage = tooltipsLanguage,
                lg_state = lg_state,
                dragWindow = dragWindow,

                // ===== Player =====
                isGodModeEnabled = isGodModeEnabled,
                isInfiniteSprint = isInfiniteSprint,
                isSpeedHackEnabled = isSpeedHackEnabled,
                walkSpeed = walkSpeed,
                sprintSpeed = sprintSpeed,
                crouchSpeed = crouchSpeed,
                isCustomJumpForceEnabled = isCustomJumpForceEnabled,
                jumpForce = jumpForce,

                isCustomFovEnabled = isCustomFovEnabled,
                fovValue = fovValue,

                isRGBPlayerEnabled = isRGBPlayerEnabled,
                RGBupdateInterval = RGBupdateInterval,

                // ===== Flashlight =====
                isFlashlightSettingsEnabled = isFlashlightSettingsEnabled,
                isFlashlightShadowsEnabled = isFlashlightShadowsEnabled,
                flashlightRange = flashlightRange,
                flashlightSpotAngle = flashlightSpotAngle,
                FLC_R = FLC_R, FLC_G = FLC_G, FLC_B = FLC_B,

                // ===== Item WH =====
                isItemWallHackEnabled = isItemsWallHackEnabled,
                items_glow_color = item_glow_color,
                item_text_color = item_text_color,
                iwh_syncTextColorWithGlow = iwh_syncTextColorWithGlow,
                IC_R = IC_R, IC_G = IC_G, IC_B = IC_B, IC_A = IC_A,
                TIC_R = TIC_R, TIC_G = TIC_G, TIC_B = TIC_B, TIC_A = TIC_A,
                showItemName = showItemName,
                showItemPrice = showItemPrice,
                sortByPrice = sortByPrice,
                sortFromPrice = sortFromPrice,
                sortToPrice = sortToPrice,
                itemTextSize = itemTextSize,

                showExtractionPoints = showExtractionPoints,
                EPC_R = EPC_R, EPC_G = EPC_G, EPC_B = EPC_B, EPC_A = EPC_A,

                showSurplusValuable = showSurplusValuable,
                moneyBagsColor = moneyBagsColor,
                SPC_R = SPC_R, SPC_G = SPC_G, SPC_B = SPC_B, SPC_A = SPC_A,

                // ===== Enemy WH =====
                isEnemyWallHackEnabled = isEnemyWallHackEnabled,
                enemy_glow_color = enemy_glow_color,
                enemy_text_color = enemy_text_color,
                ewh_syncTextColorWithGlow = ewh_syncTextColorWithGlow,
                EC_R = EC_R, EC_G = EC_G, EC_B = EC_B, EC_A = EC_A,
                TEC_R = TEC_R, TEC_G = TEC_G, TEC_B = TEC_B, TEC_A = TEC_A,
                showEnemyName = showEnemyName,
                showEnemyHealth = showEnemyHealth,
                enemyTextSize = enemyTextSize,

                // ===== Player WH =====
                isPlayerWallHackEnabled = isPlayerWallHackEnabled,
                showPlayerName = showPlayerName,
                showPlayerHealth = showPlayerHealth,
                isShowPlayerGlow = isShowPlayerGlow,
                player_glow_color = player_glow_color,
                player_text_color = player_text_color,
                pwh_syncTextColorWithGlow = pwh_syncTextColorWithGlow,
                PC_R = PC_R, PC_G = PC_G, PC_B = PC_B, PC_A = PC_A,
                PTC_R = PTC_R, PTC_G = PTC_G, PTC_B = PTC_B, PTC_A = PTC_A,
                isShowPlayerDeadHead = isShowPlayerDeadHead,
                player_deadHead_color = player_deadHead_color,
                PCDH_R = PCDH_R, PCDH_G = PCDH_G, PCDH_B = PCDH_B, PCDH_A = PCDH_A,
                playerTextSize = playerTextSize,

                // ===== Snowfall =====
                proceduralSnowfall = proceduralSnowfall,
                enableProceduralSnowfall = enableProceduralSnowfall,
                ps_onlyInMenu = ps_onlyInMenu,
                pss_spin = pss_spin,
                pss_dynamicRotateOffset = pss_dynamicRotateOffset,
                pss_dynamicSelectSide = pss_dynamicSelectSide,
                pss_side = pss_side,
                pss_spinSpeed = pss_spinSpeed,
                pss_customSource = pss_customSource,
                pss_sourcePath = pss_sourcePath,
                ps_fallSpeed = ps_fallSpeed,
                ps_scale = ps_scale,
                ps_spawnInterval = ps_spawnInterval,
                ps_dynamicScale = ps_dynamicScale,
                ps_scaleRangeA = ps_scaleRangeA,
                ps_scaleRangeB = ps_scaleRangeB,

                // ===== Cursor =====
                customCursor = customCursor,
                enableCustomCursor = enableCustomCursor,
                cursorImageOffsetX = cursorImageOffsetX,
                cursorImageOffsetY = cursorImageOffsetY,
                customCursorSource = customCursorSource,
                cursorSourcePath = cursorSourcePath,

                // ===== Misc =====
                isNoclipEnabled = isNoclipEnabled,
                noclipSpeed = noclipSpeed,

                isFullbrightEnabled = isFullbrightEnabled,
                FB_R = FB_R, FB_G = FB_G, FB_B = FB_B,

                isOneShotModeEnabled = isOneShotModeEnabled,
                isSuperStrengthEnabled = isLiteItemsModeEnabled,
                isFragilityDisabled = isFragilityDisabled,
                isColliderDisabledOnGrab = isGhostItemsMode,
                multiJumps = multiJumps,

                // ===== TP Valuables =====
                valuablesTeleporter = valuablesTeleporter,
                vt_state = vt_state,
                vtm_kinematic = vtm_kinematic,
                vtm_permanentFreeze = vtm_permanentFreeze,
                vtm_teleportOnTouch = vtm_teleportOnTouch,
                vtm_disableKinematicOnTouch = vtm_disableKinematicOnTouch,
                vtm_kinematicDisableInterval = vtm_kinematicDisableInterval,
                vtm_teleportYOffset = vtm_teleportYOffset,
                vtm_one_any = vtm_one_any,

                // ===== TP Enemies =====
                enemiesTeleporter = enemiesTeleporter,
                em_state = em_state,
                em_kinematic = em_kinematic,
                em_permanentFreeze = em_permanentFreeze,
                em_teleportOnTouch = em_teleportOnTouch,
                em_teleportYOffset = em_teleportYOffset,
                em_one_any = em_one_any
            };

            string json = JsonUtility.ToJson(cfg, true);
            File.WriteAllText(cfgPath, json);
            FireboxConsole.FireLog("[CFG] Config saved!");
        }

        public void LoadConfig()
        {
            if (!File.Exists(cfgPath))
            {
                FireboxConsole.FireLog("[CFG] No config found!");
                return;
            }

            string json = File.ReadAllText(cfgPath);
            ConfigSystem cfg = JsonUtility.FromJson<ConfigSystem>(json);

            // ===== Menu =====
            HC_R = cfg.HC_R; HC_G = cfg.HC_G; HC_B = cfg.HC_B; HC_A = cfg.HC_A;
            HideAllHints = cfg.HideAllHints;
            HideAllTooltips = cfg.HideAllTooltips;
            tooltipsLanguage = cfg.tooltipsLanguage;
            lg_state = cfg.lg_state;
            dragWindow = cfg.dragWindow;

            // ===== Player =====
            isGodModeEnabled = cfg.isGodModeEnabled;
            isInfiniteSprint = cfg.isInfiniteSprint;
            isSpeedHackEnabled = cfg.isSpeedHackEnabled;
            walkSpeed = cfg.walkSpeed;
            sprintSpeed = cfg.sprintSpeed;
            crouchSpeed = cfg.crouchSpeed;
            isCustomJumpForceEnabled = cfg.isCustomJumpForceEnabled;
            jumpForce = cfg.jumpForce;

            isCustomFovEnabled = cfg.isCustomFovEnabled;
            fovValue = cfg.fovValue;

            isRGBPlayerEnabled = cfg.isRGBPlayerEnabled;
            RGBupdateInterval = cfg.RGBupdateInterval;

            // ===== Flashlight =====
            isFlashlightSettingsEnabled = cfg.isFlashlightSettingsEnabled;
            isFlashlightShadowsEnabled = cfg.isFlashlightShadowsEnabled;
            flashlightRange = cfg.flashlightRange;
            flashlightSpotAngle = cfg.flashlightSpotAngle;
            FLC_R = cfg.FLC_R; FLC_G = cfg.FLC_G; FLC_B = cfg.FLC_B;

            // ===== Item WH =====
            isItemsWallHackEnabled = cfg.isItemWallHackEnabled;
            item_glow_color = cfg.items_glow_color;
            item_text_color = cfg.item_text_color;
            iwh_syncTextColorWithGlow = cfg.iwh_syncTextColorWithGlow;
            IC_R = cfg.IC_R; IC_G = cfg.IC_G; IC_B = cfg.IC_B; IC_A = cfg.IC_A;
            TIC_R = cfg.TIC_R; TIC_G = cfg.TIC_G; TIC_B = cfg.TIC_B; TIC_A = cfg.TIC_A;
            showItemName = cfg.showItemName;
            showItemPrice = cfg.showItemPrice;
            sortByPrice = cfg.sortByPrice;
            sortFromPrice = cfg.sortFromPrice;
            sortToPrice = cfg.sortToPrice;
            itemTextSize = cfg.itemTextSize;

            showExtractionPoints = cfg.showExtractionPoints;
            EPC_R = cfg.EPC_R; EPC_G = cfg.EPC_G; EPC_B = cfg.EPC_B; EPC_A = cfg.EPC_A;

            showSurplusValuable = cfg.showSurplusValuable;
            moneyBagsColor = cfg.moneyBagsColor;
            SPC_R = cfg.SPC_R; SPC_G = cfg.SPC_G; SPC_B = cfg.SPC_B; SPC_A = cfg.SPC_A;

            // ===== Enemy WH =====
            isEnemyWallHackEnabled = cfg.isEnemyWallHackEnabled;
            enemy_glow_color = cfg.enemy_glow_color;
            enemy_text_color = cfg.enemy_text_color;
            ewh_syncTextColorWithGlow = cfg.ewh_syncTextColorWithGlow;
            EC_R = cfg.EC_R; EC_G = cfg.EC_G; EC_B = cfg.EC_B; EC_A = cfg.EC_A;
            TEC_R = cfg.TEC_R; TEC_G = cfg.TEC_G; TEC_B = cfg.TEC_B; TEC_A = cfg.TEC_A;
            showEnemyName = cfg.showEnemyName;
            showEnemyHealth = cfg.showEnemyHealth;
            enemyTextSize = cfg.enemyTextSize;

            // ===== Player WH =====
            isPlayerWallHackEnabled = cfg.isPlayerWallHackEnabled;
            showPlayerName = cfg.showPlayerName;
            showPlayerHealth = cfg.showPlayerHealth;
            isShowPlayerGlow = cfg.isShowPlayerGlow;
            player_glow_color = cfg.player_glow_color;
            player_text_color = cfg.player_text_color;
            pwh_syncTextColorWithGlow = cfg.pwh_syncTextColorWithGlow;
            PC_R = cfg.PC_R; PC_G = cfg.PC_G; PC_B = cfg.PC_B; PC_A = cfg.PC_A;
            PTC_R = cfg.PTC_R; PTC_G = cfg.PTC_G; PTC_B = cfg.PTC_B; PTC_A = cfg.PTC_A;
            isShowPlayerDeadHead = cfg.isShowPlayerDeadHead;
            player_deadHead_color = cfg.player_deadHead_color;
            PCDH_R = cfg.PCDH_R; PCDH_G = cfg.PCDH_G; PCDH_B = cfg.PCDH_B; PCDH_A = cfg.PCDH_A;
            playerTextSize = cfg.playerTextSize;

            // ===== Snowfall =====
            proceduralSnowfall = cfg.proceduralSnowfall;
            enableProceduralSnowfall = cfg.enableProceduralSnowfall;
            ps_onlyInMenu = cfg.ps_onlyInMenu;
            pss_spin = cfg.pss_spin;
            pss_dynamicRotateOffset = cfg.pss_dynamicRotateOffset;
            pss_dynamicSelectSide = cfg.pss_dynamicSelectSide;
            pss_side = cfg.pss_side;
            pss_spinSpeed = cfg.pss_spinSpeed;
            pss_customSource = cfg.pss_customSource;
            pss_sourcePath = cfg.pss_sourcePath;
            ps_fallSpeed = cfg.ps_fallSpeed;
            ps_scale = cfg.ps_scale;
            ps_spawnInterval = cfg.ps_spawnInterval;
            ps_dynamicScale = cfg.ps_dynamicScale;
            ps_scaleRangeA = cfg.ps_scaleRangeA;
            ps_scaleRangeB = cfg.ps_scaleRangeB;

            // ===== Cursor =====
            customCursor = cfg.customCursor;
            enableCustomCursor = cfg.enableCustomCursor;
            cursorImageOffsetX = cfg.cursorImageOffsetX;
            cursorImageOffsetY = cfg.cursorImageOffsetY;
            customCursorSource = cfg.customCursorSource;
            cursorSourcePath = cfg.cursorSourcePath;

            // ===== Misc =====
            isNoclipEnabled = cfg.isNoclipEnabled;
            noclipSpeed = cfg.noclipSpeed;

            isFullbrightEnabled = cfg.isFullbrightEnabled;
            FB_R = cfg.FB_R; FB_G = cfg.FB_G; FB_B = cfg.FB_B;

            isOneShotModeEnabled = cfg.isOneShotModeEnabled;
            isLiteItemsModeEnabled = cfg.isSuperStrengthEnabled;
            isFragilityDisabled = cfg.isFragilityDisabled;
            isGhostItemsMode = cfg.isColliderDisabledOnGrab;
            multiJumps = cfg.multiJumps;

            // ===== TP Valuables =====
            valuablesTeleporter = cfg.valuablesTeleporter;
            vt_state = cfg.vt_state;
            vtm_kinematic = cfg.vtm_kinematic;
            vtm_permanentFreeze = cfg.vtm_permanentFreeze;
            vtm_teleportOnTouch = cfg.vtm_teleportOnTouch;
            vtm_disableKinematicOnTouch = cfg.vtm_disableKinematicOnTouch;
            vtm_kinematicDisableInterval = cfg.vtm_kinematicDisableInterval;
            vtm_teleportYOffset = cfg.vtm_teleportYOffset;
            vtm_one_any = cfg.vtm_one_any;

            // ===== TP Enemies =====
            enemiesTeleporter = cfg.enemiesTeleporter;
            em_state = cfg.em_state;
            em_kinematic = cfg.em_kinematic;
            em_permanentFreeze = cfg.em_permanentFreeze;
            em_teleportOnTouch = cfg.em_teleportOnTouch;
            em_teleportYOffset = cfg.em_teleportYOffset;
            em_one_any = cfg.em_one_any;

            FireboxConsole.FireLog("[CFG] Config loaded!");
        }

        public void ResetConfig()
        {
            // ===== Menu =====
            HC_R = 1f; HC_G = 1f; HC_B = 1f; HC_A = 0.7f;
            HideAllHints = false;
            HideAllTooltips = false;
            tooltipsLanguage = true;
            lg_state = 1;
            dragWindow = false;

            // ===== Player =====
            isGodModeEnabled = false;
            isInfiniteSprint = false;
            isSpeedHackEnabled = true;
            walkSpeed = 2f; sprintSpeed = 5f; crouchSpeed = 1f;
            isCustomJumpForceEnabled = true;
            jumpForce = 17f;

            isCustomFovEnabled = false;
            fovValue = 80f;

            isRGBPlayerEnabled = false;
            RGBupdateInterval = 200;

            // ===== Flashlight =====
            isFlashlightSettingsEnabled = false;
            isFlashlightShadowsEnabled = true;
            flashlightRange = 25f;
            flashlightSpotAngle = 60f;
            FLC_R = 1f; FLC_G = 0.674f; FLC_B = 0.382f;

            // ===== Item WH =====
            isItemsWallHackEnabled = true;
            item_glow_color = false;
            item_text_color = false;
            iwh_syncTextColorWithGlow = true;
            IC_R = 1f; IC_G = 0.8f; IC_B = 0.3f; IC_A = 1f;
            TIC_R = 1f; TIC_G = 0.8f; TIC_B = 0.3f; TIC_A = 1f;
            showItemName = true; showItemPrice = true; sortByPrice = false;
            sortFromPrice = 0; sortToPrice = 100000; itemTextSize = 3f;

            showExtractionPoints = true;
            EPC_R = 1f; EPC_G = 1f; EPC_B = 1f; EPC_A = 0.1f;

            showSurplusValuable = true;
            moneyBagsColor = false;
            SPC_R = 1f; SPC_G = 0f; SPC_B = 1f; SPC_A = 0.8f;

            // ===== Enemy WH =====
            isEnemyWallHackEnabled = true;
            enemy_glow_color = false;
            enemy_text_color = false;
            ewh_syncTextColorWithGlow = true;
            EC_R = 1f; EC_G = 0f; EC_B = 0f; EC_A = 0.8f;
            TEC_R = 1f; TEC_G = 0f; TEC_B = 0f; TEC_A = 0.8f;
            showEnemyName = true; showEnemyHealth = true; enemyTextSize = 3f;

            // ===== Player WH =====
            isPlayerWallHackEnabled = true;
            showPlayerName = true;
            showPlayerHealth = true;
            isShowPlayerGlow = true;
            player_glow_color = false;
            player_text_color = false;
            pwh_syncTextColorWithGlow = true;
            PC_R = 0f; PC_G = 1f; PC_B = 1f; PC_A = 0.8f;
            PTC_R = 0f; PTC_G = 1f; PTC_B = 1f; PTC_A = 0.8f;
            isShowPlayerDeadHead = true;
            player_deadHead_color = false;
            PCDH_R = 1f; PCDH_G = 0f; PCDH_B = 0.5f; PCDH_A = 0.8f;
            playerTextSize = 3f;

            // ===== Snowfall =====
            proceduralSnowfall = false;
            enableProceduralSnowfall = true;
            ps_onlyInMenu = true;
            pss_spin = true;
            pss_dynamicRotateOffset = true;
            pss_dynamicSelectSide = true;
            pss_side = 0;
            pss_spinSpeed = 60f;
            pss_customSource = false;
            pss_sourcePath = @"D:\Images\simpleImage.png";
            ps_fallSpeed = 50f;
            ps_scale = 3f;
            ps_spawnInterval = 0.2f;
            ps_dynamicScale = true;
            ps_scaleRangeA = 1.5f;
            ps_scaleRangeB = 5f;

            // ===== Cursor =====
            customCursor = false;
            enableCustomCursor = true;
            cursorImageOffsetX = -5f;
            cursorImageOffsetY = -15f;
            customCursorSource = false;
            cursorSourcePath = @"D:\Images\simpleImage.png";

            // ===== Misc =====
            isNoclipEnabled = false; noclipSpeed = 5f;
            isFullbrightEnabled = false; FB_R = 1f; FB_G = 1f; FB_B = 1f;
            isOneShotModeEnabled = false;
            isLiteItemsModeEnabled = false;
            isFragilityDisabled = false;
            isGhostItemsMode = false;
            multiJumps = false;

            // ===== TP Valuables =====
            valuablesTeleporter = false;
            vt_state = 0;
            vtm_kinematic = false;
            vtm_permanentFreeze = true;
            vtm_teleportOnTouch = false;
            vtm_disableKinematicOnTouch = false;
            vtm_kinematicDisableInterval = 3f;
            vtm_teleportYOffset = 1f;
            vtm_one_any = false;

            // ===== TP Enemies =====
            enemiesTeleporter = false;
            em_state = 2;
            em_kinematic = false;
            em_permanentFreeze = true;
            em_teleportOnTouch = false;
            em_teleportYOffset = 1f;
            em_one_any = false;

            FireboxConsole.FireLog("[CFG] Config reset to default!");
        }

    }
}