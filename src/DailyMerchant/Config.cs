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

    public void Write() =>
        File.WriteAllText(FilePath, JsonConvert.SerializeObject(this, Formatting.Indented));

    /// <summary>读配置；文件不存在就按默认值写一份。</summary>
    public static DailyMerchantConfig Read()
    {
        if (!File.Exists(FilePath))
        {
            var config = new DailyMerchantConfig();
            config.Write();
            return config;
        }

        return JsonConvert.DeserializeObject<DailyMerchantConfig>(File.ReadAllText(FilePath)) ?? new DailyMerchantConfig();
    }
}