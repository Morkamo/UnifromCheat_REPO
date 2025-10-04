using HarmonyLib;

namespace UnifromCheat_REPO.Patches;

[HarmonyPatch(typeof(CursorManager), "Update")]
public class PatchGameCursorVisible
{
    protected static bool Prefix()
    {
        if (Core.MenuState)
            return false;
        return true;
    }
}