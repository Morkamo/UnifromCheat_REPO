using HarmonyLib;

namespace UnifromCheat_REPO.Utils;

internal static class AudioLowPassLogicRefs
{
    public static readonly AccessTools.FieldRef<AudioLowPassLogic, float> Volume =
        AccessTools.FieldRefAccess<AudioLowPassLogic, float>("Volume");
}
