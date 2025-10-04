using System;
using UnityEngine;

namespace UnifromCheat_REPO
{
    public class GUIMenuSkin
    {
        public static GUISkin menuSkin;

        // Base textures
        public static Texture2D windowTex;
        public static Texture2D buttonTex;
        public static Texture2D buttonHoverTex;
        public static Texture2D buttonActiveTex;
        public static Texture2D sliderBackgroundTex;
        public static Texture2D sliderThumbTex;

        // Utility: single white pixel to tint without allocations
        private static Texture2D whiteTex;

        // Styles
        public static GUIStyle windowStyle;
        public static GUIStyle buttonStyle;
        public static GUIStyle sliderStyle;
        public static GUIStyle sliderThumbStyle;
        public static GUIStyle labelStyle;
        public static GUIStyle textFieldStyle;

        // Notify others (e.g., GUITooltip) that skin visuals changed (opacity, etc.)
        public static Action OnSkinChanged;

        private static bool _initialized;

        public static void InitSkin()
        {
            if (_initialized) return;

            windowTex          = MakeTex(4, 4, new Color(0.18f, 0.18f, 0.18f, 0.7f), Color.black);
            buttonTex          = MakeTex(4, 4, new Color(0.25f, 0.25f, 0.25f), Color.black);
            buttonHoverTex     = MakeTex(4, 4, new Color(0.35f, 0.35f, 0.35f), Color.black);
            buttonActiveTex    = MakeTex(4, 4, new Color(0.15f, 0.15f, 0.15f), Color.black);
            sliderBackgroundTex= MakeTex(4, 4, new Color(0.25f, 0.25f, 0.25f), Color.black);
            sliderThumbTex     = MakeTex(4, 4, new Color(0.45f, 0.45f, 0.45f), Color.black);
            whiteTex           = MakeTex(2, 2, Color.white, Color.white);

            menuSkin = ScriptableObject.CreateInstance<GUISkin>();

            windowStyle = new GUIStyle(GUI.skin.window);
            ApplyStyle(windowStyle, windowTex, 2);
            windowStyle.normal.textColor = Color.white;

            buttonStyle = new GUIStyle(GUI.skin.button);
            ApplyButtonStyle(buttonStyle, buttonTex, buttonHoverTex, buttonActiveTex, 2);
            buttonStyle.fontSize = 14;

            labelStyle = new GUIStyle(GUI.skin.label) { normal = { textColor = Color.white } };

            sliderStyle = new GUIStyle(GUI.skin.horizontalSlider)
            {
                normal      = { background = sliderBackgroundTex },
                active      = { background = sliderBackgroundTex },
                fixedHeight = 10
            };

            sliderThumbStyle = new GUIStyle(GUI.skin.horizontalSliderThumb)
            {
                normal   = { background = sliderThumbTex },
                hover    = { background = sliderThumbTex },
                active   = { background = sliderThumbTex },
                onNormal = { background = sliderThumbTex },
                onHover  = { background = sliderThumbTex },
                onActive = { background = sliderThumbTex },
                fixedWidth  = 12,
                fixedHeight = 14
            };

            textFieldStyle = new GUIStyle(GUI.skin.textField);
            textFieldStyle.font = GUI.skin.font;
            textFieldStyle.fontSize = 14;
            textFieldStyle.padding  = new RectOffset(4, 4, 4, 4);

            menuSkin.textField = textFieldStyle;

            GUITooltip.Init();
            _initialized = true;
        }

        public static void RefreshOpacity()
        {
            if (!_initialized) return;

            windowTex = MakeTex(4, 4, new Color(0.18f, 0.18f, 0.18f, 0.7f), Color.black);
            ApplyStyle(windowStyle, windowTex, 2);

            OnSkinChanged?.Invoke();
        }

        private static void ApplyStyle(GUIStyle style, Texture2D tex, int border)
        {
            style.normal.background    = tex;
            style.onNormal.background  = tex;
            style.active.background    = tex;
            style.onActive.background  = tex;
            style.focused.background   = tex;
            style.onFocused.background = tex;
            style.border  = new RectOffset(border, border, border, border);
            style.margin  = new RectOffset(4, 4, 4, 4);
            style.padding = new RectOffset(6, 6, 6, 6);
        }

