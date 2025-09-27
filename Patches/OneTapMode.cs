using System;
using HarmonyLib;

namespace UnifromCheat_REPO.Patches
{
    [HarmonyPatch(typeof(EnemyHealth), "Hurt")]
    public class OneTapMode
    {
        protected static bool Prefix(ref int _damage)
        {
            if (Core.isOneShotModeEnabled)
                _damage = Int32.MaxValue;
            return true;
        }
    }
}