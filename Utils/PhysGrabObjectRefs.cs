using HarmonyLib;
using Photon.Pun;
using UnityEngine;

namespace UnifromCheat_REPO.Utils;

internal static class PhysGrabObjectRefs
{
    // internal
    public static readonly AccessTools.FieldRef<PhysGrabObject, PhotonView> photonView =
        AccessTools.FieldRefAccess<PhysGrabObject, PhotonView>("photonView");
    public static readonly AccessTools.FieldRef<PhysGrabObject, PhotonTransformView> photonTransformView =
        AccessTools.FieldRefAccess<PhysGrabObject, PhotonTransformView>("photonTransformView");
    public static readonly AccessTools.FieldRef<PhysGrabObject, RoomVolumeCheck> roomVolumeCheck =
        AccessTools.FieldRefAccess<PhysGrabObject, RoomVolumeCheck>("roomVolumeCheck");
    public static readonly AccessTools.FieldRef<PhysGrabObject, PhysGrabObjectImpactDetector> impactDetector =
        AccessTools.FieldRefAccess<PhysGrabObject, PhysGrabObjectImpactDetector>("impactDetector");
    public static readonly AccessTools.FieldRef<PhysGrabObject, Vector3> targetPos =
        AccessTools.FieldRefAccess<PhysGrabObject, Vector3>("targetPos");
    public static readonly AccessTools.FieldRef<PhysGrabObject, Quaternion> targetRot =
        AccessTools.FieldRefAccess<PhysGrabObject, Quaternion>("targetRot");
    public static readonly AccessTools.FieldRef<PhysGrabObject, Vector3> grabDisplacementCurrent =
        AccessTools.FieldRefAccess<PhysGrabObject, Vector3>("grabDisplacementCurrent");
    public static readonly AccessTools.FieldRef<PhysGrabObject, PlayerAvatar> lastPlayerGrabbing =
        AccessTools.FieldRefAccess<PhysGrabObject, PlayerAvatar>("lastPlayerGrabbing");
    public static readonly AccessTools.FieldRef<PhysGrabObject, float> grabbedTimer =
        AccessTools.FieldRefAccess<PhysGrabObject, float>("grabbedTimer");
    public static readonly AccessTools.FieldRef<PhysGrabObject, float> enemyInteractTimer =
        AccessTools.FieldRefAccess<PhysGrabObject, float>("enemyInteractTimer");
    public static readonly AccessTools.FieldRef<PhysGrabObject, float> angularDragOriginal =
        AccessTools.FieldRefAccess<PhysGrabObject, float>("angularDragOriginal");
    public static readonly AccessTools.FieldRef<PhysGrabObject, float> dragOriginal =
        AccessTools.FieldRefAccess<PhysGrabObject, float>("dragOriginal");
    public static readonly AccessTools.FieldRef<PhysGrabObject, bool> isValuable =
        AccessTools.FieldRefAccess<PhysGrabObject, bool>("isValuable");
    public static readonly AccessTools.FieldRef<PhysGrabObject, bool> isEnemy =
        AccessTools.FieldRefAccess<PhysGrabObject, bool>("isEnemy");
    public static readonly AccessTools.FieldRef<PhysGrabObject, EnemyRigidbody> enemyRigidbody =
        AccessTools.FieldRefAccess<PhysGrabObject, EnemyRigidbody>("enemyRigidbody");
    public static readonly AccessTools.FieldRef<PhysGrabObject, bool> isPlayer =
        AccessTools.FieldRefAccess<PhysGrabObject, bool>("isPlayer");
    public static readonly AccessTools.FieldRef<PhysGrabObject, bool> isMelee =
        AccessTools.FieldRefAccess<PhysGrabObject, bool>("isMelee");
    public static readonly AccessTools.FieldRef<PhysGrabObject, bool> isNonValuable =
        AccessTools.FieldRefAccess<PhysGrabObject, bool>("isNonValuable");
    public static readonly AccessTools.FieldRef<PhysGrabObject, bool> isKinematic =
        AccessTools.FieldRefAccess<PhysGrabObject, bool>("isKinematic");
    public static readonly AccessTools.FieldRef<PhysGrabObject, float> lastUpdateTime =
        AccessTools.FieldRefAccess<PhysGrabObject, float>("lastUpdateTime");
    public static readonly AccessTools.FieldRef<PhysGrabObject, float> gradualLerp =
        AccessTools.FieldRefAccess<PhysGrabObject, float>("gradualLerp");
    public static readonly AccessTools.FieldRef<PhysGrabObject, Vector3> prevTargetPos =
        AccessTools.FieldRefAccess<PhysGrabObject, Vector3>("prevTargetPos");
    public static readonly AccessTools.FieldRef<PhysGrabObject, Quaternion> prevTargetRot =
        AccessTools.FieldRefAccess<PhysGrabObject, Quaternion>("prevTargetRot");
    public static readonly AccessTools.FieldRef<PhysGrabObject, Vector3> rbVelocity =
        AccessTools.FieldRefAccess<PhysGrabObject, Vector3>("rbVelocity");
    public static readonly AccessTools.FieldRef<PhysGrabObject, Vector3> rbAngularVelocity =
        AccessTools.FieldRefAccess<PhysGrabObject, Vector3>("rbAngularVelocity");
    public static readonly AccessTools.FieldRef<PhysGrabObject, Vector3> currentPosition =
        AccessTools.FieldRefAccess<PhysGrabObject, Vector3>("currentPosition");
    public static readonly AccessTools.FieldRef<PhysGrabObject, Quaternion> currentRotation =
        AccessTools.FieldRefAccess<PhysGrabObject, Quaternion>("currentRotation");
    public static readonly AccessTools.FieldRef<PhysGrabObject, float> timerZeroGravity =
        AccessTools.FieldRefAccess<PhysGrabObject, float>("timerZeroGravity");
    public static readonly AccessTools.FieldRef<PhysGrabObject, float> overrideDisableBreakEffectsTimer =
        AccessTools.FieldRefAccess<PhysGrabObject, float>("overrideDisableBreakEffectsTimer");
    public static readonly AccessTools.FieldRef<PhysGrabObject, bool> isRotating =
        AccessTools.FieldRefAccess<PhysGrabObject, bool>("isRotating");
    public static readonly AccessTools.FieldRef<PhysGrabObject, float> impactHappenedTimer =
        AccessTools.FieldRefAccess<PhysGrabObject, float>("impactHappenedTimer");
    public static readonly AccessTools.FieldRef<PhysGrabObject, float> impactLightTimer =
        AccessTools.FieldRefAccess<PhysGrabObject, float>("impactLightTimer");
    public static readonly AccessTools.FieldRef<PhysGrabObject, float> impactMediumTimer =
        AccessTools.FieldRefAccess<PhysGrabObject, float>("impactMediumTimer");
    public static readonly AccessTools.FieldRef<PhysGrabObject, float> impactHeavyTimer =
        AccessTools.FieldRefAccess<PhysGrabObject, float>("impactHeavyTimer");
    public static readonly AccessTools.FieldRef<PhysGrabObject, float> breakLightTimer =
        AccessTools.FieldRefAccess<PhysGrabObject, float>("breakLightTimer");
    public static readonly AccessTools.FieldRef<PhysGrabObject, float> breakMediumTimer =
        AccessTools.FieldRefAccess<PhysGrabObject, float>("breakMediumTimer");
    public static readonly AccessTools.FieldRef<PhysGrabObject, float> breakHeavyTimer =
        AccessTools.FieldRefAccess<PhysGrabObject, float>("breakHeavyTimer");
    public static readonly AccessTools.FieldRef<PhysGrabObject, bool> hasNeverBeenGrabbed =
        AccessTools.FieldRefAccess<PhysGrabObject, bool>("hasNeverBeenGrabbed");
    public static readonly AccessTools.FieldRef<PhysGrabObject, Vector3> spawnTorque =
        AccessTools.FieldRefAccess<PhysGrabObject, Vector3>("spawnTorque");
    public static readonly AccessTools.FieldRef<PhysGrabObject, bool> physRidingDisabled =
        AccessTools.FieldRefAccess<PhysGrabObject, bool>("physRidingDisabled");
    public static readonly AccessTools.FieldRef<PhysGrabObject, bool> isRotatingRef =
        AccessTools.FieldRefAccess<PhysGrabObject, bool>("isRotating");
    public static readonly AccessTools.FieldRef<PhysGrabObject, bool> overrideKnockOutOfGrabDisable =
        AccessTools.FieldRefAccess<PhysGrabObject, bool>("overrideKnockOutOfGrabDisable");
    public static readonly AccessTools.FieldRef<PhysGrabObject, bool> isGun =
        AccessTools.FieldRefAccess<PhysGrabObject, bool>("isGun");
    public static readonly AccessTools.FieldRef<PhysGrabObject, GameObject> deathPitEffect =
        AccessTools.FieldRefAccess<PhysGrabObject, GameObject>("deathPitEffect");
    public static readonly AccessTools.FieldRef<PhysGrabObject, float> deathPitEffectDisableTimer =
        AccessTools.FieldRefAccess<PhysGrabObject, float>("deathPitEffectDisableTimer");
}