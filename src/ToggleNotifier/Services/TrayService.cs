using System;
using System.Drawing;
using System.Windows.Forms;

namespace ToggleNotifier.Services;

public class TrayService : IDisposable
{
    private readonly NotifyIcon _notifyIcon;

    public event EventHandler? ExitRequested;
    public event EventHandler? SettingsRequested;

    public TrayService(System.Windows.Window window)
    {
        var contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add("Open settings", null, (_, _) => SettingsRequested?.Invoke(this, EventArgs.Empty));
        contextMenu.Items.Add(new ToolStripSeparator());
        contextMenu.Items.Add("Exit", null, (_, _) => ExitRequested?.Invoke(this, EventArgs.Empty));

        _notifyIcon = new NotifyIcon
        {
            Icon = SystemIcons.Information,
            Visible = true,
            Text = "Common Toggle Button UI",
            ContextMenuStrip = contextMenu
        };

        _notifyIcon.DoubleClick += (_, _) => SettingsRequested?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
    }
}
