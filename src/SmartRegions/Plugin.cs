using Microsoft.Xna.Framework;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace SmartRegions;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    private DBConnection DBConnection = null!;
    List<SmartRegion> regions = null!;
    readonly PlayerData[] players = new PlayerData[255];
    struct PlayerData
    {
        public Dictionary<SmartRegion, DateTime> cooldowns;
        public SmartRegion? regionToReplace;
        public void Reset()
        {
            this.cooldowns = new Dictionary<SmartRegion, DateTime>();
            this.regionToReplace = null;
        }
    }

    public Plugin(Main game) : base(game) { }

    public override Version Version => new Version(1, 4, 7);

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override string Author => "GameRoom，肝帝熙恩汉化修复";

    public override string Description => GetString("当玩家进入区域时运行命令。");

    public override void Initialize()
    {
        Commands.ChatCommands.Add(new Command("SmartRegions.manage", this.regionCommand, "smartregion"));
        Commands.ChatCommands.Add(new Command("SmartRegions.manage", this.replaceRegion, "replace"));

        ServerApi.Hooks.NetGreetPlayer.Register(this, this.OnGreetPlayer);
        ServerApi.Hooks.GameUpdate.Register(this, this.OnUpdate);

        var folder = Path.Combine(TShock.SavePath, "SmartRegions");
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
        else
        {
            this.ReplaceLegacyRegionStorage();
        }

        this.DBConnection = new DBConnection();
        this.DBConnection.Initialize();
        this.regions = this.DBConnection.GetRegions();
    }

    protected override void Dispose(bool Disposing)
    {
        if (Disposing)
        {
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == this.regionCommand || x.CommandDelegate == this.replaceRegion);
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, this.OnGreetPlayer);
            ServerApi.Hooks.GameUpdate.Deregister(this, this.OnUpdate);
            this.DBConnection?.Close();
        }
        base.Dispose(Disposing);
    }

    private void OnGreetPlayer(GreetPlayerEventArgs args)
    {
        this.players[args.Who].Reset();
    }

    public static event EventHandler<PlayerInRegionEventArgs>? PlayerInRegion;

    public class PlayerInRegionEventArgs : EventArgs
    {
        public required TSPlayer Player { get; set; }
        public required SmartRegion Region { get; set; }
        public bool IgnoreRegion { get; set; }
    }

    void OnUpdate(EventArgs args)
    {
        foreach (var player in TShock.Players)
        {
            if (player != null && NetMessage.buffer[player.Index].broadcast)
            {
                var inRegion = TShock.Regions.InAreaRegionName((int) (player.X / 16), (int) (player.Y / 16));
                var hs = new HashSet<string>(inRegion);
                var inSmartRegion = this.regions.Where(x => hs.Contains(x.name)).OrderByDescending(x => x.region.Z);

                var regionCounter = 0;
                foreach (var region in inSmartRegion)
                {
                    var eventArgs = new PlayerInRegionEventArgs()
                    {
                        Player = player,
                        Region = region
                    };
                    PlayerInRegion?.Invoke(this, eventArgs);
                    if (eventArgs.IgnoreRegion)
                    {
                        continue;
                    }

                    if ((regionCounter++ == 0 || !region.region.Name.EndsWith("--"))
                        && (!this.players[player.Index].cooldowns.ContainsKey(region)
                            || DateTime.UtcNow > this.players[player.Index].cooldowns[region]))
                    {
                        var file = Path.Combine(TShock.SavePath, "SmartRegions", region.command);
                        if (File.Exists(file))
                        {
                            foreach (var command in File.ReadAllLines(file))
                            {
                                Commands.HandleCommand(TSPlayer.Server, this.replaceWithName(command, player));
                            }
                        }
                        else
                        {
                            Commands.HandleCommand(TSPlayer.Server, this.replaceWithName(region.command, player));
                        }
                        if (this.players[player.Index].cooldowns.ContainsKey(region))
                        {
                            this.players[player.Index].cooldowns[region] = DateTime.UtcNow.AddSeconds(region.cooldown);
                        }
                        else
                        {
                            this.players[player.Index].cooldowns.Add(region, DateTime.UtcNow.AddSeconds(region.cooldown));
                        }
                    }
                }
            }
        }
    }

    string replaceWithName(string cmd, TSPlayer player)
    {
        return cmd.Replace("[PLAYERNAME]", $"\"tsn:{player.Name}\"");
    }

    public async void regionCommand(CommandArgs args)
    {
        try
        {
            await this.regionCommandInner(args);
        }
        catch (Exception e)
        {
            TShock.Log.Error(e.ToString());
            args.Player.SendErrorMessage(GetString("命令执行时出现错误。"));
        }
    }

    public async Task regionCommandInner(CommandArgs args)
    {
        switch (args.Parameters.ElementAtOrDefault(0))
        {
            case "add":
            {
                if (args.Parameters.Count < 4)
                {
                    args.Player.SendErrorMessage(GetString("语法错误！正确的语法是：/smartregion add <区域名称> <冷却时间> <命令或文件>"));
                }
                else
                {
                    if (!double.TryParse(args.Parameters[2], out var cooldown))
                    {
                        args.Player.SendErrorMessage(GetString("无效的语法！正确的语法是：/smartregion add <区域名> <冷却时间> <命令或文件>"));
                        return;
                    }
                    var command = string.Join(" ", args.Parameters.GetRange(3, args.Parameters.Count - 3));
                    if (!TShock.Regions.Regions.Exists(x => x.Name == args.Parameters[1]))
                    {
                        args.Player.SendErrorMessage(GetString("区域 {0} 不存在！"), args.Parameters[1]);
                        var regionNames = from region_ in TShock.Regions.Regions
                                          where region_.WorldID == Main.worldID.ToString()
                                          select region_.Name;
                        PaginationTools.SendPage(args.Player, 1, PaginationTools.BuildLinesFromTerms(regionNames),
                            new PaginationTools.Settings
                            {
                                HeaderFormat = GetString("区域 ({0}/{1}) ："),
                                FooterFormat = GetString("输入 {0}region list {{0}} 查看更多。").SFormat(Commands.Specifier),
                                NothingToDisplayString = GetString("目前没有定义任何区域。")
                            });
                    }
                    else
                    {
                        var cmdName = "";
                        for (var i = 1; i < command.Length && command[i] != ' '; i++)
                        {
                            cmdName += command[i];
                        }
                        var cmd = Commands.ChatCommands.FirstOrDefault(c => c.HasAlias(cmdName));
                        if (cmd != null && !cmd.CanRun(args.Player))
                        {
                            args.Player.SendErrorMessage(GetString("你没有权限使用此命令，因此不能创建智能区域！"));
                            return;
                        }
                        if (cmd != null && !cmd.AllowServer)
                        {
                            args.Player.SendErrorMessage(GetString("你的命令必须允许服务器执行！"));
                            return;
                        }

                        var existingRegion = this.regions.FirstOrDefault(x => x.name == args.Parameters[1]);
                        var newRegion = new SmartRegion
                        {
                            name = args.Parameters[1],
                            cooldown = cooldown,
                            command = command
                        };
                        if (existingRegion != null)
                        {
                            this.players[args.Player.Index].regionToReplace = newRegion;
                            args.Player.SendErrorMessage(GetString("智能区域 {0} 已经存在！请输入 /replace 替换它。"), args.Parameters[1]);
                        }
                        else
                        {
                            this.regions.Add(newRegion);
                            await this.DBConnection.SaveRegion(newRegion);
                            args.Player.SendSuccessMessage(GetString("智能区域已添加！"));
                        }
                    }
                }
            }
            break;
            case "remove":
            {
                if (args.Parameters.Count != 2)
                {
                    args.Player.SendErrorMessage(GetString("无效的语法！正确的语法是：/smartregion remove <区域名>"));
                }
                else
                {
                    var region = this.regions.FirstOrDefault(x => x.name == args.Parameters[1]);
                    if (region == null)
                    {
                        args.Player.SendErrorMessage(GetString("不存在这样的智能区域！"));
                    }
                    else
                    {
                        this.regions.Remove(region);
                        await this.DBConnection.RemoveRegion(region.name);
                        args.Player.SendSuccessMessage(GetString("智能区域 {0} 已被移除！"), args.Parameters[1]);
                    }
                }
            }
            break;
            case "check":
            {
                if (args.Parameters.Count != 2)
                {
                    args.Player.SendErrorMessage(GetString("无效的语法！正确的语法是：/smartregion check <区域名>"));
                }
                else
                {
                    var region = this.regions.FirstOrDefault(x => x.name == args.Parameters[1]);
                    if (region == null)
                    {
                        args.Player.SendInfoMessage(GetString("该区域没有关联的命令。"));
                    }
                    else
                    {
                        var file = Path.Combine(TShock.SavePath, "SmartRegions", region.command);
                        var commands = File.Exists(file)
                            ? GetString($"脚本中的命令如下：\n{File.ReadAllText(file)}")
                            : GetString($"命令如下：\n{region.command}");

                        args.Player.SendInfoMessage(GetString("区域 {0} 的冷却时间为 {1} 秒，使用命令：{2}"), args.Parameters[1], region.cooldown, commands);
                    }
                }
            }
            break;
            case "list":
            {
                var pageNumber = 1;
                var maxDist = int.MaxValue;
                if (args.Parameters.Count > 1)
                {
                    if (!int.TryParse(args.Parameters[1], out pageNumber))
                    {
                        pageNumber = 1;
                    }
                }
                if (args.Parameters.Count > 2)
                {
                    if (args.Player == TSPlayer.Server)
                    {
                        args.Player.SendErrorMessage(GetString("如果您是服务器后台，不能使用距离参数。"));
                        return;
                    }
                    if (!int.TryParse(args.Parameters[2], out maxDist))
                    {
                        maxDist = int.MaxValue;
                    }
                }

                var regionList = this.regions;
                if (maxDist < int.MaxValue)
                {
                    regionList = regionList
                        .Where(r => r.region != null && Vector2.Distance(args.Player.TPlayer.position, r.region.Area.Center() * 16) < maxDist * 16)
                        .ToList();
                }

                var regionNames = regionList.Select(r => r.name).ToList();
                regionNames.Sort();

                if (regionNames.Count == 0)
                {
                    args.Player.SendErrorMessage(maxDist < int.MaxValue
                        ? GetString("没有附近的智能区域。")
                        : GetString("没有智能区域。"));
                }
                else
                {
                    PaginationTools.SendPage(
                        args.Player, pageNumber, PaginationTools.BuildLinesFromTerms(regionNames),
                        new PaginationTools.Settings
                        {
                            HeaderFormat = GetString("智能区域 ({0}/{1}) ："),
                            FooterFormat = GetString($"输入 {Commands.Specifier}smartregion list {{0}} 查看更多。")
                        }
                    );
                }
            }
            break;

            default:
            {
                args.Player.SendInfoMessage(GetString("/smartregion 子命令:\nadd <区域名> <冷却时间> <命令或文件>\nremove <区域名>\ncheck <区域名>\nlist [页码] [最大距离]"));
            }
            break;
        }
    }

    void ReplaceLegacyRegionStorage()
    {
        var path = Path.Combine(TShock.SavePath, "SmartRegions", "config.txt");
        if (File.Exists(path))
        {
            var tasks = new List<Task>();
            try
            {
                var lines = File.ReadAllLines(path);
                for (var i = 0; i < lines.Length; i += 3)
                {
                    var task = this.DBConnection.SaveRegion(new SmartRegion
                    {
                        name = lines[i],
                        command = lines[i + 1],
                        cooldown = double.Parse(lines[i + 2])
                    });
                    tasks.Add(task);
                }
                Task.WaitAll(tasks.ToArray());
                File.Delete(path);
            }
            catch (Exception e)
            {
                TShock.Log.Error(e.ToString());
            }
        }
    }

    public async void replaceRegion(CommandArgs args)
    {
        try
        {
            var player = this.players[args.Player.Index];
            if (player.regionToReplace == null)
            {
                args.Player.SendErrorMessage(GetString("你现在不能做这个操作！"));
            }
            else
            {
                this.regions.RemoveAll(x => x.name == player.regionToReplace.name);
                this.regions.Add(player.regionToReplace);
                await this.DBConnection.SaveRegion(player.regionToReplace);
                player.regionToReplace = null;
                args.Player.SendSuccessMessage(GetString("区域替换成功！"));
            }
        }
        catch (Exception e)
        {
            TShock.Log.Error(e.ToString());
            args.Player.SendErrorMessage(GetString("命令执行时出现错误。"));
        }
    }
}