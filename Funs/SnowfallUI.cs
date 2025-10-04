using System.Collections.Generic;
using UnifromCheat_REPO.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnifromCheat_REPO.Core;
using Random = UnityEngine.Random;

namespace UnifromCheat_REPO.Funs
{
    public class SnowfallUI : MonoBehaviour
    {
        public static SnowfallUI Instance;

        private GameObject snowflakePrefab;
        private RectTransform canvasRect;

        private float timer;
        private readonly List<RectTransform> flakes = new();
        private readonly Dictionary<RectTransform, float> flakeOffsets = new();
        private readonly Dictionary<RectTransform, int> flakeSides = new();

        private void Awake()
        {
            Instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            Instance = null;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void Update()
        {
            if (canvasRect == null) return;

            if (!enableProceduralSnowfall)
            {
                if (flakes.Count > 0)
                    ClearFlakes();
                return;
            }

            if (snowflakePrefab == null)
                InitSnowAnimation(Core.unifromCanvasObject.GetComponent<RectTransform>());

            if (ps_onlyInMenu && !Core.MenuState)
            {
                if (flakes.Count > 0)
                    ClearFlakes();
                return;
            }

            timer += Time.deltaTime;
            if (timer >= ps_spawnInterval)
            {
                timer = 0;
                SpawnFlake();
            }

            for (int i = flakes.Count - 1; i >= 0; i--)
            {
                var flake = flakes[i];
                if (flake == null) { flakes.RemoveAt(i); continue; }

                flake.anchoredPosition -= new Vector2(0, ps_fallSpeed * Time.deltaTime);

                float offset = Mathf.Sin(Time.time * 2f + flakeOffsets[flake]) * 20f;
                var pos = flake.anchoredPosition;
                pos.x += offset * Time.deltaTime;
                flake.anchoredPosition = pos;

                if (pss_spin)
                {
                    int side = flakeSides.TryGetValue(flake, out var flakeSide) ? flakeSide : 1;
                    flake.Rotate(Vector3.forward * (pss_spinSpeed * side * Time.deltaTime));
                }

                if (flake.anchoredPosition.y < -canvasRect.rect.height / 2 - 20)
                {
                    Destroy(flake.gameObject);
                    flakes.RemoveAt(i);
                    flakeOffsets.Remove(flake);
                    flakeSides.Remove(flake);
                }
            }
        }

        private void SpawnFlake()
        {
            if (snowflakePrefab == null || canvasRect == null)
                return;

            var flakeObj = Instantiate(snowflakePrefab, canvasRect);
            var rt = flakeObj.GetComponent<RectTransform>();

            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);

            var rect = canvasRect.rect;
            float x = Random.Range(-rect.width / 2, rect.width / 2);
            rt.anchoredPosition = new Vector2(x, rect.height / 2 + 20);

            float scale = ps_dynamicScale
                ? Random.Range(ps_scaleRangeA, ps_scaleRangeB)
                : ps_scale;
            rt.localScale = new Vector3(scale, scale, scale);

            if (pss_dynamicRotateOffset)
                rt.localRotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

            int side = pss_dynamicSelectSide ? (Random.value > 0.5f ? 1 : -1) : (pss_side == 0 ? -1 : 1);
            flakeSides[rt] = side;

            flakes.Add(rt);
            flakeOffsets[rt] = Random.Range(0f, Mathf.PI * 2f);
        }

        public void InitSnowAnimation(RectTransform parentCanvas)
        {
            canvasRect = parentCanvas;

            snowflakePrefab = new GameObject("SnowflakePrefab");
            var img = snowflakePrefab.AddComponent<Image>();
            img.color = Color.white;

            ApplyFlakeTexture(img);

            img.type = Image.Type.Simple;
            var rt = snowflakePrefab.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(ps_scale, ps_scale);
        }

        public void RefreshFlakeTexture()
        {
            if (canvasRect == null || snowflakePrefab == null) return;
            var img = snowflakePrefab.GetComponent<Image>();
            if (img == null) return;

            ApplyFlakeTexture(img);
        }

        private void ApplyFlakeTexture(Image img)
        {
            Texture2D tex = pss_flakeTexture != null
                ? pss_flakeTexture
                : ResourceLoader.LoadTexture("UnifromCheat_REPO.Assets.snowflake.png");

            if (tex != null)
                img.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            ClearFlakes();

            if (unifromCanvasObject == null)
            {
                FireboxConsole.FireLog("Canvas is null, skip snow init");
                return;
            }

            InitSnowAnimation(unifromCanvasObject.GetComponent<RectTransform>());
            FireboxConsole.FireLog($"Snow reinitialized for scene {scene.name}");
        }

        private void ClearFlakes()
        {
            foreach (var flake in flakes)
                if (flake != null) Destroy(flake.gameObject);

            flakes.Clear();
            flakeOffsets.Clear();
            flakeSides.Clear();

            if (snowflakePrefab != null)
            {
                Destroy(snowflakePrefab);
                snowflakePrefab = null;
            }
        }
    }
}