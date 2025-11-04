using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using Newtonsoft.Json;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using Microsoft.Xna.Framework;
using Timer = System.Timers.Timer;

namespace XiuXianEconomy
{
    [ApiVersion(2, 1)]
    public class XiuXianEconomy : TerrariaPlugin
    {
        #region 基础配置
        private static readonly string ConfigPath = Path.Combine(TShock.SavePath, "XiuXianEconomy.json");
        private static readonly string DataPath = Path.Combine(TShock.SavePath, "XiuXianEconomyData.json");
        private static readonly string AuctionPath = Path.Combine(TShock.SavePath, "XiuXianAuction.json");
        private static readonly string TradePath = Path.Combine(TShock.SavePath, "XiuXianTrade.json");
        
        private Timer _topuiRefreshTimer;
        private Timer _chatuiRefreshTimer;
        private StatusManager statusManager;
        
        // 商店分页系统
        private static Dictionary<string, int> _playerShopPages = new Dictionary<string, int>();
        private const int ItemsPerPage = 10;
        
        // 前置插件实例
        private TerrariaPlugin _shouyuanPlugin;
        
        // 前置插件检查
        private bool CheckShouyuanPreReq()
        {
            try
            {
                var plugins = TerrariaApi.Server.ServerApi.Plugins;
                _shouyuanPlugin = plugins.FirstOrDefault(p => p.Plugin.Name == "XiuXianShouYuan" || p.Plugin.Name == "修仙星宿系统")?.Plugin;
                
                if (_shouyuanPlugin == null)
                {
                    TShock.Log.ConsoleError("【严重错误】修仙经济插件需要前置插件 XiuXianShouYuan.dll（修仙星宿系统）！");
                    TShock.Log.ConsoleError("请确保修仙主插件已正确安装并加载，然后重启服务器。");
                    return false;
                }
                
                TShock.Log.ConsoleInfo($"【成功】检测到前置插件: {_shouyuanPlugin.Name} v{_shouyuanPlugin.Version}");
                TShock.Log.ConsoleInfo("修仙经济系统已成功连接到修仙主插件！");
                return true;
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError($"检查前置插件时发生错误: {ex.Message}");
                return false;
            }
        }

        public override string Name => "修仙经济系统";
        public override string Author => "泷白";
        public override Version Version => new Version(1, 0, 1);
        public override string Description => "修仙插件经济附属系统，包含星晶货币、拍卖行、交易行和商店 - 需要XiuXianShouYuan前置";

        public XiuXianEconomy(Main game) : base(game)
        {
            Order = 2;
            statusManager = new StatusManager();
        }
        #endregion

        #region 星晶货币系统
        public enum StarCrystalGrade
        {
            下品 = 1,    // 绿色
            中品 = 2,    // 蓝色  
            上品 = 3,    // 紫色
            极品 = 4     // 金色
        }

        public class StarCrystal
        {
            public StarCrystalGrade Grade { get; set; }
            public int Amount { get; set; }
            public string ColorHex => Grade switch
            {
                StarCrystalGrade.下品 => "00FF00", // 绿色
                StarCrystalGrade.中品 => "0099FF", // 蓝色
                StarCrystalGrade.上品 => "CC00FF", // 紫色
                StarCrystalGrade.极品 => "FFD700", // 金色
                _ => "FFFFFF"
            };
            
            public string DisplayName => Grade switch
            {
                StarCrystalGrade.下品 => "下品星晶",
                StarCrystalGrade.中品 => "中品星晶", 
                StarCrystalGrade.上品 => "上品星晶",
                StarCrystalGrade.极品 => "极品星晶",
                _ => "未知星晶"
            };
            
            public int ToLowGradeValue()
            {
                return Grade switch
                {
                    StarCrystalGrade.下品 => Amount,
                    StarCrystalGrade.中品 => Amount * 100,
                    StarCrystalGrade.上品 => Amount * 10000,
                    StarCrystalGrade.极品 => Amount * 1000000,
                    _ => Amount
                };
            }
            
            public static StarCrystal FromLowGradeValue(int lowGradeValue)
            {
                var result = new Dictionary<StarCrystalGrade, int>();
                
                if (lowGradeValue >= 1000000)
                {
                    result[StarCrystalGrade.极品] = lowGradeValue / 1000000;
                    lowGradeValue %= 1000000;
                }
                if (lowGradeValue >= 10000)
                {
                    result[StarCrystalGrade.上品] = lowGradeValue / 10000;
                    lowGradeValue %= 10000;
                }
                if (lowGradeValue >= 100)
                {
                    result[StarCrystalGrade.中品] = lowGradeValue / 100;
                    lowGradeValue %= 100;
                }
                if (lowGradeValue > 0)
                {
                    result[StarCrystalGrade.下品] = lowGradeValue;
                }
                
                // 返回主要面额
                if (result.ContainsKey(StarCrystalGrade.极品))
                    return new StarCrystal { Grade = StarCrystalGrade.极品, Amount = result[StarCrystalGrade.极品] };
                else if (result.ContainsKey(StarCrystalGrade.上品))
                    return new StarCrystal { Grade = StarCrystalGrade.上品, Amount = result[StarCrystalGrade.上品] };
                else if (result.ContainsKey(StarCrystalGrade.中品))
                    return new StarCrystal { Grade = StarCrystalGrade.中品, Amount = result[StarCrystalGrade.中品] };
                else
                    return new StarCrystal { Grade = StarCrystalGrade.下品, Amount = result[StarCrystalGrade.下品] };
            }
        }

        public class EconomyData
        {
            public string PlayerName { get; set; }
            public Dictionary<StarCrystalGrade, int> StarCrystals { get; set; } = new Dictionary<StarCrystalGrade, int>();
            public List<AuctionItem> AuctionItems { get; set; } = new List<AuctionItem>();
            public List<TradeItem> TradeItems { get; set; } = new List<TradeItem>();
            public DateTime LastDailyReward { get; set; } = DateTime.MinValue;
            
            // 玩家击败的Boss记录
            public HashSet<string> DefeatedBosses { get; set; } = new HashSet<string>();
            
            public int GetTotalLowGradeValue()
            {
                int total = 0;
                foreach (var kvp in StarCrystals)
                {
                    var crystal = new StarCrystal { Grade = kvp.Key, Amount = kvp.Value };
                    total += crystal.ToLowGradeValue();
                }
                return total;
            }
            
            public void AddStarCrystals(StarCrystalGrade grade, int amount)
            {
                if (!StarCrystals.ContainsKey(grade))
                    StarCrystals[grade] = 0;
                StarCrystals[grade] += amount;
            }
            
            public bool RemoveStarCrystals(StarCrystalGrade grade, int amount)
            {
                if (!StarCrystals.ContainsKey(grade) || StarCrystals[grade] < amount)
                    return false;
                    
                StarCrystals[grade] -= amount;
                if (StarCrystals[grade] == 0)
                    StarCrystals.Remove(grade);
                    
                return true;
            }
            
            public string GetBalanceDisplay()
            {
                if (StarCrystals.Count == 0)
                    return "无星晶";
                    
                var display = new List<string>();
                foreach (var grade in Enum.GetValues(typeof(StarCrystalGrade)).Cast<StarCrystalGrade>().Reverse())
                {
                    if (StarCrystals.ContainsKey(grade) && StarCrystals[grade] > 0)
                    {
                        var crystal = new StarCrystal { Grade = grade, Amount = StarCrystals[grade] };
                        display.Add($"[c/{crystal.ColorHex}:{crystal.Amount}{GetGradeAbbr(grade)}]");
                    }
                }
                return string.Join(" ", display);
            }

            public bool CanReceiveDailyReward => LastDailyReward.Date < DateTime.Today;
            
            public bool HasDefeatedBoss(string bossName)
            {
                return DefeatedBosses.Contains(bossName);
            }
            
            public void RecordBossDefeat(string bossName)
            {
                DefeatedBosses.Add(bossName);
            }
        }

        public static Dictionary<string, EconomyData> EconomyPlayers = new Dictionary<string, EconomyData>();
        #endregion

        #region 拍卖行系统
        public class AuctionItem
        {
            public string Id { get; set; } = Guid.NewGuid().ToString();
            public string Seller { get; set; }
            public int ItemId { get; set; }
            public int Stack { get; set; }
            public byte Prefix { get; set; }
            public StarCrystalGrade PriceGrade { get; set; }
            public int PriceAmount { get; set; }
            public DateTime ListTime { get; set; } = DateTime.Now;
            public DateTime ExpireTime { get; set; } = DateTime.Now.AddDays(7);
            public string Buyer { get; set; }
            public bool IsSold { get; set; }
            public bool IsExpired => DateTime.Now > ExpireTime;
            
            public string GetPriceDisplay()
            {
                var crystal = new StarCrystal { Grade = PriceGrade, Amount = PriceAmount };
                return $"[c/{crystal.ColorHex}:{PriceAmount}{GetGradeAbbr(PriceGrade)}]";
            }
            
            public string GetItemDisplay()
            {
                var itemName = TShock.Utils.GetItemById(ItemId)?.Name ?? $"物品ID:{ItemId}";
                return $"{itemName} x{Stack}";
            }

            public string GetItemIcon()
            {
                return $"[i:{ItemId}]";
            }
        }

        public static List<AuctionItem> AuctionHouse = new List<AuctionItem>();
        
        private void InitializeAuction()
        {
            try
            {
                if (File.Exists(AuctionPath))
                {
                    var json = File.ReadAllText(AuctionPath);
                    AuctionHouse = JsonConvert.DeserializeObject<List<AuctionItem>>(json) ?? new List<AuctionItem>();
                    
                    // 清理过期的拍卖品
                    AuctionHouse.RemoveAll(a => a.IsExpired && !a.IsSold);
                    
                    TShock.Log.Info($"拍卖行数据已加载，共有 {AuctionHouse.Count} 个拍卖品");
                }
                else
                {
                    TShock.Log.Info("未找到拍卖行数据文件，将创建新的拍卖行");
                }
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"加载拍卖行数据失败: {ex.Message}");
                AuctionHouse = new List<AuctionItem>();
            }
        }
        
