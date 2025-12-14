using System;
using System.Windows;
using System.Windows.Interop;
using Microsoft.Win32;
using Color = System.Windows.Media.Color;
using Brushes = System.Windows.Media.Brushes;
using SolidColorBrush = System.Windows.Media.SolidColorBrush;

namespace ToggleNotifier.Theming;

/// <summary>
/// Provides theming services for Windows 11 including dark mode and backdrop effects.
/// </summary>
public class ThemeService
{
    private AppTheme _currentTheme = AppTheme.System;
    private bool _isDarkMode;

    public event EventHandler<bool>? ThemeChanged;

    public bool IsDarkMode => _isDarkMode;

    public AppTheme CurrentTheme
    {
        get => _currentTheme;
        set
        {
            if (_currentTheme != value)
            {
                _currentTheme = value;
                UpdateEffectiveTheme();
            }
        }
    }

    public ThemeService()
    {
        UpdateEffectiveTheme();
        SystemEvents.UserPreferenceChanged += OnSystemThemeChanged;
    }

    private void OnSystemThemeChanged(object sender, UserPreferenceChangedEventArgs e)
    {
        if (e.Category == UserPreferenceCategory.General && _currentTheme == AppTheme.System)
        {
            UpdateEffectiveTheme();
        }
    }

    private void UpdateEffectiveTheme()
    {
        var previousDarkMode = _isDarkMode;
        _isDarkMode = _currentTheme switch
        {
            AppTheme.Light => false,
            AppTheme.Dark => true,
            _ => GetSystemDarkModeSetting()
        };

        if (previousDarkMode != _isDarkMode)
        {
            ThemeChanged?.Invoke(this, _isDarkMode);
        }
    }

    private static bool GetSystemDarkModeSetting()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            var value = key?.GetValue("AppsUseLightTheme");
            return value is int intValue && intValue == 0;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Applies the current theme to a window, including dark mode title bar.
    /// </summary>
    public void ApplyThemeToWindow(Window window)
    {
        var hwnd = new WindowInteropHelper(window).Handle;
        if (hwnd == IntPtr.Zero)
        {
            window.SourceInitialized += (_, _) => ApplyThemeToWindow(window);
            return;
        }

        ApplyDarkModeToTitleBar(hwnd, _isDarkMode);
    }

    /// <summary>
    /// Applies Mica backdrop to a window (Windows 11 22H2+).
    /// </summary>
    public static void ApplyMicaBackdrop(Window window)
    {
        var hwnd = new WindowInteropHelper(window).Handle;
        if (hwnd == IntPtr.Zero)
        {
            window.SourceInitialized += (_, _) => ApplyMicaBackdrop(window);
            return;
        }

        var backdropType = (int)ThemeInterop.DWM_SYSTEMBACKDROP_TYPE.DWMSBT_MAINWINDOW;
        ThemeInterop.DwmSetWindowAttribute(hwnd, ThemeInterop.DWMWA_SYSTEMBACKDROP_TYPE,
            ref backdropType, sizeof(int));

        // Ensure window background is transparent to show Mica
        window.Background = Brushes.Transparent;
    }

    /// <summary>
    /// Applies Acrylic backdrop to a window (Windows 11).
    /// </summary>
    public static void ApplyAcrylicBackdrop(Window window)
    {
        var hwnd = new WindowInteropHelper(window).Handle;
        if (hwnd == IntPtr.Zero)
        {
            window.SourceInitialized += (_, _) => ApplyAcrylicBackdrop(window);
            return;
        }

        var backdropType = (int)ThemeInterop.DWM_SYSTEMBACKDROP_TYPE.DWMSBT_TRANSIENTWINDOW;
        ThemeInterop.DwmSetWindowAttribute(hwnd, ThemeInterop.DWMWA_SYSTEMBACKDROP_TYPE,
            ref backdropType, sizeof(int));
    }

    /// <summary>
    /// Applies rounded corners to a window (Windows 11).
    /// </summary>
    public static void ApplyRoundedCorners(Window window, bool small = false)
    {
        var hwnd = new WindowInteropHelper(window).Handle;
        if (hwnd == IntPtr.Zero)
        {
            window.SourceInitialized += (_, _) => ApplyRoundedCorners(window, small);
            return;
        }

        var preference = (int)(small
            ? ThemeInterop.DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUNDSMALL
            : ThemeInterop.DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND);
        ThemeInterop.DwmSetWindowAttribute(hwnd, ThemeInterop.DWMWA_WINDOW_CORNER_PREFERENCE,
            ref preference, sizeof(int));
    }

    private static void ApplyDarkModeToTitleBar(IntPtr hwnd, bool isDark)
    {
        var value = isDark ? 1 : 0;
        ThemeInterop.DwmSetWindowAttribute(hwnd, ThemeInterop.DWMWA_USE_IMMERSIVE_DARK_MODE,
            ref value, sizeof(int));
    }

    /// <summary>
    /// Gets theme-aware colors for UI elements.
    /// </summary>
    public ThemeColors GetColors()
    {
        return _isDarkMode
            ? new ThemeColors
            {
                Background = Color.FromArgb(230, 32, 32, 32),
                Surface = Color.FromArgb(255, 44, 44, 44),
                Border = Color.FromArgb(255, 60, 60, 60),
                Text = Color.FromArgb(255, 255, 255, 255),
                TextSecondary = Color.FromArgb(255, 180, 180, 180),
                Accent = Color.FromArgb(255, 96, 205, 255)
            }
            : new ThemeColors
            {
                Background = Color.FromArgb(230, 249, 249, 249),
                Surface = Color.FromArgb(255, 255, 255, 255),
                Border = Color.FromArgb(255, 229, 229, 229),
                Text = Color.FromArgb(255, 0, 0, 0),
                TextSecondary = Color.FromArgb(255, 96, 96, 96),
                Accent = Color.FromArgb(255, 0, 120, 212)
            };
    }
}

/// <summary>
/// Theme-aware color palette.
/// </summary>
public class ThemeColors
{
    public Color Background { get; init; }
    public Color Surface { get; init; }
    public Color Border { get; init; }
    public Color Text { get; init; }
    public Color TextSecondary { get; init; }
    public Color Accent { get; init; }

    public SolidColorBrush BackgroundBrush => new(Background);
    public SolidColorBrush SurfaceBrush => new(Surface);
    public SolidColorBrush BorderBrush => new(Border);
    public SolidColorBrush TextBrush => new(Text);
    public SolidColorBrush TextSecondaryBrush => new(TextSecondary);
    public SolidColorBrush AccentBrush => new(Accent);
}
