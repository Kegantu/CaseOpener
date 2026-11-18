using CaseOpener.Models;

namespace CaseOpener.Services;

public interface ICaseCatalog
{
    IReadOnlyList<CaseModel> GetCases();
}

/// <summary>
/// Sample data so the store renders out of the box. Replace with a JSON file / API
/// (keep the ICaseCatalog contract) and verify the item lists against the real cases.
/// </summary>
public sealed class CaseCatalog : ICaseCatalog
{
    private static readonly SkinItem[] Knives =
    {
        new("★ Karambit", "Fade", Rarity.RareSpecial),
        new("★ Butterfly Knife", "Doppler", Rarity.RareSpecial),
        new("★ M9 Bayonet", "Marble Fade", Rarity.RareSpecial),
        new("★ Sport Gloves", "Pandora's Box", Rarity.RareSpecial),
    };

    public IReadOnlyList<CaseModel> GetCases() => _cases ??= Build();
    private IReadOnlyList<CaseModel>? _cases;

    private static IReadOnlyList<CaseModel> Build() => new List<CaseModel>
    {
        new()
        {
            Id = "revolution", Name = "Revolution Case", Kind = CaseKind.Weapon, Price = 250,
            Items = With(
                Knives, new SkinItem("MAC-10", "Sakkaku", Rarity.MilSpec), new SkinItem("P90", "Neoqueen", Rarity.MilSpec),
                new SkinItem("SG 553", "Cyberforce", Rarity.MilSpec), new SkinItem("MP9", "Featherweight", Rarity.MilSpec),
                new SkinItem("P250", "Re.built", Rarity.Restricted), new SkinItem("Glock-18", "Umbral Rabbit", Rarity.Restricted),
                new SkinItem("R8 Revolver", "Banana Cannon", Rarity.Restricted),
                new SkinItem("AWP", "Duality", Rarity.Classified), new SkinItem("MP7", "Bloodsport", Rarity.Classified),
                new SkinItem("AK-47", "Head Shot", Rarity.Covert), new SkinItem("M4A4", "Temukau", Rarity.Covert))
        },
        new()
        {
            Id = "dreams", Name = "Dreams & Nightmares Case", Kind = CaseKind.Weapon, Price = 180,
            Items = With(
                Knives, new SkinItem("MAC-10", "Ensnared", Rarity.MilSpec), new SkinItem("MP9", "Starlight Protector", Rarity.MilSpec),
                new SkinItem("XM1014", "Zombie Offensive", Rarity.MilSpec), new SkinItem("FAMAS", "Rapid Eye Movement", Rarity.MilSpec),
                new SkinItem("SSG 08", "Dezastre", Rarity.Restricted), new SkinItem("Five-SeveN", "Scrawl", Rarity.Restricted),
                new SkinItem("PP-Bizon", "Space Cat", Rarity.Restricted),
                new SkinItem("MP7", "Abyssal Apparition", Rarity.Classified), new SkinItem("Dual Berettas", "Melondrama", Rarity.Classified),
                new SkinItem("AK-47", "Nightwish", Rarity.Covert), new SkinItem("MP5-SD", "Necro Jr.", Rarity.Covert))
        },
        new()
        {
            Id = "recoil", Name = "Recoil Case", Kind = CaseKind.Weapon, Price = 120,
            Items = With(
                Knives, new SkinItem("Negev", "Drop Me", Rarity.MilSpec), new SkinItem("P2000", "Acid Etched", Rarity.MilSpec),
                new SkinItem("UMP-45", "Roadblock", Rarity.MilSpec), new SkinItem("Glock-18", "Winterized", Rarity.MilSpec),
                new SkinItem("Galil AR", "Destroyer", Rarity.Restricted), new SkinItem("M4A4", "Poly Mag", Rarity.Restricted),
                new SkinItem("AWP", "Chromatic Aberration", Rarity.Classified),
                new SkinItem("USP-S", "Printstream", Rarity.Covert), new SkinItem("AK-47", "Ice Coaled", Rarity.Covert))
        },
        new()
        {
            Id = "kilowatt", Name = "Kilowatt Case", Kind = CaseKind.Weapon, Price = 210,
            Items = With(
                Knives, new SkinItem("Tec-9", "Slag", Rarity.MilSpec), new SkinItem("MAC-10", "Light Box", Rarity.MilSpec),
                new SkinItem("Sawed-Off", "Analog Input", Rarity.MilSpec), new SkinItem("Dual Berettas", "Hideout", Rarity.MilSpec),
                new SkinItem("M4A4", "Etch Lord", Rarity.Restricted), new SkinItem("Zeus x27", "Olympus", Rarity.Restricted),
                new SkinItem("AK-47", "Inheritance", Rarity.Covert), new SkinItem("AWP", "Chrome Cannon", Rarity.Classified))
        },
        new()
        {
            Id = "dust2", Name = "Dust 2 Collection", Kind = CaseKind.Collection, Price = 90,
            Items = new List<SkinItem>
            {
                new("P250", "Sand Dune", Rarity.MilSpec), new("SCAR-20", "Sand Mesh", Rarity.MilSpec),
                new("Nova", "Predator", Rarity.MilSpec), new("MP9", "Sand Dashed", Rarity.MilSpec),
                new("SG 553", "Damascus Steel", Rarity.Restricted), new("AWP", "Pit Viper", Rarity.Restricted),
                new("AK-47", "Safari Mesh", Rarity.Classified)
            }
        },
        new()
        {
            Id = "souvenir-mirage", Name = "Mirage Souvenir Package", Kind = CaseKind.Souvenir, Price = 60,
            Items = new List<SkinItem>
            {
                new("Tec-9", "Blue Titanium", Rarity.MilSpec), new("MP7", "Anodized Navy", Rarity.MilSpec),
                new("P250", "Facets", Rarity.Restricted), new("AK-47", "Frontside Misty", Rarity.Classified),
                new("AWP", "Dragon Lore", Rarity.Covert)
            }
        }
    };
    
    private static IReadOnlyList<SkinItem> With(SkinItem[] knives, params SkinItem[] items) =>
        items.Concat(knives).ToList();
}
