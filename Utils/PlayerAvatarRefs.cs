using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace UnifromCheat_REPO.Utils;

internal static class PlayerAvatarRefs
{
    // private
    public static readonly AccessTools.FieldRef<PlayerAvatar, Collider> collider =
        AccessTools.FieldRefAccess<PlayerAvatar, Collider>("collider");
    public static readonly AccessTools.FieldRef<PlayerAvatar, Camera> localCamera =
        AccessTools.FieldRefAccess<PlayerAvatar, Camera>("localCamera");
    public static readonly AccessTools.FieldRef<PlayerAvatar, bool> spawnImpulse =
        AccessTools.FieldRefAccess<PlayerAvatar, bool>("spawnImpulse");
    public static readonly AccessTools.FieldRef<PlayerAvatar, int> spawnFrames =
        AccessTools.FieldRefAccess<PlayerAvatar, int>("spawnFrames");
    public static readonly AccessTools.FieldRef<PlayerAvatar, bool> spawnDoneImpulse =
        AccessTools.FieldRefAccess<PlayerAvatar, bool>("spawnDoneImpulse");
    public static readonly AccessTools.FieldRef<PlayerAvatar, Vector3> spawnPosition =
        AccessTools.FieldRefAccess<PlayerAvatar, Vector3>("spawnPosition");
    public static readonly AccessTools.FieldRef<PlayerAvatar, bool> Interact =
        AccessTools.FieldRefAccess<PlayerAvatar, bool>("Interact");
    public static readonly AccessTools.FieldRef<PlayerAvatar, Rigidbody> rb =
        AccessTools.FieldRefAccess<PlayerAvatar, Rigidbody>("rb");
    public static readonly AccessTools.FieldRef<PlayerAvatar, float> rbDiscreteTimer =
        AccessTools.FieldRefAccess<PlayerAvatar, float>("rbDiscreteTimer");
    public static readonly AccessTools.FieldRef<PlayerAvatar, float> deadTime =
        AccessTools.FieldRefAccess<PlayerAvatar, float>("deadTime");
    public static readonly AccessTools.FieldRef<PlayerAvatar, float> deadTimer =
        AccessTools.FieldRefAccess<PlayerAvatar, float>("deadTimer");
    public static readonly AccessTools.FieldRef<PlayerAvatar, Transform> deadEnemyLookAtTransform =
        AccessTools.FieldRefAccess<PlayerAvatar, Transform>("deadEnemyLookAtTransform");
    public static readonly AccessTools.FieldRef<PlayerAvatar, bool> fallDamageResetStatePrevious =
        AccessTools.FieldRefAccess<PlayerAvatar, bool>("fallDamageResetStatePrevious");
    public static readonly AccessTools.FieldRef<PlayerAvatar, float> fallDamageResetTimer =
        AccessTools.FieldRefAccess<PlayerAvatar, float>("fallDamageResetTimer");
    public static readonly AccessTools.FieldRef<PlayerAvatar, bool> falling =
        AccessTools.FieldRefAccess<PlayerAvatar, bool>("falling");
    public static readonly AccessTools.FieldRef<PlayerAvatar, float> playerPingTimer =
        AccessTools.FieldRefAccess<PlayerAvatar, float>("playerPingTimer");

    // override animation speed stuff
    public static readonly AccessTools.FieldRef<PlayerAvatar, float> overrrideAnimationSpeedTimer =
        AccessTools.FieldRefAccess<PlayerAvatar, float>("overrrideAnimationSpeedTimer");
    public static readonly AccessTools.FieldRef<PlayerAvatar, float> overrrideAnimationSpeedTarget =
        AccessTools.FieldRefAccess<PlayerAvatar, float>("overrrideAnimationSpeedTarget");
    public static readonly AccessTools.FieldRef<PlayerAvatar, float> overrrideAnimationSpeedIn =
        AccessTools.FieldRefAccess<PlayerAvatar, float>("overrrideAnimationSpeedIn");
    public static readonly AccessTools.FieldRef<PlayerAvatar, float> overrrideAnimationSpeedOut =
        AccessTools.FieldRefAccess<PlayerAvatar, float>("overrrideAnimationSpeedOut");
    public static readonly AccessTools.FieldRef<PlayerAvatar, float> overrideAnimationSpeedLerp =
        AccessTools.FieldRefAccess<PlayerAvatar, float>("overrideAnimationSpeedLerp");
    public static readonly AccessTools.FieldRef<PlayerAvatar, bool> overrideAnimationSpeedActive =
        AccessTools.FieldRefAccess<PlayerAvatar, bool>("overrideAnimationSpeedActive");
    public static readonly AccessTools.FieldRef<PlayerAvatar, float> overrideAnimationSpeedTime =
        AccessTools.FieldRefAccess<PlayerAvatar, float>("overrideAnimationSpeedTime");

