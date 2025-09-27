using System.Collections.Generic;
using System.Numerics;
using HarmonyLib;

namespace UnifromCheat_REPO.Utils;

internal static class PlayerControllerRefs
{
    // bool
    public static readonly AccessTools.FieldRef<PlayerController, bool> previousCrouchingState =
        AccessTools.FieldRefAccess<PlayerController, bool>("previousCrouchingState");
    public static readonly AccessTools.FieldRef<PlayerController, bool> previousCrawlingState =
        AccessTools.FieldRefAccess<PlayerController, bool>("previousCrawlingState");
    public static readonly AccessTools.FieldRef<PlayerController, bool> previousSprintingState =
        AccessTools.FieldRefAccess<PlayerController, bool>("previousSprintingState");
    public static readonly AccessTools.FieldRef<PlayerController, bool> previousSlidingState =
        AccessTools.FieldRefAccess<PlayerController, bool>("previousSlidingState");
    public static readonly AccessTools.FieldRef<PlayerController, bool> previousMovingState =
        AccessTools.FieldRefAccess<PlayerController, bool>("previousMovingState");
    public static readonly AccessTools.FieldRef<PlayerController, bool> GroundedPrevious =
        AccessTools.FieldRefAccess<PlayerController, bool>("GroundedPrevious");
    public static readonly AccessTools.FieldRef<PlayerController, bool> CanLand =
        AccessTools.FieldRefAccess<PlayerController, bool>("CanLand");
    public static readonly AccessTools.FieldRef<PlayerController, bool> VelocityIdle =
        AccessTools.FieldRefAccess<PlayerController, bool>("VelocityIdle");
    public static readonly AccessTools.FieldRef<PlayerController, bool> Sliding =
        AccessTools.FieldRefAccess<PlayerController, bool>("Sliding");
    public static readonly AccessTools.FieldRef<PlayerController, bool> JumpFirst =
        AccessTools.FieldRefAccess<PlayerController, bool>("JumpFirst");
    public static readonly AccessTools.FieldRef<PlayerController, bool> JumpImpulse =
        AccessTools.FieldRefAccess<PlayerController, bool>("JumpImpulse");
    public static readonly AccessTools.FieldRef<PlayerController, bool> toggleSprint =
        AccessTools.FieldRefAccess<PlayerController, bool>("toggleSprint");
    public static readonly AccessTools.FieldRef<PlayerController, bool> toggleCrouch =
        AccessTools.FieldRefAccess<PlayerController, bool>("toggleCrouch");
    public static readonly AccessTools.FieldRef<PlayerController, bool> debugSlow =
        AccessTools.FieldRefAccess<PlayerController, bool>("debugSlow");

