using LazyAPI;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BanNpc;

[ApiVersion(2, 1)]
public class Plugin : LazyPlugin
{
    public override string Author => "Patrikk,GK 改良 + 唉唉有更新Boss全局分段滑动窗口限流";
    public override string Description => GetString("禁止指定怪物的出没 + NPC全局分段滑动窗口限流");
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 0, 0, 13);

    /// <summary>全局召唤时间记录：NpcId → 该NPC成功召唤的时间戳列表</summary>
    private Dictionary<int, List<DateTime>> _globalSpawnRecords = new Dictionary<int, List<DateTime>>();
    /// <summary>NpcId → 强制锁死到期时间；DateTime.MinValue代表无锁</summary>
    private Dictionary<int, DateTime> _bossLockUntil = new Dictionary<int, DateTime>();
    /// <summary>NpcId → 上一次广播提示时间，用于防抖防刷屏</summary>
    private Dictionary<int, DateTime> _lastBroadcastTime = new Dictionary<int, DateTime>();

    /// <summary>同个Boss限流提示最小广播间隔(秒)</summary>
    private const int BroadcastCooldownSec = 15;

    public Plugin(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        Commands.ChatCommands.Add(new Command("bannpc.use", this.BanCommand, "bm"));
        ServerApi.Hooks.NpcSpawn.Register(this, this.OnSpawn);
        ServerApi.Hooks.NpcTransform.Register(this, this.OnTransform);
        GeneralHooks.ReloadEvent += OnTShockReload;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == BanCommand);
            ServerApi.Hooks.NpcSpawn.Deregister(this, OnSpawn);
            ServerApi.Hooks.NpcTransform.Deregister(this, OnTransform);
            GeneralHooks.ReloadEvent -= OnTShockReload;

            _globalSpawnRecords.Clear();
            _bossLockUntil.Clear();
            _lastBroadcastTime.Clear();
        }
        base.Dispose(disposing);
    }

    private void OnTShockReload(ReloadEventArgs args)
    {
        _globalSpawnRecords.Clear();
        _bossLockUntil.Clear();
        _lastBroadcastTime.Clear();
        TShock.Log.ConsoleInfo("[BanNpc] /reload 已清空全部NPC召唤记录、锁状态、广播防抖状态");
    }

    private void BanCommand(CommandArgs args)
    {
        if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "list")
        {
            if (Config.Instance.BanNpcs.Count < 1)
            {
                args.Player.SendInfoMessage(GetString("当前阻止表为空."));
            }
            else
            {
                args.Player.SendInfoMessage(GetString("阻止怪物表: ") + string.Join(", ", Config.Instance.BanNpcs.Select(x => TShock.Utils.GetNPCById(x)?.FullName + $"({x})")));
            }

            if (Config.Instance.BossRateLimitList.Any())
            {
                var rateText = string.Join(", ", Config.Instance.BossRateLimitList.Select(cfg =>
                {
                    var windows = string.Join(" | ", cfg.RateWindows.Select(w => $"{w.WindowSeconds}s:{w.MaxCount}次"));
                    return $"{TShock.Utils.GetNPCById(cfg.NpcId)?.FullName}({cfg.NpcId}) 窗口[{windows}] 存活上限:{cfg.GlobalMaxAlive} 广播:{cfg.BroadcastDeny}";
                }));
                args.Player.SendInfoMessage($"[速率限制配置]: {rateText}");
            }
            else
            {
                args.Player.SendInfoMessage("[速率限制配置]: 为空");
            }
            return;
        }
        else if (args.Parameters.Count == 2)
        {
            var matchedNPCs = TShock.Utils.GetNPCByIdOrName(args.Parameters[1]);
            if (matchedNPCs.Count == 0)
            {
                args.Player.SendErrorMessage(GetString("无效NPC: {0} !"), args.Parameters[1]);
                return;
            }
            if (matchedNPCs.Count > 1)
            {
                args.Player.SendMultipleMatchError(matchedNPCs.Select(i => i.FullName));
                return;
            }
            NPC npc = matchedNPCs[0];

            switch (args.Parameters[0].ToLower())
            {
                case "add":
                    {
                        if (Config.Instance.BanNpcs.Contains(npc.netID))
                        {
                            args.Player.SendErrorMessage(GetString("NPC ID {0} 已在阻止列表!"), npc.netID);
                            return;
                        }
                        Config.Instance.BanNpcs.Add(npc.netID);
                        Config.Save();
                        args.Player.SendSuccessMessage(GetString("已添加NPC ID {0}到阻止列表"), npc.netID);
                        break;
                    }
                case "delete":
                case "del":
                case "remove":
                    {
                        if (!Config.Instance.BanNpcs.Contains(npc.netID))
                        {
                            args.Player.SendErrorMessage(GetString("NPC ID {0} 不在阻止列表!"), npc.netID);
                            return;
                        }
                        Config.Instance.BanNpcs.Remove(npc.netID);
                        Config.Save();
                        args.Player.SendSuccessMessage(GetString("已从阻止列表删除NPC ID {0}"), npc.netID);
                        break;
                    }
                default:
                    args.Player.SendErrorMessage(GetString("语法错误：/bm <add/del> [NPC名称或ID]"));
                    break;
            }
        }
        else
        {
            args.Player.SendInfoMessage("/bm list");
            args.Player.SendInfoMessage("/bm add [NPC名称或ID]");
            args.Player.SendInfoMessage("/bm del [NPC名称或ID]");
        }
    }

    private void OnTransform(NpcTransformationEventArgs args)
    {
        if (args.Handled) return;
        var targetNpc = Main.npc[args.NpcId];
        if (Config.Instance.BanNpcs.Contains(targetNpc.netID))
        {
            targetNpc.active = false;
        }
    }

    private void OnSpawn(NpcSpawnEventArgs args)
    {
        if (args.Handled) return;
        var npc = Main.npc[args.NpcId];
        int npcId = npc.netID;

        //黑名单优先拦截
        if (Config.Instance.BanNpcs.Contains(npcId))
        {
            args.Handled = true;
            npc.active = false;
            return;
        }

        var rateCfg = Config.Instance.BossRateLimitList.FirstOrDefault(c => c.NpcId == npcId);
        if (rateCfg is null)
            return;

        DateTime now = DateTime.Now;

        if (!_bossLockUntil.ContainsKey(npcId))
            _bossLockUntil[npcId] = DateTime.MinValue;
        if (!_lastBroadcastTime.ContainsKey(npcId))
            _lastBroadcastTime[npcId] = DateTime.MinValue;
        if (!_globalSpawnRecords.ContainsKey(npcId))
            _globalSpawnRecords[npcId] = new List<DateTime>();

        var lockEnd = _bossLockUntil[npcId];
        var lastBroadcast = _lastBroadcastTime[npcId];
        var timeList = _globalSpawnRecords[npcId];

        // BroadcastDeny配置生效：true=开启防抖广播，false=静默拦截
        bool needBroadcast = rateCfg.BroadcastDeny && ((now - lastBroadcast).TotalSeconds > BroadcastCooldownSec);

        // 当前处于强制锁死状态，直接拦截（锁死状态不输出窗口信息，只输出基础提示）
        if (lockEnd > now)
        {
            args.Handled = true;
            npc.active = false;
            if (needBroadcast)
            {
                TShock.Utils.Broadcast(rateCfg.DenyMessage, Microsoft.Xna.Framework.Color.OrangeRed);
                _lastBroadcastTime[npcId] = now;
            }
            return;
        }

        bool isTriggerLimit = false;
        int triggerWindowSec = 0;
        int triggerWindowMaxCount = 0;
        int remainSec = 0;

        if (rateCfg.RateWindows.Any())
        {
            //清理掉所有超出最大窗口时长的过期时间戳
            var maxWindow = rateCfg.RateWindows.Max(w => w.WindowSeconds);
            var expireTime = now.AddSeconds(-maxWindow);
            timeList.RemoveAll(t => t < expireTime);

            //锁刚刚到期，本次成功召唤，重置锁状态，清空历史时间
            bool isLockJustExpired = lockEnd <= now && _bossLockUntil[npcId] != DateTime.MinValue;
            if (isLockJustExpired)
            {
                timeList.Clear();
                _bossLockUntil[npcId] = DateTime.MinValue;
            }

            foreach (var win in rateCfg.RateWindows)
            {
                var cutOff = now.AddSeconds(-win.WindowSeconds);
                int count = timeList.Count(t => t >= cutOff);
                if (count >= win.MaxCount)
                {
                    isTriggerLimit = true;
                    triggerWindowSec = win.WindowSeconds;
                    triggerWindowMaxCount = win.MaxCount;
                    break;
                }
            }

            if (isTriggerLimit)
            {
                //触发超限，设置强制锁死
                _bossLockUntil[npcId] = now.AddSeconds(triggerWindowSec);
                args.Handled = true;
                npc.active = false;
                if (needBroadcast)
                {
                    string finalMsg = $"{rateCfg.DenyMessage} (限制：{triggerWindowSec}s/{triggerWindowMaxCount}次)";
                    TShock.Utils.Broadcast(finalMsg, Microsoft.Xna.Framework.Color.OrangeRed);
                    _lastBroadcastTime[npcId] = now;
                }
                return;
            }

            //放行：追加本次成功召唤时间戳，不再无脑清空全部列表
            timeList.Add(now);
        }
        else if (rateCfg.PlayerCooldownSeconds > 0)
        {
            //简单冷却模式，无强制锁死
            var cutOff = now.AddSeconds(-rateCfg.PlayerCooldownSeconds);
            var lastSpawn = timeList.Where(t => t >= cutOff).OrderByDescending(t => t).FirstOrDefault();
            if (lastSpawn != default)
            {
                remainSec = (int)(lastSpawn.AddSeconds(rateCfg.PlayerCooldownSeconds) - now).TotalSeconds;
                if (remainSec < 0) remainSec = 0;

                args.Handled = true;
                npc.active = false;
                if (needBroadcast)
                {
                    string msg = $"{rateCfg.DenyMessage} 请在 {remainSec} 秒后再尝试！";
                    TShock.Utils.Broadcast(msg, Microsoft.Xna.Framework.Color.OrangeRed);
                    _lastBroadcastTime[npcId] = now;
                }
                return;
            }
            timeList.Add(now);
        }

        //存活上限校验
        if (rateCfg.GlobalMaxAlive > 0)
        {
            int aliveCount = 0;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                var checkNpc = Main.npc[i];
                if (checkNpc.active && checkNpc.netID == rateCfg.NpcId)
                {
                    aliveCount++;
                }
            }
            if (aliveCount > rateCfg.GlobalMaxAlive)
            {
                args.Handled = true;
                npc.active = false;
                if (needBroadcast)
                {
                    TShock.Utils.Broadcast("该Boss当前存活数量已达上限！", Microsoft.Xna.Framework.Color.OrangeRed);
                    _lastBroadcastTime[npcId] = now;
                }
                return;
            }
        }
    }
}