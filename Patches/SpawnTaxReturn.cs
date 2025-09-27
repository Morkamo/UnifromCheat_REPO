using HarmonyLib;
using UnifromCheat_REPO.WallHack;
using UnityEngine;
using static UnifromCheat_REPO.Utils.FireboxConsole;

namespace UnifromCheat_REPO.Patches;

[HarmonyPatch(typeof(ExtractionPoint), "SpawnTaxReturn")]
public class SpawnTaxReturn
{
    protected static void Postfix()
    {
        var surplusList = Object.FindObjectsOfType<SurplusValuable>(true);
        foreach (var s in surplusList)
        {
            if (s == null) 
                continue;

            if (!ItemsWallHack.surplusOutlines.ContainsKey(s))
            {
                var outline = ItemsWallHack.CreateWHCopy(
                    s.gameObject,
                    new Color(Core.SPC_R, Core.SPC_G, Core.SPC_B, Core.SPC_A)
                );
                ItemsWallHack.surplusOutlines[s] = outline;

                FireLog($"[WH] Surplus registered: {s.name}");
            }
        }
    }
}