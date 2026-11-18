using System.Collections.ObjectModel;
using CaseOpener.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CaseOpener.ViewModels;

public sealed partial class SettingsViewModel : ObservableObject
{
    private readonly IThemeService _theme;
    private readonly IDeepLinkService _links;

    public SettingsViewModel(IThemeService theme, IDeepLinkService links)
    {
        _theme = theme;
        _links = links;

        ThemeOptions = new ObservableCollection<ThemeOptionViewModel>
        {
            new(ThemeMode.System, "System", "Follow the device's light / dark setting"),
            new(ThemeMode.Light, "Light", "Bright background"),
            new(ThemeMode.Dark, "Dark", "Dark background (default)")
        };

        foreach (var option in ThemeOptions)
            option.IsSelected = option.Mode == _theme.Mode;
    }

    public ObservableCollection<ThemeOptionViewModel> ThemeOptions { get; }

    [RelayCommand]
    private void SelectTheme(ThemeOptionViewModel option)
    {
        foreach (var o in ThemeOptions) o.IsSelected = ReferenceEquals(o, option);
        _theme.SetMode(option.Mode); // applies to all pages and saves to Preferences
    }

    // ---- Deep link tester (same code path as a link opened from outside the app) ----

    [ObservableProperty]
    private string deepLinkText = "caseopener://case/revolution";

    [ObservableProperty]
    private string deepLinkStatus = string.Empty;

    [RelayCommand]
    private async Task OpenDeepLinkAsync()
    {
        if (!Uri.TryCreate(DeepLinkText?.Trim(), UriKind.Absolute, out var uri))
        {
            DeepLinkStatus = "That is not a valid link.";
            return;
        }

        var handled = await _links.HandleAsync(uri);
        DeepLinkStatus = handled ? "Link opened." : "Unknown link or case id.";
    }
}
