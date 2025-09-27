using HarmonyLib;
using Photon.Pun;
using UnityEngine;

namespace UnifromCheat_REPO.Utils;

internal static class PlayerTumbleRefs
{
    // internal
    public static readonly AccessTools.FieldRef<PlayerTumble, bool> setup =
        AccessTools.FieldRefAccess<PlayerTumble, bool>("setup");
    public static readonly AccessTools.FieldRef<PlayerTumble, Rigidbody> rb =
        AccessTools.FieldRefAccess<PlayerTumble, Rigidbody>("rb");
    public static readonly AccessTools.FieldRef<PlayerTumble, PhysGrabObject> physGrabObject =
        AccessTools.FieldRefAccess<PlayerTumble, PhysGrabObject>("physGrabObject");
    public static readonly AccessTools.FieldRef<PlayerTumble, PhotonView> photonView =
        AccessTools.FieldRefAccess<PlayerTumble, PhotonView>("photonView");
    public static readonly AccessTools.FieldRef<PlayerTumble, bool> isTumbling =
        AccessTools.FieldRefAccess<PlayerTumble, bool>("isTumbling");
    public static readonly AccessTools.FieldRef<PlayerTumble, float> tumbleSetTimer =
        AccessTools.FieldRefAccess<PlayerTumble, float>("tumbleSetTimer");
    public static readonly AccessTools.FieldRef<PlayerTumble, float> notMovingTimer =
        AccessTools.FieldRefAccess<PlayerTumble, float>("notMovingTimer");
    public static readonly AccessTools.FieldRef<PlayerTumble, bool> tumbleOverride =
        AccessTools.FieldRefAccess<PlayerTumble, bool>("tumbleOverride");
    public static readonly AccessTools.FieldRef<PlayerTumble, int> tumbleLaunch =
        AccessTools.FieldRefAccess<PlayerTumble, int>("tumbleLaunch");
    public static readonly AccessTools.FieldRef<PlayerTumble, float> impactHurtTimer =
        AccessTools.FieldRefAccess<PlayerTumble, float>("impactHurtTimer");
    public static readonly AccessTools.FieldRef<PlayerTumble, int> impactHurtDamage =
        AccessTools.FieldRefAccess<PlayerTumble, int>("impactHurtDamage");
    public static readonly AccessTools.FieldRef<PlayerTumble, bool> isPlayerInputTriggered =
        AccessTools.FieldRefAccess<PlayerTumble, bool>("isPlayerInputTriggered");

    // private
    public static readonly AccessTools.FieldRef<PlayerTumble, float> customGravityOverrideTimer =
        AccessTools.FieldRefAccess<PlayerTumble, float>("customGravityOverrideTimer");
    public static readonly AccessTools.FieldRef<PlayerTumble, bool> isTumblingPrevious =
        AccessTools.FieldRefAccess<PlayerTumble, bool>("isTumblingPrevious");
    public static readonly AccessTools.FieldRef<PlayerTumble, Vector3> notMovingPositionLast =
        AccessTools.FieldRefAccess<PlayerTumble, Vector3>("notMovingPositionLast");
    public static readonly AccessTools.FieldRef<PlayerTumble, Vector3> tumbleForce =
        AccessTools.FieldRefAccess<PlayerTumble, Vector3>("tumbleForce");
    public static readonly AccessTools.FieldRef<PlayerTumble, Vector3> tumbleTorque =
        AccessTools.FieldRefAccess<PlayerTumble, Vector3>("tumbleTorque");
    public static readonly AccessTools.FieldRef<PlayerTumble, float> tumbleForceTimer =
        AccessTools.FieldRefAccess<PlayerTumble, float>("tumbleForceTimer");
    public static readonly AccessTools.FieldRef<PlayerTumble, float> tumbleOverrideTimer =
        AccessTools.FieldRefAccess<PlayerTumble, float>("tumbleOverrideTimer");
    public static readonly AccessTools.FieldRef<PlayerTumble, bool> tumbleOverridePrevious =
        AccessTools.FieldRefAccess<PlayerTumble, bool>("tumbleOverridePrevious");
    public static readonly AccessTools.FieldRef<PlayerTumble, float> lookAtLerp =
        AccessTools.FieldRefAccess<PlayerTumble, float>("lookAtLerp");
    public static readonly AccessTools.FieldRef<PlayerTumble, float> tumbleMoveSoundTimer =
        AccessTools.FieldRefAccess<PlayerTumble, float>("tumbleMoveSoundTimer");
    public static readonly AccessTools.FieldRef<PlayerTumble, float> tumbleMoveSoundSpeed =
        AccessTools.FieldRefAccess<PlayerTumble, float>("tumbleMoveSoundSpeed");
    public static readonly AccessTools.FieldRef<PlayerTumble, float> overrideEnemyHurtTimer =
        AccessTools.FieldRefAccess<PlayerTumble, float>("overrideEnemyHurtTimer");
    public static readonly AccessTools.FieldRef<PlayerTumble, float> hurtColliderPauseTimer =
        AccessTools.FieldRefAccess<PlayerTumble, float>("hurtColliderPauseTimer");
    public static readonly AccessTools.FieldRef<PlayerTumble, float> breakFreeCooldown =
        AccessTools.FieldRefAccess<PlayerTumble, float>("breakFreeCooldown");
}