using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnifromCheat_REPO.WallHack
{
    internal static class WallHackRenderUtils
    {
        private const int OverlayRenderQueue = 4000;

        private static readonly float[] NoLayerCullDistances = new float[32];
        private static readonly int ColorID = Shader.PropertyToID("_Color");
        private static readonly int FaceColorID = Shader.PropertyToID("_FaceColor");
        private static readonly int ZTest = Shader.PropertyToID("_ZTest");
        private static readonly int Cull = Shader.PropertyToID("_Cull");
        private static readonly int CullModeID = Shader.PropertyToID("_CullMode");
        private static readonly int ZWrite = Shader.PropertyToID("_ZWrite");

        public static Material CreateOverlayMaterial(Color color)
        {
            var material = new Material(Shader.Find("Hidden/Internal-Colored"))
            {
                hideFlags = HideFlags.HideAndDontSave,
                renderQueue = GetRenderQueue()
            };

            ConfigureOverlayMaterial(material, color);
            return material;
        }

        public static void ConfigureOverlayMaterial(Material material, Color color)
        {
            if (material == null) return;

            material.renderQueue = GetRenderQueue();

            if (material.HasProperty(ZTest))
                material.SetInt(ZTest, (int)CompareFunction.Always);
            if (material.HasProperty(Cull))
                material.SetInt(Cull, (int)CullMode.Front);
            if (material.HasProperty(CullModeID))
                material.SetInt(CullModeID, (int)CullMode.Front);
            if (material.HasProperty(ZWrite))
                material.SetInt(ZWrite, 0);
            if (material.HasProperty(ColorID))
                material.SetColor(ColorID, color);
            if (material.HasProperty(FaceColorID))
                material.SetColor(FaceColorID, color);
        }

        public static void ConfigureOverlayRenderer(Renderer renderer)
        {
            if (renderer == null) return;

            renderer.enabled = true;
            renderer.forceRenderingOff = false;
            renderer.allowOcclusionWhenDynamic = false;
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            renderer.sortingOrder = short.MaxValue;
        }

        public static void AssignOverlayMaterial(Renderer renderer, Material material)
        {
            if (renderer == null || material == null) return;

            renderer.sharedMaterial = material;
            var owner = renderer.GetComponent<WallHackOwnedMaterial>();
            if (owner == null)
                owner = renderer.gameObject.AddComponent<WallHackOwnedMaterial>();
            owner.Init(material);
        }

        public static void ConfigureOverlayText(TextMeshPro text)
        {
            if (text == null) return;

            text.isOverlay = true;
            text.extraPadding = true;

            Material material = text.fontSharedMaterial;
            if (material != null && !material.name.EndsWith("_UnifromWH"))
            {
                material = new Material(material)
                {
                    name = material.name + "_UnifromWH",
                    hideFlags = HideFlags.HideAndDontSave
                };
                text.fontSharedMaterial = material;

                var owner = text.GetComponent<WallHackOwnedMaterial>();
                if (owner == null)
                    owner = text.gameObject.AddComponent<WallHackOwnedMaterial>();
                owner.Init(material);
            }

            if (material != null)
            {
                material.renderQueue = GetRenderQueue();
                material.SetFloat(ShaderUtilities.ShaderTag_ZTestMode, 0f);
            }

            ConfigureOverlayRenderer(text.GetComponent<Renderer>());
            SetTextColor(text, text.color);
            text.UpdateMeshPadding();

            var billboard = text.GetComponent<WallHackTextBillboard>();
            if (billboard == null)
                billboard = text.gameObject.AddComponent<WallHackTextBillboard>();
            billboard.Init(text);
        }

        public static void SetTextColor(TextMeshPro text, Color color)
        {
            if (text == null) return;

            text.color = color;
            Material material = text.fontSharedMaterial;
            if (material == null) return;

            material.renderQueue = GetRenderQueue();
            material.SetFloat(ShaderUtilities.ShaderTag_ZTestMode, 0f);
        }

        public static void SetRendererColor(Renderer renderer, Color color)
        {
            if (renderer == null || renderer.sharedMaterial == null) return;

            Material material = renderer.sharedMaterial;
            material.renderQueue = GetRenderQueue();
            if (material.HasProperty(ColorID))
                material.SetColor(ColorID, color);
            if (material.HasProperty(FaceColorID))
                material.SetColor(FaceColorID, color);
        }

        private static int GetRenderQueue()
        {
            return OverlayRenderQueue;
        }

        public static void ApplyCameraOverrides(Camera camera)
        {
            if (camera == null)
                return;

            Core.ApplyRenderDistance(camera);

            if (!IsAnyWallHackEnabled())
                return;

            camera.layerCullSpherical = false;
            camera.layerCullDistances = NoLayerCullDistances;
            camera.useOcclusionCulling = false;
        }

        private static bool IsAnyWallHackEnabled()
        {
            return Core.isItemsWallHackEnabled
                   || Core.isEnemyWallHackEnabled
                   || Core.isPlayerWallHackEnabled
                   || Core.isCosmeticBoxesWallHackEnabled;
        }

        public static void FaceTextToCamera(TextMeshPro text, Camera camera)
        {
            if (text == null || camera == null) return;

            Vector3 direction = text.transform.position - camera.transform.position;
            if (direction.sqrMagnitude < 0.0001f)
                direction = camera.transform.forward;

            text.transform.rotation = Quaternion.LookRotation(direction.normalized, camera.transform.up);
        }
    }

    internal class WallHackOwnedMaterial : MonoBehaviour
    {
        private Material material;

        public void Init(Material ownedMaterial)
        {
            material = ownedMaterial;
        }

        private void OnDestroy()
        {
            if (material != null)
                Destroy(material);
            material = null;
        }
    }

    internal class WallHackTextBillboard : MonoBehaviour
    {
        private TextMeshPro text;

        public void Init(TextMeshPro textMesh)
        {
            text = textMesh;
        }

        private void LateUpdate()
        {
            if (text == null)
            {
                Destroy(this);
                return;
            }

            WallHackRenderUtils.FaceTextToCamera(text, Camera.main);
        }
    }
}
