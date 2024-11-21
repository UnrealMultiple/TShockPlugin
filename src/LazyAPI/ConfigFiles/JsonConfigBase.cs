using Newtonsoft.Json;
using System.Globalization;
using System.Reflection;
using TShockAPI;
using TShockAPI.Hooks;

namespace LazyAPI.ConfigFiles;

public abstract class JsonConfigBase<T> where T : JsonConfigBase<T>, new()
{
    private static T? _instance;

    private static JsonSerializerSettings _settings = null!;

    private static CultureInfo cultureInfo = null!;

    protected virtual string Filename => typeof(T).Namespace ?? typeof(T).Name;

    protected virtual void SetDefault()
    {
    }

    private string FullFilename => Path.Combine(TShock.SavePath, $"{this.Filename}.{cultureInfo.Name}.json");

    protected JsonConfigBase()
    {
        cultureInfo = Terraria.Program.LaunchParameters.GetValueOrDefault("-culture")?.ToLower() switch
        {
            "zh" or "zh-cn" => new CultureInfo("zh-CN"),
            "en" or "en-us" => new CultureInfo("en-US"),
            _ => (CultureInfo) typeof(TShock).Assembly.GetType("TShockAPI.I18n")!.GetProperty(
        "TranslationCultureInfo",
        BindingFlags.NonPublic | BindingFlags.Static)!.GetValue(null)!
        };

        _settings = new JsonSerializerSettings()
        {
            ContractResolver = new CultureContractResolver(cultureInfo),
            Formatting = Formatting.Indented,
        };
    }

    private static T GetConfig()
    {
        var t = new T();
        var file = t.FullFilename;
        if (File.Exists(file))
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(file), _settings) ?? new();
        }
        else
        {
            t.SetDefault();
        }
        File.WriteAllText(file, JsonConvert.SerializeObject(t, _settings));
        return t;
    }

    // .cctor is lazy load
    public static string Load()
    {
        GeneralHooks.ReloadEvent += _ => _instance = GetConfig();
        return Instance.Filename;
    }

    public static T Instance => _instance ??= GetConfig();
}