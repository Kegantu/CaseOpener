using CaseOpener.ViewModels;
using Microsoft.Extensions.Logging;

namespace CaseOpener.Views;

public partial class SettingsPage : ContentPage
{
    private readonly ILogger<SettingsPage> _log;

    public SettingsPage(SettingsViewModel viewModel, ILogger<SettingsPage> log)
    {
        InitializeComponent();
        _log = log;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _log.LogInformation("SettingsPage appearing");
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _log.LogInformation("SettingsPage disappearing");
    }
}
