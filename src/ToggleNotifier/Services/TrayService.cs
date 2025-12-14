using System;
using ToggleNotifier.Assets;
using ToggleNotifier.Theming;
using System.Windows.Forms;

namespace ToggleNotifier.Services;

public class TrayService : IDisposable
{
    private readonly NotifyIcon _notifyIcon;
    private readonly ThemeService _themeService;

    public event EventHandler? ExitRequested;
    public event EventHandler? SettingsRequested;

    public TrayService(System.Windows.Window window, ThemeService themeService)
    {
        _themeService = themeService;

        var contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add("Open settings", null, (_, _) => SettingsRequested?.Invoke(this, EventArgs.Empty));
        contextMenu.Items.Add(new ToolStripSeparator());
        contextMenu.Items.Add("Exit", null, (_, _) => ExitRequested?.Invoke(this, EventArgs.Empty));

        _notifyIcon = new NotifyIcon
        {
            Icon = IconGenerator.CreateTrayIcon(_themeService.IsDarkMode),
            Visible = true,
            Text = "Toggle Notifier",
            ContextMenuStrip = contextMenu
        };

        _notifyIcon.DoubleClick += (_, _) => SettingsRequested?.Invoke(this, EventArgs.Empty);

        // Update icon when theme changes
        _themeService.ThemeChanged += OnThemeChanged;
    }

    private void OnThemeChanged(object? sender, bool isDarkMode)
    {
        _notifyIcon.Icon?.Dispose();
        _notifyIcon.Icon = IconGenerator.CreateTrayIcon(isDarkMode);
    }

    public void Dispose()
    {
        _themeService.ThemeChanged -= OnThemeChanged;
        _notifyIcon.Icon?.Dispose();
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
    }
}
