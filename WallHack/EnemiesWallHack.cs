﻿using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using static UnifromCheat_REPO.Core;

namespace UnifromCheat_REPO.WallHack
{
    public class EnemiesWallHack : MonoBehaviour
    {
        private static Dictionary<Enemy, TextMeshPro> trackedEnemies = new Dictionary<Enemy, TextMeshPro>();

        public static void RenderEnemies()
        {
            if (!isEnemyWallHackEnabled || !isWallHackEnabled)
            {
                ClearEnemyLabels();
                return;
            }

            var enemyParents = EnemyDirector.instance.enemiesSpawned;
                
            if (enemyParents.Count == 0) return;

            var mainCamera = Camera.main;

            foreach (var enemyParent in enemyParents)
            {
                var enemy = enemyParent.GetComponentInChildren<Enemy>();
                
                if (enemy == null || enemy.gameObject == null)
                    continue;

                if (trackedEnemies.ContainsKey(enemy))
                {
                    UpdateText(enemyParent, enemy, mainCamera);
                    continue;
                }

                CreateTextLabel(enemy, mainCamera);
            }
        }

        private static void CreateTextLabel(Enemy enemy, Camera mainCamera)
        {
            GameObject textObject = new GameObject("Enemy_Label");
            TextMeshPro textMesh = textObject.AddComponent<TextMeshPro>();

            textMesh.text = "<color=red>NULL</color>";
            textMesh.fontSize = 3;
            textMesh.color = new Color(EC_R, EC_G, EC_B, EC_A);
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.enableWordWrapping = false;
            textMesh.isOverlay = true;
            textMesh.fontSharedMaterial = textMesh.font.material;

            textObject.transform.SetParent(enemy.transform, false);

            if (mainCamera != null)
            {
                textObject.transform.LookAt(mainCamera.transform);
                textObject.transform.Rotate(0, 180, 0);
            }

            trackedEnemies[enemy] = textMesh;
        }

        private static string GetEnemyInfo(EnemyParent enemyParent, Enemy enemy)
        {
            string enemyName = enemyParent.enemyName
                .Replace("Enemy -", "")
                .Replace("(Clone)", "")
                .Trim();
            
            string info = "□";

            if (showEnemyName)
                info += $"\n{enemyName}";

            if (showEnemyHealth)
            {
                var currentHealth = typeof(EnemyHealth)
                    .GetField("healthCurrent", BindingFlags.NonPublic | BindingFlags.Instance)?
                    .GetValue(enemy.GetComponent<EnemyHealth>());

                info += $"\n<b>{currentHealth}HP</b>";
            }

            return info;
        }

        private static void UpdateText(EnemyParent enemyParent, Enemy enemy, Camera mainCamera)
        {
            if (trackedEnemies.TryGetValue(enemy, out var textMesh) && mainCamera != null)
            {
                float distance = Vector3.Distance(mainCamera.transform.position, enemy.transform.position);
                float dynamicSize = Mathf.Clamp((0.2f + distance) - 1, 0.2f, enemyTextSize);
                
                textMesh.text = GetEnemyInfo(enemyParent, enemy);
                textMesh.fontSize = dynamicSize;
                textMesh.color = new Color(EC_R, EC_G, EC_B, EC_A);

                textMesh.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);
            }
        }

        private static void ClearEnemyLabels()
        {
            foreach (var enemy in trackedEnemies.Values)
            {
                if (enemy != null)
                    Destroy(enemy.gameObject);
            }

            trackedEnemies.Clear();
        }

        private void Update()
        {
            RenderEnemies();
        }
    }
}