    // float
    public static readonly AccessTools.FieldRef<PlayerController, float> landCooldown =
        AccessTools.FieldRefAccess<PlayerController, float>("landCooldown");
    public static readonly AccessTools.FieldRef<PlayerController, float> SprintSpeedCurrent =
        AccessTools.FieldRefAccess<PlayerController, float>("SprintSpeedCurrent");
    public static readonly AccessTools.FieldRef<PlayerController, float> SprintSpeedLerp =
        AccessTools.FieldRefAccess<PlayerController, float>("SprintSpeedLerp");
    public static readonly AccessTools.FieldRef<PlayerController, float> SprintedTimer =
        AccessTools.FieldRefAccess<PlayerController, float>("SprintedTimer");
    public static readonly AccessTools.FieldRef<PlayerController, float> SprintDrainTimer =
        AccessTools.FieldRefAccess<PlayerController, float>("SprintDrainTimer");
    public static readonly AccessTools.FieldRef<PlayerController, float> CrouchActiveTimer =
        AccessTools.FieldRefAccess<PlayerController, float>("CrouchActiveTimer");
    public static readonly AccessTools.FieldRef<PlayerController, float> CrouchInactiveTimer =
        AccessTools.FieldRefAccess<PlayerController, float>("CrouchInactiveTimer");
    public static readonly AccessTools.FieldRef<PlayerController, float> SlideTimer =
        AccessTools.FieldRefAccess<PlayerController, float>("SlideTimer");
    public static readonly AccessTools.FieldRef<PlayerController, float> JumpCooldown =
        AccessTools.FieldRefAccess<PlayerController, float>("JumpCooldown");
    public static readonly AccessTools.FieldRef<PlayerController, float> JumpGroundedBuffer =
        AccessTools.FieldRefAccess<PlayerController, float>("JumpGroundedBuffer");
    public static readonly AccessTools.FieldRef<PlayerController, float> JumpInputBuffer =
        AccessTools.FieldRefAccess<PlayerController, float>("JumpInputBuffer");
    public static readonly AccessTools.FieldRef<PlayerController, float> OverrideJumpCooldownAmount =
        AccessTools.FieldRefAccess<PlayerController, float>("OverrideJumpCooldownAmount");
    public static readonly AccessTools.FieldRef<PlayerController, float> OverrideJumpCooldownCurrent =
        AccessTools.FieldRefAccess<PlayerController, float>("OverrideJumpCooldownCurrent");
    public static readonly AccessTools.FieldRef<PlayerController, float> OverrideJumpCooldownTimer =
        AccessTools.FieldRefAccess<PlayerController, float>("OverrideJumpCooldownTimer");
    public static readonly AccessTools.FieldRef<PlayerController, float> sprintRechargeTimer =
        AccessTools.FieldRefAccess<PlayerController, float>("sprintRechargeTimer");
    public static readonly AccessTools.FieldRef<PlayerController, float> sprintRechargeTime =
        AccessTools.FieldRefAccess<PlayerController, float>("sprintRechargeTime");
    public static readonly AccessTools.FieldRef<PlayerController, float> sprintRechargeAmount =
        AccessTools.FieldRefAccess<PlayerController, float>("sprintRechargeAmount");
    public static readonly AccessTools.FieldRef<PlayerController, float> movingResetTimer =
        AccessTools.FieldRefAccess<PlayerController, float>("movingResetTimer");
    public static readonly AccessTools.FieldRef<PlayerController, float> InputDisableTimer =
        AccessTools.FieldRefAccess<PlayerController, float>("InputDisableTimer");
    public static readonly AccessTools.FieldRef<PlayerController, float> overrideSpeedTimer =
        AccessTools.FieldRefAccess<PlayerController, float>("overrideSpeedTimer");
    public static readonly AccessTools.FieldRef<PlayerController, float> overrideLookSpeedTimer =
        AccessTools.FieldRefAccess<PlayerController, float>("overrideLookSpeedTimer");
    public static readonly AccessTools.FieldRef<PlayerController, float> overrideLookSpeedLerp =
        AccessTools.FieldRefAccess<PlayerController, float>("overrideLookSpeedLerp");
    public static readonly AccessTools.FieldRef<PlayerController, float> overrideLookSpeedProgress =
        AccessTools.FieldRefAccess<PlayerController, float>("overrideLookSpeedProgress");
    public static readonly AccessTools.FieldRef<PlayerController, float> overrideVoicePitchTimer =
        AccessTools.FieldRefAccess<PlayerController, float>("overrideVoicePitchTimer");
    public static readonly AccessTools.FieldRef<PlayerController, float> overrideTimeScaleTimer =
        AccessTools.FieldRefAccess<PlayerController, float>("overrideTimeScaleTimer");
    public static readonly AccessTools.FieldRef<PlayerController, float> antiGravityTimer =
        AccessTools.FieldRefAccess<PlayerController, float>("antiGravityTimer");
    public static readonly AccessTools.FieldRef<PlayerController, float> featherTimer =
        AccessTools.FieldRefAccess<PlayerController, float>("featherTimer");
    public static readonly AccessTools.FieldRef<PlayerController, float> deathSeenTimer =
        AccessTools.FieldRefAccess<PlayerController, float>("deathSeenTimer");
    public static readonly AccessTools.FieldRef<PlayerController, float> tumbleInputDisableTimer =
        AccessTools.FieldRefAccess<PlayerController, float>("tumbleInputDisableTimer");
    public static readonly AccessTools.FieldRef<PlayerController, float> kinematicTimer =
        AccessTools.FieldRefAccess<PlayerController, float>("kinematicTimer");
    public static readonly AccessTools.FieldRef<PlayerController, float> rbOriginalMass =
        AccessTools.FieldRefAccess<PlayerController, float>("rbOriginalMass");
    public static readonly AccessTools.FieldRef<PlayerController, float> rbOriginalDrag =
        AccessTools.FieldRefAccess<PlayerController, float>("rbOriginalDrag");
    public static readonly AccessTools.FieldRef<PlayerController, float> playerOriginalMoveSpeed =
        AccessTools.FieldRefAccess<PlayerController, float>("playerOriginalMoveSpeed");
    public static readonly AccessTools.FieldRef<PlayerController, float> playerOriginalCustomGravity =
        AccessTools.FieldRefAccess<PlayerController, float>("playerOriginalCustomGravity");
    public static readonly AccessTools.FieldRef<PlayerController, float> playerOriginalSprintSpeed =
        AccessTools.FieldRefAccess<PlayerController, float>("playerOriginalSprintSpeed");
    public static readonly AccessTools.FieldRef<PlayerController, float> playerOriginalCrouchSpeed =
        AccessTools.FieldRefAccess<PlayerController, float>("playerOriginalCrouchSpeed");

