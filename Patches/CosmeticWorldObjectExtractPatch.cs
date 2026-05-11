using HarmonyLib;
using Photon.Pun;
using System.Reflection;
using UnifromCheat_REPO.Utils;

namespace UnifromCheat_REPO.Patches
{
    [HarmonyPatch(typeof(CosmeticWorldObject), "ExtractRPC")]
    internal static class CosmeticWorldObjectExtractPatch
    {
        private static readonly FieldInfo InExtractionField = AccessTools.Field(typeof(CosmeticWorldObject), "inExtraction");

        [HarmonyPrefix]
        private static bool Prefix(CosmeticWorldObject __instance, PhotonMessageInfo _info = default)
        {
            if (!ObjectSpawner.TryGetSpawnedCosmeticWorldObjectRarity(__instance, out SemiFunc.Rarity rarity))
                return true;

            if (!SemiFunc.MasterOnlyRPC(_info) || !IsInExtraction(__instance))
                return false;

            CosmeticWorldObjectUI.instance?.Add(rarity);
            RoundDirector.instance?.CosmeticWorldObjectExtracted(rarity);
            return false;
        }

        private static bool IsInExtraction(CosmeticWorldObject instance)
        {
            return InExtractionField?.GetValue(instance) is bool value && value;
        }
    }
}
