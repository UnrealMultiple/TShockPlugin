using LazyAPI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.GameContent.UI.States;
using Terraria.IO;
using Terraria.Utilities;
using Terraria.WorldBuilding;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;

namespace AutoReset;

[ApiVersion(2, 1)]
// ReSharper disable once ClassNeverInstantiated.Global
public class AutoResetPlugin(Main game) : LazyPlugin(game)
{
    public static readonly string FolderName = "AutoReset";

    private readonly string _replaceFilePath = Path.Combine(TShock.SavePath, FolderName, "ReplaceFiles");

    private Status _status;

    private GenerationProgress? _generationProgress;

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(2025, 06, 23, 0);

    public override string Author => "cc04 & Leader & 棱镜 & Cai & 肝帝熙恩";

    public override string Description => GetString("完全自动重置插件");

    public override void Initialize()
    {
        this.addCommands.Add(new Command("reset.admin", this.ResetCmd, "reset", "重置世界"));
        this.addCommands.Add(new Command("reset.admin", this.ResetDataCmd, "resetdata", "重置数据"));
        this.addCommands.Add(new Command(this.OnWho, "who", "playing", "online"));
        this.addCommands.Add(new Command("reset.admin", this.ResetSetting, "rs", "重置设置"));
        ServerApi.Hooks.ServerJoin.Register(this, this.OnServerJoin, int.MaxValue);
        ServerApi.Hooks.WorldSave.Register(this, this.OnWorldSave, int.MaxValue);
        ServerApi.Hooks.NpcKilled.Register(this, this.CountKill);
        Terraria.Utils.TryCreatingDirectory(this._replaceFilePath);
        base.Initialize();
    }

    private void ResetDataCmd(CommandArgs args)
    {
        this.PostReset();
        TSPlayer.All.SendSuccessMessage(GetString("[AutoReset]服务器数据重置成功~"));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.NpcKilled.Deregister(this, this.CountKill);
            ServerApi.Hooks.ServerJoin.Deregister(this, this.OnServerJoin);
            ServerApi.Hooks.WorldSave.Deregister(this, this.OnWorldSave);
        }

