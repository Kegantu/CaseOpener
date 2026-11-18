using CaseOpener.Services;
using Microsoft.Extensions.Logging;

namespace CaseOpener;

public partial class App : Application
{
    private readonly IAppStateService _state;
    private readonly IDeepLinkService _deepLinks;
    private readonly ILogger<App> _log;

    public App(IAppStateService state, IThemeService theme, IDeepLinkService deepLinks, ILogger<App> log)
    {
        InitializeComponent();

        _state = state;
        _deepLinks = deepLinks;
        _log = log;

        // Apply the saved light/dark/system choice before any page is created.
        theme.Initialize();
    }

    // AppShell is created here (after InitializeComponent has merged the styles), and the pages
    // inside it are created lazily by Shell through DI.
    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new AppShell(_state, _deepLinks)) { Title = "CS2 Case Opener" };

        window.Created += (_, _) => _log.LogInformation("Window: Created");
        window.Activated += (_, _) => _log.LogInformation("Window: Activated (came to the foreground)");

        window.Deactivated += (_, _) =>
        {
            _log.LogInformation("Window: Deactivated (lost focus), saving state");
            _state.Save();
        };

        window.Stopped += (_, _) =>
        {
            _log.LogInformation("Window: Stopped (minimised / in the background), saving state");
            _state.Save();
        };

        window.Resumed += (_, _) => _log.LogInformation(
            "Window: Resumed, restored state: tab '{Route}', store search '{Search}'",
            _state.LastRoute, _state.StoreSearch);

        window.Destroying += (_, _) =>
        {
            _log.LogInformation("Window: Destroying (app is closing), saving state");
            _state.Save();
        };

        return window;
    }

    // Used on platforms that deliver app links through the MAUI Application (iOS / macOS).
    protected override void OnAppLinkRequestReceived(Uri uri)
    {
        base.OnAppLinkRequestReceived(uri);
        _log.LogInformation("App link received: {Uri}", uri);
        _ = _deepLinks.HandleAsync(uri);
    }
}
