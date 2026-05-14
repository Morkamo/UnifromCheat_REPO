using System.Collections.Generic;
using System;
using UnifromCheat_REPO.Utils;
using UnityEngine;
using static UnifromCheat_REPO.GUIMenuSkin;
using static UnifromCheat_REPO.TooltipsLanguages;

namespace UnifromCheat_REPO;

public partial class Core
{
    private const int GameControllerWindowId = 3003;
    private const int GameControllerConfirmWindowId = 3004;
    private const int GameControllerUpgradesWindowId = 3005;
    private const float GameControllerButtonHeight = 34f;

    private enum GameControllerTab
    {
        Players,
        Maps,
        Gameplay
    }

    private GameControllerTab gameControllerTab = GameControllerTab.Players;
    private GameController.MapEntry pendingMapEntry;
    private GameControllerPendingAction pendingAction;
    private Rect gameControllerConfirmRect;
    private Rect gameControllerUpgradesRect = new Rect(760f, 120f, 520f, 560f);
    private Vector2 gameControllerUpgradesScroll;
    private PlayerAvatar gameControllerUpgradesAvatar;
    private string gameControllerUpgradesPlayerName;
    private string setLevelInput = "1";
    private string setMoneyInput = "0";
    private readonly Dictionary<string, string> upgradeInputs = new Dictionary<string, string>();

    private sealed class GameControllerPendingAction
    {
        public string Title;
        public string Body;
        public Func<string> Execute;
    }

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

        if (gameControllerUpgradesAvatar != null)
            DrawGameControllerUpgradesWindow(eased);

