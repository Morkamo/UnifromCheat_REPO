using System.Collections.Generic;
using UnityEngine;

namespace UnifromCheat_REPO
{
    public static class GUITooltip
    {
        private static readonly List<(string text, Rect rect)> tooltips = new();

        private static GUIStyle tooltipStyle;
        private static Texture2D tooltipBg;

        private static bool _initialized;

        public static void Init()
        {
            if (_initialized) return;

            tooltipStyle = new GUIStyle(GUI.skin.box)
            {
                alignment = TextAnchor.MiddleLeft,
                wordWrap = true,
                fontSize = 12,
                normal = { textColor = Color.white }
            };

            MakeBackground();

            GUIMenuSkin.OnSkinChanged += MakeBackground;

            _initialized = true;
        }

        private static void MakeBackground()
        {
            tooltipBg = MakeTex(4, 4, new Color(0f, 0f, 0f, Core.menuOpacity * 0.85f));
            tooltipStyle.normal.background = tooltipBg;
        }

        private static Texture2D MakeTex(int w, int h, Color c)
        {
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Point;
            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                    tex.SetPixel(x, y, c);
            tex.Apply(false, false);
            return tex;
        }

        /// <summary>
        /// Добавляет тултип для вывода поверх указанного rect-а.
        /// </summary>
        public static void Show(string text, Rect rect)
        {
            if (string.IsNullOrEmpty(text) || Core.HideAllTooltips) return;
            tooltips.Add((text, rect));
        }

        /// <summary>
        /// Вызывается в конце OnGUI для отрисовки всех накопленных тултипов.
        /// </summary>
        public static void Draw()
        {
            if (tooltips.Count == 0) return;

            foreach (var (text, rect) in tooltips)
            {
                if (!rect.Contains(Event.current.mousePosition)) continue;

                Vector2 size = tooltipStyle.CalcSize(new GUIContent(text));
                Rect tooltipRect = new Rect(Event.current.mousePosition.x + 15,
                                            Event.current.mousePosition.y + 15,
                                            size.x + 8,
                                            size.y + 6);

                GUI.Box(tooltipRect, text, tooltipStyle);
            }

            tooltips.Clear();
        }
    }
}
