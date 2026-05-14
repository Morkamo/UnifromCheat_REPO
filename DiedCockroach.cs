using HarmonyLib;
using Photon.Pun;
using UnifromCheat_REPO.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnifromCheat_REPO;

public partial class Core
{
    private const float DiedCockroachJumpImpulse = 2.2f;
    private static readonly AccessTools.FieldRef<PlayerAvatar, PlayerDeathHead> DiedCockroachDeathHead =
        AccessTools.FieldRefAccess<PlayerAvatar, PlayerDeathHead>("playerDeathHead");
    private static readonly AccessTools.FieldRef<PlayerDeathHead, PhysGrabObject> DiedCockroachPhysGrabObject =
        AccessTools.FieldRefAccess<PlayerDeathHead, PhysGrabObject>("physGrabObject");
    private static readonly AccessTools.FieldRef<PlayerDeathHead, bool> DiedCockroachSpectated =
        AccessTools.FieldRefAccess<PlayerDeathHead, bool>("spectated");
    private static readonly AccessTools.FieldRef<PlayerAvatar, bool> DiedCockroachAvatarIsDisabled =
        AccessTools.FieldRefAccess<PlayerAvatar, bool>("isDisabled");
    private static readonly AccessTools.FieldRef<PhysGrabObject, PhotonTransformView> DiedCockroachPhotonTransformView =
        AccessTools.FieldRefAccess<PhysGrabObject, PhotonTransformView>("photonTransformView");

    private bool diedCockroachJumpHeld;
    private PhysGrabObject diedCockroachNoclipPhysGrabObject;
    private Rigidbody diedCockroachNoclipRb;
    private PhotonTransformView diedCockroachNoclipPhotonTransformView;
    private bool diedCockroachNoclipStoredGravity;
    private bool diedCockroachNoclipStoredKinematic;

    private void OnDestroy()
    {
        ResetHeadNoclipState();
    }

    private void FixedUpdateDiedCockroach()
    {
        if (!isDiedCockroachEnabled || Patches.Noclip.isActive || !HostOnlyGuard.IsHostOnlyActive())
            return;

        if (!TryGetActiveHeadPhys(out _, out PhysGrabObject physGrabObject, out Rigidbody rb))
            return;

        Vector3 force = GetHeadMovementDirection();
        if (force.sqrMagnitude > 1f)
            force.Normalize();

        bool jump = TryConsumeHeadJump();
        if (force.sqrMagnitude <= 0.001f && !jump)
            return;

        ApplyDiedCockroachMovement(physGrabObject, rb, force, jump, diedCockroachForce);
    }

    private static void ApplyDiedCockroachMovement(PhysGrabObject physGrabObject, Rigidbody rb, Vector3 force, bool jump, float forceMultiplier)
    {
        if (force.sqrMagnitude > 0.001f)
            rb.AddForce(force * forceMultiplier, ForceMode.Acceleration);

        if (jump)
            rb.AddForce(Vector3.up * DiedCockroachJumpImpulse, ForceMode.Impulse);

        rb.WakeUp();
        physGrabObject.OverrideTorqueStrength(0f);
    }

    internal void UpdateHeadNoclip()
    {
        if (!Patches.Noclip.isActive || !TryGetActiveHeadPhys(out _, out PhysGrabObject physGrabObject, out Rigidbody rb))
        {
            ResetHeadNoclipState();
            return;
        }

        if (diedCockroachNoclipRb != rb)
        {
            ResetHeadNoclipState();
            diedCockroachNoclipPhysGrabObject = physGrabObject;
            diedCockroachNoclipRb = rb;
            diedCockroachNoclipStoredGravity = rb.useGravity;
            diedCockroachNoclipStoredKinematic = rb.isKinematic;
            diedCockroachNoclipPhotonTransformView = DiedCockroachPhotonTransformView(physGrabObject);
        }

        if (diedCockroachNoclipPhotonTransformView != null)
            diedCockroachNoclipPhotonTransformView.enabled = false;

        rb.useGravity = false;
        rb.isKinematic = true;

        Vector3 direction = GetHeadMovementDirection();
        if (direction.sqrMagnitude > 1f)
            direction.Normalize();

        Vector3 position = physGrabObject.transform.position;
        if (direction.sqrMagnitude > 0.001f)
            position += direction * (noclipSpeed * Time.deltaTime * 5f);

        physGrabObject.transform.position = position;
        rb.position = position;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        physGrabObject.Teleport(position, physGrabObject.transform.rotation);
    }

