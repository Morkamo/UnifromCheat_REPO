using HarmonyLib;
using UnifromCheat_REPO.Utils;

namespace UnifromCheat_REPO.Patches
{
    [HarmonyPatch(typeof(PhysGrabObject), "FixedUpdate")]
    public class OverridePhysAttributes
    {
        protected static bool Prefix(PhysGrabObject __instance)
        {
            bool hostOnlyActive = HostOnlyGuard.IsHostOnlyActive();

            if (Core.isLiteItemsModeEnabled && hostOnlyActive)
            {
                __instance.OverrideMass(1);
            }

            if (Core.isFragilityDisabled && hostOnlyActive)
            {
                __instance.OverrideFragility(0);
            }

            if (Core.isGhostItemsMode && hostOnlyActive && __instance.grabbed && IsGhostItemTarget(__instance.rb))
            {
                __instance.rb.detectCollisions = false;
            }
            else
            {
                __instance.rb.detectCollisions = true;
            }

            return true;
        }

        private static bool IsGhostItemTarget(UnityEngine.Rigidbody rb)
        {
            if (rb == null)
                return false;

            return rb.GetComponentInParent<ValuableObject>() ||
                   rb.GetComponentInParent<CosmeticWorldObject>();
        }
    }
}
