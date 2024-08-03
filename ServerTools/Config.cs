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

    [JsonProperty("限制发言长度")]
    public int ChatLength = 2;

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

    [JsonProperty("死亡倒计时")]
    public bool DeadTimer = false;

    [JsonProperty("阻止死亡角色进入")]
    public bool PreventsDeathStateJoin = true;

    [JsonProperty("禁止双箱")]
    public bool KeepOpenChest = true;

    [JsonProperty("禁止双饰品")]
    public bool KeepArmor = true;

    [JsonProperty("禁止肉前第七格饰品")]
    public bool KeepArmor2 = true;

    [JsonProperty("死亡倒计时格式")]
    public string DeadFormat = "你还有{0}秒复活!";

    [JsonProperty("未注册阻止语句")]
    public string BlockEntryStatement = "未注册不能进入服务器";

    [JsonProperty("未注册启动服务器执行命令")]
    public string[] ResetExecCommands = Array.Empty<string>();

    [JsonProperty("开启NPC保护", Order = 7)]
    public bool NpcProtect = false;

    [JsonProperty("NPC保护表", Order = 7)]
    public List<int> NpcProtectList = new ();

    [JsonProperty("禁止多鱼线", Order = 8)]
    public bool MultipleFishingRodsAreProhibited = true;

    [JsonProperty("浮漂列表", Order = 8)]
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
