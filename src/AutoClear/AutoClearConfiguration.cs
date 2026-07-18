using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TShockAPI;

namespace AutoClear
{
    public sealed class AutoClearConfiguration
    {
        private static readonly string[] LegacyConfigNames =
        {
            "WorldItemCleanupGuard.json",
            "Autoclear.zh-CN.json",
            "AutoClear.zh-CN.json",
            "Autoclear.en-US.json",
            "AutoClear.en-US.json",
        };

        [JsonProperty("启用自动清扫")]
        public bool EnableAutomaticSweep { get; set; } = true;

        [JsonProperty("检测间隔秒")]
        public int DetectionIntervalSeconds { get; set; } = 10;

        [JsonProperty("排除物品ID")]
        public List<int> NonSweepableItemIds { get; set; } = new List<int>();

        [JsonProperty("清扫阈值")]
        public int SmartSweepThreshold { get; set; } = 100;

        [JsonProperty("延迟清扫秒")]
        public int DelayedSweepTimeoutSeconds { get; set; } = 10;

        [JsonProperty("延迟清扫消息")]
        public string DelayedSweepCustomMessage { get; set; } = "世界物品已达到清扫阈值，将在 {0} 秒后进行安全清理。";

        [JsonProperty("清扫挥动武器")]
        public bool SweepSwinging { get; set; } = true;

        [JsonProperty("清扫投掷武器")]
        public bool SweepThrowable { get; set; } = true;

        [JsonProperty("清扫普通物品")]
        public bool SweepRegular { get; set; } = true;

        [JsonProperty("清扫装备")]
        public bool SweepEquipment { get; set; } = true;

        [JsonProperty("清扫时装")]
        public bool SweepVanity { get; set; } = true;

        [JsonProperty("完成清扫消息")]
        public string CustomMessage { get; set; } = string.Empty;

        [JsonProperty("显示分类统计")]
        public bool SpecificMessage { get; set; } = true;

        [JsonIgnore]
        internal HashSet<int> ExcludedItemIdSet { get; private set; } = new HashSet<int>();

        public static string ConfigPath => Path.Combine(TShock.SavePath, "AutoClear.json");

        internal static AutoClearConfiguration Load()
        {
            try
            {
                AutoClearConfiguration configuration;
                if (File.Exists(ConfigPath))
                {
                    JObject root = JObject.Parse(File.ReadAllText(ConfigPath, Encoding.UTF8));
                    configuration = FromLegacyObject(root);
                }
                else if (!TryImportLegacy(out configuration))
                {
                    configuration = new AutoClearConfiguration();
                }

                configuration.Normalize();
                configuration.Write();
                return configuration;
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError($"[AutoClear] Failed to load configuration: {ex}");
                AutoClearConfiguration fallback = new AutoClearConfiguration();
                fallback.Normalize();
                return fallback;
            }
        }

        internal void Normalize()
        {
            DetectionIntervalSeconds = Math.Max(1, DetectionIntervalSeconds);
            SmartSweepThreshold = Math.Max(1, SmartSweepThreshold);
            DelayedSweepTimeoutSeconds = Math.Max(0, DelayedSweepTimeoutSeconds);
            DelayedSweepCustomMessage ??= string.Empty;
            CustomMessage ??= string.Empty;
            NonSweepableItemIds = (NonSweepableItemIds ?? new List<int>())
                .Where(itemId => itemId > 0)
                .Distinct()
                .OrderBy(itemId => itemId)
                .ToList();
            ExcludedItemIdSet = new HashSet<int>(NonSweepableItemIds);
        }

        internal void Write()
        {
            string directory = Path.GetDirectoryName(ConfigPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(
                ConfigPath,
                JsonConvert.SerializeObject(this, Formatting.Indented),
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        }

        private static bool TryImportLegacy(out AutoClearConfiguration configuration)
        {
            configuration = null;
            foreach (string fileName in LegacyConfigNames)
            {
                string path = Path.Combine(TShock.SavePath, fileName);
                if (!File.Exists(path))
                {
                    continue;
                }

                JObject root = JObject.Parse(File.ReadAllText(path, Encoding.UTF8));
                configuration = FromLegacyObject(root);
                TShock.Log.ConsoleInfo(
                    $"[AutoClear] Imported legacy configuration from {path}");
                return true;
            }

            return false;
        }

        internal static AutoClearConfiguration FromLegacyObject(JObject root)
        {
            AutoClearConfiguration configuration = new AutoClearConfiguration();
            configuration.EnableAutomaticSweep = ReadValue(root, configuration.EnableAutomaticSweep,
                "启用自动清扫", "EnableAutomaticSweep");
            configuration.DetectionIntervalSeconds = ReadValue(root, configuration.DetectionIntervalSeconds,
                "检测间隔秒", "清理间隔", "多久检测一次(s)", "Interval");
            configuration.NonSweepableItemIds = ReadValue(root, configuration.NonSweepableItemIds,
                "排除物品ID", "排除列表", "不清扫的物品ID列表", "Exclude");
            configuration.SmartSweepThreshold = ReadValue(root, configuration.SmartSweepThreshold,
                "清扫阈值", "清理阈值", "智能清扫数量临界值", "Threshold");
            configuration.DelayedSweepTimeoutSeconds = ReadValue(root, configuration.DelayedSweepTimeoutSeconds,
                "延迟清扫秒", "延迟清扫", "延迟清扫(s)", "Dealy", "Delay");
            configuration.DelayedSweepCustomMessage = ReadValue(root, configuration.DelayedSweepCustomMessage,
                "延迟清扫消息", "延迟清扫自定义消息", "DealyMsg", "DelayMsg");
            configuration.SweepSwinging = ReadValue(root, configuration.SweepSwinging,
                "清扫挥动武器", "是否清扫挥动武器", "SweepSwinging");
            configuration.SweepThrowable = ReadValue(root, configuration.SweepThrowable,
                "清扫投掷武器", "是否清扫投掷武器", "SweepThrowable");
            configuration.SweepRegular = ReadValue(root, configuration.SweepRegular,
                "清扫普通物品", "是否清扫普通物品", "SweepRegular", "SweepRegaular");
            configuration.SweepEquipment = ReadValue(root, configuration.SweepEquipment,
                "清扫装备", "是否清扫装备", "SweepEquipment");
            configuration.SweepVanity = ReadValue(root, configuration.SweepVanity,
                "清扫时装", "是否清扫时装", "SweepVanity");
            configuration.CustomMessage = ReadValue(root, configuration.CustomMessage,
                "完成清扫消息", "完成清扫自定义消息", "SweepMsg");
            configuration.SpecificMessage = ReadValue(root, configuration.SpecificMessage,
                "显示分类统计", "清理提示", "具体消息", "SweepTip");
            configuration.Normalize();
            return configuration;
        }

        private static T ReadValue<T>(JObject root, T fallback, params string[] aliases)
        {
            foreach (string alias in aliases)
            {
                if (!root.TryGetValue(alias, StringComparison.OrdinalIgnoreCase, out JToken token))
                {
                    continue;
                }

                T value = token.ToObject<T>();
                return value != null ? value : fallback;
            }

            return fallback;
        }
    }
}
