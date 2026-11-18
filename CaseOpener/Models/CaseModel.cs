namespace CaseOpener.Models;

public enum CaseKind
{
    Weapon,
    Souvenir,
    Collection
}

public sealed class CaseModel
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required CaseKind Kind { get; init; }

    /// <summary>Price in in-game currency.</summary>
    public required int Price { get; init; }

    public string? ImageFile { get; init; }
    public required IReadOnlyList<SkinItem> Items { get; init; }

    public Rarity TopRarity => Items.Count == 0 ? Rarity.MilSpec : Items.Max(i => i.Rarity);
}

/// <summary>Result of a single case opening.</summary>
public sealed record DropResult(SkinItem Item, double Float, string Wear, bool StatTrak);
