using System.Globalization;
using CaseOpener.Models;

namespace CaseOpener.Converters;

internal static class RarityPalette
{
    public static Color For(Rarity r) => r switch
    {
        Rarity.MilSpec => Color.FromArgb("#4B69FF"),
        Rarity.Restricted => Color.FromArgb("#8847FF"),
        Rarity.Classified => Color.FromArgb("#D32CE6"),
        Rarity.Covert => Color.FromArgb("#EB4B4B"),
        Rarity.RareSpecial => Color.FromArgb("#FFD700"),
        _ => Colors.White
    };
}

/// <summary>Rarity → Color (BoxView.Color, Label.TextColor...).</summary>
public sealed class RarityToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is Rarity r ? RarityPalette.For(r) : Colors.White;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}

/// <summary>Rarity → Brush (Border.Stroke, Background...).</summary>
public sealed class RarityToBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        new SolidColorBrush(value is Rarity r ? RarityPalette.For(r) : Colors.White);

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}

/// <summary>Rarity → soft glow gradient used behind images.</summary>
public sealed class RarityToGlowConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var c = value is Rarity r ? RarityPalette.For(r) : Colors.White;
        return new LinearGradientBrush(
            new GradientStopCollection
            {
                new GradientStop(c.WithAlpha(0.05f), 0f),
                new GradientStop(c.WithAlpha(0.32f), 1f)
            },
            new Point(0.5, 0), new Point(0.5, 1));
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}

public sealed class InverseBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is bool b && !b;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is bool b && !b;
}

/// <summary>string? → bool (true when non-empty). Used to show the hint line.</summary>
public sealed class StringToBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        !string.IsNullOrEmpty(value as string);

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
