using HarmonyLib;
using UnityEngine;

namespace UnifromCheat_REPO.Patches
{
    [HarmonyPatch(typeof(PlayerController), "FixedUpdate")]
    public class InfiniteSprint
    {
        protected static void Postfix(PlayerController __instance)
        {
            if (Core.isInfiniteSprint)
                __instance.EnergyCurrent = __instance.EnergyStart;
        }
    }
}