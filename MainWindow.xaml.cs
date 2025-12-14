using System;
using System.Windows;
using System.Windows.Threading;
using CommonToggleButtonUI.ViewModels;

namespace CommonToggleButtonUI;

public partial class MainWindow : Window
{
    private readonly DispatcherTimer _toastTimer;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = new SettingsViewModel();

        _toastTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(3)
        };
        _toastTimer.Tick += (_, _) => HideToast();
    }

    private void OnShowToast(object sender, RoutedEventArgs e)
    {
        ToastOverlay.Visibility = Visibility.Visible;
        _toastTimer.Stop();
        _toastTimer.Start();
    }

    private void HideToast()
    {
        _toastTimer.Stop();
        ToastOverlay.Visibility = Visibility.Collapsed;
    }
}
