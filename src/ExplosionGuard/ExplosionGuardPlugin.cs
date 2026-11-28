using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using MonoMod.RuntimeDetour;
using Newtonsoft.Json;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace ExplosionGuard
{
    /// <summary>
    /// ExplosionGuard —— 爆炸守卫：非玩家弹幕的爆炸不破坏图格，只保留伤害。
    ///
    /// 原理（依据 1.4.5.8 源码验证）：
    /// - 所有爆炸类弹幕（炸弹/手榴弹/火箭/核弹等）的图格破坏都汇入 Projectile.ExplodeTiles，
    ///   共两条调用路径：Kill_ExplodeTiles（常规爆炸）与 Kill_Bombs_DoUsualKillCode（特殊巨型爆炸）；
    /// - 专用服务器 Main.myPlayer = 255（Netplay.cs:713）：
    ///   * NPC 弹幕（机械骷髅王炸弹、蛾怪弹等）owner = 255 → 通过 "owner != Main.myPlayer" 门，
    ///     毁图在服务器端执行；
    ///   * 玩家弹幕 owner = 玩家槽位(0~254) ≠ 255 → 服务器侧 Kill_ExplodeTiles 直接早退，
    ///     破图由弹幕主人的客户端执行，再以 TileKill 包同步回服务器。
    /// - 因此本插件 detour ExplodeTiles：owner 不是玩家槽位的弹幕直接跳过（只丢伤害不断图）；
    ///   玩家弹幕要么走客户端破图路径（不受本 detour 影响），要么 owner 合法放行。
    /// - 爆炸对玩家/NPC 的伤害在另一段代码（Kill_Bombs_DoUsualKillCode）里，与图格破坏无关，完整保留。
    ///
    /// 白名单：列出的弹幕 ID 即使 owner 非玩家也保留破图；黑名单：即使玩家发射也禁破图。
    /// detour 绑定失败时插件自动降级为无操作，不影响服务器其余功能。
    /// </summary>
    [ApiVersion(2, 1)]
    public class ExplosionGuardPlugin : TerrariaPlugin
    {
        public override string Name => "ExplosionGuard";
        public override string Author => "不是现在";
        public override string Description => "非玩家弹幕（BOSS炸弹等）的爆炸只保留伤害，不破坏图格；玩家炸弹/火箭/核弹不受影响。";
        public override Version Version => new Version(1, 0, 0);

        private static string ConfigPath => Path.Combine(TShock.SavePath, "爆炸守卫.json");

        private IDisposable? _detour;

        internal sealed class GuardConfig
        {
            [JsonProperty("插件开关")]
            public bool Enabled = true;

            [JsonProperty("白名单-这些弹幕ID即使非玩家发射也保留破图")]
            public List<int> AlwaysAllowIds = new();

            [JsonProperty("黑名单-这些弹幕ID即使玩家发射也禁止破图")]
            public List<int> AlwaysBlockIds = new();
        }

        private GuardConfig _config = new();

        public ExplosionGuardPlugin(Main game) : base(game)
        {
        }

        public override void Initialize()
        {
            LoadConfig();

            // 所有触碰 MonoMod 的代码集中在 DetourBridge，绑定失败自动降级
            try
            {
                ExplodeTilesDetourDelegate detour = ExplodeTilesDetour;
                _detour = DetourBridge.TryCreate(detour);
            }
            catch (Exception ex)
            {
                _detour = null;
                TShock.Log.ConsoleWarn("[ExplosionGuard] ExplodeTiles detour 绑定失败，插件降级为无操作：{0}", ex.Message);
            }

            Commands.ChatCommands.Add(new Command("explosionguard.reload", ReloadCommand, "egreload"));
            Commands.ChatCommands.Add(new Command("eginfo", InfoCommand, "eg"));
            GeneralHooks.ReloadEvent += OnReload;

            TShock.Log.ConsoleInfo(
                "[ExplosionGuard] v{0} 已加载：模式={1}，开关={2}，白名单 {3} 项，黑名单 {4} 项",
                Version, _detour is not null ? "生效(detour)" : "降级(无操作)", _config.Enabled,
                _config.AlwaysAllowIds.Count, _config.AlwaysBlockIds.Count);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _detour?.Dispose();
                GeneralHooks.ReloadEvent -= OnReload;
                Commands.ChatCommands.RemoveAll(x => x.CommandDelegate is not null && (x.Name == "egreload" || x.Name == "eg"));
            }
            base.Dispose(disposing);
        }

        private void OnReload(ReloadEventArgs e) => LoadConfig();

        // ------------------------------------------------------------------
        // 配置
        // ------------------------------------------------------------------

        private void LoadConfig()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    _config = JsonConvert.DeserializeObject<GuardConfig>(File.ReadAllText(ConfigPath)) ?? new GuardConfig();
                }
                else
                {
                    _config = new GuardConfig();
                    SaveConfig();
                    TShock.Log.ConsoleInfo("[ExplosionGuard] 已生成默认配置：{0}", ConfigPath);
                }
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError("[ExplosionGuard] 配置读取失败，使用默认值：{0}", ex.Message);
                _config = new GuardConfig();
            }
        }

        private void SaveConfig()
        {
            File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(_config, Formatting.Indented));
        }

        // ------------------------------------------------------------------
        // 拦截：ExplodeTiles 是所有爆炸毁图的必经之路（实例方法，this = 弹幕）
        // ------------------------------------------------------------------

        internal bool ShouldSkipTileDestruction(Projectile proj)
        {
            if (!_config.Enabled)
                return false;

            if (_config.AlwaysAllowIds.Contains(proj.type))
                return false;
            if (_config.AlwaysBlockIds.Contains(proj.type))
                return true;

            // owner 不是玩家槽位（0~254）= NPC/系统弹幕 → 不破坏图格，伤害保留
            return proj.owner < 0 || proj.owner >= 255;
        }

        private void ExplodeTilesDetour(ExplodeTilesOrig orig, Projectile self, Vector2 compareSpot,
            int radius, int minI, int maxI, int minJ, int maxJ, bool wallSplode, bool explodeHardmodeOres)
        {
            try
            {
                if (ShouldSkipTileDestruction(self))
                    return;
                orig(self, compareSpot, radius, minI, maxI, minJ, maxJ, wallSplode, explodeHardmodeOres);
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError("[ExplosionGuard] detour 处理异常：{0}", ex);
                orig(self, compareSpot, radius, minI, maxI, minJ, maxJ, wallSplode, explodeHardmodeOres);
            }
        }

        // ------------------------------------------------------------------
        // 指令
        // ------------------------------------------------------------------

        private void ReloadCommand(CommandArgs args)
        {
            LoadConfig();
            args.Player.SendSuccessMessage("[ExplosionGuard] 已重载：开关={0}，白名单 {1} 项，黑名单 {2} 项",
                _config.Enabled, _config.AlwaysAllowIds.Count, _config.AlwaysBlockIds.Count);
        }

        private void InfoCommand(CommandArgs args)
        {
            args.Player.SendInfoMessage("[ExplosionGuard] NPC/非玩家弹幕爆炸不破坏图格（伤害保留），玩家炸弹/火箭/核弹照常");
            args.Player.SendInfoMessage("开关：{0}　白名单：[{1}]　黑名单：[{2}]",
                _config.Enabled,
                string.Join(",", _config.AlwaysAllowIds),
                string.Join(",", _config.AlwaysBlockIds));
        }
    }
}
