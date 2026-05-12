using HarmonyLib;
using Photon.Pun;
using UnifromCheat_REPO.Utils;

namespace UnifromCheat_REPO.Patches
{
    [HarmonyPatch(typeof(CosmeticWorldObject), "ExtractRPC")]
    internal static class CosmeticWorldObjectExtractPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(CosmeticWorldObject __instance, PhotonMessageInfo _info = default)
        {
            if (!ObjectSpawner.TryGetSpawnedCosmeticWorldObjectRarity(__instance, out SemiFunc.Rarity rarity))
                return true;

            if (!SemiFunc.MasterOnlyRPC(_info))
                return false;

            CosmeticWorldObjectUI.instance?.Add(rarity);
            RoundDirector.instance?.CosmeticWorldObjectExtracted(rarity);
            return false;
        }
    }

    [HarmonyPatch(typeof(CosmeticWorldObject))]
    internal static class CosmeticWorldObjectSpawnSyncPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void AwakePostfix(CosmeticWorldObject __instance)
        {
            ObjectSpawner.TryRegisterSpawnedCosmeticWorldObjectFromInstantiationData(__instance);
        }

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void StartPostfix(CosmeticWorldObject __instance)
        {
            ObjectSpawner.TryRegisterSpawnedCosmeticWorldObjectFromInstantiationData(__instance);
        }
    }

    [HarmonyPatch(typeof(ValuableDirector), "CosmeticWorldObjectLevelLoopsClampedGet")]
    internal static class CosmeticWorldObjectLimitPatch
    {
        [HarmonyPostfix]
        private static void Postfix(ref int __result)
        {
            __result = System.Math.Max(__result, ObjectSpawner.GetSpawnedCosmeticWorldObjectCount());
        }
    }

}
