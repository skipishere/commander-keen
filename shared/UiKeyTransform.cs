using System.Linq;
using Godot;

public static class UiKeyTransform
{
    public static string GetKeycode(string action)
    {
        var value = InputMap.ActionGetEvents(action).First();
        if (value is InputEventKey key)
        {
            var code = DisplayServer.KeyboardGetKeycodeFromPhysical(key.PhysicalKeycode);
            return GetUiSymbol(OS.GetKeycodeString(code));
        }

        return string.Empty;
    }

    private static string GetUiSymbol(string keycode)
    {
        return keycode switch
        {
            "arrow_keys" => PromptFont.KEYBOARD_ARROWS,
            "Up" => PromptFont.KEYBOARD_UP,
            "Down" => PromptFont.KEYBOARD_DOWN,
            "Left" => PromptFont.KEYBOARD_LEFT,
            "Right" => PromptFont.KEYBOARD_RIGHT,
            "Ctrl" => PromptFont.KEYBOARD_CONTROL,
            "Enter" => PromptFont.KEYBOARD_ENTER,
            "Space" => PromptFont.KEYBOARD_SPACE,
            "Alt" => PromptFont.KEYBOARD_ALT,
            "Escape" => PromptFont.KEYBOARD_ESCAPE,
            _ => $"Unknown: {keycode}"
        };
    }
}