        private static void ApplyButtonStyle(GUIStyle style, Texture2D normal, Texture2D hover, Texture2D active, int border)
        {
            style.normal.background    = normal;
            style.onNormal.background  = normal;
            style.hover.background     = hover;
            style.onHover.background   = hover;
            style.active.background    = active;
            style.onActive.background  = active;
            style.focused.background   = normal;
            style.onFocused.background = normal;
            style.border  = new RectOffset(border, border, border, border);
            style.margin  = new RectOffset(4, 4, 4, 4);
            style.padding = new RectOffset(6, 6, 6, 6);
            style.normal.textColor = Color.white;
            style.hover.textColor  = Color.yellow;
            style.active.textColor = Color.green;
        }

        private static Texture2D MakeTex(int w, int h, Color fill, Color border)
        {
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            tex.wrapMode   = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Point;
            tex.anisoLevel = 0;

            for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
                tex.SetPixel(x, y, (x == 0 || y == 0 || x == w - 1 || y == h - 1) ? border : fill);

            tex.Apply(false, false);
            return tex;
        }

        public static void DrawSlider(string label, ref float value, float min, float max, float? defaultValue = null, string tooltip = null)
        {
            GUILayout.BeginHorizontal();
            GUI.contentColor = Color.white;
            GUILayout.Label($"{label}: {value:F1}", labelStyle, GUILayout.Width(120));

            value = GUILayout.HorizontalSlider(value, min, max, sliderStyle, sliderThumbStyle, GUILayout.Width(120));
            Rect rect = GUILayoutUtility.GetLastRect();
            if (!string.IsNullOrEmpty(tooltip) && !Core.HideAllTooltips) GUITooltip.Show(tooltip, rect);

            if (defaultValue.HasValue)
            {
                if (GUILayout.Button("Default", buttonStyle, GUILayout.Width(70)))
                    value = defaultValue.Value;
                Rect bRect = GUILayoutUtility.GetLastRect();
                if (!string.IsNullOrEmpty(tooltip) && !Core.HideAllTooltips) GUITooltip.Show(tooltip, bRect);
            }

            GUILayout.EndHorizontal();
        }

        public static void DrawSliderInt(string label, ref int value, int min, int max, int? defaultValue = null, string tooltip = null)
        {
            GUILayout.BeginHorizontal();
            GUI.contentColor = Color.white;
            GUILayout.Label($"{label}: {value}", labelStyle, GUILayout.Width(100));

            value = (int)GUILayout.HorizontalSlider(value, min, max, sliderStyle, sliderThumbStyle, GUILayout.Width(120));
            Rect rect = GUILayoutUtility.GetLastRect();
            if (!string.IsNullOrEmpty(tooltip) && !Core.HideAllTooltips) GUITooltip.Show(tooltip, rect);

            if (defaultValue.HasValue)
            {
                if (GUILayout.Button("Default", buttonStyle, GUILayout.Width(70)))
                    value = defaultValue.Value;
                Rect bRect = GUILayoutUtility.GetLastRect();
                if (!string.IsNullOrEmpty(tooltip) && !Core.HideAllTooltips) GUITooltip.Show(tooltip, bRect);
            }

            GUILayout.EndHorizontal();
        }

        public static void DrawToggle(string label, ref bool value, Color? onColor = null, string tooltip = null)
        {
            GUILayout.BeginHorizontal();

            Color activeColor   = onColor ?? Color.green;
            Color inactiveColor = Color.gray;
            int iconSize = 18;

            Rect rect = GUILayoutUtility.GetRect(iconSize, iconSize, GUILayout.ExpandWidth(false));
            bool hover = rect.Contains(Event.current.mousePosition);
            Color drawColor = value ? activeColor : inactiveColor;
            if (hover) drawColor = Tint(drawColor, 1.2f);

            Rect iconRect = rect;
            iconRect.y += 5;

            var prevCol = GUI.color;
            GUI.color = drawColor;
            GUI.DrawTexture(iconRect, whiteTex);
            GUI.color = prevCol;

            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                value = !value;
                Event.current.Use();
            }

            GUIStyle tempLabel = new GUIStyle(labelStyle) { alignment = TextAnchor.MiddleLeft };
            Rect labelRect = GUILayoutUtility.GetRect(new GUIContent(label), tempLabel, GUILayout.ExpandWidth(true));

