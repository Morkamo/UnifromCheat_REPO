using System;
using System.Collections.Generic;
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
        public static Texture2D sliderFillTex;
        public static Texture2D lineTex;

        // Utility: single white pixel to tint without allocations
        private static Texture2D whiteTex;
        private static readonly Dictionary<string, float> stateAnimations = new Dictionary<string, float>();

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

            windowTex          = MakeTex(8, 8, new Color(0.105f, 0.112f, 0.122f, 0.86f), new Color(0.30f, 0.34f, 0.38f, 0.95f));
            buttonTex          = MakeTex(8, 8, new Color(0.145f, 0.155f, 0.168f, 0.96f), new Color(0.34f, 0.38f, 0.42f, 0.9f));
            buttonHoverTex     = MakeTex(8, 8, new Color(0.205f, 0.222f, 0.242f, 0.98f), new Color(0.55f, 0.70f, 0.78f, 1f));
            buttonActiveTex    = MakeTex(8, 8, new Color(0.075f, 0.095f, 0.105f, 1f), new Color(0.56f, 0.95f, 0.70f, 1f));
            sliderBackgroundTex= MakeTex(8, 8, new Color(0.09f, 0.10f, 0.11f, 1f), new Color(0.28f, 0.31f, 0.34f, 1f));
            sliderThumbTex     = MakeTex(8, 8, new Color(0.76f, 0.88f, 0.95f, 1f), new Color(0.18f, 0.26f, 0.30f, 1f));
            sliderFillTex      = MakeTex(4, 4, new Color(0.34f, 0.78f, 0.84f, 1f), new Color(0.34f, 0.78f, 0.84f, 1f));
            lineTex            = MakeTex(2, 2, new Color(0.52f, 0.68f, 0.74f, 1f), new Color(0.52f, 0.68f, 0.74f, 1f));
            whiteTex           = MakeTex(2, 2, Color.white, Color.white);

            menuSkin = ScriptableObject.CreateInstance<GUISkin>();

            windowStyle = new GUIStyle(GUI.skin.window);
            ApplyStyle(windowStyle, windowTex, 3);
            windowStyle.normal.textColor = Color.white;
            windowStyle.fontSize = 13;

            buttonStyle = new GUIStyle(GUI.skin.button);
            ApplyButtonStyle(buttonStyle, buttonTex, buttonHoverTex, buttonActiveTex, 3);
            buttonStyle.fontSize = 14;
            buttonStyle.richText = true;

            labelStyle = new GUIStyle(GUI.skin.label) { normal = { textColor = new Color(0.93f, 0.96f, 0.98f, 1f) } };
            labelStyle.richText = true;

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
            textFieldStyle.padding  = new RectOffset(6, 6, 4, 4);
            textFieldStyle.normal.textColor = Color.white;
            textFieldStyle.focused.textColor = Color.white;
            textFieldStyle.normal.background = MakeTex(8, 8, new Color(0.12f, 0.13f, 0.145f, 1f), new Color(0.32f, 0.36f, 0.4f, 1f));
            textFieldStyle.focused.background = MakeTex(8, 8, new Color(0.15f, 0.17f, 0.185f, 1f), new Color(0.52f, 0.72f, 0.8f, 1f));

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
            style.margin  = new RectOffset(4, 4, 5, 5);
            style.padding = new RectOffset(8, 8, 8, 8);
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
            style.margin  = new RectOffset(4, 4, 5, 5);
            style.padding = new RectOffset(8, 8, 7, 7);
            style.normal.textColor = Color.white;
            style.hover.textColor  = new Color(0.85f, 1f, 1f, 1f);
            style.active.textColor = new Color(0.65f, 1f, 0.72f, 1f);
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

            Rect sliderRect = GUILayoutUtility.GetRect(120, 16, GUILayout.ExpandWidth(false));
            DrawSliderTrack(sliderRect, value, min, max);
            value = GUI.HorizontalSlider(sliderRect, value, min, max, sliderStyle, sliderThumbStyle);
            if (sliderRect.Contains(Event.current.mousePosition))
                GUI.DrawTexture(new Rect(sliderRect.x, sliderRect.yMax + 1f, sliderRect.width, 1f), lineTex);
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

            Rect sliderRect = GUILayoutUtility.GetRect(120, 16, GUILayout.ExpandWidth(false));
            DrawSliderTrack(sliderRect, value, min, max);
            value = (int)GUI.HorizontalSlider(sliderRect, value, min, max, sliderStyle, sliderThumbStyle);
            if (sliderRect.Contains(Event.current.mousePosition))
                GUI.DrawTexture(new Rect(sliderRect.x, sliderRect.yMax + 1f, sliderRect.width, 1f), lineTex);
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

        public static void DrawToggle(string label, ref bool value, Color? onColor = null, string tooltip = null, string animationKey = null)
        {
            GUILayout.BeginHorizontal();

            Color activeColor   = onColor ?? Color.green;
            Color inactiveColor = Color.gray;
            int iconSize = 18;

            Rect rect = GUILayoutUtility.GetRect(iconSize, iconSize, GUILayout.ExpandWidth(false));
            bool hover = rect.Contains(Event.current.mousePosition);
            string stateKey = GetControlAnimationKey("toggle", label, animationKey);
            float stateProgress = GetStateAnimation(stateKey, value);
            Color drawColor = Color.Lerp(inactiveColor, activeColor, stateProgress);
            if (hover) drawColor = Tint(drawColor, 1.2f);

            Rect iconRect = rect;
            iconRect.y += 5;

            var prevCol = GUI.color;
            float inheritedAlpha = prevCol.a;
            GUI.color = new Color(0f, 0f, 0f, 0.35f * inheritedAlpha);
            GUI.DrawTexture(new Rect(iconRect.x + 1f, iconRect.y + 1f, iconRect.width, iconRect.height), whiteTex);
            GUI.color = new Color(drawColor.r, drawColor.g, drawColor.b, drawColor.a * inheritedAlpha);
            GUI.DrawTexture(iconRect, whiteTex);
            if (stateProgress > 0.001f)
            {
                GUI.color = new Color(1f, 1f, 1f, 0.65f * stateProgress * inheritedAlpha);
                GUI.DrawTexture(new Rect(iconRect.x + 4f, iconRect.y + 4f, iconRect.width - 8f, iconRect.height - 8f), whiteTex);
            }
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

        public static void DrawRadio(string label, int optionIndex, ref int selectedIndex, Color? onColor = null, string tooltip = null, string animationKey = null)
        {
            GUILayout.BeginHorizontal();

            Color activeColor   = onColor ?? Color.green;
            Color inactiveColor = Color.gray;
            int iconSize = 18;

            Rect rect = GUILayoutUtility.GetRect(iconSize, iconSize, GUILayout.ExpandWidth(false));
            bool hover = rect.Contains(Event.current.mousePosition);
            bool isActive = selectedIndex == optionIndex;
            string stateKey = GetControlAnimationKey("radio", label + ":" + optionIndex, animationKey);
            float stateProgress = GetStateAnimation(stateKey, isActive);
            Color drawColor = Color.Lerp(inactiveColor, activeColor, stateProgress);
            if (hover) drawColor = Tint(drawColor, 1.2f);

            Rect iconRect = rect;
            iconRect.y += 5;

            var prevCol = GUI.color;
            float inheritedAlpha = prevCol.a;
            GUI.color = new Color(0f, 0f, 0f, 0.35f * inheritedAlpha);
            GUI.DrawTexture(new Rect(iconRect.x + 1f, iconRect.y + 1f, iconRect.width, iconRect.height), whiteTex);
            GUI.color = new Color(drawColor.r, drawColor.g, drawColor.b, drawColor.a * inheritedAlpha);
            GUI.DrawTexture(iconRect, whiteTex);
            if (stateProgress > 0.001f)
            {
                GUI.color = new Color(1f, 1f, 1f, 0.65f * stateProgress * inheritedAlpha);
                GUI.DrawTexture(new Rect(iconRect.x + 4f, iconRect.y + 4f, iconRect.width - 8f, iconRect.height - 8f), whiteTex);
            }
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

        private static float GetStateAnimation(string key, bool active)
        {
            if (!stateAnimations.TryGetValue(key, out float progress))
                progress = active ? 1f : 0f;

            float target = active ? 1f : 0f;
            progress = Mathf.MoveTowards(progress, target, Time.unscaledDeltaTime * 4.5f);
            stateAnimations[key] = progress;
            return progress * progress * (3f - 2f * progress);
        }

        private static string GetControlAnimationKey(string prefix, string label, string explicitKey)
        {
            if (!string.IsNullOrEmpty(explicitKey))
                return prefix + ":" + explicitKey;

            return prefix + ":auto:" + GUIUtility.GetControlID(FocusType.Passive) + ":" + label;
        }

        private static void DrawSliderTrack(Rect rect, float value, float min, float max)
        {
            if (sliderFillTex == null) return;

            float t = Mathf.InverseLerp(min, max, value);
            Rect fillRect = new Rect(rect.x + 2f, rect.y + 4f, Mathf.Max(0f, (rect.width - 4f) * t), 6f);
            Color previousColor = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, 0.85f);
            GUI.DrawTexture(fillRect, sliderFillTex);
            GUI.color = previousColor;
        }

        public static void DrawAnimatedSectionFrame(Rect rect, float time, float alpha, int seed)
        {
            if (whiteTex == null || Event.current.type != EventType.Repaint || alpha <= 0f)
                return;

            Rect frame = new Rect(rect.x + 1f, rect.y + 1f, rect.width - 2f, rect.height - 2f);
            if (frame.width <= 16f || frame.height <= 16f)
                return;

            Color previousColor = GUI.color;
            float phase = seed * 1.37f;
            DrawAnimatedLine(new Rect(frame.x, frame.y, frame.width, 2f), time, alpha, phase, true);
            DrawAnimatedLine(new Rect(frame.x, frame.yMax - 2f, frame.width, 2f), time, alpha, phase + 1.7f, true);
            DrawAnimatedLine(new Rect(frame.x, frame.y, 2f, frame.height), time, alpha, phase + 3.1f, false);
            DrawAnimatedLine(new Rect(frame.xMax - 2f, frame.y, 2f, frame.height), time, alpha, phase + 4.4f, false);

            DrawFrameSweep(frame, time, alpha, seed);
            GUI.color = previousColor;
        }

        private static void DrawAnimatedLine(Rect rect, float time, float alpha, float phase, bool horizontal)
        {
            int segments = horizontal ? 36 : 22;
            float length = horizontal ? rect.width : rect.height;
            float segmentLength = Mathf.Max(1f, length / segments);

            for (int i = 0; i < segments; i++)
            {
                float t = segments <= 1 ? 0f : i / (float)(segments - 1);
                float wave = 0.5f + 0.5f * Mathf.Sin(time * 1.9f + phase + t * Mathf.PI * 2f);
                float strength = Mathf.Lerp(0.18f, 0.52f, wave) * alpha;
                GUI.color = new Color(0.62f, 0.66f, 0.69f, strength);

                Rect segment = horizontal
                    ? new Rect(rect.x + i * segmentLength, rect.y, Mathf.Min(segmentLength + 1f, rect.xMax - (rect.x + i * segmentLength)), rect.height)
                    : new Rect(rect.x, rect.y + i * segmentLength, rect.width, Mathf.Min(segmentLength + 1f, rect.yMax - (rect.y + i * segmentLength)));

                if (segment.width > 0f && segment.height > 0f)
                    GUI.DrawTexture(segment, whiteTex);
            }
        }

        private static void DrawFrameSweep(Rect frame, float time, float alpha, int seed)
        {
            float sweepWidth = 150f;
            float offset = seed * 97f;
            float topX = frame.x + Mathf.Repeat(time * (205f + seed * 8f) + offset, frame.width + sweepWidth) - sweepWidth;
            float bottomX = frame.xMax - Mathf.Repeat(time * (175f + seed * 6f) + 80f + offset, frame.width + sweepWidth);
            float sideY = frame.y + Mathf.Repeat(time * (130f + seed * 5f) + 40f + offset, frame.height + sweepWidth) - sweepWidth;

            GUI.color = new Color(0.92f, 0.96f, 1f, 0.42f * alpha);
            DrawClippedTexture(new Rect(topX, frame.y, sweepWidth, 2f), frame);
            DrawClippedTexture(new Rect(bottomX, frame.yMax - 2f, sweepWidth, 2f), frame);

            GUI.color = new Color(0.82f, 0.88f, 0.92f, 0.26f * alpha);
            DrawClippedTexture(new Rect(frame.x, sideY, 2f, sweepWidth), frame);
            DrawClippedTexture(new Rect(frame.xMax - 2f, frame.yMax - sideY + frame.y - sweepWidth, 2f, sweepWidth), frame);
        }

        private static void DrawClippedTexture(Rect rect, Rect clip)
        {
            float xMin = Mathf.Max(rect.xMin, clip.xMin);
            float yMin = Mathf.Max(rect.yMin, clip.yMin);
            float xMax = Mathf.Min(rect.xMax, clip.xMax);
            float yMax = Mathf.Min(rect.yMax, clip.yMax);

            if (xMax <= xMin || yMax <= yMin)
                return;

            GUI.DrawTexture(new Rect(xMin, yMin, xMax - xMin, yMax - yMin), whiteTex);
        }

    }
}
