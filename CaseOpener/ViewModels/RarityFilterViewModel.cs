using CaseOpener.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CaseOpener.ViewModels;

public sealed partial class RarityFilterViewModel : ObservableObject
{
    public RarityFilterViewModel(string label, Rarity? rarity)
    {
        Label = label;
        Rarity = rarity;
    }

    public string Label { get; }

    /// <summary>null = "All".</summary>
    public Rarity? Rarity { get; }

    [ObservableProperty]
    private bool isSelected;
}
