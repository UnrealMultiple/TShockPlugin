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

    public override Version Version => new (2, 0, 0, 6);
    

    public override void Initialize()
    {
        Database.Init();
        Config.Settings.Read();
        Config.Settings.Write();
        SpawnGuard.Register(this);
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
            SpawnGuard.Deregister(this);
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
                                        GetString("/bl reset auto 按配置的下一个北京时间重置进度\n") +
                                        GetString("/bl edit 修改进度配置\n") +
                                        GetString("/bl unlock 解锁进度"));
            return;
        }
        switch (args.Parameters[0])
        {
            case "重置":
            case "初始化":
            case "reset":
                try
                {
                    // reset 必须读取刚编辑的磁盘配置，不能把旧内存配置覆盖回去。
                    Config.Settings.Read();
                }
                catch (Exception ex)
                {
                    TShock.Log.Error(ex.ToString());
                    args.Player.SendErrorMessage(GetString("[BossLock]配置读取失败，现有进度保持不变。"));
                    return;
                }
                var time = DateTime.Now;
                if (args.Parameters.Count == 2)
                {
                    try
                    {
                        time = string.Equals(args.Parameters[1], "auto", StringComparison.OrdinalIgnoreCase)
                            ? new DateTimeOffset(GetResetBaseTime(DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).DateTime), TimeSpan.FromHours(8)).LocalDateTime
                            : DateTime.ParseExact(args.Parameters[1], "yyyy-MM-dd-HH:mm:ss", null);
                    }
                    catch
                    {
                        args.Player.SendErrorMessage(GetString("日期或自动重置时刻配置错误! 日期格式: yyyy-MM-dd-HH:mm:ss；BossLock.json 的 AutoResetTimes 必须是非空的 HH:mm 时刻列表。"));
                        return;
                    }
                }
                try
                {
                    var rows = BossGroup.Configured().ToDictionary(i => i.NpcId, i =>
                        (i.LockSeconds == -1 ? DateTime.MaxValue : time.AddSeconds(i.LockSeconds))
                        .ToString("yyyy-MM-dd-HH:mm:ss"));
                    Database.ReplaceAll(rows);
                }
                catch (Exception ex)
                {
                    TShock.Log.Error(ex.ToString());
                    args.Player.SendErrorMessage(GetString("[BossLock]重置失败，已有进度未替换。请检查配置和数据库。"));
                    return;
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
                    
                    if (Database.LoadBoss(int.Parse(args.Parameters[1])) == null &&
                        !Config.Settings.Locks.Any(i => BossGroup.Root(i.NpcId) == BossGroup.Root(int.Parse(args.Parameters[1]))))
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

    
    internal static DateTime GetResetBaseTime(DateTime resetLocalTime)
    {
        return ResetSchedule.Next(resetLocalTime, Config.Settings.AutoResetTimes);
    }

    internal static bool IsLocked(TimeSpan remaining) => remaining > TimeSpan.Zero;

}
