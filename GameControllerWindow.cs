using System.Collections.Generic;
using UnifromCheat_REPO.Utils;
using UnityEngine;
using static UnifromCheat_REPO.GUIMenuSkin;
using static UnifromCheat_REPO.TooltipsLanguages;

namespace UnifromCheat_REPO;

public partial class Core
{
    private const int GameControllerWindowId = 3003;
    private const int GameControllerConfirmWindowId = 3004;
    private const float GameControllerButtonHeight = 34f;

    private enum GameControllerTab
    {
        Players,
        Maps,
        Gameplay
    }

    private GameControllerTab gameControllerTab = GameControllerTab.Players;
    private GameController.MapEntry pendingMapEntry;
    private Rect gameControllerConfirmRect;

    private void DrawGameControllerWindow()
    {
        float target = gameControllerWindowOpen ? 1f : 0f;
        if (Event.current.type == EventType.Layout)
            gameControllerAnimationProgress = Mathf.MoveTowards(gameControllerAnimationProgress, target, Time.unscaledDeltaTime * 7.5f);

        float combinedProgress = gameControllerAnimationProgress * menuAnimationProgress;
        if (!gameControllerWindowOpen && gameControllerAnimationProgress <= 0.001f)
            return;

        if (combinedProgress <= 0.001f)
            return;

        float eased = combinedProgress * combinedProgress * (3f - 2f * combinedProgress);
        Rect drawRect = gameControllerRect;
        drawRect.y -= 22f * (1f - eased);

        Color previousColor = GUI.color;
        Matrix4x4 previousMatrix = GUI.matrix;
        int previousDepth = GUI.depth;
        GUI.depth = -6000;
        GUI.color = new Color(previousColor.r, previousColor.g, previousColor.b, previousColor.a * eased);

        gameControllerRect = GUI.Window(GameControllerWindowId, drawRect, DrawGameControllerContents, string.Empty, windowStyle);
        GUI.BringWindowToFront(GameControllerWindowId);
        gameControllerRect.y += 22f * (1f - eased);
        DrawAnimatedSectionFrame(drawRect, Time.unscaledTime, eased, GameControllerWindowId, 0.28f);

        if (pendingMapEntry != null)
            DrawMapConfirmation(eased);

        GUI.depth = previousDepth;
        GUI.matrix = previousMatrix;
        GUI.color = previousColor;
    }

    private void DrawGameControllerContents(int id)
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
        GUILayout.Label("<b>GAME CONTROLLER</b>", headerStyle, GUILayout.Height(28));
        if (GUILayout.Button("<b>X</b>", buttonStyle, GUILayout.Width(34), GUILayout.Height(28)))
        {
            gameControllerWindowOpen = false;
            CloseGameControllerConfirmation();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(4);
        GUILayout.BeginHorizontal();
        DrawGameControllerTabButton("Players", GameControllerTab.Players, Get("gameControllerPlayersTab"));
        DrawGameControllerTabButton("Maps", GameControllerTab.Maps, Get("gameControllerMapsTab"));
        DrawGameControllerTabButton("Gameplay", GameControllerTab.Gameplay, Get("gameControllerGameplayTab"));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUITooltip.Draw();

        GUILayout.Space(6);
        gameControllerScroll = GUILayout.BeginScrollView(gameControllerScroll, windowStyle);

        switch (gameControllerTab)
        {
            case GameControllerTab.Players:
                DrawPlayersTab();
                break;
            case GameControllerTab.Maps:
                DrawMapsTab();
                break;
            case GameControllerTab.Gameplay:
                DrawGameplayTab();
                break;
        }

        GUITooltip.Draw();
        GUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUI.DragWindow(new Rect(0, 0, gameControllerRect.width - 44f, 34f));
    }

    private void DrawGameControllerTabButton(string label, GameControllerTab tab, string tooltip)
    {
        string text = gameControllerTab == tab ? $"<b>[ {label} ]</b>" : $"<b>{label}</b>";
        if (GUILayout.Button(text, buttonStyle, GUILayout.Width(120), GUILayout.Height(30)))
        {
            if (gameControllerTab != tab)
            {
                gameControllerScroll = Vector2.zero;
                CloseGameControllerConfirmation();
            }

            gameControllerTab = tab;
        }

        if (!string.IsNullOrWhiteSpace(tooltip) && !HideAllTooltips)
            GUITooltip.Show(tooltip, GUILayoutUtility.GetLastRect());
    }

    private void DrawPlayersTab()
    {
        List<GameController.PlayerEntry> players = GameController.GetPlayers();
        if (players.Count == 0)
        {
            GUILayout.Space(Mathf.Max(80f, gameControllerRect.height * 0.3f));
            GUIStyle emptyStyle = new GUIStyle(labelStyle)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 13,
                richText = true
            };
            Color previous = GUI.contentColor;
            GUI.contentColor = Color.gray;
            GUILayout.Label("<b>No players found.</b>", emptyStyle, GUILayout.ExpandWidth(true), GUILayout.Height(30));
            GUI.contentColor = previous;
            return;
        }

        GUIStyle playerNameStyle = new GUIStyle(labelStyle)
        {
            alignment = TextAnchor.MiddleLeft,
            fontSize = 13,
            richText = true,
            wordWrap = true
        };

