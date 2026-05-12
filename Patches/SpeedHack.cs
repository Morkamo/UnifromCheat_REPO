using System.Collections.Generic;
using HarmonyLib;

namespace UnifromCheat_REPO.Patches
{
    [HarmonyPatch(typeof(PlayerController), "FixedUpdate")]
    public class SpeedHack
    {
        private static readonly AccessTools.FieldRef<PlayerController, float> PlayerOriginalMoveSpeed =
            AccessTools.FieldRefAccess<PlayerController, float>("playerOriginalMoveSpeed");
        private static readonly AccessTools.FieldRef<PlayerController, float> PlayerOriginalSprintSpeed =
            AccessTools.FieldRefAccess<PlayerController, float>("playerOriginalSprintSpeed");
        private static readonly AccessTools.FieldRef<PlayerController, float> PlayerOriginalCrouchSpeed =
            AccessTools.FieldRefAccess<PlayerController, float>("playerOriginalCrouchSpeed");

        private sealed class SpeedSnapshot
        {
            public bool WasEnabled;
        }

        private static readonly Dictionary<PlayerController, SpeedSnapshot> Snapshots = new Dictionary<PlayerController, SpeedSnapshot>();

        protected static void Postfix(PlayerController __instance)
        {
            if (__instance == null)
                return;

            if (!Snapshots.TryGetValue(__instance, out SpeedSnapshot snapshot))
            {
                snapshot = new SpeedSnapshot();
                Snapshots[__instance] = snapshot;
            }

            if (!Core.isSpeedHackEnabled)
            {
                if (snapshot.WasEnabled)
                    RestoreSpeed(__instance);

                snapshot.WasEnabled = false;
                return;
            }

            snapshot.WasEnabled = true;
            __instance.MoveSpeed = Core.walkSpeed;
            __instance.SprintSpeed = Core.sprintSpeed;
            __instance.CrouchSpeed = Core.crouchSpeed;
        }

        private static void RestoreSpeed(PlayerController controller)
        {
            controller.MoveSpeed = PlayerOriginalMoveSpeed(controller);
            controller.SprintSpeed = PlayerOriginalSprintSpeed(controller);
            controller.CrouchSpeed = PlayerOriginalCrouchSpeed(controller);
        }
    }
}
