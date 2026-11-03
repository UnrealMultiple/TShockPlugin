using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TShockAPI;

namespace Starfall
{
    /// <summary>
    /// 插件配置。保存在 TShock 的 SavePath 下的 Starfall.json。
    /// JSON 中的键名全部使用中文（通过 JsonProperty 指定），C# 成员仍用英文以便阅读。
    /// </summary>
    public class StarfallConfig
    {
        // ===== 总开关 =====

        /// <summary>总开关。</summary>
        [JsonProperty("启用")]
        public bool Enabled = true;

        /// <summary>是否启用落星增强。</summary>
        [JsonProperty("启用落星")]
        public bool EnableStars = true;

        /// <summary>是否启用天气增强（下雨 / 风 / 沙尘暴 / 钱币雨 / 陨石雨）。</summary>
        [JsonProperty("启用天气增强")]
        public bool EnableWeather = true;

        // ===== 落星 =====

        /// <summary>落星出现频率倍率（指坠落动画/弹幕的生成速率）。1.0 约等于原版夜晚总量。</summary>
        [JsonProperty("落星频率倍率")]
        public double StarMultiplier = 2.0;

        /// <summary>白天是否也允许落星（原版仅夜晚掉落，默认关闭以遵循原版）。</summary>
        [JsonProperty("白天可落星")]
        public bool AllowDaytimeStars = false;

        /// <summary>白天频率相对夜晚的倍率（默认 0.1，即白天的 1/10）。</summary>
        [JsonProperty("白天频率倍率")]
        public double DaytimeMultiplier = 0.1;

        /// <summary>
        /// 落星弹幕（弹幕 type 12）的伤害。原版只有在伤害 &gt; 500 时才会落地掉落“坠落之星”，
        /// 为把掉落改由本插件控制，默认 500（即关闭原版掉落）。若调成 &gt; 500，原版会额外
        /// 掉落一个坠落之星，与本插件掉落表叠加。
        /// </summary>
        [JsonProperty("落星伤害")]
        public int Damage = 500;

        /// <summary>
        /// 落星落地/爆炸时的掉落表。按“权重”加权随机，权重不必凑满 100。
        /// 默认：50% 掉落 1 个坠落之星(75)、40% 掉落 1 个魔力水晶(109)、10% 掉落 1 个生命水晶(29)。
        /// </summary>
        // ObjectCreationHandling.Replace：反序列化时用文件内容整体替换默认列表，否则会在默认 3 条上追加。
        [JsonProperty("落星掉落表", ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public List<DropEntry> Drops = new List<DropEntry>
        {
            new DropEntry { ItemId = 75,  Stack = 1, Weight = 50.0 },
            new DropEntry { ItemId = 109, Stack = 1, Weight = 40.0 },
            new DropEntry { ItemId = 29,  Stack = 1, Weight = 10.0 },
        };

        // ===== 天气倍率 =====

        /// <summary>下雨出现频率倍率。</summary>
        [JsonProperty("下雨倍率")]
        public double RainMultiplier = 2.0;

        /// <summary>风力强度倍率（越大越容易出现“大风天”）。</summary>
        [JsonProperty("风倍率")]
        public double WindMultiplier = 2.0;

        /// <summary>沙尘暴出现频率倍率。</summary>
        [JsonProperty("沙尘暴倍率")]
        public double SandstormMultiplier = 2.0;

        /// <summary>钱币雨出现概率倍率（每场雨开始时，钱币雨跟随发生的概率；原版约 1/25，受幸运影响）。</summary>
        [JsonProperty("钱币雨概率倍率")]
        public double CoinRainChanceMultiplier = 2.0;

        /// <summary>钱币雨产出倍率（一场钱币雨的总落币“预算”）。</summary>
        [JsonProperty("钱币雨产出倍率")]
        public double CoinRainYieldMultiplier = 2.0;

        /// <summary>陨石雨出现概率倍率（原版每晚约 1/50 尝试坠星、失败再 1/3 转陨石雨 ≈ 1/150；需已击败克脑/世界吞噬者）。</summary>
        [JsonProperty("陨石雨概率倍率")]
        public double MeteorShowerChanceMultiplier = 2.0;

        /// <summary>陨石雨产出倍率（一场陨石雨的流星总数）。</summary>
        [JsonProperty("陨石雨产出倍率")]
        public double MeteorShowerYieldMultiplier = 2.0;

        /// <summary>把明显越界的值收敛到合理范围，避免手改配置把服务器搞崩。</summary>
        public void Normalize()
        {
            StarMultiplier = Clamp01(StarMultiplier, 10000.0);
            DaytimeMultiplier = Clamp01(DaytimeMultiplier, 10000.0);
            RainMultiplier = Clamp01(RainMultiplier, 1000.0);
            WindMultiplier = Clamp01(WindMultiplier, 100.0);
            SandstormMultiplier = Clamp01(SandstormMultiplier, 1000.0);
            CoinRainChanceMultiplier = Clamp01(CoinRainChanceMultiplier, 1000.0);
            CoinRainYieldMultiplier = Clamp01(CoinRainYieldMultiplier, 1000.0);
            MeteorShowerChanceMultiplier = Clamp01(MeteorShowerChanceMultiplier, 1000.0);
            MeteorShowerYieldMultiplier = Clamp01(MeteorShowerYieldMultiplier, 1000.0);

            if (Damage < 0) Damage = 0;
            if (Drops == null) Drops = new List<DropEntry>();
            foreach (var d in Drops)
            {
                if (d == null) continue;
                if (d.Stack < 1) d.Stack = 1;
                if (d.Weight < 0) d.Weight = 0;
            }
        }

        private static double Clamp01(double v, double max)
        {
            if (double.IsNaN(v) || v < 0) return 0;
            if (v > max) return max;
            return v;
        }

        public static StarfallConfig Load(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    var cfg = JsonConvert.DeserializeObject<StarfallConfig>(File.ReadAllText(path));
                    if (cfg != null)
                    {
                        cfg.Normalize();
                        return cfg;
                    }
                }
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError("[Starfall] 读取配置失败: " + ex.Message);
            }

            var fresh = new StarfallConfig();
            fresh.Save(path);
            return fresh;
        }

        public void Save(string path)
        {
            try
            {
                File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError("[Starfall] 保存配置失败: " + ex.Message);
            }
        }
    }

    /// <summary>掉落表条目。</summary>
    public class DropEntry
    {
        /// <summary>物品 ID。坠落之星=75、魔力水晶=109、生命水晶=29。</summary>
        [JsonProperty("物品ID")]
        public int ItemId;

        /// <summary>掉落数量。</summary>
        [JsonProperty("数量")]
        public int Stack = 1;

        /// <summary>相对权重（加权随机，不必凑满 100）。</summary>
        [JsonProperty("权重")]
        public double Weight = 1.0;
    }
}
