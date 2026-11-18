namespace CaseOpener.Models;

/// <summary>One owned skin: the drop plus where and when it was obtained, and what it sells for.</summary>
public sealed record InventoryItem(
    Guid Id,
    SkinItem Item,
    double Float,
    string Wear,
    bool StatTrak,
    string CaseName,
    DateTime ObtainedAt,
    int Value)
{
    public Rarity Rarity => Item.Rarity;
    public string Subtitle => $"{Wear} · {Float:0.0000}";
    public string FromText => $"From {CaseName}";
    public string DisplayName => (StatTrak ? "StatTrak™ " : "") + Item.FullName;
    public string SellText => $"Sell   ◆ {Value:N0}";

    public static InventoryItem From(DropResult drop, string caseName, int value) =>
        new(Guid.NewGuid(), drop.Item, drop.Float, drop.Wear, drop.StatTrak, caseName, DateTime.Now, value);
}
