using CaseOpener.Models;

namespace CaseOpener.ViewModels;

/// <summary>Read-only presentation wrapper for one tile in the store grid.</summary>
public sealed class CaseCardViewModel
{
    public CaseCardViewModel(CaseModel model) => Model = model;

    public CaseModel Model { get; }

    public string Name => Model.Name;
    public int Price => Model.Price;
    public string PriceText => $"◆ {Model.Price:N0}";
    public Rarity TopRarity => Model.TopRarity;
    public string? ImageFile => Model.ImageFile;

    /// <summary>ZZZ-hub style meta line: "Weapon Case · 15 items · ★ Rare Special".</summary>
    public string MetaLine
    {
        get
        {
            var kind = Model.Kind switch
            {
                CaseKind.Weapon => "Weapon Case",
                CaseKind.Souvenir => "Souvenir",
                _ => "Collection"
            };
            var special = Model.Items.Any(i => i.Rarity == Rarity.RareSpecial) ? " · ★ Rare Special" : "";
            return $"{kind} · {Model.Items.Count} items{special}";
        }
    }
}
