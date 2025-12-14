using System;
using System.Windows;
using System.Windows.Threading;
using ToggleNotifier.Configuration;
using ToggleNotifier.Interop;
using ToggleNotifier.Theming;
using WinFormsScreen = System.Windows.Forms.Screen;
using WinFormsControl = System.Windows.Forms.Control;
using Color = System.Windows.Media.Color;
using SolidColorBrush = System.Windows.Media.SolidColorBrush;

namespace ToggleNotifier.Notifications;

public partial class CompactToastWindow : Window
{
    private readonly DispatcherTimer _timer;
    private readonly AppSettings _settings;

    public CompactToastWindow(string keyName, bool isOn, AppSettings settings, ThemeService? themeService = null)
    {
        InitializeComponent();
        _settings = settings;

        Glyph.Text = keyName switch
        {
            "Caps Lock" => "\u21EA",
            "Num Lock" => "#",
            "Scroll Lock" => "\u21C5",
            _ => "\u21E7"
        };

        ApplyTheme(themeService, isOn);

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(Math.Max(500, settings.ToastDismissMilliseconds))
        };
        _timer.Tick += (_, _) => Close();

        // Position window before it becomes visible
        SourceInitialized += OnSourceInitialized;
    }

    private void OnSourceInitialized(object? sender, EventArgs e)
    {
        PositionWindow();
        Opacity = 1;
    }

    private void ApplyTheme(ThemeService? themeService, bool isOn)
    {
        var indicatorOnColor = Color.FromRgb(16, 185, 129);  // Emerald green
        var indicatorOffColor = Color.FromRgb(107, 114, 128); // Gray

        if (SystemParameters.HighContrast)
        {
            // High contrast mode - use system colors
            Container.Background = System.Windows.SystemColors.WindowBrush;
            Container.BorderBrush = System.Windows.SystemColors.WindowFrameBrush;
            Glyph.Foreground = System.Windows.SystemColors.WindowTextBrush;
            GlyphContainer.Background = System.Windows.SystemColors.ControlBrush;
            StateIndicator.Background = isOn
                ? new SolidColorBrush(indicatorOnColor)
                : new SolidColorBrush(indicatorOffColor);
            IndicatorBorderBrush.Color = System.Windows.SystemColors.WindowColor;
            return;
        }

        var isDark = themeService?.IsDarkMode ?? false;
        var colors = themeService?.GetColors() ?? GetDefaultColors(isDark);

        // Apply theme colors
        Container.Background = colors.BackgroundBrush;
        Container.BorderBrush = colors.BorderBrush;
        Glyph.Foreground = colors.AccentBrush;

        // Glyph container background
        GlyphContainer.Background = new SolidColorBrush(
            isDark ? Color.FromArgb(40, 255, 255, 255) : Color.FromArgb(40, 0, 0, 0));

        // State indicator - green for on, gray for off
        StateIndicator.Background = isOn
            ? new SolidColorBrush(indicatorOnColor)
            : new SolidColorBrush(indicatorOffColor);

        // Indicator border matches container background for clean cutout effect
        IndicatorBorderBrush.Color = isDark 
            ? Color.FromArgb(245, 39, 39, 42) 
            : Color.FromArgb(250, 255, 255, 255);
    }

    private static ThemeColors GetDefaultColors(bool isDark)
    {
        return isDark
            ? new ThemeColors
            {
                Background = Color.FromArgb(245, 39, 39, 42),
                Surface = Color.FromArgb(255, 44, 44, 44),
                Border = Color.FromArgb(255, 63, 63, 70),
                Text = Color.FromArgb(255, 250, 250, 250),
                TextSecondary = Color.FromArgb(255, 161, 161, 170),
                Accent = Color.FromArgb(255, 96, 165, 250)
            }
            : new ThemeColors
            {
                Background = Color.FromArgb(250, 255, 255, 255),
                Surface = Color.FromArgb(255, 255, 255, 255),
                Border = Color.FromArgb(255, 228, 228, 231),
                Text = Color.FromArgb(255, 24, 24, 27),
                TextSecondary = Color.FromArgb(255, 113, 113, 122),
                Accent = Color.FromArgb(255, 37, 99, 235)
            };
    }

    protected override void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);
        _timer.Start();
    }

    protected override void OnClosed(EventArgs e)
    {
        _timer.Stop();
        base.OnClosed(e);
    }

    private void PositionWindow()
    {
        var handle = NativeMethods.GetForegroundWindow();
        var screen = handle != IntPtr.Zero
            ? WinFormsScreen.FromHandle(handle)
            : WinFormsScreen.FromPoint(WinFormsControl.MousePosition);

        var workingArea = screen.WorkingArea;
        var source = PresentationSource.FromVisual(this);
        var dpi = source?.CompositionTarget?.TransformToDevice;
        var dpiX = dpi?.M11 ?? 1.0;
        var dpiY = dpi?.M22 ?? 1.0;

        // Convert screen pixels to WPF DIPs
        var workingAreaWidth = workingArea.Width / dpiX;
        var workingAreaHeight = workingArea.Height / dpiY;
        var workingAreaLeft = workingArea.Left / dpiX;
        var workingAreaTop = workingArea.Top / dpiY;

        // Position based on settings
        var position = _settings.ToastPosition;
        Left = position switch
        {
            ToastPosition.TopLeft or ToastPosition.BottomLeft =>
                workingAreaLeft + _settings.ToastOffsets.Horizontal,
            _ => workingAreaLeft + workingAreaWidth - ActualWidth - _settings.ToastOffsets.Horizontal
        };

        Top = position switch
        {
            ToastPosition.TopLeft or ToastPosition.TopRight =>
                workingAreaTop + _settings.ToastOffsets.Vertical,
            _ => workingAreaTop + workingAreaHeight - ActualHeight - _settings.ToastOffsets.Vertical
        };
    }
}