    // int
    public static readonly AccessTools.FieldRef<PlayerController, int> JumpExtraCurrent =
        AccessTools.FieldRefAccess<PlayerController, int>("JumpExtraCurrent");
    public static readonly AccessTools.FieldRef<PlayerController, int> JumpExtra =
        AccessTools.FieldRefAccess<PlayerController, int>("JumpExtra");

    // Vector3
    public static readonly AccessTools.FieldRef<PlayerController, Vector3> VelocityRelativeNew =
        AccessTools.FieldRefAccess<PlayerController, Vector3>("VelocityRelativeNew");
    public static readonly AccessTools.FieldRef<PlayerController, Vector3> VelocityImpulse =
        AccessTools.FieldRefAccess<PlayerController, Vector3>("VelocityImpulse");
    public static readonly AccessTools.FieldRef<PlayerController, Vector3> SlideDirection =
        AccessTools.FieldRefAccess<PlayerController, Vector3>("SlideDirection");
    public static readonly AccessTools.FieldRef<PlayerController, Vector3> SlideDirectionCurrent =
        AccessTools.FieldRefAccess<PlayerController, Vector3>("SlideDirectionCurrent");
    public static readonly AccessTools.FieldRef<PlayerController, Vector3> positionPrevious =
        AccessTools.FieldRefAccess<PlayerController, Vector3>("positionPrevious");
    public static readonly AccessTools.FieldRef<PlayerController, Vector3> MoveForceDirection =
        AccessTools.FieldRefAccess<PlayerController, Vector3>("MoveForceDirection");
    public static readonly AccessTools.FieldRef<PlayerController, float> MoveForceAmount =
        AccessTools.FieldRefAccess<PlayerController, float>("MoveForceAmount");
    public static readonly AccessTools.FieldRef<PlayerController, float> MoveForceTimer =
        AccessTools.FieldRefAccess<PlayerController, float>("MoveForceTimer");
    public static readonly AccessTools.FieldRef<PlayerController, float> MoveMultiplier =
        AccessTools.FieldRefAccess<PlayerController, float>("MoveMultiplier");
    public static readonly AccessTools.FieldRef<PlayerController, float> MoveMultiplierTimer =
        AccessTools.FieldRefAccess<PlayerController, float>("MoveMultiplierTimer");
    public static readonly AccessTools.FieldRef<PlayerController, Vector3> originalVelocity =
        AccessTools.FieldRefAccess<PlayerController, Vector3>("originalVelocity");
    public static readonly AccessTools.FieldRef<PlayerController, Vector3> originalAngularVelocity =
        AccessTools.FieldRefAccess<PlayerController, Vector3>("originalAngularVelocity");

    // списки
    public static readonly AccessTools.FieldRef<PlayerController, List<PhysGrabObject>> JumpGroundedObjects =
        AccessTools.FieldRefAccess<PlayerController, List<PhysGrabObject>>("JumpGroundedObjects");
}