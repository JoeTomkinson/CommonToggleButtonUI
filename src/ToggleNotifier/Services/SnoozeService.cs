using System;
using System.Windows.Threading;

namespace ToggleNotifier.Services;

/// <summary>
/// Manages temporary snoozing of notifications.
/// </summary>
public class SnoozeService
{
    private readonly DispatcherTimer _timer;
    private DateTime _snoozeEndTime;

    /// <summary>
    /// Raised when the snooze state changes (started or ended).
    /// </summary>
    public event EventHandler<bool>? SnoozeStateChanged;

    /// <summary>
    /// Gets whether notifications are currently snoozed.
    /// </summary>
    public bool IsSnoozed => DateTime.Now < _snoozeEndTime;

    /// <summary>
    /// Gets the remaining snooze time, or TimeSpan.Zero if not snoozed.
    /// </summary>
    public TimeSpan RemainingSnoozeTime => IsSnoozed 
        ? _snoozeEndTime - DateTime.Now 
        : TimeSpan.Zero;

    public SnoozeService()
    {
        _snoozeEndTime = DateTime.MinValue;
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(30)
        };
        _timer.Tick += OnTimerTick;
    }

    /// <summary>
    /// Snoozes notifications for the specified duration.
    /// </summary>
    /// <param name="minutes">Duration in minutes to snooze.</param>
    public void Snooze(int minutes)
    {
        _snoozeEndTime = DateTime.Now.AddMinutes(minutes);
        _timer.Start();
        SnoozeStateChanged?.Invoke(this, true);
    }

    /// <summary>
    /// Cancels any active snooze and resumes notifications immediately.
    /// </summary>
    public void CancelSnooze()
    {
        if (!IsSnoozed) return;
        
        _snoozeEndTime = DateTime.MinValue;
        _timer.Stop();
        SnoozeStateChanged?.Invoke(this, false);
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        if (!IsSnoozed)
        {
            _timer.Stop();
            SnoozeStateChanged?.Invoke(this, false);
        }
    }
}
