using System;
using System.Media;
using System.Windows;
using ToggleNotifier.Configuration;
using ToggleNotifier.Notifications;
using ToggleNotifier.Services;
using ToggleNotifier.Theming;

namespace ToggleNotifier;

public partial class App : System.Windows.Application
{
    private KeyToggleListener? _keyListener;
    private OverlayService? _overlayService;
    private TrayService? _trayService;
    private ConfigService? _configService;
    private StartupManager? _startupManager;
    private ThemeService? _themeService;
    private SnoozeService? _snoozeService;
    private AppSettings? _settings;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _configService = new ConfigService();
        _settings = _configService.Load();

        // Initialize theme service
        _themeService = new ThemeService
        {
            CurrentTheme = _settings.Theme
        };

        // Initialize snooze service
        _snoozeService = new SnoozeService();

        _startupManager = new StartupManager("CommonToggleButtonUI", AppContext.BaseDirectory);
        if (_settings.LaunchOnSignIn)
        {
            _startupManager.EnableStartup();
        }
        else
        {
            _startupManager.DisableStartup();
        }

        _overlayService = new OverlayService(_settings, _themeService, _snoozeService);
        _overlayService.SettingsUpdated += OnSettingsUpdated;

        var mainWindow = new MainWindow(_settings, _configService, _startupManager, _overlayService, _themeService)
        {
            Visibility = Visibility.Hidden
        };
        MainWindow = mainWindow;

        _trayService = new TrayService(mainWindow, _themeService, _snoozeService);
        _trayService.ExitRequested += OnExitRequested;
        _trayService.SettingsRequested += OnSettingsRequested;

        _keyListener = new KeyToggleListener();
        _keyListener.KeyStateChanged += OnKeyStateChanged;
        _keyListener.Start();
    }

    private void OnSettingsUpdated(object? sender, AppSettings settings)
    {
        _settings = settings;
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
        if (_overlayService == null || _settings == null)
        {
            return;
        }

        // Check if this key is enabled for notifications
        var shouldNotify = e.KeyName switch
        {
            "Caps Lock" => _settings.KeySettings.CapsLock,
            "Num Lock" => _settings.KeySettings.NumLock,
            "Scroll Lock" => _settings.KeySettings.ScrollLock,
            _ => true
        };

        if (!shouldNotify)
        {
            return;
        }

        Dispatcher.Invoke(() =>
        {
            _overlayService.ShowToast(e);

            if (_settings.PlaySound)
            {
                SystemSounds.Asterisk.Play();
            }
        });
    }
}
