using Newtonsoft.Json;
using TShockAPI;
using TShockAPI.Hooks;

namespace LazyAPI.ConfigFiles;

public abstract class JsonConfigBase<T> where T : JsonConfigBase<T>, new()
{
    private static T? _instance;
    protected virtual string Filename => typeof(T).Namespace ?? typeof(T).Name;
    private string FullFilename => Path.Combine(TShock.SavePath, this.Filename + ".json");

    private static T GetConfig()
    {
        var t = new T();
        var file = t.FullFilename;
        if (File.Exists(file))
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(file));
        }

        File.WriteAllText(file, JsonConvert.SerializeObject(t, Formatting.Indented));
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