using System.Collections.Generic;
using UnifromCheat_REPO.Utils;
using UnityEngine;
using static UnifromCheat_REPO.GUIMenuSkin;
using static UnifromCheat_REPO.TooltipsLanguages;

namespace UnifromCheat_REPO;

public partial class Core 
{
    private const int ObjectSpawnerWindowId = 3002;
    private const int ObjectSpawnerColumns = 3;
    private const float ObjectSpawnerButtonHeight = 48f;
    private const float ObjectSpawnerButtonGap = 8f;
    private const float ObjectSpawnerVisibleHeight = 500f;
    private readonly Dictionary<string, float> objectSpawnerButtonAnimations = new();
    private readonly Dictionary<string, string> objectSpawnerFormattedLabels = new();

    private enum ObjectSpawnerTab
    {
        Items,
        Valuables,
        Entity
    }

    private ObjectSpawnerTab objectSpawnerTab = ObjectSpawnerTab.Items;

    private void DrawObjectSpawnerWindow()
    {
        float target = objectSpawnerWindowOpen ? 1f : 0f;
        if (Event.current.type == EventType.Layout)
            objectSpawnerAnimationProgress = Mathf.MoveTowards(objectSpawnerAnimationProgress, target, Time.unscaledDeltaTime * 7.5f);

        float combinedProgress = objectSpawnerAnimationProgress * menuAnimationProgress;
        if (!objectSpawnerWindowOpen && objectSpawnerAnimationProgress <= 0.001f)
            return;

        if (combinedProgress <= 0.001f)
            return;

        float eased = combinedProgress * combinedProgress * (3f - 2f * combinedProgress);
        Rect drawRect = objectSpawnerRect;
        drawRect.y -= 22f * (1f - eased);

        Color previousColor = GUI.color;
        Matrix4x4 previousMatrix = GUI.matrix;
        int previousDepth = GUI.depth;
        GUI.depth = -5000;
        GUI.color = new Color(previousColor.r, previousColor.g, previousColor.b, previousColor.a * eased);

        objectSpawnerRect = GUI.Window(ObjectSpawnerWindowId, drawRect, DrawObjectSpawnerContents, string.Empty, windowStyle);
        GUI.BringWindowToFront(ObjectSpawnerWindowId);
        objectSpawnerRect.y += 22f * (1f - eased);
        DrawAnimatedSectionFrame(drawRect, Time.unscaledTime, eased, ObjectSpawnerWindowId, 0.28f);

        GUI.depth = previousDepth;
        GUI.matrix = previousMatrix;
        GUI.color = previousColor;
    }

    private void DrawObjectSpawnerContents(int id)
    {
        GUIStyle headerStyle = new GUIStyle(labelStyle)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 15,
            richText = true
        };

        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Space(34);
        GUILayout.Label("<b>OBJECT SPAWNER</b>", headerStyle, GUILayout.Height(28));
        if (GUILayout.Button("<b>X</b>", buttonStyle, GUILayout.Width(34), GUILayout.Height(28)))
            objectSpawnerWindowOpen = false;
        GUILayout.EndHorizontal();

        GUILayout.Space(4);
        GUILayout.BeginHorizontal();
        DrawObjectSpawnerTabButton("Items", ObjectSpawnerTab.Items, Get("objectSpawnerItemsTab"));
        DrawObjectSpawnerTabButton("Valuables", ObjectSpawnerTab.Valuables, Get("objectSpawnerValuablesTab"));
        DrawObjectSpawnerTabButton("Entity", ObjectSpawnerTab.Entity, Get("objectSpawnerEntityTab"));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(6);
        objectSpawnerScroll = GUILayout.BeginScrollView(objectSpawnerScroll, windowStyle);

        switch (objectSpawnerTab)
        {
            case ObjectSpawnerTab.Items:
                DrawItemSpawnerGrid();
                break;
            case ObjectSpawnerTab.Valuables:
                DrawEntrySpawnerGrid(ObjectSpawner.GetSpawnableValuables(), "No valuables found.");
                break;
            case ObjectSpawnerTab.Entity:
                DrawEntrySpawnerGrid(ObjectSpawner.GetSpawnableEntities(), "No entities found.");
                break;
        }

