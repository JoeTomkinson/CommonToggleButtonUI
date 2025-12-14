using ToggleNotifier.Theming;

namespace ToggleNotifier.Configuration;

/// <summary>
/// Application settings for the toggle notifier.
/// </summary>
public class AppSettings
{
    /// <summary>
    /// Toast notification offset configuration.
    /// </summary>
    public ToastOffsets ToastOffsets { get; set; } = new();

    /// <summary>
    /// Duration in milliseconds before the toast auto-dismisses.
    /// </summary>
    public int ToastDismissMilliseconds { get; set; } = 2200;

    /// <summary>
    /// Whether to launch the application on Windows sign-in.
    /// </summary>
    public bool LaunchOnSignIn { get; set; } = true;

    /// <summary>
    /// Application theme preference.
    /// </summary>
    public AppTheme Theme { get; set; } = AppTheme.System;

    /// <summary>
    /// Visual style of the toast notification.
    /// </summary>
    public ToastStyle ToastStyle { get; set; } = ToastStyle.Standard;

    /// <summary>
    /// Whether to suppress notifications when a fullscreen application is running.
    /// </summary>
    public bool SuppressInFullscreen { get; set; } = true;

    /// <summary>
    /// Position of the toast notification on screen.
    /// </summary>
    public ToastPosition ToastPosition { get; set; } = ToastPosition.BottomRight;

    /// <summary>
    /// Whether to play a sound when a key is toggled.
    /// </summary>
    public bool PlaySound { get; set; } = false;

    /// <summary>
    /// Per-key notification settings.
    /// </summary>
    public KeyNotificationSettings KeySettings { get; set; } = new();
}

/// <summary>
/// Toast notification offset values in pixels.
/// </summary>
public class ToastOffsets
{
    public double Horizontal { get; set; } = 24;
    public double Vertical { get; set; } = 24;
}

/// <summary>
/// Position of the toast notification on screen.
/// </summary>
public enum ToastPosition
{
    BottomRight = 0,
    BottomLeft = 1,
    TopRight = 2,
    TopLeft = 3
}

/// <summary>
/// Visual style of the toast notification.
/// </summary>
public enum ToastStyle
{
    /// <summary>
    /// Standard toast with icon, key name, state text, and indicator.
    /// </summary>
    Standard = 0,

    /// <summary>
    /// Compact toast showing only the icon and status indicator dot.
    /// </summary>
    Compact = 1
}

/// <summary>
/// Per-key notification enable/disable settings.
/// </summary>
public class KeyNotificationSettings
{
    /// <summary>
    /// Show notification for Caps Lock changes.
    /// </summary>
    public bool CapsLock { get; set; } = true;

    /// <summary>
    /// Show notification for Num Lock changes.
    /// </summary>
    public bool NumLock { get; set; } = true;

    /// <summary>
    /// Show notification for Scroll Lock changes.
    /// </summary>
    public bool ScrollLock { get; set; } = true;
}
