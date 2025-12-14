using System;
using System.IO;
using System.Text.Json;

namespace ToggleNotifier.Configuration;

public class ConfigService
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    private readonly string _configPath;

    public ConfigService()
    {
        var directory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "CommonToggleButtonUI");
        Directory.CreateDirectory(directory);
        _configPath = Path.Combine(directory, "appsettings.json");
    }

    public AppSettings Load()
    {
        if (!File.Exists(_configPath))
        {
            return LoadBundledDefaults();
        }

        try
        {
            var json = File.ReadAllText(_configPath);
            var settings = JsonSerializer.Deserialize<AppSettings>(json, _jsonOptions);
            return settings ?? LoadBundledDefaults();
        }
        catch
        {
            return LoadBundledDefaults();
        }
    }

    public void Save(AppSettings settings)
    {
        var json = JsonSerializer.Serialize(settings, _jsonOptions);
        File.WriteAllText(_configPath, json);
    }

    private AppSettings LoadBundledDefaults()
    {
        try
        {
            var bundledPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            if (File.Exists(bundledPath))
            {
                var json = File.ReadAllText(bundledPath);
                var settings = JsonSerializer.Deserialize<AppSettings>(json, _jsonOptions);
                if (settings != null)
                {
                    return settings;
                }
            }
        }
        catch
        {
            // ignore fallback errors and return defaults
        }

        return new AppSettings();
    }
}
