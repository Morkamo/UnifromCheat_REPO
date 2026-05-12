using HarmonyLib;
using UnifromCheat_REPO.Utils;
using UnityEngine;

namespace UnifromCheat_REPO.Patches
{
    [HarmonyPatch(typeof(PlayerController))]
    internal static class TumbleBypassPlayerControllerPatch
    {
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void UpdatePostfix(PlayerController __instance)
        {
            ResetLocalInputLocks(__instance);
        }

        [HarmonyPatch("FixedUpdate")]
        [HarmonyPostfix]
        private static void FixedUpdatePostfix(PlayerController __instance)
        {
            ResetLocalInputLocks(__instance);
        }

        private static void ResetLocalInputLocks(PlayerController controller)
        {
            if (!Core.isTumbleBypassEnabled || controller == null || controller != PlayerController.instance)
                return;

            PlayerControllerRefs.tumbleInputDisableTimer(controller) = 0f;
        }
    }

    [HarmonyPatch(typeof(PlayerTumble))]
    internal static class TumbleBypassPlayerTumblePatch
    {
        private static readonly System.Reflection.MethodInfo BreakFreeMethod =
            AccessTools.Method(typeof(PlayerTumble), "BreakFree");

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void UpdatePostfix(PlayerTumble __instance)
        {
            ResetLocalTumbleCooldowns(__instance);
            TryNetworkEscape(__instance);
        }

        [HarmonyPatch("TumbleSetRPC")]
        [HarmonyPostfix]
        private static void TumbleSetRpcPostfix(PlayerTumble __instance)
        {
            ResetLocalTumbleCooldowns(__instance);
        }

        private static void ResetLocalTumbleCooldowns(PlayerTumble tumble)
        {
            if (!Core.isTumbleBypassEnabled || !IsLocalPlayerTumble(tumble))
                return;

            PlayerTumbleRefs.breakFreeCooldown(tumble) = 0f;

            PlayerController controller = PlayerController.instance;
            if (controller != null)
                PlayerControllerRefs.tumbleInputDisableTimer(controller) = 0f;
        }

        private static bool IsLocalPlayerTumble(PlayerTumble tumble)
        {
            if (tumble == null)
                return false;

            PlayerController controller = PlayerController.instance;
            return controller != null && controller.playerAvatarScript == PlayerTumbleRefs.playerAvatar(tumble);
        }

        private static void TryNetworkEscape(PlayerTumble tumble)
        {
            if (!Core.isTumbleBypassEnabled || !IsLocalPlayerTumble(tumble))
                return;

            bool wantsOut = SemiFunc.InputDown(InputKey.Crouch) || SemiFunc.InputDown(InputKey.Jump);
            if (!wantsOut)
                return;

            bool isForced = PlayerTumbleRefs.tumbleOverride(tumble) || PlayerTumbleRefs.tumbleOverrideTimer(tumble) > 0f;
            PhysGrabObject physGrabObject = PlayerTumbleRefs.physGrabObject(tumble);
            bool isGrabbed = physGrabObject != null && physGrabObject.playerGrabbing.Count > 0;
            if (!isForced && !isGrabbed)
                return;

            PlayerTumbleRefs.tumbleOverrideTimer(tumble) = 0f;
            if (isForced)
                tumble.TumbleOverride(false);

            if (isGrabbed)
            {
                Vector3 escapeDirection = GetEscapeDirection(tumble);
                tumble.TumbleForce(escapeDirection * 15f);
                tumble.TumbleTorque(tumble.transform.right * 10f);
                BreakFreeMethod.Invoke(tumble, new object[] { escapeDirection });
            }

            tumble.TumbleRequest(false, true);
            PlayerTumbleRefs.breakFreeCooldown(tumble) = 0f;
        }

        private static Vector3 GetEscapeDirection(PlayerTumble tumble)
        {
            PlayerAvatar avatar = PlayerTumbleRefs.playerAvatar(tumble);
            Transform cameraTransform = avatar != null && avatar.localCamera != null
                ? avatar.localCamera.GetOverrideTransform()
                : null;

            return cameraTransform != null ? cameraTransform.forward : tumble.transform.forward;
        }
    }

    [HarmonyPatch(typeof(PhysGrabber))]
    internal static class TumbleBypassPhysGrabberPatch
    {
        [HarmonyPatch("OverrideDisableSpecialGrabPowers")]
        [HarmonyPrefix]
        private static bool OverrideDisableSpecialGrabPowersPrefix(PhysGrabber __instance)
        {
            if (!Core.isTumbleBypassEnabled || !IsLocalGrabber(__instance))
                return true;

            PhysGrabberRefs.overrideDisableSpecialGrabPowersTimer(__instance) = 0f;
            PhysGrabberRefs.overrideDisableSpecialGrabPowers(__instance) = false;
            return false;
        }

        [HarmonyPatch("OverrideSpecialPowersLogic")]
        [HarmonyPostfix]
        private static void OverrideSpecialPowersLogicPostfix(PhysGrabber __instance)
        {
            if (!Core.isTumbleBypassEnabled || !IsLocalGrabber(__instance))
                return;

            PhysGrabberRefs.overrideDisableSpecialGrabPowersTimer(__instance) = 0f;
            PhysGrabberRefs.overrideDisableSpecialGrabPowers(__instance) = false;
        }

        private static bool IsLocalGrabber(PhysGrabber grabber)
        {
            return grabber != null && PhysGrabberRefs.isLocal(grabber);
        }
    }
}