        base.Dispose(disposing);
    }

    private void OnWho(CommandArgs args)
    {
        if (ResetConfig.Instance.KillToReset.KillCount != 0 && ResetConfig.Instance.KillToReset.KillCount != ResetConfig.Instance.KillToReset.NeedKillCount)
        {
            args.Player.SendInfoMessage(
                args.Player.RealPlayer ? GetString($"[i:3611]击杀自动重置:[c/DC143C:{Lang.GetNPCName(ResetConfig.Instance.KillToReset.NpcId)}] ([c/98FB98:{ResetConfig.Instance.KillToReset.KillCount}]/{ResetConfig.Instance.KillToReset.NeedKillCount})") : GetString($"📝击杀自动重置:{Lang.GetNPCName(ResetConfig.Instance.KillToReset.NpcId)}({ResetConfig.Instance.KillToReset.KillCount}/{ResetConfig.Instance.KillToReset.NeedKillCount})"));
        }

        var status = this._status;
        switch (status)
        {
            case Status.Cleaning:
                args.Player.SendInfoMessage(GetString("重置数据中, 请稍后..."));
                break;
            case Status.Generating:
                args.Player.SendInfoMessage(GetString("生成地图中: ") + this.GetProgress());
                break;
            case Status.Available:
                break;
        }
    }

    private void CountKill(NpcKilledEventArgs args)
    {

        if (ResetConfig.Instance.KillToReset.Enable && args.npc.netID == ResetConfig.Instance.KillToReset.NpcId)
        {
            ResetConfig.Instance.KillToReset.KillCount++;
            ResetConfig.Instance.SaveTo();
            TShock.Utils.Broadcast(GetString($"[重置计数器]已经击杀[c/DC143C:{Lang.GetNPCName(ResetConfig.Instance.KillToReset.NpcId)}] ([c/98FB98:{ResetConfig.Instance.KillToReset.KillCount}]/{ResetConfig.Instance.KillToReset.NeedKillCount})"), Color.Gold);
            if (ResetConfig.Instance.KillToReset.NeedKillCount <= ResetConfig.Instance.KillToReset.KillCount)
            {
                this.ResetCmd(null);
            }
        }
    }

    private void ResetCmd(CommandArgs? e)
    {
        if (this._status != Status.Available)
        {
            return;
        }

        Task.Run(delegate
        {
            var worldName = Main.worldName;
            TShock.Utils.Broadcast(GetString("[AutoReset]服务器即将[c/DC143C:开始重置]..."), Color.Orange);
            for (var i = 60; i >= 0; i--)
            {
                TShock.Utils.Broadcast(string.Format(GetString("[AutoReset][c/98FB98:{0}s]后[c/DC143C:关闭服务器]..."), i), Color.Orange);
                Thread.Sleep(1000);
            }
            this._status = Status.Cleaning;
            TShock.Players.ForEach(delegate (TSPlayer? p)
            {
                p?.Kick(GetString("[AutoReset]服务器已开始重置..."), true, true);
            });


            ResetConfig.Instance.PreResetCommands.ForEach(delegate (string c) { Commands.HandleCommand(TSPlayer.Server, c); });
            Main.WorldFileMetadata = null;
            Main.gameMenu = true;
            var seed = !string.IsNullOrEmpty(ResetConfig.Instance.SetWorld.Seed) ? ResetConfig.Instance.SetWorld.Seed : "";
            seed = seed.Trim();
            if (string.IsNullOrEmpty(seed))
            {
                Main.ActiveWorldFileData.SetSeedToRandom();
            }
            else
            {
                Main.ActiveWorldFileData.SetSeed(seed);
            }

            UIWorldCreation.ProcessSpecialWorldSeeds(seed);
            WorldGen.generatingWorld = true;
            Main.rand = new UnifiedRandom(Main.ActiveWorldFileData.Seed);
            Main.menuMode = 10;
            this._generationProgress = new GenerationProgress();
            var task = WorldGen.CreateNewWorld(this._generationProgress);
            this._status = Status.Generating;
            while (!task.IsCompleted)
            {
                TSPlayer.Server.SendWarningMessage(this.GetProgress());
                Main.worldName = worldName + this.GetShortProgress();
                Thread.Sleep(1000);
            }

            this._status = Status.Cleaning;
            Main.rand = new UnifiedRandom((int) DateTime.Now.Ticks);
            // Main.wo = Main.ActiveWorldFileData.UniqueId;
            //
            // MessageBuffer
            
            WorldFile.LoadWorld(false);
            Main.dayTime = WorldFile._tempDayTime;
            Main.time = WorldFile._tempTime;
            Main.raining = WorldFile._tempRaining;
            Main.rainTime = WorldFile._tempRainTime;
            Main.maxRaining = WorldFile._tempMaxRain;
            Main.cloudAlpha = WorldFile._tempMaxRain;
            Main.moonPhase = WorldFile._tempMoonPhase;
            Main.bloodMoon = WorldFile._tempBloodMoon;
            Main.eclipse = WorldFile._tempEclipse;
            Main.gameMenu = false;
            Main.worldName = worldName;
            Main.invasionSize = 0;
            LanternNight.WorldClear();
            DD2Event.StopInvasion();
            try
            {
                if (ResetConfig.Instance.SetWorld.Name != null)
                {
                    Main.worldName = ResetConfig.Instance.SetWorld.Name;
                }

                this.PostReset();
                ResetConfig.Instance.KillToReset.KillCount = 0;
                ResetConfig.Instance.SetWorld = new ResetConfig.SetWorldConfig();
                ResetConfig.Instance.SaveTo();
            }
            finally
            {
                Console.WriteLine("WorldID: "+ Main.worldID);
                this._generationProgress = null;
                this._status = Status.Available;
            }
        });
    }


    private void ResetSetting(CommandArgs args)
    {
        var op = args.Player;

        #region help

        void ShowHelpText()
        {
            if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, op, out var pageNumber))
            {
                return;
            }

            List<string> lines = new()
            {
                "/rs info",
                GetString("/rs name <地图名>"),
                GetString("/rs seed <种子>"),
                GetString("/reset 重置世界"),
                GetString("/resetdata 重置数据")
            };

            PaginationTools.SendPage(
                op, pageNumber, lines,
                new PaginationTools.Settings
                {
                    HeaderFormat = GetString("帮助 ({0}/{1})："),
                    FooterFormat = GetString("输入 {0}rs help {{0}} 查看更多").SFormat(Commands.Specifier)
                }
            );
        }

        if (args.Parameters.Count == 0)
        {
            ShowHelpText();
            return;
        }


        switch (args.Parameters[0].ToLowerInvariant())
        {
            // 帮助
            case "help":
                ShowHelpText();
                return;

            default:
                ShowHelpText();
                break;

            // 世界信息
            case "信息":
            case "info":
                op.SendInfoMessage(GetString($"地图名: {ResetConfig.Instance.SetWorld.Name ?? Main.worldName}\n") +
                                   GetString($"种子: {ResetConfig.Instance.SetWorld.Seed ?? GetString("随机")}\n") +
                                   GetString($"击杀重置: {ResetConfig.Instance.KillToReset.Enable}\n") +
                                   GetString($"击杀Npc: {Lang.GetNPCName(ResetConfig.Instance.KillToReset.NpcId)}({ResetConfig.Instance.KillToReset.NpcId})\n") +
                                   GetString($"目标击杀数: {ResetConfig.Instance.KillToReset.NeedKillCount}\n") +
                                   GetString($"已击杀数: {ResetConfig.Instance.KillToReset.KillCount}"));
                break;
            case "名字":
            case "name":
                if (args.Parameters.Count < 2)
                {
                    ResetConfig.Instance.SetWorld.Name = null;
                    ResetConfig.Instance.SaveTo();
                    op.SendSuccessMessage(GetString("世界名字已设置为跟随原世界"));
                }
                else
                {
                    ResetConfig.Instance.SetWorld.Name = args.Parameters[1];
                    ResetConfig.Instance.SaveTo();
                    op.SendSuccessMessage(GetString("世界名字已设置为 ") + args.Parameters[1]);
                }

                break;
            case "种子":
            case "seed":
                if (args.Parameters.Count < 2)
                {
                    ResetConfig.Instance.SetWorld.Seed = null;
                    ResetConfig.Instance.SaveTo();
                    op.SendSuccessMessage(GetString("世界种子已设为随机"));
                }
                else
                {
                    var flag = true;
                    List<string> seedParts = new();
                    foreach (var i in args.Parameters)
                    {
                        if (flag)
                        {
                            flag = false;
                            continue;
                        }

                        seedParts.Add(i);
                    }

                    ResetConfig.Instance.SetWorld.Seed = string.Join(" ", seedParts);
                    ResetConfig.Instance.SaveTo();
                    op.SendSuccessMessage(GetString("世界种子已设置为:") + ResetConfig.Instance.SetWorld.Seed);
                }

                break;
        }
    }

    private void PostReset()
    {
        ResetConfig.Instance.SqLs.ForEach(delegate (string c)
        {
            try
            {
                TShock.DB.Query(c);
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleWarn(GetString($"[AutoReset]重置SQL({c})执行失败: {ex.Message}"));
            }
        });
        foreach (var keyValuePair in ResetConfig.Instance.Files!)
        {
            try
            {
                if (!string.IsNullOrEmpty(keyValuePair.Value))
                {
                    File.Copy(Path.Combine(this._replaceFilePath, keyValuePair.Value),
                        Path.Combine(Environment.CurrentDirectory, keyValuePair.Key), true);
                }
                else
                {
                    File.Delete(keyValuePair.Key);
                }
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleWarn(GetString($"[AutoReset]重置文件({keyValuePair.Key})替换失败: {ex.Message}"));
            }
        }

        ResetConfig.Instance.PostResetCommands.ForEach(delegate (string c) { Commands.HandleCommand(TSPlayer.Server, c); });
    }

    private string GetProgress()
    {
        return string.Format("{0:0.0%} - " + this._generationProgress!.Message + " - {1:0.0%}",
            this._generationProgress.TotalProgress, this._generationProgress.Value);
    }

    private string GetShortProgress()
    {
        return string.Format(" {0:0.0%}" + "(" + this._generationProgress!.Message + ")",
            this._generationProgress.TotalProgress);
    }

    private void OnServerJoin(JoinEventArgs args)
    {
        var plr = TShock.Players[args.Who];

        var status = this._status;
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (status)
        {
            case Status.Cleaning:
                plr.Disconnect(GetString("[AutoReset]重置数据中，请稍后..."));
                args.Handled = true;
                break;
            case Status.Generating:
                plr.Disconnect(GetString("[AutoReset]生成地图中:\n") + this.GetProgress());
                args.Handled = true;
                break;
            case Status.Available:
                break;
        }
    }

    private void OnWorldSave(WorldSaveEventArgs args)
    {
        args.Handled = this._status != Status.Available && Main.WorldFileMetadata == null;
    }
}

#endregion