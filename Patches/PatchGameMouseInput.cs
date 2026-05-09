using HarmonyLib;

namespace UnifromCheat_REPO.Patches;

public static class PatchGameMouseInput
{
    private static bool ShouldBlock(InputKey key)
    {
        if (!Core.MenuState)
            return false;

        return key == InputKey.Grab ||
               key == InputKey.Rotate ||
               key == InputKey.SpectateNext ||
               key == InputKey.SpectatePrevious;
    }

    [HarmonyPatch(typeof(InputManager), "KeyDown")]
    private static class KeyDownPatch
    {
        private static bool Prefix(InputKey key, ref bool __result)
        {
            if (!ShouldBlock(key))
                return true;

            __result = false;
            return false;
        }
    }

    [HarmonyPatch(typeof(InputManager), "KeyUp")]
    private static class KeyUpPatch
    {
        private static bool Prefix(InputKey key, ref bool __result)
        {
            if (!ShouldBlock(key))
                return true;

            __result = false;
            return false;
        }
    }

    [HarmonyPatch(typeof(InputManager), "KeyHold")]
    private static class KeyHoldPatch
    {
        private static bool Prefix(InputKey key, ref bool __result)
        {
            if (!ShouldBlock(key))
                return true;

            __result = false;
            return false;
        }
    }
}
