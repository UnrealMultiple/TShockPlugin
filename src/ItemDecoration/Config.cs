using System;
using System.IO;
using Newtonsoft.Json;
using TShockAPI;

namespace ConfigPlugin;

public static class ConfigManager
{
    public static T LoadConfig<T>(string configDirectory, string fileName, T defaultConfig) where T : class
    {
        var filePath = Path.Combine(configDirectory, fileName);

        try
        {
            if (!Directory.Exists(configDirectory))
            {
                Directory.CreateDirectory(configDirectory);
            }

            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<T>(json) ?? defaultConfig;
            }

            SaveConfig(filePath, defaultConfig);
            return defaultConfig;
        }
        catch (Exception ex)
        {
            TShock.Log.Error($"Error al cargar la configuración {fileName}: {ex.Message}");
            return defaultConfig;
        }
    }

    public static void SaveConfig<T>(string filePath, T config)
    {
        try
        {
            var json = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            TShock.Log.Error($"Error al guardar la configuración {filePath}: {ex.Message}");
        }
    }
}
