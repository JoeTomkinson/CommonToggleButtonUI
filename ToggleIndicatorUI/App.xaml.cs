using System.Windows;
using ToggleIndicatorUI.Services;
using ToggleIndicatorUI.ViewModels;

namespace ToggleIndicatorUI
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var settingsService = new SettingsService();
            var mainWindow = new MainWindow
            {
                DataContext = new MainViewModel(settingsService)
            };

            mainWindow.Show();
        }
    }
}
