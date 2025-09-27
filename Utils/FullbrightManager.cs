using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnifromCheat_REPO.Utils
{
    internal static class FullbrightManager
    {
        private static Color defaultAmbientLight = Color.black;

        static FullbrightManager()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!Core.isFullbrightEnabled)
            {
                SaveDefaultAmbient();
            }
            else
            {
                SaveDefaultAmbient();
                ApplyFullbright();
            }
        }

        public static void SaveDefaultAmbient()
        {
            defaultAmbientLight = RenderSettings.ambientLight;
        }

        public static void RestoreDefaultAmbient()
        {
            RenderSettings.ambientLight = defaultAmbientLight;
        }

        public static void ApplyFullbright()
        {
            RenderSettings.ambientLight = new Color(Core.FB_R, Core.FB_G, Core.FB_B);
        }
    }
}