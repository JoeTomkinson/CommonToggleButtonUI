using ToggleIndicatorUI.Models;

namespace ToggleIndicatorUI.ViewModels
{
    public class IndicatorDisplayItem
    {
        public IndicatorDisplayItem(string name, double size, double opacity, ThemePreset theme)
        {
            Name = name;
            Size = size;
            Opacity = opacity;
            Theme = theme;
        }

        public string Name { get; }

        public double Size { get; }

        public double Opacity { get; }

        public ThemePreset Theme { get; }
    }
}
