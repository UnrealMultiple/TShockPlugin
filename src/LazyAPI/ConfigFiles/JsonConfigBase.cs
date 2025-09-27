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

    internal static GeneralHooks.ReloadEventD? ReloadEvent { get; set; }

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
            "es" or "es-es" => new CultureInfo("es-ES"),
            "ru" or "ru-ru" => new CultureInfo("ru-RU"),
            _ => (CultureInfo) typeof(TShock).Assembly.GetType("TShockAPI.I18n")!.GetProperty(
        "TranslationCultureInfo",
        BindingFlags.NonPublic | BindingFlags.Static)!.GetValue(null)!
        };
        
        if (string.IsNullOrEmpty(cultureInfo.Name))
        {
            cultureInfo = new CultureInfo("en-US");
        } 
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
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(file), _settings) ?? t;
        }

        t.SetDefault();
        t.SaveTo();
        return t;
    }

    public virtual void SaveTo(string? path = null)
    {
        var filepath = path ?? this.FullFilename;
        var dirPath = Path.GetDirectoryName(filepath);
        if (!string.IsNullOrEmpty(dirPath))
        {
            var dirInfo = new DirectoryInfo(dirPath);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
        }
        File.WriteAllText(filepath, JsonConvert.SerializeObject(this, _settings));
    }

    protected virtual void Reload(ReloadEventArgs args)
    {
        args.Player.SendSuccessMessage(GetString($"[{this.Filename}.{cultureInfo.Name}.json] config reloaded successfully!!!"));
    }


    public static void Save()
    {
        Instance.SaveTo();
    }

    // .cctor is lazy load
    public static string Load()
    {
        ReloadEvent = args =>
        {
            _instance = GetConfig();
            _instance.Reload(args);
        };
        GeneralHooks.ReloadEvent += ReloadEvent;
        return Instance.Filename;
    }

    public static T Instance => _instance ??= GetConfig();
}