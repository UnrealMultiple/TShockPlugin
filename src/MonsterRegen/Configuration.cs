using Newtonsoft.Json;
using System.Text;
using Terraria.ID;
using TShockAPI;

namespace Plugin.Configuration;

internal class Configuration
{
    [JsonProperty("插件开关", Order = -14)]
    public bool Enable { get; set; } = true;
    [JsonProperty("开启进度模式", Order = -13)]
    public bool Process { get; set; } = true;
    [JsonProperty("默认间隔/秒", Order = -12)]
    public double DefaultTimer { get; set; } = 1.25f;
    [JsonProperty("升阶减多少间隔", Order = -11)]
    public double MinusInterval { get; set; } = 0.05;
    [JsonProperty("间隔到多少不减", Order = -11)]
    public double IntervalMax { get; set; } = 0.0;
    [JsonProperty("默认回复血量/百分比", Order = -10)]
    public double HealRatio { get; set; } = 0.005;
    [JsonProperty("每次最少回复", Order = -9)]
    public int HealMin { get; set; } = 20;
    [JsonProperty("每次最多回复", Order = -9)]
    public int HealMax { get; set; } = 1000;

    [JsonProperty("还原间隔的阶级", Order = -8)]
    public int ResetLevel { get; set; } = 6;
    [JsonProperty("还原默认间隔值", Order = -8)]
    public double RepairTimer { get; set; } = 1.25f;

    [JsonProperty("进度阶级（阶级数:回复量）", Order = -4)]
    public Dictionary<int, double> LevelList { get; set; } = new Dictionary<int, double> { };
    [JsonProperty("进度BOSS表（怪物ID:阶级数）", Order = 3)]
    [JsonConverter(typeof(LevelDataConverter))]
    public List<LevelData> BossList { get; set; } = new List<LevelData>();

    [JsonProperty("禁止回血表", Order = 2)]
    public List<int> Excluded { get; set; } = new List<int>();

    #region 读取与创建配置文件方法

    public static readonly string FilePath = Path.Combine(TShock.SavePath, "怪物进度回血.json");

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
            var NewConfig = new Configuration();
            NewConfig.Init();
            NewConfig.levels();
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

    #region 阶级数据，转换键值
    //数据
    public class LevelData
    {
        [JsonProperty("怪物ID")]
        public int ID { get; set; }
        [JsonProperty("阶段")]
        public int Level { get; set; }

        public LevelData(int id, int level)
        {
            this.ID = id;
            this.Level = level;
        }
    }

    //预设怪物对应阶级
    public void Init()
    {
        this.BossList = new List<LevelData>
        {
            new LevelData(NPCID.EaterofWorldsHead,1 ),
            new LevelData(NPCID.BrainofCthulhu, 1),
            new LevelData(NPCID.WallofFlesh, 2),
            new LevelData(NPCID.TheDestroyer, 3),
            new LevelData(NPCID.Spazmatism, 3),
            new LevelData(NPCID.SkeletronPrime, 3),
            new LevelData(NPCID.Plantera, 4),
            new LevelData(NPCID.Golem, 5),
            new LevelData(NPCID.MoonLordCore, 6)
        };
    }

    //预设阶级的回复量
    public void levels()
    {
        this.LevelList = new Dictionary<int, double>
        {
            { 1, 0.01 },
            { 2, 0.015 },
            { 3, 0.02 },
            { 4, 0.025},
            { 5, 0.03 },
            { 6, 0.005}
        };
    }

    //List转换键值方法
    public class LevelDataConverter : JsonConverter<List<LevelData>>
    {
        public override void WriteJson(JsonWriter writer, List<LevelData>? value, JsonSerializer serializer)
        {
            var StageDict = value!.ToDictionary(item => item.ID, item => item.Level);
            serializer.Serialize(writer, StageDict);
        }

        public override List<LevelData> ReadJson(JsonReader reader, Type objectType, List<LevelData>? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var LevelDict = serializer.Deserialize<Dictionary<int, int>>(reader);
            return LevelDict?.Select(kv => new LevelData(kv.Key, kv.Value)).ToList() ?? new List<LevelData>();
        }
    }
    #endregion

}