        GUILayout.EndScrollView();
        GUITooltip.Draw();
        GUILayout.EndVertical();

        GUI.DragWindow(new Rect(0, 0, objectSpawnerRect.width - 44f, 34f));
    }

    private void DrawObjectSpawnerTabButton(string label, ObjectSpawnerTab tab, string tooltip)
    {
        string text = objectSpawnerTab == tab ? $"<b>[ {label} ]</b>" : $"<b>{label}</b>";
        if (GUILayout.Button(text, buttonStyle, GUILayout.Width(120), GUILayout.Height(30)))
        {
            if (objectSpawnerTab != tab)
                objectSpawnerScroll = Vector2.zero;

            objectSpawnerTab = tab;
        }

        if (!string.IsNullOrWhiteSpace(tooltip) && !HideAllTooltips)
            GUITooltip.Show(tooltip, GUILayoutUtility.GetLastRect());
    }

    private void DrawItemSpawnerGrid()
    {
        List<Item> items = ObjectSpawner.GetSpawnableItems();
        if (items.Count == 0)
        {
            DrawLabel("No spawnable items found.", Color.gray);
            CleanupObjectSpawnerAnimations("items:", null);
            return;
        }

        DrawVirtualizedGrid(
            items.Count,
            "items:",
            index => ObjectSpawner.GetDisplayName(items[index]),
            index => ObjectSpawner.SpawnItem(items[index], out string resultMessage)
                ? (true, resultMessage)
                : (false, resultMessage)
        );
    }

    private void DrawEntrySpawnerGrid(List<ObjectSpawner.SpawnableEntry> entries, string emptyMessage)
    {
        if (entries.Count == 0)
        {
            DrawLabel(emptyMessage, Color.gray);
            CleanupObjectSpawnerAnimations("entries:" + objectSpawnerTab + ":", null);
            return;
        }

        string prefix = "entries:" + objectSpawnerTab + ":";
        DrawVirtualizedGrid(
            entries.Count,
            prefix,
            index => entries[index].DisplayName,
            index => ObjectSpawner.SpawnEntry(entries[index], out string resultMessage)
                ? (true, resultMessage)
                : (false, resultMessage)
        );
    }

    private void DrawVirtualizedGrid(int count, string animationPrefix, System.Func<int, string> getLabel, System.Func<int, (bool spawned, string message)> spawn)
    {
        GUIStyle spawnerButtonStyle = new GUIStyle(buttonStyle)
        {
            wordWrap = true,
            alignment = TextAnchor.MiddleCenter,
            clipping = TextClipping.Clip,
            padding = new RectOffset(8, 8, 6, 6)
        };

        int rows = Mathf.CeilToInt(count / (float)ObjectSpawnerColumns);
        float rowHeight = ObjectSpawnerButtonHeight + ObjectSpawnerButtonGap;
        float totalHeight = Mathf.Max(rows * rowHeight, ObjectSpawnerButtonHeight);
        float availableWidth = Mathf.Max(120f, objectSpawnerRect.width - 42f);
        float buttonWidth = (availableWidth - (ObjectSpawnerColumns - 1) * ObjectSpawnerButtonGap) / ObjectSpawnerColumns;
        Rect contentRect = GUILayoutUtility.GetRect(availableWidth, totalHeight, GUILayout.ExpandWidth(true));

        float scrollY = objectSpawnerScroll.y;
        float viewportHeight = Mathf.Min(ObjectSpawnerVisibleHeight, objectSpawnerRect.height - 140f);
        int firstRow = Mathf.Max(0, Mathf.FloorToInt(scrollY / rowHeight) - 2);
        int lastRow = Mathf.Min(rows - 1, Mathf.CeilToInt((scrollY + viewportHeight) / rowHeight) + 2);
        HashSet<string> visibleKeys = new HashSet<string>();

        for (int row = firstRow; row <= lastRow; row++)
        {
            for (int col = 0; col < ObjectSpawnerColumns; col++)
            {
                int index = row * ObjectSpawnerColumns + col;
                if (index >= count)
                    continue;

                string key = animationPrefix + index;
                visibleKeys.Add(key);
                float progress = GetObjectSpawnerButtonProgress(key);
                float eased = progress * progress * (3f - 2f * progress);

                Rect buttonRect = new Rect(
                    contentRect.x + col * (buttonWidth + ObjectSpawnerButtonGap),
                    contentRect.y + row * rowHeight + 8f * (1f - eased),
                    buttonWidth,
                    ObjectSpawnerButtonHeight
                );

                Color previousColor = GUI.color;
                GUI.color = new Color(previousColor.r, previousColor.g, previousColor.b, previousColor.a * eased);
                string label = FormatSpawnerButtonLabel(getLabel(index), spawnerButtonStyle, buttonWidth - 16f);
                if (GUI.Button(buttonRect, label, spawnerButtonStyle))
                {
                    var result = spawn(index);
                    if (!string.IsNullOrWhiteSpace(result.message))
                        ShowMessage(result.message, result.spawned ? 2.5f : 3.5f);
                }
                GUI.color = previousColor;
            }
        }

        CleanupObjectSpawnerAnimations(animationPrefix, visibleKeys);
    }

    private float GetObjectSpawnerButtonProgress(string key)
    {
        if (!objectSpawnerButtonAnimations.TryGetValue(key, out float progress))
            progress = 0f;

        progress = Mathf.MoveTowards(progress, 1f, Time.unscaledDeltaTime * 8f);
        objectSpawnerButtonAnimations[key] = progress;
        return progress;
    }

    private void CleanupObjectSpawnerAnimations(string prefix, HashSet<string> visibleKeys)
    {
        List<string> removeKeys = null;
        foreach (var key in objectSpawnerButtonAnimations.Keys)
        {
            if (!key.StartsWith(prefix))
                continue;

            if (visibleKeys != null && visibleKeys.Contains(key))
                continue;

            removeKeys ??= new List<string>();
            removeKeys.Add(key);
        }

        if (removeKeys == null)
            return;

        foreach (string key in removeKeys)
            objectSpawnerButtonAnimations.Remove(key);
    }

    private string FormatSpawnerButtonLabel(string label, GUIStyle style, float maxWidth)
    {
        if (string.IsNullOrWhiteSpace(label))
            return string.Empty;

        string cacheKey = Mathf.RoundToInt(maxWidth) + "|" + label;
        if (objectSpawnerFormattedLabels.TryGetValue(cacheKey, out string cachedLabel))
            return cachedLabel;

        if (objectSpawnerFormattedLabels.Count > 1200)
            objectSpawnerFormattedLabels.Clear();

        if (style.CalcSize(new GUIContent(label)).x <= maxWidth || !label.Contains(" "))
        {
            objectSpawnerFormattedLabels[cacheKey] = label;
            return label;
        }

        string[] words = label.Split(' ');
        string firstLine = words[0];
        string secondLine = string.Empty;

        for (int i = 1; i < words.Length; i++)
        {
            string candidate = string.IsNullOrEmpty(secondLine)
                ? words[i]
                : secondLine + " " + words[i];

            string firstCandidate = firstLine + " " + words[i];
            if (style.CalcSize(new GUIContent(firstCandidate)).x <= maxWidth && string.IsNullOrEmpty(secondLine))
            {
                firstLine = firstCandidate;
                continue;
            }

            secondLine = candidate;
        }

        if (string.IsNullOrEmpty(secondLine))
        {
            objectSpawnerFormattedLabels[cacheKey] = label;
            return label;
        }

        string formattedLabel = firstLine + "\n" + secondLine;
        objectSpawnerFormattedLabels[cacheKey] = formattedLabel;
        return formattedLabel;
    }
}
