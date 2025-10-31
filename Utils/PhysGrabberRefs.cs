using System.Collections.Generic;
using HarmonyLib;
using Photon.Pun;
using UnityEngine;

namespace UnifromCheat_REPO.Utils
{
    internal static class PhysGrabberRefs
    {
        // === приватные / internal ===
        public static readonly AccessTools.FieldRef<PhysGrabber, Camera> playerCamera =
            AccessTools.FieldRefAccess<PhysGrabber, Camera>("playerCamera");

        public static readonly AccessTools.FieldRef<PhysGrabber, float> minDistanceFromPlayerOriginal =
            AccessTools.FieldRefAccess<PhysGrabber, float>("minDistanceFromPlayerOriginal");

        public static readonly AccessTools.FieldRef<PhysGrabber, PhysGrabBeam> physGrabBeamScript =
            AccessTools.FieldRefAccess<PhysGrabber, PhysGrabBeam>("physGrabBeamScript");

        public static readonly AccessTools.FieldRef<PhysGrabber, GameObject> physGrabPointVisual1 =
            AccessTools.FieldRefAccess<PhysGrabber, GameObject>("physGrabPointVisual1");
        public static readonly AccessTools.FieldRef<PhysGrabber, GameObject> physGrabPointVisual2 =
            AccessTools.FieldRefAccess<PhysGrabber, GameObject>("physGrabPointVisual2");

        public static readonly AccessTools.FieldRef<PhysGrabber, bool> physGrabBeamActive =
            AccessTools.FieldRefAccess<PhysGrabber, bool>("physGrabBeamActive");

        public static readonly AccessTools.FieldRef<PhysGrabber, Transform> physGrabPointVisualRotate =
            AccessTools.FieldRefAccess<PhysGrabber, Transform>("physGrabPointVisualRotate");
        public static readonly AccessTools.FieldRef<PhysGrabber, List<GameObject>> physGrabPointVisualGridObjects =
            AccessTools.FieldRefAccess<PhysGrabber, List<GameObject>>("physGrabPointVisualGridObjects");

        public static readonly AccessTools.FieldRef<PhysGrabber, float> overrideMinimumGrabDistance =
            AccessTools.FieldRefAccess<PhysGrabber, float>("overrideMinimumGrabDistance");
        public static readonly AccessTools.FieldRef<PhysGrabber, float> overrideMinimumGrabDistanceTimer =
            AccessTools.FieldRefAccess<PhysGrabber, float>("overrideMinimumGrabDistanceTimer");

        public static readonly AccessTools.FieldRef<PhysGrabber, int> prevColorState =
            AccessTools.FieldRefAccess<PhysGrabber, int>("prevColorState");
        public static readonly AccessTools.FieldRef<PhysGrabber, float> colorStateOverrideTimer =
            AccessTools.FieldRefAccess<PhysGrabber, float>("colorStateOverrideTimer");

        /*public static readonly AccessTools.FieldRef<PhysGrabber, bool> overrideGrab =
            AccessTools.FieldRefAccess<PhysGrabber, bool>("overrideGrab");*/
        public static readonly AccessTools.FieldRef<PhysGrabber, bool> overrideGrabRelease =
            AccessTools.FieldRefAccess<PhysGrabber, bool>("overrideGrabRelease");
        public static readonly AccessTools.FieldRef<PhysGrabber, PhysGrabObject> overrideGrabTarget =
            AccessTools.FieldRefAccess<PhysGrabber, PhysGrabObject>("overrideGrabTarget");

        public static readonly AccessTools.FieldRef<PhysGrabber, float> physGrabBeamAlpha =
            AccessTools.FieldRefAccess<PhysGrabber, float>("physGrabBeamAlpha");
        public static readonly AccessTools.FieldRef<PhysGrabber, float> physGrabBeamAlphaChangeTo =
            AccessTools.FieldRefAccess<PhysGrabber, float>("physGrabBeamAlphaChangeTo");
        public static readonly AccessTools.FieldRef<PhysGrabber, float> physGramBeamAlphaTimer =
            AccessTools.FieldRefAccess<PhysGrabber, float>("physGramBeamAlphaTimer");
        public static readonly AccessTools.FieldRef<PhysGrabber, float> physGrabBeamAlphaChangeProgress =
            AccessTools.FieldRefAccess<PhysGrabber, float>("physGrabBeamAlphaChangeProgress");
        public static readonly AccessTools.FieldRef<PhysGrabber, float> physGrabBeamAlphaOriginal =
            AccessTools.FieldRefAccess<PhysGrabber, float>("physGrabBeamAlphaOriginal");