    // pupil size override
    public static readonly AccessTools.FieldRef<PlayerAvatar, SpringFloat> overridePupilSizeSpring =
        AccessTools.FieldRefAccess<PlayerAvatar, SpringFloat>("overridePupilSizeSpring");
    public static readonly AccessTools.FieldRef<PlayerAvatar, float> overridePupilSizeTimer =
        AccessTools.FieldRefAccess<PlayerAvatar, float>("overridePupilSizeTimer");
    public static readonly AccessTools.FieldRef<PlayerAvatar, float> overridePupilSizeTime =
        AccessTools.FieldRefAccess<PlayerAvatar, float>("overridePupilSizeTime");
    public static readonly AccessTools.FieldRef<PlayerAvatar, float> overridePupilSizeMultiplier =
        AccessTools.FieldRefAccess<PlayerAvatar, float>("overridePupilSizeMultiplier");
    public static readonly AccessTools.FieldRef<PlayerAvatar, float> overridePupilSizeMultiplierTarget =
        AccessTools.FieldRefAccess<PlayerAvatar, float>("overridePupilSizeMultiplierTarget");
    public static readonly AccessTools.FieldRef<PlayerAvatar, float> overridePupilSpringSpeedIn =
        AccessTools.FieldRefAccess<PlayerAvatar, float>("overridePupilSpringSpeedIn");
    public static readonly AccessTools.FieldRef<PlayerAvatar, float> overridePupilSpringDampIn =
        AccessTools.FieldRefAccess<PlayerAvatar, float>("overridePupilSpringDampIn");
    public static readonly AccessTools.FieldRef<PlayerAvatar, float> overridePupilSpringSpeedOut =
        AccessTools.FieldRefAccess<PlayerAvatar, float>("overridePupilSpringSpeedOut");
    public static readonly AccessTools.FieldRef<PlayerAvatar, float> overridePupilSpringDampOut =
        AccessTools.FieldRefAccess<PlayerAvatar, float>("overridePupilSpringDampOut");
    public static readonly AccessTools.FieldRef<PlayerAvatar, int> overridePupilSizePrio =
        AccessTools.FieldRefAccess<PlayerAvatar, int>("overridePupilSizePrio");

    // color failsafe
    public static readonly AccessTools.FieldRef<PlayerAvatar, bool> colorWasSet =
        AccessTools.FieldRefAccess<PlayerAvatar, bool>("colorWasSet");
    public static readonly AccessTools.FieldRef<PlayerAvatar, float> noColorFailsafeTimer =
        AccessTools.FieldRefAccess<PlayerAvatar, float>("noColorFailsafeTimer");