        foreach (GameController.PlayerEntry player in players)
        {
            GUILayout.BeginVertical(windowStyle);
            GUILayout.Label($"<b>{player.Name}</b>", playerNameStyle, GUILayout.Height(24));
            string state = player.IsDead ? "DEAD" : $"{player.Health}/{player.MaxHealth} HP";
            DrawLabel(state, player.IsDead ? new Color(1f, 0.35f, 0.32f, 1f) : Color.white);

            GUILayout.BeginHorizontal();
            DrawPlayerActionButton("HEAL", player.Avatar, GameController.Heal);
            DrawPlayerActionButton("REVIVE", player.Avatar, GameController.Revive);
            DrawPlayerActionButton("GOTO", player.Avatar, GameController.GoTo);
            DrawPlayerActionButton("BRING", player.Avatar, GameController.Bring);
            DrawPlayerActionButton("KILL", player.Avatar, GameController.Kill);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space(6);
        }
    }

    private delegate bool PlayerAction(PlayerAvatar avatar, out string message);

    private void DrawPlayerActionButton(string label, PlayerAvatar avatar, PlayerAction action)
    {
        if (!GUILayout.Button($"<b>{label}</b>", buttonStyle, GUILayout.Height(GameControllerButtonHeight)))
            return;

        bool ok = action(avatar, out string message);
        if (!ok && !string.IsNullOrWhiteSpace(message))
            ShowMessage(message, 3f);
    }

    private void DrawMapsTab()
    {
        List<GameController.MapEntry> maps = GameController.GetMaps();
        if (maps.Count == 0)
        {
            DrawLabel("No maps found.", Color.gray);
            return;
        }

        GUIStyle mapButtonStyle = new GUIStyle(buttonStyle)
        {
            wordWrap = true,
            alignment = TextAnchor.MiddleCenter,
            clipping = TextClipping.Clip,
            padding = new RectOffset(8, 8, 5, 5)
        };

        int columns = 2;
        float gap = 8f;
        float availableWidth = Mathf.Max(240f, gameControllerRect.width - 44f);
        float buttonWidth = (availableWidth - gap) / columns;

        for (int i = 0; i < maps.Count; i += columns)
        {
            GUILayout.BeginHorizontal();
            for (int col = 0; col < columns; col++)
            {
                int index = i + col;
                if (index >= maps.Count)
                {
                    GUILayout.Space(buttonWidth + gap);
                    continue;
                }

                string label = FormatSpawnerButtonLabel(maps[index].DisplayName, mapButtonStyle, buttonWidth - 16f);
                if (GUILayout.Button(label, mapButtonStyle, GUILayout.Width(buttonWidth), GUILayout.Height(44f)))
                    OpenMapConfirmation(maps[index]);

                if (col < columns - 1)
                    GUILayout.Space(gap);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(6);
        }
    }

    private void DrawGameplayTab()
    {
        DrawGameplayToggle("Disable Spawn AI", ref gcDisableSpawnAI, Get("gameControllerDisableSpawnAIToggle"), "gc.disableSpawnAI");
        DrawGameplayToggle("Disable Auto Extract", ref gcDisableAutoExtract, Get("gameControllerDisableAutoExtractToggle"), "gc.disableAutoExtract");
        DrawGameplayToggle("Shared Upgrades", ref gcSharedUpgrades, Get("gameControllerSharedUpgradesToggle"), "gc.sharedUpgrades");
        DrawGameplayToggle("Auto Revive", ref gcAutoRevive, Get("gameControllerAutoReviveToggle"), "gc.autoRevive");
    }

    private void DrawGameplayToggle(string label, ref bool value, string tooltip, string animationKey)
    {
        DrawHostOnlyToggle(label, ref value, Color.green, tooltip, animationKey);
    }

    private void OpenMapConfirmation(GameController.MapEntry mapEntry)
    {
        pendingMapEntry = mapEntry;
        float width = 360f;
        float height = 150f;
        gameControllerConfirmRect = new Rect(
            gameControllerRect.x + (gameControllerRect.width - width) / 2f,
            gameControllerRect.y + 92f,
            width,
            height
        );
    }

    private void DrawMapConfirmation(float alpha)
    {
        if (pendingMapEntry == null)
            return;

        Event current = Event.current;
        if (current.type == EventType.MouseDown && !gameControllerConfirmRect.Contains(current.mousePosition))
        {
            CloseGameControllerConfirmation();
            current.Use();
            return;
        }

        Color previousColor = GUI.color;
        int previousDepth = GUI.depth;
        GUI.depth = -7000;
        GUI.color = new Color(previousColor.r, previousColor.g, previousColor.b, previousColor.a * alpha);
        gameControllerConfirmRect = GUI.Window(GameControllerConfirmWindowId, gameControllerConfirmRect, DrawMapConfirmationContents, string.Empty, windowStyle);
        GUI.BringWindowToFront(GameControllerConfirmWindowId);
        GUI.depth = previousDepth;
        GUI.color = previousColor;
    }

    private void DrawMapConfirmationContents(int id)
    {
        GUILayout.BeginVertical();
        DrawLabel("Change map?", Color.white);
        GUILayout.Label(pendingMapEntry?.DisplayName ?? string.Empty, labelStyle, GUILayout.Height(34));
        GUILayout.FlexibleSpace();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("<b>CONFIRM</b>", buttonStyle, GUILayout.Height(32)))
        {
            GameController.MapEntry map = pendingMapEntry;
            CloseGameControllerConfirmation();
            bool ok = GameController.ChangeMap(map?.Level, out string message);
            if (!string.IsNullOrWhiteSpace(message))
                ShowMessage(message, ok ? 2.5f : 3.5f);
        }

        if (GUILayout.Button("<b>CANCEL</b>", buttonStyle, GUILayout.Height(32)))
            CloseGameControllerConfirmation();
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    private void CloseGameControllerConfirmation()
    {
        pendingMapEntry = null;
    }
}
