using UnifromCheat_REPO.Utils;
using UnifromCheat_REPO.WallHack;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnifromCheat_REPO;

public partial class Core
{
    private HideMeSnapshot hideMeSnapshot;

    private struct HideMeSnapshot
    {
        public bool Captured;
        public bool MenuWasOpen;
        public bool ObjectSpawnerWasOpen;
        public bool GameControllerWasOpen;
        public bool ItemsWallHack;
        public bool CosmeticBoxesWallHack;
        public bool EnemyWallHack;
        public bool PlayerWallHack;
        public bool CustomFov;
        public bool RgbPlayer;
        public bool NoPostProcessing;
        public bool FlashlightSettings;
    }

    private void UpdateHideMe()
    {
        if (!isHideMeEnabled)
        {
            if (isHideMeActive)
                DeactivateHideMe();
            return;
        }

        if (Keyboard.current == null)
            return;

        if (WasHideMeHotkeyPressed())
            ToggleHideMe();
    }

    private void ToggleHideMe()
    {
        if (isHideMeActive)
            DeactivateHideMe();
        else
            ActivateHideMe();
    }

    private void ActivateHideMe()
    {
        if (isHideMeActive)
            return;

        hideMeSnapshot = new HideMeSnapshot
        {
            Captured = true,
            MenuWasOpen = MenuState,
            ObjectSpawnerWasOpen = objectSpawnerWindowOpen,
            GameControllerWasOpen = gameControllerWindowOpen,
            ItemsWallHack = isItemsWallHackEnabled,
            CosmeticBoxesWallHack = isCosmeticBoxesWallHackEnabled,
            EnemyWallHack = isEnemyWallHackEnabled,
            PlayerWallHack = isPlayerWallHackEnabled,
            CustomFov = isCustomFovEnabled,
            RgbPlayer = isRGBPlayerEnabled,
            NoPostProcessing = isNoPostProcessingEnabled,
            FlashlightSettings = isFlashlightSettingsEnabled
        };

        isHideMeActive = true;

        isItemsWallHackEnabled = false;
        isCosmeticBoxesWallHackEnabled = false;
        isEnemyWallHackEnabled = false;
        isPlayerWallHackEnabled = false;
        isCustomFovEnabled = false;
        isRGBPlayerEnabled = false;
        isNoPostProcessingEnabled = false;
        isFlashlightSettingsEnabled = false;

        RGBPlayer.StopCycle();
        NoPostProcessingManager.RestoreAll();
        MiscFunctions.RestoreAllCameraFov();
        MiscFunctions.RestoreDefaultFlashlightSettings();
        SetWallHackTemporaryVisible(false);
        SetHideMeInterfaceVisible(false);
    }

    private void DeactivateHideMe()
    {
        if (!isHideMeActive)
            return;

        var snapshot = hideMeSnapshot;
        isHideMeActive = false;

        if (snapshot.Captured)
        {
            isItemsWallHackEnabled = snapshot.ItemsWallHack;
            isCosmeticBoxesWallHackEnabled = snapshot.CosmeticBoxesWallHack;
            isEnemyWallHackEnabled = snapshot.EnemyWallHack;
            isPlayerWallHackEnabled = snapshot.PlayerWallHack;
            isCustomFovEnabled = snapshot.CustomFov;
            isRGBPlayerEnabled = snapshot.RgbPlayer;
            isNoPostProcessingEnabled = snapshot.NoPostProcessing;
            isFlashlightSettingsEnabled = snapshot.FlashlightSettings;

            if (isRGBPlayerEnabled)
                RGBPlayer.StartCycle();

            NoPostProcessingManager.SetEnabled(isNoPostProcessingEnabled);

            if (isFlashlightSettingsEnabled)
                MiscFunctions.ApplyConfiguredFlashlightSettings();
            else
                MiscFunctions.RestoreDefaultFlashlightSettings();

            SetWallHackTemporaryVisible(true);
            RestoreMenuAfterHideMe(snapshot);
        }

        ApplyConfiguredHintVisibility();
        hideMeSnapshot = default;
    }

    private static void SetWallHackTemporaryVisible(bool visible)
    {
        ItemsWallHack.SetTemporaryVisible(visible);
        CosmeticBoxesWallHack.SetTemporaryVisible(visible);
        EnemiesWallHack.SetTemporaryVisible(visible);
        PlayersWallHack.SetTemporaryVisible(visible);
    }

    private void SetHideMeInterfaceVisible(bool visible)
    {
        if (!visible)
        {
            MenuState = false;
            menuAnimationProgress = 0f;
            objectSpawnerWindowOpen = false;
            gameControllerWindowOpen = false;
            CloseGameControllerConfirmation();

            if (unifromCanvasObject != null)
                unifromCanvasObject.SetActive(false);

            ApplyMenuCursorState(false);
            SetHintsVisible(false);
            return;
        }

        ApplyConfiguredHintVisibility();
    }

    private void RestoreMenuAfterHideMe(HideMeSnapshot snapshot)
    {
        bool menuWasOpen = snapshot.MenuWasOpen;
        MenuState = menuWasOpen;
        menuAnimationProgress = menuWasOpen ? 1f : 0f;
        objectSpawnerWindowOpen = menuWasOpen && snapshot.ObjectSpawnerWasOpen;
        gameControllerWindowOpen = menuWasOpen && snapshot.GameControllerWasOpen;

        if (unifromCanvasObject != null)
            unifromCanvasObject.SetActive(menuWasOpen || (!ps_onlyInMenu && enableProceduralSnowfall));

        ApplyMenuCursorState(menuWasOpen);
    }

    private static void ApplyConfiguredHintVisibility()
    {
        SetHintsVisible(!HideAllHints && !isHideMeActive);
    }

    private static void SetHintsVisible(bool visible)
    {
        foreach (var hint in UnifromHints)
        {
            if (hint == null)
                continue;

            hint.SetActive(visible);
        }
    }

    private static void ApplyMenuCursorState(bool menuOpen)
    {
        if (menuOpen)
        {
            if (CursorTexture == null)
            {
                CursorTexture = ResourceLoader.LoadTexture("UnifromCheat_REPO.Assets.unifrom_cursor.png");
                if (CursorTexture != null)
                {
                    CursorTexture.filterMode = FilterMode.Point;
                    CursorTexture.wrapMode = TextureWrapMode.Clamp;
                }
            }

            if (CursorTexture != null)
            {
                Vector2 hotspot = new Vector2(
                    CursorTexture.width / 2f + cursorImageOffsetX,
                    CursorTexture.height / 2f + cursorImageOffsetY
                );

                Cursor.SetCursor(CursorTexture, hotspot, CursorMode.Auto);
                enableCustomCursor = true;
            }

            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            return;
        }

        if (enableCustomCursor)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            enableCustomCursor = false;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
