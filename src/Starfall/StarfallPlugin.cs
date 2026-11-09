using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.Localization;
using TerrariaApi.Server;
using TShockAPI;

namespace Starfall
{
    /// <summary>
    /// 服务端复刻版：控制坠落之星生成速率与掉落，并可调整原版天气事件的倍率。
    ///
    /// 背景：
    /// - 原版落星生成完全在客户端完成（netMode != 2），所以服务端无法直接改写它的倍率；
    ///   本插件改为由服务端主动生成“坠落之星”弹幕（type 12），并自己接管掉落。
    /// - 天气事件（下雨/风/沙尘暴/钱币雨/陨石雨）则是服务端权威的，状态通过 7 号包(WorldData)
    ///   同步给客户端，因此可以直接在服务端读写它们的字段来提升频率/强度/数量。
    /// </summary>
    [ApiVersion(2, 1)]
    public class StarfallPlugin : TerrariaPlugin
    {
        public override string Name => "Starfall";
        public override string Author => "不是现在";
        public override string Description => GetString("服务端控制坠落之星生成速率、掉落与天气事件倍率。");
        public override Version Version => new Version(2, 0, 3, 0);

        internal static StarfallConfig Config;

        private static string ConfigPath => Path.Combine(TShock.SavePath, "Starfall.json");

        // 原版参考值：小世界平均约 40.5 颗/夜，夜长 Main.nightLength = 32400 tick。
        private const double VanillaStarsPerTick = 40.5 / 32400.0;

        // 与原版落星一致的生成范围（已不再作为配置项暴露）。
        private const int HorizontalSpreadTiles = 40;
        private const int SpawnHeightTiles = 40;
        // 每 tick 最多生成的颗数（纯安全上限，防止极端倍率卡住主线程）。
        private const int MaxSpawnsPerTick = 20;

        // 原版天气触发常量（见 Main.UpdateTime / Sandstorm.UpdateTime）。
        private const double RainDenominator = 86400.0 * 5.75; // 下雨：每 tick 约 dayRate / 496800
        private const double SandstormWindThreshold = 0.6;     // 沙尘暴需要 |windSpeedCurrent| >= 0.6
        private const float WindClamp = 0.8f;                  // 风力上限（与原版一致）

        // 7 号包 = WorldData（携带 windSpeedTarget / maxRaining / 沙尘暴状态），用于强制同步天气。
        private const int MsgWorldData = 7;

        private double _accumulator;

        // 跟踪本插件生成的落星（弹幕索引 -> 最后一次已知中心点），用于在它消失时掉落战利品。
        private readonly Dictionary<int, Vector2> _trackedStars = new Dictionary<int, Vector2>();

        private int _lastWorldId;
        private float _lastWindTarget = float.NaN;
        private double _lastCoinRain;
        private int _lastMeteorCount;
        private bool _lastRaining;
        private bool _lastDayTime;

        public StarfallPlugin(Main game) : base(game)
        {
            Order = 1;
        }

        public override void Initialize()
        {
            Config = StarfallConfig.Load(ConfigPath);

            ServerApi.Hooks.GameUpdate.Register(this, OnGameUpdate);
            Commands.ChatCommands.Add(new Command("starfall.admin", OnCommand, "starfall", "sf"));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameUpdate.Deregister(this, OnGameUpdate);
                Commands.ChatCommands.RemoveAll(c => c.Names.Contains("starfall"));
            }

            base.Dispose(disposing);
        }

        private void OnGameUpdate(EventArgs args)
        {
            if (Main.gameMenu || Main.maxTilesX <= 0)
                return;

            // 切换世界时清空跟踪与累计状态，避免把上个世界的落星位置/预算带过来。
            if (Main.worldID != _lastWorldId)
            {
                _lastWorldId = Main.worldID;
                _trackedStars.Clear();
                _accumulator = 0;
                _lastWindTarget = float.NaN;
                _lastCoinRain = 0;
                _lastMeteorCount = 0;
                // 初始化为当前状态，防止首 tick 误判“正在下雨/刚入夜”的事件跃迁。
                _lastRaining = Main.raining;
                _lastDayTime = Main.dayTime;
            }

            // 先结算“已消失的落星”再生成新的：先检测后生成可避免弹幕槽位复用造成误判。
            ProcessStarDrops();

            if (!Config.Enabled)
                return;

            if (Config.EnableWeather)
                ApplyWeather();

            if (Config.EnableStars)
                SpawnStars();
        }

