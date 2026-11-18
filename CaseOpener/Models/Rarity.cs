namespace CaseOpener.Models;

/// <summary>CS2 item grades, lowest to highest.</summary>
public enum Rarity
{
    MilSpec,
    Restricted,
    Classified,
    Covert,
    RareSpecial
}

public static class RarityExtensions
{
    public static string DisplayName(this Rarity rarity) => rarity switch
    {
        Rarity.MilSpec => "Mil-Spec",
        Rarity.Restricted => "Restricted",
        Rarity.Classified => "Classified",
        Rarity.Covert => "Covert",
        Rarity.RareSpecial => "Rare Special Item",
        _ => rarity.ToString()
    };

    /// <summary>Official CS2 case drop chances (percent).</summary>
    public static double DropChance(this Rarity rarity) => rarity switch
    {
        Rarity.MilSpec => 79.92,
        Rarity.Restricted => 15.98,
        Rarity.Classified => 3.20,
        Rarity.Covert => 0.64,
        Rarity.RareSpecial => 0.26,
        _ => 0
    };
}
