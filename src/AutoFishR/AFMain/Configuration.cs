using System.Globalization;
using System.Reflection;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using TShockAPI;

namespace AutoFish.AFMain;

/// <summary>
///     插件配置模型，负责序列化与默认值初始化。
/// </summary>
internal class Configuration
{
    /// <summary>配置目录。</summary>
    public static readonly string ConfigDirectory = Path.Combine(TShock.SavePath, "AutoFish");

    /// <summary>配置文件路径。</summary>
    public static readonly string FilePath = Path.Combine(ConfigDirectory, "config.yml");

    private static readonly IDeserializer Deserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();

    private static readonly ISerializer Serializer = new SerializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .WithIndentedSequences()
        .Build();

    /// <summary>插件总开关。</summary>
    public bool PluginEnabled { get; set; } = true;

    /// <summary>界面与输出语言，默认 zh-cn。</summary>
    public string Language { get; set; } = "zh-cn";

    /// <summary>全局启用自动钓鱼功能。</summary>
    public bool GlobalAutoFishFeatureEnabled { get; set; } = true;

    /// <summary>玩家默认是否开启自动钓鱼。</summary>
    public bool DefaultAutoFishEnabled { get; set; }

    /// <summary>全局启用自动 Buff 功能。</summary>
    public bool GlobalBuffFeatureEnabled { get; set; } = true;

    /// <summary>玩家默认是否开启自动 Buff。</summary>
    public bool DefaultBuffEnabled { get; set; }

    /// <summary>全局启用多钩功能。</summary>
    public bool GlobalMultiHookFeatureEnabled { get; set; } = true;

    /// <summary>多钩数量上限。</summary>
    public int GlobalMultiHookMaxNum { get; set; } = 5;

    /// <summary>玩家默认是否开启多钩。</summary>
    public bool DefaultMultiHookEnabled { get; set; }

    /// <summary>全局启用消耗模式。</summary>
    public bool GlobalConsumptionModeEnabled { get; set; }

    /// <summary>玩家默认是否开启消耗模式。</summary>
    public bool DefaultConsumptionEnabled { get; set; }

    /// <summary>奖励消耗的鱼饵数量。</summary>
    public int BaitConsumeCount { get; set; } = 10;

    /// <summary>奖励 Buff 持续分钟数。</summary>
    public int RewardDurationMinutes { get; set; } = 12;

    /// <summary>消耗模式允许的鱼饵 ID 列表。</summary>
    public List<int> BaitItemIds { get; set; } = new()
    {
        2002, // 天界蜻蜓
        2675, // 魔金虫
        2676, // 火焰苍蝇
        3191, // 魔煞虫
        3194  // 恶魔心
    };

    /// <summary>全局跳过不可堆叠渔获。</summary>
    public bool GlobalSkipNonStackableLoot { get; set; } = true;

    /// <summary>玩家默认是否跳过不可堆叠渔获。</summary>
    public bool DefaultSkipNonStackableLoot { get; set; } = true;

    /// <summary>全局禁止钓上怪物。</summary>
    public bool GlobalBlockMonsterCatch { get; set; } = true;

    /// <summary>玩家默认是否禁止钓上怪物。</summary>
    public bool DefaultBlockMonsterCatch { get; set; } = true;

    /// <summary>全局跳过钓鱼动画。</summary>
    public bool GlobalSkipFishingAnimation { get; set; } = true;

    /// <summary>玩家默认是否跳过钓鱼动画。</summary>
    public bool DefaultSkipFishingAnimation { get; set; } = true;

    /// <summary>全局保护贵重鱼饵。</summary>
    public bool GlobalProtectValuableBaitEnabled { get; set; } = true;

    /// <summary>玩家默认是否保护贵重鱼饵。</summary>
    public bool DefaultProtectValuableBaitEnabled { get; set; } = true;

    /// <summary>贵重鱼饵 ID 列表。</summary>
    public List<int> ValuableBaitItemIds { get; set; } = new()
    {
        2673, // 松露虫
        1999, // 帛斑蝶
        2436, // 蓝水母
        2437, // 绿水母
        2438, // 粉水母
        2891, // 金蝴蝶
        4340, // 金蜻蜓
        2893, // 金蚱蜢
        4362, // 金瓢虫
        4419, // 金水黾
        2895  // 金蠕虫
    };

