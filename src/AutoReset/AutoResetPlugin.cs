using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Terraria;
using Terraria.GameContent.UI.States;
using Terraria.IO;
using Terraria.Utilities;
using Terraria.WorldBuilding;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;

namespace AutoReset.MainPlugin;

[ApiVersion(2, 1)]
public class AutoResetPlugin : TerrariaPlugin
{
    public static ResetConfig Config;

    private static string AllPath = Path.Combine(TShock.SavePath, "AutoReset");

    private readonly string _configPath = Path.Combine(AllPath, "reset_config.json");
    private readonly string _filePath = Path.Combine(AllPath, "backup_files");

    private Status _status;

    public GenerationProgress? GenerationProgress;

    public AutoResetPlugin(Main game) : base(game)
    {
    }

    public override string Name => "AutoReset";

    public override Version Version => new(2024, 8, 25);

    public override string Author => "cc04 & Leader & 棱镜 & Cai & 肝帝熙恩";

    public override string Description => "完全自动重置插件";

    public override void Initialize()
    {
        LoadConfig();
        Commands.ChatCommands.Add(new Command("reset.admin", ResetCmd, "reset", "重置世界"));
        Commands.ChatCommands.Add(new Command("", OnWho, "who", "playing", "online"));

        Commands.ChatCommands.Add(new Command("reset.admin", ResetSetting, "rs", "重置设置"));
        ServerApi.Hooks.ServerJoin.Register(this, OnServerJoin, int.MaxValue);
        ServerApi.Hooks.WorldSave.Register(this, OnWorldSave, int.MaxValue);
        ServerApi.Hooks.NpcKilled.Register(this, CountKill);
        GeneralHooks.ReloadEvent += ReloadConfig;
        ;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Commands.ChatCommands.RemoveAll(c => c.CommandDelegate == ResetCmd || c.CommandDelegate == OnWho || c.CommandDelegate == ResetSetting);
            ServerApi.Hooks.NpcKilled.Deregister(this, CountKill);
            ServerApi.Hooks.ServerJoin.Deregister(this, OnServerJoin);
            ServerApi.Hooks.WorldSave.Deregister(this, OnWorldSave);
            GeneralHooks.ReloadEvent -= ReloadConfig;
        }

