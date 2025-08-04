using Godot;

/// <summary>
/// UI component for displaying version information in the game.
/// Add this to any scene where you want to show the version (title screen, pause menu, etc.)
/// </summary>
public partial class VersionUI : Label
{
    [Export] public bool ShowFullVersion = false;
    [Export] public string Prefix = "Version: ";

    public override void _Ready()
    {
        UpdateVersionDisplay();
        
        // Log version info when first loaded (useful for debugging)
        VersionInfo.LogVersionInfo();
    }

    private void UpdateVersionDisplay()
    {
        var versionText = ShowFullVersion ? VersionInfo.FullVersion : VersionInfo.DisplayVersion;
        Text = $"{Prefix}{versionText}";
    }

    /// <summary>
    /// Call this method to refresh the version display if needed
    /// </summary>
    public void RefreshVersion()
    {
        UpdateVersionDisplay();
    }
}
