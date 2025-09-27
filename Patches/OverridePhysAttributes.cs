using HarmonyLib;

namespace UnifromCheat_REPO.Patches
{
    [HarmonyPatch(typeof(PhysGrabObject), "FixedUpdate")]
    public class OverridePhysAttributes
    {
        protected static bool Prefix(PhysGrabObject __instance)
        {
            if (Core.isLiteItemsModeEnabled)
            {
                __instance.OverrideMass(1);
            }
            
            if (Core.isFragilityDisabled)
            {
                __instance.OverrideFragility(0);
            }
            
            if (Core.isGhostItemsMode && __instance.grabbed && __instance.rb.GetComponentInParent<ValuableObject>())
            {
                __instance.rb.detectCollisions = false;
            }
            else
            {
                __instance.rb.detectCollisions = true;
            }

            return true;
        }
    }
}