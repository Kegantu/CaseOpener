using CaseOpener.ViewModels;
using Microsoft.Extensions.Logging;

namespace CaseOpener.Services;

/// <summary>
/// Handles links like <c>caseopener://case/revolution</c>, <c>caseopener://inventory</c>,
/// <c>caseopener://settings</c>. Links that arrive before the UI exists (cold start from a link)
/// are kept and processed once the shell has loaded.
/// </summary>
public interface IDeepLinkService
{
    /// <summary>Returns false if the link isn't ours or points to something unknown.</summary>
    Task<bool> HandleAsync(Uri uri);

    /// <summary>Called by the shell when it has loaded: marks the UI ready and runs a waiting link.</summary>
    Task OnUiReadyAsync();
}

public sealed class DeepLinkService : IDeepLinkService
{
    public const string Scheme = "caseopener";

    private readonly IServiceProvider _services;
    private readonly ILogger<DeepLinkService> _log;

    private Uri? _pending;
    private bool _uiReady;

    public DeepLinkService(IServiceProvider services, ILogger<DeepLinkService> log)
    {
        _services = services;
        _log = log;
    }

    public Task<bool> HandleAsync(Uri uri) =>
        MainThread.InvokeOnMainThreadAsync(() => HandleCoreAsync(uri));

    public async Task OnUiReadyAsync()
    {
        _uiReady = true;
        if (_pending is { } uri)
        {
            _pending = null;
            _log.LogInformation("Processing deep link that arrived at startup: {Uri}", uri);
            await HandleAsync(uri);
        }
    }

    private async Task<bool> HandleCoreAsync(Uri uri)
    {
        if (!string.Equals(uri.Scheme, Scheme, StringComparison.OrdinalIgnoreCase))
        {
            _log.LogWarning("Ignoring link with unknown scheme: {Uri}", uri);
            return false;
        }

        if (!_uiReady || Shell.Current is not { } shell)
        {
            _log.LogInformation("UI not ready, deep link queued: {Uri}", uri);
            _pending = uri;
            return true;
        }

        _log.LogInformation("Handling deep link: {Uri}", uri);

        switch (uri.Host.ToLowerInvariant())
        {
            case "case":
            {
                var id = uri.AbsolutePath.Trim('/');
                await shell.GoToAsync("//store");
                var opened = id.Length > 0 && _services.GetRequiredService<StoreViewModel>().OpenCaseById(id);
                if (!opened) _log.LogWarning("Deep link: unknown case '{Id}' (or a case is being opened)", id);
                return opened;
            }
            case "inventory":
                await shell.GoToAsync("//inventory");
                return true;
            case "settings":
                await shell.GoToAsync("//settings");
                return true;
            default:
                _log.LogWarning("Deep link: unknown target '{Host}'", uri.Host);
                return false;
        }
    }
}
