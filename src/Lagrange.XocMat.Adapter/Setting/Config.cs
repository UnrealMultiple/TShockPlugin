using Lagrange.XocMat.Adapter.Setting.Configs;
using Newtonsoft.Json;
using System.Reflection;
using TShockAPI;
using TShockAPI.Hooks;

namespace Lagrange.XocMat.Adapter.Setting;

public class Config
{
    [JsonProperty("阻止未注册进入")]
    public bool LimitJoin { get; set; }

    [JsonProperty("阻止语句")]
    public string[] DisConnentFormat { get; set; } = { "未注禁止进入服务器！" };

    [JsonProperty("Socket")]
    public SocketConfig SocketConfig { get; set; } = new();

    [JsonProperty("重置设置")]
    public ResetConfig ResetConfig { get; set; } = new();

    private static string PATH => Path.Combine(TShock.SavePath, "Lagrange.XocMat.Adapter.json");

    private static Config? _instance;

    public static Config Instance => _instance ??= Read();

    public static void Write(Config? config = null)
    {
        var str = JsonConvert.SerializeObject(config ?? _instance, Formatting.Indented);
        File.WriteAllText(PATH, str);
    }   

    public static Config Read()
    {
        var c = new Config();
        if (!File.Exists(PATH))
        {

            Write(c);
            return c;
        }
        var str = File.ReadAllText(PATH);
        var ret = JsonConvert.DeserializeObject<Config>(str) ?? new();
        Write(ret);
        return ret;
    }

    public static void Reload(ReloadEventArgs e)
    {
        Plugin.RemoveAssemblyCommands(Assembly.GetExecutingAssembly());
        _instance = Read();
        Instance.SocketConfig.EmptyCommand.ForEach(x => Commands.ChatCommands.Add(new("", (_) => { }, x)));
    }
}