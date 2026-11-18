using CaseOpener.ViewModels;
using Microsoft.Extensions.Logging;

namespace CaseOpener.Views;

public partial class InventoryPage : ContentPage
{
    private const double MinTileWidth = 190;
    private readonly ILogger<InventoryPage> _log;

    public InventoryPage(InventoryViewModel viewModel, ILogger<InventoryPage> log)
    {
        InitializeComponent();
        _log = log;
        BindingContext = viewModel;
        SizeChanged += OnSizeChanged;
    }

    // Shell keeps both tabs alive, so reload whenever this one is shown again.
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _log.LogInformation("InventoryPage appearing: reloading data");
        if (BindingContext is InventoryViewModel vm) vm.ReloadCommand.Execute(null);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _log.LogInformation("InventoryPage disappearing");
    }

    /// <summary>Responsive column count (layout concern, so it lives in the view).</summary>
    private void OnSizeChanged(object? sender, EventArgs e)
    {
        if (Width <= 0 || ItemGrid.ItemsLayout is not GridItemsLayout grid) return;
        var span = Math.Max(1, (int)((Width - 48) / MinTileWidth));
        if (grid.Span != span) grid.Span = span;
    }
}
