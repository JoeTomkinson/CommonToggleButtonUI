using System;
using ToggleNotifier.Interop;

namespace ToggleNotifier.Services;

/// <summary>
/// Detects whether a fullscreen application is currently running.
/// </summary>
public static class FullscreenDetector
{
    /// <summary>
    /// Determines if the current foreground window is running in fullscreen mode.
    /// </summary>
    /// <returns>True if a fullscreen application is detected, false otherwise.</returns>
    public static bool IsFullscreenAppRunning()
    {
        var foregroundWindow = NativeMethods.GetForegroundWindow();
        
        if (foregroundWindow == IntPtr.Zero)
            return false;

        // Ignore desktop and shell windows
        var shellWindow = NativeMethods.GetShellWindow();
        var desktopWindow = NativeMethods.GetDesktopWindow();
        
        if (foregroundWindow == shellWindow || foregroundWindow == desktopWindow)
            return false;

        // Get the window rectangle
        if (!NativeMethods.GetWindowRect(foregroundWindow, out var windowRect))
            return false;

        // Get the monitor info for the window's monitor
        var monitor = NativeMethods.MonitorFromWindow(foregroundWindow, NativeMethods.MONITOR_DEFAULTTONEAREST);
        if (monitor == IntPtr.Zero)
            return false;

        var monitorInfo = new NativeMethods.MONITORINFO { cbSize = System.Runtime.InteropServices.Marshal.SizeOf<NativeMethods.MONITORINFO>() };
        if (!NativeMethods.GetMonitorInfo(monitor, ref monitorInfo))
            return false;

        // Check if the window covers the entire monitor
        var monitorRect = monitorInfo.rcMonitor;
        
        return windowRect.Left <= monitorRect.Left &&
               windowRect.Top <= monitorRect.Top &&
               windowRect.Right >= monitorRect.Right &&
               windowRect.Bottom >= monitorRect.Bottom;
    }
}
