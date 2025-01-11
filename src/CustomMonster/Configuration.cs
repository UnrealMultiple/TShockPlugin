using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using System.Text;

namespace CustomMonster;

public class Configuration
{
    [JsonProperty(PropertyName = "是否隐藏没用到配置项的指令", Order = -10)]
    public bool HideUselessConfig { get; set; } = false;

    [JsonProperty(PropertyName = "自定义强制隐藏哪些配置项", Order = -9)]
    public List<string> ForceHideConfigItem = new List<string>();

    [JsonProperty(PropertyName = "配置作者", Order = 0)]
    public string Author = "GK 阁下";

    [JsonProperty(PropertyName = "配置优化", Order = 1)]
    public string Optimize = "羽学";

    [JsonProperty(PropertyName = "控制台广告", Order = 2)]
    public bool Advertisement = false;

    [JsonProperty(PropertyName = "配置说明", Order = 3)]
    public string Describe = "版本①;难度直线提升,怪物最低3个人血量,几乎全Boss加强!";

    [JsonProperty(PropertyName = "启动错误报告", Order = 4)]
    public bool ErrorLogs = false;

    [JsonProperty(PropertyName = "配置文件插件版本号", Order = 5)]
    public string Version = "1.0.4.39";

    [JsonProperty(PropertyName = "启动死亡队友视角", Order = 6)]
    public bool DeathPerspective = false;

    [JsonProperty(PropertyName = "队友视角仅BOSS时", Order = 7)]
    public bool DeathPerspectiveInBOSS = true;

    [JsonProperty(PropertyName = "队友视角流畅度", Order = 8)]
    public int SmoothPerspective = -1;

    [JsonProperty(PropertyName = "队友视角等待范围", Order = 9)]
    public int PerspectiveWaiting = 18;

    [JsonProperty(PropertyName = "统一对怪物伤害修正", Order = 10)]
    public float UnifiedDamageCorrectionForMonsters = 1f;

    [JsonProperty(PropertyName = "统一怪物弹幕伤害修正", Order = 11)]
    public float UnifiedMonsterProjectileDamageCorrection = 1f;

    [JsonProperty(PropertyName = "统一初始怪物玩家系数", Order = 12)]
    public int UnifiedInitialMonsterPlayerCoefficient = 0;

    [JsonProperty(PropertyName = "统一初始玩家系数不低于人数", Order = 13)]
    public bool UnifiedInitialPlayerCoefficientNotLessThanTheNumberOfPlayers = true;

    [JsonProperty(PropertyName = "统一初始怪物强化系数", Order = 14)]
    public float UnifiedInitialMonsterEnhancementCoefficient = 0f;

    [JsonProperty(PropertyName = "统一怪物血量倍数", Order = 15)]
    public double UnifiedMonsterLifeMaxMultiplier = 1.0;

    [JsonProperty(PropertyName = "统一血量不低于正常", Order = 16)]
    public bool UnifiedLifeNotLowerThanNormal = true;

    [JsonProperty(PropertyName = "统一怪物强化倍数", Order = 17)]
    public double UnifiedMonsterEnhancementFactor = 1.0;

    [JsonProperty(PropertyName = "统一强化不低于正常", Order = 18)]
    public bool UnifiedReinforcementNotLowerThanNormal = true;

    [JsonProperty(PropertyName = "统一免疫熔岩类型", Order = 19)]
    public int UnifiedMonsterImmuneLava = 1;

    [JsonProperty(PropertyName = "统一免疫陷阱类型", Order = 20)]
    public int UnifiedMonsterImmuneTrap = 0;

    [JsonProperty(PropertyName = "统一设置例外怪物", Order = 21)]
    public List<int> IgnoreMonsterTable = new List<int>();

    [JsonProperty(PropertyName = "启动动态血量上限", Order = 22)]
    public bool DynamicLifeLimit = true;

    [JsonProperty(PropertyName = "启动怪物时间限制", Order = 23)]
    public bool MonsterTimeLimit = true;

    [JsonProperty(PropertyName = "启动动态时间限制", Order = 24)]
    public bool DynamicTimeLimit = true;

    [JsonProperty(PropertyName = "怪物节集", Order = 25)]
    public MonsterGroup[] MonsterGroup = new MonsterGroup[0];

    #region 隐藏默认值
    public class ContractResolver : DefaultContractResolver
    {
        private readonly Configuration config;
        private readonly bool hide;
        public ContractResolver(Configuration Config, bool hide)
        {
            this.config = Config;
            this.hide = hide;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            var custom = this.config.ForceHideConfigItem;
            if (this.hide && custom.Contains(member.Name)) { property.ShouldSerialize = instance => false; }

            if (member is PropertyInfo propertyInfo)
            {
                var defaultValue = Activator.CreateInstance(propertyInfo.PropertyType);
                property.ShouldSerialize = instance =>
                {
                    var value = propertyInfo.GetValue(instance);
                    var defaultValue = Activator.CreateInstance(propertyInfo.PropertyType);
                    return !(!this.config.HideUselessConfig && !Equals(value, defaultValue));
                };
            }
            return property;
        }
    }
    #endregion

    #region 读取与写入
    public Configuration Write(string path)
    {
        var settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = this.HideUselessConfig ? NullValueHandling.Ignore : NullValueHandling.Include,
            DefaultValueHandling = this.HideUselessConfig ? DefaultValueHandling.Ignore : DefaultValueHandling.Include,
            ContractResolver = new ContractResolver(this, this.HideUselessConfig),
        };

        using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
        using (var sw = new StreamWriter(fs, new UTF8Encoding(false)))
        using (var jtw = new JsonTextWriter(sw))
        {
            JsonSerializer.CreateDefault(settings).Serialize(jtw, this);
        }

        return this;
    }

    public static Configuration Read(string path)
    {
        if (!File.Exists(path))
        {
            //var NewConfig = new Configuration();
            //new Configuration().Write(path);
            //return NewConfig;
            WriteExample(path);
        }
        return JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(path))!;
    }
    #endregion

    #region 内嵌文件方法
    public static void WriteExample(string path)
    {
        var FullName = $"{Assembly.GetExecutingAssembly().GetName().Name}.CustomMonster.json";

        using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(FullName))
        {
            using (var reader = new StreamReader(stream!, Encoding.UTF8))
            {
                var text = reader.ReadToEnd();
                var config2 = JsonConvert.DeserializeObject<Configuration>(text);
                if (config2 != null)
                {
                    // 确保 path 包含文件名
                    if (!Path.HasExtension(path))
                    {
                        path = Path.Combine(path, "CustomMonster.json");
                    }
                    config2.Write(path);
                }
            }
        }
    }
    #endregion
}
