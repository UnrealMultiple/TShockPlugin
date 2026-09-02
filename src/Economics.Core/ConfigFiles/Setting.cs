using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Economics.Core.ConfigFiles;

public class Setting : JsonConfigBase<Setting>
{
    [JsonProperty("禁用雕像")]
    public bool IgnoreStatue = false;

    [JsonProperty("显示信息")]
    public bool StatusText = true;

    [JsonProperty("显示信息左移")]
    public int StatusTextShiftLeft = 60;

    [JsonProperty("显示信息下移")]
    public int StatusTextShiftDown = 0;

    [JsonProperty("显示信息内容")]
    public List<string> StatusTextContent = [];

    [JsonProperty("渐变颜色")]
    public List<string> GradientColor = [];

    [JsonProperty("货币配置")]
    public List<CurrencyDefinition> Currencies = [];

    protected override string Filename => "Economics.json";

    protected override void SetDefault()
    {
        this.StatusTextContent =
        [
            "[i:1503]玩家名称: {player}",
            "[i:2502]延迟: {ping}ms",
            "[i:267]在线人数: {online}/{maxonline}",
            "[i:893]世界名称: {world}",
            "[i:29]生命值: {life}/{maxlife}",
            "[i:110]魔力值: {mana}/{maxmana}",
            "[i:17]游戏时间: {time}",
            "[i:3122]群系: {biomes}",
            "[i:3123]当前区域: {region}",
            "[i:1327]复活计时: {respawntimer}",
            "[i:2450]渔夫任务鱼: {anglerfishname}",
            "[i:2450]渔夫任务ID: {anglerfishid}",
            "[i:3036]钓鱼地点: {anglerfishingbiome}",
            "[i:2374]渔夫任务状态: {anglerfishcompleted}",
            "[i:346]玩家组别: {playergroup}",
            "[i:855]幸运值: {playerluck}",
            "[i:58]存活状态: {playerisalive}",
            "[i:74]货币信息: {currencies}",
            "[i:1999]当前职业: {level}",
            "[i:2000]升级职业: {levelRank}",
            "[i:531]当前技能: {skill}"
        ];

        this.GradientColor =
        [
            "[c/00ffbf:{0}]",
            "[c/1aecb8:{0}]",
            "[c/33d9b1:{0}]",
            "[c/A6D5EA:{0}]",
            "[c/A6BBEA:{0}]",
            "[c/B7A6EA:{0}]",
            "[c/A6EAB3:{0}]",
            "[c/D5F0AA:{0}]",
            "[c/F5F7AF:{0}]",
            "[c/F8ECB0:{0}]",
            "[c/F8DEB0:{0}]",
            "[c/F8D0B0:{0}]",
            "[c/F8B6B0:{0}]",
            "[c/EFA9C6:{0}]",
            "[c/00ffbf:{0}]",
            "[c/1aecb8:{0}]"
        ];

        // 设置默认货币配置示例 - 展示灵活的兑换关系
        this.Currencies =
        [
            new CurrencyDefinition
            {
                Name = "金币",
                Description = "基础货币，通过击杀怪物获得",
                QueryFormat = "[c/FFD700: 金币: {1}]",
                ExchangeRates = new Dictionary<string, long>
                {
                    { "银币", 100 },
                    { "铜币", 10000 }
                },
                CurrencyObtain = new CurrencyObtainOption
                {
                    CurrencyObtainType = Enumerates.CurrencyObtainType.KillNpc,
                    ConversionRate = 0.5f
                }
            },
            new CurrencyDefinition
            {
                Name = "银币",
                Description = "次级货币，可由金币兑换",
                QueryFormat = "[c/C0C0C0: 银币: {1}]",
                ExchangeRates = new Dictionary<string, long>
                {
                    { "铜币", 100 }
                }
            },
            new CurrencyDefinition
            {
                Name = "铜币",
                Description = "最低级货币，无法继续兑换",
                QueryFormat = "[c/B87333: 铜币: {1}]",
                ExchangeRates = new Dictionary<string, long>()
            },
            new CurrencyDefinition
            {
                Name = "钻石",
                Description = "高级货币，只能通过金币兑换",
                QueryFormat = "[c/00CED1: 钻石: {1}]",
                ExchangeRates = new Dictionary<string, long>()
            }
        ];
    }

    public CurrencyDefinition? GetCurrency(string name)
    {
        return this.Currencies.Find(x => x.Name == name);
    }

    public bool HasCurrency(string type)
    {
        return this.Currencies.Any(x => x.Name == type);
    }
}
