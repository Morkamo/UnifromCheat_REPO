using System.Collections;
using HarmonyLib;
using UnifromCheat_REPO.WallHack;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnifromCheat_REPO.Utils.FireboxConsole;

namespace UnifromCheat_REPO.Patches;

[HarmonyPatch(typeof(TruckScreenText), "PlayerChatBoxStateLockedStartingTruck")]
public class WallHackSceneTransitionPatch
{
    protected static void Postfix()
    {
        if (!Core.WH_AlreadyCleared)
        {
            PlayersWallHack.ClearAll();
            ItemsWallHack.ClearAll();
            EnemiesWallHack.ClearAll();


            Core.WH_BlockUpdates = true;
            Core.WH_AlreadyCleared = true;
            
            FireLog("[WH] Cleared all caches before scene transition. Functions locked.");
            
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }
    
    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        MiscFunctions.Instance.StartCoroutine(UnblockAfterDelay());
    }
    
    private static IEnumerator UnblockAfterDelay()
    {
        yield return new WaitForSeconds(Core.WH_UnlockDelay);
        Core.WH_BlockUpdates = false;
        Core.WH_AlreadyCleared = false;
        FireLog("[WH] Scene loaded. Functions unlocked.");
    }
}