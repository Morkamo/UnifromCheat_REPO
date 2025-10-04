using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using static UnifromCheat_REPO.Core;

namespace UnifromCheat_REPO.WallHack
{
    public class EnemiesWallHack : MonoBehaviour
    {
        private static readonly Dictionary<Enemy, TextMeshPro> trackedEnemies = new();
        private static readonly Dictionary<Enemy, GameObject> enemyOutlineRoots = new();

        public static Material GlowMaterial;

        private static readonly int ZTest   = Shader.PropertyToID("_ZTest");
        private static readonly int Cull    = Shader.PropertyToID("_Cull");
        private static readonly int ZWrite  = Shader.PropertyToID("_ZWrite");
        private static readonly int ColorID = Shader.PropertyToID("_Color");

        public static void RenderEnemies()
        {
            var enemyParents = EnemyDirector.instance.enemiesSpawned;
            if (enemyParents == null || enemyParents.Count == 0)
            {
                ClearEnemyLabels();
                ClearAllOutlines();
                return;
            }

            var mainCamera = Camera.main;
            
            var glowColor = new Color(EC_R, EC_G, EC_B, EC_A);
            
            s_processedThisFrame.Clear();

            foreach (var enemyParent in enemyParents)
            {
                if (enemyParent == null) continue;

                var enemy = enemyParent.GetComponentInChildren<Enemy>();
                if (enemy == null || enemy.gameObject == null) continue;

                s_processedThisFrame.Add(enemy);
                
                if (isEnemyWallHackEnabled && showEnemyGlow)
                {
                    if (!enemyOutlineRoots.TryGetValue(enemy, out var root) || root == null)
                    {
                        var source = enemyParent.gameObject;
                        var newRoot = CreateWHCopy(source, glowColor);
                        enemyOutlineRoots[enemy] = newRoot;
                    }
                    else
                    {
                        root.SetActive(true);
                        UpdateWHCopyColor(root, glowColor);
                    }
                }
                else
                {
                    RemoveOutline(enemy);
                }
                
                if (isEnemyWallHackEnabled)
                {
                    if (trackedEnemies.ContainsKey(enemy))
                        UpdateText(enemyParent, enemy, mainCamera);
                    else
                        CreateTextLabel(enemy, mainCamera);
                }
            }
            
            if (!isEnemyWallHackEnabled)
                ClearEnemyLabels();
            
            SweepMissingOutlines();
        }
        
