using Godot;
using System.Reflection;

/// <summary>
/// Utility class for accessing version information embedded during build.
/// This reads from assembly metadata that gets populated from Git tags.
/// </summary>
[GlobalClass]
public partial class VersionInfo : Node
{
    private static string _version;
    private static string _fullVersion;

    /// <summary>
    /// Gets the version string (e.g., "1.2.3" or "1.2.3-beta")
    /// </summary>
    public static string DisplayVersion
    {
        get
        {
            if (_version == null)
            {
                var assembly = Assembly.GetExecutingAssembly();
                var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion
                             ?? assembly.GetName().Version?.ToString()
                             ?? "Unknown";

                // Clean up version string (remove build metadata after +)
                var plusIndex = version.IndexOf('+');
                _version = plusIndex > 0 ? version.Substring(0, plusIndex) : version;
            }
            return _version;
        }
    }

    /// <summary>
    /// Gets the full version string including build metadata
    /// </summary>
    public static string FullVersion
    {
        get
        {
            if (_fullVersion == null)
            {
                var assembly = Assembly.GetExecutingAssembly();
                _fullVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                              ?? assembly.GetName().Version?.ToString()
                              ?? "Unknown";
            }
            return _fullVersion;
        }
    }

    /// <summary>
    /// Logs version information to Godot console (useful for debugging)
    /// </summary>
    public static void LogVersionInfo()
    {
        GD.Print($"Commander Keen Version: {DisplayVersion}");
        GD.Print($"Full Version Info: {FullVersion}");
    }
}
