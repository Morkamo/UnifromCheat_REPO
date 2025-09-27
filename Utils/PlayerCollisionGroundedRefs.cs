using HarmonyLib;
using UnityEngine;

namespace UnifromCheat_REPO.Utils;

internal static class PlayerCollisionGroundedRefs
{
    // internal
    public static readonly AccessTools.FieldRef<PlayerCollisionGrounded, bool> Grounded =
        AccessTools.FieldRefAccess<PlayerCollisionGrounded, bool>("Grounded");
    public static readonly AccessTools.FieldRef<PlayerCollisionGrounded, bool> onPhysObject =
        AccessTools.FieldRefAccess<PlayerCollisionGrounded, bool>("onPhysObject");

    // private
    public static readonly AccessTools.FieldRef<PlayerCollisionGrounded, float> GroundedTimer =
        AccessTools.FieldRefAccess<PlayerCollisionGrounded, float>("GroundedTimer");
    public static readonly AccessTools.FieldRef<PlayerCollisionGrounded, SphereCollider> Collider =
        AccessTools.FieldRefAccess<PlayerCollisionGrounded, SphereCollider>("Collider");
    public static readonly AccessTools.FieldRef<PlayerCollisionGrounded, bool> colliderCheckActive =
        AccessTools.FieldRefAccess<PlayerCollisionGrounded, bool>("colliderCheckActive");

    // [HideInInspector] — но всё равно public/ internal по факту
    public static readonly AccessTools.FieldRef<PlayerCollisionGrounded, bool> physRiding =
        AccessTools.FieldRefAccess<PlayerCollisionGrounded, bool>("physRiding");
    public static readonly AccessTools.FieldRef<PlayerCollisionGrounded, int> physRidingID =
        AccessTools.FieldRefAccess<PlayerCollisionGrounded, int>("physRidingID");
    public static readonly AccessTools.FieldRef<PlayerCollisionGrounded, Vector3> physRidingPosition =
        AccessTools.FieldRefAccess<PlayerCollisionGrounded, Vector3>("physRidingPosition");
}