        // ===================== 落星生成 =====================

        private void SpawnStars()
        {
            bool day = Main.dayTime;
            if (day && !Config.AllowDaytimeStars)
            {
                _accumulator = 0;
                return;
            }

            double mult = Config.StarMultiplier * (day ? Config.DaytimeMultiplier : 1.0);
            if (mult <= 0.0)
            {
                _accumulator = 0;
                return;
            }

            _accumulator += VanillaStarsPerTick * mult;

            // 每 tick 最多生成的颗数，避免极端倍率下一帧刷太多。
            int maxPerTick = MaxSpawnsPerTick;
            int spawned = 0;

            while (_accumulator >= 1.0 && spawned < maxPerTick)
            {
                if (!SpawnStar())
                {
                    _accumulator = 0;
                    break;
                }

                _accumulator -= 1.0;
                spawned++;
            }

            // 丢弃积压，避免下一帧爆炸。
            if (_accumulator > 1.0)
                _accumulator = 1.0;
        }

        /// <summary>
        /// 在某个在线玩家上方生成一颗服务端落星。
        /// 遵循原版规则（Main.cs 中 Star.UpdateStars 仅当屏幕位于世界地表附近时运行）：
        /// 只有身处地表层（position.Y ≤ 世界地表 + 1 格）的在线玩家才会触发落星；
        /// 地下/深入洞穴的玩家不会出现落星。
        /// </summary>
        private bool SpawnStar()
        {
            float surfaceY = (float)(Main.worldSurface * 16.0) + 16f;

            var players = new List<TSPlayer>();
            if (TShock.Players != null)
            {
                foreach (var p in TShock.Players)
                {
                    if (p != null && p.Active && p.TPlayer != null && !p.TPlayer.dead &&
                        p.TPlayer.position.Y <= surfaceY)
                        players.Add(p);
                }
            }

            if (players.Count == 0)
                return false;

            TSPlayer target = players[Main.rand.Next(players.Count)];

            float centerX = target.TPlayer.Center.X;
            float playerTop = target.TPlayer.position.Y;

            float x = centerX + Main.rand.Next(-HorizontalSpreadTiles, HorizontalSpreadTiles + 1) * 16f;
            float y = playerTop - SpawnHeightTiles * 16f;
            if (y < 16f)
                y = 16f;

            // 与原版 Star.Fall() 一致的初速度范围。
            float vx = Main.rand.Next(-400, 401) * 0.01f;
            float vy = Main.rand.Next(700, 1001) * 0.01f;

            // 注意：服务端 NewProjectile 要求 Owner == Main.myPlayer（服务器上为 255），否则会断言失败
            // 且不会同步/结算。用 Main.myPlayer 让服务端权威模拟这颗落星。
            int index = Projectile.NewProjectile(new EntitySource_WorldEvent(), x, y, vx, vy, 12, Config.Damage, 10f, Main.myPlayer);
            if (index < 0 || index >= Main.maxProjectiles)
                return false;

            _trackedStars[index] = Main.projectile[index].Center;
            return true;
        }

        // ===================== 落星掉落 =====================

        private void ProcessStarDrops()
        {
            if (_trackedStars.Count == 0)
                return;

            // 先取键快照，避免在遍历中修改字典。
            List<int> indices = new List<int>(_trackedStars.Keys);
            foreach (int i in indices)
            {
                Projectile p = (i >= 0 && i < Main.maxProjectiles) ? Main.projectile[i] : null;

                if (p != null && p.active && p.type == 12 && p.owner == Main.myPlayer)
                {
                    // 还活着：记录最新位置。
                    _trackedStars[i] = p.Center;
                }
                else
                {
                    // 落星已消失（落地爆炸 / 命中 / 超时），按掉落表产出物品。
                    Vector2 pos = _trackedStars[i];
                    _trackedStars.Remove(i);
                    DropLoot(pos);
                }
            }
        }

