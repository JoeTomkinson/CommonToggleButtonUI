using System.IO;
using System.Text.Json;
using ToggleIndicatorUI.Models;

namespace ToggleIndicatorUI.Services
{
    public class SettingsService
    {
        private readonly JsonSerializerOptions _serializerOptions = new()
        {
            WriteIndented = true
        };

        public IndicatorSettings Load()
        {
            try
            {
                var path = GetSettingsPath();
                if (!File.Exists(path))
                {
                    return new IndicatorSettings();
                }

                var json = File.ReadAllText(path);
                var settings = JsonSerializer.Deserialize<IndicatorSettings>(json, _serializerOptions);
                return settings ?? new IndicatorSettings();
            }
            catch
            {
                return new IndicatorSettings();
            }
        }

        public void Save(IndicatorSettings settings)
        {
            var path = GetSettingsPath();
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(settings, _serializerOptions);
            File.WriteAllText(path, json);
        }

        private string GetSettingsPath()
        {
            var root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var folder = Path.Combine(root, "CommonToggleButtonUI");
            return Path.Combine(folder, "settings.json");
        }
    }
}
