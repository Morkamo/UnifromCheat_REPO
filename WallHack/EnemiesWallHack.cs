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

                if (!TryGetEnemyVisualBounds(enemy, out var visualBounds))
                {
                    RemoveOutline(enemy);
                    RemoveLabel(enemy);
                    continue;
                }

                s_processedThisFrame.Add(enemy);
                
                if (isEnemyWallHackEnabled && showEnemyGlow)
                {
                    if (!enemyOutlineRoots.TryGetValue(enemy, out var root) || root == null || !HasLiveOutlineRenderers(root))
                    {
                        RemoveOutline(enemy);
                        var source = GetEnemyVisualRoot(enemy);
                        var newRoot = CreateWHCopy(source, glowColor);
                        if (newRoot != null)
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
                    {
                        if (trackedEnemies[enemy] != null)
                            trackedEnemies[enemy].gameObject.SetActive(true);
                        UpdateText(enemyParent, enemy, mainCamera, visualBounds);
                    }
                    else
                    {
                        CreateTextLabel(enemy, mainCamera, visualBounds);
                    }
                }
            }
            
            if (!isEnemyWallHackEnabled)
                ClearEnemyLabels();
            
            SweepMissing();
        }
        
        private static GameObject CreateWHCopy(GameObject source, Color color)
        {
            if (source == null) return null;

            var root = new GameObject(source.name + "_WHRoot_Enemy");
            root.hideFlags = HideFlags.DontSave;
            int copiedRenderers = 0;

            foreach (var mr in source.GetComponentsInChildren<MeshRenderer>(true))
            {
                if (!ShouldUseEnemyRenderer(mr)) continue;

                var mf = mr.GetComponent<MeshFilter>();
                if (mf == null || mf.sharedMesh == null) continue;
                if (ShouldSkipEnemyMesh(mf.sharedMesh, mr.transform)) continue;

                var go = new GameObject(mf.gameObject.name + "_WH");
                go.transform.SetParent(root.transform, false);

                var follower = go.AddComponent<OutlineFollower>(); 
                follower.Init(mf.transform);

                var newMf = go.AddComponent<MeshFilter>(); 
                newMf.sharedMesh = mf.sharedMesh;

                var newMr = go.AddComponent<MeshRenderer>(); 
                newMr.material = BuildWHMaterial(color);
                WallHackRenderUtils.ConfigureOverlayRenderer(newMr);
                copiedRenderers++;
            }
            
            foreach (var smr in source.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            {
                if (!ShouldUseEnemyRenderer(smr)) continue;
                if (smr.sharedMesh == null) continue;
                if (ShouldSkipEnemyMesh(smr.sharedMesh, smr.transform)) continue;

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
                WallHackRenderUtils.ConfigureOverlayRenderer(newSmr);
                copiedRenderers++;
            }

            if (copiedRenderers == 0)
            {
                Object.Destroy(root);
                return null;
            }

            return root;
        }

        private static GameObject GetEnemyVisualRoot(Enemy enemy)
        {
            if (enemy == null) return null;

            var health = enemy.GetComponent<EnemyHealth>();
            if (health != null && health.meshParent != null)
                return health.meshParent.gameObject;

            return enemy.gameObject;
        }

        private static bool ShouldUseEnemyRenderer(Renderer renderer)
        {
            if (renderer == null || !renderer.enabled)
                return false;

            if (renderer.GetComponent<TextMeshPro>() != null)
                return false;

            string path = GetTransformPath(renderer.transform).ToLowerInvariant();
            return !path.Contains("collider")
                   && !path.Contains("hurtcollider")
                   && !path.Contains("trigger")
                   && !path.Contains("vision")
                   && !path.Contains("detect")
                   && !path.Contains("detection")
                   && !path.Contains("checker")
                   && !path.Contains("collision")
                   && !path.Contains("playercollision")
                   && !path.Contains("zone")
                   && !path.Contains("volume")
                   && !path.Contains("bounds")
                   && !path.Contains("range")
                   && !path.Contains("area")
                   && !path.Contains("ray");
        }

        private static bool ShouldSkipEnemyMesh(Mesh mesh, Transform transform)
        {
            if (mesh == null) return true;

            string meshName = mesh.name.ToLowerInvariant();
            if (meshName == "cube" || meshName.Contains("collider") || meshName.Contains("trigger"))
                return true;

            string objectName = transform != null ? transform.name.ToLowerInvariant() : string.Empty;
            return objectName.Contains("collider")
                   || objectName.Contains("hurtcollider")
                   || objectName.Contains("trigger")
                   || objectName.Contains("zone");
        }

        private static string GetTransformPath(Transform transform)
        {
            if (transform == null) return string.Empty;

            string path = transform.name;
            Transform current = transform.parent;
            while (current != null)
            {
                path = current.name + "/" + path;
                current = current.parent;
            }

            return path;
        }

        private static bool TryGetEnemyVisualBounds(Enemy enemy, out Bounds bounds)
        {
            bounds = default;

            GameObject visualRoot = GetEnemyVisualRoot(enemy);
            if (visualRoot == null || !visualRoot.activeInHierarchy)
                return false;

            bool found = false;
            foreach (var renderer in visualRoot.GetComponentsInChildren<Renderer>(true))
            {
                if (!ShouldUseEnemyRenderer(renderer))
                    continue;

                if (renderer is MeshRenderer meshRenderer)
                {
                    var meshFilter = meshRenderer.GetComponent<MeshFilter>();
                    if (meshFilter == null || ShouldSkipEnemyMesh(meshFilter.sharedMesh, renderer.transform))
                        continue;
                }
                else if (renderer is SkinnedMeshRenderer skinnedMeshRenderer)
                {
                    if (ShouldSkipEnemyMesh(skinnedMeshRenderer.sharedMesh, renderer.transform))
                        continue;
                }

                if (!renderer.gameObject.activeInHierarchy || renderer.bounds.size.sqrMagnitude <= 0.0001f)
                    continue;

                if (!found)
                {
                    bounds = renderer.bounds;
                    found = true;
                }
                else
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            if (!found)
                return false;

            return bounds.size.sqrMagnitude > 0.0001f;
        }

        private static bool HasLiveOutlineRenderers(GameObject root)
        {
            if (root == null) return false;

            foreach (var renderer in root.GetComponentsInChildren<Renderer>())
                if (renderer != null)
                    return true;

            return false;
        }

        private static Material BuildWHMaterial(Color c)
        {
            return WallHackRenderUtils.CreateOverlayMaterial(c);
        }

        private static void UpdateWHCopyColor(GameObject root, Color c)
        {
            if (root == null) return;
            foreach (var r in root.GetComponentsInChildren<Renderer>())
                WallHackRenderUtils.SetRendererColor(r, c);
        }

        private static void RemoveOutline(Enemy enemy)
        {
            if (ReferenceEquals(enemy, null)) return;
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
        private static void SweepMissing()
        {
            s_tmpToRemove.Clear();
            foreach (var kv in enemyOutlineRoots)
                if (kv.Key == null || !s_processedThisFrame.Contains(kv.Key))
                    s_tmpToRemove.Add(kv.Key);

            foreach (var e in s_tmpToRemove) RemoveOutline(e);

            s_tmpToRemove.Clear();
            foreach (var kv in trackedEnemies)
                if (kv.Key == null || !s_processedThisFrame.Contains(kv.Key))
                    s_tmpToRemove.Add(kv.Key);

            foreach (var e in s_tmpToRemove) RemoveLabel(e);
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

        private static void CreateTextLabel(Enemy enemy, Camera mainCamera, Bounds visualBounds)
        {
            GameObject textObject = new GameObject("Enemy_Label");
            textObject.hideFlags = HideFlags.DontSave;
            TextMeshPro textMesh = textObject.AddComponent<TextMeshPro>();

            textMesh.text = "<color=red></color>";
            textMesh.fontSize = 3;
            
            if (ewh_syncTextColorWithGlow) textMesh.color = new Color(EC_R, EC_G, EC_B, EC_A);
            else textMesh.color = new Color(TEC_R, TEC_G, TEC_B, TEC_A);
            
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.enableWordWrapping = false;
            textMesh.isOverlay = true;
            WallHackRenderUtils.ConfigureOverlayText(textMesh);

            if (mainCamera != null)
            {
                UpdateTextTransform(textMesh, visualBounds, mainCamera);
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

        private static void UpdateText(EnemyParent enemyParent, Enemy enemy, Camera mainCamera, Bounds visualBounds)
        {
            if (trackedEnemies.TryGetValue(enemy, out var textMesh) && mainCamera != null)
            {
                float distance = Vector3.Distance(mainCamera.transform.position, visualBounds.center);
                float dynamicSize = Mathf.Clamp((0.2f + distance) - 1, 0.2f, enemyTextSize);

                textMesh.text = GetEnemyInfo(enemyParent, enemy);
                textMesh.fontSize = dynamicSize;
                
                Color textColor = ewh_syncTextColorWithGlow
                    ? new Color(EC_R, EC_G, EC_B, EC_A)
                    : new Color(TEC_R, TEC_G, TEC_B, TEC_A);
                WallHackRenderUtils.SetTextColor(textMesh, textColor);

                UpdateTextTransform(textMesh, visualBounds, mainCamera);
            }
        }

        private static void UpdateTextTransform(TextMeshPro textMesh, Bounds visualBounds, Camera mainCamera)
        {
            Vector3 center = visualBounds.center;
            float yOffset = Mathf.Max(visualBounds.size.y, 1f);

            textMesh.transform.position = center + Vector3.up * (yOffset + 0.5f);
            WallHackRenderUtils.FaceTextToCamera(textMesh, mainCamera);
        }

        private static void RemoveLabel(Enemy enemy)
        {
            if (ReferenceEquals(enemy, null)) return;
            if (trackedEnemies.TryGetValue(enemy, out var label) && label != null)
                Object.Destroy(label.gameObject);
            trackedEnemies.Remove(enemy);
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
