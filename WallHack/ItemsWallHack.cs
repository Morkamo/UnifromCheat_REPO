using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnifromCheat_REPO.Core;
using static UnifromCheat_REPO.Utils.FireboxConsole;
using Object = UnityEngine.Object;

namespace UnifromCheat_REPO.WallHack
{
    public class ItemsWallHack : MonoBehaviour
    {
        public static ItemsWallHack Instance;

        private static readonly Dictionary<ValuableObject, TextMeshPro> trackedItems = new();
        private static readonly Dictionary<ValuableObject, GameObject> itemOutlines = new();

        internal static readonly Dictionary<SurplusValuable, GameObject> surplusOutlines = new();
        private static readonly Dictionary<ExtractionPoint, GameObject> extractionOutlines = new();

        private static float itemsUpdateInterval = 0.07f;
        private static float epColorUpdateInterval = 0.20f;
        private static int epWarmupProbes = 10;
        private static float epWarmupPeriod = 0.50f;

        private float itemsTimer;
        private Coroutine epColorUpdaterCo;
        private Coroutine epWarmupCo;

        private static readonly int ColorID = Shader.PropertyToID("_Color");
        private static readonly int ZTest = Shader.PropertyToID("_ZTest");
        private static readonly int Cull = Shader.PropertyToID("_Cull");
        private static readonly int ZWrite = Shader.PropertyToID("_ZWrite");

        private void OnEnable()
        {
            Instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            epColorUpdaterCo = StartCoroutine(EPColorUpdaterLoop());
        }

        private void OnDisable()
        {
            if (epColorUpdaterCo != null) StopCoroutine(epColorUpdaterCo);
            if (epWarmupCo != null) StopCoroutine(epWarmupCo);

            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            ClearExtractionCache();
            FireLog($"[WH] Scene loaded: {scene.name}, EP cache cleared");

            if (showExtractionPoints)
            {
                AddMissingExtractionPoints();
                RestartEPWarmup();
            }
        }

        private void RestartEPWarmup()
        {
            if (epWarmupCo != null) StopCoroutine(epWarmupCo);
            epWarmupCo = StartCoroutine(EPWarmupRoutine(epWarmupProbes, epWarmupPeriod));
        }

        private IEnumerator EPWarmupRoutine(int probes, float period)
        {
            for (int i = 0; i < probes; i++)
            {
                if (!showExtractionPoints) yield break;

                bool added = AddMissingExtractionPoints();
                if (added) FireLog($"[WH] EP warmup: added new points (step {i + 1}/{probes})");

                yield return new WaitForSeconds(period);
            }
        }

