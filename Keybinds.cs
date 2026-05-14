using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnifromCheat_REPO.GUIMenuSkin;
using static UnifromCheat_REPO.TooltipsLanguages;

namespace UnifromCheat_REPO;

public partial class Core
{
    internal static string noclipBind = "LeftAlt";
    internal static string hideMeBind = "F9";
    internal static string freecamBind = "F6";

    private string activeKeybindCaptureId;
    private string keybindCaptureError;
    private float keybindCaptureErrorUntil;
    private static int suppressKeybindHotkeysFrame = -1;

    internal static bool WasNoclipHotkeyPressed()
    {
        return WasKeybindPressed(noclipBind);
    }

    internal static bool WasHideMeHotkeyPressed()
    {
        return WasKeybindPressed(hideMeBind);
    }

    internal static bool WasFreecamHotkeyPressed()
    {
        return WasKeybindPressed(freecamBind);
    }

    private void UpdateKeybindCapture()
    {
        if (string.IsNullOrEmpty(activeKeybindCaptureId))
            return;

        var keyboard = Keyboard.current;
        if (keyboard == null)
            return;

        if (keyboard.allKeys.Count == 0)
            return;

        Key pressedKey = Key.None;
        foreach (var keyControl in keyboard.allKeys)
        {
            if (keyControl == null || !keyControl.wasPressedThisFrame || keyControl.keyCode == Key.None)
                continue;

            pressedKey = keyControl.keyCode;
            break;
        }

        if (pressedKey == Key.None)
            return;

        ApplyCapturedKeybind(pressedKey);
    }

    private void DrawKeybindField(string label, ref string serializedBind, string defaultBind, string captureId)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(label, labelStyle, GUILayout.Width(40));

        bool capturing = activeKeybindCaptureId == captureId;
        if (capturing)
            HandleKeybindGuiEvent();

        string display = capturing ? Get("keybindPressKeys") : FormatKeybind(serializedBind);
        Rect fieldRect = GUILayoutUtility.GetRect(132, 24, GUILayout.ExpandWidth(false));

        GUI.Box(fieldRect, GUIContent.none, textFieldStyle);
        GUI.Label(new Rect(fieldRect.x + 6f, fieldRect.y + 3f, fieldRect.width - 12f, fieldRect.height), display, labelStyle);

        if (GUILayout.Button("<b>SET</b>", buttonStyle, GUILayout.Width(46), GUILayout.Height(24)))
        {
            activeKeybindCaptureId = captureId;
            keybindCaptureError = null;
            GUI.FocusControl(null);
        }

        if (GUILayout.Button("<b>R</b>", buttonStyle, GUILayout.Width(28), GUILayout.Height(24)))
        {
            if (activeKeybindCaptureId == captureId)
                activeKeybindCaptureId = null;

            serializedBind = defaultBind;
            keybindCaptureError = null;
            GUI.FocusControl(null);
        }

        GUILayout.EndHorizontal();

        if (capturing && !string.IsNullOrEmpty(keybindCaptureError) && Time.unscaledTime < keybindCaptureErrorUntil)
            DrawLabel(keybindCaptureError, new Color(1f, 0.55f, 0.25f, 1f));
    }

    private void HandleKeybindGuiEvent()
    {
        Event current = Event.current;
        if (current == null || current.type != EventType.KeyDown)
            return;

        if (!TryMapKeyCode(current.keyCode, out Key mainKey))
            return;

        ApplyCapturedKeybind(mainKey);
        current.Use();
    }

    private void ApplyCapturedKeybind(Key pressedKey)
    {
        if (pressedKey == Key.Insert)
        {
            suppressKeybindHotkeysFrame = Time.frameCount;
            SetKeybindCaptureError(Get("keybindNoInsert"));
            return;
        }

        if (pressedKey == Key.None)
            return;

        suppressKeybindHotkeysFrame = Time.frameCount;
        string serialized = pressedKey.ToString();
        if (activeKeybindCaptureId == "noclip")
            noclipBind = serialized;
        else if (activeKeybindCaptureId == "hideMe")
            hideMeBind = serialized;
        else if (activeKeybindCaptureId == "freecam")
            freecamBind = serialized;

        activeKeybindCaptureId = null;
        keybindCaptureError = null;
        GUI.FocusControl(null);
    }

    private static bool TryMapKeyCode(KeyCode keyCode, out Key key)
    {
        key = Key.None;
        if (keyCode == KeyCode.None)
            return false;

        switch (keyCode)
        {
            case KeyCode.LeftControl: key = Key.LeftCtrl; return true;
            case KeyCode.RightControl: key = Key.RightCtrl; return true;
            case KeyCode.LeftShift: key = Key.LeftShift; return true;
            case KeyCode.RightShift: key = Key.RightShift; return true;
            case KeyCode.LeftAlt: key = Key.LeftAlt; return true;
            case KeyCode.RightAlt: key = Key.RightAlt; return true;
            case KeyCode.Insert: key = Key.Insert; return true;
        }

        string name = keyCode.ToString();
        if (name.StartsWith("Alpha") && name.Length == 6)
            return Enum.TryParse("Digit" + name.Substring(5), out key);

        if (name.StartsWith("Keypad"))
            return Enum.TryParse("Numpad" + name.Substring(6), out key);

        return Enum.TryParse(name, out key);
    }

    private static bool WasKeybindPressed(string serializedBind)
    {
        var keyboard = Keyboard.current;
        if (keyboard == null)
            return false;

        if (suppressKeybindHotkeysFrame == Time.frameCount)
            return false;

        Key[] keys = ParseKeybind(serializedBind);
        if (keys.Length == 0)
            return false;

        bool anyPressedThisFrame = false;
        foreach (Key key in keys)
        {
            if (key == Key.Insert)
                return false;

            var control = GetKeyControlSafe(keyboard, key);
            if (control == null || !control.isPressed)
                return false;

            if (control.wasPressedThisFrame)
                anyPressedThisFrame = true;
        }

        return anyPressedThisFrame;
    }

    private static UnityEngine.InputSystem.Controls.KeyControl GetKeyControlSafe(Keyboard keyboard, Key key)
    {
        if (keyboard == null || key == Key.None)
            return null;

        try
        {
            return keyboard[key];
        }
        catch
        {
            return null;
        }
    }

    private static Key[] ParseKeybind(string serializedBind)
    {
        if (string.IsNullOrWhiteSpace(serializedBind))
            return Array.Empty<Key>();

        return serializedBind
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(part => Enum.TryParse(part.Trim(), out Key key) ? key : Key.None)
            .Where(key => key != Key.None)
            .Take(3)
            .ToArray();
    }

    private static string FormatKeybind(string serializedBind)
    {
        Key[] keys = ParseKeybind(serializedBind);
        if (keys.Length == 0)
            return "None";

        return string.Join(" + ", keys.Select(FormatKey).ToArray());
    }

    private static string FormatKey(Key key)
    {
        string text = key.ToString();
        text = text.Replace("Left", "Left ");
        text = text.Replace("Right", "Right ");
        text = text.Replace("Digit", string.Empty);
        return text;
    }

    private void SetKeybindCaptureError(string message)
    {
        keybindCaptureError = string.IsNullOrWhiteSpace(message) ? "Invalid keybind" : message;
        keybindCaptureErrorUntil = Time.unscaledTime + 1.8f;
    }
}