        private void DropLoot(Vector2 position)
        {
            var drops = Config.Drops;
            if (drops == null || drops.Count == 0)
                return;

            double total = 0.0;
            foreach (var d in drops)
            {
                if (d != null && d.Weight > 0.0)
                    total += d.Weight;
            }

            if (total <= 0.0)
                return;

            double roll = Main.rand.NextDouble() * total;
            DropEntry chosen = null;
            foreach (var d in drops)
            {
                if (d == null || d.Weight <= 0.0)
                    continue;

                roll -= d.Weight;
                if (roll <= 0.0)
                {
                    chosen = d;
                    break;
                }
            }

            if (chosen == null)
                chosen = drops[drops.Count - 1];
            if (chosen == null || chosen.ItemId <= 0)
                return;

            int stack = Math.Max(1, chosen.Stack);
            Item.NewItem(new EntitySource_WorldEvent(), position, chosen.ItemId, stack);
        }

        // ===================== 天气倍率 =====================

        private void ApplyWeather()
        {
            ApplyRain();
            ApplyWind();
            ApplySandstorm();
            ApplyCoinRainChance();
            ApplyCoinRain();
            ApplyMeteorShowerChance();
            ApplyMeteorShower();
        }

        /// <summary>下雨频率倍率：在“本应可以开始下雨”的判定上，额外补掷若干概率。</summary>
        private void ApplyRain()
        {
            double mult = Config.RainMultiplier;
            if (mult <= 1.0 || Main.dayRate == 0)
                return;

            // 对齐原版条件：地表世界、未在下雨、非史莱姆雨、且至少有一名 120 生命上限的玩家。
            if (Main.raining || Main.slimeRain || !Main.isThereAWorldSurface || Main.remixWorld)
                return;

            if (!HasQualifiedPlayer())
                return;

            double vanillaChance = Main.dayRate / RainDenominator;
            if (RollExtra(vanillaChance, mult))
                Main.StartRain();
        }

        /// <summary>风倍率：放大服务端随机出的风力目标值（越大越容易出现大风天）。</summary>
        private void ApplyWind()
        {
            double mult = Config.WindMultiplier;
            float current = Main.windSpeedTarget;

            if (mult == 1.0)
            {
                _lastWindTarget = current;
                return;
            }

            // 仅在原版刚刚改了风向/风力目标时（值与我们上次写入的不同）缩放，避免每 tick 复利放大。
            if (float.IsNaN(_lastWindTarget) || Math.Abs(current - _lastWindTarget) > 0.0005f)
            {
                float scaled = (float)Math.Clamp(current * mult, -WindClamp, WindClamp);
                Main.windSpeedTarget = scaled;
                _lastWindTarget = scaled;
                NetMessage.SendData(MsgWorldData);
            }
        }

        /// <summary>沙尘暴频率倍率：在可触发条件下额外补掷概率，直接置位并同步。</summary>
        private void ApplySandstorm()
        {
            double mult = Config.SandstormMultiplier;
            if (mult <= 1.0 || Main.dayRate == 0)
                return;

            if (Sandstorm.Happening || Math.Abs(Main.windSpeedCurrent) < SandstormWindThreshold)
                return;

            int denominator = Main.hardMode ? 43200 : 64800;
            double vanillaChance = Main.dayRate / (double)denominator;
            if (!RollExtra(vanillaChance, mult))
                return;

            // StartSandstorm() 是 private，这里等价地直接置位（与源码一致）。
            Sandstorm.Happening = true;
            Sandstorm.TimeLeft = Main.rand.Next(28800, 86401);
            Sandstorm.IntendedSeverity = 0.4f + Main.rand.NextFloat();
            NetMessage.SendData(MsgWorldData);
        }

