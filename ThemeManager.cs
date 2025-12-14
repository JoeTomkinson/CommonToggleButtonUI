using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace CommonToggleButtonUI;

public static class ThemeManager
{
    private const string LightThemeSource = "Resources/Theme/Light.xaml";
    private const string DarkThemeSource = "Resources/Theme/Dark.xaml";
    private const string AccentDictionarySource = "Resources/Accents.xaml";

    private static readonly Dictionary<string, Uri> ThemeUris = new(StringComparer.OrdinalIgnoreCase)
    {
        { "Light", new Uri(LightThemeSource, UriKind.Relative) },
        { "Dark", new Uri(DarkThemeSource, UriKind.Relative) }
    };

    public static IReadOnlyList<string> ThemeNames => ThemeUris.Keys.ToList();

    public static IReadOnlyList<string> AccentNames { get; private set; } = Array.Empty<string>();

    public static void Initialize()
    {
        var accentDictionary = LoadDictionary(AccentDictionarySource);
        AccentNames = accentDictionary.Keys
            .OfType<string>()
            .Where(key => key.StartsWith("AccentColor.", StringComparison.Ordinal))
            .Select(key => key.Replace("AccentColor.", string.Empty))
            .OrderBy(key => key)
            .ToList();

        ApplyAccent("Blue");
    }

    public static void ApplyTheme(string themeName)
    {
        if (!ThemeUris.TryGetValue(themeName, out var source))
        {
            return;
        }

        var app = Application.Current;
        var dictionaries = app.Resources.MergedDictionaries;

        var existingTheme = dictionaries.FirstOrDefault(dict => dict.Source != null &&
            (string.Equals(dict.Source.OriginalString, LightThemeSource, StringComparison.OrdinalIgnoreCase) ||
             string.Equals(dict.Source.OriginalString, DarkThemeSource, StringComparison.OrdinalIgnoreCase)));

        var newTheme = LoadDictionary(source);

        if (existingTheme != null)
        {
            var index = dictionaries.IndexOf(existingTheme);
            dictionaries.RemoveAt(index);
            dictionaries.Insert(index, newTheme);
        }
        else
        {
            dictionaries.Insert(0, newTheme);
        }
    }

    public static void ApplyAccent(string accentName)
    {
        var accentKey = $"AccentColor.{accentName}";
        var accentBrushKey = $"AccentBrush.{accentName}";

        var accentDictionary = LoadDictionary(AccentDictionarySource);

        if (!accentDictionary.Contains(accentKey) || !accentDictionary.Contains(accentBrushKey))
        {
            return;
        }

        if (accentDictionary[accentKey] is Color accentColor)
        {
            Application.Current.Resources["AccentColor"] = accentColor;
        }

        if (accentDictionary[accentBrushKey] is SolidColorBrush accentBrush)
        {
            Application.Current.Resources["AccentBrush"] = accentBrush;
        }
    }

    private static ResourceDictionary LoadDictionary(string path) => new() { Source = new Uri(path, UriKind.Relative) };

    private static ResourceDictionary LoadDictionary(Uri path) => new() { Source = path };
}
