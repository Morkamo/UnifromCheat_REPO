using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnifromCheat_REPO.Utils;
using UnityEngine;

namespace UnifromCheat_REPO.Patches
{
    [HarmonyPatch(typeof(EnemyParent), "Spawn")]
    internal static class DisableSpawnAIPatch
    {
        private static bool Prefix()
        {
            return !GameController.IsDisableSpawnAIActive();
        }
    }

    [HarmonyPatch(typeof(ExtractionPoint), "StateSet")]
    internal static class DisableAutoExtractPatch
    {
        private static bool Prefix(ExtractionPoint __instance, ExtractionPoint.State newState)
        {
            if (!GameController.IsDisableAutoExtractActive() || newState != ExtractionPoint.State.Success)
                return true;

            if (SemiFunc.RunIsShop())
                return true;

            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerAvatar), "PlayerDeathRPC")]
    internal static class AutoRevivePatch
    {
        private static void Postfix(PlayerAvatar __instance)
        {
            if (!GameController.IsAutoReviveActive())
                return;

            GameController.QueueAutoRevive(__instance);
        }
    }

    [HarmonyPatch]
    internal static class SharedUpgradesPatch
    {
        private static readonly string[] UpgradeTypes =
        {
            "ItemUpgradePlayerHealth",
            "ItemUpgradePlayerEnergy",
            "ItemUpgradePlayerExtraJump",
            "ItemUpgradeMapPlayerCount",
            "ItemUpgradePlayerTumbleLaunch",
            "ItemUpgradePlayerTumbleClimb",
            "ItemUpgradeDeathHeadBattery",
            "ItemUpgradePlayerTumbleWings",
            "ItemUpgradePlayerSprintSpeed",
            "ItemUpgradePlayerCrouchRest",
            "ItemUpgradePlayerGrabStrength",
            "ItemUpgradePlayerGrabThrow",
            "ItemUpgradePlayerGrabRange"
        };

        private static IEnumerable<MethodBase> TargetMethods()
        {
            Assembly assembly = typeof(ItemUpgrade).Assembly;
            foreach (string typeName in UpgradeTypes)
            {
                MethodInfo method = assembly.GetType(typeName)?.GetMethod(
                    "Upgrade",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                );

                if (method != null)
                    yield return method;
            }
        }

        private static void Postfix(Component __instance, MethodBase __originalMethod)
        {
            GameController.ApplySharedUpgrade(__instance, __originalMethod.DeclaringType?.Name);
        }
    }
}