        base.Dispose(disposing);
    }

    private void ReloadConfig(ReloadEventArgs e)
    {
        if (File.Exists(_configPath))
        {
            Config = JsonConvert.DeserializeObject<ResetConfig>(File.ReadAllText(_configPath))!;
        }
        else
        {
            Config = new ResetConfig
            {
                KillToReset = new ResetConfig.AutoReset(),
                SetWorld = new ResetConfig.SetWorldConfig(),
                PreResetCommands = Array.Empty<string>(),
                PostResetCommands = Array.Empty<string>(),
                SqLs = new[]
                {
                        "DELETE FROM tsCharacter"
                    },
                Files = new Dictionary<string, string>()
            };
            File.WriteAllText(_configPath, Config.ToJson());
        }

        e.Player.SendSuccessMessage("[AutoReset]自动重置插件配置已重载");
    }

    private void LoadConfig()
    {
        string _AllPath = Path.Combine(TShock.SavePath, "AutoReset");
        if (!Directory.Exists(_AllPath))
            Directory.CreateDirectory(_AllPath);
        if (!Directory.Exists(_filePath))
            Directory.CreateDirectory(_filePath);
        if (!File.Exists(_configPath))
        {
            Config = new ResetConfig
            {
                KillToReset = new ResetConfig.AutoReset(),
                SetWorld = new ResetConfig.SetWorldConfig(),
                PreResetCommands = new string[] { "/结算金币" },
                PostResetCommands = new string[] { "/reload", "/初始化进度补给箱", "/rpg reset" },
                SqLs = new[]
                {
                    "DELETE FROM tsCharacter"
                },
                Files = new Dictionary<string, string>()
                {
                    {"/tshock/原神.json","原神.json"},
                    {"/tshock/XSB数据缓存.json",""}
                }
            };
            File.WriteAllText(_configPath, Config.ToJson());
        }
        else
        {
            Config = JsonConvert.DeserializeObject<ResetConfig>(File.ReadAllText(_configPath))!;
        }
    }

    private void OnWho(CommandArgs args)
    {
        if (Config.KillToReset.KillCount != 0 && Config.KillToReset.KillCount != Config.KillToReset.NeedKillCount)
        {
            if (args.Player.RealPlayer)
                args.Player.SendInfoMessage(
                    $"[i:3611]击杀自动重置:{Lang.GetNPCName(Config.KillToReset.NpcId)}({Config.KillToReset.KillCount}/{Config.KillToReset.NeedKillCount})");
            else
                args.Player.SendInfoMessage(
                    $"📝击杀自动重置:{Lang.GetNPCName(Config.KillToReset.NpcId)}({Config.KillToReset.KillCount}/{Config.KillToReset.NeedKillCount})");
        }

        Status status = _status;
        switch (status)
        {
            case Status.Cleaning:
                args.Player.SendInfoMessage("重置数据中, 请稍后...");
                break;
            case Status.Generating:
                args.Player.SendInfoMessage("生成地图中: " + GetProgress());
                break;
            case Status.Available:
                break;
        }
    }

    private void CountKill(NpcKilledEventArgs args)
    {

        if (Config.KillToReset.Enable && args.npc.netID == Config.KillToReset.NpcId)
        {
            Config.KillToReset.KillCount++;
            File.WriteAllText(_configPath, Config.ToJson());
            TShock.Utils.Broadcast(
                string.Format(
                    $"[自动重置]服务器中已经击杀{Lang.GetNPCName(Config.KillToReset.NpcId)}{Config.KillToReset.KillCount}/{Config.KillToReset.NeedKillCount}"),
                Color.Orange);
            if (Config.KillToReset.NeedKillCount <= Config.KillToReset.KillCount)
                ResetCmd(null);
        }
    }

    private void ResetCmd(CommandArgs e)
    {
        if (_status != Status.Available) return;
        Task.Run(delegate
        {
            _status = Status.Cleaning;
            TShock.Utils.Broadcast("[自动重置]服务器即将开始重置...", Color.Orange);
            for (int i = 60; i >= 0; i--)
            {
                TShock.Utils.Broadcast(string.Format("[自动重置]{0}s后关闭服务器...", i), Color.Orange);
                Thread.Sleep(1000);
            }

            TShock.Players.ForEach(delegate (TSPlayer? p)
            {
                if (p != null) p.Kick("[自动重置]服务器已开始重置...", true, true);
            });


            Config.PreResetCommands.ForEach(delegate (string c) { Commands.HandleCommand(TSPlayer.Server, c); });
            Main.WorldFileMetadata = null;
            Main.gameMenu = true;
            string seed;
            if (!string.IsNullOrEmpty(Config.SetWorld.Seed))
                seed = Config.SetWorld.Seed;
            else
                seed = "";
            seed = seed.Trim();
            if (string.IsNullOrEmpty(seed))
                Main.ActiveWorldFileData.SetSeedToRandom();
            else
                Main.ActiveWorldFileData.SetSeed(seed);
            UIWorldCreation.ProcessSpecialWorldSeeds(seed);
            WorldGen.generatingWorld = true;
            Main.rand = new UnifiedRandom(Main.ActiveWorldFileData.Seed);
            Main.menuMode = 10;
            GenerationProgress = new GenerationProgress();
            Task task = WorldGen.CreateNewWorld(GenerationProgress);
            _status = Status.Generating;
            while (!task.IsCompleted)
            {
                TShock.Log.ConsoleInfo(GetProgress());
                Thread.Sleep(1000);
            }

            _status = Status.Cleaning;
            Main.rand = new UnifiedRandom((int)DateTime.Now.Ticks);
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
            try
            {
                if (Config.SetWorld.Name != null) Main.worldName = Config.SetWorld.Name;
                PostReset();
                Config.KillToReset.KillCount = 0;
                Config.SetWorld = new ResetConfig.SetWorldConfig();
                File.WriteAllText(_configPath, Config.ToJson());
            }
            finally
            {
                Utils.CallApi();
                GenerationProgress = null;
                _status = Status.Available;
            }
        });
    }


    private void ResetSetting(CommandArgs args)
    {
        TSPlayer op = args.Player;

        #region help

        void ShowHelpText()
        {
            if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, op, out int pageNumber))
                return;

            List<string> lines = new()
            {
                "/rs info",
                "/rs name <地图名>",
                "/rs seed <种子>",
                "或",
                "/rs 信息",
                "/rs 名字 <地图名>",
                "/rs 种子 <种子>"
            };

            PaginationTools.SendPage(
                op, pageNumber, lines,
                new PaginationTools.Settings
                {
                    HeaderFormat = "帮助 ({0}/{1})：",
                    FooterFormat = "输入 {0}rs help {{0}} 查看更多".SFormat(Commands.Specifier)
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
                op.SendInfoMessage($"地图名: {(Config.SetWorld.Name == null ? Main.worldName : Config.SetWorld.Name)}\n" +
                                   $"种子: {(Config.SetWorld.Seed == null ? "随机" : Config.SetWorld.Seed)}");
                break;
            case "名字":
            case "name":
                if (args.Parameters.Count < 2)
                {
                    Config.SetWorld.Name = null;
                    File.WriteAllText(_configPath, Config.ToJson());
                    op.SendSuccessMessage("世界名字已设置为跟随原世界");
                }
                else
                {
                    Config.SetWorld.Name = args.Parameters[1];
                    File.WriteAllText(_configPath, Config.ToJson());
                    op.SendSuccessMessage("世界名字已设置为 " + args.Parameters[1]);
                }

                break;
            case "种子":
            case "seed":
                if (args.Parameters.Count < 2)
                {
                    Config.SetWorld.Seed = null;
                    File.WriteAllText(_configPath, Config.ToJson());
                    op.SendSuccessMessage("世界种子已设为随机");
                }
                else
                {
                    bool flag = true;
                    List<string> seedParts = new();
                    foreach (string? i in args.Parameters)
                    {
                        if (flag)
                        {
                            flag = false;
                            continue;
                        }

                        seedParts.Add(i);
                    }

                    Config.SetWorld.Seed = string.Join(" ", seedParts);
                    File.WriteAllText(_configPath, Config.ToJson());
                    op.SendSuccessMessage("世界种子已设置为:" + Config.SetWorld.Seed);
                }

                break;
        }
    }

    private void PostReset()
    {
        Config.SqLs.ForEach(delegate (string c)
        {
            try
            {
                TShock.DB.Query(c, Array.Empty<object>());
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleWarn($"[AutoReset]重置SQL({c})执行失败: {ex.Message}");
            }
        });
        foreach (KeyValuePair<string, string> keyValuePair in Config.Files!)
            try
            {
                if (!string.IsNullOrEmpty(keyValuePair.Value))
                    File.Copy(Path.Combine(_filePath, keyValuePair.Value),
                        Path.Combine(Environment.CurrentDirectory, keyValuePair.Key), true);
                else
                    File.Delete(keyValuePair.Key);
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleWarn($"[AutoReset]重置文件({keyValuePair.Key})替换失败: {ex.Message}");
            }

        Config.PostResetCommands.ForEach(delegate (string c) { Commands.HandleCommand(TSPlayer.Server, c); });
    }

    private string GetProgress()
    {
        return string.Format("{0:0.0%} - " + GenerationProgress!.Message + " - {1:0.0%}",
            GenerationProgress.TotalProgress, GenerationProgress.Value);
    }

    private void OnServerJoin(JoinEventArgs args)
    {
        TSPlayer? plr = TShock.Players[args.Who];

        Status status = _status;
        switch (status)
        {
            case Status.Cleaning:
                plr.Disconnect("[AutoReset]重置数据中，请稍后...");
                args.Handled = true;
                break;
            case Status.Generating:
                plr.Disconnect("[AutoReset]生成地图中:\n" + GetProgress());
                args.Handled = true;
                break;
            case Status.Available:
                break;
        }
    }

    private void OnWorldSave(WorldSaveEventArgs args)
    {
        args.Handled = _status != Status.Available && Main.WorldFileMetadata == null;
    }
}

#endregion
