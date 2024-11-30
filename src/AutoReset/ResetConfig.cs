using LazyAPI;
using LazyAPI.ConfigFiles;
using Newtonsoft.Json;

namespace AutoReset.MainPlugin;

[Config]
public class ResetConfig : JsonConfigBase<ResetConfig>
{
    [LocalizedPropertyName(CultureType.Chinese, "CaiBot服务器令牌", Order = 8)]
    [LocalizedPropertyName(CultureType.English,"CaiBotToken")]
    public string CaiBotToken = "西江超级可爱喵";

    [LocalizedPropertyName(CultureType.Chinese, "替换文件", Order = 1)]
    [LocalizedPropertyName(CultureType.English, "replaceFiles")]
    public Dictionary<string, string>? Files;

    [LocalizedPropertyName(CultureType.Chinese, "击杀重置", Order = 2)]
    [LocalizedPropertyName(CultureType.English, "killReset")]
    public AutoReset KillToReset = new();

    [LocalizedPropertyName(CultureType.Chinese, "重置后指令", Order = 3)]
    [LocalizedPropertyName(CultureType.English, "afterResetCommand")]
    public string[]? PostResetCommands;

    [LocalizedPropertyName(CultureType.Chinese, "重置前指令", Order = 4)]
    [LocalizedPropertyName(CultureType.English, "beforeResetCommand")]
    public string[]? PreResetCommands;

    [LocalizedPropertyName(CultureType.Chinese, "重置提醒", Order = 7)]
    [LocalizedPropertyName(CultureType.English, "resetCaution")]
    public bool ResetCaution;

    [LocalizedPropertyName(CultureType.Chinese, "地图预设", Order = 6)]
    [LocalizedPropertyName(CultureType.English, "worldSetting")]
    public SetWorldConfig SetWorld = new();

    [LocalizedPropertyName(CultureType.Chinese, "重置后SQL命令", Order = 5)]
    [LocalizedPropertyName(CultureType.English, "resetSQL")]
    public string[]? SqLs;

    protected override string Filename => Path.Combine(AutoResetPlugin.AllPath, "Config");

    protected override void SetDefault()
    {
        this.KillToReset = new AutoReset();
        this.SetWorld = new SetWorldConfig();
        this.PreResetCommands = Array.Empty<string>();
        this.PostResetCommands = Array.Empty<string>();
        this.SqLs = new[]
        {
                "DELETE FROM tsCharacter"
        };
        this.Files = new Dictionary<string, string>()
        {
            {"/tshock/原神.json","原神.json"},
            {"/tshock/XSB数据缓存.json",""}
        };
    }

    public class SetWorldConfig
    {
        [LocalizedPropertyName(CultureType.Chinese, "地图名", Order = 0)]
        [LocalizedPropertyName(CultureType.English, "worldName")]
        public string? Name;

        [LocalizedPropertyName(CultureType.Chinese, "地图种子", Order = 1)]
        [LocalizedPropertyName(CultureType.English, "worldSeed")]
        public string? Seed;
    }

    public class AutoReset
    {
        [LocalizedPropertyName(CultureType.Chinese, "击杀重置开关", Order = 0)]
        [LocalizedPropertyName(CultureType.English, "killResetEnable")]
        public bool Enable;

        [LocalizedPropertyName(CultureType.Chinese, "已击杀次数", Order = 1)]
        [LocalizedPropertyName(CultureType.English, "currentKillCount")]
        public int KillCount;

        [LocalizedPropertyName(CultureType.Chinese, "需要击杀次数", Order = 3)]
        [LocalizedPropertyName(CultureType.English, "threshold")]
        public int NeedKillCount = 50;

        [LocalizedPropertyName(CultureType.Chinese, "生物ID", Order = 2)]
        [LocalizedPropertyName(CultureType.English, "npcid")]
        public int NpcId = 50;
    }
}