using ToggleNotifier.Configuration;
using ToggleNotifier.Services;

namespace ToggleNotifier.Notifications;

public class OverlayService
{
    private AppSettings _settings;

    public OverlayService(AppSettings settings)
    {
        _settings = settings;
    }

    public void UpdateSettings(AppSettings settings)
    {
        _settings = settings;
    }

    public void ShowToast(KeyStateChangedEventArgs args)
    {
        var toast = new ToastWindow(args.KeyName, args.IsOn, _settings)
        {
            Topmost = true
        };

        toast.Show();
    }
}
