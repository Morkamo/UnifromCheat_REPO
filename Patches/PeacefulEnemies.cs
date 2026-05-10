using HarmonyLib;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnifromCheat_REPO.Utils;

namespace UnifromCheat_REPO.Patches
{
    [HarmonyPatch]
    internal static class PeacefulEnemies
    {
        private static readonly FieldInfo TargetPlayerAvatarField = AccessTools.Field(typeof(Enemy), "TargetPlayerAvatar");
        private static readonly FieldInfo TargetPlayerViewIDField = AccessTools.Field(typeof(Enemy), "TargetPlayerViewID");
        private static readonly FieldInfo EnemyVisionHasVisionField = AccessTools.Field(typeof(EnemyVision), "HasVision");

        [HarmonyPatch(typeof(EnemyVision), "VisionTrigger")]
        [HarmonyPrefix]
        private static bool EnemyVisionTriggerPrefix(EnemyVision __instance, int playerID)
        {
            if (!IsEnabled())
                return true;

            ResetVisionForPlayer(__instance, playerID);
            return false;
        }

        [HarmonyPatch(typeof(Enemy), "SetChaseTarget")]
        [HarmonyPrefix]
        private static bool EnemySetChaseTargetPrefix(Enemy __instance)
        {
            if (!IsEnabled())
                return true;

            ResetEnemy(__instance);
            return false;
        }

        [HarmonyPatch(typeof(Enemy), "Update")]
        [HarmonyPostfix]
        private static void EnemyUpdatePostfix(Enemy __instance)
        {
            if (!IsEnabled())
                return;

            ResetEnemy(__instance);
        }

        private static void ResetEnemy(Enemy enemy)
        {
            if (enemy == null)
                return;

            if (GameManager.Multiplayer() && !PhotonNetwork.IsMasterClient)
                return;

            var vision = enemy.GetComponent<EnemyVision>();
            if (vision != null)
            {
                vision.DisableVision(0.35f);
                EnemyVisionHasVisionField?.SetValue(vision, false);

                foreach (var key in vision.VisionTriggered.Keys.ToList())
                    if (vision.VisionTriggered.ContainsKey(key))
                        vision.VisionTriggered[key] = false;

                foreach (var key in vision.VisionsTriggered.Keys.ToList())
                    if (vision.VisionsTriggered.ContainsKey(key))
                        vision.VisionsTriggered[key] = 0;
            }

            enemy.DisableChase(0.35f);
            TargetPlayerAvatarField?.SetValue(enemy, null);
            TargetPlayerViewIDField?.SetValue(enemy, 0);

            if (IsAggressiveState(enemy.CurrentState))
                enemy.CurrentState = EnemyState.Roaming;
        }

        private static bool IsAggressiveState(EnemyState state)
        {
            return state == EnemyState.ChaseBegin ||
                   state == EnemyState.Chase ||
                   state == EnemyState.ChaseSlow ||
                   state == EnemyState.ChaseEnd ||
                   state == EnemyState.Sneak ||
                   state == EnemyState.LookUnder;
        }

        internal static bool IsEnabled()
        {
            return Core.isPeacefulEnemiesEnabled && HostOnlyGuard.IsHostOnlyActive();
        }

        private static void ResetVisionForPlayer(EnemyVision vision, int playerID)
        {
            if (vision == null)
                return;

            if (vision.VisionTriggered.ContainsKey(playerID))
                vision.VisionTriggered[playerID] = false;

            if (vision.VisionsTriggered.ContainsKey(playerID))
                vision.VisionsTriggered[playerID] = 0;
        }
    }

    [HarmonyPatch]
    internal static class PeacefulEnemyReactionEvents
    {
        private static readonly HashSet<string> BlockedReactionMethodNames = new HashSet<string>
        {
            "OnVision",
            "OnVisionTrigger",
            "OnVisionTriggered",
            "VisionTriggered",
            "OnInvestigate",
            "Investigate",
            "TriggerNearby",
            "SeeTarget",
            "OnGrabbed",
            "OnTouchPlayerGrabbedObject",
            "UpdatePlayerTarget"
        };

        private static IEnumerable<MethodBase> TargetMethods()
        {
            return AccessTools.GetTypesFromAssembly(typeof(Enemy).Assembly)
                .Where(type => type.Name.StartsWith("Enemy"))
                .SelectMany(type => type.GetMethods(AccessTools.allDeclared))
                .Where(ShouldBlockMethod);
        }

        private static bool ShouldBlockMethod(MethodInfo method)
        {
            if (BlockedReactionMethodNames.Contains(method.Name))
                return true;

            if (method.Name != "SetTarget")
                return false;

            return method.GetParameters().Any(parameter => parameter.ParameterType == typeof(PlayerAvatar));
        }

        private static bool Prefix()
        {
            return !PeacefulEnemies.IsEnabled();
        }
    }

    [HarmonyPatch(typeof(HurtCollider), "PlayerHurt")]
    internal static class PeacefulEnemyHurtCollider
    {
        private static bool Prefix(HurtCollider __instance)
        {
            return !PeacefulEnemies.IsEnabled() || __instance == null || __instance.enemyHost == null;
        }
    }

    [HarmonyPatch(typeof(PlayerTumble), "HitEnemy")]
    internal static class PeacefulEnemyTumbleHit
    {
        private static bool Prefix()
        {
            return !PeacefulEnemies.IsEnabled();
        }
    }

    [HarmonyPatch(typeof(PlayerTumble), "ImpactHurtSet")]
    internal static class PeacefulEnemyImpactHurt
    {
        private static bool Prefix()
        {
            return !PeacefulEnemies.IsEnabled();
        }
    }

    [HarmonyPatch(typeof(PlayerHealth), "Hurt")]
    internal static class PeacefulEnemyPlayerHealthHurt
    {
        private static bool Prefix(int enemyIndex)
        {
            return !PeacefulEnemies.IsEnabled() || enemyIndex < 0;
        }
    }
}
