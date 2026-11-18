using CaseOpener.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CaseOpener.ViewModels;

public sealed partial class ThemeOptionViewModel : ObservableObject
{
    public ThemeOptionViewModel(ThemeMode mode, string title, string description)
    {
        Mode = mode;
        Title = title;
        Description = description;
    }

    public ThemeMode Mode { get; }
    public string Title { get; }
    public string Description { get; }

    [ObservableProperty]
    private bool isSelected;
}
