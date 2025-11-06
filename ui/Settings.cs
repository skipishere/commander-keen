using Godot;

public partial class Settings : PanelContainer
{
    [Export]
    private Label vibrationIntensityLabel;

    [Export]
    private CheckButton vibrationEnabledCheckbox;

    [Export]
    public Control DefaultButton;

    public override void _Ready()
    {
        UpdateVibrationDisplay();
    }

    public void OnVisibilityChanged()
    {
        if (Visible)
        {
            UpdateVibrationDisplay();
            DefaultButton.GrabFocus();
        }
    }

    public void ToggleVibration(bool pressed)
    {
        VibrationManager.Enabled = pressed;
        TestVibration();
    }

    public void IncreaseVibration()
    {
        VibrationManager.Intensity = Mathf.Min(VibrationManager.Intensity + 0.1f, 2.0f);
        TestVibration();
    }

    public void DecreaseVibration()
    {
        VibrationManager.Intensity = Mathf.Max(VibrationManager.Intensity - 0.1f, 0.1f);
        TestVibration();
    }

    public void TestVibration()
    {
        UpdateVibrationDisplay();
        VibrationManager.StopVibration();
        VibrationManager.TryStartVibration(0.5f, 0.5f, 0.3f);
    }

    public void BackToMainMenu()
    {
        Visible = false;
        GetParent<Pause>().ShowMainMenu();
    }

    private void UpdateVibrationDisplay()
    {
        vibrationEnabledCheckbox.ButtonPressed = VibrationManager.Enabled;
        vibrationIntensityLabel.Text = $"Intensity: {VibrationManager.Intensity:F1}x";
    }
}
