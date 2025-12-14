using System;
using System.Windows;
using ToggleNotifier.Configuration;
using ToggleNotifier.Services;
using ToggleNotifier.Theming;

namespace ToggleNotifier.Notifications;

public class OverlayService
{
    private AppSettings _settings;
    private readonly ThemeService _themeService;
    private readonly SnoozeService _snoozeService;

    public event EventHandler<AppSettings>? SettingsUpdated;

    public AppSettings CurrentSettings => _settings;
    public SnoozeService SnoozeService => _snoozeService;

    public OverlayService(AppSettings settings, ThemeService themeService, SnoozeService snoozeService)
    {
        _settings = settings;
        _themeService = themeService;
        _snoozeService = snoozeService;
    }

    public void UpdateSettings(AppSettings settings)
    {
        _settings = settings;
        _themeService.CurrentTheme = settings.Theme;
        SettingsUpdated?.Invoke(this, settings);
    }

    public void ShowToast(KeyStateChangedEventArgs args)
    {
        // Check if notifications are snoozed
        if (_snoozeService.IsSnoozed)
        {
            return;
        }

        // Check if we should suppress notifications in fullscreen mode
        if (_settings.SuppressInFullscreen && FullscreenDetector.IsFullscreenAppRunning())
        {
            return;
        }

        Window toast = _settings.ToastStyle switch
        {
            ToastStyle.Compact => new CompactToastWindow(args.KeyName, args.IsOn, _settings, _themeService)
            {
                Topmost = true
            },
            _ => new ToastWindow(args.KeyName, args.IsOn, _settings, _themeService)
            {
                Topmost = true
            }
        };

        toast.Show();
    }
}
