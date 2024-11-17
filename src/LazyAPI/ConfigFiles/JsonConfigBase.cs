using Newtonsoft.Json;
using System.Globalization;
using System.Reflection;
using TShockAPI;
using TShockAPI.Hooks;

namespace LazyAPI.ConfigFiles;

public abstract class JsonConfigBase<T> where T : JsonConfigBase<T>, new()
{
    private static T? _instance;

    private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings()
    { 
        ContractResolver = new LocalizationContractResolver(),
        Formatting = Formatting.Indented,
    };

    private readonly CultureInfo cultureInfo = (CultureInfo) typeof(TShock).Assembly.GetType("TShockAPI.I18n")!.GetProperty(
            "TranslationCultureInfo",
            BindingFlags.NonPublic | BindingFlags.Static)!.GetValue(null)!;

    protected virtual string Filename => typeof(T).Namespace ?? typeof(T).Name;

    private string FullFilename => Path.Combine(TShock.SavePath, $"{this.Filename}.{this.cultureInfo.Name}.json");

    private static T GetConfig()
    {
        var t = new T();
        var file = t.FullFilename;
        if (File.Exists(file))
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(file), _settings) ?? new();
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