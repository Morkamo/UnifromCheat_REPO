using HarmonyLib;

namespace UnifromCheat_REPO.Patches
{
    [HarmonyPatch(typeof(PlayerController), "FixedUpdate")]
    public class SpeedHack
    {
        static void Postfix(PlayerController __instance)
        {
            __instance.MoveSpeed = Core.Instance.walkSpeed;
            __instance.SprintSpeed = Core.Instance.sprintSpeed;
            __instance.CrouchSpeed = Core.Instance.crouchSpeed;
        }
    }
}