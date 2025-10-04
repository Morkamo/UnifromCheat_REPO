using HarmonyLib;
using UnityEngine;

namespace UnifromCheat_REPO.Patches;

[HarmonyPatch(typeof(Cursor), nameof(Cursor.visible), MethodType.Setter)]
class PatchUnityCursorVisible
{
    protected static bool Prefix(ref bool value)
    {
        if (Core.MenuState)
        {
            value = true;
        }
        return true;
    }
}
