using System.Collections.ObjectModel;
using CaseOpener.Models;
using CaseOpener.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CaseOpener.ViewModels;

public sealed partial class InventoryViewModel : ObservableObject
{
    private readonly IInventoryService _inventory;
    private readonly IWalletService _wallet;
    private readonly IDialogService _dialogs;
    private readonly IAppStateService _state;

    public InventoryViewModel(IInventoryService inventory, IWalletService wallet, IDialogService dialogs,
        IAppStateService state)
    {
        _inventory = inventory;
        _wallet = wallet;
        _dialogs = dialogs;
        _state = state;

        Filters = new ObservableCollection<RarityFilterViewModel>
        {
            new("All", null) { IsSelected = true },
            new("Mil-Spec", Rarity.MilSpec),
            new("Restricted", Rarity.Restricted),
            new("Classified", Rarity.Classified),
            new("Covert", Rarity.Covert),
            new("★ Rare Special", Rarity.RareSpecial)
        };

        // Restore search / sort / rarity filter from the last session (fields set directly so no
        // refresh runs before the constructor finishes).
        searchText = _state.InventorySearch;
        sortByRarity = _state.InventorySortByRarity;
        var savedChip = Filters.FirstOrDefault(f => f.Rarity == _state.InventoryFilter);
        if (savedChip is not null)
            foreach (var f in Filters) f.IsSelected = ReferenceEquals(f, savedChip);

        // Drops made on the Store tab (and sales made here) refresh the list immediately.
        _inventory.Items.CollectionChanged += (_, _) => Refresh();
        _wallet.PropertyChanged += (_, _) => OnPropertyChanged(nameof(BalanceText));

        Refresh();
    }

    public ObservableCollection<RarityFilterViewModel> Filters { get; }

    /// <summary>Replaced with a fresh collection on every refresh, so the grid always rebinds.</summary>
    [ObservableProperty]
    private ObservableCollection<InventoryItem> items = new();

    public bool IsEmpty => Items.Count == 0;
    public bool HasItems => Items.Count > 0;

    public string BalanceText => $"◆ {_wallet.Balance:N0}";

    public string CountText =>
        $"{Items.Count} shown · {_inventory.Items.Count} total · worth ◆ {_inventory.Items.Sum(i => i.Value):N0}";

    public string SellAllText => $"Sell all shown   ◆ {Items.Sum(i => i.Value):N0}";
    public string SortLabel => SortByRarity ? "Sort: Rarity" : "Sort: Newest";

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SortLabel))]
    private bool sortByRarity;

    partial void OnSearchTextChanged(string value)
    {
        _state.InventorySearch = value;
        Refresh();
    }

    partial void OnSortByRarityChanged(bool value)
    {
        _state.InventorySortByRarity = value;
        Refresh();
    }

    [RelayCommand]
    private void SelectFilter(RarityFilterViewModel chip)
    {
        foreach (var f in Filters) f.IsSelected = ReferenceEquals(f, chip);
        _state.InventoryFilter = chip.Rarity;
        Refresh();
    }

    [RelayCommand]
    private void ToggleSort() => SortByRarity = !SortByRarity;

    /// <summary>Rebuilds the list from the inventory service. The page calls it when the tab is shown.</summary>
    [RelayCommand]
    private void Reload()
    {
        OnPropertyChanged(nameof(BalanceText));
        Refresh();
    }

    /// <summary>Sells one item. Covert and Rare Special items ask for confirmation first.</summary>
    [RelayCommand]
    private async Task SellAsync(InventoryItem item)
    {
        if (item.Rarity >= Rarity.Covert)
        {
            var ok = await _dialogs.ConfirmAsync(
                "Sell this item?",
                $"{item.DisplayName}\n{item.Subtitle}\n\nSell for ◆ {item.Value:N0}?",
                "Sell", "Keep");
            if (!ok) return;
        }

        // Remove() is false if the item is already gone (e.g. a double tap), so coins are paid once.
        if (_inventory.Remove(item.Id)) _wallet.Add(item.Value);
    }

    /// <summary>Sells everything the current filter/search shows, after confirming the total.</summary>
    [RelayCommand]
    private async Task SellAllShownAsync()
    {
        var targets = Items.ToList();
        if (targets.Count == 0) return;

        var total = targets.Sum(i => i.Value);
        var ok = await _dialogs.ConfirmAsync(
            "Sell all shown?",
            $"Sell {targets.Count} item(s) for ◆ {total:N0}?",
            "Sell all", "Cancel");
        if (!ok) return;

        var paid = 0;
        foreach (var t in targets)
            if (_inventory.Remove(t.Id)) paid += t.Value;

        _wallet.Add(paid);
    }

    private void Refresh()
    {
        var rarity = Filters.FirstOrDefault(f => f.IsSelected)?.Rarity;
        var query = SearchText?.Trim() ?? "";

        IEnumerable<InventoryItem> result = _inventory.Items
            .Where(i => rarity is null || i.Rarity == rarity)
            .Where(i => query.Length == 0 ||
                        i.Item.FullName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                        i.CaseName.Contains(query, StringComparison.OrdinalIgnoreCase));

        result = SortByRarity
            ? result.OrderByDescending(i => i.Rarity).ThenByDescending(i => i.Value)
            : result.OrderByDescending(i => i.ObtainedAt);

        Items = new ObservableCollection<InventoryItem>(result.ToList());
        OnPropertyChanged(nameof(CountText));
        OnPropertyChanged(nameof(SellAllText));
        OnPropertyChanged(nameof(IsEmpty));
        OnPropertyChanged(nameof(HasItems));
    }
}
