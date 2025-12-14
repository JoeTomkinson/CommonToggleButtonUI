using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;

namespace ToggleNotifier.Services;

public class StartupManager
{
    private readonly string _appName;
    private readonly string _executablePath;

    public StartupManager(string appName, string executableDirectory)
    {
        _appName = appName;
        // Use the actual executing assembly name instead of hardcoded appName
        var exeName = Path.GetFileName(Assembly.GetExecutingAssembly().Location);
        // Fallback for single-file publish scenarios
        if (string.IsNullOrEmpty(exeName) || !exeName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
        {
            exeName = $"{Assembly.GetExecutingAssembly().GetName().Name}.exe";
        }
        _executablePath = Path.Combine(executableDirectory, exeName);
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
