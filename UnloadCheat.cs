using System.Collections.Generic;
using UnifromCheat_REPO.Utils;
using UnifromCheat_REPO.WallHack;
using UnityEngine;
using static UnifromCheat_REPO.GUIMenuSkin;
using static UnifromCheat_REPO.TooltipsLanguages;
using Object = UnityEngine.Object;

namespace UnifromCheat_REPO;

public partial class Core
{
    private const int UnloadConfirmWindowId = 3010;
    private bool unloadConfirmationOpen;
    private Rect unloadConfirmRect;

    private void OpenUnloadConfirmation()
    {
        unloadConfirmationOpen = true;
        float width = 420f;
        float height = 190f;
        unloadConfirmRect = new Rect(
            RectMenu.x + (RectMenu.width - width) / 2f,
            RectMenu.y + 120f,
            width,
            height
        );
    }

    private void DrawUnloadConfirmationWindow(float alpha)
    {
        if (!unloadConfirmationOpen)
            return;

        Event current = Event.current;
        if (current.type == EventType.MouseDown && !unloadConfirmRect.Contains(current.mousePosition))
        {
            unloadConfirmationOpen = false;
            current.Use();
            return;
        }

        Color previousColor = GUI.color;
        int previousDepth = GUI.depth;
        GUI.depth = -7100;
        GUI.color = new Color(previousColor.r, previousColor.g, previousColor.b, previousColor.a * alpha);
        unloadConfirmRect = GUI.Window(UnloadConfirmWindowId, unloadConfirmRect, DrawUnloadConfirmationContents, string.Empty, windowStyle);
        GUI.BringWindowToFront(UnloadConfirmWindowId);
        GUI.depth = previousDepth;
        GUI.color = previousColor;
    }

    private void DrawUnloadConfirmationContents(int id)
    {
        GUILayout.BeginVertical();
        DrawLabel(Get("unloadConfirmTitle"), Color.white);
        GUILayout.Label(Get("unloadConfirmWarning"), labelStyle, GUILayout.Height(34));
        GUILayout.Label(Get("unloadConfirmDynamicWarning"), labelStyle, GUILayout.Height(44));
        GUILayout.FlexibleSpace();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button($"<b>{Get("confirmButton")}</b>", buttonStyle, GUILayout.Height(32)))
        {
            unloadConfirmationOpen = false;
            UnloadCheatFromGame();
        }

        if (GUILayout.Button($"<b>{Get("cancelButton")}</b>", buttonStyle, GUILayout.Height(32)))
            unloadConfirmationOpen = false;
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    private void UnloadCheatFromGame()
    {
        FireboxConsole.FireLog("[Unifrom] Dynamic unload started.");

        isHideMeEnabled = false;
        isHideMeActive = false;
        hideMeSnapshot = default;

        MenuState = false;
        menuAnimationProgress = 0f;
        objectSpawnerWindowOpen = false;
        gameControllerWindowOpen = false;
        CloseGameControllerConfirmation();
        unloadConfirmationOpen = false;

        Noclip?.ForceDisable();
        isNoclipEnabled = false;
        isCustomFovEnabled = false;
        isRGBPlayerEnabled = false;
        isFlashlightSettingsEnabled = false;
        isFullbrightEnabled = false;

        RGBPlayer.StopCycle();
        MiscFunctions.RestoreAllCameraFov();
        MiscFunctions.RestoreDefaultFlashlightSettings();

        ItemsWallHack.ClearAll();
        CosmeticBoxesWallHack.ClearAll();
        EnemiesWallHack.ClearAll();
        PlayersWallHack.ClearAll();

        ClearMessages();
        DestroyHints();
        DestroyMenuCanvas();
        ApplyMenuCursorState(false);

        harmony?.UnpatchSelf();
        harmony = null;

        IsUnifromReady = false;
        Instance = null;

        FireboxConsole.FireLog("[Unifrom] Dynamic unload completed.");
        FireboxConsole.Close();

        GameObject target = Loader.LoaderObject != null ? Loader.LoaderObject : gameObject;
        Loader.LoaderObject = null;
        Object.Destroy(target);
    }

    private static void DestroyHints()
    {
        var destroyed = new HashSet<GameObject>();
        foreach (GameObject hint in UnifromHints)
        {
            if (hint == null || destroyed.Contains(hint))
                continue;

            destroyed.Add(hint);
            Object.Destroy(hint);
        }

        UnifromHints.Clear();

        if (HintsController.globalCanvas != null)
        {
            Object.Destroy(HintsController.globalCanvas.gameObject);
            HintsController.globalCanvas = null;
        }
    }

    private static void DestroyMenuCanvas()
    {
        if (unifromCanvasObject == null)
            return;

        Object.Destroy(unifromCanvasObject);
        unifromCanvasObject = null;
    }
}