        public static readonly AccessTools.FieldRef<PhysGrabber, float> overrideGrabDistance =
            AccessTools.FieldRefAccess<PhysGrabber, float>("overrideGrabDistance");
        public static readonly AccessTools.FieldRef<PhysGrabber, float> overrideGrabDistanceTimer =
            AccessTools.FieldRefAccess<PhysGrabber, float>("overrideGrabDistanceTimer");

        public static readonly AccessTools.FieldRef<PhysGrabber, float> overrideDisableRotationControlsTimer =
            AccessTools.FieldRefAccess<PhysGrabber, float>("overrideDisableRotationControlsTimer");
        public static readonly AccessTools.FieldRef<PhysGrabber, bool> overrideDisableRotationControls =
            AccessTools.FieldRefAccess<PhysGrabber, bool>("overrideDisableRotationControls");

        public static readonly AccessTools.FieldRef<PhysGrabber, LayerMask> mask =
            AccessTools.FieldRefAccess<PhysGrabber, LayerMask>("mask");
        public static readonly AccessTools.FieldRef<PhysGrabber, float> grabCheckTimer =
            AccessTools.FieldRefAccess<PhysGrabber, float>("grabCheckTimer");

        public static readonly AccessTools.FieldRef<PhysGrabber, float> physRotatingTimer =
            AccessTools.FieldRefAccess<PhysGrabber, float>("physRotatingTimer");
        public static readonly AccessTools.FieldRef<PhysGrabber, Quaternion> physRotationBase =
            AccessTools.FieldRefAccess<PhysGrabber, Quaternion>("physRotationBase");

        public static readonly AccessTools.FieldRef<PhysGrabber, float> isRotatingTimer =
            AccessTools.FieldRefAccess<PhysGrabber, float>("isRotatingTimer");
        public static readonly AccessTools.FieldRef<PhysGrabber, float> isPushingTimer =
            AccessTools.FieldRefAccess<PhysGrabber, float>("isPushingTimer");
        public static readonly AccessTools.FieldRef<PhysGrabber, float> isPullingTimer =
            AccessTools.FieldRefAccess<PhysGrabber, float>("isPullingTimer");
        public static readonly AccessTools.FieldRef<PhysGrabber, float> prevPullerDistance =
            AccessTools.FieldRefAccess<PhysGrabber, float>("prevPullerDistance");

        public static readonly AccessTools.FieldRef<PhysGrabber, bool> toggleGrab =
            AccessTools.FieldRefAccess<PhysGrabber, bool>("toggleGrab");
        public static readonly AccessTools.FieldRef<PhysGrabber, float> toggleGrabTimer =
            AccessTools.FieldRefAccess<PhysGrabber, float>("toggleGrabTimer");

        public static readonly AccessTools.FieldRef<PhysGrabber, float> overrideGrabPointTimer =
            AccessTools.FieldRefAccess<PhysGrabber, float>("overrideGrabPointTimer");
        public static readonly AccessTools.FieldRef<PhysGrabber, Transform> overrideGrabPointTransform =
            AccessTools.FieldRefAccess<PhysGrabber, Transform>("overrideGrabPointTransform");

        public static readonly AccessTools.FieldRef<PhysGrabber, float> physGrabBeamOverChargeAmount =
            AccessTools.FieldRefAccess<PhysGrabber, float>("physGrabBeamOverChargeAmount");
        public static readonly AccessTools.FieldRef<PhysGrabber, float> physGrabBeamOverChargeTimer =
            AccessTools.FieldRefAccess<PhysGrabber, float>("physGrabBeamOverChargeTimer");
        public static readonly AccessTools.FieldRef<PhysGrabber, float> physGrabBeamOverchargeDecreaseCooldown =
            AccessTools.FieldRefAccess<PhysGrabber, float>("physGrabBeamOverchargeDecreaseCooldown");
        public static readonly AccessTools.FieldRef<PhysGrabber, float> physGrabBeamOverchargeInitialBoostCooldown =
            AccessTools.FieldRefAccess<PhysGrabber, float>("physGrabBeamOverchargeInitialBoostCooldown");

        public static readonly AccessTools.FieldRef<PhysGrabber, float> test =
            AccessTools.FieldRefAccess<PhysGrabber, float>("test");

        // === публичные + [HideInInspector] ===
        public static readonly AccessTools.FieldRef<PhysGrabber, float> grabRange =
            AccessTools.FieldRefAccess<PhysGrabber, float>("grabRange");
        public static readonly AccessTools.FieldRef<PhysGrabber, float> grabReleaseDistance =
            AccessTools.FieldRefAccess<PhysGrabber, float>("grabReleaseDistance");
        public static readonly AccessTools.FieldRef<PhysGrabber, float> minDistanceFromPlayer =
            AccessTools.FieldRefAccess<PhysGrabber, float>("minDistanceFromPlayer");
        public static readonly AccessTools.FieldRef<PhysGrabber, float> maxDistanceFromPlayer =
            AccessTools.FieldRefAccess<PhysGrabber, float>("maxDistanceFromPlayer");

