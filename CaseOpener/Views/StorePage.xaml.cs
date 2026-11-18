using CaseOpener.ViewModels;
using Microsoft.Extensions.Logging;

namespace CaseOpener.Views;

public partial class StorePage : ContentPage
{
    private const double MinTileWidth = 230;
    private readonly ILogger<StorePage> _log;

    public StorePage(StoreViewModel viewModel, ILogger<StorePage> log)
    {
        InitializeComponent();
        _log = log;
        BindingContext = viewModel;
        SizeChanged += OnSizeChanged;
    }

    // The page became visible: ask the ViewModel to (re)load its data.
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _log.LogInformation("StorePage appearing: reloading data");
        if (BindingContext is StoreViewModel vm) vm.ReloadCommand.Execute(null);
    }

    // The page is being hidden (another tab, minimise): log the transition.
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _log.LogInformation("StorePage disappearing");
    }

    /// <summary>Pure layout concern, so it lives in the view: responsive column count.</summary>
    private void OnSizeChanged(object? sender, EventArgs e)
    {
        if (Width <= 0 || CaseGrid.ItemsLayout is not GridItemsLayout grid) return;
        var span = Math.Max(1, (int)((Width - 48) / MinTileWidth));
        if (grid.Span != span) grid.Span = span;
    }
}