        private IEnumerator EPColorUpdaterLoop()
        {
            var wait = new WaitForSeconds(epColorUpdateInterval);
            while (true)
            {
                if (showExtractionPoints && extractionOutlines.Count > 0)
                    UpdateExtractionOutlines();
                
                if (surplusOutlines.Count > 0)
                    UpdateSurplusOutlines();

                yield return wait;
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private void Update()
        {
            itemsTimer += Time.deltaTime;
            if (itemsTimer >= itemsUpdateInterval)
            {
                itemsTimer = 0f;
                RenderItems(); 
                // Surplus теперь обновляется через UpdateSurplusOutlines()
            }
        }

        // ============================= EXTRACTION =============================

        public static bool CacheExtractionPointsFull()
        {
            ClearExtractionCache();
            return AddMissingExtractionPoints();
        }

        private static bool AddMissingExtractionPoints()
        {
            bool addedAny = false;

            var eps = GameObject.FindObjectsOfType<ExtractionPoint>(true);
            if (eps == null || eps.Length == 0) return false;

            foreach (var ep in eps)
            {
                if (ep == null) continue;
                if (extractionOutlines.ContainsKey(ep)) continue;

                var outline = CreateWHCopy(ep.gameObject, new Color(EPC_R, EPC_G, EPC_B, EPC_A));
                if (outline != null)
                {
                    extractionOutlines[ep] = outline;
                    addedAny = true;
                }
            }

            return addedAny;
        }

        private static void UpdateExtractionOutlines()
        {
            Color color = new Color(EPC_R, EPC_G, EPC_B, EPC_A);

            s_tmpEPToRemove.Clear();

            foreach (var kvp in extractionOutlines)
            {
                var root = kvp.Value;
                if (kvp.Key == null || root == null) { s_tmpEPToRemove.Add(kvp.Key); continue; }

                foreach (var r in root.GetComponentsInChildren<MeshRenderer>())
                    if (r != null && r.material != null)
                        r.material.color = color;
            }

            foreach (var dead in s_tmpEPToRemove)
            {
                if (dead != null && extractionOutlines.TryGetValue(dead, out var go) && go != null)
                    Object.Destroy(go);
                extractionOutlines.Remove(dead);
            }
        }
        private static readonly List<ExtractionPoint> s_tmpEPToRemove = new();

        public static void ClearExtractionCache()
        {
            foreach (var outline in extractionOutlines.Values)
                if (outline != null)
                    Object.Destroy(outline);
            extractionOutlines.Clear();
        }

        // ============================= VALUABLES =============================

        public static void RenderItems()
        {
            if (!isItemWallHackEnabled)
            {
                SetActiveAll(false);
                return;
            }

            var list = ValuableDirector.instance.valuableList;
            if (list == null) return;

            Camera cam = Camera.main;
            if (cam == null) return;

            foreach (var item in list)
            {
                if (item == null || item.gameObject == null) continue;

                if (!trackedItems.ContainsKey(item))
                {
                    trackedItems[item] = Create3DText(item, cam);
                    itemOutlines[item] = CreateWHCopy(item.gameObject, new Color(IC_R, IC_G, IC_B, IC_A));
                }
                else
                {
                    trackedItems[item].gameObject.SetActive(true);
                    if (itemOutlines.TryGetValue(item, out var o) && o != null)
                        o.SetActive(true);
                }

                UpdateText(item, cam);
                UpdateItemOutline(item);
            }
        }

        // ============================= SURPLUS =============================

        private static void UpdateSurplusOutlines()
        {
            Color color = new Color(SPC_R, SPC_G, SPC_B, SPC_A);

            s_tmpSurplusToRemove.Clear();

            foreach (var kvp in surplusOutlines)
            {
                var root = kvp.Value;
                if (kvp.Key == null || root == null) { s_tmpSurplusToRemove.Add(kvp.Key); continue; }

                // Активность зависит от WH и showSurplusValuable
                root.SetActive(isItemWallHackEnabled && showSurplusValuable);

                if (!root.activeSelf) continue;

                foreach (var r in root.GetComponentsInChildren<MeshRenderer>())
                    if (r != null && r.material != null)
                        r.material.color = color;
            }

            foreach (var dead in s_tmpSurplusToRemove)
            {
                if (dead != null && surplusOutlines.TryGetValue(dead, out var go) && go != null)
                    Object.Destroy(go);
                surplusOutlines.Remove(dead);
            }
        }

        private static readonly List<SurplusValuable> s_tmpSurplusToRemove = new();

        // ============================= SHARED =============================

        internal static GameObject CreateWHCopy(GameObject source, Color color)
        {
            var root = new GameObject(source.name + "_WHRoot");

            foreach (var mr in source.GetComponentsInChildren<MeshRenderer>(true))
            {
                if (mr == null || !mr.enabled) continue;

                var mf = mr.GetComponent<MeshFilter>();
                if (mf == null || mf.sharedMesh == null) continue;

                if (mf.sharedMesh.name == "Cube")
                    continue;

                var go = new GameObject(mf.gameObject.name + "_WH");
                go.transform.SetParent(root.transform, false);

                var follower = go.AddComponent<OutlineFollower>();
                follower.Init(mf.transform);

                var newMf = go.AddComponent<MeshFilter>();
                newMf.sharedMesh = mf.sharedMesh;

                var newMr = go.AddComponent<MeshRenderer>();
                var mat = new Material(Shader.Find("Hidden/Internal-Colored"))
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
                mat.SetInt(ZTest, (int)UnityEngine.Rendering.CompareFunction.Always);
                mat.SetInt(Cull, (int)UnityEngine.Rendering.CullMode.Front);
                mat.SetInt(ZWrite, 0);
                mat.SetColor(ColorID, color);
                newMr.material = mat;
            }

            return root;
        }

        private static TextMeshPro Create3DText(ValuableObject item, Camera cam)
        {
            var go = new GameObject($"ItemText_{item.name}");
            go.transform.SetParent(item.transform, false);

            var tmp = go.AddComponent<TextMeshPro>();
            tmp.fontSize = 3;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.enableWordWrapping = false;
            tmp.isOverlay = true;
            tmp.color = new Color(IC_R, IC_G, IC_B, IC_A);
            tmp.fontSharedMaterial = tmp.font.material;

            UpdateTextTransform(tmp, item, cam, -1f);
            tmp.text = GetItemInfo(item);
            return tmp;
        }

        private static void UpdateText(ValuableObject item, Camera cam)
        {
            if (!trackedItems.TryGetValue(item, out var tmp) || cam == null) return;

            float distance = Vector3.Distance(cam.transform.position, item.transform.position);
            tmp.fontSize = Mathf.Clamp(0.2f + distance - 2f, 0.5f, itemTextSize);

            bool inPriceRange = GetDollarValueCurrent(item) >= sortFromPrice &&
                                GetDollarValueCurrent(item) <= sortToPrice;

            tmp.text = sortByPrice && !inPriceRange ? string.Empty : GetItemInfo(item);
            tmp.color = new Color(IC_R, IC_G, IC_B, IC_A);

            UpdateTextTransform(tmp, item, cam, -1f);
        }

        private static void UpdateTextTransform(TextMeshPro tmp, ValuableObject item, Camera cam, float offsetY)
        {
            Renderer rend = item.GetComponent<Renderer>();
            Vector3 center = rend != null ? rend.bounds.center : item.transform.position;
            float yOffset = rend != null ? rend.bounds.size.y : 1f;

            tmp.transform.position = center + new Vector3(0f, yOffset + offsetY, 0f);
            tmp.transform.rotation = Quaternion.LookRotation(cam.transform.position - tmp.transform.position) *
                                     Quaternion.Euler(0, 180f, 0);
        }

        private static void UpdateItemOutline(ValuableObject item)
        {
            if (!itemOutlines.TryGetValue(item, out var outlineObj) || outlineObj == null) return;

            bool inPriceRange = GetDollarValueCurrent(item) >= sortFromPrice &&
                                GetDollarValueCurrent(item) <= sortToPrice;
            outlineObj.SetActive(!sortByPrice || inPriceRange);

            Color color = new Color(IC_R, IC_G, IC_B, IC_A);
            foreach (var r in outlineObj.GetComponentsInChildren<MeshRenderer>())
                if (r != null && r.material != null)
                    r.material.color = color;
        }

        private static string GetItemInfo(ValuableObject item)
        {
            string name = item.name.Replace("Valuable", "").Replace("(Clone)", "").Trim();
            string text = "";
            if (showItemName) text += "\n" + name;
            if (showItemPrice) text += $"\n<b>{GetDollarValueCurrent(item)}$</b>";
            return text;
        }

        private static float GetDollarValueCurrent(ValuableObject target)
        {
            if (target == null) return 0f;

            var type = target.GetType();
            var field = type.GetField("dollarValueCurrent", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (field != null) return (float)field.GetValue(target);

            var prop = type.GetProperty("dollarValueCurrent", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            return prop != null ? (float)prop.GetValue(target) : 0f;
        }

        private static void SetActiveAll(bool active)
        {
            foreach (var t in trackedItems.Values)
                if (t != null) t.gameObject.SetActive(active);

            foreach (var o in itemOutlines.Values)
                if (o != null) o.SetActive(active);

            foreach (var o in surplusOutlines.Values)
                if (o != null) o.SetActive(active && showSurplusValuable);

            foreach (var ep in extractionOutlines.Values)
                if (ep != null) ep.SetActive(active);
        }

        public void OnEPVisibilityEnabledFromUI()
        {
            AddMissingExtractionPoints();
            RestartEPWarmup();
        }

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
    }
}
