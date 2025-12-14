using System;
using ToggleNotifier.Configuration;
using ToggleNotifier.Services;
using ToggleNotifier.Theming;

namespace ToggleNotifier.Notifications;

public class OverlayService
{
    private AppSettings _settings;
    private readonly ThemeService _themeService;

    public event EventHandler<AppSettings>? SettingsUpdated;

    public AppSettings CurrentSettings => _settings;

    public OverlayService(AppSettings settings, ThemeService themeService)
    {
        _settings = settings;
        _themeService = themeService;
    }

    public void UpdateSettings(AppSettings settings)
    {
        _settings = settings;
        _themeService.CurrentTheme = settings.Theme;
        SettingsUpdated?.Invoke(this, settings);
    }

    public void ShowToast(KeyStateChangedEventArgs args)
    {
        var toast = new ToastWindow(args.KeyName, args.IsOn, _settings, _themeService)
        {
            Topmost = true
        };

        toast.Show();
    }
}
