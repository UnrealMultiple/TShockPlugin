using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;

namespace AutoReset;

[Config]
public class ResetConfig : JsonConfigBase<ResetConfig>
{
    [LocalizedPropertyName(CultureType.Chinese, "替换文件", Order = 1)]
    [LocalizedPropertyName(CultureType.English, "ReplaceFiles")]
    public Dictionary<string, string>? Files;

    [LocalizedPropertyName(CultureType.Chinese, "击杀重置", Order = 2)]
    [LocalizedPropertyName(CultureType.English, "KillToReset")]
    public AutoReset KillToReset = new();

    [LocalizedPropertyName(CultureType.Chinese, "重置后指令", Order = 3)]
    [LocalizedPropertyName(CultureType.English, "AfterResetCommand")]
    public string[]? PostResetCommands;

    [LocalizedPropertyName(CultureType.Chinese, "重置前指令", Order = 4)]
    [LocalizedPropertyName(CultureType.English, "BeforeResetCommand")]
    public string[]? PreResetCommands;

    [LocalizedPropertyName(CultureType.Chinese, "地图预设", Order = 6)]
    [LocalizedPropertyName(CultureType.English, "WorldSetting")]
    public SetWorldConfig WorldSetting = new();

    [LocalizedPropertyName(CultureType.Chinese, "重置后SQL命令", Order = 5)]
    [LocalizedPropertyName(CultureType.English, "AfterResetSQL")]
    public string[]? SqLs;

    protected override string Filename => Path.Combine(AutoResetPlugin.FolderName, "AutoReset");

    protected override void SetDefault()
    {
        this.KillToReset = new AutoReset();
        this.WorldSetting = new SetWorldConfig();
        this.PreResetCommands = [];
        this.PostResetCommands = new string[] { };
        this.SqLs =
        [
            "DELETE FROM tsCharacter"
        ];
        this.Files = new Dictionary<string, string>()
        {
            {"/tshock/原神.json","原神.json"},
            {"/tshock/XSB.json",""}
        };
    }

    public class SetWorldConfig
    {
        [LocalizedPropertyName(CultureType.Chinese, "地图名", Order = 0)]
        [LocalizedPropertyName(CultureType.English, "WorldName")]
        public string? Name;

        [LocalizedPropertyName(CultureType.Chinese, "地图种子", Order = 1)]
        [LocalizedPropertyName(CultureType.English, "WorldSeed")]
        public string Seed = "";
        
        [LocalizedPropertyName(CultureType.Chinese, "特殊种子", Order = 1)]
        [LocalizedPropertyName(CultureType.English, "SpecialSeed")]
        public long GenerationOptionsFlag;
    }

    public class AutoReset
    {
        [LocalizedPropertyName(CultureType.Chinese, "击杀重置开关", Order = 0)]
        [LocalizedPropertyName(CultureType.English, "KillResetEnable")]
        public bool Enable;

        [LocalizedPropertyName(CultureType.Chinese, "已击杀次数", Order = 1)]
        [LocalizedPropertyName(CultureType.English, "CurrentKillCount")]
        public int KillCount;

        [LocalizedPropertyName(CultureType.Chinese, "需要击杀次数", Order = 3)]
        [LocalizedPropertyName(CultureType.English, "NeedKillCount")]
        public int NeedKillCount = 50;

        [LocalizedPropertyName(CultureType.Chinese, "生物ID", Order = 2)]
        [LocalizedPropertyName(CultureType.English, "NpcId")]
        public int NpcId = 50;
    }
}