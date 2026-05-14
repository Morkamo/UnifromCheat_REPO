using HarmonyLib;

namespace UnifromCheat_REPO.Patches
{
    [HarmonyPatch(typeof(SpectateCamera), "LateUpdate")]
    internal static class FreecamSpectatePatch
    {
        private static void Postfix(SpectateCamera __instance)
        {
            Core.Instance?.UpdateFreecamAfterSpectateCameraLateUpdate(__instance);
        }
    }
}
