using System;
using System.Windows;
using ToggleNotifier.Configuration;
using ToggleNotifier.Notifications;
using ToggleNotifier.Services;

namespace ToggleNotifier;

public partial class App : Application
{
    private KeyToggleListener? _keyListener;
    private OverlayService? _overlayService;
    private TrayService? _trayService;
    private ConfigService? _configService;
    private StartupManager? _startupManager;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _configService = new ConfigService();
        var settings = _configService.Load();

        _startupManager = new StartupManager("CommonToggleButtonUI", AppContext.BaseDirectory);
        if (settings.LaunchOnSignIn)
        {
            _startupManager.EnableStartup();
        }
        else
        {
            _startupManager.DisableStartup();
        }

        _overlayService = new OverlayService(settings);

        var mainWindow = new MainWindow(settings, _configService, _startupManager, _overlayService)
        {
            Visibility = Visibility.Hidden
        };
        MainWindow = mainWindow;

        _trayService = new TrayService(mainWindow);
        _trayService.ExitRequested += OnExitRequested;
        _trayService.SettingsRequested += OnSettingsRequested;

        _keyListener = new KeyToggleListener();
        _keyListener.KeyStateChanged += OnKeyStateChanged;
        _keyListener.Start();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _keyListener?.Dispose();
        _trayService?.Dispose();
        base.OnExit(e);
    }

    private void OnSettingsRequested(object? sender, EventArgs e)
    {
        if (MainWindow == null)
        {
            return;
        }

        if (MainWindow.Visibility != Visibility.Visible)
        {
            MainWindow.Show();
        }

        MainWindow.Activate();
        MainWindow.Focus();
    }

    private void OnExitRequested(object? sender, EventArgs e)
    {
        Shutdown();
    }

    private void OnKeyStateChanged(object? sender, KeyStateChangedEventArgs e)
    {
        if (_overlayService == null)
        {
            return;
        }

        Dispatcher.Invoke(() => _overlayService.ShowToast(e));
    }
}