    internal bool IsNoclipControllingHead()
    {
        return Patches.Noclip.isActive && TryGetActiveHeadPhys(out _, out _, out _);
    }

    internal void ResetHeadNoclipState()
    {
        if (diedCockroachNoclipRb == null)
            return;

        if (diedCockroachNoclipPhysGrabObject != null)
            diedCockroachNoclipPhysGrabObject.Teleport(diedCockroachNoclipPhysGrabObject.transform.position,
                diedCockroachNoclipPhysGrabObject.transform.rotation);

        diedCockroachNoclipRb.useGravity = diedCockroachNoclipStoredGravity;
        diedCockroachNoclipRb.isKinematic = diedCockroachNoclipStoredKinematic;
        diedCockroachNoclipRb.velocity = Vector3.zero;
        if (diedCockroachNoclipPhotonTransformView != null)
            diedCockroachNoclipPhotonTransformView.enabled = true;

        diedCockroachNoclipPhysGrabObject = null;
        diedCockroachNoclipRb = null;
        diedCockroachNoclipPhotonTransformView = null;
    }

    private static bool TryGetActiveHeadPhys(out PlayerDeathHead deathHead, out PhysGrabObject physGrabObject, out Rigidbody rb)
    {
        deathHead = null;
        physGrabObject = null;
        rb = null;

        PlayerAvatar avatar = PlayerController.instance != null ? PlayerController.instance.playerAvatarScript : PlayerAvatar.instance;
        if (avatar == null || !DiedCockroachAvatarIsDisabled(avatar))
            return false;

        SpectateCamera spectateCamera = SpectateCamera.instance;
        if (spectateCamera == null || !spectateCamera.CheckState(SpectateCamera.State.Head))
            return false;

        deathHead = DiedCockroachDeathHead(avatar);
        if (deathHead == null || !DiedCockroachSpectated(deathHead))
            return false;

        physGrabObject = DiedCockroachPhysGrabObject(deathHead);
        rb = physGrabObject != null ? physGrabObject.rb : null;
        return rb != null;
    }

    private static Vector3 GetHeadMovementDirection()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
            return Vector3.zero;

        Transform cameraTransform = GetHeadCameraTransform();
        Vector3 forward = cameraTransform != null ? cameraTransform.forward : Vector3.forward;
        Vector3 right = cameraTransform != null ? cameraTransform.right : Vector3.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 direction = Vector3.zero;
        if (keyboard.wKey.isPressed) direction += forward;
        if (keyboard.sKey.isPressed) direction -= forward;
        if (keyboard.dKey.isPressed) direction += right;
        if (keyboard.aKey.isPressed) direction -= right;

        if (Patches.Noclip.isActive)
        {
            if (keyboard.spaceKey.isPressed) direction += Vector3.up;
            if (keyboard.leftCtrlKey.isPressed || keyboard.rightCtrlKey.isPressed) direction += Vector3.down;
        }

        return direction;
    }

    private bool TryConsumeHeadJump()
    {
        Keyboard keyboard = Keyboard.current;
        bool pressed = keyboard != null && keyboard.spaceKey.isPressed;
        bool shouldJump = pressed && !diedCockroachJumpHeld;
        diedCockroachJumpHeld = pressed;
        return shouldJump;
    }

    private static Transform GetHeadCameraTransform()
    {
        Camera camera = Camera.main;
        if (camera != null)
            return camera.transform;

        return SpectateCamera.instance != null ? SpectateCamera.instance.transform : null;
    }
}
