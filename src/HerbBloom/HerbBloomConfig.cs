using System;
using System.IO;
using Newtonsoft.Json;
using TShockAPI;

namespace HerbBloom
{
    /// <summary>
    /// HerbBloom 配置。JSON 键名全部使用中文（与 Starfall 等插件风格一致），
    /// C# 成员用英文便于阅读。文件位置：tshock/HerbBloom.json。
    /// </summary>
    public class HerbBloomConfig
    {
        /// <summary>总开关：关闭后插件完全不再改动草药（保留原版逻辑）。</summary>
        [JsonProperty("启用")]
        public bool Enabled = true;

        /// <summary>成熟草药立即开花，无视昼夜/天气/月相/深度的原版开花窗口。</summary>
        [JsonProperty("无视条件开花")]
        public bool BloomIgnoreConditions = true;

        /// <summary>已开花的草药不再回退到未开花（针对闪光根的随机回退、熔岩火焰花的水量切换）。</summary>
        [JsonProperty("开花后不回退")]
        public bool PreventUnbloom = true;

        /// <summary>true = 连幼年草也直接开花；false = 幼草按生长速度成熟后再开花（霜花式）。</summary>
        [JsonProperty("幼草直接开花")]
        public bool BloomImmatureHerbs = false;

        /// <summary>幼草成熟速度倍率；0 = 不额外加速（默认，保持原版速度），越大越快。</summary>
        [JsonProperty("生长速度倍率")]
        public double GrowthSpeedMultiplier = 0.0;

        /// <summary>每次扫描每株幼草成熟的基础概率（实际 = 基础 × 倍率，上限 1）。</summary>
        [JsonProperty("生长基础概率")]
        public double BaseGrowthChancePerScan = 0.2;

        /// <summary>扫描周期（秒）。</summary>
        [JsonProperty("扫描间隔秒")]
        public double ScanIntervalSeconds = 10.0;

        /// <summary>玩家周边即时处理半径（格）。生长加速只作用于玩家附近区域。</summary>
        [JsonProperty("玩家扫描半径")]
        public int PlayerScanRadiusTiles = 160;

        /// <summary>每个周期全世界推进扫描的列数（扫描完一轮自动从头再来）。</summary>
        [JsonProperty("全局扫描列数")]
        public int WorldSweepColumnsPerCycle = 600;

        /// <summary>每个游戏刻最多广播的 tile 改动数（限流，防止首次全图转换时刷包）。</summary>
        [JsonProperty("每刻最大同步数")]
        public int MaxTileSquaresPerTick = 1500;

        [JsonProperty("详细日志")]
        public bool VerboseLog = false;

        public static HerbBloomConfig Load(string path)
        {
            HerbBloomConfig cfg;
            if (!File.Exists(path))
            {
                cfg = new HerbBloomConfig();
                cfg.Save(path);
                return cfg;
            }
            try
            {
                cfg = JsonConvert.DeserializeObject<HerbBloomConfig>(File.ReadAllText(path)) ?? new HerbBloomConfig();
            }
            catch (Exception ex)
            {
                try
                {
                    File.Copy(path, path + ".broken-" + DateTime.Now.ToString("yyyyMMddHHmmss"), true);
                }
                catch
                {
                }
                TShock.Log.ConsoleError("[HerbBloom] 配置解析失败（已备份损坏文件并重建默认配置）：{0}", ex.Message);
                cfg = new HerbBloomConfig();
                cfg.Save(path);
            }
            cfg.Sanitize();
            return cfg;
        }

        public void Save(string path)
        {
            var dir = Path.GetDirectoryName(Path.GetFullPath(path));
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        /// <summary>把不合理的配置值约束到安全区间，避免手改坏配置导致异常。</summary>
        public void Sanitize()
        {
            GrowthSpeedMultiplier = Math.Max(0.0, Math.Min(1000.0, GrowthSpeedMultiplier));
            BaseGrowthChancePerScan = Math.Max(0.0, Math.Min(1.0, BaseGrowthChancePerScan));
            ScanIntervalSeconds = Math.Max(1.0, Math.Min(3600.0, ScanIntervalSeconds));
            PlayerScanRadiusTiles = Math.Max(16, Math.Min(1024, PlayerScanRadiusTiles));
            WorldSweepColumnsPerCycle = Math.Max(10, Math.Min(100000, WorldSweepColumnsPerCycle));
            MaxTileSquaresPerTick = Math.Max(1, Math.Min(20000, MaxTileSquaresPerTick));
        }

        /// <summary>按当前配置计算：每次扫描每株幼草的成熟概率。</summary>
        public double EffectiveGrowthChancePerScan()
            => Math.Max(0.0, Math.Min(1.0, BaseGrowthChancePerScan * Math.Max(0.0, GrowthSpeedMultiplier)));
    }
}
