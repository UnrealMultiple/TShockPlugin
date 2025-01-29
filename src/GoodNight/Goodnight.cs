using Microsoft.Xna.Framework;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using static TShockAPI.GetDataHandlers;

namespace Goodnight;

[ApiVersion(2, 1)]
public class Goodnight : TerrariaPlugin
{
    #region 变量与插件信息
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!; public override string Author => "Jonesn 羽学 少司命";
    public override Version Version => new Version(2, 7, 6);
    public override string Description => GetString("设置服务器无法进入或禁止生成怪物的时段");
    internal static Configuration Config = null!;
    #endregion

    #region 构造注册卸载
    public Goodnight(Main game) : base(game) { }

    public override void Initialize()
    {
        LoadConfig();
        NewProjectile += this.NewProj!;
        GeneralHooks.ReloadEvent += LoadConfig;
        ServerApi.Hooks.NpcSpawn.Register(this, this.OnSpawn);
        ServerApi.Hooks.NpcTransform.Register(this, this.OnTransform);
        ServerApi.Hooks.NpcKilled.Register(this, this.OnNPCKilled);
        ServerApi.Hooks.ServerJoin.Register(this, this.OnJoin);
        TShockAPI.Commands.ChatCommands.Add(new Command("goodnight.admin", Commands.GnCmd, "gn", "宵禁"));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            NewProjectile -= this.NewProj!;
            GeneralHooks.ReloadEvent -= LoadConfig;
            ServerApi.Hooks.NpcSpawn.Deregister(this, this.OnSpawn);
            ServerApi.Hooks.NpcTransform.Deregister(this, this.OnTransform);
            ServerApi.Hooks.NpcKilled.Deregister(this, this.OnNPCKilled);
            ServerApi.Hooks.ServerJoin.Deregister(this, this.OnJoin);
            TShockAPI.Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == Commands.GnCmd);

        }
        base.Dispose(disposing);
    }
    #endregion

    #region 配置文件创建与重读加载方法
    internal static void LoadConfig(ReloadEventArgs args = null!)
    {
        Config = Configuration.Read();
        Config.Write();
        if (args != null && args.Player != null)
        {
            args.Player.SendSuccessMessage(GetString("[宵禁]重新加载配置完毕。"));
        }
    }
    #endregion

    #region 宵禁原功能（白名单方法）
    private void OnJoin(JoinEventArgs args)
    {
        if (!Config.Enabled)
        {
            return;
        }

        var plr = TShock.Players[args.Who];
        if (DateTime.Now.TimeOfDay >= Config.Time.Start && DateTime.Now.TimeOfDay < Config.Time.Stop)
        {
            if (Config.DiscPlayers)
            {
                if (plr != null && !Config.Exempt(plr.Name))
                {
                    plr.Disconnect(GetString($"{Config.JoinMessage} \n禁止游戏时间:{Config.Time.Start}-{Config.Time.Stop}"));
                }
            }
        }
    }

    private void NewProj(object sender, NewProjectileEventArgs e)
    {
        if (!Config.Enabled)
        {
            return;
        }

        if (DateTime.Now.TimeOfDay >= Config.Time.Start && DateTime.Now.TimeOfDay < Config.Time.Stop)
        {
            if (Config.DiscPlayers)
            {
                if (e.Player != null && !Config.Exempt(e.Player.Name) && !e.Player.HasPermission("goodnight.admin"))
                {
                    e.Player.Disconnect(GetString($"{Config.NewProjMessage} \n禁止游戏时间:{Config.Time.Start}-{Config.Time.Stop}"));
                }
            }
        }
    }
    #endregion

    #region 禁止召唤怪物
    private void OnSpawn(NpcSpawnEventArgs args)
    {
        if (args.Handled || !Config.Enabled)
        {
            return;
        }

        var PlayerCount = TShock.Utils.GetActivePlayerCount();
        var NpcList = Config.Npcs.Contains(Main.npc[args.NpcId].netID);
        var NpcDead = Config.NpcDead.Contains(Main.npc[args.NpcId].netID);
        var NoPlr = PlayerCount <= Config.MaxPlayers && Config.MaxPlayers > 0;
        var NpcDeadInfo = string.Join(", ", Config.NpcDead.Select(x => TShock.Utils.GetNPCById(x)?.FullName));
        var isInRegion = Config.PlayersInRegion ? InRegion() : InRegion2();
        var RegionInfo = Config.PlayersInRegion ? GetString($"所有在线人员([c/FF3A4B:{PlayerCount}人])") : GetString("有[c/FF3A4B:1人]");
        var BcstSwitch = Config.BcstSwitch ? this.IsBoss(args) : this.IsNotBoss(args);
        var BcstDefault = Config.BcstSwitch;
        var BcstSwitchOFF = Config.BcstSwitchOff;

        if (DateTime.Now.TimeOfDay >= Config.Time.Start && DateTime.Now.TimeOfDay < Config.Time.Stop)
        {
            if (NoPlr)
            {
                if (Config.Region)
                {
                    if (isInRegion)
                    {
                        if (NpcDead)
                        {
                            args.Handled = false;
                            Main.npc[args.NpcId].active = true;
                            if (!string.IsNullOrEmpty(NpcDeadInfo))
                            {
                                if (BcstSwitchOFF || (BcstDefault && BcstSwitch))
                                {
                                    TShock.Utils.Broadcast(
                                        GetString($"【宵禁】当前[c/338AE1:服务器]存在 [c/FF3A4B:{PlayerCount}]/{Config.MaxPlayers}个玩家! \n") +
                                        GetString($"检测到{RegionInfo}已在【[c/E2FA76:{Config.RegionName}]】\n") +
                                        GetString($"允许召唤以下怪物：\n") +
                                        GetString($"[c/6EABE9:{NpcDeadInfo}]"), Color.Aquamarine);
                                }
                            }
                            else
                            {
                                if (BcstSwitchOFF || (BcstDefault && BcstSwitch))
                                {
                                    TShock.Utils.Broadcast(
                                        GetString($"【宵禁】当前[c/338AE1:服务器]存在 [c/FF3A4B:{PlayerCount}]/{Config.MaxPlayers}]个玩家! \n") +
                                        GetString($"允许召唤表为[c/6EABE9:空]，请在满足[c/FF3A4B:{Config.MaxPlayers}人]\n") +
                                        GetString($"或[c/338AE1:宵禁时段]外: [c/DF95EC:{Config.Time.Start}] — [c/FF9187:{Config.Time.Stop}]\n") +
                                        GetString($"尽可能击败该怪物([c/FF3A4B:{Config.DeadCount}次])来获取允许召唤权"), Color.AntiqueWhite);
                                }
                            }
                        }
                        else if (NpcList)
                        {
                            args.Handled = true;
                            Main.npc[args.NpcId].active = false;
                            if (!string.IsNullOrEmpty(NpcDeadInfo))
                            {
                                if (BcstSwitchOFF || (BcstDefault && BcstSwitch))
                                {
                                    TShock.Utils.Broadcast(
                                        GetString($"【宵禁】当前服务器处于维护时间\n") +
                                        GetString($"{RegionInfo}需要到【[c/E2FA76:{Config.RegionName}]】\n") +
                                        GetString($"可召唤以下怪物：\n") +
                                        GetString($"[c/6EABE9:{NpcDeadInfo}] \n") +
                                        GetString($"宵禁时段: [c/DF95EC:{Config.Time.Start}] — [c/FF9187:{Config.Time.Stop}]"), Color.AntiqueWhite);
                                }
                            }
                            else
                            {
                                if (BcstSwitchOFF || (BcstDefault && BcstSwitch))
                                {
                                    TShock.Utils.Broadcast(
                                        GetString($"【宵禁】当前服务器处于维护时间\n") +
                                        GetString($"允许召唤表为[c/6EABE9:空]，请在满足[c/FF3A4B:{Config.MaxPlayers}人]\n") +
                                        GetString($"或[c/338AE1:宵禁时段]外: [c/DF95EC:{Config.Time.Start}] — [c/FF9187:{Config.Time.Stop}]\n") +
                                        GetString($"尽可能击败该怪物([c/FF3A4B:{Config.DeadCount}次])，来获取允许召唤权。"), Color.AntiqueWhite);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (NpcList)
                        {
                            args.Handled = true;
                            Main.npc[args.NpcId].active = false;
                            if (BcstSwitchOFF || (BcstDefault && BcstSwitch))
                            {
                                TShock.Utils.Broadcast(
                                    GetString($"【宵禁】不在【[c/E2FA76:{Config.RegionName}]】无法生成指定怪物\n") +
                                    GetString($"{RegionInfo}需要到[c/338AE1:{Config.RegionName}]才能召唤怪物\n") +
                                    GetString($"请联系管理员使用[c/6EABE9:/region]创建一个[c/E2FA76:'{Config.RegionName}']\n") +
                                    GetString($"如已创建请输入指令传送:[c/E2FAB4:/region tp {Config.RegionName}]"), Color.AntiqueWhite);
                            }
                        }
                    }
                }
                else if (!Config.Region)
                {
                    if (NpcDead)
                    {
                        args.Handled = false;
                        Main.npc[args.NpcId].active = true;
                        if (!string.IsNullOrEmpty(NpcDeadInfo))
                        {
                            if (BcstSwitchOFF || (BcstDefault && BcstSwitch))
                            {
                                TShock.Utils.Broadcast(
                                    GetString($"【宵禁】当前服务器处于维护时间\n") +
                                    GetString($"当前在线人数不满足:[c/FF3A4B:{PlayerCount}]/{Config.MaxPlayers}人\n") +
                                    GetString($"且处于宵禁时段: [c/DF95EC:{Config.Time.Start}] — [c/FF9187:{Config.Time.Stop}]\n") +
                                    GetString($"仅允许召唤以下怪物：\n") +
                                    GetString($"[c/6EABE9:{NpcDeadInfo}]\n"), Color.Aquamarine);
                            }
                        }
                        else
                        {
                            if (BcstSwitchOFF || (BcstDefault && BcstSwitch))
                            {
                                TShock.Utils.Broadcast(
                                    GetString($"【宵禁】当前服务器处于维护时间\n") +
                                    GetString($"允许召唤表为[c/6EABE9:空]，请在满足[c/FF3A4B:{Config.MaxPlayers}人]\n") +
                                    GetString($"或[c/338AE1:宵禁时段]外: [c/DF95EC:{Config.Time.Start}] — [c/FF9187:{Config.Time.Stop}]\n") +
                                    GetString($"尽可能击败该怪物([c/FF3A4B:{Config.DeadCount}次])，来获取允许召唤权。"), Color.AntiqueWhite);
                            }
                        }
                    }
                    else if (NpcList)
                    {
                        args.Handled = true;
                        Main.npc[args.NpcId].active = false;
                        if (BcstSwitchOFF || (BcstDefault && BcstSwitch))
                        {
                            TShock.Utils.Broadcast(
                                GetString($"【宵禁】当前服务器处于维护时间\n") +
                                GetString($"你所召唤的怪物未被击败:[c/338AE1:{Config.DeadCount}次]\n") +
                                GetString($"请在服务器人数满足:[c/FF3A4B:{Config.MaxPlayers}人]\n") +
                                GetString($"或[c/338AE1:宵禁时段]外: [c/DF95EC:{Config.Time.Start}] — [c/FF9187:{Config.Time.Stop}]\n") +
                                GetString($"尽可能击败该怪物([c/FF3A4B:{Config.DeadCount}次])，来获取允许召唤权。"), Color.AntiqueWhite);
                        }
                    }
                }
            }
            else
            {
                if (NpcList)
                {
                    args.Handled = false;
                }

                Main.npc[args.NpcId].active = true;
            }
        }
    }

    private void OnTransform(NpcTransformationEventArgs args)
    {
        var PlayerCount = TShock.Utils.GetActivePlayerCount();
        var NpcList = Config.Npcs.Contains(Main.npc[args.NpcId].netID);
        var NpcDead = Config.NpcDead.Contains(Main.npc[args.NpcId].netID);
        var NoPlr = PlayerCount <= Config.MaxPlayers && Config.MaxPlayers > 0;
        var isInRegion = Config.PlayersInRegion ? InRegion() : InRegion2();

        if (args.Handled || !Config.Enabled)
        {
            return;
        }
        else if (DateTime.Now.TimeOfDay >= Config.Time.Start && DateTime.Now.TimeOfDay < Config.Time.Stop)
        {
            if (NoPlr)
            {
                if (Config.Region)
                {
                    if (isInRegion)
                    {
                        if (NpcDead)
                        {
                            args.Handled = false;
                            Main.npc[args.NpcId].active = true;
                        }
                        else if (NpcList)
                        {
                            args.Handled = true;
                            Main.npc[args.NpcId].active = false;
                        }
                    }
                    else
                    {
                        if (NpcList)
                        {
                            args.Handled = true;
                            Main.npc[args.NpcId].active = false;
                        }
                    }
                }
                else if (!Config.Region)
                {
                    if (NpcDead)
                    {
                        args.Handled = false;
                        Main.npc[args.NpcId].active = true;
                    }
                    else if (NpcList)
                    {
                        args.Handled = true;
                        Main.npc[args.NpcId].active = false;
                    }
                }
            }
            else
            {
                if (NpcList)
                {
                    args.Handled = false;
                }

                Main.npc[args.NpcId].active = true;
            }
        }
    }
    #endregion

    #region 判断杀怪计数到《允许召唤表》方法
    internal static Dictionary<int, int> KillCounters = new Dictionary<int, int>();
    private void OnNPCKilled(NpcKilledEventArgs args)
    {
        if (!Config.Enabled || args.npc == null)
        {
            return;
        }

        var KillNpc = args.npc.netID;
        var npcName = TShock.Utils.GetNPCById(KillNpc)?.FullName ?? GetString("未知NPC");
        var NpcListInfo = string.Join(", ", Config.NpcDead.Select(x => TShock.Utils.GetNPCById(x)?.FullName + $"({x})"));

        if (Config.Npcs.Contains(KillNpc))
        {
            if (!KillCounters.ContainsKey(KillNpc))
            {
                KillCounters[KillNpc] = 1;
            }
            else
            {
                KillCounters[KillNpc]++;
            }

            if (!Config.NpcDead.Contains(KillNpc))
            {
                TShock.Utils.Broadcast(
                    GetString($"【宵禁】击败NPC【[c/FF9187:{npcName}({KillNpc})]】\n") +
                    GetString($"计入《允许召唤表》要求次数:[c/FF3A4B:{KillCounters[KillNpc]}]/[c/E2FA75:{Config.DeadCount}次]"), Color.AntiqueWhite);
            }

            if (KillCounters[KillNpc] >= Config.DeadCount)
            {
                if (!Config.NpcDead.Contains(KillNpc))
                {
                    Config.NpcDead.Add(KillNpc);
                    Config.Write();
                    TShock.Utils.Broadcast(
                        GetString($"\n因击杀次数达到[c/E2FA75:{Config.DeadCount}次] 将不再播报计数\n") +
                        GetString($"已计入《允许召唤表》：\n[c/6EABE9:{NpcListInfo}]\n") +
                        GetString($"宵禁时段：[c/DF95EC:{Config.Time.Start}] — [c/FF9187:{Config.Time.Stop}]"), Color.AntiqueWhite);
                    KillCounters[KillNpc] = 0;
                }
            }
        }

        else if (KillNpc == Config.ResetNpcDead)
        {
            Config.NpcDead.Clear();
            KillCounters.Clear();
            Config.Write();
            TShock.Utils.Broadcast(GetString($"【宵禁】玩家已击败:[c/FF9187:{npcName}({KillNpc})]，现清空《允许召唤表》"), Color.AntiqueWhite);
        }
    }
    #endregion

    #region 判断玩家在召唤区方法
    //需要所有人在召唤区才能召唤
    private static bool InRegion()
    {
        var PlayerCount = TShock.Utils.GetActivePlayerCount();
        var inRegionCount = 0;
        foreach (var plr in TShock.Players)
        {
            if (plr != null
                && plr.Active
                && plr.CurrentRegion != null
                && plr.CurrentRegion.Name == Config.RegionName)
            {
                inRegionCount++;
            }
        }
        return inRegionCount == PlayerCount;
    }

    //需有一人在召唤区才能召唤
    private static bool InRegion2()
    {
        foreach (var plr in TShock.Players)
        {
            if (plr != null &&
                plr.Active &&
                plr.CurrentRegion != null &&
                plr.CurrentRegion.Name == Config.RegionName)
            {
                return true;
            }
        }
        return false;
    }
    #endregion

    #region 播报类型：判断boss与取反第1种方法判断非BOSS（用于解决自然刷新怪ID在禁怪表时广播刷屏问题）
    private bool IsBoss(NpcSpawnEventArgs args)
    {
        return Main.npc[args.NpcId].boss;
    }

    private bool IsNotBoss(NpcSpawnEventArgs args)
    {
        return !this.IsBoss(args);
    }
    #endregion
}