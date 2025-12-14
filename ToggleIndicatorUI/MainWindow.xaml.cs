using System.Windows;
using ToggleIndicatorUI.ViewModels;

namespace ToggleIndicatorUI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                var settingsWindow = new SettingsWindow(vm.Settings, vm.ThemeNames)
                {
                    Owner = this
                };
                settingsWindow.ShowDialog();
            }
        }
    }
}
