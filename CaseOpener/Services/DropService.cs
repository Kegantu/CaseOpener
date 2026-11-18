using CaseOpener.Models;

namespace CaseOpener.Services;

public interface IDropService
{
    DropResult Roll(CaseModel source);

    /// <summary>
    /// Builds the roulette strip: rarity-weighted filler items with <paramref name="winner"/>
    /// placed near the end. Returns the strip and the winner's index.
    /// </summary>
    (IReadOnlyList<SkinItem> Items, int WinnerIndex) BuildReel(CaseModel source, SkinItem winner, int length = 60);
}

public sealed class DropService : IDropService
{
    public DropResult Roll(CaseModel source)
    {
        var rng = Random.Shared;
        var item = PickItem(BuildPools(source), rng);
        var floatValue = Math.Round(rng.NextDouble(), 8);
        var statTrak = rng.NextDouble() < 0.10;
        return new DropResult(item, floatValue, WearFor(floatValue), statTrak);
    }

    public (IReadOnlyList<SkinItem> Items, int WinnerIndex) BuildReel(CaseModel source, SkinItem winner, int length = 60)
    {
        var rng = Random.Shared;
        var pools = BuildPools(source);

        // Land late in the strip (like CS2), with a few items after it so the right side isn't empty.
        var winnerIndex = rng.Next(length - 15, length - 7);

        var items = new SkinItem[length];
        for (var i = 0; i < length; i++)
        {
            items[i] = i == winnerIndex ? winner : PickItem(pools, rng);
        }

        return (items, winnerIndex);
    }

    // Only rarities that exist in this case take part; weights are renormalised.
    private List<(Rarity Rarity, List<SkinItem> Items, double Weight)> BuildPools(CaseModel source) =>
        source.Items
            .GroupBy(i => i.Rarity)
            .Select(g => (Rarity: g.Key, Items: g.ToList(), Weight: g.Key.DropChance()))
            .ToList();

    private SkinItem PickItem(List<(Rarity Rarity, List<SkinItem> Items, double Weight)> pools, Random rng)
    {
        var roll = rng.NextDouble() * pools.Sum(p => p.Weight);
        var chosen = pools[^1];
        foreach (var pool in pools)
        {
            if (roll < pool.Weight) { chosen = pool; break; }
            roll -= pool.Weight;
        }
        return chosen.Items[rng.Next(chosen.Items.Count)];
    }

    private string WearFor(double f) => f switch
    {
        < 0.07 => "Factory New",
        < 0.15 => "Minimal Wear",
        < 0.38 => "Field-Tested",
        < 0.45 => "Well-Worn",
        _ => "Battle-Scarred"
    };
}