            if (Event.current.type == EventType.MouseDown && labelRect.Contains(Event.current.mousePosition))
            {
                value = !value;
                Event.current.Use();
            }

            GUI.Label(labelRect, label, tempLabel);

            if (!string.IsNullOrEmpty(tooltip) && !Core.HideAllTooltips)
            {
                GUITooltip.Show(tooltip, rect);
                GUITooltip.Show(tooltip, labelRect);
            }

            GUILayout.EndHorizontal();
        }

        public static void DrawRadio(string label, int optionIndex, ref int selectedIndex, Color? onColor = null, string tooltip = null)
        {
            GUILayout.BeginHorizontal();

            Color activeColor   = onColor ?? Color.green;
            Color inactiveColor = Color.gray;
            int iconSize = 18;

            Rect rect = GUILayoutUtility.GetRect(iconSize, iconSize, GUILayout.ExpandWidth(false));
            bool hover = rect.Contains(Event.current.mousePosition);
            bool isActive = selectedIndex == optionIndex;
            Color drawColor = isActive ? activeColor : inactiveColor;
            if (hover) drawColor = Tint(drawColor, 1.2f);

            Rect iconRect = rect;
            iconRect.y += 5;

            var prevCol = GUI.color;
            GUI.color = drawColor;
            GUI.DrawTexture(iconRect, whiteTex);
            GUI.color = prevCol;

            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                selectedIndex = optionIndex;
                Event.current.Use();
            }

            GUIStyle tempLabel = new GUIStyle(labelStyle) { alignment = TextAnchor.MiddleLeft };
            Rect labelRect = GUILayoutUtility.GetRect(new GUIContent(label), tempLabel, GUILayout.ExpandWidth(true));

            if (Event.current.type == EventType.MouseDown && labelRect.Contains(Event.current.mousePosition))
            {
                selectedIndex = optionIndex;
                Event.current.Use();
            }

            GUI.Label(labelRect, label, tempLabel);

            if (!string.IsNullOrEmpty(tooltip) && !Core.HideAllTooltips)
            {
                GUITooltip.Show(tooltip, rect);
                GUITooltip.Show(tooltip, labelRect);
            }

            GUILayout.EndHorizontal();
        }

        public static void DrawLabel(string text, Color color, string tooltip = null)
        {
            GUIStyle style = new GUIStyle(labelStyle)
            {
                normal = { textColor = color },
                alignment = TextAnchor.MiddleLeft
            };

            Rect rect = GUILayoutUtility.GetRect(new GUIContent(text), style, GUILayout.ExpandWidth(true));
            GUI.Label(rect, text, style);

            if (!string.IsNullOrEmpty(tooltip) && !Core.HideAllTooltips)
            {
                GUITooltip.Show(tooltip, rect);
            }
        }

        public static void DrawTextField(string label, ref string value, int labelWidth = 50 ,int fieldWidth = 120, string tooltip = null)
        {
            GUILayout.BeginHorizontal();

            GUI.contentColor = Color.white;
            GUILayout.Label(label, labelStyle, GUILayout.Width(labelWidth));

            Rect rect = GUILayoutUtility.GetRect(fieldWidth, 22, GUILayout.ExpandWidth(false));
            
            Color bg = new Color(0.2f, 0.2f, 0.2f, 1f);
            GUI.color = bg;
            GUI.DrawTexture(rect, whiteTex);
            
            GUI.color = Color.black;
            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, 1), whiteTex);
            GUI.DrawTexture(new Rect(rect.x, rect.yMax - 1, rect.width, 1), whiteTex);
            GUI.DrawTexture(new Rect(rect.x, rect.y, 1, rect.height), whiteTex);
            GUI.DrawTexture(new Rect(rect.xMax - 1, rect.y, 1, rect.height), whiteTex);
            
            GUI.color = Color.white;
            value = GUI.TextField(rect, value, textFieldStyle);

            if (!string.IsNullOrEmpty(tooltip) && !Core.HideAllTooltips)
                GUITooltip.Show(tooltip, rect);

            GUILayout.EndHorizontal();
        }

        private static Color Tint(Color c, float mul)
        {
            return new Color(
                Mathf.Clamp01(c.r * mul),
                Mathf.Clamp01(c.g * mul),
                Mathf.Clamp01(c.b * mul),
                c.a
            );
        }
    }
}
