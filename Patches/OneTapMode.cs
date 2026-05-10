using System;
using HarmonyLib;
using UnifromCheat_REPO.Utils;

namespace UnifromCheat_REPO.Patches
{
    [HarmonyPatch(typeof(EnemyHealth), "Hurt")]
    public class OneTapMode
    {
        protected static bool Prefix(ref int _damage)
        {
            if (Core.isOneShotModeEnabled && HostOnlyGuard.IsHostOnlyActive())
                _damage = Int32.MaxValue;
            return true;
        }
    }
}
