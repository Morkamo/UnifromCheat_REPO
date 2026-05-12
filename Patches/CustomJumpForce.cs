using System.Collections.Generic;
using HarmonyLib;

namespace UnifromCheat_REPO.Patches
{
    [HarmonyPatch(typeof(PlayerController), "FixedUpdate")]
    public class CustomJumpForce
    {
        private sealed class JumpSnapshot
        {
            public bool Captured;
            public bool WasEnabled;
            public float BaseJumpForce;
        }

        private static readonly Dictionary<PlayerController, JumpSnapshot> Snapshots = new Dictionary<PlayerController, JumpSnapshot>();

        protected static void Prefix(PlayerController __instance)
        {
            if (__instance == null)
                return;

            if (!Snapshots.TryGetValue(__instance, out JumpSnapshot snapshot))
            {
                snapshot = new JumpSnapshot();
                Snapshots[__instance] = snapshot;
            }

            if (!snapshot.Captured || (!snapshot.WasEnabled && !Core.isCustomJumpForceEnabled))
            {
                snapshot.BaseJumpForce = __instance.JumpForce;
                snapshot.Captured = true;
            }
        }

        protected static void Postfix(PlayerController __instance)
        {
            if (__instance == null)
                return;

            if (!Snapshots.TryGetValue(__instance, out JumpSnapshot snapshot))
            {
                snapshot = new JumpSnapshot
                {
                    Captured = true,
                    BaseJumpForce = __instance.JumpForce
                };
                Snapshots[__instance] = snapshot;
            }

            if (!Core.isCustomJumpForceEnabled)
            {
                if (snapshot.WasEnabled)
                    __instance.JumpForce = snapshot.BaseJumpForce;

                snapshot.WasEnabled = false;
                return;
            }

            snapshot.WasEnabled = true;
            __instance.JumpForce = Core.jumpForce;
        }
    }
}
