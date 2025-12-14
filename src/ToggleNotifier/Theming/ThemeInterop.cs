using System;
using System.Runtime.InteropServices;

namespace ToggleNotifier.Theming;

/// <summary>
/// Native methods for Windows 11 backdrop and theming support.
/// </summary>
internal static partial class ThemeInterop
{
    internal const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
    internal const int DWMWA_SYSTEMBACKDROP_TYPE = 38;
    internal const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;

    internal enum DWM_SYSTEMBACKDROP_TYPE
    {
        DWMSBT_AUTO = 0,
        DWMSBT_NONE = 1,
        DWMSBT_MAINWINDOW = 2,      // Mica
        DWMSBT_TRANSIENTWINDOW = 3, // Acrylic
        DWMSBT_TABBEDWINDOW = 4     // Tabbed Mica
    }

    internal enum DWM_WINDOW_CORNER_PREFERENCE
    {
        DWMWCP_DEFAULT = 0,
        DWMWCP_DONOTROUND = 1,
        DWMWCP_ROUND = 2,
        DWMWCP_ROUNDSMALL = 3
    }

    [LibraryImport("dwmapi.dll")]
    internal static partial int DwmSetWindowAttribute(
        IntPtr hwnd,
        int dwAttribute,
        ref int pvAttribute,
        int cbAttribute);

    [LibraryImport("uxtheme.dll", EntryPoint = "#135", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool ShouldAppsUseDarkMode();

    [LibraryImport("uxtheme.dll", EntryPoint = "#132", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool ShouldSystemUseDarkMode();
}
