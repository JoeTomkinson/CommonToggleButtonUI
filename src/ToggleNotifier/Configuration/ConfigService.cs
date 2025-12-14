using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ToggleNotifier.Configuration;

public class ConfigService
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
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
            
            // Merge with defaults to ensure new properties have default values
            if (settings != null)
            {
                return MergeWithDefaults(settings);
            }
            
            return LoadBundledDefaults();
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

    /// <summary>
    /// Ensures that all new properties have their default values when loading
    /// an older config file that may not contain them.
    /// </summary>
    private static AppSettings MergeWithDefaults(AppSettings loaded)
    {
        var defaults = new AppSettings();
        
        // Ensure nested objects are not null
        loaded.ToastOffsets ??= defaults.ToastOffsets;
        loaded.KeySettings ??= defaults.KeySettings;
        
        return loaded;
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
                    return MergeWithDefaults(settings);
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
