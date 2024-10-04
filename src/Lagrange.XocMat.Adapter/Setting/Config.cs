using MorMorAdapter.Setting.Configs;
using Newtonsoft.Json;
using TShockAPI;

namespace MorMorAdapter.Setting;

public class Config
{
    [JsonProperty("阻止未注册进入")]
    public bool LimitJoin { get; set; }

    [JsonProperty("阻止语句")]
    public string DisConnentFormat { get; set; } = "未注禁止进入服务器！";

    [JsonProperty("Socket")]
    public SocketConfig SocketConfig { get; set; } = new();

    [JsonProperty("重置设置")]
    public ResetConfig ResetConfig { get; set; } = new();

    [JsonIgnore]
    public string PATH => Path.Combine(TShock.SavePath, "Lagrange.XocMat.Adapter.json");
    public void Write()
    {
        var str = JsonConvert.SerializeObject(this, Formatting.Indented);
        File.WriteAllText(this.PATH, str);
    }

    public void Write(Config config)
    {
        var str = JsonConvert.SerializeObject(config, Formatting.Indented);
        File.WriteAllText(this.PATH, str);
    }

    public Config Read()
    {
        if (!File.Exists(this.PATH))
        {
            this.Write();
            return this;
        }
        var str = File.ReadAllText(this.PATH);
        var ret = JsonConvert.DeserializeObject<Config>(str) ?? new();
        this.Write(ret);
        return ret;
    }
}