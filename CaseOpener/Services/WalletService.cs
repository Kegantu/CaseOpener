using CommunityToolkit.Mvvm.ComponentModel;

namespace CaseOpener.Services;

public interface IWalletService : System.ComponentModel.INotifyPropertyChanged
{
    int Balance { get; }
    bool TrySpend(int amount);
    void Add(int amount);
}

/// <summary>In-memory wallet. Swap for a persisted implementation (Preferences / SQLite) later.</summary>
public sealed partial class WalletService : ObservableObject, IWalletService
{
    [ObservableProperty]
    private int balance = 5000;

    public bool TrySpend(int amount)
    {
        if (amount < 0 || amount > Balance) return false;
        Balance -= amount;
        return true;
    }

    public void Add(int amount) => Balance += amount;
}
