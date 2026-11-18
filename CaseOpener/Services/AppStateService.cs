using CaseOpener.Models;

namespace CaseOpener.Services;

/// <summary>
/// UI/session state that should survive minimising or killing the app: current tab, search text and
/// filters. Held in memory while the app runs (singleton) and written to Preferences when the
/// window is stopped / deactivated / destroyed.
/// </summary>
public interface IAppStateService
{
    /// <summary>"store", "inventory" or "settings".</summary>
    string LastRoute { get; set; }

    string StoreSearch { get; set; }
    CaseKind? StoreFilter { get; set; }

    string InventorySearch { get; set; }
    Rarity? InventoryFilter { get; set; }
    bool InventorySortByRarity { get; set; }

    void Save();
}

public sealed class AppStateService : IAppStateService
{
    private const string Prefix = "state.";

    public AppStateService()
    {
        LastRoute = Preferences.Get(Prefix + "route", "store");
        StoreSearch = Preferences.Get(Prefix + "store.search", string.Empty);
        StoreFilter = ReadEnum<CaseKind>(Prefix + "store.filter");
        InventorySearch = Preferences.Get(Prefix + "inv.search", string.Empty);
        InventoryFilter = ReadEnum<Rarity>(Prefix + "inv.filter");
        InventorySortByRarity = Preferences.Get(Prefix + "inv.sort", false);
    }

    public string LastRoute { get; set; }
    public string StoreSearch { get; set; }
    public CaseKind? StoreFilter { get; set; }
    public string InventorySearch { get; set; }
    public Rarity? InventoryFilter { get; set; }
    public bool InventorySortByRarity { get; set; }

    public void Save()
    {
        Preferences.Set(Prefix + "route", LastRoute);
        Preferences.Set(Prefix + "store.search", StoreSearch ?? string.Empty);
        Preferences.Set(Prefix + "store.filter", StoreFilter?.ToString() ?? string.Empty);
        Preferences.Set(Prefix + "inv.search", InventorySearch ?? string.Empty);
        Preferences.Set(Prefix + "inv.filter", InventoryFilter?.ToString() ?? string.Empty);
        Preferences.Set(Prefix + "inv.sort", InventorySortByRarity);
    }

    private static T? ReadEnum<T>(string key) where T : struct, Enum =>
        Enum.TryParse<T>(Preferences.Get(key, string.Empty), out var value) ? value : null;
}
