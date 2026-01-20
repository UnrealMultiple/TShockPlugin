using System.Globalization;
using System.Reflection;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using TShockAPI;

namespace AutoFish.AFMain;

/// <summary>
///     简易语言管理器：从 tshock/AutoFish/lang 下加载 YAML 词典，支持嵌入模板回落与缺省键回退。
/// </summary>
internal static class Lang
{
    private const string DefaultCulture = "zh-cn";

    private static readonly string LangDirectory = Path.Combine(TShock.SavePath, "AutoFish", "lang");

    private static readonly IDeserializer Deserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();

    private static readonly Dictionary<string, string> Strings = new(StringComparer.OrdinalIgnoreCase);

    public static string CurrentCulture { get; private set; } = DefaultCulture;

    /// <summary>
    ///     读取语言文件，缺失则从嵌入资源导出后再读取。
    /// </summary>
    public static void Load(string? culture = null)
    {
        var targetCulture = string.IsNullOrWhiteSpace(culture) ? DefaultCulture : culture.ToLowerInvariant();
        CurrentCulture = targetCulture;

        var langPath = Path.Combine(LangDirectory, $"{targetCulture}.yml");
        EnsureLangFileExists(targetCulture, langPath);

        var yaml = File.ReadAllText(langPath);
        var dict = Deserializer.Deserialize<Dictionary<string, string>>(yaml) ?? new Dictionary<string, string>();

        Strings.Clear();
        foreach (var kv in dict) Strings[kv.Key] = kv.Value;
    }

    /// <summary>
    ///     获取语言文本，不存在则返回 [key] 方便排查。
    /// </summary>
    public static string T(string key, params object[] args)
    {
        if (!Strings.TryGetValue(key, out var value)) return $"[{key}]";
        if (args == null || args.Length == 0) return value;
        return string.Format(CultureInfo.InvariantCulture, value, args);
    }

    private static void EnsureLangFileExists(string culture, string path)
    {
        Directory.CreateDirectory(LangDirectory);

        if (File.Exists(path)) return;

        if (TryExportEmbeddedLang(culture, path)) return;

        if (!culture.Equals(DefaultCulture, StringComparison.OrdinalIgnoreCase) &&
            TryExportEmbeddedLang(DefaultCulture, path))
        {
            return;
        }

        File.WriteAllText(path, $"common.enabled: 启用\ncommon.disabled: 禁用\n# missing language for {culture}\n");
    }

    private static bool TryExportEmbeddedLang(string culture, string path)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith($"lang.{culture}.yml", StringComparison.OrdinalIgnoreCase));

        if (resourceName == null) return false;

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null) return false;

        using var reader = new StreamReader(stream);
        var content = reader.ReadToEnd();
        File.WriteAllText(path, content);
        return true;
    }
}