        public static readonly AccessTools.FieldRef<PhysGrabber, PhysGrabBeam> physGrabBeamComponent =
            AccessTools.FieldRefAccess<PhysGrabber, PhysGrabBeam>("physGrabBeamComponent");
        public static readonly AccessTools.FieldRef<PhysGrabber, GameObject> physGrabBeam =
            AccessTools.FieldRefAccess<PhysGrabber, GameObject>("physGrabBeam");

        public static readonly AccessTools.FieldRef<PhysGrabber, Transform> physGrabPoint =
            AccessTools.FieldRefAccess<PhysGrabber, Transform>("physGrabPoint");
        public static readonly AccessTools.FieldRef<PhysGrabber, Transform> physGrabPointPuller =
            AccessTools.FieldRefAccess<PhysGrabber, Transform>("physGrabPointPuller");
        public static readonly AccessTools.FieldRef<PhysGrabber, Transform> physGrabPointPlane =
            AccessTools.FieldRefAccess<PhysGrabber, Transform>("physGrabPointPlane");

        public static readonly AccessTools.FieldRef<PhysGrabber, PhysGrabObject> grabbedPhysGrabObject =
            AccessTools.FieldRefAccess<PhysGrabber, PhysGrabObject>("grabbedPhysGrabObject");
        public static readonly AccessTools.FieldRef<PhysGrabber, int> grabbedPhysGrabObjectColliderID =
            AccessTools.FieldRefAccess<PhysGrabber, int>("grabbedPhysGrabObjectColliderID");
        public static readonly AccessTools.FieldRef<PhysGrabber, Collider> grabbedPhysGrabObjectCollider =
            AccessTools.FieldRefAccess<PhysGrabber, Collider>("grabbedPhysGrabObjectCollider");
        public static readonly AccessTools.FieldRef<PhysGrabber, StaticGrabObject> grabbedStaticGrabObject =
            AccessTools.FieldRefAccess<PhysGrabber, StaticGrabObject>("grabbedStaticGrabObject");
        public static readonly AccessTools.FieldRef<PhysGrabber, Rigidbody> grabbedObject =
            AccessTools.FieldRefAccess<PhysGrabber, Rigidbody>("grabbedObject");
        public static readonly AccessTools.FieldRef<PhysGrabber, Transform> grabbedObjectTransform =
            AccessTools.FieldRefAccess<PhysGrabber, Transform>("grabbedObjectTransform");

        public static readonly AccessTools.FieldRef<PhysGrabber, PhotonView> photonView =
            AccessTools.FieldRefAccess<PhysGrabber, PhotonView>("photonView");

        public static readonly AccessTools.FieldRef<PhysGrabber, bool> isLocal =
            AccessTools.FieldRefAccess<PhysGrabber, bool>("isLocal");
        public static readonly AccessTools.FieldRef<PhysGrabber, bool> grabbed =
            AccessTools.FieldRefAccess<PhysGrabber, bool>("grabbed");

        public static readonly AccessTools.FieldRef<PhysGrabber, float> grabDisableTimer =
            AccessTools.FieldRefAccess<PhysGrabber, float>("grabDisableTimer");

        public static readonly AccessTools.FieldRef<PhysGrabber, PlayerAvatar> playerAvatar =
            AccessTools.FieldRefAccess<PhysGrabber, PlayerAvatar>("playerAvatar");

        public static readonly AccessTools.FieldRef<PhysGrabber, Sound> startSound =
            AccessTools.FieldRefAccess<PhysGrabber, Sound>("startSound");
        public static readonly AccessTools.FieldRef<PhysGrabber, Sound> loopSound =
            AccessTools.FieldRefAccess<PhysGrabber, Sound>("loopSound");
        public static readonly AccessTools.FieldRef<PhysGrabber, Sound> stopSound =
            AccessTools.FieldRefAccess<PhysGrabber, Sound>("stopSound");

        public static readonly AccessTools.FieldRef<PhysGrabber, Material> physGrabBeamMaterial =
            AccessTools.FieldRefAccess<PhysGrabber, Material>("physGrabBeamMaterial");
        public static readonly AccessTools.FieldRef<PhysGrabber, Material> physGrabBeamMaterialBatteryCharge =
            AccessTools.FieldRefAccess<PhysGrabber, Material>("physGrabBeamMaterialBatteryCharge");

