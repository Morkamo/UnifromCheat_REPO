using HarmonyLib;
using UnifromCheat_REPO.Utils;

namespace UnifromCheat_REPO.Patches
{
    [HarmonyPatch(typeof(DataDirector), nameof(DataDirector.SaveDeleteCheck))]
    public static class ProtectedSessionPatch
    {
        private static readonly AccessTools.FieldRef<RunManager, bool> AllPlayersDead =
            AccessTools.FieldRefAccess<RunManager, bool>("allPlayersDead");

        private static readonly AccessTools.FieldRef<RunManager, Level> LevelPrevious =
            AccessTools.FieldRefAccess<RunManager, Level>("levelPrevious");

        private static bool Prefix()
        {
            if (!Core.isProtectedSession || !HostOnlyGuard.IsHostOnlyActive())
                return true;

            RunManager runManager = RunManager.instance;
            if (runManager == null)
                return true;

            Level levelPrevious = LevelPrevious(runManager);
            bool gameOverDelete =
                SemiFunc.RunIsArena() ||
                (AllPlayersDead(runManager) &&
                 levelPrevious != runManager.levelMainMenu &&
                 levelPrevious != runManager.levelLobbyMenu &&
                 levelPrevious != runManager.levelTutorial &&
                 levelPrevious != runManager.levelLobby &&
                 !SemiFunc.IsLevelShop(levelPrevious) &&
                 levelPrevious != runManager.levelRecording);

            return !gameOverDelete;
        }
    }
}
