using System;
using ToggleNotifier.Assets;
using ToggleNotifier.Theming;
using System.Windows.Forms;

namespace ToggleNotifier.Services;

public class TrayService : IDisposable
{
    private readonly NotifyIcon _notifyIcon;
    private readonly ThemeService _themeService;
    private readonly SnoozeService _snoozeService;
    private readonly ToolStripMenuItem _snoozeMenuItem;
    private readonly ToolStripMenuItem _resumeMenuItem;

    public event EventHandler? ExitRequested;
    public event EventHandler? SettingsRequested;

    public TrayService(System.Windows.Window window, ThemeService themeService, SnoozeService snoozeService)
    {
        _themeService = themeService;
        _snoozeService = snoozeService;

        // Create snooze submenu
        _snoozeMenuItem = new ToolStripMenuItem("Snooze notifications");
        _snoozeMenuItem.DropDownItems.Add("30 minutes", null, (_, _) => _snoozeService.Snooze(30));
        _snoozeMenuItem.DropDownItems.Add("1 hour", null, (_, _) => _snoozeService.Snooze(60));
        _snoozeMenuItem.DropDownItems.Add("2 hours", null, (_, _) => _snoozeService.Snooze(120));

        // Create resume menu item (hidden by default)
        _resumeMenuItem = new ToolStripMenuItem("Resume notifications", null, (_, _) => _snoozeService.CancelSnooze())
        {
            Visible = false
        };

        var contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add("Open settings", null, (_, _) => SettingsRequested?.Invoke(this, EventArgs.Empty));
        contextMenu.Items.Add(new ToolStripSeparator());
        contextMenu.Items.Add(_snoozeMenuItem);
        contextMenu.Items.Add(_resumeMenuItem);
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

        // Update menu when snooze state changes
        _snoozeService.SnoozeStateChanged += OnSnoozeStateChanged;
    }

    private void OnThemeChanged(object? sender, bool isDarkMode)
    {
        _notifyIcon.Icon?.Dispose();
        _notifyIcon.Icon = IconGenerator.CreateTrayIcon(isDarkMode);
    }

    private void OnSnoozeStateChanged(object? sender, bool isSnoozed)
    {
        // Toggle visibility of snooze/resume menu items
        _snoozeMenuItem.Visible = !isSnoozed;
        _resumeMenuItem.Visible = isSnoozed;

        // Update tooltip to show snooze status
        if (isSnoozed)
        {
            var remaining = _snoozeService.RemainingSnoozeTime;
            var minutes = (int)Math.Ceiling(remaining.TotalMinutes);
            _notifyIcon.Text = $"Toggle Notifier (Snoozed - {minutes} min remaining)";
            _resumeMenuItem.Text = $"Resume notifications ({minutes} min left)";
        }
        else
        {
            _notifyIcon.Text = "Toggle Notifier";
        }
    }

    public void Dispose()
    {
        _themeService.ThemeChanged -= OnThemeChanged;
        _snoozeService.SnoozeStateChanged -= OnSnoozeStateChanged;
        _notifyIcon.Icon?.Dispose();
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
    }
}