        // === оставшиеся поля ===
        /*public static readonly AccessTools.FieldRef<PhysGrabber, bool> physGrabForcesDisabled =
            AccessTools.FieldRefAccess<PhysGrabber, bool>("physGrabForcesDisabled");*/
        public static readonly AccessTools.FieldRef<PhysGrabber, float> initialPressTimer =
            AccessTools.FieldRefAccess<PhysGrabber, float>("initialPressTimer");
        public static readonly AccessTools.FieldRef<PhysGrabber, bool> debugStickyGrabber =
            AccessTools.FieldRefAccess<PhysGrabber, bool>("debugStickyGrabber");
        public static readonly AccessTools.FieldRef<PhysGrabber, float> stopRotationTimer =
            AccessTools.FieldRefAccess<PhysGrabber, float>("stopRotationTimer");
        public static readonly AccessTools.FieldRef<PhysGrabber, Quaternion> nextPhysRotation =
            AccessTools.FieldRefAccess<PhysGrabber, Quaternion>("nextPhysRotation");
        public static readonly AccessTools.FieldRef<PhysGrabber, bool> isRotating =
            AccessTools.FieldRefAccess<PhysGrabber, bool>("isRotating");
        public static readonly AccessTools.FieldRef<PhysGrabber, bool> isPushing =
            AccessTools.FieldRefAccess<PhysGrabber, bool>("isPushing");
        public static readonly AccessTools.FieldRef<PhysGrabber, bool> isPulling =
            AccessTools.FieldRefAccess<PhysGrabber, bool>("isPulling");
        public static readonly AccessTools.FieldRef<PhysGrabber, bool> prevGrabbed =
            AccessTools.FieldRefAccess<PhysGrabber, bool>("prevGrabbed");
        public static readonly AccessTools.FieldRef<PhysGrabber, Vector3> grabbedcObjectPrevCamRelForward =
            AccessTools.FieldRefAccess<PhysGrabber, Vector3>("grabbedcObjectPrevCamRelForward");
        public static readonly AccessTools.FieldRef<PhysGrabber, Vector3> grabbedObjectPrevCamRelUp =
            AccessTools.FieldRefAccess<PhysGrabber, Vector3>("grabbedObjectPrevCamRelUp");
        public static readonly AccessTools.FieldRef<PhysGrabber, Vector3> physGrabPointPosition =
            AccessTools.FieldRefAccess<PhysGrabber, Vector3>("physGrabPointPosition");
        public static readonly AccessTools.FieldRef<PhysGrabber, Vector3> physGrabPointPullerPosition =
            AccessTools.FieldRefAccess<PhysGrabber, Vector3>("physGrabPointPullerPosition");
        public static readonly AccessTools.FieldRef<PhysGrabber, Vector3> localGrabPosition =
            AccessTools.FieldRefAccess<PhysGrabber, Vector3>("localGrabPosition");
        public static readonly AccessTools.FieldRef<PhysGrabber, Vector3> cameraRelativeGrabbedForward =
            AccessTools.FieldRefAccess<PhysGrabber, Vector3>("cameraRelativeGrabbedForward");
        public static readonly AccessTools.FieldRef<PhysGrabber, Vector3> cameraRelativeGrabbedUp =
            AccessTools.FieldRefAccess<PhysGrabber, Vector3>("cameraRelativeGrabbedUp");
        public static readonly AccessTools.FieldRef<PhysGrabber, Vector3> cameraRelativeGrabbedRight =
            AccessTools.FieldRefAccess<PhysGrabber, Vector3>("cameraRelativeGrabbedRight");
        public static readonly AccessTools.FieldRef<PhysGrabber, Vector3> currentGrabForce =
            AccessTools.FieldRefAccess<PhysGrabber, Vector3>("currentGrabForce");
        public static readonly AccessTools.FieldRef<PhysGrabber, Vector3> currentTorqueForce =
            AccessTools.FieldRefAccess<PhysGrabber, Vector3>("currentTorqueForce");
        public static readonly AccessTools.FieldRef<PhysGrabber, Quaternion> physRotation =
            AccessTools.FieldRefAccess<PhysGrabber, Quaternion>("physRotation");
        public static readonly AccessTools.FieldRef<PhysGrabber, Vector3> mouseTurningVelocity =
            AccessTools.FieldRefAccess<PhysGrabber, Vector3>("mouseTurningVelocity");

        public static readonly AccessTools.FieldRef<PhysGrabber, float> grabStrength =
            AccessTools.FieldRefAccess<PhysGrabber, float>("grabStrength");
        public static readonly AccessTools.FieldRef<PhysGrabber, float> throwStrength =
            AccessTools.FieldRefAccess<PhysGrabber, float>("throwStrength");
        public static readonly AccessTools.FieldRef<PhysGrabber, byte> physGrabBeamOverCharge =
            AccessTools.FieldRefAccess<PhysGrabber, byte>("physGrabBeamOverCharge");
        public static readonly AccessTools.FieldRef<PhysGrabber, float> physGrabBeamOverChargeFloat =
            AccessTools.FieldRefAccess<PhysGrabber, float>("physGrabBeamOverChargeFloat");
    }
}