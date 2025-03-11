using UnityEngine;

namespace UnifromCheat_REPO.Patches
{
    public class FullBrightMode : MonoBehaviour
    {
        private void Update()
        {
            if (Core.isFullBrightEnabled)
            {
                RenderSettings.fog = false;
                RenderSettings.ambientLight = Color.white;
            }
            else
            {
                RenderSettings.fog = true;
            }
        }
    }
}