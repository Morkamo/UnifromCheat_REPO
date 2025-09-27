using HarmonyLib;

namespace UnifromCheat_REPO.Patches
{
    [HarmonyPatch(typeof(PlayerController), "FixedUpdate")]
    public class SpeedHack
    {
        protected static void Postfix(PlayerController __instance)
        {
            __instance.MoveSpeed = Core.walkSpeed;
            __instance.SprintSpeed = Core.sprintSpeed;
            __instance.CrouchSpeed = Core.crouchSpeed;
        }
    }
}