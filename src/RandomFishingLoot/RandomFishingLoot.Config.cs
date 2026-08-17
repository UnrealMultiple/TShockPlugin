using System.Text.Encodings.Web;
using System.Text.Json;

namespace RandomFishingLoot;

public sealed partial class RandomFishingLoot
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    private bool TryLoadConfig(out string? error)
    {
        error = null;

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_configPath)!);

            FishingLootConfig config;
            if (!File.Exists(_configPath))
            {
                config = FishingLootConfig.CreateDefault();
                SaveConfig(config);
            }
            else
            {
                string json = File.ReadAllText(_configPath);
                config = JsonSerializer.Deserialize<FishingLootConfig>(json, JsonOptions) ?? FishingLootConfig.CreateDefault();
            }

            _config = NormalizeConfig(config);
            _npcPool = BuildNpcPool(_config.RandomNpc);
            _loadSummary = BuildLoadSummary(_config);
            _pendingChoices.Clear();
            _networkCatches.Clear();
            return true;
        }
        catch (Exception ex)
        {
            error = ex.Message;
            return false;
        }
    }

    private void SaveConfig(FishingLootConfig config)
    {
        File.WriteAllText(_configPath, JsonSerializer.Serialize(config, JsonOptions) + Environment.NewLine);
    }

    private static string ResolveConfigPath()
    {
        string savePath = TShockAPI.TShock.SavePath ?? Path.Combine(AppContext.BaseDirectory, "tshock");
        Directory.CreateDirectory(savePath);
        return Path.Combine(savePath, "RandomFishingLoot.json");
    }
}
