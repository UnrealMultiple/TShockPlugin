using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;
using Newtonsoft.Json;

namespace ServerTools;

[Config]
public class Config : JsonConfigBase<Config>
{
    [LocalizedPropertyName(CultureType.Chinese, "死亡延续")]
    [LocalizedPropertyName(CultureType.English, "DeathLast")]
    public bool DeathLast = true;

    [LocalizedPropertyName(CultureType.Chinese, "限制哨兵数量")]
    [LocalizedPropertyName(CultureType.English, "SentryLimit")]
    public int sentryLimit = 20;

    [LocalizedPropertyName(CultureType.Chinese, "限制召唤物数量")]
    [LocalizedPropertyName(CultureType.English, "SummonLimit")]
    public int summonLimit = 11;

    [LocalizedPropertyName(CultureType.Chinese, "仅允许软核进入")]
    [LocalizedPropertyName(CultureType.English, "OnlySoftCoresAreAllowed")]
    public bool OnlySoftCoresAreAllowed = false;

    [LocalizedPropertyName(CultureType.Chinese, "是否设置世界模式")]
    [LocalizedPropertyName(CultureType.English, "SetWorldMode")]
    public bool SetWorldMode = true;

    [LocalizedPropertyName(CultureType.Chinese, "世界模式")]
    [LocalizedPropertyName(CultureType.English, "WorldMode")]
    public int WorldMode = 2;

    [LocalizedPropertyName(CultureType.Chinese, "限制发言长度")]
    [LocalizedPropertyName(CultureType.English, "ChatLength")]
    public int ChatLength = 50;

    [LocalizedPropertyName(CultureType.Chinese, "设置旅途模式难度")]
    [LocalizedPropertyName(CultureType.English, "SetJourneyDifficult")]
    public bool SetJourneyDifficult = false;

    [LocalizedPropertyName(CultureType.Chinese, "禁用Emoji消息")]
    [LocalizedPropertyName(CultureType.English, "DisableEmojiMessage")]
    public bool DisableEmojiMessage = false;

    [LocalizedPropertyName(CultureType.Chinese, "旅途模式难度")]
    [LocalizedPropertyName(CultureType.English, "JourneyDifficult")]
    public string JourneyMode = "master";

    [LocalizedPropertyName(CultureType.Chinese, "阻止未注册进入")]
    [LocalizedPropertyName(CultureType.English, "BlockUnregisteredEntry")]
    public bool BlockUnregisteredEntry = false;

    [LocalizedPropertyName(CultureType.Chinese, "禁止怪物捡钱")]
    [LocalizedPropertyName(CultureType.English, "MonsterPickUpMoney")]
    public bool PickUpMoney = true;

    [LocalizedPropertyName(CultureType.Chinese, "清理掉落物")]
    [LocalizedPropertyName(CultureType.English, "ClearDrop")]
    public bool ClearDrop = false;

    [LocalizedPropertyName(CultureType.Chinese, "死亡倒计时")]
    [LocalizedPropertyName(CultureType.English, "DeadTimer")]
    public bool DeadTimer = false;

    [LocalizedPropertyName(CultureType.Chinese, "阻止死亡角色进入")]
    [LocalizedPropertyName(CultureType.English, "PreventsDeath")]
    public bool PreventsDeathStateJoin = true;

    [LocalizedPropertyName(CultureType.Chinese, "禁止双箱")]
    [LocalizedPropertyName(CultureType.English, "KeepOpenChest")]
    public bool KeepOpenChest = true;

    [LocalizedPropertyName(CultureType.Chinese, "禁止双饰品")]
    [LocalizedPropertyName(CultureType.English, "KeepArmor")]
    public bool KeepArmor = true;

    [LocalizedPropertyName(CultureType.Chinese, "禁止肉前第七格饰品")]
    [LocalizedPropertyName(CultureType.English, "KeepArmor2")]
    public bool KeepArmor2 = true;

    [LocalizedPropertyName(CultureType.Chinese, "死亡倒计时格式")]
    [LocalizedPropertyName(CultureType.English, "DeadFormat")]
    public string DeadFormat = "你还有{0}秒复活!";

    [LocalizedPropertyName(CultureType.Chinese, "未注册阻止语句")]
    [LocalizedPropertyName(CultureType.English, "BlockEntryStatement")]
    public string BlockEntryStatement = "未注册不能进入服务器";

    [LocalizedPropertyName(CultureType.Chinese, "未注册启动服务器执行命令")]
    [LocalizedPropertyName(CultureType.English, "BlockEntryExecCommands")]
    public string[] ResetExecCommands = Array.Empty<string>();

    [JsonProperty("开启NPC保护", Order = 7)]
    [LocalizedPropertyName(CultureType.English, "EnableNpcProtect")]
    public bool NpcProtect = false;

    [JsonProperty("NPC保护表", Order = 7)]
    [LocalizedPropertyName(CultureType.English, "NpcProtects")]
    public List<int> NpcProtectList = new();

    [JsonProperty("禁止多鱼线", Order = 8)]
    [LocalizedPropertyName(CultureType.English, "MultipleFishingRodsAreProhibited")]
    public bool MultipleFishingRodsAreProhibited = true;

    [JsonProperty("浮漂列表", Order = 8)]
    [LocalizedPropertyName(CultureType.English, "ForbiddenBuoys")]
    public List<short> ForbiddenBuoys = new();

    protected override string Filename => "ServerTools";

    protected override void SetDefault()
    {
        this.ForbiddenBuoys = new List<short>() { 360, 361, 362, 363, 364, 365, 366, 381, 382, 760, 775, 986, 987, 988, 989, 990, 991, 992, 993 };
        this.NpcProtectList = new List<int>() { 17, 18, 19, 20, 38, 105, 106, 107, 108, 160, 123, 124, 142, 207, 208, 227, 228, 229, 353, 354, 376, 441, 453, 550, 579, 588, 589, 633, 663, 678, 679, 680, 681, 682, 683, 684, 685, 686, 687, 375, 442, 443, 539, 444, 445, 446, 447, 448, 605, 627, 601, 613 };

    }
}