        private void SaveAuction()
        {
            try
            {
                File.WriteAllText(AuctionPath, JsonConvert.SerializeObject(AuctionHouse, Formatting.Indented));
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"保存拍卖行数据失败: {ex.Message}");
            }
        }
        #endregion

        #region 交易行系统
        public class TradeItem
        {
            public string Id { get; set; } = Guid.NewGuid().ToString();
            public string Seller { get; set; }
            public int ItemId { get; set; }
            public int Stack { get; set; }
            public byte Prefix { get; set; }
            public StarCrystalGrade PriceGrade { get; set; }
            public int PriceAmount { get; set; }
            public DateTime ListTime { get; set; } = DateTime.Now;
            public DateTime ExpireTime { get; set; } = DateTime.Now.AddDays(3); // 交易行有效期较短
            public string Buyer { get; set; }
            public bool IsSold { get; set; }
            public bool IsExpired => DateTime.Now > ExpireTime;
            
            public string GetPriceDisplay()
            {
                var crystal = new StarCrystal { Grade = PriceGrade, Amount = PriceAmount };
                return $"[c/{crystal.ColorHex}:{PriceAmount}{GetGradeAbbr(PriceGrade)}]";
            }
            
            public string GetItemDisplay()
            {
                var itemName = TShock.Utils.GetItemById(ItemId)?.Name ?? $"物品ID:{ItemId}";
                return $"{itemName} x{Stack}";
            }

            public string GetItemIcon()
            {
                return $"[i:{ItemId}]";
            }
        }

        public static List<TradeItem> TradeHouse = new List<TradeItem>();
        
        private void InitializeTrade()
        {
            try
            {
                if (File.Exists(TradePath))
                {
                    var json = File.ReadAllText(TradePath);
                    TradeHouse = JsonConvert.DeserializeObject<List<TradeItem>>(json) ?? new List<TradeItem>();
                    
                    // 清理过期的交易品
                    TradeHouse.RemoveAll(t => t.IsExpired && !t.IsSold);
                    
                    TShock.Log.Info($"交易行数据已加载，共有 {TradeHouse.Count} 个交易品");
                }
                else
                {
                    TShock.Log.Info("未找到交易行数据文件，将创建新的交易行");
                }
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"加载交易行数据失败: {ex.Message}");
                TradeHouse = new List<TradeItem>();
            }
        }
        
        private void SaveTrade()
        {
            try
            {
                File.WriteAllText(TradePath, JsonConvert.SerializeObject(TradeHouse, Formatting.Indented));
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"保存交易行数据失败: {ex.Message}");
            }
        }
        #endregion

        #region 商店系统 - 修正物品ID并添加更多物品
        public class ShopItem
        {
            [JsonProperty("物品ID")]
            public int ItemId { get; set; }
            
            [JsonProperty("库存")]
            public int Stock { get; set; } = -1; // -1 表示无限
            
            [JsonProperty("价格品质")]
            public StarCrystalGrade PriceGrade { get; set; }
            
            [JsonProperty("价格数量")]
            public int PriceAmount { get; set; }
            
            [JsonProperty("购买限制")]
            public int PurchaseLimit { get; set; } = -1; // -1 表示无限制
            
            [JsonProperty("玩家购买记录")]
            public Dictionary<string, int> PlayerPurchases { get; set; } = new Dictionary<string, int>();
            
            [JsonProperty("进度要求")]
            public string ProgressRequirement { get; set; } = "无";
            
            [JsonProperty("Boss要求")]
            public string BossRequirement { get; set; } = "无";
            
            public string GetPriceDisplay()
            {
                var crystal = new StarCrystal { Grade = PriceGrade, Amount = PriceAmount };
                return $"[c/{crystal.ColorHex}:{PriceAmount}{GetGradeAbbr(PriceGrade)}]";
            }
            
            public string GetItemDisplay()
            {
                var itemName = TShock.Utils.GetItemById(ItemId)?.Name ?? $"物品ID:{ItemId}";
                return $"{itemName}";
            }

            public string GetItemIcon()
            {
                return $"[i:{ItemId}]";
            }
            
            public bool CanPlayerPurchase(string playerName, int quantity = 1)
            {
                if (PurchaseLimit == -1) return true;
                
                if (!PlayerPurchases.ContainsKey(playerName))
                    PlayerPurchases[playerName] = 0;
                    
                return PlayerPurchases[playerName] + quantity <= PurchaseLimit;
            }
            
            public void RecordPurchase(string playerName, int quantity = 1)
            {
                if (PurchaseLimit == -1) return;
                
                if (!PlayerPurchases.ContainsKey(playerName))
                    PlayerPurchases[playerName] = 0;
                    
                PlayerPurchases[playerName] += quantity;
            }
            
            public bool MeetsProgressRequirement(EconomyData playerData)
            {
                if (BossRequirement == "无" || string.IsNullOrEmpty(BossRequirement))
                    return true;
                    
                return playerData.HasDefeatedBoss(BossRequirement);
            }
            
            public string GetRequirementDisplay()
            {
                if (BossRequirement == "无" || string.IsNullOrEmpty(BossRequirement))
                    return "";
                    
                return $"[需击败: {BossRequirement}]";
            }
        }

        public class EconomyConfig
        {
            [JsonProperty("商店物品")]
            public List<ShopItem> ShopItems { get; set; } = new List<ShopItem>();
            
            [JsonProperty("下品掉落概率")]
            public double DropChanceLow { get; set; } = 0.3;
            
            [JsonProperty("中品掉落概率")]
            public double DropChanceMedium { get; set; } = 0.15;
            
            [JsonProperty("上品掉落概率")]
            public double DropChanceHigh { get; set; } = 0.05;
            
            [JsonProperty("极品掉落概率")]
            public double DropChanceExtreme { get; set; } = 0.01;
            
            [JsonProperty("每日奖励下品")]
            public int DailyRewardLow { get; set; } = 10;
            
            [JsonProperty("每日奖励中品")]
            public int DailyRewardMedium { get; set; } = 5;
            
            [JsonProperty("每日奖励上品")]
            public int DailyRewardHigh { get; set; } = 2;
            
            [JsonProperty("每日奖励极品")]
            public int DailyRewardExtreme { get; set; } = 1;
            
            [JsonProperty("顶部ui偏移X")]
            public int TopuiOffsetX { get; set; } = -13;
            
            [JsonProperty("顶部ui偏移Y")]
            public int TopuiOffsetY { get; set; } = 12;
            
            [JsonProperty("聊天ui偏移X")]
            public int ChatuiOffsetX { get; set; } = 2;
            
            [JsonProperty("聊天ui偏移Y")]
            public int ChatuiOffsetY { get; set; } = 1;

            [JsonProperty("经济系统启用")]
            public bool EconomyEnabled { get; set; } = true;

            [JsonProperty("自动给予初始星晶")]
            public bool GiveInitialCrystals { get; set; } = true;

            [JsonProperty("初始星晶数量")]
            public int InitialCrystals { get; set; } = 100;

            [JsonProperty("顶部ui刷新频率毫秒")]
            public int TopuiRefreshInterval { get; set; } = 5000; // 默认5秒
            
            [JsonProperty("聊天ui刷新频率毫秒")]
            public int ChatuiRefreshInterval { get; set; } = 90000;
            
            public static EconomyConfig Instance;
            
            public static void Load(string path)
            {
                try
                {
                    if (File.Exists(path))
                    {
                        Instance = JsonConvert.DeserializeObject<EconomyConfig>(File.ReadAllText(path));
                        TShock.Log.Info($"经济配置已加载，商店物品: {Instance.ShopItems?.Count ?? 0} 个");
                    }
                    else
                    {
                        Instance = new EconomyConfig();
                        InitializeDefaultShop();
                        Save(path);
                        TShock.Log.Info("创建了默认经济配置文件");
                    }
                }
                catch (Exception ex)
                {
                    TShock.Log.Error($"加载经济配置失败: {ex.Message}");
                    Instance = new EconomyConfig();
                }
            }
            
            public static void Save(string path)
            {
                try
                {
                    File.WriteAllText(path, JsonConvert.SerializeObject(Instance, Formatting.Indented));
                    TShock.Log.Info("经济配置已保存");
                }
                catch (Exception ex)
                {
                    TShock.Log.Error($"保存经济配置失败: {ex.Message}");
                }
            }
            
            private static void InitializeDefaultShop()
            {
                // 从开荒到毕业的完整物品列表 - 修正了物品ID
                Instance.ShopItems.AddRange(new[]
                {
                    // === 开荒阶段 (无要求) ===
                    new ShopItem { ItemId = 7, Stock = -1, PriceGrade = StarCrystalGrade.下品, PriceAmount = 10, BossRequirement = "无" }, // 铁锤
                    new ShopItem { ItemId = 28, Stock = -1, PriceGrade = StarCrystalGrade.下品, PriceAmount = 5, BossRequirement = "无" },  // 弱效治疗药水
                    new ShopItem { ItemId = 8, Stock = -1, PriceGrade = StarCrystalGrade.下品, PriceAmount = 8, BossRequirement = "无" },  // 火把
                    new ShopItem { ItemId = 9, Stock = 10, PriceGrade = StarCrystalGrade.下品, PriceAmount = 15, PurchaseLimit = 1, BossRequirement = "无" }, // 木材
                    new ShopItem { ItemId = 6, Stock = -1, PriceGrade = StarCrystalGrade.下品, PriceAmount = 20, BossRequirement = "无" }, // 铁短剑
                    new ShopItem { ItemId = 1, Stock = -1, PriceGrade = StarCrystalGrade.下品, PriceAmount = 20, BossRequirement = "无" }, // 铁镐
                    new ShopItem { ItemId = 10, Stock = -1, PriceGrade = StarCrystalGrade.下品, PriceAmount = 25, BossRequirement = "无" }, // 铁斧
                    new ShopItem { ItemId = 39, Stock = -1, PriceGrade = StarCrystalGrade.下品, PriceAmount = 9, BossRequirement = "无" }, // 木弓
                    new ShopItem { ItemId = 40, Stock = -1, PriceGrade = StarCrystalGrade.下品, PriceAmount = 5, BossRequirement = "无" }, // 木箭
                    new ShopItem { ItemId = 2350, Stock = -1, PriceGrade = StarCrystalGrade.下品, PriceAmount = 15, BossRequirement = "无" }, // 回忆药水
                    
                    // === 基础材料 ===
                    new ShopItem { ItemId = 23, Stock = 50, PriceGrade = StarCrystalGrade.下品, PriceAmount = 2, BossRequirement = "无" }, // 凝胶
                    new ShopItem { ItemId = 12, Stock = 50, PriceGrade = StarCrystalGrade.下品, PriceAmount = 3, BossRequirement = "无" }, // 铜矿
                    new ShopItem { ItemId = 11, Stock = 50, PriceGrade = StarCrystalGrade.下品, PriceAmount = 3, BossRequirement = "无" }, // 铁矿
                    new ShopItem { ItemId = 14, Stock = 50, PriceGrade = StarCrystalGrade.下品, PriceAmount = 4, BossRequirement = "无" }, // 银矿
                    new ShopItem { ItemId = 13, Stock = 50, PriceGrade = StarCrystalGrade.下品, PriceAmount = 5, BossRequirement = "无" }, // 金矿
                    new ShopItem { ItemId = 75, Stock = 30, PriceGrade = StarCrystalGrade.中品, PriceAmount = 2, BossRequirement = "无" }, // 坠落之星
                    new ShopItem { ItemId = 56, Stock = 20, PriceGrade = StarCrystalGrade.中品, PriceAmount = 3, BossRequirement = "无" }, // 魔矿
                    new ShopItem { ItemId = 880, Stock = 20, PriceGrade = StarCrystalGrade.中品, PriceAmount = 3, BossRequirement = "无" }, // 猩红矿
                    
                    // === 克苏鲁之眼后 ===
                    new ShopItem { ItemId = 46, Stock = 5, PriceGrade = StarCrystalGrade.下品, PriceAmount = 100, BossRequirement = "克苏鲁之眼" }, // 魔光剑
                    new ShopItem { ItemId = 47, Stock = 5, PriceGrade = StarCrystalGrade.下品, PriceAmount = 80, BossRequirement = "克苏鲁之眼" }, // 邪箭
                    new ShopItem { ItemId = 104, Stock = 3, PriceGrade = StarCrystalGrade.下品, PriceAmount = 120, BossRequirement = "克苏鲁之眼" }, // 魔锤
                    new ShopItem { ItemId = 103, Stock = 3, PriceGrade = StarCrystalGrade.下品, PriceAmount = 120, BossRequirement = "克苏鲁之眼" }, // 梦魇镐
                    new ShopItem { ItemId = 45, Stock = 3, PriceGrade = StarCrystalGrade.下品, PriceAmount = 120, BossRequirement = "克苏鲁之眼" }, // 暗影战斧
                    new ShopItem { ItemId = 44, Stock = 5, PriceGrade = StarCrystalGrade.下品, PriceAmount = 90, BossRequirement = "克苏鲁之眼" }, // 恶魔弓
                    
                    // === 世界吞噬者/克苏鲁之脑后 ===
                    new ShopItem { ItemId = 86, Stock = 10, PriceGrade = StarCrystalGrade.中品, PriceAmount = 2, BossRequirement = "世界吞噬者" }, // 暗影鳞片
                    new ShopItem { ItemId = 1329, Stock = 10, PriceGrade = StarCrystalGrade.中品, PriceAmount = 2, BossRequirement = "克苏鲁之脑" }, // 组织样本
                    new ShopItem { ItemId = 102, Stock = 3, PriceGrade = StarCrystalGrade.中品, PriceAmount = 8, BossRequirement = "世界吞噬者" }, // 暗影鳞甲头盔
                    new ShopItem { ItemId = 101, Stock = 3, PriceGrade = StarCrystalGrade.中品, PriceAmount = 10, BossRequirement = "世界吞噬者" }, // 暗影鳞甲胸甲
                    new ShopItem { ItemId = 100, Stock = 3, PriceGrade = StarCrystalGrade.中品, PriceAmount = 8, BossRequirement = "世界吞噬者" }, // 暗影鳞甲护腿
                    new ShopItem { ItemId = 734, Stock = 3, PriceGrade = StarCrystalGrade.中品, PriceAmount = 15, BossRequirement = "世界吞噬者" }, // 虚空袋
                    
                    // === 骷髅王后 ===
                    new ShopItem { ItemId = 1273, Stock = 3, PriceGrade = StarCrystalGrade.中品, PriceAmount = 20, BossRequirement = "骷髅王" }, // 骷髅王之手
                    new ShopItem { ItemId = 2360, Stock = 3, PriceGrade = StarCrystalGrade.中品, PriceAmount = 25, BossRequirement = "骷髅王" }, // 鱼钩
                    new ShopItem { ItemId = 84, Stock = 5, PriceGrade = StarCrystalGrade.中品, PriceAmount = 30, BossRequirement = "骷髅王" }, // 钩爪
                    new ShopItem { ItemId = 1169, Stock = 3, PriceGrade = StarCrystalGrade.中品, PriceAmount = 40, BossRequirement = "骷髅王" }, // 骨头钥匙
                    
                    // === 困难模式前期 ===
                    new ShopItem { ItemId = 364, Stock = 20, PriceGrade = StarCrystalGrade.上品, PriceAmount = 1, BossRequirement = "血肉墙" }, // 钴矿石
                    new ShopItem { ItemId = 365, Stock = 20, PriceGrade = StarCrystalGrade.上品, PriceAmount = 2, BossRequirement = "血肉墙" }, // 秘银矿石
                    new ShopItem { ItemId = 366, Stock = 20, PriceGrade = StarCrystalGrade.上品, PriceAmount = 3, BossRequirement = "血肉墙" }, // 精金矿石
                    new ShopItem { ItemId = 367, Stock = 5, PriceGrade = StarCrystalGrade.上品, PriceAmount = 3, BossRequirement = "血肉墙" }, // 神锤
                    new ShopItem { ItemId = 426, Stock = 5, PriceGrade = StarCrystalGrade.上品, PriceAmount = 5, BossRequirement = "血肉墙" }, // 毁灭刀
                    new ShopItem { ItemId = 514, Stock = 3, PriceGrade = StarCrystalGrade.上品, PriceAmount = 8, BossRequirement = "血肉墙" }, // 激光步枪
                    new ShopItem { ItemId = 434, Stock = 3, PriceGrade = StarCrystalGrade.上品, PriceAmount = 10, BossRequirement = "血肉墙" }, // 发条式突击步枪
                    new ShopItem { ItemId = 779, Stock = 5, PriceGrade = StarCrystalGrade.上品, PriceAmount = 6, BossRequirement = "血肉墙" }, // 环境改造枪
                    new ShopItem { ItemId = 780, Stock = 20, PriceGrade = StarCrystalGrade.上品, PriceAmount = 1, BossRequirement = "血肉墙" }, // 绿溶液
                    
                    // === 机械三王后 ===
                    new ShopItem { ItemId = 1225, Stock = 15, PriceGrade = StarCrystalGrade.上品, PriceAmount = 2, BossRequirement = "机械蠕虫" }, // 神圣锭
                    new ShopItem { ItemId = 1225, Stock = 15, PriceGrade = StarCrystalGrade.上品, PriceAmount = 2, BossRequirement = "机械骷髅王" }, // 神圣锭
                    new ShopItem { ItemId = 1225, Stock = 15, PriceGrade = StarCrystalGrade.上品, PriceAmount = 2, BossRequirement = "机械双子眼" }, // 神圣锭
                    new ShopItem { ItemId = 553, Stock = 5, PriceGrade = StarCrystalGrade.上品, PriceAmount = 25, BossRequirement = "机械三王" }, // 神圣头盔
                    new ShopItem { ItemId = 551, Stock = 5, PriceGrade = StarCrystalGrade.上品, PriceAmount = 30, BossRequirement = "机械三王" }, // 神圣板甲
                    new ShopItem { ItemId = 552, Stock = 5, PriceGrade = StarCrystalGrade.上品, PriceAmount = 25, BossRequirement = "机械三王" }, // 神圣护胫
                    new ShopItem { ItemId = 493, Stock = 3, PriceGrade = StarCrystalGrade.上品, PriceAmount = 40, BossRequirement = "机械三王" }, // 天使之翼
                    new ShopItem { ItemId = 561, Stock = 5, PriceGrade = StarCrystalGrade.上品, PriceAmount = 15, BossRequirement = "机械三王" }, // 光辉飞盘
                    
                    // === 世纪之花后 ===
                    new ShopItem { ItemId = 757, Stock = 3, PriceGrade = StarCrystalGrade.上品, PriceAmount = 50, BossRequirement = "世纪之花" }, // 泰拉刃
                    new ShopItem { ItemId = 1316, Stock = 3, PriceGrade = StarCrystalGrade.上品, PriceAmount = 35, BossRequirement = "世纪之花" }, // 海龟头盔
                    new ShopItem { ItemId = 1317, Stock = 3, PriceGrade = StarCrystalGrade.上品, PriceAmount = 40, BossRequirement = "世纪之花" }, // 海龟铠甲
                    new ShopItem { ItemId = 1318, Stock = 3, PriceGrade = StarCrystalGrade.上品, PriceAmount = 35, BossRequirement = "世纪之花" }, // 海龟护腿
                    new ShopItem { ItemId = 1327, Stock = 3, PriceGrade = StarCrystalGrade.上品, PriceAmount = 45, BossRequirement = "世纪之花" }, // 死神镰刀
                    new ShopItem { ItemId = 1006, Stock = 5, PriceGrade = StarCrystalGrade.上品, PriceAmount = 20, BossRequirement = "世纪之花" }, // 叶绿锭
                    
                    // === 石巨人后 ===
                    new ShopItem { ItemId = 2199, Stock = 3, PriceGrade = StarCrystalGrade.极品, PriceAmount = 2, BossRequirement = "石巨人" }, // 甲虫头盔
                    new ShopItem { ItemId = 2200, Stock = 3, PriceGrade = StarCrystalGrade.极品, PriceAmount = 3, BossRequirement = "石巨人" }, // 甲虫胸甲
                    new ShopItem { ItemId = 2202, Stock = 3, PriceGrade = StarCrystalGrade.极品, PriceAmount = 2, BossRequirement = "石巨人" }, // 甲虫护腿
                    new ShopItem { ItemId = 899, Stock = 3, PriceGrade = StarCrystalGrade.极品, PriceAmount = 4, BossRequirement = "石巨人" }, // 太阳石
                    new ShopItem { ItemId = 900, Stock = 3, PriceGrade = StarCrystalGrade.极品, PriceAmount = 4, BossRequirement = "石巨人" }, // 月亮石
                    new ShopItem { ItemId = 2280, Stock = 5, PriceGrade = StarCrystalGrade.极品, PriceAmount = 8, BossRequirement = "石巨人" }, // 甲虫翼
                    
                    // === 月亮领主 (毕业) ===
                    new ShopItem { ItemId = 4956, Stock = 1, PriceGrade = StarCrystalGrade.极品, PriceAmount = 50, PurchaseLimit = 1, BossRequirement = "月亮领主" }, // 天顶剑
                    new ShopItem { ItemId = 3389, Stock = 1, PriceGrade = StarCrystalGrade.极品, PriceAmount = 40, PurchaseLimit = 1, BossRequirement = "月亮领主" }, // 泰拉悠悠球
                    new ShopItem { ItemId = 3531, Stock = 1, PriceGrade = StarCrystalGrade.极品, PriceAmount = 45, PurchaseLimit = 1, BossRequirement = "月亮领主" }, // 星尘龙法杖
                    new ShopItem { ItemId = 3469, Stock = 1, PriceGrade = StarCrystalGrade.极品, PriceAmount = 35, PurchaseLimit = 1, BossRequirement = "月亮领主" }, // 星璇强化翼
                    new ShopItem { ItemId = 4954, Stock = 1, PriceGrade = StarCrystalGrade.极品, PriceAmount = 30, PurchaseLimit = 1, BossRequirement = "月亮领主" }, // 天界星盘
                    new ShopItem { ItemId = 5005, Stock = 1, PriceGrade = StarCrystalGrade.极品, PriceAmount = 60, PurchaseLimit = 1, BossRequirement = "月亮领主" }, // 泰拉棱镜
                    new ShopItem { ItemId = 3460, Stock = 10, PriceGrade = StarCrystalGrade.极品, PriceAmount = 5, BossRequirement = "月亮领主" }, // 夜明矿
                    new ShopItem { ItemId = 3467, Stock = 5, PriceGrade = StarCrystalGrade.极品, PriceAmount = 10, BossRequirement = "月亮领主" }, // 夜明锭
                    new ShopItem { ItemId = 3458, Stock = 3, PriceGrade = StarCrystalGrade.极品, PriceAmount = 25, BossRequirement = "月亮领主" }, // 日耀碎片
                    new ShopItem { ItemId = 3456, Stock = 3, PriceGrade = StarCrystalGrade.极品, PriceAmount = 25, BossRequirement = "月亮领主" }, // 星旋碎片
                    new ShopItem { ItemId = 3457, Stock = 3, PriceGrade = StarCrystalGrade.极品, PriceAmount = 25, BossRequirement = "月亮领主" }, // 星云碎片
                    new ShopItem { ItemId = 3459, Stock = 3, PriceGrade = StarCrystalGrade.极品, PriceAmount = 25, BossRequirement = "月亮领主" }, // 星尘碎片
                    
                    // === 特殊和稀有物品 ===
                    new ShopItem { ItemId = 74, Stock = 1, PriceGrade = StarCrystalGrade.极品, PriceAmount = 100, PurchaseLimit = 1, BossRequirement = "月亮领主" }, // 铂金币
                    new ShopItem { ItemId = 29, Stock = 1, PriceGrade = StarCrystalGrade.上品, PriceAmount = 80, PurchaseLimit = 1, BossRequirement = "无" }, // 生命水晶
                    new ShopItem { ItemId = 502, Stock = 5, PriceGrade = StarCrystalGrade.上品, PriceAmount = 15, BossRequirement = "血肉墙" }, // 水晶碎块
                    new ShopItem { ItemId = 116, Stock = 10, PriceGrade = StarCrystalGrade.中品, PriceAmount = 5, BossRequirement = "克苏鲁之眼" }, // 陨石
                    new ShopItem { ItemId = 331, Stock = 3, PriceGrade = StarCrystalGrade.上品, PriceAmount = 20, BossRequirement = "世纪之花" }, // 丛林孢子
                    new ShopItem { ItemId = 209, Stock = 3, PriceGrade = StarCrystalGrade.上品, PriceAmount = 25, BossRequirement = "世纪之花" }, // 毒刺
                    new ShopItem { ItemId = 173, Stock = 5, PriceGrade = StarCrystalGrade.中品, PriceAmount = 10, BossRequirement = "骷髅王" }, // 黑曜石
                    new ShopItem { ItemId = 169, Stock = 10, PriceGrade = StarCrystalGrade.中品, PriceAmount = 8, BossRequirement = "骷髅王" }, // 沙块
                    new ShopItem { ItemId = 1246, Stock = 10, PriceGrade = StarCrystalGrade.中品, PriceAmount = 12, BossRequirement = "骷髅王" }, // 珍珠沙块
                });
            }
        }
        #endregion

        #region Boss进度系统
        private Dictionary<string, string> BossNames = new Dictionary<string, string>
        {
            { "克苏鲁之眼", "Eye of Cthulhu" },
            { "世界吞噬者", "Eater of Worlds" },
            { "克苏鲁之脑", "Brain of Cthulhu" },
            { "蜂王", "Queen Bee" },
            { "骷髅王", "Skeletron" },
            { "血肉墙", "Wall of Flesh" },
            { "机械蠕虫", "The Destroyer" },
            { "机械骷髅王", "Skeletron Prime" },
            { "机械双子眼", "The Twins" },
            { "世纪之花", "Plantera" },
            { "石巨人", "Golem" },
            { "猪龙鱼公爵", "Duke Fishron" },
            { "月亮领主", "Moon Lord" }
        };

        private void OnNpcKilled(NpcKilledEventArgs args)
        {
            try
            {
                // 检查经济系统是否启用
                if (!EconomyConfig.Instance.EconomyEnabled)
                    return;

                NPC npc = args.npc;
                var npcName = npc.GivenOrTypeName;
                
                // 检查是否是Boss
                var bossEntry = BossNames.FirstOrDefault(x => x.Value == npcName);
                if (bossEntry.Key != null)
                {
                    // 找到击杀Boss的玩家
                    var players = TShock.Players.Where(p => p != null && p.Active && p.IsLoggedIn).ToList();
                    if (players.Count > 0)
                    {
                        // 记录所有在线玩家击败了该Boss（简化处理，实际应该记录造成伤害的玩家）
                        foreach (var player in players)
                        {
                            var data = GetPlayerEconomy(player.Name);
                            data.RecordBossDefeat(bossEntry.Key);
                            player.SendSuccessMessage($"恭喜！你已击败 {bossEntry.Key}，解锁了新的商店物品！");
                        }
                        
                        TShock.Log.Info($"玩家们击败了 {bossEntry.Key}，已记录进度");
                    }
                }

                // 原有的掉落处理 - 修复这里的问题
                var onlinePlayers = TShock.Players.Where(p => p != null && p.Active && p.IsLoggedIn).ToList();
                if (onlinePlayers.Count > 0)
                {
                    var luckyPlayer = onlinePlayers[new Random().Next(onlinePlayers.Count)];
                    ProcessMonsterDrop(luckyPlayer, npc);
                }
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"处理怪物掉落失败: {ex.Message}");
            }
        }
        
        private void ProcessMonsterDrop(TSPlayer player, NPC npc)
        {
            var random = new Random();
            var config = EconomyConfig.Instance;
            
            // 根据NPC的强度决定掉落品质
            double powerFactor = npc.lifeMax / 100.0;
            powerFactor = Math.Min(powerFactor, 10.0); // 限制最大系数
            
            // 计算掉落几率
            double lowChance = config.DropChanceLow * powerFactor;
            double mediumChance = config.DropChanceMedium * powerFactor;
            double highChance = config.DropChanceHigh * powerFactor;
            double extremeChance = config.DropChanceExtreme * powerFactor;
            
            StarCrystalGrade? dropGrade = null;
            int dropAmount = 1;
            
            double roll = random.NextDouble();
            
            if (roll < extremeChance)
            {
                dropGrade = StarCrystalGrade.极品;
                dropAmount = random.Next(1, 3);
            }
            else if (roll < highChance)
            {
                dropGrade = StarCrystalGrade.上品;
                dropAmount = random.Next(1, 5);
            }
            else if (roll < mediumChance)
            {
                dropGrade = StarCrystalGrade.中品;
                dropAmount = random.Next(1, 10);
            }
            else if (roll < lowChance)
            {
                dropGrade = StarCrystalGrade.下品;
                dropAmount = random.Next(1, 20);
            }
            
            if (dropGrade.HasValue)
            {
                var data = GetPlayerEconomy(player.Name);
                data.AddStarCrystals(dropGrade.Value, dropAmount);
                
                var crystal = new StarCrystal { Grade = dropGrade.Value, Amount = dropAmount };
                player.SendMessage($"★★★ 击败 {npc.GivenOrTypeName} 获得 [c/{crystal.ColorHex}:{dropAmount}{GetGradeAbbr(dropGrade.Value)}品星晶]！ ★★★", Color.Yellow);
                
                UpdateEconomyui(player, data);
                SavePlayerEconomy(player.Name);
            }
        }
        
        // 辅助方法 - 获取品级缩写
        private static string GetGradeAbbr(StarCrystalGrade grade)
        {
            return grade switch
            {
                StarCrystalGrade.下品 => "下",
                StarCrystalGrade.中品 => "中", 
                StarCrystalGrade.上品 => "上",
                StarCrystalGrade.极品 => "极",
                _ => "?"
            };
        }
        #endregion

        #region 数据管理
        public static EconomyData GetPlayerEconomy(string name)
        {
            if (!EconomyPlayers.TryGetValue(name, out EconomyData data))
            {
                data = new EconomyData { PlayerName = name };
                
                // 如果配置允许，给予初始星晶
                if (EconomyConfig.Instance.GiveInitialCrystals)
                {
                    data.StarCrystals[StarCrystalGrade.下品] = EconomyConfig.Instance.InitialCrystals;
                }
                
                EconomyPlayers[name] = data;
            }
            return data;
        }

        public static void LoadEconomy(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    EconomyPlayers = JsonConvert.DeserializeObject<Dictionary<string, EconomyData>>(File.ReadAllText(path))
                                   ?? new Dictionary<string, EconomyData>();
                    TShock.Log.Info($"经济数据已加载，共有 {EconomyPlayers.Count} 名玩家的经济数据");
                }
                else
                {
                    TShock.Log.Info("未找到经济数据文件，将创建新的经济数据");
                }
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"加载经济数据失败: {ex.Message}");
            }
        }

        public static void SaveEconomy(string path)
        {
            try
            {
                File.WriteAllText(path, JsonConvert.SerializeObject(EconomyPlayers, Formatting.Indented));
                TShock.Log.Info($"经济数据已保存，共有 {EconomyPlayers.Count} 名玩家的经济数据");
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"保存经济数据失败: {ex.Message}");
            }
        }

        public static void SavePlayerEconomy(string name)
        {
            try
            {
                if (EconomyPlayers.ContainsKey(name))
                    SaveEconomy(DataPath);
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"保存玩家 {name} 经济数据失败: {ex.Message}");
            }
        }
        #endregion

        #region ui系统
        private void InitializeuiTimers()
        {
            // 初始化顶部UI定时器
            if (_topuiRefreshTimer != null)
            {
                _topuiRefreshTimer.Stop();
                _topuiRefreshTimer.Dispose();
            }
            
            _topuiRefreshTimer = new Timer(EconomyConfig.Instance.TopuiRefreshInterval);
            _topuiRefreshTimer.AutoReset = true;
            _topuiRefreshTimer.Elapsed += RefreshTopuiForAllPlayers;
            _topuiRefreshTimer.Start();
            
            // 初始化聊天UI定时器
            if (_chatuiRefreshTimer != null)
            {
                _chatuiRefreshTimer.Stop();
                _chatuiRefreshTimer.Dispose();
            }
            
            _chatuiRefreshTimer = new Timer(EconomyConfig.Instance.ChatuiRefreshInterval);
            _chatuiRefreshTimer.AutoReset = true;
            _chatuiRefreshTimer.Elapsed += RefreshChatuiForAllPlayers;
            _chatuiRefreshTimer.Start();
            
            TShock.Log.Info($"UI定时器已初始化 - 顶部UI: {EconomyConfig.Instance.TopuiRefreshInterval}ms, 聊天UI: {EconomyConfig.Instance.ChatuiRefreshInterval}ms");
        }
        
        private void UpdateEconomyui(TSPlayer player, EconomyData data)
        {
            UpdateChatui(player, data);
            UpdateTopui(player, data);
        }
        
        private void UpdateChatui(TSPlayer player, EconomyData data)
        {
            var sb = new System.Text.StringBuilder();
            var config = EconomyConfig.Instance;
            
            int offsetX = config.ChatuiOffsetX;
            int offsetY = config.ChatuiOffsetY;
            
            if (offsetY > 0)
            {
                sb.Append(new string('\n', offsetY));
            }
            
            string xOffset = (offsetX > 0) ? new string(' ', offsetX) : "";
            
            // 精简显示，避免重叠
            sb.Append($"{xOffset}星晶: {data.GetBalanceDisplay()}");
            
            // 只在可领取时显示每日奖励提示
            if (data.CanReceiveDailyReward)
            {
                sb.Append($" {xOffset}[每日奖励:可领取]");
            }
            
            player.SendMessage(sb.ToString(), Color.LightGreen);
        }
        
        private void UpdateTopui(TSPlayer player, EconomyData data)
        {
            var sb = new System.Text.StringBuilder();
            var config = EconomyConfig.Instance;

            if (config.TopuiOffsetY > 0)
            {
                sb.Append(new string('\n', config.TopuiOffsetY));
            }

            int absXOffset = Math.Abs(config.TopuiOffsetX);
            string xOffset = new string(' ', absXOffset);

            Func<string, string> applyXOffset = line =>
            {
                if (config.TopuiOffsetX < 0)
                    return line + xOffset;
                else
                    return xOffset + line;
            };

            // 使用与修仙插件一致的顶部ui风格
            sb.AppendLine(applyXOffset($"星晶货币: {data.GetBalanceDisplay()}".Color(Color.LightSkyBlue)));
            
            // 显示财富等级
            int totalValue = data.GetTotalLowGradeValue();
            string wealthLevel = totalValue switch
            {
                > 1000000 => "[c/FFD700:富可敌国]",
                > 100000 => "[c/CC00FF:家财万贯]", 
                > 10000 => "[c/0099FF:小有资产]",
                > 1000 => "[c/00FF00:温饱有余]",
                _ => "[c/AAAAAA:一贫如洗]"
            };
            
            sb.AppendLine(applyXOffset($"财富等级: {wealthLevel}".Color(Color.LightGreen)));

            // 显示市场信息
            int activeAuctions = AuctionHouse.Count(a => !a.IsSold && !a.IsExpired);
            int activeTrades = TradeHouse.Count(t => !t.IsSold && !t.IsExpired);
            
            if (activeAuctions > 0 || activeTrades > 0)
            {
                sb.AppendLine(applyXOffset($"市场: 拍{activeAuctions} 交{activeTrades}".Color(Color.Orange)));
            }

            // 只在可领取时显示每日奖励状态
            if (data.CanReceiveDailyReward)
            {
                sb.AppendLine(applyXOffset("每日奖励: [c/00FF00:可领取]".Color(Color.Yellow)));
            }

            statusManager.AddOrUpdateText(player, "top_economy_info", sb.ToString().TrimEnd());
        }
        
        private void RefreshTopuiForAllPlayers(object sender, ElapsedEventArgs e)
        {
            try
            {
                // 检查经济系统是否启用
                if (!EconomyConfig.Instance.EconomyEnabled)
                    return;

                foreach (var player in TShock.Players.Where(p => p != null && p.Active && p.IsLoggedIn))
                {
                    try
                    {
                        var data = GetPlayerEconomy(player.Name);
                        UpdateTopui(player, data);
                    }
                    catch (Exception ex)
                    {
                        TShock.Log.Error($"刷新 {player.Name} 的顶部UI失败: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"刷新顶部UI失败: {ex.Message}");
            }
        }
        
        private void RefreshChatuiForAllPlayers(object sender, ElapsedEventArgs e)
        {
            try
            {
                // 检查经济系统是否启用
                if (!EconomyConfig.Instance.EconomyEnabled)
                    return;

                foreach (var player in TShock.Players.Where(p => p != null && p.Active && p.IsLoggedIn))
                {
                    try
                    {
                        var data = GetPlayerEconomy(player.Name);
                        UpdateChatui(player, data);
                    }
                    catch (Exception ex)
                    {
                        TShock.Log.Error($"刷新 {player.Name} 的聊天UI失败: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"刷新聊天UI失败: {ex.Message}");
            }
        }
        #endregion

        #region 商店分页系统
        private void OpenShop(CommandArgs args)
        {
            if (!EconomyConfig.Instance.EconomyEnabled)
            {
                args.Player.SendErrorMessage("经济系统当前已关闭");
                return;
            }

            var shopItems = EconomyConfig.Instance.ShopItems;
            if (shopItems.Count == 0)
            {
                args.Player.SendErrorMessage("商店暂无商品！");
                return;
            }
            
            // 获取或设置玩家当前页码
            int currentPage = 1;
            if (args.Parameters.Count > 0)
            {
                if (!int.TryParse(args.Parameters[0], out currentPage) || currentPage < 1)
                {
                    args.Player.SendErrorMessage("无效的页码！");
                    return;
                }
            }
            
            _playerShopPages[args.Player.Name] = currentPage;
            DisplayShopPage(args.Player, currentPage);
        }
        
        private void DisplayShopPage(TSPlayer player, int page)
        {
            var shopItems = EconomyConfig.Instance.ShopItems;
            var playerData = GetPlayerEconomy(player.Name);
            
            // 计算分页
            int totalPages = (int)Math.Ceiling(shopItems.Count / (double)ItemsPerPage);
            if (page > totalPages)
            {
                player.SendErrorMessage($"页码超出范围！最大页码为 {totalPages}");
                return;
            }
            
            int startIndex = (page - 1) * ItemsPerPage;
            int endIndex = Math.Min(startIndex + ItemsPerPage, shopItems.Count);
            var pageItems = shopItems.Skip(startIndex).Take(ItemsPerPage).ToList();
            
            player.SendMessage($"══════════ 星晶商店 (第 {page}/{totalPages} 页) ══════════", Color.Gold);
            
            int availableItems = 0;
            for (int i = 0; i < pageItems.Count; i++)
            {
                var item = pageItems[i];
                var globalIndex = startIndex + i + 1;
                
                // 检查玩家是否满足购买条件
                bool canBuy = item.MeetsProgressRequirement(playerData);
                var itemName = TShock.Utils.GetItemById(item.ItemId)?.Name ?? $"物品ID:{item.ItemId}";
                string stockInfo = item.Stock == -1 ? "无限" : $"剩余:{item.Stock}";
                string limitInfo = item.PurchaseLimit == -1 ? "" : $", 限购:{item.PurchaseLimit}";
                string requirementInfo = item.GetRequirementDisplay();
                
                // 显示物品图标和状态
                string statusIcon = canBuy ? "✅" : "❌";
                player.SendMessage($"{globalIndex}. {statusIcon} {item.GetItemIcon()} {itemName} - {item.GetPriceDisplay()} ({stockInfo}{limitInfo}) {requirementInfo}", 
                    canBuy ? Color.White : Color.Gray);
                
                if (canBuy) availableItems++;
            }
            
            player.SendMessage($"本页可用商品: {availableItems}/{pageItems.Count} (✅可购买 ❌未解锁)", Color.Yellow);
            player.SendMessage("使用 /购买 <编号> [数量] 购买商品", Color.Yellow);
            player.SendMessage("使用 /翻页 <页码> 翻页 或 /翻页 下一页 /翻页 上一页", Color.Yellow);
            player.SendMessage("使用 /查看进度 查看已击败的Boss", Color.Yellow);
            player.SendMessage("══════════════════════════════════════════", Color.Gold);
        }
        
        private void ShopNextPage(CommandArgs args)
        {
            if (!EconomyConfig.Instance.EconomyEnabled)
            {
                args.Player.SendErrorMessage("经济系统当前已关闭");
                return;
            }

            var shopItems = EconomyConfig.Instance.ShopItems;
            int totalPages = (int)Math.Ceiling(shopItems.Count / (double)ItemsPerPage);
            
            // 获取当前页码
            int currentPage = 1;
            if (_playerShopPages.ContainsKey(args.Player.Name))
            {
                currentPage = _playerShopPages[args.Player.Name];
            }
            
            // 计算下一页
            int nextPage = currentPage + 1;
            if (nextPage > totalPages)
            {
                args.Player.SendErrorMessage("已经是最后一页了！");
                return;
            }
            
            _playerShopPages[args.Player.Name] = nextPage;
            DisplayShopPage(args.Player, nextPage);
        }
        
        private void ShopPrevPage(CommandArgs args)
        {
            if (!EconomyConfig.Instance.EconomyEnabled)
            {
                args.Player.SendErrorMessage("经济系统当前已关闭");
                return;
            }

            // 获取当前页码
            int currentPage = 1;
            if (_playerShopPages.ContainsKey(args.Player.Name))
            {
                currentPage = _playerShopPages[args.Player.Name];
            }
            
            // 计算上一页
            int prevPage = currentPage - 1;
            if (prevPage < 1)
            {
                args.Player.SendErrorMessage("已经是第一页了！");
                return;
            }
            
            _playerShopPages[args.Player.Name] = prevPage;
            DisplayShopPage(args.Player, prevPage);
        }
        
        private void ShopPage(CommandArgs args)
        {
            if (!EconomyConfig.Instance.EconomyEnabled)
            {
                args.Player.SendErrorMessage("经济系统当前已关闭");
                return;
            }

            if (args.Parameters.Count < 1)
            {
                args.Player.SendErrorMessage("用法: /翻页 <页码>");
                return;
            }
            
            if (!int.TryParse(args.Parameters[0], out int page) || page < 1)
            {
                args.Player.SendErrorMessage("无效的页码！");
                return;
            }
            
            _playerShopPages[args.Player.Name] = page;
            DisplayShopPage(args.Player, page);
        }
        #endregion

        #region 命令系统
        private void InitializeCommands()
        {
            Commands.ChatCommands.Add(new Command("economy.player", CheckBalance, "星晶余额"));
            Commands.ChatCommands.Add(new Command("economy.player", DailyReward, "每日奖励"));
            Commands.ChatCommands.Add(new Command("economy.player", OpenShop, "星晶商店"));
            Commands.ChatCommands.Add(new Command("economy.player", ShopNextPage, "翻页 下一页"));
            Commands.ChatCommands.Add(new Command("economy.player", ShopPrevPage, "翻页 上一页"));
            Commands.ChatCommands.Add(new Command("economy.player", ShopPage, "翻页"));
            Commands.ChatCommands.Add(new Command("economy.player", OpenAuction, "拍卖行"));
            Commands.ChatCommands.Add(new Command("economy.player", ListAuction, "上架拍卖"));
            Commands.ChatCommands.Add(new Command("economy.player", BuyAuction, "购买拍卖"));
            Commands.ChatCommands.Add(new Command("economy.player", OpenTrade, "交易行"));
            Commands.ChatCommands.Add(new Command("economy.player", ListTrade, "上架交易"));
            Commands.ChatCommands.Add(new Command("economy.player", BuyTrade, "购买交易"));
            Commands.ChatCommands.Add(new Command("economy.player", BuyShop, "购买"));
            Commands.ChatCommands.Add(new Command("economy.player", CheckProgress, "查看进度"));
            Commands.ChatCommands.Add(new Command("economy.admin", AdminAddCrystals, "添加星晶"));
            Commands.ChatCommands.Add(new Command("economy.admin", AdminReloadEconomy, "重读经济"));
            Commands.ChatCommands.Add(new Command("economy.admin", ToggleEconomy, "开关经济系统"));
            Commands.ChatCommands.Add(new Command("economy.admin", SetuiRefreshRate, "设置ui刷新频率"));
            Commands.ChatCommands.Add(new Command("economy.player", CheckuiRefreshRate, "查看ui刷新频率"));
            Commands.ChatCommands.Add(new Command("economy.admin", AdminSetProgress, "设置进度"));
            
            // 创建默认权限组
            CreateDefaultPermissions();
        }
        
        private void CreateDefaultPermissions()
        {
            try
            {
                // 为修仙弟子组添加经济权限
                var xiuxianGroup = TShock.Groups.GetGroupByName("修仙弟子");
                if (xiuxianGroup != null && !xiuxianGroup.HasPermission("economy.player"))
                {
                    TShock.Groups.AddPermissions("修仙弟子", new List<string> { "economy.player" });
                    TShock.Log.Info("已为修仙弟子组添加经济权限");
                }

                // 为修仙仙尊组添加管理员权限
                var adminGroup = TShock.Groups.GetGroupByName("修仙仙尊");
                if (adminGroup != null && !adminGroup.HasPermission("economy.admin"))
                {
                    TShock.Groups.AddPermissions("修仙仙尊", new List<string> { "economy.player", "economy.admin" });
                    TShock.Log.Info("已为修仙仙尊组添加经济管理员权限");
                }
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"创建经济权限组失败: {ex.Message}");
            }
        }
        
        private void CheckBalance(CommandArgs args)
        {
            if (!EconomyConfig.Instance.EconomyEnabled)
            {
                args.Player.SendErrorMessage("经济系统当前已关闭");
                return;
            }

            var data = GetPlayerEconomy(args.Player.Name);
            args.Player.SendMessage("══════════ 星晶余额 ══════════", Color.Cyan);
            args.Player.SendMessage($"总价值: {data.GetTotalLowGradeValue()} 下品星晶", Color.White);
            args.Player.SendMessage($"详细余额: {data.GetBalanceDisplay()}", Color.White);
            
            // 显示财富等级
            int totalValue = data.GetTotalLowGradeValue();
            string wealthLevel = totalValue switch
            {
                > 1000000 => "[c/FFD700:富可敌国]",
                > 100000 => "[c/CC00FF:家财万贯]", 
                > 10000 => "[c/0099FF:小有资产]",
                > 1000 => "[c/00FF00:温饱有余]",
                _ => "[c/AAAAAA:一贫如洗]"
            };
            args.Player.SendMessage($"财富等级: {wealthLevel}", Color.White);
            
            args.Player.SendMessage("══════════════════════════════", Color.Cyan);
        }
        
        private void DailyReward(CommandArgs args)
        {
            if (!EconomyConfig.Instance.EconomyEnabled)
            {
                args.Player.SendErrorMessage("经济系统当前已关闭");
                return;
            }

            var data = GetPlayerEconomy(args.Player.Name);
            var config = EconomyConfig.Instance;
            
            if (!data.CanReceiveDailyReward)
            {
                args.Player.SendErrorMessage("今日已领取过每日奖励！");
                return;
            }
            
            data.LastDailyReward = DateTime.Now;
            data.AddStarCrystals(StarCrystalGrade.下品, config.DailyRewardLow);
            data.AddStarCrystals(StarCrystalGrade.中品, config.DailyRewardMedium);
            data.AddStarCrystals(StarCrystalGrade.上品, config.DailyRewardHigh);
            data.AddStarCrystals(StarCrystalGrade.极品, config.DailyRewardExtreme);
            
            args.Player.SendSuccessMessage("每日奖励领取成功！");
            args.Player.SendMessage($"获得: {config.DailyRewardLow}下品星晶, {config.DailyRewardMedium}中品星晶, {config.DailyRewardHigh}上品星晶, {config.DailyRewardExtreme}极品星晶", Color.Yellow);
            
            UpdateEconomyui(args.Player, data);
            SavePlayerEconomy(args.Player.Name);
        }
        
        private void BuyShop(CommandArgs args)
        {
            if (!EconomyConfig.Instance.EconomyEnabled)
            {
                args.Player.SendErrorMessage("经济系统当前已关闭");
                return;
            }

            if (args.Parameters.Count < 1)
            {
                args.Player.SendErrorMessage("用法: /购买 <编号> [数量]");
                args.Player.SendErrorMessage("编号是在商店中显示的全局编号");
                return;
            }
            
            if (!int.TryParse(args.Parameters[0], out int index) || index < 1)
            {
                args.Player.SendErrorMessage("无效的商品编号！");
                return;
            }
            
            int quantity = 1;
            if (args.Parameters.Count > 1 && (!int.TryParse(args.Parameters[1], out quantity) || quantity < 1))
            {
                args.Player.SendErrorMessage("无效的数量！");
                return;
            }
            
            var shopItems = EconomyConfig.Instance.ShopItems;
            if (index > shopItems.Count)
            {
                args.Player.SendErrorMessage("商品编号不存在！");
                return;
            }
            
            var item = shopItems[index - 1];
            var data = GetPlayerEconomy(args.Player.Name);
            
            // 检查进度要求
            if (!item.MeetsProgressRequirement(data))
            {
                args.Player.SendErrorMessage($"未满足购买条件！需要击败: {item.BossRequirement}");
                args.Player.SendErrorMessage("击败对应Boss后即可解锁购买");
                return;
            }
            
            // 检查库存
            if (item.Stock != -1 && item.Stock < quantity)
            {
                args.Player.SendErrorMessage($"商品库存不足！剩余: {item.Stock}");
                return;
            }
            
            // 检查购买限制
            if (!item.CanPlayerPurchase(args.Player.Name, quantity))
            {
                args.Player.SendErrorMessage($"已达到购买限制！限购: {item.PurchaseLimit}");
                return;
            }
            
            // 检查余额
            int totalCost = item.PriceAmount * quantity;
            if (!data.RemoveStarCrystals(item.PriceGrade, totalCost))
            {
                args.Player.SendErrorMessage($"星晶不足！需要: {item.GetPriceDisplay()} x{quantity}");
                return;
            }
            
            // 执行购买
            if (item.Stock != -1)
                item.Stock -= quantity;
                
            item.RecordPurchase(args.Player.Name, quantity);
            
            // 给予物品
            args.Player.GiveItem(item.ItemId, quantity, 0);
            
            args.Player.SendSuccessMessage($"成功购买 {item.GetItemDisplay()} x{quantity}！");
            args.Player.SendMessage($"消耗: {item.GetPriceDisplay()} x{quantity}", Color.Yellow);
            
            UpdateEconomyui(args.Player, data);
            SavePlayerEconomy(args.Player.Name);
            EconomyConfig.Save(ConfigPath);
        }
        
        private void CheckProgress(CommandArgs args)
        {
            if (!EconomyConfig.Instance.EconomyEnabled)
            {
                args.Player.SendErrorMessage("经济系统当前已关闭");
                return;
            }

            var data = GetPlayerEconomy(args.Player.Name);
            args.Player.SendMessage("══════════ 击败Boss进度 ══════════", Color.Orange);
            
            if (data.DefeatedBosses.Count == 0)
            {
                args.Player.SendMessage("尚未击败任何Boss", Color.Gray);
            }
            else
            {
                foreach (var boss in data.DefeatedBosses.OrderBy(b => GetBossProgressOrder(b)))
                {
                    args.Player.SendMessage($"✅ {boss}", Color.LightGreen);
                }
            }
            
            args.Player.SendMessage("══════════════════════════════", Color.Orange);
            args.Player.SendMessage("提示: 击败Boss可以解锁商店中的新物品！", Color.Yellow);
        }
        
        private int GetBossProgressOrder(string bossName)
        {
            var order = new List<string>
            {
                "克苏鲁之眼", "世界吞噬者", "克苏鲁之脑", "蜂王", "骷髅王",
                "血肉墙", "机械蠕虫", "机械骷髅王", "机械双子眼", 
                "世纪之花", "石巨人", "猪龙鱼公爵", "月亮领主"
            };
            
            return order.IndexOf(bossName);
        }
        
        private void OpenAuction(CommandArgs args)
        {
            if (!EconomyConfig.Instance.EconomyEnabled)
            {
                args.Player.SendErrorMessage("经济系统当前已关闭");
                return;
            }

            var activeAuctions = AuctionHouse.Where(a => !a.IsSold && !a.IsExpired).ToList();
            if (activeAuctions.Count == 0)
            {
                args.Player.SendErrorMessage("拍卖行暂无商品！");
                return;
            }
            
            args.Player.SendMessage("══════════ 拍卖行 ══════════", Color.Orange);
            for (int i = 0; i < Math.Min(activeAuctions.Count, 10); i++) // 只显示前10个
            {
                var auction = activeAuctions[i];
                string timeLeft = (auction.ExpireTime - DateTime.Now).ToString(@"dd\:hh\:mm");
                // 显示物品图标
                args.Player.SendMessage($"{i + 1}. {auction.GetItemIcon()} {auction.GetItemDisplay()} - {auction.GetPriceDisplay()} (卖家:{auction.Seller}, 剩余:{timeLeft})", Color.White);
            }
            args.Player.SendMessage("使用 /购买拍卖 <编号> 购买物品", Color.Yellow);
            args.Player.SendMessage("══════════════════════════════", Color.Orange);
        }
        
        private void ListAuction(CommandArgs args)
        {
            if (!EconomyConfig.Instance.EconomyEnabled)
            {
                args.Player.SendErrorMessage("经济系统当前已关闭");
                return;
            }

            if (args.Parameters.Count < 2)
            {
                args.Player.SendErrorMessage("用法: /上架拍卖 <价格> <品质>");
                args.Player.SendErrorMessage("品质: 下品, 中品, 上品, 极品");
                return;
            }
            
            if (!int.TryParse(args.Parameters[0], out int price) || price < 1)
            {
                args.Player.SendErrorMessage("无效的价格！必须是正整数");
                return;
            }
            
            string gradeStr = args.Parameters[1].ToLower();
            StarCrystalGrade grade = StarCrystalGrade.下品;
            
            switch (gradeStr)
            {
                case "下品": grade = StarCrystalGrade.下品; break;
                case "中品": grade = StarCrystalGrade.中品; break;
                case "上品": grade = StarCrystalGrade.上品; break;
                case "极品": grade = StarCrystalGrade.极品; break;
                default:
                    args.Player.SendErrorMessage("无效的品质！可用: 下品, 中品, 上品, 极品");
                    return;
            }
            
            // 检查玩家手中是否有物品
            var playerItem = args.Player.TPlayer.inventory[args.Player.TPlayer.selectedItem];
            if (playerItem.type == 0 || playerItem.stack < 1)
            {
                args.Player.SendErrorMessage("请手持要拍卖的物品！");
                return;
            }
            
            // 创建拍卖品
            var auction = new AuctionItem
            {
                Seller = args.Player.Name,
                ItemId = playerItem.type,
                Stack = playerItem.stack,
                Prefix = playerItem.prefix,
                PriceGrade = grade,
                PriceAmount = price
            };
            
            // 从玩家手中移除物品
            playerItem.SetDefaults(0);
            args.Player.SendData(PacketTypes.PlayerSlot, "", args.Player.Index, args.Player.TPlayer.selectedItem);
            
            // 添加到拍卖行
            AuctionHouse.Add(auction);
            SaveAuction();
            
            args.Player.SendSuccessMessage("物品上架成功！");
            args.Player.SendMessage($"上架: {auction.GetItemIcon()} {auction.GetItemDisplay()} - {auction.GetPriceDisplay()}", Color.Yellow);
        }
        
        private void BuyAuction(CommandArgs args)
        {
            if (!EconomyConfig.Instance.EconomyEnabled)
            {
                args.Player.SendErrorMessage("经济系统当前已关闭");
                return;
            }

            if (args.Parameters.Count < 1)
            {
                args.Player.SendErrorMessage("用法: /购买拍卖 <编号>");
                return;
            }
            
            if (!int.TryParse(args.Parameters[0], out int index) || index < 1)
            {
                args.Player.SendErrorMessage("无效的拍卖编号！");
                return;
            }
            
            var activeAuctions = AuctionHouse.Where(a => !a.IsSold && !a.IsExpired).ToList();
            if (index > activeAuctions.Count)
            {
                args.Player.SendErrorMessage("拍卖编号不存在！");
                return;
            }
            
            var auction = activeAuctions[index - 1];
            var data = GetPlayerEconomy(args.Player.Name);
            
            // 检查是否是自己的拍卖
            if (auction.Seller == args.Player.Name)
            {
                args.Player.SendErrorMessage("不能购买自己的拍卖物品！");
                return;
            }
            
            // 检查余额
            if (!data.RemoveStarCrystals(auction.PriceGrade, auction.PriceAmount))
            {
                args.Player.SendErrorMessage($"星晶不足！需要: {auction.GetPriceDisplay()}");
                return;
            }
            
            // 执行交易
            auction.IsSold = true;
            auction.Buyer = args.Player.Name;
            
            // 给予买家物品
            args.Player.GiveItem(auction.ItemId, auction.Stack, auction.Prefix);
            
            // 给予卖家星晶
            var sellerData = GetPlayerEconomy(auction.Seller);
            sellerData.AddStarCrystals(auction.PriceGrade, auction.PriceAmount);
            
            args.Player.SendSuccessMessage($"成功购买 {auction.GetItemIcon()} {auction.GetItemDisplay()}！");
            
            // 通知卖家
            var seller = TShock.Players.FirstOrDefault(p => p?.Name == auction.Seller);
            if (seller != null)
            {
                seller.SendSuccessMessage($"你的 {auction.GetItemDisplay()} 已售出！");
                seller.SendMessage($"获得: {auction.GetPriceDisplay()}", Color.Yellow);
                UpdateEconomyui(seller, sellerData);
            }
            
            UpdateEconomyui(args.Player, data);
            SavePlayerEconomy(args.Player.Name);
            SavePlayerEconomy(auction.Seller);
            SaveAuction();
        }

        private void OpenTrade(CommandArgs args)
        {
            if (!EconomyConfig.Instance.EconomyEnabled)
            {
                args.Player.SendErrorMessage("经济系统当前已关闭");
                return;
            }

            var activeTrades = TradeHouse.Where(t => !t.IsSold && !t.IsExpired).ToList();
            if (activeTrades.Count == 0)
            {
                args.Player.SendErrorMessage("交易行暂无商品！");
                return;
            }
            
            args.Player.SendMessage("══════════ 交易行 ══════════", Color.LightBlue);
            for (int i = 0; i < Math.Min(activeTrades.Count, 10); i++) // 只显示前10个
            {
                var trade = activeTrades[i];
                string timeLeft = (trade.ExpireTime - DateTime.Now).ToString(@"dd\:hh\:mm");
                // 显示物品图标
                args.Player.SendMessage($"{i + 1}. {trade.GetItemIcon()} {trade.GetItemDisplay()} - {trade.GetPriceDisplay()} (卖家:{trade.Seller}, 剩余:{timeLeft})", Color.White);
            }
            args.Player.SendMessage("使用 /购买交易 <编号> 购买物品", Color.Yellow);
            args.Player.SendMessage("══════════════════════════════", Color.LightBlue);
        }
        
        private void ListTrade(CommandArgs args)
        {
            if (!EconomyConfig.Instance.EconomyEnabled)
            {
                args.Player.SendErrorMessage("经济系统当前已关闭");
                return;
            }

            if (args.Parameters.Count < 2)
            {
                args.Player.SendErrorMessage("用法: /上架交易 <价格> <品质>");
                args.Player.SendErrorMessage("品质: 下品, 中品, 上品, 极品");
                return;
            }
            
            if (!int.TryParse(args.Parameters[0], out int price) || price < 1)
            {
                args.Player.SendErrorMessage("无效的价格！必须是正整数");
                return;
            }
            
            string gradeStr = args.Parameters[1].ToLower();
            StarCrystalGrade grade = StarCrystalGrade.下品;
            
            switch (gradeStr)
            {
                case "下品": grade = StarCrystalGrade.下品; break;
                case "中品": grade = StarCrystalGrade.中品; break;
                case "上品": grade = StarCrystalGrade.上品; break;
                case "极品": grade = StarCrystalGrade.极品; break;
                default:
                    args.Player.SendErrorMessage("无效的品质！可用: 下品, 中品, 上品, 极品");
                    return;
            }
            
            // 检查玩家手中是否有物品
            var playerItem = args.Player.TPlayer.inventory[args.Player.TPlayer.selectedItem];
            if (playerItem.type == 0 || playerItem.stack < 1)
            {
                args.Player.SendErrorMessage("请手持要交易的物品！");
                return;
            }
            
            // 创建交易品
            var trade = new TradeItem
            {
                Seller = args.Player.Name,
                ItemId = playerItem.type,
                Stack = playerItem.stack,
                Prefix = playerItem.prefix,
                PriceGrade = grade,
                PriceAmount = price
            };
            
            // 从玩家手中移除物品
            playerItem.SetDefaults(0);
            args.Player.SendData(PacketTypes.PlayerSlot, "", args.Player.Index, args.Player.TPlayer.selectedItem);
            
            // 添加到交易行
            TradeHouse.Add(trade);
            SaveTrade();
            
            args.Player.SendSuccessMessage("物品上架交易行成功！");
            args.Player.SendMessage($"上架: {trade.GetItemIcon()} {trade.GetItemDisplay()} - {trade.GetPriceDisplay()}", Color.Yellow);
        }
        
        private void BuyTrade(CommandArgs args)
        {
            if (!EconomyConfig.Instance.EconomyEnabled)
            {
                args.Player.SendErrorMessage("经济系统当前已关闭");
                return;
            }

            if (args.Parameters.Count < 1)
            {
                args.Player.SendErrorMessage("用法: /购买交易 <编号>");
                return;
            }
            
            if (!int.TryParse(args.Parameters[0], out int index) || index < 1)
            {
                args.Player.SendErrorMessage("无效的交易编号！");
                return;
            }
            
            var activeTrades = TradeHouse.Where(t => !t.IsSold && !t.IsExpired).ToList();
            if (index > activeTrades.Count)
            {
                args.Player.SendErrorMessage("交易编号不存在！");
                return;
            }
            
            var trade = activeTrades[index - 1];
            var data = GetPlayerEconomy(args.Player.Name);
            
            // 检查是否是自己的交易
            if (trade.Seller == args.Player.Name)
            {
                args.Player.SendErrorMessage("不能购买自己的交易物品！");
                return;
            }
            
            // 检查余额
            if (!data.RemoveStarCrystals(trade.PriceGrade, trade.PriceAmount))
            {
                args.Player.SendErrorMessage($"星晶不足！需要: {trade.GetPriceDisplay()}");
                return;
            }
            
            // 执行交易
            trade.IsSold = true;
            trade.Buyer = args.Player.Name;
            
            // 给予买家物品
            args.Player.GiveItem(trade.ItemId, trade.Stack, trade.Prefix);
            
            // 给予卖家星晶
            var sellerData = GetPlayerEconomy(trade.Seller);
            sellerData.AddStarCrystals(trade.PriceGrade, trade.PriceAmount);
            
            args.Player.SendSuccessMessage($"成功购买 {trade.GetItemIcon()} {trade.GetItemDisplay()}！");
            
            // 通知卖家
            var seller = TShock.Players.FirstOrDefault(p => p?.Name == trade.Seller);
            if (seller != null)
            {
                seller.SendSuccessMessage($"你的 {trade.GetItemDisplay()} 已售出！");
                seller.SendMessage($"获得: {trade.GetPriceDisplay()}", Color.Yellow);
                UpdateEconomyui(seller, sellerData);
            }
            
            UpdateEconomyui(args.Player, data);
            SavePlayerEconomy(args.Player.Name);
            SavePlayerEconomy(trade.Seller);
            SaveTrade();
        }
        
        private void AdminAddCrystals(CommandArgs args)
        {
            if (args.Parameters.Count < 3)
            {
                args.Player.SendErrorMessage("用法: /添加星晶 <玩家> <品质> <数量>");
                args.Player.SendErrorMessage("品质: 下品, 中品, 上品, 极品");
                return;
            }
            
            string targetName = args.Parameters[0];
            string gradeStr = args.Parameters[1].ToLower();
            
            if (!int.TryParse(args.Parameters[2], out int amount) || amount < 1)
            {
                args.Player.SendErrorMessage("无效的数量！必须是正整数");
                return;
            }
            
            StarCrystalGrade grade = StarCrystalGrade.下品;
            switch (gradeStr)
            {
                case "下品": grade = StarCrystalGrade.下品; break;
                case "中品": grade = StarCrystalGrade.中品; break;
                case "上品": grade = StarCrystalGrade.上品; break;
                case "极品": grade = StarCrystalGrade.极品; break;
                default:
                    args.Player.SendErrorMessage("无效的品质！可用: 下品, 中品, 上品, 极品");
                    return;
            }
            
            var targetData = GetPlayerEconomy(targetName);
            targetData.AddStarCrystals(grade, amount);
            
            args.Player.SendSuccessMessage($"已为 {targetName} 添加 {amount} {grade}星晶");
            
            var targetPlayer = TShock.Players.FirstOrDefault(p => p?.Name == targetName);
            if (targetPlayer != null)
            {
                targetPlayer.SendSuccessMessage($"管理员 {args.Player.Name} 为你添加了 {amount} {grade}星晶");
                UpdateEconomyui(targetPlayer, targetData);
            }
            
            SavePlayerEconomy(targetName);
        }
        
        private void AdminSetProgress(CommandArgs args)
        {
            if (args.Parameters.Count < 2)
            {
                args.Player.SendErrorMessage("用法: /设置进度 <玩家> <Boss名称>");
                args.Player.SendErrorMessage("可用Boss: 克苏鲁之眼, 世界吞噬者, 克苏鲁之脑, 蜂王, 骷髅王, 血肉墙, 机械蠕虫, 机械骷髅王, 机械双子眼, 世纪之花, 石巨人, 猪龙鱼公爵, 月亮领主");
                return;
            }
            
            string targetName = args.Parameters[0];
            string bossName = args.Parameters[1];
            
            if (!BossNames.ContainsKey(bossName))
            {
                args.Player.SendErrorMessage("无效的Boss名称！");
                args.Player.SendErrorMessage("可用Boss: " + string.Join(", ", BossNames.Keys));
                return;
            }
            
            var targetData = GetPlayerEconomy(targetName);
            targetData.RecordBossDefeat(bossName);
            
            args.Player.SendSuccessMessage($"已为 {targetName} 设置击败 {bossName} 的进度");
            
            var targetPlayer = TShock.Players.FirstOrDefault(p => p?.Name == targetName);
            if (targetPlayer != null)
            {
                targetPlayer.SendSuccessMessage($"管理员 {args.Player.Name} 为你解锁了 {bossName} 进度");
                targetPlayer.SendSuccessMessage($"现在可以购买需要击败 {bossName} 的商店物品了！");
            }
            
            SavePlayerEconomy(targetName);
        }
        
        private void AdminReloadEconomy(CommandArgs args)
        {
            try
            {
                EconomyConfig.Load(ConfigPath);
                LoadEconomy(DataPath);
                InitializeAuction();
                InitializeTrade();
                InitializeuiTimers(); // 重新初始化UI定时器
                
                args.Player.SendSuccessMessage("经济配置重读完成！");
                
                // 更新所有在线玩家的ui
                foreach (var player in TShock.Players.Where(p => p != null && p.Active))
                {
                    var data = GetPlayerEconomy(player.Name);
                    UpdateEconomyui(player, data);
                }
            }
            catch (Exception ex)
            {
                args.Player.SendErrorMessage($"重读经济配置失败: {ex.Message}");
            }
        }

        private void ToggleEconomy(CommandArgs args)
        {
            EconomyConfig.Instance.EconomyEnabled = !EconomyConfig.Instance.EconomyEnabled;
            EconomyConfig.Save(ConfigPath);
            
            string status = EconomyConfig.Instance.EconomyEnabled ? "开启" : "关闭";
            args.Player.SendSuccessMessage($"经济系统已{status}");
            
            // 只向在线的玩家发送消息，避免在初始化时发送
            var onlinePlayers = TShock.Players.Where(p => p != null && p.Active).ToList();
            if (onlinePlayers.Count > 0)
            {
                if (EconomyConfig.Instance.EconomyEnabled)
                {
                    TSPlayer.All.SendInfoMessage($"经济系统已开启！使用 /星晶余额 查看余额");
                }
                else
                {
                    TSPlayer.All.SendInfoMessage($"经济系统已关闭，所有经济功能暂时不可用");
                }
            }
        }

        private void SetuiRefreshRate(CommandArgs args)
        {
            if (args.Parameters.Count < 2)
            {
                args.Player.SendErrorMessage("用法: /设置ui刷新频率 <类型> <毫秒>");
                args.Player.SendErrorMessage("类型: top - 顶部UI, chat - 聊天UI");
                args.Player.SendErrorMessage("示例: /设置ui刷新频率 top 5000 (顶部UI 5秒刷新一次)");
                args.Player.SendErrorMessage("示例: /设置ui刷新频率 chat 10000 (聊天UI 10秒刷新一次)");
                args.Player.SendErrorMessage("最小值为1000毫秒(1秒)，最大值为60000毫秒(1分钟)");
                return;
            }
            
            string uiType = args.Parameters[0].ToLower();
            if (!int.TryParse(args.Parameters[1], out int interval) || interval < 1000 || interval > 60000)
            {
                args.Player.SendErrorMessage("无效的刷新频率！必须是1000到60000之间的整数");
                return;
            }
            
            bool updated = false;
            switch (uiType)
            {
                case "top":
                    EconomyConfig.Instance.TopuiRefreshInterval = interval;
                    updated = true;
                    break;
                case "chat":
                    EconomyConfig.Instance.ChatuiRefreshInterval = interval;
                    updated = true;
                    break;
                default:
                    args.Player.SendErrorMessage("无效的UI类型！可用: top, chat");
                    return;
            }
            
            if (updated)
            {
                EconomyConfig.Save(ConfigPath);
                
                // 重新初始化UI定时器
                InitializeuiTimers();
                
                args.Player.SendSuccessMessage($"{uiType.ToUpper()} UI刷新频率已设置为 {interval} 毫秒");
                args.Player.SendInfoMessage($"相当于每 {interval / 1000.0:F1} 秒刷新一次");
                
                // 记录到日志
                TShock.Log.Info($"管理员 {args.Player.Name} 将{uiType} UI刷新频率设置为 {interval} 毫秒");
            }
        }

        private void CheckuiRefreshRate(CommandArgs args)
        {
            var topInterval = EconomyConfig.Instance.TopuiRefreshInterval;
            var chatInterval = EconomyConfig.Instance.ChatuiRefreshInterval;
            
            args.Player.SendMessage("══════════ UI刷新频率 ══════════", Color.Cyan);
            args.Player.SendMessage($"顶部UI刷新间隔: {topInterval} 毫秒", Color.White);
            args.Player.SendMessage($"相当于: {topInterval / 1000.0:F1} 秒", Color.White);
            args.Player.SendMessage($"每分钟刷新次数: {60000 / topInterval} 次", Color.White);
            args.Player.SendMessage($"聊天UI刷新间隔: {chatInterval} 毫秒", Color.White);
            args.Player.SendMessage($"相当于: {chatInterval / 1000.0:F1} 秒", Color.White);
            args.Player.SendMessage($"每分钟刷新次数: {60000 / chatInterval} 次", Color.White);
            
            if (args.Player.HasPermission("economy.admin"))
            {
                args.Player.SendMessage("使用 /设置ui刷新频率 <类型> <毫秒> 调整刷新频率", Color.Yellow);
                args.Player.SendMessage("类型: top - 顶部UI, chat - 聊天UI", Color.Yellow);
            }
            
            args.Player.SendMessage("══════════════════════════════", Color.Cyan);
        }
        #endregion

        #region 插件生命周期
        public override void Initialize()
        {
            // 前置插件检查
            if (!CheckShouyuanPreReq())
            {
                TShock.Log.ConsoleError("【加载失败】修仙经济插件因缺少前置插件而无法加载！");
                TShock.Log.ConsoleError("请安装 XiuXianShouYuan.dll（修仙星宿系统）后重启服务器。");
                return;
            }
            
            // 加载配置和数据
            EconomyConfig.Load(ConfigPath);
            LoadEconomy(DataPath);
            InitializeAuction();
            InitializeTrade();
            InitializeuiTimers(); // 初始化UI定时器
            
            // 注册事件和命令
            ServerApi.Hooks.NpcKilled.Register(this, OnNpcKilled);
            PlayerHooks.PlayerPostLogin += OnPlayerPostLogin;
            PlayerHooks.PlayerLogout += OnPlayerLogout;
            
            InitializeCommands();
            
            TShock.Log.ConsoleInfo("═══════════════════════════════════════════");
            TShock.Log.ConsoleInfo("修仙经济系统 v1.2.0 已成功加载！");
            TShock.Log.ConsoleInfo($"经济系统状态: {(EconomyConfig.Instance.EconomyEnabled ? "已启用" : "已禁用")}");
            TShock.Log.ConsoleInfo($"商店物品数量: {EconomyConfig.Instance.ShopItems.Count}");
            TShock.Log.ConsoleInfo($"玩家经济数据: {EconomyPlayers.Count} 名玩家");
            TShock.Log.ConsoleInfo($"拍卖行物品: {AuctionHouse.Count} 个");
            TShock.Log.ConsoleInfo($"交易行物品: {TradeHouse.Count} 个");
            TShock.Log.ConsoleInfo($"进度系统: 已集成 {BossNames.Count} 个Boss");
            TShock.Log.ConsoleInfo("═══════════════════════════════════════════");
            
            // 为所有在线玩家更新UI
            foreach (var player in TShock.Players.Where(p => p != null && p.Active && p.IsLoggedIn))
            {
                var data = GetPlayerEconomy(player.Name);
                UpdateEconomyui(player, data);
            }
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.NpcKilled.Deregister(this, OnNpcKilled);
                PlayerHooks.PlayerPostLogin -= OnPlayerPostLogin;
                PlayerHooks.PlayerLogout -= OnPlayerLogout;
                
                if (_topuiRefreshTimer != null)
                {
                    _topuiRefreshTimer.Stop();
                    _topuiRefreshTimer.Dispose();
                }
                
                if (_chatuiRefreshTimer != null)
                {
                    _chatuiRefreshTimer.Stop();
                    _chatuiRefreshTimer.Dispose();
                }
                
                SaveEconomy(DataPath);
                SaveAuction();
                SaveTrade();
                
                TShock.Log.ConsoleInfo("修仙经济系统已卸载");
            }
            base.Dispose(disposing);
        }
        
        private void OnPlayerPostLogin(PlayerPostLoginEventArgs args)
        {
            var data = GetPlayerEconomy(args.Player.Name);
            UpdateEconomyui(args.Player, data);
            
            if (EconomyConfig.Instance.EconomyEnabled)
            {
                args.Player.SendSuccessMessage("星晶经济系统已加载！使用 /星晶余额 查看余额");
                args.Player.SendInfoMessage("使用 /查看进度 查看已击败的Boss和可购买物品");
                
                // 如果是新玩家且配置允许，发送欢迎信息
                if (data.StarCrystals.Count == 0 && EconomyConfig.Instance.GiveInitialCrystals)
                {
                    args.Player.SendInfoMessage($"你获得了 {EconomyConfig.Instance.InitialCrystals} 初始下品星晶！");
                }
            }
        }
        
        private void OnPlayerLogout(PlayerLogoutEventArgs args)
        {
            SavePlayerEconomy(args.Player.Name);
            statusManager.RemoveText(args.Player);
        }
        #endregion

        #region StatusManager
        private class StatusManager
        {
            private class PlayerStatus
            {
                public string Key { get; set; }
                public string Text { get; set; }
            }

            private readonly Dictionary<TSPlayer, List<PlayerStatus>> _playerStatuses = new Dictionary<TSPlayer, List<PlayerStatus>>();

            public void AddOrUpdateText(TSPlayer player, string key, string text)
            {
                if (!_playerStatuses.TryGetValue(player, out var statuses))
                {
                    statuses = new List<PlayerStatus>();
                    _playerStatuses[player] = statuses;
                }

                var existing = statuses.FirstOrDefault(s => s.Key == key);
                if (existing != null)
                {
                    existing.Text = text;
                }
                else
                {
                    statuses.Add(new PlayerStatus { Key = key, Text = text });
                }

                UpdatePlayerStatus(player);
            }

            public void RemoveText(TSPlayer player, string key)
            {
                if (_playerStatuses.TryGetValue(player, out var statuses))
                {
                    var item = statuses.FirstOrDefault(s => s.Key == key);
                    if (item != null)
                    {
                        statuses.Remove(item);
                        UpdatePlayerStatus(player);
                    }
                }
            }

            public void RemoveText(TSPlayer player)
            {
                if (_playerStatuses.ContainsKey(player))
                {
                    _playerStatuses.Remove(player);
                    player.SendData(PacketTypes.Status, "", 0, 0x1f);
                }
            }

            private void UpdatePlayerStatus(TSPlayer player)
            {
                if (!_playerStatuses.TryGetValue(player, out var statuses) || !statuses.Any())
                    return;

                var combined = new System.Text.StringBuilder();
                foreach (var status in statuses)
                {
                    combined.AppendLine(status.Text);
                }

                player.SendData(PacketTypes.Status, combined.ToString(), 0, 0x1f);
            }
        }
        #endregion
    }
    
    // 颜色扩展
    public static class StringExtensions
    {
        public static string Color(this string text, Color color)
        {
            return $"[c/{color.R:X2}{color.G:X2}{color.B:X2}:{text}]";
        }
    }
}