using CaseOpener.Services;

namespace CaseOpener;

public partial class AppShell : Shell
{
    private static readonly string[] Routes = { "store", "inventory", "settings" };

    private readonly IAppStateService _state;

    public AppShell(IAppStateService state, IDeepLinkService deepLinks)
    {
        InitializeComponent();
        _state = state;

        // Restore the tab that was open last time.
        var tabs = Items[0];
        var index = Array.IndexOf(Routes, _state.LastRoute);
        if (index >= 0 && index < tabs.Items.Count) tabs.CurrentItem = tabs.Items[index];

        // Remember the current tab (written to Preferences when the window stops).
        Navigated += (_, e) =>
        {
            var location = e.Current?.Location?.OriginalString ?? string.Empty;
            var route = Routes.FirstOrDefault(r => location.Contains(r, StringComparison.OrdinalIgnoreCase));
            if (route is not null) _state.LastRoute = route;
        };

        // A link that started the app is handled only once the shell is on screen.
        Loaded += async (_, _) => await deepLinks.OnUiReadyAsync();
    }
}