        private static GameObject CreateWHCopy(GameObject source, Color color)
        {
            if (source == null) return null;

            var root = new GameObject(source.name + "_WHRoot_Enemy");
            root.hideFlags = HideFlags.DontSave;

            // MeshRenderer
            foreach (var mr in source.GetComponentsInChildren<MeshRenderer>(true))
            {
                if (mr == null || !mr.enabled) continue;
                if (mr.GetComponent<TextMeshPro>() != null) continue;

                var mf = mr.GetComponent<MeshFilter>();
                if (mf == null || mf.sharedMesh == null) continue;

                var go = new GameObject(mf.gameObject.name + "_WH");
                go.transform.SetParent(root.transform, false);

                var follower = go.AddComponent<OutlineFollower>(); 
                follower.Init(mf.transform);

                var newMf = go.AddComponent<MeshFilter>(); 
                newMf.sharedMesh = mf.sharedMesh;

                var newMr = go.AddComponent<MeshRenderer>(); 
                newMr.material = BuildWHMaterial(color);
            }
            
            foreach (var smr in source.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            {
                if (smr == null || !smr.enabled || smr.sharedMesh == null) continue;

                var go = new GameObject(smr.gameObject.name + "_WH_Skinned");
                go.transform.SetParent(root.transform, false);

                var follower = go.AddComponent<OutlineFollower>(); 
                follower.Init(smr.transform);

                var newSmr = go.AddComponent<SkinnedMeshRenderer>();
                newSmr.sharedMesh = smr.sharedMesh;
                newSmr.rootBone   = smr.rootBone;
                newSmr.bones      = smr.bones;
                newSmr.updateWhenOffscreen = true;
                newSmr.material   = BuildWHMaterial(color);
            }

            return root;
        }

        private static Material BuildWHMaterial(Color c)
        {
            var mat = new Material(Shader.Find("Hidden/Internal-Colored")) { hideFlags = HideFlags.HideAndDontSave };
            mat.SetInt(ZTest, (int)UnityEngine.Rendering.CompareFunction.Always);
            mat.SetInt(Cull, (int)UnityEngine.Rendering.CullMode.Front);
            mat.SetInt(ZWrite, 0);
            mat.SetColor(ColorID, c);
            return mat;
        }

        private static void UpdateWHCopyColor(GameObject root, Color c)
        {
            if (root == null) return;
            foreach (var r in root.GetComponentsInChildren<Renderer>())
                if (r != null && r.material != null) r.material.color = c;
        }

        private static void RemoveOutline(Enemy enemy)
        {
            if (enemy == null) return;
            if (enemyOutlineRoots.TryGetValue(enemy, out var root) && root != null)
                Object.Destroy(root);
            enemyOutlineRoots.Remove(enemy);
        }

        private static void ClearAllOutlines()
        {
            foreach (var kv in enemyOutlineRoots)
                if (kv.Value != null) Object.Destroy(kv.Value);
            enemyOutlineRoots.Clear();
        }

        private static readonly HashSet<Enemy> s_processedThisFrame = new();
        private static void SweepMissingOutlines()
        {
            s_tmpToRemove.Clear();
            foreach (var kv in enemyOutlineRoots)
                if (kv.Key == null || !s_processedThisFrame.Contains(kv.Key))
                    s_tmpToRemove.Add(kv.Key);

            foreach (var e in s_tmpToRemove) RemoveOutline(e);
            s_processedThisFrame.Clear();
        }
        private static readonly List<Enemy> s_tmpToRemove = new();

        private class OutlineFollower : MonoBehaviour
        {
            private Transform target;
            public void Init(Transform t) => target = t;

            private void LateUpdate()
            {
                if (target == null) { Destroy(gameObject); return; }
                var trs = transform;
                trs.position = target.position;
                trs.rotation = target.rotation;
                trs.localScale = target.lossyScale + new Vector3(0.01f, 0.01f, 0.01f);
            }
        }

        private static void CreateTextLabel(Enemy enemy, Camera mainCamera)
        {
            GameObject textObject = new GameObject("Enemy_Label");
            TextMeshPro textMesh = textObject.AddComponent<TextMeshPro>();

            textMesh.text = "<color=red></color>";
            textMesh.fontSize = 3;
            
            if (ewh_syncTextColorWithGlow) textMesh.color = new Color(EC_R, EC_G, EC_B, EC_A);
            else textMesh.color = new Color(TEC_R, TEC_G, TEC_B, TEC_A);
            
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.enableWordWrapping = false;
            textMesh.isOverlay = true;

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

            string info = "";

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
                
                if (ewh_syncTextColorWithGlow) textMesh.color = new Color(EC_R, EC_G, EC_B, EC_A);
                else textMesh.color = new Color(TEC_R, TEC_G, TEC_B, TEC_A);

                textMesh.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);
            }
        }

        private static void ClearEnemyLabels()
        {
            foreach (var enemy in trackedEnemies.Values)
                if (enemy != null) Object.Destroy(enemy.gameObject);
            trackedEnemies.Clear();
        }
        
        public static void ClearAll()
        {
            foreach (var label in trackedEnemies.Values)
                if (label != null) Object.Destroy(label.gameObject);
            trackedEnemies.Clear();
            
            foreach (var root in enemyOutlineRoots.Values)
                if (root != null) Object.Destroy(root);
            enemyOutlineRoots.Clear();
        }

        private void Update()
        {
            if (Core.WH_BlockUpdates) 
                return;
            
            RenderEnemies();
        }
    }
}
