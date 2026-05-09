using HarmonyLib;
using UnifromCheat_REPO.Utils;
using UnityEngine;

namespace UnifromCheat_REPO.Patches;

/*[HarmonyPatch(typeof(CameraJump), "Jump")]
public class CosmeticObjectSearcher
{
    static void Postfix() 
    {
        FireboxConsole.FireLog("Jumped");
        foreach (var obj in RoundDirector.instance.cosmeticWorldObjects)
        {
            FireboxConsole.FireLog($"{obj.name}: {obj.GetInstanceID()}");
        } 
    }
}*/