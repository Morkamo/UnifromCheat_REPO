using System.Reflection;
using HarmonyLib;

namespace UnifromCheat_REPO.Patches
{
    [HarmonyPatch(typeof(FlashlightController), "Update")]
    internal static class PermanentFlashlightPatch
    {
        private static readonly FieldInfo HideFlashlightField = AccessTools.Field(typeof(FlashlightController), "hideFlashlight");
        private static readonly FieldInfo ActiveField = AccessTools.Field(typeof(FlashlightController), "active");
        private static readonly FieldInfo CurrentStateField = AccessTools.Field(typeof(FlashlightController), "currentState");
        private static readonly FieldInfo StateTimerField = AccessTools.Field(typeof(FlashlightController), "stateTimer");
        private static readonly FieldInfo HiddenScaleField = AccessTools.Field(typeof(FlashlightController), "hiddenScale");
        private static readonly FieldInfo IsCrouchingField = AccessTools.Field(typeof(PlayerAvatar), "isCrouching");
        private static readonly FieldInfo IsTumblingField = AccessTools.Field(typeof(PlayerAvatar), "isTumbling");
        private static readonly FieldInfo IsSlidingField = AccessTools.Field(typeof(PlayerAvatar), "isSliding");

        private struct FlashlightActionState
        {
            public PlayerAvatar Avatar;
            public bool IsCrouching;
            public bool IsTumbling;
            public bool IsSliding;
        }

        private static void Prefix(FlashlightController __instance, ref FlashlightActionState __state)
        {
            if (!Core.isFlashlightSettingsEnabled || !Core.isPermanentFlashlightEnabled || !IsLocalFlashlight(__instance))
                return;

            PlayerAvatar avatar = __instance.PlayerAvatar;
            __state = new FlashlightActionState
            {
                Avatar = avatar,
                IsCrouching = GetBool(IsCrouchingField, avatar),
                IsTumbling = GetBool(IsTumblingField, avatar),
                IsSliding = GetBool(IsSlidingField, avatar)
            };

            HideFlashlightField?.SetValue(__instance, false);
            ActiveField?.SetValue(__instance, true);

            SetBool(IsCrouchingField, avatar, false);
            SetBool(IsTumblingField, avatar, false);
            SetBool(IsSlidingField, avatar, false);
        }

        private static void Postfix(FlashlightController __instance, FlashlightActionState __state)
        {
            if (__state.Avatar == null)
                return;

            SetBool(IsCrouchingField, __state.Avatar, __state.IsCrouching);
            SetBool(IsTumblingField, __state.Avatar, __state.IsTumbling);
            SetBool(IsSlidingField, __state.Avatar, __state.IsSliding);

            if (!Core.isFlashlightSettingsEnabled || !Core.isPermanentFlashlightEnabled || !IsLocalFlashlight(__instance))
                return;

            if (IsHidingState(__instance))
            {
                ActiveField?.SetValue(__instance, true);
                StateTimerField?.SetValue(__instance, 0f);
                HiddenScaleField?.SetValue(__instance, 1f);

                if (CurrentStateField != null)
                    CurrentStateField.SetValue(__instance, StateValue(3));
            }

            __instance.LightActive = true;

            if (__instance.mesh != null)
                __instance.mesh.enabled = true;
            if (__instance.meshShadows != null)
                __instance.meshShadows.enabled = true;
            if (__instance.spotlight != null)
                __instance.spotlight.enabled = true;
            if (__instance.halo != null)
                __instance.halo.enabled = true;
        }

        private static bool IsLocalFlashlight(FlashlightController flashlight)
        {
            try
            {
                return flashlight != null &&
                       PlayerController.instance != null &&
                       PlayerController.instance.playerAvatarScript == flashlight.PlayerAvatar;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsHidingState(FlashlightController flashlight)
        {
            if (CurrentStateField == null)
                return false;

            int state = (int)CurrentStateField.GetValue(flashlight);
            return state == 0 || state == 4 || state == 5;
        }

        private static object StateValue(int value)
        {
            return System.Enum.ToObject(CurrentStateField.FieldType, value);
        }

        private static bool GetBool(FieldInfo field, object instance)
        {
            return field != null && instance != null && (bool)field.GetValue(instance);
        }

        private static void SetBool(FieldInfo field, object instance, bool value)
        {
            field?.SetValue(instance, value);
        }
    }
}
