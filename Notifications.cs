using System.Collections.Generic;
using UnityEngine;
using static UnifromCheat_REPO.GUIMenuSkin;

namespace UnifromCheat_REPO;

public partial class Core
{
    private const float MessageEnterDuration = 0.85f;
    private const float MessageExitDuration = 0.65f;
    private const int MessageWindowIdBase = 9000;
    private static readonly List<UnifromMessage> activeMessages = new();
    private static int nextMessageId;

    private class UnifromMessage
    {
        public int Id;
        public string Text;
        public float CreatedAt;
        public float ShowDuration;
        public float CloseStartedAt;
        public bool Closing;
    }

    public static void ShowMessage(string message, float showDuration)
    {
        if (string.IsNullOrWhiteSpace(message))
            return;

        activeMessages.Add(new UnifromMessage
        {
            Id = nextMessageId++,
            Text = message,
            CreatedAt = Time.unscaledTime,
            ShowDuration = Mathf.Max(0.1f, showDuration),
            CloseStartedAt = -1f,
            Closing = false
        });
    }

    private void DrawMessages()
    {
        if (activeMessages.Count == 0)
            return;

        GUIStyle titleStyle = new GUIStyle(labelStyle)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 14,
            richText = true
        };

        GUIStyle bodyStyle = new GUIStyle(labelStyle)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 13,
            richText = true,
            wordWrap = true
        };

        float now = Time.unscaledTime;
        float screenWidth = Screen.width / dpiScaling;
        float screenHeight = Screen.height / dpiScaling;
        float stackOffset = 0f;
        int previousDepth = GUI.depth;
        Color previousGuiColor = GUI.color;
        Color previousBackgroundColor = GUI.backgroundColor;
        Color previousContentColor = GUI.contentColor;
        GUI.depth = -20000;
        GUI.backgroundColor = new Color(HC_R, HC_G, HC_B, HC_A);
        GUI.contentColor = Color.white;

        for (int i = activeMessages.Count - 1; i >= 0; i--)
        {
            UnifromMessage msg = activeMessages[i];
            if (!msg.Closing && now - msg.CreatedAt >= msg.ShowDuration)
            {
                msg.Closing = true;
                msg.CloseStartedAt = now;
            }

            float progress = msg.Closing
                ? 1f - Mathf.Clamp01((now - msg.CloseStartedAt) / MessageExitDuration)
                : Mathf.Clamp01((now - msg.CreatedAt) / MessageEnterDuration);

            float eased = progress * progress * (3f - 2f * progress);
            if (msg.Closing && progress <= 0f)
            {
                activeMessages.RemoveAt(i);
                continue;
            }

            const float paddingX = 28f;
            const float paddingY = 20f;
            float maxWidth = Mathf.Min(560f, screenWidth - 80f);
            float desiredWidth = Mathf.Max(
                titleStyle.CalcSize(new GUIContent("Unifrom Message")).x,
                bodyStyle.CalcSize(new GUIContent(msg.Text)).x
            ) + paddingX * 2f;
            float width = Mathf.Clamp(desiredWidth, 260f, maxWidth);
            float bodyHeight = bodyStyle.CalcHeight(new GUIContent(msg.Text), width - paddingX * 2f);
            float height = Mathf.Clamp(46f + bodyHeight + paddingY, 72f, screenHeight - 80f);

            float targetY = screenHeight - 28f - stackOffset - height;
            float hiddenY = screenHeight + 32f;
            float y = Mathf.Lerp(hiddenY, targetY, eased);
            Rect rect = new Rect((screenWidth - width) * 0.5f, y, width, height);
            int windowId = MessageWindowIdBase + msg.Id;

            GUI.color = new Color(HC_R, HC_G, HC_B, HC_A * eased);
            GUI.Window(windowId, rect, _ =>
            {
                GUILayout.Label("<b>Unifrom Message</b>", titleStyle);
                GUILayout.Space(4f);
                GUILayout.Label(msg.Text, bodyStyle);
            }, string.Empty, windowStyle);
            GUI.BringWindowToFront(windowId);
            DrawAnimatedSectionFrame(rect, now, eased, 100 + msg.Id);

            stackOffset += height + 10f;
        }

        GUI.color = previousGuiColor;
        GUI.backgroundColor = previousBackgroundColor;
        GUI.contentColor = previousContentColor;
        GUI.depth = previousDepth;
    }
}
