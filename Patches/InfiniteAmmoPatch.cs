using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using UnifromCheat_REPO.Utils;
using UnityEngine;

namespace UnifromCheat_REPO.Patches
{
    [HarmonyPatch(typeof(ItemGun), nameof(ItemGun.Shoot))]
    public class InfiniteAmmoPatch
    {
        static readonly FieldInfo ItemBatteryField =
            AccessTools.Field(typeof(ItemGun), "itemBattery");

        static readonly FieldInfo BatteryLifeIntField =
            AccessTools.Field(typeof(ItemBattery), "batteryLifeInt");

        private static readonly ConditionalWeakTable<ItemBattery, OriginalBatteryColor> OriginalColors = new();

        private sealed class OriginalBatteryColor
        {
            public Color Color;
        }

        static bool Prefix(ItemGun __instance)
        {
            if (Core.isInfiniteAmmo && HostOnlyGuard.IsHostOnlyActive())
            {
                ItemBattery battery =
                    (ItemBattery)ItemBatteryField.GetValue(__instance);

                if (battery == null)
                    return true;

                if (!OriginalColors.TryGetValue(battery, out _))
                    OriginalColors.Add(battery, new OriginalBatteryColor { Color = battery.batteryColor });

                battery.batteryLife = 100f;
                BatteryLifeIntField.SetValue(battery, battery.batteryBars);

                battery.batteryColor = new Color32(0, 190, 240, 255);
            }
            
            return true;
        }

        [HarmonyPatch(typeof(ItemGun), "Update")]
        public class InfiniteAmmoColorRestorePatch
        {
            static void Postfix(ItemGun __instance)
            {
                if (Core.isInfiniteAmmo && HostOnlyGuard.IsHostOnlyActive())
                    return;

                ItemBattery battery = (ItemBattery)ItemBatteryField.GetValue(__instance);
                if (battery == null)
                    return;

                if (OriginalColors.TryGetValue(battery, out OriginalBatteryColor original))
                {
                    battery.batteryColor = original.Color;
                    OriginalColors.Remove(battery);
                }
            }
        }
    }
}
