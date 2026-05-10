using UnityEngine;

namespace UnifromCheat_REPO.Utils
{
    internal static class HostOnlyGuard
    {
        private static float lastHostOnlyMessageTime = -10f;

        public static bool CanUseHostOnly(bool notify = true, string functionName = null)
        {
            if (!IsPlayableSession())
                return false;

            if (!HasHostAccess())
            {
                if (notify)
                {
                    string message = string.IsNullOrWhiteSpace(functionName)
                        ? LocalizedMessages.Get("hostOnlyUnavailable")
                        : LocalizedMessages.Format("hostOnlyFunctionUnavailable", functionName);
                    ShowThrottledMessage(message, ref lastHostOnlyMessageTime);
                }

                return false;
            }

            return true;
        }

        public static bool IsHostOnlyActive()
        {
            return IsPlayableSession() && HasHostAccess();
        }

        public static bool ShouldShowUnavailableStatus()
        {
            return IsPlayableSession() && !HasHostAccess();
        }

        public static bool IsPlayableSession()
        {
            return PlayerController.instance != null &&
                   PlayerController.instance.playerAvatar != null &&
                   PlayerController.instance.playerAvatarScript != null &&
                   !SemiFunc.MenuLevel();
        }

        private static bool HasHostAccess()
        {
            return SemiFunc.IsMasterClientOrSingleplayer();
        }

        private static void ShowThrottledMessage(string message, ref float lastTime)
        {
            if (Time.unscaledTime - lastTime < 1f)
                return;

            lastTime = Time.unscaledTime;
            Core.ShowMessage(message, 3f);
        }
    }
}
