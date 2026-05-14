using UnityEngine;
using UnityEngine.InputSystem;
using HarmonyLib;

namespace UnifromCheat_REPO;

public partial class Core
{
    private const float FreecamBaseSpeed = 10f;
    private const float FreecamFastMultiplier = 3f;
    private const float FreecamMouseSensitivity = 0.08f;

    private Vector3 freecamPosition;
    private float freecamYaw;
    private float freecamPitch;
    
    private static readonly AccessTools.FieldRef<SpectateCamera, PlayerAvatar> FreecamSpectatePlayer =
        AccessTools.FieldRefAccess<SpectateCamera, PlayerAvatar>("player");

    private static readonly AccessTools.FieldRef<PlayerAvatar, bool> FreecamPlayerIsDisabled =
        AccessTools.FieldRefAccess<PlayerAvatar, bool>("isDisabled");

    private static readonly AccessTools.FieldRef<PlayerAvatar, bool> FreecamPlayerSpectating =
        AccessTools.FieldRefAccess<PlayerAvatar, bool>("spectating");

    private static readonly AccessTools.FieldRef<CameraAim, bool> FreecamCameraAimOverrideAimStop =
        AccessTools.FieldRefAccess<CameraAim, bool>("overrideAimStop");

    private static readonly AccessTools.FieldRef<GameplayManager, bool> FreecamGameplayAimInvertVertical =
        AccessTools.FieldRefAccess<GameplayManager, bool>("aimInvertVertical");

    private void UpdateFreecamHotkey()
    {
        if (!isFreecamEnabled || isHideMeActive)
        {
            DeactivateFreecam();
            return;
        }

        if (isFreecamActive && !CanUseFreecam(SpectateCamera.instance))
        {
            DeactivateFreecam();
            return;
        }

        if (Keyboard.current == null)
            return;

        if (WasFreecamHotkeyPressed())
        {
            if (isFreecamActive)
                DeactivateFreecam();
            else
                TryActivateFreecam();
        }
    }

    internal void UpdateFreecamAfterSpectateCameraLateUpdate(SpectateCamera spectateCamera)
    {
        if (!isFreecamActive)
            return;

        if (!CanUseFreecam(spectateCamera))
        {
            DeactivateFreecam();
            return;
        }

        if (!MenuState)
            UpdateFreecamMovement();

        spectateCamera.transform.SetPositionAndRotation(
            freecamPosition,
            Quaternion.Euler(freecamPitch, freecamYaw, 0f)
        );

        SemiFunc.LightManagerSetCullTargetTransform(spectateCamera.transform);
    }

    private void TryActivateFreecam()
    {
        SpectateCamera spectateCamera = SpectateCamera.instance;
        if (!CanUseFreecam(spectateCamera))
            return;

        Vector3 euler = spectateCamera.transform.rotation.eulerAngles;
        freecamPosition = spectateCamera.transform.position;
        freecamYaw = euler.y;
        freecamPitch = NormalizePitch(euler.x);
        isFreecamActive = true;

        SemiFunc.LightManagerSetCullTargetTransform(spectateCamera.transform);
        if (PlayerController.instance.playerAvatarScript.localCamera != null)
            PlayerController.instance.playerAvatarScript.localCamera.Teleported();
    }

    private void DeactivateFreecam()
    {
        if (!isFreecamActive)
            return;

        isFreecamActive = false;

        SpectateCamera spectateCamera = SpectateCamera.instance;
        if (spectateCamera == null)
            return;

        PlayerAvatar spectatedPlayer = FreecamSpectatePlayer(spectateCamera);
        if (spectatedPlayer != null)
            SemiFunc.LightManagerSetCullTargetTransform(spectatedPlayer.transform);

        if (PlayerController.instance != null &&
            PlayerController.instance.playerAvatarScript != null &&
            PlayerController.instance.playerAvatarScript.localCamera != null)
        {
            PlayerController.instance.playerAvatarScript.localCamera.Teleported();
        }
    }

    private static bool CanUseFreecam(SpectateCamera spectateCamera)
    {
        if (spectateCamera == null || !isFreecamEnabled)
            return false;

        if (isHideMeActive)
            return false;

        if (!spectateCamera.CheckState(SpectateCamera.State.Normal))
            return false;

        PlayerController controller = PlayerController.instance;
        PlayerAvatar avatar = controller != null ? controller.playerAvatarScript : null;
        return avatar != null && FreecamPlayerIsDisabled(avatar) && FreecamPlayerSpectating(avatar);
    }

    private void UpdateFreecamMovement()
    {
        Keyboard keyboard = Keyboard.current;
        Mouse mouse = Mouse.current;
        if (keyboard == null)
            return;

        if (mouse != null && !FreecamCameraAimOverrideAimStop(CameraAim.Instance))
        {
            Vector2 mouseDelta = mouse.delta.ReadValue();
            float mouseY = FreecamGameplayAimInvertVertical(GameplayManager.instance) ? mouseDelta.y : -mouseDelta.y;
            freecamYaw += mouseDelta.x * FreecamMouseSensitivity * CameraAim.Instance.AimSpeedMouse;
            freecamPitch += mouseY * FreecamMouseSensitivity * CameraAim.Instance.AimSpeedMouse;
            freecamPitch = Mathf.Clamp(freecamPitch, -89f, 89f);
        }

        Quaternion rotation = Quaternion.Euler(freecamPitch, freecamYaw, 0f);
        Vector3 direction = Vector3.zero;

        if (keyboard.wKey.isPressed) direction += Vector3.forward;
        if (keyboard.sKey.isPressed) direction += Vector3.back;
        if (keyboard.dKey.isPressed) direction += Vector3.right;
        if (keyboard.aKey.isPressed) direction += Vector3.left;
        if (keyboard.spaceKey.isPressed) direction += Vector3.up;
        if (keyboard.leftCtrlKey.isPressed || keyboard.rightCtrlKey.isPressed) direction += Vector3.down;

        if (direction.sqrMagnitude > 1f)
            direction.Normalize();

        float speed = FreecamBaseSpeed;
        if (keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed)
            speed *= FreecamFastMultiplier;

        freecamPosition += rotation * direction * (speed * Time.unscaledDeltaTime);
    }

    private static float NormalizePitch(float pitch)
    {
        if (pitch > 180f)
            pitch -= 360f;

        return Mathf.Clamp(pitch, -89f, 89f);
    }
}
