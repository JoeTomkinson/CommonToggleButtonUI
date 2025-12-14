namespace ToggleNotifier.Configuration;

public class AppSettings
{
    public ToastOffsets ToastOffsets { get; set; } = new();

    public int ToastDismissMilliseconds { get; set; } = 2200;

    public bool LaunchOnSignIn { get; set; } = true;
}

public class ToastOffsets
{
    public double Horizontal { get; set; } = 20;
    public double Vertical { get; set; } = 20;
}
