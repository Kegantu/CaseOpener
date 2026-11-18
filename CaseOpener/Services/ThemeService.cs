namespace CaseOpener.Services;

public enum ThemeMode
{
    System,
    Light,
    Dark
}

public interface IThemeService
{
    ThemeMode Mode { get; }

    /// <summary>Loads the saved choice and applies it. Call once, right after the App's InitializeComponent.</summary>
    void Initialize();

    /// <summary>Applies the theme to every page and saves the choice in Preferences.</summary>
    void SetMode(ThemeMode mode);
}

/// <summary>
/// Pages pick their colours with {AppThemeBinding Light=..., Dark=...} (see Resources/Styles/Palette.xaml),
/// so switching the theme is just setting Application.UserAppTheme: MAUI re-evaluates every binding itself.
/// </summary>
public sealed class ThemeService : IThemeService
{
    private const string PreferenceKey = "theme_mode";

    public ThemeMode Mode { get; private set; } = ThemeMode.Dark;

    public void Initialize()
    {
        var saved = Preferences.Get(PreferenceKey, nameof(ThemeMode.Dark));
        Mode = Enum.TryParse<ThemeMode>(saved, out var mode) ? mode : ThemeMode.Dark;
        Apply();
    }

    public void SetMode(ThemeMode mode)
    {
        Mode = mode;
        Preferences.Set(PreferenceKey, mode.ToString());
        Apply();
    }

    private void Apply()
    {
        if (Application.Current is not { } app) return;

        // Unspecified = follow the OS light/dark setting (and keep following it when it changes).
        app.UserAppTheme = Mode switch
        {
            ThemeMode.Light => AppTheme.Light,
            ThemeMode.Dark => AppTheme.Dark,
            _ => AppTheme.Unspecified
        };
    }
}
