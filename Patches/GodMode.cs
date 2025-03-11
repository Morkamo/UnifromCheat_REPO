using HarmonyLib;

namespace UnifromCheat_REPO.Patches
{
    [HarmonyPatch(typeof(PlayerHealth), "Hurt")]
    public class GodMode
    {
        static bool Prefix()
        {
            if (Core.isGodModeEnabled)
                return false;

            return true;
        }
    }
}