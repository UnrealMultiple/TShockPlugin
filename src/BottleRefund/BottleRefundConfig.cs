using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TShockAPI;

namespace BottleRefund
{
    /// <summary>
    /// BottleRefund 配置。JSON 键名全部中文，文件位置：tshock/BottleRefund.json。
    /// 修改后需重启服务器生效（无游戏内指令）。
    /// </summary>
    public class BottleRefundConfig
    {
        /// <summary>总开关。</summary>
        [JsonProperty("启用")]
        public bool Enabled = true;

        /// <summary>每喝一瓶药水的返还概率（0~1）。</summary>
        [JsonProperty("返还概率")]
        public double RefundChance = 1.0;

        /// <summary>每喝一瓶返还的数量。</summary>
        [JsonProperty("每次喝药返还数量")]
        public int RefundCountPerDrink = 1;

        /// <summary>返还的物品 ID（默认 31 = 玻璃瓶）。</summary>
        [JsonProperty("返还物品ID")]
        public int RefundItemId = 31;

        /// <summary>结算延迟（秒）：流水静默且最老一笔达到该时长后，全面结算并发放。</summary>
        [JsonProperty("结算延迟秒")]
        public double SettlementDelaySeconds = 8.0;

        /// <summary>不参与返还的物品 ID 列表（如不想给某类药水返瓶）。</summary>
        [JsonProperty("药水黑名单")]
        public List<int> Blacklist = new List<int>();

        /// <summary>单玩家流水上限：持续批量操作导致流水超限时整批放弃（不返还），防止饿死结算。</summary>
        [JsonProperty("流水上限")]
        public int LedgerLimit = 300;

        [JsonProperty("详细日志")]
        public bool VerboseLog = false;

        public static BottleRefundConfig Load(string path)
        {
            BottleRefundConfig cfg;
            if (!File.Exists(path))
            {
                cfg = new BottleRefundConfig();
                cfg.Save(path);
                return cfg;
            }
            try
            {
                cfg = JsonConvert.DeserializeObject<BottleRefundConfig>(File.ReadAllText(path)) ?? new BottleRefundConfig();
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
                TShock.Log.ConsoleError("[BottleRefund] 配置解析失败（已备份损坏文件并重建默认配置）：{0}", ex.Message);
                cfg = new BottleRefundConfig();
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

        public void Sanitize()
        {
            RefundChance = Math.Max(0.0, Math.Min(1.0, RefundChance));
            RefundCountPerDrink = Math.Max(0, Math.Min(999, RefundCountPerDrink));
            RefundItemId = Math.Max(1, Math.Min(10000, RefundItemId));
            SettlementDelaySeconds = Math.Max(1.0, Math.Min(300.0, SettlementDelaySeconds));
            LedgerLimit = Math.Max(10, Math.Min(100000, LedgerLimit));
            if (Blacklist == null)
                Blacklist = new List<int>();
            Blacklist.RemoveAll(id => id <= 0);
        }
    }
}
