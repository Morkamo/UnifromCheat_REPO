using HarmonyLib;
using UnifromCheat_REPO.Utils;

namespace UnifromCheat_REPO.Patches;

[HarmonyPatch(typeof(PlayerController), "Update")]
public class MultiJumps
{
    protected static bool Prefix()
    {
        if (SemiFunc.InputDown(InputKey.Jump) && Core.multiJumps)
        {
            PlayerCollisionGroundedRefs.Grounded(PlayerCollisionGrounded.instance) = true;
        }

        return true;
    }
}