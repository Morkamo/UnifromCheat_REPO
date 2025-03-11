using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnifromCheat_REPO.Core;

namespace UnifromCheat_REPO.WallHack
{
    public class ItemsWallHack : MonoBehaviour
    {
        private static Dictionary<ValuableObject, TextMeshPro> trackedItems =
            new Dictionary<ValuableObject, TextMeshPro>();
        
        public static void RenderItems()
        {
            if (!isItemWallHackEnabled || !isWallHackEnabled)
            {
                ClearItemLabels();
                return;
            }

            var valuableList = ValuableDirector.instance.valuableList;
            if (valuableList == null) return;

            var mainCamera = Camera.main;

            foreach (var valuable in valuableList)
            {
                if (valuable == null || valuable.gameObject == null)
                    continue;

                if (trackedItems.ContainsKey(valuable))
                {
                    UpdateText(valuable, mainCamera);
                    continue;
                }

                CreateTextLabel(valuable, mainCamera);
            }
        }

        private static void CreateTextLabel(ValuableObject valuable, Camera mainCamera)
        {
            GameObject textObject = new GameObject("Item_ValuableObjectLabel");
            TextMeshPro textMesh = textObject.AddComponent<TextMeshPro>();

            textMesh.text = "<color=red>NULL</color>";
            textMesh.fontSize = 3;
            textMesh.color = new Color(IC_R, IC_G, IC_B, IC_A);
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.enableWordWrapping = false;
            textMesh.isOverlay = true;
            textMesh.fontSharedMaterial = textMesh.font.material;
            
            textObject.transform.SetParent(valuable.transform, false);

            if (mainCamera != null)
            {
                textObject.transform.LookAt(mainCamera.transform);
                textObject.transform.Rotate(0, 180, 0);
            }

            trackedItems[valuable] = textMesh;
            if (mainCamera != null)
            {
                textObject.transform.LookAt(mainCamera.transform);
                textObject.transform.Rotate(0, 180, 0);
            }

            trackedItems[valuable] = textMesh;
        }

        private static string GetItemInfo(ValuableObject item)
        {
            string itemName = item.name
                .Replace("Valuable", "")
                .Replace("(Clone)", "")
                .Trim();

            string info = "□";

            if (showItemName)
                info += $"\n{itemName}";

            if (showItemPrice)
                info += $"\n<b>{item.dollarValueCurrent}$</b>";

            return info;
        }


        private static void UpdateText(ValuableObject target, Camera mainCamera)
        {
            if (trackedItems.TryGetValue(target, out var textMesh) && mainCamera != null)
            {
                float distance = Vector3.Distance(mainCamera.transform.position, target.transform.position);
                float dynamicSize = Mathf.Clamp((0.2f + distance) - 1, 0.2f, itemTextSize);
                
                bool isInPriceRange = target.dollarValueCurrent >= sortFromPrice && target.dollarValueCurrent <= sortToPrice;

                textMesh.text = sortByPrice && !isInPriceRange ? String.Empty : GetItemInfo(target);
                textMesh.fontSize = dynamicSize;
                textMesh.color = new Color(IC_R, IC_G, IC_B, IC_A);
                
                textMesh.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);
            }
        }

        private static void ClearItemLabels()
        {
            foreach (var item in trackedItems.Values)
            {
                if (item != null)
                    Destroy(item.gameObject);
            }

            trackedItems.Clear();
        }
        
        private void Update()
        {
            RenderItems();
        }
    }
}
