using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnifromCheat_REPO.Core;
using Object = UnityEngine.Object;

namespace UnifromCheat_REPO.WallHack
{
    public class CosmeticBoxesWallHack : MonoBehaviour
    {
        private static readonly Dictionary<CosmeticWorldObject, TextMeshPro> trackedBoxes = new();
        private static readonly Dictionary<CosmeticWorldObject, GameObject> boxOutlines = new();

        private static readonly List<CosmeticWorldObject> s_seenThisUpdate = new();
        private static readonly List<CosmeticWorldObject> s_tmpToRemove = new();

        private const float BoxesUpdateInterval = 0.07f;
        private float boxesTimer;

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            ClearAll();
        }

        private void Update()
        {
            if (Core.WH_BlockUpdates)
                return;

            boxesTimer += Time.deltaTime;
            if (boxesTimer >= BoxesUpdateInterval)
            {
                boxesTimer = 0f;
                RenderCosmeticBoxes();
            }
        }

        private static void RenderCosmeticBoxes()
        {
            if (RoundDirector.instance == null || RoundDirector.instance.cosmeticWorldObjects == null)
            {
                ClearAll();
                return;
            }

            if (!isCosmeticBoxesWallHackEnabled)
            {
                SetActiveAll(false);
                return;
            }

            var boxes = RoundDirector.instance?.cosmeticWorldObjects;
            if (boxes == null || boxes.Count == 0)
            {
                ClearMissing(null);
                return;
            }

            Camera cam = Camera.main;
            if (cam == null) return;

            s_seenThisUpdate.Clear();

            foreach (var box in boxes)
            {
                if (box == null || box.gameObject == null) continue;

                s_seenThisUpdate.Add(box);

                if (!trackedBoxes.ContainsKey(box))
                {
                    trackedBoxes[box] = Create3DText(box, cam);
                    boxOutlines[box] = CreateWHCopy(box.gameObject, new Color(CBC_R, CBC_G, CBC_B, CBC_A));
                }
                else
                {
                    trackedBoxes[box].gameObject.SetActive(true);
                    if (boxOutlines.TryGetValue(box, out var outline) && outline != null)
                        outline.SetActive(true);
                }

                UpdateText(box, cam);
                UpdateOutline(box);
            }

            ClearMissing(s_seenThisUpdate);
        }

        private static TextMeshPro Create3DText(CosmeticWorldObject box, Camera cam)
        {
            var go = new GameObject($"CosmeticBoxText_{box.name}");
            go.hideFlags = HideFlags.DontSave;

            var tmp = go.AddComponent<TextMeshPro>();
            tmp.fontSize = 3;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.enableWordWrapping = false;
            tmp.isOverlay = true;
            WallHackRenderUtils.ConfigureOverlayText(tmp);
            var follower = go.AddComponent<CosmeticBoxTextFollower>();
            follower.Init(tmp, box, -1f);

            UpdateTextTransform(tmp, box, cam, -1f);
            tmp.text = GetBoxInfo(box);
            tmp.color = GetTextColor(box);
            return tmp;
        }

        private static void UpdateText(CosmeticWorldObject box, Camera cam)
        {
            if (!trackedBoxes.TryGetValue(box, out var tmp) || cam == null) return;

            float distance = Vector3.Distance(cam.transform.position, box.transform.position);
            tmp.fontSize = Mathf.Clamp(0.2f + distance - 2f, 0.5f, cosmeticBoxTextSize);
            tmp.text = GetBoxInfo(box);
            WallHackRenderUtils.SetTextColor(tmp, GetTextColor(box));

            UpdateTextTransform(tmp, box, cam, -1f);
        }

        private static void UpdateTextTransform(TextMeshPro tmp, CosmeticWorldObject box, Camera cam, float offsetY)
        {
            Renderer rend = box.GetComponent<Renderer>();
            Vector3 center = rend != null ? rend.bounds.center : box.transform.position;
            float yOffset = rend != null ? rend.bounds.size.y : 1f;

            tmp.transform.position = center + new Vector3(0f, yOffset + offsetY, 0f);
            WallHackRenderUtils.FaceTextToCamera(tmp, cam);
        }