    /// <summary>随机渔获功能开关。</summary>
    public bool RandomLootEnabled { get; set; }

    /// <summary>额外掉落的物品 ID 列表。</summary>
    public List<int> ExtraCatchItemIds { get; set; } = new()
    {
        5,    // 蘑菇
        72,   // 银币
        75,   // 坠落之星
        276,  // 仙人掌
        3093, // 草药袋
        4345  // 蠕虫罐头
    };

    /// <summary>Buff ID 与持续秒数映射。</summary>
    public Dictionary<int, int> BuffDurations { get; set; } = new();

    /// <summary>禁用发射物 ID 列表。</summary>
    public int[] DisabledProjectileIds { get; set; } =
    {
        623, 625, 626, 627, 628, 831, 832, 833, 834, 835, 963, 970
    };

    /// <summary>
    ///     将当前配置写入磁盘。
    /// </summary>
    public void Write()
    {
        Directory.CreateDirectory(ConfigDirectory);
        var yaml = Serializer.Serialize(this);
        File.WriteAllText(FilePath, yaml);
    }

    /// <summary>
    ///     读取配置文件，若不存在则创建默认配置。
    /// </summary>
    public static Configuration Read()
    {
        EnsureConfigFileExists();

        var yamlContent = File.ReadAllText(FilePath);
        var config = Deserializer.Deserialize<Configuration>(yamlContent) ?? new Configuration();
        config.Normalize();
        return config;
    }

    private void Normalize()
    {
        Language = string.IsNullOrWhiteSpace(Language) ? "zh-cn" : Language.ToLowerInvariant();
        BaitItemIds ??= new List<int> { 2002, 2675, 2676, 3191, 3194 };
        ValuableBaitItemIds ??= new List<int>
        {
            2673,
            1999,
            2436,
            2437,
            2438,
            2891,
            4340,
            2893,
            4362,
            4419,
            2895
        };
        ExtraCatchItemIds ??= new List<int> { 5, 72, 75, 276, 3093, 4345 };
        BuffDurations ??= new Dictionary<int, int>();
        DisabledProjectileIds ??= new[] { 623, 625, 626, 627, 628, 831, 832, 833, 834, 835, 963, 970 };
    }

    private static void EnsureConfigFileExists()
    {
        Directory.CreateDirectory(ConfigDirectory);

        if (File.Exists(FilePath))
        {
            Console.WriteLine("[AutoFish]配置文件成功找到并加载");
            return;
        }

        var preferredCulture = ResolvePreferredConfigCulture();
        if (TryExportEmbeddedConfig(preferredCulture))
        {
            Console.WriteLine($"[AutoFish]导出 {preferredCulture} 默认配置成功");
            return;
        }

        if (!preferredCulture.Equals("en-us", StringComparison.OrdinalIgnoreCase) &&
            TryExportEmbeddedConfig("en-us"))
        {
            Console.WriteLine("[AutoFish]导出 en-us 默认配置成功");
            return;
        }

        if (!preferredCulture.Equals("zh-cn", StringComparison.OrdinalIgnoreCase) &&
            TryExportEmbeddedConfig("zh-cn"))
        {
            Console.WriteLine("[AutoFish]导出 zh-cn 默认配置成功");
            return;
        }

        Console.WriteLine($"[AutoFish]无法导出默认配置！！！！ {preferredCulture} ");
        var defaultConfig = new Configuration();
        defaultConfig.Write();
    }

    private static bool TryExportEmbeddedConfig(string culture)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith($"{culture}.yml", StringComparison.OrdinalIgnoreCase));

        if (resourceName == null)
        {
            return false;
        }

        using var resourceStream = assembly.GetManifestResourceStream(resourceName);
        if (resourceStream == null)
        {
            return false;
        }

        using var reader = new StreamReader(resourceStream);
        var content = reader.ReadToEnd();
        File.WriteAllText(FilePath, content);
        return true;
    }

    private static string ResolvePreferredConfigCulture()
    {
        var uiCulture = CultureInfo.CurrentUICulture;
        var name = uiCulture.Name.ToLowerInvariant();

        if (name.StartsWith("zh"))
        {
            return "zh-cn";
        }

        if (name.StartsWith("en"))
        {
            return "en-us";
        }

        return "en-us";
    }
}