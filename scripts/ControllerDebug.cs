using Godot;

/// <summary>
/// Debug utility to capture and display all controller input events.
/// Attach this to any Node in your scene to see what buttons are being pressed.
/// </summary>
public partial class ControllerDebug : Node
{
    private Label debugLabel;
    private string lastInput = "Waiting for input...";

    public override void _Ready()
    {
        // Create debug label overlay
        debugLabel = new Label();
        debugLabel.Position = new Vector2(10, 10);
        debugLabel.AddThemeColorOverride("font_color", Colors.Yellow);
        debugLabel.AddThemeColorOverride("font_shadow_color", Colors.Black);
        debugLabel.AddThemeFontSizeOverride("font_size", 12);
        debugLabel.Scale = new Vector2(0.8f, 0.8f);
        debugLabel.Text = lastInput;
        AddChild(debugLabel);

        GD.Print("Controller Debug Active - Press any button/axis");
    }

    public override void _Input(InputEvent @event)
    {
        string debugInfo = "";

        if (@event is InputEventJoypadButton buttonEvent)
        {
            debugInfo = $"Button: {buttonEvent.ButtonIndex} ({GetButtonName(buttonEvent.ButtonIndex)}) - Pressed: {buttonEvent.Pressed}";
            GD.Print(debugInfo);
            lastInput = debugInfo;
            UpdateLabel();
        }
        else if (@event is InputEventJoypadMotion motionEvent)
        {
            // Only log significant axis movement to avoid spam
            if (Mathf.Abs(motionEvent.AxisValue) > 0.5f)
            {
                debugInfo = $"Axis: {motionEvent.Axis} ({GetAxisName(motionEvent.Axis)}) - Value: {motionEvent.AxisValue:F2}";
                GD.Print(debugInfo);
                lastInput = debugInfo;
                UpdateLabel();
            }
        }
    }

    private void UpdateLabel()
    {
        if (debugLabel != null)
        {
            debugLabel.Text = $"Input: {lastInput}";
        }
    }

    private string GetButtonName(JoyButton button)
    {
        return button switch
        {
            JoyButton.A => "A/Cross",
            JoyButton.B => "B/Circle",
            JoyButton.X => "X/Square",
            JoyButton.Y => "Y/Triangle",
            JoyButton.Back => "Back/Select/View (4)",
            JoyButton.Guide => "Guide/PS/Xbox",
            JoyButton.Start => "Start/Options (6)",
            JoyButton.LeftStick => "L3",
            JoyButton.RightStick => "R3",
            JoyButton.LeftShoulder => "L1/LB",
            JoyButton.RightShoulder => "R1/RB",
            JoyButton.DpadUp => "D-Pad Up",
            JoyButton.DpadDown => "D-Pad Down",
            JoyButton.DpadLeft => "D-Pad Left",
            JoyButton.DpadRight => "D-Pad Right",
            _ => $"Unknown ({(int)button})"
        };
    }

    private string GetAxisName(JoyAxis axis)
    {
        return axis switch
        {
            JoyAxis.LeftX => "Left Stick X",
            JoyAxis.LeftY => "Left Stick Y",
            JoyAxis.RightX => "Right Stick X",
            JoyAxis.RightY => "Right Stick Y",
            JoyAxis.TriggerLeft => "L2/LT",
            JoyAxis.TriggerRight => "R2/RT",
            _ => $"Unknown ({(int)axis})"
        };
    }
}
