using UnifromCheat_REPO.Utils;
using UnityEngine;
using static UnifromCheat_REPO.Utils.FireboxConsole;

namespace UnifromCheat_REPO;

public partial class Core
{
    internal string cheatVersion = "3.1.0"; 
    
    private Rect RectMenu;
    private float dpiScaling => Mathf.Min(Screen.width / 1920f, Screen.height / 1080f);
    private Vector2 scrollPosition = Vector2.zero;
    private bool isMenuInitialized;
    
    public byte MenuID = 1;
    internal static bool MenuState = false;
    internal static bool dragWindow = false;
    internal static bool IsUnifromReady = false;

    internal static bool customCursor = false;
    internal static bool enableCustomCursor = true;
    public static Texture2D CursorTexture;
    internal static float cursorImageOffsetX = -5f;
    internal static float cursorImageOffsetY = -15f;
    
    internal static bool customCursorSource = false;
    internal static string cursorSourcePath = @"D:\Images\simpleImage.png";

    internal static bool tooltipsLanguage = true;
    internal static int lg_state = 1;

    internal static float HC_R = 1f;
    internal static float HC_G = 1f;
    internal static float HC_B = 1f;
    internal static float HC_A = 0.7f;

    private bool advancedMenuSettings = true;
    private bool PlayerTab = true;
    private bool WallHackTab = true;
    private bool MiscTab = true;
    private bool ConfigTab = true;
    private bool HostFunctsTab = true;
    internal static bool HideAllHints;
    internal static bool HideAllTooltips;
    
    /*internal static float menuOpacity = 0.7f;
    internal static float MC_R = 0.18f;
    internal static float MC_G = 0.18f;
    internal static float MC_B = 0.18f;*/

    internal static bool isGodModeEnabled;
    internal static bool isInfiniteSprint;
    /*internal static bool isInfiniteEnergy;*/

    internal static bool _isFullbrightEnabled;
    internal static bool isFullbrightEnabled
    {
        get => _isFullbrightEnabled;
        set
        {
            if (_isFullbrightEnabled == value)
                return;

            _isFullbrightEnabled = value;

            if (value)
            {
                FullbrightManager.SaveDefaultAmbient();
                FullbrightManager.ApplyFullbright();
            }
            else
            {
                FullbrightManager.RestoreDefaultAmbient();
            }
        }
    }
    
    internal static float FB_R = 1f;
    internal static float FB_G = 1f;
    internal static float FB_B = 1f;
    
    internal static bool isCustomFovEnabled;
    internal static float fovValue = 80;

    internal static bool isSpeedHackEnabled = true;
    internal static float walkSpeed = 2;
    internal static float sprintSpeed = 5;
    internal static float crouchSpeed = 1;

    internal static bool isCustomJumpForceEnabled = true;
    internal static float jumpForce = 17;
    
    internal static bool isItemsWallHackEnabled = true;
    internal static bool item_glow_color = false;
    internal static bool item_text_color = false;
    internal static bool iwh_syncTextColorWithGlow = true;
    
    internal static float IC_R = 1f;
    internal static float IC_G = 0.8f;
    internal static float IC_B = 0.3f;
    internal static float IC_A = 1;
    
    internal static float TIC_R = 1f;
    internal static float TIC_G = 0.8f;
    internal static float TIC_B = 0.3f;
    internal static float TIC_A = 1;

    internal static bool _showExtractionPoints = true;
    internal static bool showExtractionPoints
    {
        get => _showExtractionPoints;
        set
        {
            if (_showExtractionPoints == value) return;
            _showExtractionPoints = value;

            if (!value)
            {
                WallHack.ItemsWallHack.ClearExtractionCache();
                FireLog("[WH] Extraction points OFF");
            }
            else
            {
                FireLog("[WH] Extraction points ON");
                if (WallHack.ItemsWallHack.Instance != null)
                    WallHack.ItemsWallHack.Instance.OnEPVisibilityEnabledFromUI();
            }
        }
    }
    internal static bool extractionPointsColor = false;

    internal static float EPC_R = 1f;
    internal static float EPC_G = 1f;
    internal static float EPC_B = 1f;
    internal static float EPC_A = 0.1f;

    internal static bool showItemName = true;
    internal static bool showItemPrice = true;
    internal static bool sortByPrice;
    internal static float sortFromPrice;
    internal static float sortToPrice = 50000;
    internal static float itemTextSize = 3;

    internal static bool isEnemyWallHackEnabled = true;
    internal static bool enemy_glow_color = false;
    internal static bool enemy_text_color = false;
    
    internal static float EC_R = 1f;
    internal static float EC_G = 0f;
    internal static float EC_B = 0f;
    internal static float EC_A = 0.8f;