        /// <summary>
        /// 钱币雨概率倍率：在“每次开始下雨”这个事件上，于原版 1/25 幸运判定之外额外补掷一份，
        /// 命中即按原版规则手动开启钱币雨（预算赋值 + 广播）。总发生概率约 ×倍率。
        /// </summary>
        private void ApplyCoinRainChance()
        {
            double mult = Config.CoinRainChanceMultiplier;
            if (mult <= 1.0)
                return;

            bool rainingNow = Main.raining;
            if (rainingNow && !_lastRaining && Config.EnableWeather)
            {
                // 原版每次下雨开始时钱币雨的概率约为 1/25（受幸运影响，见 Main.StartRain）。
                if (RollExtra(1.0 / 25.0, mult))
                    StartCoinRain();
            }

            _lastRaining = rainingNow;
        }

        /// <summary>按原版 StartRain 的方式开启一场钱币雨（预算赋值 + 广播）。预算随后由产出倍率放大。</summary>
        private void StartCoinRain()
        {
            float scale = (float)Main.maxTilesX / 4200f;
            Main.coinRain = Main.rand.Next(75, 151) * 100 * 100;
            Main.coinRain = (int)((float)Main.coinRain * scale);
            ChatHelper.BroadcastChatMessage(NetworkText.FromKey(Lang.gen[93].Key), new Color(255, 200, 150));
        }

        /// <summary>钱币雨产出倍率：在原版刚启动一场钱币雨（预算由 0 变正）时，放大其数量预算。</summary>
        private void ApplyCoinRain()
        {
            double mult = Config.CoinRainYieldMultiplier;
            if (mult > 1.0 && _lastCoinRain <= 0.0 && Main.coinRain > 0)
            {
                double scaled = Main.coinRain * mult;
                Main.coinRain = scaled >= int.MaxValue ? int.MaxValue : (int)scaled;
            }

            _lastCoinRain = Main.coinRain;
        }

        /// <summary>
        /// 陨石雨概率倍率：原版每晚约 1/50 尝试坠落陨石、坠落失败再 1/3 转陨石雨（≈ 1/150）；
        /// 插件在每次入夜额外补掷一份，命中即直接开启陨石雨，总发生概率约 ×倍率。
        /// 与原版一致，需已击败克脑/世界吞噬者（NPC.downedBoss2）才有效。
        /// </summary>
        private void ApplyMeteorShowerChance()
        {
            double mult = Config.MeteorShowerChanceMultiplier;
            if (mult <= 1.0)
                return;

            bool dayNow = Main.dayTime;
            if (!dayNow && _lastDayTime && Config.EnableWeather
                && NPC.downedBoss2 && Main.isThereAWorldSurface && !Main.remixWorld)
            {
                if (RollExtra((1.0 / 50.0) * (1.0 / 3.0), mult))
                    WorldGen.StartMeteorShower();
            }

            _lastDayTime = dayNow;
        }

        /// <summary>陨石雨产出倍率：在原版刚开启一场陨石雨时，放大流星总数。</summary>
        private void ApplyMeteorShower()
        {
            double mult = Config.MeteorShowerYieldMultiplier;
            if (mult > 1.0 && _lastMeteorCount <= 0 && WorldGen.meteorShowerCount > 0)
            {
                double scaled = WorldGen.meteorShowerCount * mult;
                WorldGen.meteorShowerCount = scaled >= int.MaxValue ? int.MaxValue : (int)scaled;
            }

            _lastMeteorCount = WorldGen.meteorShowerCount;
        }

