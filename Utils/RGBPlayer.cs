using System.Reflection;
using UnityEngine;

namespace UnifromCheat_REPO.Utils;

public class RGBPlayer : MonoBehaviour
{
    private static readonly int[] RainbowOrder =
    {
        4, 3, 7, 8,
        10, 11, 12, 13, 15,
        16, 17,
        18, 19,
        21, 22, 23,
        26, 27, 28,
        29, 30, 31,
        5, 6, 20, 24, 25, 32, 33, 34, 35, 1, 2, 0
    };

    public static RGBPlayer Instance { get; private set; }

    private static readonly FieldInfo ColorsEquippedField = typeof(MetaManager).GetField(
        "colorsEquipped",
        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
    );

    private static readonly FieldInfo ColorsPreviewEnabledField = typeof(MetaManager).GetField(
        "colorsPreviewEnabled",
        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
    );

    private int currentIndex;
    private float nextUpdateTime;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        if (Instance == null)
            Instance = this;

        ResetTimer();
    }

    private void OnDisable()
    {
        if (Instance == this)
            Instance = null;
    }

        private void Update()
        {
            if (!Core.isRGBPlayerEnabled || !Core.visualsMasterSwitch)
                return;

        if (Time.unscaledTime < nextUpdateTime)
            return;

        ApplyCurrentColor();
        currentIndex = (currentIndex + 1) % RainbowOrder.Length;
        nextUpdateTime = Time.unscaledTime + Mathf.Max(Core.RGBupdateInterval, (ushort)1) / 1000f;
    }

    public static void StartCycle()
    {
        if (Instance == null)
            return;

        Instance.currentIndex = 0;
        Instance.ResetTimer();
        Instance.ApplyCurrentColor();
    }

    public static void StopCycle()
    {
        Instance?.ResetTimer();
    }

    private void ResetTimer()
    {
        nextUpdateTime = Time.unscaledTime;
    }

    private void ApplyCurrentColor()
    {
        int colorIndex = RainbowOrder[currentIndex];
        ApplyCosmeticColor(colorIndex);
    }

    private static void ApplyCosmeticColor(int colorIndex)
    {
        var metaManager = MetaManager.instance;
        if (metaManager == null)
            return;

        var colorsEquipped = ColorsEquippedField?.GetValue(metaManager) as int[];
        if (colorsEquipped == null || colorsEquipped.Length == 0)
            return;

        int colorCount = metaManager.colors != null ? metaManager.colors.Count : 0;
        if (colorCount > 0 && colorIndex >= colorCount)
            colorIndex = 0;

        for (int i = 0; i < colorsEquipped.Length; i++)
            metaManager.CosmeticColorSet(i, colorIndex, false);

        ColorsPreviewEnabledField?.SetValue(metaManager, false);
        metaManager.CosmeticPlayerUpdateLocal(true, false);
    }
}