    // internal
    public static readonly AccessTools.FieldRef<PlayerAvatar, string> playerName =
        AccessTools.FieldRefAccess<PlayerAvatar, string>("playerName");
    public static readonly AccessTools.FieldRef<PlayerAvatar, string> steamID =
        AccessTools.FieldRefAccess<PlayerAvatar, string>("steamID");
    public static readonly AccessTools.FieldRef<PlayerAvatar, Vector3> localCameraPosition =
        AccessTools.FieldRefAccess<PlayerAvatar, Vector3>("localCameraPosition");
    public static readonly AccessTools.FieldRef<PlayerAvatar, Quaternion> localCameraRotation =
        AccessTools.FieldRefAccess<PlayerAvatar, Quaternion>("localCameraRotation");
    public static readonly AccessTools.FieldRef<PlayerAvatar, bool> isLocal =
        AccessTools.FieldRefAccess<PlayerAvatar, bool>("isLocal");
    public static readonly AccessTools.FieldRef<PlayerAvatar, bool> isDisabled =
        AccessTools.FieldRefAccess<PlayerAvatar, bool>("isDisabled");
    public static readonly AccessTools.FieldRef<PlayerAvatar, bool> outroDone =
        AccessTools.FieldRefAccess<PlayerAvatar, bool>("outroDone");
    public static readonly AccessTools.FieldRef<PlayerAvatar, bool> spawned =
        AccessTools.FieldRefAccess<PlayerAvatar, bool>("spawned");
    public static readonly AccessTools.FieldRef<PlayerAvatar, Quaternion> spawnRotation =
        AccessTools.FieldRefAccess<PlayerAvatar, Quaternion>("spawnRotation");
    public static readonly AccessTools.FieldRef<PlayerAvatar, bool> finalHeal =
        AccessTools.FieldRefAccess<PlayerAvatar, bool>("finalHeal");
    public static readonly AccessTools.FieldRef<PlayerAvatar, bool> isCrouching =
        AccessTools.FieldRefAccess<PlayerAvatar, bool>("isCrouching");
    public static readonly AccessTools.FieldRef<PlayerAvatar, bool> isSprinting =
        AccessTools.FieldRefAccess<PlayerAvatar, bool>("isSprinting");
    public static readonly AccessTools.FieldRef<PlayerAvatar, bool> isCrawling =
        AccessTools.FieldRefAccess<PlayerAvatar, bool>("isCrawling");
    public static readonly AccessTools.FieldRef<PlayerAvatar, bool> isSliding =
        AccessTools.FieldRefAccess<PlayerAvatar, bool>("isSliding");
    public static readonly AccessTools.FieldRef<PlayerAvatar, bool> isMoving =
        AccessTools.FieldRefAccess<PlayerAvatar, bool>("isMoving");
    public static readonly AccessTools.FieldRef<PlayerAvatar, bool> isGrounded =
        AccessTools.FieldRefAccess<PlayerAvatar, bool>("isGrounded");
    public static readonly AccessTools.FieldRef<PlayerAvatar, bool> isTumbling =
        AccessTools.FieldRefAccess<PlayerAvatar, bool>("isTumbling");
    public static readonly AccessTools.FieldRef<PlayerAvatar, Vector3> InputDirection =
        AccessTools.FieldRefAccess<PlayerAvatar, Vector3>("InputDirection");
    public static readonly AccessTools.FieldRef<PlayerAvatar, Vector3> LastNavmeshPosition =
        AccessTools.FieldRefAccess<PlayerAvatar, Vector3>("LastNavmeshPosition");
    public static readonly AccessTools.FieldRef<PlayerAvatar, float> LastNavMeshPositionTimer =
        AccessTools.FieldRefAccess<PlayerAvatar, float>("LastNavMeshPositionTimer");
    public static readonly AccessTools.FieldRef<PlayerAvatar, PlayerVoiceChat> voiceChat =
        AccessTools.FieldRefAccess<PlayerAvatar, PlayerVoiceChat>("voiceChat");
    public static readonly AccessTools.FieldRef<PlayerAvatar, bool> voiceChatFetched =
        AccessTools.FieldRefAccess<PlayerAvatar, bool>("voiceChatFetched");
    public static readonly AccessTools.FieldRef<PlayerAvatar, Vector3> rbVelocity =
        AccessTools.FieldRefAccess<PlayerAvatar, Vector3>("rbVelocity");
    public static readonly AccessTools.FieldRef<PlayerAvatar, Vector3> rbVelocityRaw =
        AccessTools.FieldRefAccess<PlayerAvatar, Vector3>("rbVelocityRaw");
    public static readonly AccessTools.FieldRef<PlayerAvatar, Vector3> clientPosition =
        AccessTools.FieldRefAccess<PlayerAvatar, Vector3>("clientPosition");
    public static readonly AccessTools.FieldRef<PlayerAvatar, Vector3> clientPositionCurrent =
        AccessTools.FieldRefAccess<PlayerAvatar, Vector3>("clientPositionCurrent");
    public static readonly AccessTools.FieldRef<PlayerAvatar, float> clientPositionDelta =
        AccessTools.FieldRefAccess<PlayerAvatar, float>("clientPositionDelta");
    public static readonly AccessTools.FieldRef<PlayerAvatar, Quaternion> clientRotation =
        AccessTools.FieldRefAccess<PlayerAvatar, Quaternion>("clientRotation");
    public static readonly AccessTools.FieldRef<PlayerAvatar, Quaternion> clientRotationCurrent =
        AccessTools.FieldRefAccess<PlayerAvatar, Quaternion>("clientRotationCurrent");
    public static readonly AccessTools.FieldRef<PlayerAvatar, bool> clientPhysRiding =
        AccessTools.FieldRefAccess<PlayerAvatar, bool>("clientPhysRiding");
    public static readonly AccessTools.FieldRef<PlayerAvatar, int> clientPhysRidingID =
        AccessTools.FieldRefAccess<PlayerAvatar, int>("clientPhysRidingID");
    public static readonly AccessTools.FieldRef<PlayerAvatar, Vector3> clientPhysRidingPosition =
        AccessTools.FieldRefAccess<PlayerAvatar, Vector3>("clientPhysRidingPosition");
    public static readonly AccessTools.FieldRef<PlayerAvatar, Transform> clientPhysRidingTransform =
        AccessTools.FieldRefAccess<PlayerAvatar, Transform>("clientPhysRidingTransform");
    public static readonly AccessTools.FieldRef<PlayerAvatar, bool> spectating =
        AccessTools.FieldRefAccess<PlayerAvatar, bool>("spectating");
    public static readonly AccessTools.FieldRef<PlayerAvatar, bool> deadSet =
        AccessTools.FieldRefAccess<PlayerAvatar, bool>("deadSet");
    public static readonly AccessTools.FieldRef<PlayerAvatar, int> steamIDshort =
        AccessTools.FieldRefAccess<PlayerAvatar, int>("steamIDshort");
    public static readonly AccessTools.FieldRef<PlayerAvatar, bool> fallDamageResetState =
        AccessTools.FieldRefAccess<PlayerAvatar, bool>("fallDamageResetState");
    public static readonly AccessTools.FieldRef<PlayerAvatar, int> playerPing =
        AccessTools.FieldRefAccess<PlayerAvatar, int>("playerPing");
    public static readonly AccessTools.FieldRef<PlayerAvatar, bool> quitApplication =
        AccessTools.FieldRefAccess<PlayerAvatar, bool>("quitApplication");

