using Newtonsoft.Json;
using TShockAPI;

namespace DM;

/// <summary>
/// 插件配置：两个商人各一个开关，默认都开。
///
/// 用仓库里通用的做法（参照 DamageRuleLoot）：配置文件放在 TShock 配置目录下，
/// 叫 DailyMerchant.json，不存在就按默认值写一份；改完重启服务器生效
/// （服务器上装了 TSConfig 的话也能走 TShock 的配置重载事件）。
/// </summary>
public class DailyMerchantConfig
{
    [JsonProperty("启用旅商每日到访", Order = 0)]
    public bool TravellingMerchantEnabled { get; set; } = true;

    [JsonProperty("启用骷髅商人每日到访", Order = 1)]
    public bool SkeletonMerchantEnabled { get; set; } = true;

    // ------------------------------------------------------------------ 读取与写入

    public static readonly string FilePath = Path.Combine(TShock.SavePath, "DailyMerchant.json");

    /// <summary>
    /// 读配置。任何一步出岔子（文件不存在、内容坏了、读不了、写不回）都退回默认值，
    /// 绝不让异常冒到插件初始化——否则一个坏掉的配置文件就能让整个服务器起不来。
    /// </summary>
    public static DailyMerchantConfig Read()
    {
        if (File.Exists(FilePath))
        {
            try
            {
                DailyMerchantConfig? loaded = JsonConvert.DeserializeObject<DailyMerchantConfig>(File.ReadAllText(FilePath));
                if (loaded != null)
                    return loaded;
            }
            catch (Exception ex) when (ex is JsonException or IOException or UnauthorizedAccessException)
            {
                // 内容损坏、被锁住或没有读权限：都落到下面按默认值重建。
            }
        }

        var config = new DailyMerchantConfig();

        try
        {
            config.Write();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            // 默认配置写不出去（例如配置目录只读）时，内存里的默认值照样能用，
            // 只是这次不会留下配置文件；不为此让服务器起不来。
        }

        return config;
    }

    public void Write() =>
        File.WriteAllText(FilePath, JsonConvert.SerializeObject(this, Formatting.Indented));
}