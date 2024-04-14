using DataSync;
using Newtonsoft.Json;
using TShockAPI;

namespace ProgressRestrict;

public class ProgressAllowed
{
    [JsonProperty("限制物品")]
    public List<int> RestrictedItems = new();
    [JsonProperty("限制弹幕")]
    public List<int> RestrictedProjectiles = new();
    [JsonProperty("限制状态")]
    public List<int> RestrictedBuffs = new();
    [JsonProperty("对应进度")]
    public ProgressType Progress;
    [JsonProperty("跨服解禁")]
    public bool AllowRemoteUnlocked;
}

public class Config
{
    [JsonProperty("惩罚违规")]
    public bool PunishPlayer = true;
    [JsonProperty("惩罚Debuff时长")]
    public int PunishTime = 5;
    [JsonProperty("公示违规者")]
    public bool Broadcast = true;
    [JsonProperty("写入日志")]
    public bool WriteLog = true;
    [JsonProperty("清除违规物品")]
    public bool ClearItem = true;
    [JsonProperty("清除违规状态")]
    public bool ClearBuff = true;
    [JsonProperty("踢出违规玩家")]
    public bool KickPlayer = false;
    [JsonProperty("限制列表")]
    public List<ProgressAllowed> Restrictions = new List<ProgressAllowed>();

    public static Config LoadConfig(string path)
    {
        var result = new Config();
        try
        {
            if (!File.Exists(path))
            {
                result.Restrictions.AddRange(Enum.GetValues(typeof(ProgressType)).Cast<ProgressType>().Select(t => new ProgressAllowed { Progress = t }));
                FileTools.CreateIfNot(path, JsonConvert.SerializeObject(result, Formatting.Indented));
            }
            result = JsonConvert.DeserializeObject<Config>(File.ReadAllText(path))!;
            File.WriteAllText(path, JsonConvert.SerializeObject(result, Formatting.Indented));
        }
        catch (Exception ex)
        {
            TShock.Log.Error(ex.ToString());
        }
        return result;
    }
}