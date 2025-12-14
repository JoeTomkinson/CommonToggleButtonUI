using System;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Forms;
using ToggleNotifier.Configuration;
using ToggleNotifier.Interop;

namespace ToggleNotifier.Notifications;

public partial class ToastWindow : Window
{
    private readonly DispatcherTimer _timer;
    private readonly AppSettings _settings;

    public ToastWindow(string keyName, bool isOn, AppSettings settings)
    {
        InitializeComponent();
        _settings = settings;

        TitleBlock.Text = keyName;
        StateBlock.Text = isOn ? "On" : "Off";
        Glyph.Text = keyName switch
        {
            "Caps Lock" => "\u21EA",
            "Num Lock" => "#",
            "Scroll Lock" => "\u21C4",
            _ => "\u21E7"
        };

        if (SystemParameters.HighContrast)
        {
            Container.Background = SystemColors.WindowBrush;
            Container.BorderBrush = SystemColors.WindowFrameBrush;
            StateBlock.Foreground = SystemColors.WindowTextBrush;
            TitleBlock.Foreground = SystemColors.WindowTextBrush;
            Glyph.Foreground = SystemColors.WindowTextBrush;
        }

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(Math.Max(500, settings.ToastDismissMilliseconds))
        };
        _timer.Tick += (_, _) => Close();
    }

    protected override void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);
        PositionWindow();
        _timer.Start();
    }

    protected override bool ShowWithoutActivation => true;

    protected override void OnClosed(EventArgs e)
    {
        _timer.Stop();
        base.OnClosed(e);
    }

    private void PositionWindow()
    {
        var handle = NativeMethods.GetForegroundWindow();
        var screen = handle != IntPtr.Zero
            ? Screen.FromHandle(handle)
            : Screen.FromPoint(System.Windows.Forms.Control.MousePosition);

        var workingArea = screen.WorkingArea;
        var source = PresentationSource.FromVisual(this);
        var dpi = source?.CompositionTarget?.TransformToDevice;
        var dpiX = dpi?.M11 ?? 1.0;
        var dpiY = dpi?.M22 ?? 1.0;

        var width = Width * (1 / dpiX);
        var height = Height * (1 / dpiY);

        Left = workingArea.Left + (workingArea.Width - width) - _settings.ToastOffsets.Horizontal;
        Top = workingArea.Top + (workingArea.Height - height) - _settings.ToastOffsets.Vertical;
    }
}
