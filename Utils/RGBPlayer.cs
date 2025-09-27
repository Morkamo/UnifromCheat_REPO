using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnifromCheat_REPO.Utils;

public class RGBPlayer : MonoBehaviour
{
    private static readonly int[] rainbowOrder =
    {
        4,  3,  7,  8,
        10, 11, 12, 13, 15,
        16, 17,
        18, 19,
        21, 22, 23,
        26, 27, 28,
        29, 30, 31,
        5, 6, 20, 24, 25, 32, 33, 34, 35, 1, 2, 0
    };

    private static bool isCycling;
    private static Task cycleTask;
    private static bool stopRequested;

    public static void StartCycle()
    {
        if (isCycling) return;
        stopRequested = false;
        cycleTask = CycleColors();
    }

    public static void StopCycle()
    {
        stopRequested = true;
        isCycling = false;
    }

    private static async Task CycleColors()
    {
        isCycling = true;
        int idx = 0;

        while (!stopRequested)
        {
            PlayerController.instance.playerAvatarScript.PlayerAvatarSetColor(rainbowOrder[idx]);

            idx++;
            if (idx >= rainbowOrder.Length) idx = 0;

            await Task.Delay(Core.RGBupdateInterval);
        }

        isCycling = false;
    }
}