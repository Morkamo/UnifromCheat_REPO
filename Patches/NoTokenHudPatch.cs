using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace UnifromCheat_REPO.Patches
{
    [HarmonyPatch(typeof(CosmeticTokenUI))]
    internal static class NoTokenHudPatch
    {
        private static readonly FieldInfo TokenObjectsField =
            AccessTools.Field(typeof(CosmeticTokenUI), "tokenObjects");
        private static readonly HashSet<int> SuppressedInstances = new HashSet<int>();

        [HarmonyPatch("Setup")]
        [HarmonyPostfix]
        private static void SetupPostfix(CosmeticTokenUI __instance)
        {
            if (Core.isNoTokenHudEnabled)
                SetTokensVisible(__instance, false);
        }

        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        private static bool UpdatePrefix(CosmeticTokenUI __instance)
        {
            if (!Core.isNoTokenHudEnabled)
            {
                if (__instance != null && SuppressedInstances.Remove(__instance.GetInstanceID()))
                    SetTokensVisible(__instance, true);

                return true;
            }

            SetTokensVisible(__instance, false);
            return false;
        }

        private static void SetTokensVisible(CosmeticTokenUI tokenUi, bool visible)
        {
            if (tokenUi == null)
                return;

            List<CosmeticTokenUIElement> tokenObjects = TokenObjectsField?.GetValue(tokenUi) as List<CosmeticTokenUIElement>;
            if (tokenObjects == null)
                return;

            if (!visible)
                SuppressedInstances.Add(tokenUi.GetInstanceID());

            foreach (CosmeticTokenUIElement element in tokenObjects)
            {
                if (element != null)
                    element.gameObject.SetActive(visible);
            }
        }
    }
}
