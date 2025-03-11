using System;
using HarmonyLib;
using Photon.Pun;
using UnityEngine;

namespace UnifromCheat_REPO.Patches
{
    [HarmonyPatch(typeof(EnemyHealth), "Hurt")]
    public class OneTapMode
    {
        static bool Prefix(ref int _damage)
        {
            if (Core.isOneShotModeEnabled)
                _damage = Int32.MaxValue;
            return true;
        }
    }
}