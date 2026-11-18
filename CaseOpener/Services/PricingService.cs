using CaseOpener.Models;

namespace CaseOpener.Services;

public interface IPricingService
{
    /// <summary>Sell value in coins for a dropped skin.</summary>
    int GetValue(SkinItem item, string wear, bool statTrak);
}

/// <summary>
/// value = base(rarity) × skin factor × wear multiplier × StatTrak multiplier.
/// Tune the tables below to rebalance the economy. Cases cost 60–250 coins and the average
/// drop sells for roughly 70, so opening cases is a gamble that mostly loses a little.
/// </summary>
public sealed class PricingService : IPricingService
{
    private static double BaseValue(Rarity rarity) => rarity switch
    {
        Rarity.MilSpec => 25,
        Rarity.Restricted => 90,
        Rarity.Classified => 380,
        Rarity.Covert => 1_500,
        Rarity.RareSpecial => 9_000,
        _ => 10
    };

    private static double WearMultiplier(string wear) => wear switch
    {
        "Factory New" => 1.60,
        "Minimal Wear" => 1.25,
        "Field-Tested" => 1.00,
        "Well-Worn" => 0.80,
        "Battle-Scarred" => 0.65,
        _ => 1.00
    };

    private const double StatTrakMultiplier = 1.5;

    public int GetValue(SkinItem item, string wear, bool statTrak)
    {
        var value = BaseValue(item.Rarity)
                    * SkinFactor(item)
                    * WearMultiplier(wear)
                    * (statTrak ? StatTrakMultiplier : 1.0);

        return Math.Max(1, (int)Math.Round(value));
    }

    /// <summary>
    /// Stable 0.75–1.35 factor per skin, so two skins of the same rarity don't cost exactly the same.
    /// FNV-1a rather than string.GetHashCode(), which changes between runs.
    /// </summary>
    private static double SkinFactor(SkinItem item)
    {
        uint hash = 2166136261;
        foreach (var ch in item.FullName)
        {
            hash ^= ch;
            hash *= 16777619;
        }
        return 0.75 + (hash % 1000) / 1000.0 * 0.60;
    }
}
