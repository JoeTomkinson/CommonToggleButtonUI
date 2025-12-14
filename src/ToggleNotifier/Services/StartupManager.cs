using Microsoft.Win32;
using System.IO;

namespace ToggleNotifier.Services;

public class StartupManager
{
    private readonly string _appName;
    private readonly string _executablePath;

    public StartupManager(string appName, string executableDirectory)
    {
        _appName = appName;
        _executablePath = Path.Combine(executableDirectory, $"{appName}.exe");
    }

    public void EnableStartup()
    {
        using var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", writable: true) ??
                        Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
        key?.SetValue(_appName, _executablePath);
    }

    public void DisableStartup()
    {
        using var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", writable: true);
        key?.DeleteValue(_appName, throwOnMissingValue: false);
    }
}
