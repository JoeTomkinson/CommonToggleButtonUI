using System.Windows.Media;

namespace ToggleIndicatorUI.Models
{
    public class ThemePreset
    {
        public ThemePreset(string name, string accent, string text, string background)
        {
            Name = name;
            AccentBrush = (Brush)new BrushConverter().ConvertFromString(accent)!;
            TextBrush = (Brush)new BrushConverter().ConvertFromString(text)!;
            BackgroundBrush = (Brush)new BrushConverter().ConvertFromString(background)!;
        }

        public string Name { get; }

        public Brush AccentBrush { get; }

        public Brush TextBrush { get; }

        public Brush BackgroundBrush { get; }
    }
}
