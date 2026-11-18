using CaseOpener.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CaseOpener.ViewModels;

public sealed partial class FilterChipViewModel : ObservableObject
{
    public FilterChipViewModel(string label, CaseKind? kind)
    {
        Label = label;
        Kind = kind;
    }

    public string Label { get; }

    /// <summary>null = "All".</summary>
    public CaseKind? Kind { get; }

    [ObservableProperty]
    private bool isSelected;
}
