using System;
using System.ComponentModel;
using System.IO;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using TerrariaApi.Server;
using TShockAPI;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace CustomDropPlugin
{
    [ApiVersion(2, 1)]
    public class TreeAndPotDropReplacer : TerrariaPlugin
    {
        public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
        public override Version Version => new Version(1, 0, 2);
        public override string Author => "泷白";
        public override string Description => "额外的掉落物 - 支持自定义物品";

        private static string configPath;
        public static Config Config { get; private set; }
        private Random random = new Random();
        
        // 树叶瓦片类型
        private readonly HashSet<ushort> _leafTiles = new HashSet<ushort> { 384, 385 };
        
        // 惩罚系统
        private Dictionary<int, PlayerPunishment> _playerPunishments = new Dictionary<int, PlayerPunishment>();
        
        public TreeAndPotDropReplacer(Main game) : base(game)
        {
            Order = 1;
        }

        public override void Initialize()
        {
            TShock.Log.ConsoleInfo($"Plugin {Name} v{Version} (by {Author}) initiated.");
            
            configPath = Path.Combine(TShock.SavePath, "TreeAndPotDropReplacer.json");
            LoadConfig();
            
            // 注册命令
            Commands.ChatCommands.Add(new Command("treedrop.reload", ReloadConfig, "tadrreload"));
            Commands.ChatCommands.Add(new Command("treedrop.togglemsg", ToggleMessages, "tadrtogglemsg", "tadrmsg"));
            
            // 美观的控制台输出
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine($"║          {Name} v{Version}           ║");
            Console.WriteLine("╠══════════════════════════════════════╣");
            Console.WriteLine($"║  作者: {Author}                      ║");
            Console.WriteLine($"║  描述: {Description}  ║");
            Console.WriteLine("║                                      ║");
            Console.WriteLine("║      Terraria TShock 服务器插件      ║");
            Console.WriteLine("║      配置文件重构版      ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
            Console.ResetColor();
            
            TShock.Log.ConsoleInfo($"=== {Name} v{Version} 已加载 ===");
            TShock.Log.ConsoleInfo($"作者: {Author}");
            TShock.Log.ConsoleInfo($"描述: {Description}");
            TShock.Log.ConsoleInfo($"配置文件: {configPath}");
            TShock.Log.ConsoleInfo($"武器掉落物数量: {Config?.武器掉落?.Count ?? 0}");
            TShock.Log.ConsoleInfo($"矿物掉落物数量: {Config?.矿物掉落?.Count ?? 0}");
            TShock.Log.ConsoleInfo($"材料掉落物数量: {Config?.材料掉落?.Count ?? 0}");
            TShock.Log.ConsoleInfo($"盔甲掉落物数量: {Config?.盔甲掉落?.Count ?? 0}");
            TShock.Log.ConsoleInfo($"其他掉落物数量: {Config?.其他掉落?.Count ?? 0}");
            TShock.Log.ConsoleInfo("==============================");
            
            try
            {
                On.Terraria.WorldGen.ShakeTree += OnShakeTree;
                On.Terraria.WorldGen.KillTile += OnKillTile;
                ServerApi.Hooks.GameUpdate.Register(this, OnGameUpdate);
                
                TShock.Log.ConsoleInfo($"[{Name}] Hook已成功应用！");
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError($"[{Name}] 应用Hook时出错: {ex}");
                throw;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                On.Terraria.WorldGen.ShakeTree -= OnShakeTree;
                On.Terraria.WorldGen.KillTile -= OnKillTile;
                ServerApi.Hooks.GameUpdate.Deregister(this, OnGameUpdate);
                
                TShock.Log.ConsoleInfo($"[{Name}] 插件已卸载！");
            }
            base.Dispose(disposing);
        }

        private void LoadConfig()
        {
            try
            {
                if (File.Exists(configPath))
                {
                    Config = Config.Read(configPath);
                    if (Config == null)
                    {
                        TShock.Log.ConsoleError($"[{Name}] 配置读取失败，使用默认配置");
                        Config = CreateDefaultConfig();
                    }
                    else
                    {
                        // 确保所有列表不为空
                        Config.巨石雨配置 ??= new 巨石雨配置();
                        Config.武器掉落 ??= new List<自定义掉落>();
                        Config.矿物掉落 ??= new List<自定义掉落>();
                        Config.材料掉落 ??= new List<自定义掉落>();
                        Config.盔甲掉落 ??= new List<自定义掉落>();
                        Config.其他掉落 ??= new List<自定义掉落>();
                    }
                    TShock.Log.ConsoleInfo($"[{Name}] 配置加载成功!");
                }
                else
                {
                    Config = CreateDefaultConfig();
                    Config.Write(configPath);
                    TShock.Log.ConsoleInfo($"[{Name}] 默认配置文件已创建!");
                }
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError($"[{Name}] 加载配置时出错: {ex}");
                Config = CreateDefaultConfig();
            }
        }

        private Config CreateDefaultConfig()
        {
            return new Config
            {
                巨石雨配置 = new 巨石雨配置(),
                武器掉落 = CreateWeaponDrops(),
                矿物掉落 = CreateOreDrops(),
                材料掉落 = CreateMaterialDrops(),
                盔甲掉落 = CreateArmorDrops(),
                其他掉落 = CreateOtherDrops()
            };
        }

        // 创建武器掉落配置
        private List<自定义掉落> CreateWeaponDrops()
        {
            return new List<自定义掉落>
            {
                new 自定义掉落(ItemID.TinShortsword, 1, 2f, 掉落来源.两者, true, 进度限制.无),
                new 自定义掉落(ItemID.CopperBroadsword, 1, 2f, 掉落来源.两者, true, 进度限制.无),
                new 自定义掉落(ItemID.WoodenBow, 1, 3f, 掉落来源.树, true, 进度限制.无),
                new 自定义掉落(ItemID.WandofSparking, 1, 1f, 掉落来源.罐子, true, 进度限制.无),
                new 自定义掉落(ItemID.BlueMoon, 1, 0.5f, 掉落来源.罐子, true, 进度限制.骷髅王前),
                new 自定义掉落(ItemID.Sunfury, 1, 0.3f, 掉落来源.罐子, true, 进度限制.肉山前),
                new 自定义掉落(ItemID.StarCannon, 1, 0.2f, 掉落来源.两者, true, 进度限制.肉山前),
                new 自定义掉落(ItemID.BreakerBlade, 1, 0.1f, 掉落来源.树, true, 进度限制.肉山后),
                new 自定义掉落(ItemID.ClockworkAssaultRifle, 1, 0.1f, 掉落来源.罐子, true, 进度限制.肉山后)
            };
        }

        // 创建矿物掉落配置
        private List<自定义掉落> CreateOreDrops()
        {
            return new List<自定义掉落>
            {
                new 自定义掉落(ItemID.CopperOre, 5, 8f, 掉落来源.两者, true, 进度限制.无),
                new 自定义掉落(ItemID.TinOre, 5, 8f, 掉落来源.两者, true, 进度限制.无),
                new 自定义掉落(ItemID.IronOre, 4, 6f, 掉落来源.罐子, true, 进度限制.无),
                new 自定义掉落(ItemID.LeadOre, 4, 6f, 掉落来源.罐子, true, 进度限制.无),
                new 自定义掉落(ItemID.SilverOre, 3, 4f, 掉落来源.罐子, true, 进度限制.无),
                new 自定义掉落(ItemID.TungstenOre, 3, 4f, 掉落来源.罐子, true, 进度限制.无),
                new 自定义掉落(ItemID.GoldOre, 2, 3f, 掉落来源.罐子, true, 进度限制.无),
                new 自定义掉落(ItemID.PlatinumOre, 2, 3f, 掉落来源.罐子, true, 进度限制.无),
                new 自定义掉落(ItemID.DemoniteOre, 2, 2f, 掉落来源.罐子, true, 进度限制.骷髅王前),
                new 自定义掉落(ItemID.CrimtaneOre, 2, 2f, 掉落来源.罐子, true, 进度限制.骷髅王前),
                new 自定义掉落(ItemID.Meteorite, 3, 1f, 掉落来源.两者, true, 进度限制.骷髅王前),
                new 自定义掉落(ItemID.Hellstone, 2, 0.5f, 掉落来源.罐子, true, 进度限制.肉山前),
                new 自定义掉落(ItemID.CobaltOre, 2, 1f, 掉落来源.罐子, true, 进度限制.肉山后),
                new 自定义掉落(ItemID.PalladiumOre, 2, 1f, 掉落来源.罐子, true, 进度限制.肉山后),
                new 自定义掉落(ItemID.MythrilOre, 2, 0.8f, 掉落来源.罐子, true, 进度限制.肉山后),
                new 自定义掉落(ItemID.OrichalcumOre, 2, 0.8f, 掉落来源.罐子, true, 进度限制.肉山后),
                new 自定义掉落(ItemID.AdamantiteOre, 2, 0.6f, 掉落来源.罐子, true, 进度限制.肉山后),
                new 自定义掉落(ItemID.TitaniumOre, 2, 0.6f, 掉落来源.罐子, true, 进度限制.肉山后),
                new 自定义掉落(ItemID.ChlorophyteOre, 2, 0.3f, 掉落来源.树, true, 进度限制.世纪之花后),
                new 自定义掉落(3464, 1, 0.1f, 掉落来源.两者, true, 进度限制.月亮领主后)
            };
        }

        // 创建材料掉落配置
        private List<自定义掉落> CreateMaterialDrops()
        {
            return new List<自定义掉落>
            {
                new 自定义掉落(ItemID.Wood, 10, 15f, 掉落来源.树, true, 进度限制.无),
                new 自定义掉落(ItemID.Gel, 5, 12f, 掉落来源.树, true, 进度限制.无),
                new 自定义掉落(ItemID.StoneBlock, 8, 10f, 掉落来源.两者, true, 进度限制.无),
                new 自定义掉落(ItemID.FallenStar, 1, 5f, 掉落来源.树, true, 进度限制.无),
                new 自定义掉落(ItemID.Lens, 1, 4f, 掉落来源.罐子, true, 进度限制.无),
                new 自定义掉落(ItemID.WormTooth, 2, 3f, 掉落来源.罐子, true, 进度限制.无),
                new 自定义掉落(ItemID.AntlionMandible, 2, 3f, 掉落来源.罐子, true, 进度限制.无),
                new 自定义掉落(ItemID.BeeWax, 2, 2f, 掉落来源.树, true, 进度限制.骷髅王前),
                new 自定义掉落(ItemID.Bone, 5, 4f, 掉落来源.罐子, true, 进度限制.骷髅王前),
                new 自定义掉落(ItemID.Cobweb, 10, 8f, 掉落来源.树, true, 进度限制.无),
                new 自定义掉落(ItemID.ShadowScale, 2, 2f, 掉落来源.罐子, true, 进度限制.骷髅王前),
                new 自定义掉落(ItemID.TissueSample, 2, 2f, 掉落来源.罐子, true, 进度限制.骷髅王前),
                new 自定义掉落(ItemID.HellstoneBar, 1, 0.8f, 掉落来源.罐子, true, 进度限制.肉山前),
                new 自定义掉落(ItemID.CrystalShard, 2, 3f, 掉落来源.树, true, 进度限制.肉山后),
                new 自定义掉落(ItemID.Ectoplasm, 1, 0.5f, 掉落来源.罐子, true, 进度限制.世纪之花后),
                new 自定义掉落(ItemID.BeetleHusk, 1, 0.3f, 掉落来源.树, true, 进度限制.石巨人后)
            };
        }

        // 创建盔甲掉落配置
        private List<自定义掉落> CreateArmorDrops()
        {
            return new List<自定义掉落>
            {
                new 自定义掉落(ItemID.WoodHelmet, 1, 2f, 掉落来源.树, true, 进度限制.无),
                new 自定义掉落(ItemID.WoodBreastplate, 1, 2f, 掉落来源.树, true, 进度限制.无),
                new 自定义掉落(ItemID.WoodGreaves, 1, 2f, 掉落来源.树, true, 进度限制.无),
                new 自定义掉落(ItemID.CopperHelmet, 1, 1.5f, 掉落来源.罐子, true, 进度限制.无),
                new 自定义掉落(ItemID.IronChainmail, 1, 1.5f, 掉落来源.罐子, true, 进度限制.无),
                new 自定义掉落(ItemID.ShadowHelmet, 1, 0.8f, 掉落来源.罐子, true, 进度限制.骷髅王前),
                new 自定义掉落(ItemID.MoltenHelmet, 1, 0.5f, 掉落来源.罐子, true, 进度限制.肉山前),
                new 自定义掉落(ItemID.CobaltHelmet, 1, 0.4f, 掉落来源.罐子, true, 进度限制.肉山后),
                new 自定义掉落(ItemID.MythrilHood, 1, 0.3f, 掉落来源.罐子, true, 进度限制.肉山后),
                new 自定义掉落(ItemID.HallowedHeadgear, 1, 0.2f, 掉落来源.罐子, true, 进度限制.肉山后),
                new 自定义掉落(ItemID.ChlorophyteHeadgear, 1, 0.1f, 掉落来源.树, true, 进度限制.世纪之花后)
            };
        }

        // 创建其他物品掉落配置
        private List<自定义掉落> CreateOtherDrops()
        {
            return new List<自定义掉落>
            {
                new 自定义掉落(ItemID.LifeCrystal, 1, 1f, 掉落来源.树, true, 进度限制.无),
                new 自定义掉落(ItemID.ManaCrystal, 1, 1f, 掉落来源.罐子, true, 进度限制.无),
                new 自定义掉落(ItemID.RecallPotion, 2, 6f, 掉落来源.两者, true, 进度限制.无),
                new 自定义掉落(ItemID.HealingPotion, 2, 5f, 掉落来源.罐子, true, 进度限制.无),
                new 自定义掉落(ItemID.ManaPotion, 2, 5f, 掉落来源.罐子, true, 进度限制.无),
                new 自定义掉落(ItemID.GoldCoin, 1, 4f, 掉落来源.罐子, true, 进度限制.无),
                new 自定义掉落(ItemID.SilverCoin, 10, 8f, 掉落来源.两者, true, 进度限制.无),
                new 自定义掉落(ItemID.ShinyRedBalloon, 1, 1f, 掉落来源.树, true, 进度限制.无),
                new 自定义掉落(ItemID.LuckyHorseshoe, 1, 0.8f, 掉落来源.树, true, 进度限制.无),
                new 自定义掉落(ItemID.HermesBoots, 1, 0.5f, 掉落来源.罐子, true, 进度限制.无),
                new 自定义掉落(ItemID.CloudinaBottle, 1, 0.5f, 掉落来源.树, true, 进度限制.无),
                new 自定义掉落(ItemID.MagicMirror, 1, 0.3f, 掉落来源.罐子, true, 进度限制.无),
                new 自定义掉落(ItemID.DepthMeter, 1, 0.4f, 掉落来源.罐子, true, 进度限制.无),
                new 自定义掉落(ItemID.Compass, 1, 0.4f, 掉落来源.罐子, true, 进度限制.无)
            };
        }

        private void ReloadConfig(CommandArgs args)
        {
            LoadConfig();
            if (Config != null)
            {
                args.Player.SendSuccessMessage($"[{Name}] 配置已重新加载！");
            }
            else
            {
                args.Player.SendErrorMessage($"[{Name}] 配置重新加载失败，使用默认配置");
            }
        }

        private void ToggleMessages(CommandArgs args)
        {
            var player = args.Player;
            if (!player.ContainsData("CustomDropMessageEnabled"))
            {
                player.SetData("CustomDropMessageEnabled", true);
            }
            
            bool enabled = player.GetData<bool>("CustomDropMessageEnabled");
            enabled = !enabled;
            player.SetData("CustomDropMessageEnabled", enabled);
            
            if (enabled)
            {
                player.SendSuccessMessage($"[{Name}] 掉落提示消息已启用");
            }
            else
            {
                player.SendInfoMessage($"[{Name}] 掉落提示消息已禁用");
            }
        }

        // 检查玩家个人消息设置
        private bool IsPlayerMessageEnabled(TSPlayer player)
        {
            return player?.GetData<bool>("CustomDropMessageEnabled") ?? true;
        }

        // 摇树处理方法
        private void OnShakeTree(On.Terraria.WorldGen.orig_ShakeTree orig, int x, int y)
        {
            orig(x, y);
            
            try
            {
                if (Config?.巨石雨配置 == null)
                    return;

                // 查找最近的玩家
                var nearestPlayer = FindNearestPlayer(x, y);
                if (nearestPlayer == null) return;

                // 查找树叶位置
                int leafY = FindLeafTop(x, y);
                
                if (leafY > 0)
                {
                    // 处理巨石雨掉落
                    if (random.NextDouble() * 100 < Config.巨石雨配置.掉落概率)
                    {
                        if (CheckSpaceAbovePlayer(nearestPlayer))
                        {
                            // 有足够空间，立即生成巨石雨
                            SpawnBoulderRain(nearestPlayer, "摇树");
                            
                            if (Config.巨石雨配置.显示提示信息 && IsPlayerMessageEnabled(nearestPlayer))
                            {
                                nearestPlayer.SendWarningMessage($"[{Name}] 摇树掉落物有概率触发巨石雨！");
                            }
                        }
                        else
                        {
                            // 空间不足，记录惩罚
                            SchedulePunishment(nearestPlayer, "摇树");
                            if (IsPlayerMessageEnabled(nearestPlayer))
                            {
                                nearestPlayer.SendWarningMessage($"[{Name}] 你头顶空间不足，巨石雨将在你有足够空间时落下！");
                            }
                        }
                    }

                    // 处理所有自定义掉落物
                    ProcessAllCustomDrops(x, leafY, 掉落来源.树, "摇树", nearestPlayer);
                }
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError($"[{Name}] 处理摇树时出错: {ex}");
            }
        }

        // 使用KillTile来检测罐子破坏
        private void OnKillTile(On.Terraria.WorldGen.orig_KillTile orig, int i, int j, bool fail, bool effectOnly, bool noItem)
        {
            ITile tile = Main.tile[i, j];
            bool wasPot = tile.active() && tile.type == TileID.Pots;
            
            orig(i, j, fail, effectOnly, noItem);
            
            if (wasPot && !fail && !effectOnly)
            {
                try
                {
                    if (Config?.巨石雨配置 == null)
                        return;

                    // 查找最近的玩家
                    var nearestPlayer = FindNearestPlayer(i, j);
                    if (nearestPlayer == null) return;

                    // 处理巨石雨掉落
                    if (random.NextDouble() * 100 < Config.巨石雨配置.掉落概率)
                    {
                        if (CheckSpaceAbovePlayer(nearestPlayer))
                        {
                            // 有足够空间，立即生成巨石雨
                            SpawnBoulderRain(nearestPlayer, "砸罐子");
                            
                            if (Config.巨石雨配置.显示提示信息 && IsPlayerMessageEnabled(nearestPlayer))
                            {
                                nearestPlayer.SendWarningMessage($"[{Name}] 砸罐子掉落物有概率触发巨石雨！");
                            }
                        }
                        else
                        {
                            // 空间不足，记录惩罚
                            SchedulePunishment(nearestPlayer, "砸罐子");
                            if (IsPlayerMessageEnabled(nearestPlayer))
                            {
                                nearestPlayer.SendWarningMessage($"[{Name}] 你头顶空间不足，巨石雨将在你有足够空间时落下！");
                            }
                        }
                    }

                    // 处理所有自定义掉落物
                    ProcessAllCustomDrops(i, j, 掉落来源.罐子, "砸罐子", nearestPlayer);
                }
                catch (Exception ex)
                {
                    TShock.Log.ConsoleError($"[{Name}] 处理砸罐子时出错: {ex}");
                }
            }
        }

        // 处理所有类型的自定义掉落物
        private void ProcessAllCustomDrops(int x, int y, 掉落来源 source, string sourceName, TSPlayer player)
        {
            if (Config == null || player == null) return;

            // 合并所有掉落列表
            var allDrops = new List<自定义掉落>();
            allDrops.AddRange(Config.武器掉落 ?? new List<自定义掉落>());
            allDrops.AddRange(Config.矿物掉落 ?? new List<自定义掉落>());
            allDrops.AddRange(Config.材料掉落 ?? new List<自定义掉落>());
            allDrops.AddRange(Config.盔甲掉落 ?? new List<自定义掉落>());
            allDrops.AddRange(Config.其他掉落 ?? new List<自定义掉落>());

            foreach (var drop in allDrops)
            {
                if (drop == null) continue;
                if ((drop.掉落来源 & source) == 0) continue;
                if (!CheckProgressionRequirement(drop.进度要求)) continue;

                if (random.NextDouble() * 100 < drop.掉落概率)
                {
                    SpawnCustomItem(x, y, drop, player);
                    
                    if (drop.显示提示信息 && IsPlayerMessageEnabled(player))
                    {
                        player.SendSuccessMessage($"[{Name}] {sourceName}触发了自定义掉落: {GetItemName(drop.物品ID)} x{drop.物品数量}");
                    }
                }
            }
        }

        // 检查进度要求
        private bool CheckProgressionRequirement(进度限制 requirement)
        {
            switch (requirement)
            {
                case 进度限制.无:
                    return true;
                case 进度限制.骷髅王前:
                    return !NPC.downedBoss3; // 骷髅王
                case 进度限制.肉山前:
                    return !Main.hardMode;
                case 进度限制.肉山后:
                    return Main.hardMode;
                case 进度限制.世纪之花后:
                    return NPC.downedPlantBoss;
                case 进度限制.石巨人后:
                    return NPC.downedGolemBoss;
                case 进度限制.月亮领主后:
                    return NPC.downedMoonlord;
                default:
                    return true;
            }
        }

        // 生成自定义物品
        private void SpawnCustomItem(int tileX, int tileY, 自定义掉落 drop, TSPlayer player)
        {
            try
            {
                if (drop == null || player == null) return;

                IEntitySource source = new EntitySource_WorldEvent();
                Vector2 position = new Vector2(tileX * 16f + 8f, tileY * 16f);
                
                if (drop.掉落来源.HasFlag(掉落来源.树))
                {
                    position.Y = tileY * 16f - 40f;
                }
                else if (drop.掉落来源.HasFlag(掉落来源.罐子))
                {
                    position.Y -= 24f;
                }
                
                position.X += (float)(random.NextDouble() - 0.5) * 32f;
                
                int itemIndex = Item.NewItem(
                    source,
                    position,
                    new Vector2(8f, 8f),
                    drop.物品ID,
                    drop.物品数量,
                    false,
                    0,
                    false,
                    true
                );

                if (itemIndex >= 0 && itemIndex < Main.maxItems)
                {
                    TSPlayer.All.SendData(PacketTypes.ItemDrop, "", itemIndex);
                }
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError($"[{Name}] 生成自定义物品时出错: {ex}");
            }
        }

        // 查找最近的玩家
        private TSPlayer FindNearestPlayer(int x, int y)
        {
            TSPlayer nearestPlayer = null;
            float nearestDistance = float.MaxValue;
            
            foreach (TSPlayer player in TShock.Players)
            {
                if (player != null && player.Active && player.TPlayer != null)
                {
                    float distance = Math.Abs(player.TileX - x) + Math.Abs(player.TileY - y);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestPlayer = player;
                    }
                }
            }
            
            return nearestPlayer;
        }

        // 检查玩家上方是否有足够空间
        private bool CheckSpaceAbovePlayer(TSPlayer player)
        {
            if (player?.TPlayer == null) return false;
            
            int centerX = (int)(player.X / 16f);
            int feetY = (int)(player.Y / 16f);
            int headY = feetY - 3; // 玩家高度大约3格
            
            // 检查玩家上方10格内是否有足够的空间
            for (int checkY = headY - 10; checkY < headY; checkY++)
            {
                for (int checkX = centerX - 2; checkX <= centerX + 2; checkX++)
                {
                    if (checkX < 0 || checkX >= Main.maxTilesX || checkY < 0 || checkY >= Main.maxTilesY)
                        continue;
                        
                    ITile tile = Main.tile[checkX, checkY];
                    if (tile.active() && Main.tileSolid[tile.type])
                    {
                        return false;
                    }
                }
            }
            
            return true;
        }

        // 生成巨石雨
        private void SpawnBoulderRain(TSPlayer player, string source)
        {
            if (player?.TPlayer == null) return;

            var config = Config.巨石雨配置;
            int boulderCount = random.Next(config.最小巨石数量, config.最大巨石数量 + 1);
            
            for (int i = 0; i < boulderCount; i++)
            {
                Vector2 position = new Vector2(
                    player.X + (float)(random.NextDouble() - 0.5) * config.覆盖范围,
                    player.Y - 200f - (float)random.NextDouble() * 100f // 从玩家上方200-300像素处开始
                );
                
                Vector2 velocity = new Vector2(
                    (float)(random.NextDouble() - 0.5) * 4f,
                    2f + (float)random.NextDouble() * 3f
                );
                
                IEntitySource entitySource = new EntitySource_WorldEvent();
                
                int projectileIndex = Projectile.NewProjectile(
                    entitySource,
                    position,
                    velocity,
                    config.巨石弹幕ID,
                    config.巨石伤害值,
                    config.巨石击退力,
                    Main.myPlayer
                );

                if (projectileIndex >= 0 && projectileIndex < Main.maxProjectiles && 
                    Main.projectile[projectileIndex] != null && Main.projectile[projectileIndex].active)
                {
                    NetMessage.SendData((int)PacketTypes.ProjectileNew, -1, -1, null, projectileIndex);
                }
            }
            
            TShock.Log.ConsoleDebug($"[{Name}] 为玩家 {player.Name} 生成了 {boulderCount} 个巨石 ({source})");
        }

        // 安排惩罚
        private void SchedulePunishment(TSPlayer player, string reason)
        {
            if (player?.Index < 0) return;
            
            _playerPunishments[player.Index] = new PlayerPunishment
            {
                PlayerIndex = player.Index,
                ScheduledTime = DateTime.Now.AddSeconds(30), // 30秒后检查
                Reason = reason
            };
            
            TShock.Log.ConsoleDebug($"[{Name}] 为玩家 {player.Name} 安排了巨石雨惩罚 ({reason})");
        }

        // 游戏更新事件，用于处理惩罚
        private void OnGameUpdate(EventArgs args)
        {
            try
            {
                var now = DateTime.Now;
                var completedPunishments = new List<int>();
                
                foreach (var kvp in _playerPunishments)
                {
                    var punishment = kvp.Value;
                    if (now >= punishment.ScheduledTime)
                    {
                        var player = TShock.Players[punishment.PlayerIndex];
                        if (player != null && player.Active)
                        {
                            if (CheckSpaceAbovePlayer(player))
                            {
                                // 有足够空间，执行惩罚
                                SpawnBoulderRain(player, $"惩罚({punishment.Reason})");
                                if (IsPlayerMessageEnabled(player))
                                {
                                    player.SendWarningMessage($"[{Name}] 延迟的巨石雨落下了！");
                                }
                                completedPunishments.Add(kvp.Key);
                            }
                            else
                            {
                                // 仍然没有空间，重新安排
                                punishment.ScheduledTime = now.AddSeconds(10);
                            }
                        }
                        else
                        {
                            // 玩家不在线，移除惩罚
                            completedPunishments.Add(kvp.Key);
                        }
                    }
                }
                
                // 移除已完成的惩罚
                foreach (int key in completedPunishments)
                {
                    _playerPunishments.Remove(key);
                }
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError($"[{Name}] 处理惩罚时出错: {ex}");
            }
        }

        // 获取物品名称
        private string GetItemName(int itemID)
        {
            if (itemID >= 0 && itemID < ItemID.Count)
            {
                var item = new Item();
                item.SetDefaults(itemID);
                return item.Name;
            }
            return $"物品ID:{itemID}";
        }

        // 查找树叶顶部位置
        private int FindLeafTop(int x, int startY)
        {
            int currentY = startY;
            
            while (currentY > 10)
            {
                ITile tile = Main.tile[x, currentY];
                if (tile.active() && _leafTiles.Contains(tile.type))
                {
                    while (currentY > 10 && Main.tile[x, currentY - 1].active() && _leafTiles.Contains(Main.tile[x, currentY - 1].type))
                    {
                        currentY--;
                    }
                    return currentY;
                }
                currentY--;
            }
            
            return startY - 3;
        }
    }

    // 进度限制枚举
    public enum 进度限制
    {
        无 = 0,
        骷髅王前 = 1,
        肉山前 = 2,
        肉山后 = 3,
        世纪之花后 = 4,
        石巨人后 = 5,
        月亮领主后 = 6 
    }

    // 掉落源枚举
    [Flags]
    public enum 掉落来源
    {
        无 = 0,
        树 = 1,
        罐子 = 2,
        两者 = 3
    }

    // 自定义掉落配置
    public class 自定义掉落
    {
        [Description("物品ID")]
        [JsonProperty("物品ID")]
        public int 物品ID { get; set; } = 1;

        [Description("物品数量")]
        [JsonProperty("物品数量")]
        public int 物品数量 { get; set; } = 1;

        [Description("掉落概率 (0-100)")]
        [JsonProperty("掉落概率")]
        public float 掉落概率 { get; set; } = 10f;

        [Description("掉落来源 (1=树, 2=罐子, 3=两者)")]
        [JsonProperty("掉落来源")]
        public 掉落来源 掉落来源 { get; set; } = 掉落来源.两者;

        [Description("是否显示提示信息")]
        [JsonProperty("显示提示信息")]
        public bool 显示提示信息 { get; set; } = true;

        [Description("进度限制")]
        [JsonProperty("进度要求")]
        public 进度限制 进度要求 { get; set; } = 进度限制.无;

        public 自定义掉落() { }

        public 自定义掉落(int 物品ID, int 物品数量 = 1, float 掉落概率 = 10f, 掉落来源 掉落来源 = 掉落来源.两者, 
            bool 显示提示信息 = true, 进度限制 进度要求 = 进度限制.无)
        {
            this.物品ID = 物品ID;
            this.物品数量 = 物品数量;
            this.掉落概率 = 掉落概率;
            this.掉落来源 = 掉落来源;
            this.显示提示信息 = 显示提示信息;
            this.进度要求 = 进度要求;
        }
    }

    // 巨石雨配置
    public class 巨石雨配置
    {
        [Description("巨石雨掉落概率 (0-100)")]
        [JsonProperty("掉落概率")]
        public float 掉落概率 { get; set; } = 15f;

        [Description("巨石弹幕的ID")]
        [JsonProperty("巨石弹幕ID")]
        public int 巨石弹幕ID { get; set; } = 99;

        [Description("巨石弹幕的伤害值")]
        [JsonProperty("巨石伤害值")]
        public int 巨石伤害值 { get; set; } = 60;

        [Description("巨石弹幕的击退力")]
        [JsonProperty("巨石击退力")]
        public float 巨石击退力 { get; set; } = 6f;

        [Description("最小巨石数量")]
        [JsonProperty("最小巨石数量")]
        public int 最小巨石数量 { get; set; } = 3;

        [Description("最大巨石数量")]
        [JsonProperty("最大巨石数量")]
        public int 最大巨石数量 { get; set; } = 8;

        [Description("巨石雨覆盖范围(像素)")]
        [JsonProperty("覆盖范围")]
        public float 覆盖范围 { get; set; } = 400f;

        [Description("是否显示提示信息")]
        [JsonProperty("显示提示信息")]
        public bool 显示提示信息 { get; set; } = true;
    }

    // 玩家惩罚信息
    public class PlayerPunishment
    {
        public int PlayerIndex { get; set; }
        public DateTime ScheduledTime { get; set; }
        public string Reason { get; set; } = "";
    }

    // 主配置文件
    public class Config
    {
        [Description("巨石雨配置")]
        [JsonProperty("巨石雨配置")]
        public 巨石雨配置 巨石雨配置 { get; set; } = new 巨石雨配置();

        [Description("武器掉落列表")]
        [JsonProperty("武器掉落")]
        public List<自定义掉落> 武器掉落 { get; set; } = new List<自定义掉落>();

        [Description("矿物掉落列表")]
        [JsonProperty("矿物掉落")]
        public List<自定义掉落> 矿物掉落 { get; set; } = new List<自定义掉落>();

        [Description("材料掉落列表")]
        [JsonProperty("材料掉落")]
        public List<自定义掉落> 材料掉落 { get; set; } = new List<自定义掉落>();

        [Description("盔甲掉落列表")]
        [JsonProperty("盔甲掉落")]
        public List<自定义掉落> 盔甲掉落 { get; set; } = new List<自定义掉落>();

        [Description("其他物品掉落列表")]
        [JsonProperty("其他掉落")]
        public List<自定义掉落> 其他掉落 { get; set; } = new List<自定义掉落>();

        public static Config Read(string path)
        {
            if (!File.Exists(path))
            {
                return new Config();
            }
            
            try
            {
                string json = File.ReadAllText(path);
                var config = JsonConvert.DeserializeObject<Config>(json);
                
                if (config == null)
                    return new Config();
                
                // 确保所有配置不为null
                config.巨石雨配置 ??= new 巨石雨配置();
                config.武器掉落 ??= new List<自定义掉落>();
                config.矿物掉落 ??= new List<自定义掉落>();
                config.材料掉落 ??= new List<自定义掉落>();
                config.盔甲掉落 ??= new List<自定义掉落>();
                config.其他掉落 ??= new List<自定义掉落>();
                
                return config;
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError($"[TreeAndPotDropReplacer] 读取配置文件时出错: {ex}");
                return new Config();
            }
        }

        public void Write(string path)
        {
            try
            {
                string directory = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                    
                string json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError($"[TreeAndPotDropReplacer] 写入配置文件时出错: {ex}");
            }
        }
    }
}