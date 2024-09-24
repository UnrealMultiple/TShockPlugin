using Newtonsoft.Json;
using System.ComponentModel;
using System.Reflection;

namespace AutoPluginManager;

public class Config
{
    public const string Path = "tshock/AutoPluginManager.json";

    public static Config PluginConfig = new();

    public static readonly Dictionary<string, Platforms> _names = typeof(Platforms)
        .GetFields()
        .Where(x => x.FieldType == typeof(Platforms))
        .ToDictionary(f => f.GetCustomAttribute<DescriptionAttribute>()!.Description, f => (Platforms)f.GetValue(-1)!);

    public static readonly Dictionary<Platforms, string> _values = typeof(Platforms)
        .GetFields()
        .Where(x => x.FieldType == typeof(Platforms))
        .ToDictionary( f => (Platforms) f.GetValue(-1)!, f => f.GetCustomAttribute<DescriptionAttribute>()!.Description);
    public void Write()
    {
        using FileStream fileStream = new(Path, FileMode.Create, FileAccess.Write, FileShare.Write);
        this.Write(fileStream);
    }

    public static Platforms GetPlatformType(string pf)
    { 
        return _names.TryGetValue(pf, out var platfrom) ? platfrom : default;
    }

    public static string GetPlatformName(Platforms plat)
    {
        return _values[plat];
    }

    private void Write(Stream stream)
    {
        var value = JsonConvert.SerializeObject(this, Formatting.Indented);
        using (StreamWriter streamWriter = new(stream))
        {
            streamWriter.Write(value);
        }
    }

    public static void Read()
    {
        Config? result;
        if (!File.Exists(Path))
        {
            result = new Config();
            result.Write();
        }
        else
        {
            using FileStream fileStream = new(Path, FileMode.Open, FileAccess.Read, FileShare.Read);
            result = Read(fileStream);
        }
        PluginConfig = result!;
    }

    private static Config? Read(Stream stream)
    {
        Config? result;
        using StreamReader streamReader = new(stream);
        result = JsonConvert.DeserializeObject<Config>(streamReader.ReadToEnd());
        return result;
    }
    [JsonProperty("允许自动更新插件")] public bool AutoUpdate = false;
    [JsonProperty("使用Github源")] public bool UseGithubSource = false;
    [JsonProperty("插件排除列表")] public List<string> UpdateBlackList = new();
    [JsonProperty("插件运行平台")]
    [JsonConverter(typeof(PlatformConverter))]
    public Platforms RunPlatform { get; set; } = _runplatform;

    private static Platforms _runplatform
    { 
        get
        {
            return Environment.OSVersion.Platform switch
            {
                PlatformID.Win32NT => Platforms.WindowsX64,
                PlatformID.Unix => Platforms.Linux64,
                _ => Platforms.LinuxArm,
            };
        } 
    }

    internal class PlatformConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Platforms);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            return GetPlatformType($"{reader.Value}");
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is Platforms plat)
            {
                writer.WriteValue(GetPlatformName(plat));
            }
        }
    }
}