    public static readonly AccessTools.FieldRef<PlayerAvatar, bool> overridePupilSizeActive =
        AccessTools.FieldRefAccess<PlayerAvatar, bool>("overridePupilSizeActive");
    public static readonly AccessTools.FieldRef<PlayerAvatar, int> upgradeMapPlayerCount =
        AccessTools.FieldRefAccess<PlayerAvatar, int>("upgradeMapPlayerCount");
    public static readonly AccessTools.FieldRef<PlayerAvatar, bool> levelAnimationCompleted =
        AccessTools.FieldRefAccess<PlayerAvatar, bool>("levelAnimationCompleted");
    public static readonly AccessTools.FieldRef<PlayerAvatar, float> upgradeCrouchRest =
        AccessTools.FieldRefAccess<PlayerAvatar, float>("upgradeCrouchRest");
    public static readonly AccessTools.FieldRef<PlayerAvatar, float> upgradeTumbleWings =
        AccessTools.FieldRefAccess<PlayerAvatar, float>("upgradeTumbleWings");
    public static readonly AccessTools.FieldRef<PlayerAvatar, bool> upgradeTumbleWingsVisualsActive =
        AccessTools.FieldRefAccess<PlayerAvatar, bool>("upgradeTumbleWingsVisualsActive");
    public static readonly AccessTools.FieldRef<PlayerAvatar, WorldSpaceUIPlayerName> worldSpaceUIPlayerName =
        AccessTools.FieldRefAccess<PlayerAvatar, WorldSpaceUIPlayerName>("worldSpaceUIPlayerName");
    public static readonly AccessTools.FieldRef<PlayerAvatar, Dictionary<int, float>> playerExpressions =
        AccessTools.FieldRefAccess<PlayerAvatar, Dictionary<int, float>>("playerExpressions");
}