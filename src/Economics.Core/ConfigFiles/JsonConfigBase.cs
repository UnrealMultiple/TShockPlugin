using Newtonsoft.Json;
using TShockAPI;
using TShockAPI.Hooks;

namespace Economics.Core.ConfigFiles;

public abstract class JsonConfigBase<T> where T : JsonConfigBase<T>, new()
{
    private static T? _instance;

    protected virtual string Filename => typeof(T).Namespace ?? typeof(T).Name;

    protected virtual void SetDefault()
    {
    }

    protected virtual void Initialize()
    { 
    
    }

    private string FullFilename => Path.Combine(Economics.SaveDirPath, this.Filename);


    private static T GetConfig()
    {
        var t = new T();
        var file = t.FullFilename;
        if (File.Exists(file))
        {
            var s =  JsonConvert.DeserializeObject<T>(File.ReadAllText(file)) ?? t;
            s.Initialize();
            return s;
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
        File.WriteAllText(filepath, JsonConvert.SerializeObject(this, Formatting.Indented));
    }

    protected virtual void Reload(ReloadEventArgs args)
    {
        _instance = GetConfig();
        args.Player.SendSuccessMessage(GetString($"[{this.Filename}.json] config reloaded successfully!!!"));
        Save();
    }


    public static void Save()
    {
        Instance.SaveTo();
    }

    // .cctor is lazy load
    public static void Load()
    {
        _instance = GetConfig();
        GeneralHooks.ReloadEvent += Instance.Reload;
    }

    public static void UnLoad()
    {
        GeneralHooks.ReloadEvent -= Instance.Reload;
    }

    public static T Instance => _instance ??= GetConfig();
}