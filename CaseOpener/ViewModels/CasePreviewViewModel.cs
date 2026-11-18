using System.Collections.ObjectModel;
using CaseOpener.Models;
using CaseOpener.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CaseOpener.ViewModels;

/// <summary>Items of one rarity inside the preview, with the group's drop chance.</summary>
public sealed class RarityGroup : List<SkinItem>
{
    public RarityGroup(Rarity rarity, IEnumerable<SkinItem> items) : base(items) => Rarity = rarity;

    public Rarity Rarity { get; }
    public string Name => Rarity.DisplayName();
    public string ChanceText => $"{Rarity.DropChance():0.##}%";
}

public sealed partial class CasePreviewViewModel : ObservableObject, IDisposable
{
    private readonly IWalletService _wallet;
    private readonly IInventoryService _inventory;
    private readonly IDropService _drops;
    private readonly Action _close;

    public CasePreviewViewModel(CaseModel model, IWalletService wallet, IInventoryService inventory,
        IDropService drops, Action close)
    {
        Model = model;
        _wallet = wallet;
        _inventory = inventory;
        _drops = drops;
        _close = close;

        Groups = new ObservableCollection<RarityGroup>(
            model.Items
                .GroupBy(i => i.Rarity)
                .OrderBy(g => g.Key)
                .Select(g => new RarityGroup(g.Key, g)));

        _wallet.PropertyChanged += OnWalletChanged;
    }

    public CaseModel Model { get; }
    public string Name => Model.Name;
    public string PriceText => $"◆ {Model.Price:N0}";
    public string ItemCountText => $"{Model.Items.Count} possible items";
    public Rarity TopRarity => Model.TopRarity;
    public ObservableCollection<RarityGroup> Groups { get; }

    public int Balance => _wallet.Balance;
    public bool CanAfford => _wallet.Balance >= Model.Price;
    public bool CanOpen => CanAfford && !IsOpening;
    public string OpenButtonText => IsOpening ? "OPENING…" : $"OPEN CASE   ◆ {Model.Price:N0}";
    public string? Hint => CanAfford ? null : $"Not enough coins — you need ◆ {Model.Price - _wallet.Balance:N0} more.";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanOpen), nameof(OpenButtonText))]
    [NotifyCanExecuteChangedFor(nameof(OpenCaseCommand), nameof(CloseCommand))]
    private bool isOpening;

    /// <summary>True while the roulette is on screen.</summary>
    [ObservableProperty]
    private bool isSpinning;

    /// <summary>The strip shown in the roulette.</summary>
    [ObservableProperty]
    private IReadOnlyList<SkinItem> reelItems = Array.Empty<SkinItem>();

    /// <summary>
    /// Supplied by the view: plays the spin animation toward the given item index and completes
    /// when it has stopped. Keeps the ViewModel free of UI/animation types.
    /// </summary>
    public Func<int, Task>? ReelPlayer { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasResult), nameof(ResultTitle), nameof(ResultSubtitle), nameof(ResultRarity))]
    private DropResult? lastResult;

    public bool HasResult => LastResult is not null;
    public Rarity ResultRarity => LastResult?.Item.Rarity ?? Rarity.MilSpec;
    public string ResultTitle => LastResult is null
        ? ""
        : (LastResult.StatTrak ? "StatTrak™ " : "") + LastResult.Item.FullName;
    public string ResultSubtitle => LastResult is null
        ? ""
        : $"{LastResult.Item.Rarity.DisplayName()} · {LastResult.Wear} · {LastResult.Float:0.0000}";

    [RelayCommand(CanExecute = nameof(CanOpen))]
    private async Task OpenCaseAsync()
    {
        if (!_wallet.TrySpend(Model.Price)) return;

        LastResult = null; // hides the previous result
        IsOpening = true;

        // The outcome is decided up-front; the reel is just the presentation of it.
        var result = _drops.Roll(Model);
        var (reel, winnerIndex) = _drops.BuildReel(Model, result.Item);
        ReelItems = reel;
        IsSpinning = true;

        try
        {
            if (ReelPlayer is not null) await ReelPlayer(winnerIndex);
            else await Task.Delay(1500);
        }
        finally
        {
            IsSpinning = false;
            _inventory.Add(result, Model.Name); // the coins are spent, so the item is always kept
            LastResult = result;
            IsOpening = false;
        }
    }

    [RelayCommand]
    private void DismissResult() => LastResult = null;

    // Can't leave mid-spin: the coins are already spent and the drop would be lost.
    [RelayCommand(CanExecute = nameof(CanClose))]
    private void Close() => _close();

    private bool CanClose() => !IsOpening;

    private void OnWalletChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(Balance));
        OnPropertyChanged(nameof(CanAfford));
        OnPropertyChanged(nameof(CanOpen));
        OnPropertyChanged(nameof(Hint));
        OpenCaseCommand.NotifyCanExecuteChanged();
    }

    public void Dispose() => _wallet.PropertyChanged -= OnWalletChanged;
}
