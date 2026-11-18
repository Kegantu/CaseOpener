using System.Collections.ObjectModel;
using CaseOpener.Models;
using CaseOpener.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CaseOpener.ViewModels;

public sealed partial class StoreViewModel : ObservableObject
{
    private readonly ICaseCatalog _catalog;
    private readonly IWalletService _wallet;
    private readonly IInventoryService _inventory;
    private readonly IDropService _drops;
    private readonly IAppStateService _state;

    public StoreViewModel(ICaseCatalog catalog, IWalletService wallet, IInventoryService inventory,
        IDropService drops, IAppStateService state)
    {
        _catalog = catalog;
        _wallet = wallet;
        _inventory = inventory;
        _drops = drops;
        _state = state;

        Filters = new ObservableCollection<FilterChipViewModel>
        {
            new("All", null) { IsSelected = true },
            new("Weapon Cases", CaseKind.Weapon),
            new("Collections", CaseKind.Collection),
            new("Souvenir", CaseKind.Souvenir)
        };

        // Restore the search text and filter from the last session (field set directly so it
        // doesn't trigger a filter pass before everything is initialised).
        searchText = _state.StoreSearch;
        var savedChip = Filters.FirstOrDefault(f => f.Kind == _state.StoreFilter);
        if (savedChip is not null)
            foreach (var f in Filters) f.IsSelected = ReferenceEquals(f, savedChip);

        _wallet.PropertyChanged += (_, _) => OnPropertyChanged(nameof(BalanceText));
        ApplyFilter();
    }

    public ObservableCollection<FilterChipViewModel> Filters { get; }
    public ObservableCollection<CaseCardViewModel> Cases { get; } = new();

    public string BalanceText => $"◆ {_wallet.Balance:N0}";
    public string CountText => $"{Cases.Count} cases available.";

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsPreviewOpen))]
    private CasePreviewViewModel? preview;

    public bool IsPreviewOpen => Preview is not null;

    partial void OnSearchTextChanged(string value)
    {
        _state.StoreSearch = value;
        ApplyFilter();
    }

    [RelayCommand]
    private void SelectFilter(FilterChipViewModel chip)
    {
        foreach (var f in Filters) f.IsSelected = ReferenceEquals(f, chip);
        _state.StoreFilter = chip.Kind;
        ApplyFilter();
    }

    /// <summary>Called by the page's OnAppearing: refreshes the balance and the case list.</summary>
    [RelayCommand]
    private void Reload()
    {
        OnPropertyChanged(nameof(BalanceText));
        ApplyFilter();
    }

    /// <summary>Opens the preview for a case by id (used by deep links). False if unknown or busy.</summary>
    public bool OpenCaseById(string id)
    {
        if (Preview is { IsOpening: true }) return false; // don't pull the case out from under a spin

        var model = _catalog.GetCases().FirstOrDefault(c => c.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        if (model is null) return false;

        OpenPreview(new CaseCardViewModel(model));
        return true;
    }

    /// <summary>Fired when a case tile is pressed: builds and shows the case preview.</summary>
    [RelayCommand]
    private void OpenPreview(CaseCardViewModel card)
    {
        Preview?.Dispose();
        Preview = new CasePreviewViewModel(card.Model, _wallet, _inventory, _drops, ClosePreview);
    }

    [RelayCommand]
    private void ClosePreview()
    {
        Preview?.Dispose();
        Preview = null;
    }

    private void ApplyFilter()
    {
        var kind = Filters.FirstOrDefault(f => f.IsSelected)?.Kind;
        var query = SearchText?.Trim() ?? "";

        var result = _catalog.GetCases()
            .Where(c => kind is null || c.Kind == kind)
            .Where(c => query.Length == 0 || c.Name.Contains(query, StringComparison.OrdinalIgnoreCase));

        Cases.Clear();
        foreach (var c in result) Cases.Add(new CaseCardViewModel(c));
        OnPropertyChanged(nameof(CountText));
    }
}
