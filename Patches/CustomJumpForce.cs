using HarmonyLib;

namespace UnifromCheat_REPO.Patches
{
    [HarmonyPatch(typeof(PlayerController), "FixedUpdate")]
    public class CustomJumpForce
    {
        protected static void Postfix(PlayerController __instance)
        {
            __instance.JumpForce = Core.jumpForce;
        }
    }
}