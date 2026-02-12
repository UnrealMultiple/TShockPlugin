using System.Globalization;
using System.Reflection;
using TShockAPI;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

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

    /// <summary>是否首次生成配置文件（仅本次运行内有效）。</summary>
    internal static bool IsFirstInstall { get; private set; }

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

    /// <summary>
    ///     消耗模式鱼饵配置。
    ///     键：鱼饵物品ID
    ///     值：Tuple<每次消耗数量, 兑换分钟数>
    /// </summary>
    public Dictionary<int, BaitReward> BaitRewards { get; set; } = new()
    {
        { 2002, new BaitReward { Count = 1, Minutes = 1 } }, // 蠣虫
        { 2675, new BaitReward { Count = 1, Minutes = 5 } }, // 熟手诱饵
        { 2676, new BaitReward { Count = 1, Minutes = 10 } }, // 大师诱饵
        { 3191, new BaitReward { Count = 1, Minutes = 8 } }, // 附魔夜行者
        { 3194, new BaitReward { Count = 1, Minutes = 5 } } // 蝗虫
    };

    /// <summary>全局禁止钓上怪物。</summary>
    public bool GlobalBlockMonsterCatch { get; set; } = true;

    /// <summary>玩家默认是否禁止钓上怪物。</summary>
    public bool DefaultBlockMonsterCatch { get; set; } = true;

    /// <summary>全局跳过钓鱼动画。</summary>
    public bool GlobalSkipFishingAnimation { get; set; } = true;

    /// <summary>玩家默认是否跳过钓鱼动画。</summary>
    public bool DefaultSkipFishingAnimation { get; set; } = false;

    /// <summary>全局屏蔽任务鱼。</summary>
    public bool GlobalBlockQuestFish { get; set; } = true;

    /// <summary>玩家默认是否屏蔽任务鱼。</summary>
    public bool DefaultBlockQuestFish { get; set; } = false;

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
        2895 // 金蠕虫
    };

    /// <summary>Buff ID 与持续秒数映射。</summary>
    public Dictionary<int, int> BuffDurations { get; set; } = new();


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
        var config = Deserializer.Deserialize<Configuration>(yamlContent);
        config.Normalize();
        return config;
    }

    private void Normalize()
    {
        Language = string.IsNullOrWhiteSpace(Language) ? "zh-cn" : Language.ToLowerInvariant();
    }

    private static void EnsureConfigFileExists()
    {
        Directory.CreateDirectory(ConfigDirectory);

        if (File.Exists(FilePath))
        {
            Console.WriteLine("[AutoFish]配置文件成功找到并加载");
            return;
        }

        IsFirstInstall = true;

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

        if (resourceName == null) return false;

        using var resourceStream = assembly.GetManifestResourceStream(resourceName);
        if (resourceStream == null) return false;

        using var reader = new StreamReader(resourceStream);
        var content = reader.ReadToEnd();
        File.WriteAllText(FilePath, content);
        return true;
    }

    private static string ResolvePreferredConfigCulture()
    {
        var uiCulture = CultureInfo.CurrentUICulture;
        var name = uiCulture.Name.ToLowerInvariant();

        if (name.StartsWith("zh")) return "zh-cn";

        if (name.StartsWith("en")) return "en-us";

        return "en-us";
    }
}

/// <summary>
///     鱼饵奖励配置。
/// </summary>
public class BaitReward
{
    /// <summary>每次消耗的鱼饵数量。</summary>
    public int Count { get; set; }

    /// <summary>兑换的时长（分钟）。</summary>
    public int Minutes { get; set; }
}