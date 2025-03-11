using HarmonyLib;

namespace UnifromCheat_REPO.Patches
{
    [HarmonyPatch(typeof(PhysGrabObject), "FixedUpdate")]
    public class OverridePhysAttributes
    {
        static bool Prefix(PhysGrabObject __instance)
        {
            if (Core.isSuperStrengthEnabled)
            {
                __instance.OverrideMass(1);
            }
            
            if (Core.isFragilityDisabled)
            {
                __instance.OverrideFragility(0);
            }
            
            if (Core.isColliderDisabledOnGrab && __instance.grabbed)
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