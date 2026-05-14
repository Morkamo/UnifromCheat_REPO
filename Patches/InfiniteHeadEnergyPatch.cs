using HarmonyLib;

namespace UnifromCheat_REPO.Patches
{
    [HarmonyPatch]
    internal static class InfiniteHeadEnergyPatch
    {
        private static readonly AccessTools.FieldRef<SpectateCamera, float> HeadEnergy =
            AccessTools.FieldRefAccess<SpectateCamera, float>("headEnergy");

        private static readonly AccessTools.FieldRef<SpectateCamera, bool> HeadEnergyEnough =
            AccessTools.FieldRefAccess<SpectateCamera, bool>("headEnergyEnough");

        private static readonly AccessTools.FieldRef<SpectateCamera, float> HeadEnergyPauseTimer =
            AccessTools.FieldRefAccess<SpectateCamera, float>("headEnergyPauseTimer");

        private static readonly AccessTools.FieldRef<PlayerDeathHead, bool> SpectatedLowEnergy =
            AccessTools.FieldRefAccess<PlayerDeathHead, bool>("spectatedLowEnergy");

        [HarmonyPatch(typeof(SpectateCamera), "HeadEnergyLogic")]
        [HarmonyPostfix]
        private static void KeepHeadEnergyFull(SpectateCamera __instance)
        {
            if (!Core.isInfiniteHeadEnergy || __instance == null)
                return;

            HeadEnergy(__instance) = 1f;
            HeadEnergyEnough(__instance) = true;
            HeadEnergyPauseTimer(__instance) = 0f;
        }

        [HarmonyPatch(typeof(PlayerDeathHead), "SpectatedLowEnergySet")]
        [HarmonyPrefix]
        private static void PreventLowEnergyEffect(ref bool _active)
        {
            if (Core.isInfiniteHeadEnergy && _active)
                _active = false;
        }

        [HarmonyPatch(typeof(PlayerDeathHead), "Update")]
        [HarmonyPostfix]
        private static void ClearExistingLowEnergyEffect(PlayerDeathHead __instance)
        {
            if (Core.isInfiniteHeadEnergy && __instance != null && SpectatedLowEnergy(__instance))
                SpectatedLowEnergy(__instance) = false;
        }
    }
}
