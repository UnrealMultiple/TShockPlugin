using Newtonsoft.Json;

namespace ServerTools;


public class Config
{
    [JsonProperty("死亡延续")]
    public bool DeathLast = true;

    [JsonProperty("限制哨兵数量")]
    public int sentryLimit = 10;

    [JsonProperty("仅允许软核进入")]
    public bool OnlySoftCoresAreAllowed = false;

    [JsonProperty("是否设置世界模式")]
    public bool SetWorldMode = true;

    [JsonProperty("世界模式")]
    public int WorldMode = 2;

    [JsonProperty("设置旅途模式难度")]
    public bool SetJourneyDifficult = false;

    [JsonProperty("旅途模式难度")]
    public string JourneyMode = "master";

    [JsonProperty("阻止未注册进入")]
    public bool BlockUnregisteredEntry = false;

    [JsonProperty("禁止怪物捡钱")]
    public bool PickUpMoney = true;

    [JsonProperty("清理掉落物")]
    public bool ClearDrop = false;

    [JsonProperty("禁止多鱼线")]
    public bool MultipleFishingRodsAreProhibited = true;

    [JsonProperty("禁止快速放入箱子")]
    public bool LimitForceItemIntoNearestChest = false;

    [JsonProperty("阻止死亡角色进入")]
    public bool PreventsDeathStateJoin = true;

    [JsonProperty("未注册阻止语句")]
    public string BlockEntryStatement = "未注册不能进入服务器";

    [JsonProperty("未注册启动服务器执行命令")]
    public string[] ResetExecCommands = Array.Empty<string>();

    [JsonProperty("浮漂列表")]
    public List<short> ForbiddenBuoys = new();

    public static Config Read(string PATH)
    {
        if (File.Exists(PATH))
            return JsonConvert.DeserializeObject<Config>(File.ReadAllText(PATH)) ?? new();
        return new();
    }

    public void Write(string PATH)
    {
        File.WriteAllText(PATH, JsonConvert.SerializeObject(this, Formatting.Indented));
    }
}