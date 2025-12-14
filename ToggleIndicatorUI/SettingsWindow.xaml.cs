using System;
using System.Collections.ObjectModel;
using System.Windows;
using ToggleIndicatorUI.Models;

namespace ToggleIndicatorUI
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(IndicatorSettings settings, ObservableCollection<string> themeNames)
        {
            InitializeComponent();
            Settings = settings;
            ThemeNames = themeNames;
            DataContext = this;
        }

        public IndicatorSettings Settings { get; }

        public ObservableCollection<string> ThemeNames { get; }

        public Array CornerOptions => Enum.GetValues(typeof(ScreenCorner));

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
