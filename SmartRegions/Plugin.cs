using System;
using System.Collections.Generic;
using System.Linq;
using TShockAPI;
using Terraria;
using TerrariaApi.Server;
using System.IO;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;

namespace SmartRegions
{
    [ApiVersion(2, 1)]
    public class Plugin : TerrariaPlugin
    {
        private DBConnection DBConnection;
        List<SmartRegion> regions;
        PlayerData[] players = new PlayerData[255];
        struct PlayerData
        {
            public Dictionary<SmartRegion, DateTime> cooldowns;
            public SmartRegion regionToReplace;
            public void Reset()
            {
                cooldowns = new Dictionary<SmartRegion, DateTime>();
                regionToReplace = null;
            }
        }

        public Plugin(Main game) : base(game) { }

        public override Version Version => new Version(1, 4, 2);

        public override string Name => "Smart Regions";

        public override string Author => "GameRoom，肝帝熙恩汉化修复";

        public override string Description => "当玩家进入区域时运行命令。";

        public override void Initialize()
        {
            Commands.ChatCommands.Add(new Command("SmartRegions.manage", regionCommand, "smartregion"));
            Commands.ChatCommands.Add(new Command("SmartRegions.manage", replaceRegion, "replace"));

            ServerApi.Hooks.NetGreetPlayer.Register(this, OnGreetPlayer);
            ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);

            string folder = Path.Combine(TShock.SavePath, "SmartRegions");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            else
            {
                ReplaceLegacyRegionStorage();
            }

            DBConnection = new DBConnection();
            DBConnection.Initialize();
            regions = DBConnection.GetRegions();
        }

