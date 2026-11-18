namespace CaseOpener.Services;

/// <summary>Lets ViewModels ask the user something without referencing any page.</summary>
public interface IDialogService
{
    Task<bool> ConfirmAsync(string title, string message, string accept, string cancel);
}

public sealed class DialogService : IDialogService
{
    public async Task<bool> ConfirmAsync(string title, string message, string accept, string cancel)
    {
        var page = Shell.Current;
        if (page is null) return true; // no UI available: don't block the action

#pragma warning disable CS0618 // DisplayAlert is obsolete on .NET 10 in favour of DisplayAlertAsync; this builds on both
        return await page.DisplayAlert(title, message, accept, cancel);
#pragma warning restore CS0618
    }
}
