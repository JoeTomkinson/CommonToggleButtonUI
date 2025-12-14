using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using ToggleIndicatorUI.Models;
using ToggleIndicatorUI.Services;

namespace ToggleIndicatorUI.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly SettingsService _settingsService;
        private readonly ObservableCollection<ThemePreset> _themes = new();

        public MainViewModel(SettingsService settingsService)
        {
            _settingsService = settingsService;
            Settings = _settingsService.Load();
            Settings.PropertyChanged += SettingsOnPropertyChanged;

            BuildThemes();
            RefreshIndicators();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public IndicatorSettings Settings { get; }

        public ObservableCollection<IndicatorDisplayItem> VisibleIndicators { get; } = new();

        public ObservableCollection<string> ThemeNames { get; } = new();

        public ThemePreset CurrentTheme => _themes.FirstOrDefault(t => t.Name == Settings.SelectedTheme) ?? _themes.First();

        public HorizontalAlignment IndicatorHorizontalAlignment => Settings.Corner switch
        {
            ScreenCorner.TopLeft or ScreenCorner.BottomLeft => HorizontalAlignment.Left,
            _ => HorizontalAlignment.Right
        };

        public VerticalAlignment IndicatorVerticalAlignment => Settings.Corner switch
        {
            ScreenCorner.TopLeft or ScreenCorner.TopRight => VerticalAlignment.Top,
            _ => VerticalAlignment.Bottom
        };

        public Thickness IndicatorMargin => Settings.Corner switch
        {
            ScreenCorner.TopLeft => new Thickness(Settings.OffsetX, Settings.OffsetY, 0, 0),
            ScreenCorner.TopRight => new Thickness(0, Settings.OffsetY, Settings.OffsetX, 0),
            ScreenCorner.BottomLeft => new Thickness(Settings.OffsetX, 0, 0, Settings.OffsetY),
            _ => new Thickness(0, 0, Settings.OffsetX, Settings.OffsetY)
        };

        private void SettingsOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            RefreshIndicators();
            _settingsService.Save(Settings);
        }

        private void BuildThemes()
        {
            _themes.Clear();
            _themes.Add(new ThemePreset("Fluent", "#5B8DEF", "#0F172A", "#F7FAFF"));
            _themes.Add(new ThemePreset("Dark", "#68D391", "#F8FAFC", "#1E293B"));
            _themes.Add(new ThemePreset("Amber", "#F59E0B", "#1F2937", "#FFF7ED"));

            ThemeNames.Clear();
            foreach (var theme in _themes)
            {
                ThemeNames.Add(theme.Name);
            }
        }

        private void RefreshIndicators()
        {
            VisibleIndicators.Clear();
            AddIndicator("Caps Lock", Settings.CapsLockEnabled);
            AddIndicator("Num Lock", Settings.NumLockEnabled);
            AddIndicator("Scroll Lock", Settings.ScrollLockEnabled);

            OnPropertyChanged(nameof(CurrentTheme));
            OnPropertyChanged(nameof(IndicatorHorizontalAlignment));
            OnPropertyChanged(nameof(IndicatorVerticalAlignment));
            OnPropertyChanged(nameof(IndicatorMargin));
        }

        private void AddIndicator(string name, bool enabled)
        {
            if (!enabled)
            {
                return;
            }

            VisibleIndicators.Add(new IndicatorDisplayItem(name, Settings.IndicatorSize, Settings.IndicatorOpacity, CurrentTheme));
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