        private static bool HasQualifiedPlayer()
        {
            if (TShock.Players == null)
                return false;

            foreach (var p in TShock.Players)
            {
                if (p != null && p.Active && p.TPlayer != null && p.TPlayer.statLifeMax >= 120)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 让事件总频率约为原版的 multiplier 倍：原版已有 vanillaChance，这里补掷
        /// (multiplier - 1) * vanillaChance 的独立概率。
        /// </summary>
        private static bool RollExtra(double vanillaChance, double multiplier)
        {
            if (multiplier <= 1.0 || vanillaChance <= 0.0)
                return false;

            double extra = vanillaChance * (multiplier - 1.0);
            if (extra >= 1.0)
                return true;

            double inverse = 1.0 / extra;
            if (inverse >= int.MaxValue)
                return false;

            return Main.rand.Next((int)inverse) == 0;
        }

        // ===================== 命令 =====================

        private void OnCommand(CommandArgs args)
        {
            if (args.Parameters.Count == 0 ||
                args.Parameters[0].Equals("status", StringComparison.OrdinalIgnoreCase) ||
                args.Parameters[0].Equals("help", StringComparison.OrdinalIgnoreCase))
            {
                ShowStatus(args.Player);
                return;
            }

            switch (args.Parameters[0].ToLowerInvariant())
            {
                case "rate":
                case "multiplier":
                    if (!TryParsePositive(args, 1, out double rate))
                    {
                        args.Player.SendErrorMessage("用法: /sf rate <正数倍率>");
                        return;
                    }
                    Config.StarMultiplier = Math.Min(rate, 10000.0);
                    SaveAndReply(args.Player, $"[Starfall] 落星频率倍率已设为 x{Config.StarMultiplier}");
                    break;

                case "day":
                    if (!TryParsePositive(args, 1, out double day))
                    {
                        args.Player.SendErrorMessage("用法: /sf day <正数倍率>（白天相对夜晚的倍率）");
                        return;
                    }
                    Config.DaytimeMultiplier = Math.Min(day, 10000.0);
                    SaveAndReply(args.Player, $"[Starfall] 白天频率倍率已设为 x{Config.DaytimeMultiplier}");
                    break;

                case "rain":
                case "wind":
                case "sand":
                case "sandstorm":
                case "coin":
                case "coinyield":
                case "meteor":
                case "metyield":
                    SetWeather(args);
                    break;

                case "drop":
                    HandleDropCommand(args);
                    break;

                case "toggle":
                    Config.Enabled = !Config.Enabled;
                    SaveAndReply(args.Player, $"[Starfall] 总开关已{(Config.Enabled ? "开启" : "关闭")}");
                    break;

                case "stars":
                    Config.EnableStars = !Config.EnableStars;
                    SaveAndReply(args.Player, $"[Starfall] 落星已{(Config.EnableStars ? "开启" : "关闭")}");
                    break;

                case "weather":
                    Config.EnableWeather = !Config.EnableWeather;
                    SaveAndReply(args.Player, $"[Starfall] 天气增强已{(Config.EnableWeather ? "开启" : "关闭")}");
                    break;

                case "spawn":
                    int count = 1;
                    if (args.Parameters.Count >= 2)
                        int.TryParse(args.Parameters[1], out count);
                    count = Math.Clamp(count, 1, 200);

                    int done = 0;
                    for (int i = 0; i < count; i++)
                    {
                        if (SpawnStar())
                            done++;
                    }

                    args.Player.SendSuccessMessage($"[Starfall] 生成了 {done} 颗落星");
                    break;

                case "reload":
                    Config = StarfallConfig.Load(ConfigPath);
                    _lastWindTarget = float.NaN;
                    args.Player.SendSuccessMessage("[Starfall] 配置已重新加载");
                    break;

                default:
                    args.Player.SendErrorMessage("未知子命令。用法: /sf status|rate|day|rain|wind|sand|coin|coinyield|meteor|metyield|drop|toggle|stars|weather|spawn|reload");
                    break;
            }
        }

        private void SetWeather(CommandArgs args)
        {
            if (!TryParsePositive(args, 1, out double value))
            {
                args.Player.SendErrorMessage("用法: /sf <rain|wind|sand|coin|meteor> <正数倍率>");
                return;
            }

            value = Math.Min(value, 1000.0);
            switch (args.Parameters[0].ToLowerInvariant())
            {
                case "rain":
                    Config.RainMultiplier = value;
                    SaveAndReply(args.Player, $"[Starfall] 下雨倍率已设为 x{value}");
                    break;
                case "wind":
                    Config.WindMultiplier = value;
                    _lastWindTarget = float.NaN;
                    SaveAndReply(args.Player, $"[Starfall] 风倍率已设为 x{value}");
                    break;
                case "sand":
                case "sandstorm":
                    Config.SandstormMultiplier = value;
                    SaveAndReply(args.Player, $"[Starfall] 沙尘暴倍率已设为 x{value}");
                    break;
                case "coin":
                    Config.CoinRainChanceMultiplier = value;
                    SaveAndReply(args.Player, $"[Starfall] 钱币雨概率倍率已设为 x{value}");
                    break;
                case "coinyield":
                    Config.CoinRainYieldMultiplier = value;
                    SaveAndReply(args.Player, $"[Starfall] 钱币雨产出倍率已设为 x{value}");
                    break;
                case "meteor":
                    Config.MeteorShowerChanceMultiplier = value;
                    SaveAndReply(args.Player, $"[Starfall] 陨石雨概率倍率已设为 x{value}");
                    break;
                case "metyield":
                    Config.MeteorShowerYieldMultiplier = value;
                    SaveAndReply(args.Player, $"[Starfall] 陨石雨产出倍率已设为 x{value}");
                    break;
            }
        }

        private void HandleDropCommand(CommandArgs args)
        {
            string sub = args.Parameters.Count >= 2 ? args.Parameters[1].ToLowerInvariant() : "list";

            switch (sub)
            {
                case "list":
                    args.Player.SendInfoMessage("[Starfall] 当前掉落表：");
                    if (Config.Drops.Count == 0)
                    {
                        args.Player.SendInfoMessage("  (空)");
                        return;
                    }
                    foreach (var d in Config.Drops)
                        args.Player.SendInfoMessage($"  {d.ItemId} x{d.Stack}  权重 {d.Weight}");
                    break;

                case "clear":
                    Config.Drops.Clear();
                    SaveAndReply(args.Player, "[Starfall] 掉落表已清空（落星将不再掉落物品）");
                    break;

                case "add":
                    if (args.Parameters.Count < 3 || !int.TryParse(args.Parameters[2], out int itemId) || itemId <= 0)
                    {
                        args.Player.SendErrorMessage("用法: /sf drop add <物品ID> [数量] [权重]");
                        return;
                    }

                    int stack = 1;
                    if (args.Parameters.Count >= 4 && int.TryParse(args.Parameters[3], out int s) && s > 0)
                        stack = s;

                    double weight = 1.0;
                    if (args.Parameters.Count >= 5 && double.TryParse(args.Parameters[4], out double w) && w > 0)
                        weight = w;

                    Config.Drops.Add(new DropEntry { ItemId = itemId, Stack = stack, Weight = weight });
                    SaveAndReply(args.Player, $"[Starfall] 已添加掉落：{itemId} x{stack} 权重 {weight}");
                    break;

                default:
                    args.Player.SendErrorMessage("用法: /sf drop [list|add <物品ID> [数量] [权重]|clear]");
                    break;
            }
        }

        private void ShowStatus(TSPlayer player)
        {
            player.SendInfoMessage($"[Starfall] 总开关:{(Config.Enabled ? "开" : "关")}  " +
                                   $"落星:{(Config.EnableStars ? "开" : "关")}  天气:{(Config.EnableWeather ? "开" : "关")}");
            player.SendInfoMessage($"[Starfall] 落星频率 x{Config.StarMultiplier}  白天:{(Config.AllowDaytimeStars ? "开" : "关")} " +
                                   $"x{Config.DaytimeMultiplier}  伤害 {Config.Damage}");
            player.SendInfoMessage($"[Starfall] 天气倍率  雨 x{Config.RainMultiplier}  风 x{Config.WindMultiplier}  " +
                                   $"沙尘暴 x{Config.SandstormMultiplier}");
            player.SendInfoMessage($"[Starfall] 钱币雨 概率 x{Config.CoinRainChanceMultiplier} 产出 x{Config.CoinRainYieldMultiplier}  " +
                                   $"陨石雨 概率 x{Config.MeteorShowerChanceMultiplier} 产出 x{Config.MeteorShowerYieldMultiplier}");
            player.SendInfoMessage("[Starfall] 用法: /sf rate|day|rain|wind|sand|coin|coinyield|meteor|metyield|drop|toggle|stars|weather|spawn|reload");
        }

        private static bool TryParsePositive(CommandArgs args, int index, out double value)
        {
            value = 0;
            return args.Parameters.Count > index
                && double.TryParse(args.Parameters[index], out value)
                && value > 0;
        }

        private void SaveAndReply(TSPlayer player, string message)
        {
            Config.Normalize();
            Config.Save(ConfigPath);
            player.SendSuccessMessage(message);
        }
    }
}
