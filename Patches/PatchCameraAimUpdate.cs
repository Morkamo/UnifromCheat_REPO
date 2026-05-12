using HarmonyLib;

namespace UnifromCheat_REPO.Patches;

[HarmonyPatch(typeof(CameraAim), "Update")]
public class PatchCameraAimUpdate
{
    protected static bool Prefix()
    {
        if (!Core.MenuState)
            return true;

        if (GameDirector.instance == null || GameDirector.instance.currentState != GameDirector.gameState.Main)
            return true;

        if (SemiFunc.IsMainMenu() || SemiFunc.MenuLevel() || SemiFunc.RunIsLobbyMenu())
            return true;

        return false;
    }
}