        protected override void Dispose(bool Disposing)
        {
            if (Disposing)
            {
                ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnGreetPlayer);
                ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);
                DBConnection?.Close();
            }
            base.Dispose(Disposing);
        }

        private void OnGreetPlayer(GreetPlayerEventArgs args)
        {
            players[args.Who].Reset();
        }

        public static event EventHandler<PlayerInRegionEventArgs> PlayerInRegion;

        public class PlayerInRegionEventArgs : EventArgs
        {
            public TSPlayer Player { get; set; }
            public SmartRegion Region { get; set; }
            public bool IgnoreRegion { get; set; }
        }

        void OnUpdate(EventArgs args)
        {
            foreach (TSPlayer player in TShock.Players)
                if (player != null && NetMessage.buffer[player.Index].broadcast)
                {
                    var inRegion = TShock.Regions.InAreaRegionName((int)(player.X / 16), (int)(player.Y / 16));
                    var hs = new HashSet<string>(inRegion);
                    var inSmartRegion = regions.Where(x => hs.Contains(x.name)).OrderByDescending(x => x.region.Z);

                    int regionCounter = 0;
                    foreach (SmartRegion region in inSmartRegion)
                    {
                        PlayerInRegionEventArgs eventArgs = new PlayerInRegionEventArgs()
                        {
                            Player = player,
                            Region = region
                        };
                        PlayerInRegion?.Invoke(this, eventArgs);
                        if (eventArgs.IgnoreRegion)
                            continue;

                        if ((regionCounter++ == 0 || !region.region.Name.EndsWith("--"))
                            && (!players[player.Index].cooldowns.ContainsKey(region)
                                || DateTime.UtcNow > players[player.Index].cooldowns[region]))
                        {
                            string file = Path.Combine(TShock.SavePath, "SmartRegions", region.command);
                            if (File.Exists(file))
                            {
                                foreach (string command in File.ReadAllLines(file))
                                {
                                    Commands.HandleCommand(TSPlayer.Server, replaceWithName(command, player));
                                }
                            }
                            else
                            {
                                Commands.HandleCommand(TSPlayer.Server, replaceWithName(region.command, player));
                            }
                            if (players[player.Index].cooldowns.ContainsKey(region))
                            {
                                players[player.Index].cooldowns[region] = DateTime.UtcNow.AddSeconds(region.cooldown);
                            }
                            else
                            {
                                players[player.Index].cooldowns.Add(region, DateTime.UtcNow.AddSeconds(region.cooldown));
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
                await regionCommandInner(args);
            }
            catch (Exception e)
            {
                TShock.Log.Error(e.ToString());
                args.Player.SendErrorMessage("命令执行时出现错误。");
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
                            args.Player.SendErrorMessage("语法错误！正确的语法是：/smartregion add <区域名称> <冷却时间> <命令或文件>");
                        }
                        else
                        {
                            double cooldown = 0;
                            if (!double.TryParse(args.Parameters[2], out cooldown))
                            {
                                args.Player.SendErrorMessage("无效的语法！正确的语法是：/smartregion add <区域名> <冷却时间> <命令或文件>");
                                return;
                            }
                            string command = string.Join(" ", args.Parameters.GetRange(3, args.Parameters.Count - 3));
                            if (!TShock.Regions.Regions.Exists(x => x.Name == args.Parameters[1]))
                            {
                                args.Player.SendErrorMessage("区域 {0} 不存在！", args.Parameters[1]);
                                IEnumerable<string> regionNames = from region_ in TShock.Regions.Regions
                                                                  where region_.WorldID == Main.worldID.ToString()
                                                                  select region_.Name;
                                PaginationTools.SendPage(args.Player, 1, PaginationTools.BuildLinesFromTerms(regionNames),
                                    new PaginationTools.Settings
                                    {
                                        HeaderFormat = "区域 ({0}/{1}) ：",
                                        FooterFormat = "输入 {0}region list {{0}} 查看更多。".SFormat(Commands.Specifier),
                                        NothingToDisplayString = "目前没有定义任何区域。"
                                    });
                            }
                            else
                            {
                                string cmdName = "";
                                for (int i = 1; i < command.Length && command[i] != ' '; i++)
                                {
                                    cmdName += command[i];
                                }
                                Command cmd = Commands.ChatCommands.FirstOrDefault(c => c.HasAlias(cmdName));
                                if (cmd != null && !cmd.CanRun(args.Player))
                                {
                                    args.Player.SendErrorMessage("你没有权限使用此命令，因此不能创建智能区域！");
                                    return;
                                }
                                if (cmd != null && !cmd.AllowServer)
                                {
                                    args.Player.SendErrorMessage("你的命令必须允许服务器执行！");
                                    return;
                                }

                                var existingRegion = regions.FirstOrDefault(x => x.name == args.Parameters[1]);
                                var newRegion = new SmartRegion
                                {
                                    name = args.Parameters[1],
                                    cooldown = cooldown,
                                    command = command
                                };
                                if (existingRegion != null)
                                {
                                    players[args.Player.Index].regionToReplace = newRegion;
                                    args.Player.SendErrorMessage("智能区域 {0} 已经存在！请输入 /replace 替换它。", args.Parameters[1]);
                                }
                                else
                                {
                                    regions.Add(newRegion);
                                    await DBConnection.SaveRegion(newRegion);
                                    args.Player.SendSuccessMessage("智能区域已添加！");
                                }
                            }
                        }
                    }
                    break;
                case "remove":
                    {
                        if (args.Parameters.Count != 2)
                        {
                            args.Player.SendErrorMessage("无效的语法！正确的语法是：/smartregion remove <区域名>");
                        }
                        else
                        {
                            var region = regions.FirstOrDefault(x => x.name == args.Parameters[1]);
                            if (region == null)
                            {
                                args.Player.SendErrorMessage("不存在这样的智能区域！");
                            }
                            else
                            {
                                regions.Remove(region);
                                await DBConnection.RemoveRegion(region.name);
                                args.Player.SendSuccessMessage("智能区域 {0} 已被移除！", args.Parameters[1]);
                            }
                        }
                    }
                    break;
                case "check":
                    {
                        if (args.Parameters.Count != 2)
                        {
                            args.Player.SendErrorMessage("无效的语法！正确的语法是：/smartregion check <区域名>");
                        }
                        else
                        {
                            var region = regions.FirstOrDefault(x => x.name == args.Parameters[1]);
                            if (region == null)
                            {
                                args.Player.SendInfoMessage("该区域没有关联的命令。");
                            }
                            else
                            {
                                string file = Path.Combine(TShock.SavePath, "SmartRegions", region.command), commands;
                                if (File.Exists(file))
                                    commands = "脚本中的命令如下：\n" + File.ReadAllText(file);
                                else
                                    commands = "命令如下：\n" + region.command;

                                args.Player.SendInfoMessage("区域 {0} 的冷却时间为 {1} 秒，使用命令：{2}", args.Parameters[1], region.cooldown, commands);
                            }
                        }
                    }
                    break;
                case "list":
                    {
                        int pageNumber = 1;
                        int maxDist = int.MaxValue;
                        if (args.Parameters.Count > 1)
                        {
                            int.TryParse(args.Parameters[1], out pageNumber);
                        }
                        if (args.Parameters.Count > 2)
                        {
                            if (args.Player == TSPlayer.Server)
                            {
                                args.Player.SendErrorMessage("如果您是服务器后台，不能使用距离参数。");
                                return;
                            }
                            int.TryParse(args.Parameters[2], out maxDist);
                        }

                        List<SmartRegion> regionList = regions;
                        if (maxDist < int.MaxValue)
                        {
                            regionList = regionList
                                .Where(r => r.region != null && Vector2.Distance(args.Player.TPlayer.position, r.region.Area.Center() * 16) < maxDist * 16)
                                .ToList();
                        }

                        List<string> regionNames = regionList.Select(r => r.name).ToList();
                        regionNames.Sort();

                        if (regionNames.Count == 0)
                        {
                            string suffix = "";
                            if (maxDist < int.MaxValue)
                            {
                                suffix = "附近的";
                            }
                            args.Player.SendErrorMessage($"没有{suffix}智能区域。");
                        }
                        else
                        {
                            PaginationTools.SendPage(
                                args.Player, pageNumber, PaginationTools.BuildLinesFromTerms(regionNames),
                                new PaginationTools.Settings
                                {
                                    HeaderFormat = "智能区域 ({0}/{1}) ：",
                                    FooterFormat = $"输入 {Commands.Specifier}smartregion list {{0}} 查看更多。"
                                }
                            );
                        }
                    }
                    break;

                default:
                    {
                        args.Player.SendInfoMessage("/smartregion 子命令:\nadd <区域名> <冷却时间> <命令或文件>\nremove <区域名>\ncheck <区域名>\nlist [页码] [最大距离]");
                    }
                    break;
            }
        }

        void ReplaceLegacyRegionStorage()
        {
            string path = Path.Combine(TShock.SavePath, "SmartRegions", "config.txt");
            if (File.Exists(path))
            {
                var tasks = new List<Task>();
                try
                {
                    string[] lines = File.ReadAllLines(path);
                    for (int i = 0; i < lines.Length; i += 3)
                    {
                        var task = DBConnection.SaveRegion(new SmartRegion
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
                if (players[args.Player.Index].regionToReplace == null)
                {
                    args.Player.SendErrorMessage("你现在不能做这个操作！");
                }
                else
                {
                    regions.RemoveAll(x => x.name == players[args.Player.Index].regionToReplace.name);
                    regions.Add(players[args.Player.Index].regionToReplace);
                    await DBConnection.SaveRegion(players[args.Player.Index].regionToReplace);
                    players[args.Player.Index].regionToReplace = null;
                    args.Player.SendSuccessMessage("区域替换成功！");
                }
            }
            catch (Exception e)
            {
                TShock.Log.Error(e.ToString());
                args.Player.SendErrorMessage("命令执行时出现错误。");
            }
        }
    }
}