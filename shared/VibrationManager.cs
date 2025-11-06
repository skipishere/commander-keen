using Godot;

/// <summary>
/// Manages controller vibration.
/// </summary>
public static class VibrationManager
{
    // Vibration settings
    public static bool Enabled { get; set; } = true;
    public static float Intensity { get; set; } = 1.0f;

    /// <summary>
    /// Start controller vibration, if enabled.
    /// </summary>
    public static void TryStartVibration(float weakMagnitude, float strongMagnitude, float duration)
    {
        if (!Enabled)
        {
            return;
        }

        var adjustedWeak = Mathf.Clamp(weakMagnitude * Intensity, 0f, 1f);
        var adjustedStrong = Mathf.Clamp(strongMagnitude * Intensity, 0f, 1f);
        Input.StartJoyVibration(0, adjustedWeak, adjustedStrong, duration);
    }

    /// <summary>
    /// Force stop any active vibration and reset the delay timer.
    /// </summary>
    public static void StopVibration()
    {
        Input.StopJoyVibration(0);
    }
}