        if (pendingMapEntry != null || pendingAction != null)
            DrawGameControllerConfirmation(eased);

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
        GUILayout.Label(UiBold("GAME CONTROLLER"), headerStyle, GUILayout.Height(28));
        if (GUILayout.Button("<b>X</b>", buttonStyle, GUILayout.Width(34), GUILayout.Height(28)))
        {
            gameControllerWindowOpen = false;
            gameControllerUpgradesAvatar = null;
            CloseGameControllerConfirmation();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(4);
        GUILayout.BeginHorizontal();
        DrawGameControllerTabButton(Ui("Players"), GameControllerTab.Players, Get("gameControllerPlayersTab"));
        DrawGameControllerTabButton(Ui("Maps"), GameControllerTab.Maps, Get("gameControllerMapsTab"));
        DrawGameControllerTabButton(Ui("Gameplay"), GameControllerTab.Gameplay, Get("gameControllerGameplayTab"));
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
            GUILayout.Label(UiBold("No players found."), emptyStyle, GUILayout.ExpandWidth(true), GUILayout.Height(30));
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
            string state = player.IsDead ? Ui("DEAD") : $"{player.Health}/{player.MaxHealth} {Ui("HP")}";
            DrawLabel(state, player.IsDead ? new Color(1f, 0.35f, 0.32f, 1f) : Color.white);

            GUILayout.BeginHorizontal();
            DrawPlayerActionButton(Ui("HEAL"), player.Avatar, GameController.Heal);
            DrawPlayerActionButton(Ui("REVIVE"), player.Avatar, GameController.Revive);
            DrawPlayerActionButton(Ui("GOTO"), player.Avatar, GameController.GoTo);
            DrawPlayerActionButton(Ui("BRING"), player.Avatar, GameController.Bring);
            DrawPlayerActionButton(Ui("KILL"), player.Avatar, GameController.Kill);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            DrawPlayerActionButton(GameController.IsFrozen(player.Avatar) ? Ui("UNFREEZE") : Ui("FREEZE"), player.Avatar, GameController.ToggleFreeze);
            if (GUILayout.Button(UiBold("UPGRADES"), buttonStyle, GUILayout.Height(GameControllerButtonHeight)))
                OpenPlayerUpgrades(player);
            GUILayout.FlexibleSpace();
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

    private void OpenPlayerUpgrades(GameController.PlayerEntry player)
    {
        if (player?.Avatar == null)
        {
            ShowMessage(LocalizedMessages.Get("playerNotFound"), 3f);
            return;
        }

        if (!HostOnlyGuard.CanUseHostOnly(true, LocalizedMessages.Get("upgradesActionName")))
            return;

        gameControllerUpgradesAvatar = player.Avatar;
        gameControllerUpgradesPlayerName = player.Name;
        gameControllerUpgradesScroll = Vector2.zero;
        CloseGameControllerConfirmation();
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
        DrawSetLevelArea();
        DrawSetMoneyArea();
    }

    private void DrawGameplayToggle(string label, ref bool value, string tooltip, string animationKey)
    {
        DrawHostOnlyToggle(label, ref value, Color.green, tooltip, animationKey);
    }

    private void OpenMapConfirmation(GameController.MapEntry mapEntry)
    {
        pendingMapEntry = mapEntry;
        pendingAction = null;
        OpenGameControllerConfirmationRect();
    }

    private void DrawSetLevelArea()
    {
        GUILayout.Space(6);
        GUILayout.BeginVertical(windowStyle);
        GUILayout.BeginHorizontal();
        GUILayout.Label($"<b>{Ui("Set level")} ({Ui("current")}: {GameController.GetCurrentLevelNumber()})</b>", labelStyle, GUILayout.Width(210));
        setLevelInput = GUILayout.TextField(setLevelInput, textFieldStyle, GUILayout.Width(150), GUILayout.Height(24));
        if (GUILayout.Button(UiBold("SET"), buttonStyle, GUILayout.Width(90), GUILayout.Height(28)))
            TryOpenSetLevelConfirmation();
        GUILayout.EndHorizontal();
        Rect rect = GUILayoutUtility.GetLastRect();
        if (!string.IsNullOrWhiteSpace(Get("gameControllerSetLevel")) && !HideAllTooltips)
            GUITooltip.Show(Get("gameControllerSetLevel"), rect);
        GUILayout.EndVertical();
    }

    private void DrawSetMoneyArea()
    {
        GUILayout.Space(6);
        GUILayout.BeginVertical(windowStyle);
        GUILayout.BeginHorizontal();
        GUILayout.Label($"<b>{Ui("Set Money")} ({Ui("current")}: {GameController.GetCurrentMoney()})</b>", labelStyle, GUILayout.Width(210));
        setMoneyInput = GUILayout.TextField(setMoneyInput, textFieldStyle, GUILayout.Width(150), GUILayout.Height(24));
        if (GUILayout.Button(UiBold("SET"), buttonStyle, GUILayout.Width(70), GUILayout.Height(28)))
            TryOpenSetMoneyConfirmation(false);
        if (GUILayout.Button(UiBold("ADD"), buttonStyle, GUILayout.Width(70), GUILayout.Height(28)))
            TryOpenSetMoneyConfirmation(true);
        GUILayout.EndHorizontal();
        Rect rect = GUILayoutUtility.GetLastRect();
        if (!string.IsNullOrWhiteSpace(Get("gameControllerSetMoney")) && !HideAllTooltips)
            GUITooltip.Show(Get("gameControllerSetMoney"), rect);
        GUILayout.EndVertical();
    }

    private void TryOpenSetLevelConfirmation()
    {
        if (!ValidateGameControllerNumber(setLevelInput, 1L, GameController.MaxLevelNumber, "levelValue", out long level))
            return;

        if (!CanOpenHostOnlyGameControllerAction(LocalizedMessages.Get("setLevelActionName")))
            return;

        OpenActionConfirmation(LocalizedMessages.Get("setLevelConfirmTitle"), LocalizedMessages.Format("setLevelConfirmBody", level), () =>
        {
            bool ok = GameController.SetLevel((int)level, out string message);
            return string.IsNullOrWhiteSpace(message) ? (ok ? LocalizedMessages.Format("levelSet", level) : LocalizedMessages.Get("levelSetFailed")) : message;
        });
    }

    private void TryOpenSetMoneyConfirmation(bool add)
    {
        if (!ValidateGameControllerNumber(setMoneyInput, 0L, GameController.MaxMoney, "moneyValue", out long value))
            return;

        if (!CanOpenHostOnlyGameControllerAction(LocalizedMessages.Get(add ? "addMoneyActionName" : "setMoneyActionName")))
            return;

        long current = GameController.GetCurrentMoney();
        long finalValue = add ? current + value : value;
        if (add && finalValue > GameController.MaxMoney)
        {
            ShowMessage(LocalizedMessages.Format("moneyTotalOutOfRange", GameController.MaxMoney), 3.5f);
            return;
        }

        string title = LocalizedMessages.Get(add ? "addMoneyConfirmTitle" : "setMoneyConfirmTitle");
        string body = add
            ? LocalizedMessages.Format("addMoneyConfirmBody", value, finalValue)
            : LocalizedMessages.Format("setMoneyConfirmBody", finalValue);
        OpenActionConfirmation(title, body, () =>
        {
            string resultMessage;
            bool ok = add
                ? GameController.AddMoney((int)value, out resultMessage)
                : GameController.SetMoney((int)value, out resultMessage);
            return string.IsNullOrWhiteSpace(resultMessage) ? (ok ? LocalizedMessages.Format("moneyUpdated", finalValue) : LocalizedMessages.Get("moneyUpdateFailed")) : resultMessage;
        });
    }

    private bool ValidateGameControllerNumber(string input, long min, long max, string valueNameKey, out long value)
    {
        value = 0;
        string valueName = LocalizedMessages.Get(valueNameKey);
        if (string.IsNullOrWhiteSpace(input))
        {
            ShowMessage(LocalizedMessages.Format("enterNumber", valueName), 3f);
            return false;
        }

        if (!long.TryParse(input.Trim(), out value))
        {
            ShowMessage(LocalizedMessages.Format("invalidNumber", valueName), 3f);
            return false;
        }

        if (value < min || value > max)
        {
            ShowMessage(LocalizedMessages.Format("numberOutOfRange", valueName, min, max), 3.5f);
            return false;
        }

        return true;
    }

    private bool CanOpenHostOnlyGameControllerAction(string functionName)
    {
        if (!HostOnlyGuard.IsPlayableSession())
        {
            ShowMessage(LocalizedMessages.Get("playableOnly"), 3f);
            return false;
        }

        return HostOnlyGuard.CanUseHostOnly(true, functionName);
    }

    private void OpenActionConfirmation(string title, string body, Func<string> execute)
    {
        pendingMapEntry = null;
        pendingAction = new GameControllerPendingAction
        {
            Title = title,
            Body = body,
            Execute = execute
        };
        OpenGameControllerConfirmationRect();
    }

    private void OpenGameControllerConfirmationRect()
    {
        float width = 380f;
        float height = 158f;
        Rect sourceRect = gameControllerUpgradesAvatar != null ? gameControllerUpgradesRect : gameControllerRect;
        gameControllerConfirmRect = new Rect(
            sourceRect.x + (sourceRect.width - width) / 2f,
            sourceRect.y + 92f,
            width,
            height
        );
    }

    private void DrawGameControllerUpgradesWindow(float alpha)
    {
        Color previousColor = GUI.color;
        int previousDepth = GUI.depth;
        GUI.depth = -6500;
        GUI.color = new Color(previousColor.r, previousColor.g, previousColor.b, previousColor.a * alpha);
        gameControllerUpgradesRect = GUI.Window(GameControllerUpgradesWindowId, gameControllerUpgradesRect, DrawGameControllerUpgradesContents, string.Empty, windowStyle);
        GUI.BringWindowToFront(GameControllerUpgradesWindowId);
        GUI.depth = previousDepth;
        GUI.color = previousColor;
    }

    private void DrawGameControllerUpgradesContents(int id)
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
        GUILayout.Label($"<b>{Ui("UPGRADES")}: {gameControllerUpgradesPlayerName}</b>", headerStyle, GUILayout.Height(28));
        if (GUILayout.Button("<b>X</b>", buttonStyle, GUILayout.Width(34), GUILayout.Height(28)))
        {
            gameControllerUpgradesAvatar = null;
            CloseGameControllerConfirmation();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(6);
        gameControllerUpgradesScroll = GUILayout.BeginScrollView(gameControllerUpgradesScroll, windowStyle);
        if (gameControllerUpgradesAvatar == null)
        {
            DrawLabel(LocalizedMessages.Get("playerNotFound"), Color.gray);
        }
        else
        {
            List<GameController.UpgradeEntry> upgrades = GameController.GetPlayerUpgrades(gameControllerUpgradesAvatar);
            foreach (GameController.UpgradeEntry upgrade in upgrades)
                DrawUpgradeArea(upgrade);
        }

        GUILayout.EndScrollView();
        GUILayout.EndVertical();
        GUI.DragWindow(new Rect(0, 0, gameControllerUpgradesRect.width - 44f, 34f));
    }

    private void DrawUpgradeArea(GameController.UpgradeEntry upgrade)
    {
        string key = upgrade.Id;
        if (!upgradeInputs.ContainsKey(key))
            upgradeInputs[key] = "0";

        GUILayout.Space(6);
        GUILayout.BeginVertical(windowStyle);
        GUILayout.BeginHorizontal();
        GUILayout.Label($"<b>{upgrade.Label} ({Ui("current")}: {upgrade.CurrentLevel})</b>", labelStyle, GUILayout.Width(245));
        upgradeInputs[key] = GUILayout.TextField(upgradeInputs[key], textFieldStyle, GUILayout.Width(120), GUILayout.Height(24));
        if (GUILayout.Button(UiBold("SET"), buttonStyle, GUILayout.Width(64), GUILayout.Height(28)))
            TryOpenUpgradeConfirmation(upgrade, false);
        if (GUILayout.Button(UiBold("ADD"), buttonStyle, GUILayout.Width(64), GUILayout.Height(28)))
            TryOpenUpgradeConfirmation(upgrade, true);
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    private void TryOpenUpgradeConfirmation(GameController.UpgradeEntry upgrade, bool add)
    {
        if (!ValidateGameControllerNumber(upgradeInputs[upgrade.Id], 0L, GameController.MaxUpgradeLevel, "upgradeValue", out long value))
            return;

        if (!HostOnlyGuard.CanUseHostOnly(true, LocalizedMessages.Get("upgradesActionName")))
            return;

        long finalValue = add ? (long)upgrade.CurrentLevel + value : value;
        if (finalValue > GameController.MaxUpgradeLevel)
        {
            ShowMessage(LocalizedMessages.Format("numberOutOfRange", LocalizedMessages.Get("upgradeValue"), 0, GameController.MaxUpgradeLevel), 3.5f);
            return;
        }

        string title = LocalizedMessages.Get(add ? "addUpgradeConfirmTitle" : "setUpgradeConfirmTitle");
        string body = add
            ? LocalizedMessages.Format("addUpgradeConfirmBody", value, upgrade.Label, finalValue)
            : LocalizedMessages.Format("setUpgradeConfirmBody", upgrade.Label, finalValue);

        OpenActionConfirmation(title, body, () =>
        {
            string resultMessage;
            bool ok = add
                ? GameController.AddPlayerUpgrade(gameControllerUpgradesAvatar, upgrade.Id, (int)value, out resultMessage)
                : GameController.SetPlayerUpgrade(gameControllerUpgradesAvatar, upgrade.Id, (int)value, out resultMessage);
            return string.IsNullOrWhiteSpace(resultMessage) ? (ok ? LocalizedMessages.Format("upgradeSet", upgrade.Label, finalValue) : LocalizedMessages.Get("playerActionFailed")) : resultMessage;
        });
    }

    private void DrawGameControllerConfirmation(float alpha)
    {
        if (pendingMapEntry == null && pendingAction == null)
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
        gameControllerConfirmRect = GUI.Window(GameControllerConfirmWindowId, gameControllerConfirmRect, DrawGameControllerConfirmationContents, string.Empty, windowStyle);
        GUI.BringWindowToFront(GameControllerConfirmWindowId);
        GUI.depth = previousDepth;
        GUI.color = previousColor;
    }

    private void DrawGameControllerConfirmationContents(int id)
    {
        GUILayout.BeginVertical();
        DrawLabel(pendingAction?.Title ?? Ui("Change map?"), Color.white);
        GUILayout.Label(pendingAction?.Body ?? pendingMapEntry?.DisplayName ?? string.Empty, labelStyle, GUILayout.Height(42));
        GUILayout.FlexibleSpace();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button(UiBold("CONFIRM"), buttonStyle, GUILayout.Height(32)))
        {
            GameController.MapEntry map = pendingMapEntry;
            GameControllerPendingAction action = pendingAction;
            CloseGameControllerConfirmation();
            string message;
            bool ok = true;
            if (action != null)
            {
                message = action.Execute?.Invoke();
            }
            else
            {
                ok = GameController.ChangeMap(map?.Level, out message);
            }

            if (!string.IsNullOrWhiteSpace(message))
                ShowMessage(message, ok ? 2.5f : 3.5f);
        }

        if (GUILayout.Button(UiBold("CANCEL"), buttonStyle, GUILayout.Height(32)))
            CloseGameControllerConfirmation();
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    private void CloseGameControllerConfirmation()
    {
        pendingMapEntry = null;
        pendingAction = null;
    }
}
