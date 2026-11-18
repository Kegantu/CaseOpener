using System.Collections.ObjectModel;
using CaseOpener.Models;

namespace CaseOpener.Services;

public interface IInventoryService
{
    /// <summary>Newest first. Observable so the inventory page updates live.</summary>
    ObservableCollection<InventoryItem> Items { get; }

    void Add(DropResult drop, string caseName);

    /// <summary>Removes an item. Returns false if it was already gone (guards against double-selling).</summary>
    bool Remove(Guid id);
}

/// <summary>In-memory inventory shared by the store and inventory pages (singleton).</summary>
public sealed class InventoryService : IInventoryService
{
    private readonly IPricingService _pricing;

    public InventoryService(IPricingService pricing) => _pricing = pricing;

    public ObservableCollection<InventoryItem> Items { get; } = new();

    public void Add(DropResult drop, string caseName)
    {
        var value = _pricing.GetValue(drop.Item, drop.Wear, drop.StatTrak);
        Items.Insert(0, InventoryItem.From(drop, caseName, value));
    }

    public bool Remove(Guid id)
    {
        var item = Items.FirstOrDefault(i => i.Id == id);
        return item is not null && Items.Remove(item);
    }
}
