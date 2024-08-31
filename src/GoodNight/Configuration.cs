using Newtonsoft.Json;
using System.Text;
using TShockAPI;

namespace Goodnight;

public class TimeRange
{
    public TimeSpan Start { get; set; }
    public TimeSpan Stop { get; set; }
}


internal class Configuration
{
    public static readonly string FilePath = Path.Combine(TShock.SavePath, "宵禁.json");

    [JsonProperty("是否关闭宵禁", Order = -15)]
    public bool Enabled { get; set; } = true;
    [JsonProperty("宵禁时间设置(禁怪/断连)", Order = -14)]
    public TimeRange Time { get; set; } = new TimeRange()
    {
        Start = TimeSpan.FromHours(0),
        Stop = TimeSpan.FromHours(7)
    };
    [JsonProperty("宵禁是否断连", Order = -13)]
    public bool DiscPlayers = false;
    [JsonProperty("玩家进服拦截消息", Order = -13)]
    public string JoinMessage = "当前为宵禁时间，无法加入游戏。";
    [JsonProperty("踢出玩家断连消息", Order = -13)]
    public string NewProjMessage = "到点了，晚安";
    [JsonProperty("断连白名单", Order = -13)]
    public HashSet<string> PlayersList { get; set; } = new HashSet<string>();

    [JsonProperty("关闭禁怪所需人数(设1为关闭)", Order = -12)]
    public int MaxPlayers { get; set; } = 2;
    [JsonProperty("是否开启召唤区", Order = -11)]
    public bool Region = false;
    [JsonProperty("只播报BOSS或非BOSS", Order = -11)]
    public bool BcstSwitch = true;
    [JsonProperty("关闭切换播报类型", Order = -11)]
    public bool BcstSwitchOff = true;
    [JsonProperty("召唤区的名字", Order = -10)]
    public string RegionName = "召唤区";
    [JsonProperty("召唤区是否需要所有人", Order = -9)]
    public bool PlayersInRegion = true;
    [JsonProperty("计入'允许召唤表'的击杀次数", Order = -8)]
    public int DeadCount { get; set; } = 2;
    [JsonProperty("重置'允许召唤表'的怪物ID", Order = -7)]
    public int ResetNpcDead { get; set; } = 398;
    [JsonProperty("允许召唤表(根据禁怪表ID自动写入)", Order = -6)]
    public HashSet<int> NpcDead = new HashSet<int>();
    [JsonProperty("禁止怪物生成表(NpcID)", Order = -5)]
    public HashSet<int> Npcs = new HashSet<int>();


    #region 读取与创建配置文件方法
    public void Write()
    {
        using (var fs = new FileStream(FilePath, FileMode.Create, FileAccess.Write, FileShare.Write))
        using (var sw = new StreamWriter(fs, new UTF8Encoding(false)))
        {
            sw.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
        }
    }

    public static Configuration Read()
    {
        if (!File.Exists(FilePath))
        {
            var NewConfig = new Configuration
            {
                PlayersList = new HashSet<string>() { "羽学" },
                Npcs = new HashSet<int>() { 4, 13, 14, 15, 35, 36, 50, 113, 114, 125, 126, 127, 128, 129, 130, 131, 134, 135, 136, 222, 245, 246, 247, 248, 249, 262, 266, 370, 396, 397, 398, 400, 439, 440, 422, 493, 507, 517, 636, 657, 668 }
            };
            new Configuration().Write();
            return NewConfig;
        }
        else
        {
            var jsonContent = File.ReadAllText(FilePath);
            return JsonConvert.DeserializeObject<Configuration>(jsonContent)!;
        }
    }
    #endregion

    #region 白名单增删改查方法
    internal bool Exempt(string Name)
    {
        return this.PlayersList.Contains(Name);
    }

    public string GetExemptPlayersAsString()
    {
        return string.Join(", ", this.PlayersList);
    }

    public bool Add(string name)
    {
        if (this.Exempt(name))
        {
            return false;
        }
        this.PlayersList.Add(name);
        this.Write();
        return true;
    }

    public bool Del(string name)
    {
        if (this.Exempt(name))
        {
            this.PlayersList.Remove(name);
            this.Write();
            return true;
        }
        return false;
    }
    #endregion
}