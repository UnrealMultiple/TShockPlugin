using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace BossLock;

[ApiVersion(2, 1)]
// ReSharper disable once UnusedType.Global
public class BossLock(Main game) : TerrariaPlugin(game)
{
    public override string Author => "Cai";

    public override string Description => "进度锁";

    public override string Name => "BossLock";

    public override Version Version => new (2, 0, 0, 0);
    

    public override void Initialize()
    {
        Database.Init();
        Config.Settings.Read();
        Config.Settings.Write();
        ServerApi.Hooks.NpcSpawn.Register(this, NpcSpawn);
        PlayerHooks.PlayerPostLogin += this.PlayerHooksOnPlayerPostLogin;
        Commands.ChatCommands.Add(new Command("bosslock", Setting, "bosslock", "bl", "tb"));
        Commands.ChatCommands.Add(new Command(ProcessList, "locklist", "进度列表"));
        GeneralHooks.ReloadEvent += this.Reload;
    }
    
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            var asm = Assembly.GetExecutingAssembly();
            Commands.ChatCommands.RemoveAll(c => c.CommandDelegate.Method.DeclaringType?.Assembly == asm);
            ServerApi.Hooks.NpcSpawn.Deregister(this, NpcSpawn);
            PlayerHooks.PlayerPostLogin -= this.PlayerHooksOnPlayerPostLogin;
            GeneralHooks.ReloadEvent -= this.Reload;
        }

        base.Dispose(disposing);
    }
    

    private static void ProcessList(CommandArgs args)
    {
        Database.SendAllBossSimple(args);
    }

    private void PlayerHooksOnPlayerPostLogin(PlayerPostLoginEventArgs e)
    {
        var plr = e.Player;
        _ = ProcessPlayerLoginAsync(plr);
    }

    private static async Task ProcessPlayerLoginAsync(TSPlayer plr)
    {
        await Task.Delay(7000);
        
        if (!Utils.IsPlayerValid(plr))
        {
            return;
        }
        
        var progressStatus = Utils.GetCurrentProgressStatus();
        
        Database.SendLastestBoss(plr);
        
        await Task.Delay(2000);
        
        if (!Utils.IsPlayerValid(plr))
        {
            return;
        }
        
        plr.SendWarningMessage(GetString($"[i:1370]当前进度: [c/AF4BFF:{progressStatus}]"));
    }
    

    private void Reload(ReloadEventArgs e)
    {
        Config.Settings.Read();
        Config.Settings.Write();
        e.Player.SendSuccessMessage(GetString("[BossLock]定时进度已重载!"));
    }
    
    private static void Setting(CommandArgs args)
    {
        if (args.Parameters.Count == 0)
        {
            args.Player.SendInfoMessage(GetString("[i:43]有效子命令列表:\n") +
                                        GetString("/bl list 列出所有定时进度配置\n") +
                                        GetString("/bl reset 重置进度配置\n") +
                                        GetString("/bl edit 修改进度配置\n") +
                                        GetString("/bl unlock 解锁进度"));
            return;
        }
        switch (args.Parameters[0])
        {
            case "重置":
            case "初始化":
            case "reset":
                var time = DateTime.Now;
                if (args.Parameters.Count == 2)
                {
                    try
                    {
                        time = DateTime.ParseExact(args.Parameters[1], "yyyy-MM-dd-HH:mm:ss", null);
                    }
                    catch
                    {
                        args.Player.SendErrorMessage(GetString($"日期格式错误!正确格式: yyyy-MM-dd-HH:mm:ss\n") +
                                                     GetString("例如: 2007-05-24-23:59:59"));
                        return;
                    }
                }
                Database.ClearDb();
                Config.Settings.Write();
                var currentBoss = new List<int>();
                foreach (var i in Config.Settings.Locks)
                {
                    if (currentBoss.Contains(i.NpcId))
                    {
                        args.Player.SendWarningMessage(GetString($"[BossLock]检测到重复配置: {Lang.GetNPCNameValue(i.NpcId)} ({i.NpcId})"));
                        continue;
                    }

                    DateTime startTime;
                    if (i.LockSeconds == -1)
                    {
                        startTime = DateTime.MaxValue;
                    }
                    else
                    {
                        startTime = time + TimeSpan.FromSeconds(i.LockSeconds);
                    }
                    
                    Database.AddBoss(i.NpcId, startTime.ToString("yyyy-MM-dd-HH:mm:ss"));
                    currentBoss.Add(i.NpcId);
                }
                args.Player.SendSuccessMessage(GetString($"[BossLock]初始化完成!开服时间设为:{Utils.TimeFormat(time.ToString("yyyy-MM-dd-HH:mm:ss"))}"));
                return;
            case "list":
            case "列表":
                Database.SendAllBoss(args);
                return;

            case "修改":
            case "edit":
                if (args.Parameters.Count < 3)
                {
                    args.Player.SendErrorMessage(GetString("格式错误!正确格式:/bl edit <NPC ID> <预定日期>"));
                    return;
                }
                try
                {
                    
                    if (Database.LoadBoss(int.Parse(args.Parameters[1])) == null)
                    {
                        args.Player.SendErrorMessage(GetString($"[i:50]没有关于{Lang.GetNPCNameValue(int.Parse(args.Parameters[1]))}({int.Parse(args.Parameters[1])})的限制配置!"));
                        return;
                    }
                }
                catch (FormatException)
                {
                    args.Player.SendErrorMessage(GetString("[i:50]请输入正确ID!"));
                    return;
                }
                DateTime targetData;
                try
                {
                    targetData = DateTime.ParseExact(args.Parameters[2], "yyyy-MM-dd-HH:mm:ss", null);
                }
                catch (FormatException)
                {
                    args.Player.SendErrorMessage(GetString($"[i:50]日期格式错误!正确格式: yyyy-MM-dd-HH:mm:ss\n") +
                                                 GetString("例如: 2007-05-24-23:59:59"));
                    return;
                }
                Database.UpdateTime(int.Parse(args.Parameters[1]), targetData.ToString("yyyy-MM-dd-HH:mm:ss"));
                args.Player.SendSuccessMessage(GetString($"{Lang.GetNPCNameValue(int.Parse(args.Parameters[1]))}({int.Parse(args.Parameters[1])})的开打时间已设为{targetData:yyyy-MM-dd HH:mm:ss}"));
                return;
            case "unlock":
            case "解锁":
                if (args.Parameters.Count < 2)
                {
                    args.Player.SendErrorMessage(GetString("[i:50]格式错误!正确格式:/bl unlock <NPC ID>"));
                    return;
                }
                try
                {
                    var bossData = Database.LoadBoss(int.Parse(args.Parameters[1]));
                    if (bossData == null)
                    {
                        args.Player.SendErrorMessage($"[i:50]没有关于{Lang.GetNPCNameValue(int.Parse(args.Parameters[1]))}({int.Parse(args.Parameters[1])})的限制配置!");
                        return;
                    }

                    if (DateTime.ParseExact(bossData, "yyyy-MM-dd-HH:mm:ss", null) < DateTime.Now)
                    {
                        args.Player.SendErrorMessage($"{Lang.GetNPCNameValue(int.Parse(args.Parameters[1]))}({int.Parse(args.Parameters[1])})已解锁!");
                    }
                }
                catch
                {
                    args.Player.SendErrorMessage(GetString("[i:50]请输入正确ID!"));
                    return;
                }
                Database.UpdateTime(int.Parse(args.Parameters[1]), DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss"));
                args.Player.SendSuccessMessage(GetString($"{Lang.GetNPCNameValue(int.Parse(args.Parameters[1]))}({int.Parse(args.Parameters[1])})已解锁!"));
                return;
        }
    }

    
    private static void NpcSpawn(NpcSpawnEventArgs args)
    {
        foreach (var bossTime in Config.Settings.Locks)
        {
            if (Main.npc[args.NpcId].netID != bossTime.NpcId)
            {
                continue;
            }

            var bossData = Database.LoadBoss(Main.npc[args.NpcId].netID);
            if (bossData == null) { return; }
            var targetTime = DateTime.ParseExact(bossData, "yyyy-MM-dd-HH:mm:ss", null);
            var timeSpan = targetTime - DateTime.Now;
            if (timeSpan.Seconds <= 0)
            {
                continue;
            }

            if (!Main.npc[args.NpcId].active)
            {
                continue;
            }
            
            Main.npc[args.NpcId].active = false;
            Main.npc[args.NpcId].type = 0;
            TSPlayer.All.SendData(PacketTypes.NpcUpdate, null, args.NpcId);
            TSPlayer.All.SendErrorMessage(GetString($"[i:43][c/AF4BFF:{Main.npc[args.NpcId].FullName}]还没到解锁时间, 被禁止生成!"));
            TSPlayer.All.SendWarningMessage(GetString($"解锁时间: [c/32FF82:{Utils.TimeFormat(bossData)}] [c/FFFFFF:({Math.Round(timeSpan.TotalSeconds, 1)}秒)]"));
            
        }
    }
}