    internal static bool showEnemyName = true;
    internal static bool showEnemyGlow = true;
    internal static bool showEnemyHealth = true;
    
    internal static bool ewh_syncTextColorWithGlow = true;
    internal static float TEC_R = 1f;
    internal static float TEC_G = 0f;
    internal static float TEC_B = 0f;
    internal static float TEC_A = 0.8f;
    internal static float enemyTextSize = 3;

    internal static bool isPlayerWallHackEnabled = true;
    internal static bool isShowPlayerGlow = true;
    internal static bool isShowPlayerDeadHead = true;
    internal static bool player_glow_color = false;
    internal static bool player_text_color = false;
    internal static bool player_deadHead_color = false;
    
    internal static bool pwh_syncTextColorWithGlow = true;
    
    internal static float PC_R = 0f;
    internal static float PC_G = 1f;
    internal static float PC_B = 1f;
    internal static float PC_A = 0.8f;
    
    internal static float PTC_R = 0f;
    internal static float PTC_G = 1f;
    internal static float PTC_B = 1f;
    internal static float PTC_A = 0.8f;
    
    internal static float PCDH_R = 1f;
    internal static float PCDH_G = 0f;
    internal static float PCDH_B = 0.5f;
    internal static float PCDH_A = 0.8f;

    internal static bool showPlayerName = true;
    internal static bool showPlayerHealth = true;
    internal static float playerTextSize = 3;

    internal static bool isNoclipEnabled;
    internal static float noclipSpeed = 5f;
    
    internal static bool isOneShotModeEnabled;
    internal static bool isLiteItemsModeEnabled;
    internal static bool isFragilityDisabled;
    internal static bool isGhostItemsMode;

    internal static bool isRGBPlayerEnabled;
    internal static ushort RGBupdateInterval = 200;

    internal static bool isFlashlightSettingsEnabled = false;
    internal static bool isFlashlightShadowsEnabled = true;
    internal static float flashlightRange = 25f;
    internal static float flashlightSpotAngle = 60f;
    internal static float FLC_R = 1f;
    internal static float FLC_G = 0.674f;
    internal static float FLC_B = 0.382f;
    internal static bool prevFlashlightState = false;
    
    internal static bool showSurplusValuable = true;
    internal static bool moneyBagsColor = false;
    internal static float SPC_R = 1f;
    internal static float SPC_G = 0f;
    internal static float SPC_B = 1f;
    internal static float SPC_A = 0.8f;

    internal static bool multiJumps = false;
    
    internal static bool valuablesTeleporter = false;
    internal static int vt_state;
    internal static bool vtm_kinematic;
    internal static bool vtm_permanentFreeze = true;
    internal static bool vtm_teleportOnTouch = false;
    internal static bool vtm_disableKinematicOnTouch = true;
    internal static float vtm_kinematicDisableInterval = 3f;
    internal static float vtm_teleportYOffset = 0.5f;
    internal static bool vtm_one_any;
    
    internal static bool enemiesTeleporter = false;
    internal static int em_state = 2;
    internal static bool em_kinematic;
    internal static bool em_permanentFreeze = true;
    internal static bool em_teleportOnTouch = false;
    internal static float em_teleportYOffset = 0.5f;
    internal static bool em_one_any;
    
    internal static bool WH_BlockUpdates = false;
    internal static bool WH_AlreadyCleared = false;
    internal static float WH_UnlockDelay = 5f;

    internal static bool proceduralSnowfall = false;
    internal static bool enableProceduralSnowfall = true;
    internal static bool ps_onlyInMenu = true;
    internal static bool pss_spin = true;
    internal static bool pss_dynamicRotateOffset = true;
    internal static bool pss_dynamicSelectSide = true;
    internal static int pss_side = 0;
    internal static float pss_spinSpeed = 60f;
    internal static bool pss_customSource = false;
    public static Texture2D pss_flakeTexture = null;

    public string pss_sourcePath = @"D:\Images\simpleImage.png";
    
    internal static float ps_fallSpeed = 50f;
    internal static float ps_scale = 3f;
    internal static float ps_spawnInterval = 0.2f;
    internal static bool ps_dynamicScale = true;
    internal static float ps_scaleRangeA = 1.5f;
    internal static float ps_scaleRangeB = 5f;
    
    /*internal static float SF_R = 1f;
    internal static float SF_G = 1f;
    internal static float SF_B = 1f;
    internal static float SF_A = 1f;*/

    /*internal static bool disableCameraShake = false;*/

    /*internal static bool nicknameAnimator = false;
    internal static float na_updateInterval = 0.3f;
    internal static string na_originalName = String.Empty;*/

    /*internal static bool soulsSpawner = false;
    internal static int soulsNumber = 1;
    internal static int priceOneSoul = 10000;*/
}