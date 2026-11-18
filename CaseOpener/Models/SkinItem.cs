namespace CaseOpener.Models;

/// <param name="Weapon">e.g. "AK-47" or "★ Karambit".</param>
/// <param name="Finish">e.g. "Redline" or "Fade".</param>
/// <param name="ImageFile">Optional file in Resources/Images. A placeholder glyph is shown when null.</param>
public sealed record SkinItem(string Weapon, string Finish, Rarity Rarity, string? ImageFile = null)
{
    public string FullName => $"{Weapon} | {Finish}";
}