        private static string GetBoxInfo(CosmeticWorldObject box)
        {
            if (!showCosmeticBoxRarity)
                return "\nCosmetic Box";

            string rarityColor = ColorUtility.ToHtmlStringRGBA(GetRarityTextColor(box));
            return $"\nCosmetic Box\n<b><color=#{rarityColor}>({box.rarity})</color></b>";
        }

        private static Color GetTextColor(CosmeticWorldObject box)
        {
            return cosmeticBoxTextSyncWithGlow
                ? new Color(CBC_R, CBC_G, CBC_B, CBC_A)
                : new Color(CBTC_R, CBTC_G, CBTC_B, CBTC_A);
        }

        private static Color GetRarityTextColor(CosmeticWorldObject box)
        {
            return cosmetic_box_rarity_text_color
                ? new Color(CBRTC_R, CBRTC_G, CBRTC_B, CBRTC_A)
                : GetTextColor(box);
        }

        private static void UpdateOutline(CosmeticWorldObject box)
        {
            if (!boxOutlines.TryGetValue(box, out var outlineObj) || outlineObj == null) return;

            Color color = new Color(CBC_R, CBC_G, CBC_B, CBC_A);
            foreach (var r in outlineObj.GetComponentsInChildren<Renderer>())
                WallHackRenderUtils.SetRendererColor(r, color);
        }

        private static GameObject CreateWHCopy(GameObject source, Color color)
        {
            if (source == null) return null;

            var root = new GameObject(source.name + "_WHRoot_CosmeticBox");
            root.hideFlags = HideFlags.DontSave;

            foreach (var mr in source.GetComponentsInChildren<MeshRenderer>(true))
            {
                if (mr == null) continue;
                if (mr.GetComponent<TextMeshPro>() != null) continue;

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
                WallHackRenderUtils.AssignOverlayMaterial(newMr, BuildWHMaterial(color));
                WallHackRenderUtils.ConfigureOverlayRenderer(newMr);
            }

            return root;
        }

        private static Material BuildWHMaterial(Color c)
        {
            return WallHackRenderUtils.CreateOverlayMaterial(c);
        }

        private static void SetActiveAll(bool active)
        {
            foreach (var text in trackedBoxes.Values)
                if (text != null) text.gameObject.SetActive(active);

            foreach (var outline in boxOutlines.Values)
                if (outline != null) outline.SetActive(active);
        }

        private static void ClearMissing(List<CosmeticWorldObject> currentBoxes)
        {
            s_tmpToRemove.Clear();

            foreach (var box in trackedBoxes.Keys)
                if (box == null || currentBoxes == null || !currentBoxes.Contains(box))
                    s_tmpToRemove.Add(box);

            foreach (var box in s_tmpToRemove)
                RemoveBox(box);
        }

        private static void RemoveBox(CosmeticWorldObject box)
        {
            if (trackedBoxes.TryGetValue(box, out var tmp) && tmp != null)
                Object.Destroy(tmp.gameObject);
            trackedBoxes.Remove(box);

            if (boxOutlines.TryGetValue(box, out var outline) && outline != null)
                Object.Destroy(outline);
            boxOutlines.Remove(box);
        }

        public static void ClearAll()
        {
            foreach (var tmp in trackedBoxes.Values)
                if (tmp != null) Object.Destroy(tmp.gameObject);
            trackedBoxes.Clear();

            foreach (var outline in boxOutlines.Values)
                if (outline != null) Object.Destroy(outline);
            boxOutlines.Clear();
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

        private class CosmeticBoxTextFollower : MonoBehaviour
        {
            private TextMeshPro textMesh;
            private CosmeticWorldObject target;
            private float offsetY;

            public void Init(TextMeshPro text, CosmeticWorldObject box, float yOffset)
            {
                textMesh = text;
                target = box;
                offsetY = yOffset;
            }

            private void LateUpdate()
            {
                if (textMesh == null)
                {
                    Destroy(this);
                    return;
                }

                if (target == null || target.gameObject == null)
                {
                    Destroy(textMesh.gameObject);
                    return;
                }

                Camera cam = Camera.main;
                if (cam == null) return;

                UpdateTextTransform(textMesh, target, cam, offsetY);
            }
        }
    }
}
