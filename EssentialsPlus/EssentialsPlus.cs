using System.Data;
using EssentialsPlus.Db;
using EssentialsPlus.Extensions;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace EssentialsPlus
{
    [ApiVersion(2, 1)]
    public class EssentialsPlus : TerrariaPlugin
    {
        public static Config Config { get; private set; }
        public static IDbConnection Db { get; private set; }
        public static HomeManager Homes { get; private set; }
        public static MuteManager Mutes { get; private set; }

        public override string Author => "WhiteX等人，Average,Cjx,肝帝熙恩翻译,Cai更新";

        public override string Description => "增强版Essentials";

        public override string Name => "EssentialsPlus";

        public override Version Version => new Version(1, 0, 1);


        public EssentialsPlus(Main game)
            : base(game)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GeneralHooks.ReloadEvent -= OnReload;
                PlayerHooks.PlayerCommand -= OnPlayerCommand;

                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
                ServerApi.Hooks.GamePostInitialize.Deregister(this, OnPostInitialize);
                ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
                ServerApi.Hooks.ServerJoin.Deregister(this, OnJoin);
            }
            base.Dispose(disposing);
        }

        public override void Initialize()
        {
            GeneralHooks.ReloadEvent += OnReload;
            PlayerHooks.PlayerCommand += OnPlayerCommand;

            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
            ServerApi.Hooks.GamePostInitialize.Register(this, OnPostInitialize);
            ServerApi.Hooks.NetGetData.Register(this, OnGetData);
            ServerApi.Hooks.ServerJoin.Register(this, OnJoin);
        }

        private void OnReload(ReloadEventArgs e)
        {
            string path = Path.Combine(TShock.SavePath, "essentials.json");
            Config = Config.Read(path);
            if (!File.Exists(path))
            {
                Config.Write(path);
            }
            Homes.Reload();
            e.Player.SendSuccessMessage("[EssentialsPlus] 重新加载配置和家!");
        }

        private readonly List<string> teleportCommands = new List<string>
        {
            "tp", "tppos", "tpnpc", "warp", "spawn", "home"
        };

        private void OnPlayerCommand(PlayerCommandEventArgs e)
        {
            if (e.Handled || e.Player == null)
            {
                return;
            }

            Command command = e.CommandList.FirstOrDefault();
            if (command == null || (command.Permissions.Any() && !command.Permissions.Any(s => e.Player.Group.HasPermission(s))))
            {
                return;
            }

            if (e.Player.TPlayer.hostile &&
                command.Names.Select(s => s.ToLowerInvariant())
                    .Intersect(Config.DisabledCommandsInPvp.Select(s => s.ToLowerInvariant()))
                    .Any())
            {
                e.Player.SendErrorMessage("在PvP中无法使用该命令！");
                e.Handled = true;
                return;
            }

            if (e.Player.Group.HasPermission(Permissions.LastCommand) && command.CommandDelegate != Commands.RepeatLast)
            {
                e.Player.GetPlayerInfo().LastCommand = e.CommandText;
            }

            if (teleportCommands.Contains(e.CommandName) && e.Player.Group.HasPermission(Permissions.TpBack))
            {
                e.Player.GetPlayerInfo().PushBackHistory(e.Player.TPlayer.position);
            }
        }


        private void OnInitialize(EventArgs e)
        {
            #region Config

            string path = Path.Combine(TShock.SavePath, "essentials.json");
            Config = Config.Read(path);
            if (!File.Exists(path))
            {
                Config.Write(path);
            }

            #endregion

            #region Database

            if (TShock.Config.Settings.StorageType.Equals("mysql", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(Config.MySqlHost) ||
                    string.IsNullOrWhiteSpace(Config.MySqlDbName))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[Essentials+] MySQL已启用，但未设置Essentials+ MySQL配置。");
                    Console.WriteLine("[Essentials+] 请在essentials.json中配置您的MySQL服务器信息，然后重新启动服务器。");
                    Console.WriteLine("[Essentials+] 此插件现在将禁用自身...");
                    Console.ResetColor();

                    GeneralHooks.ReloadEvent -= OnReload;
                    PlayerHooks.PlayerCommand -= OnPlayerCommand;

                    ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
                    ServerApi.Hooks.GamePostInitialize.Deregister(this, OnPostInitialize);
                    ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
                    ServerApi.Hooks.ServerJoin.Deregister(this, OnJoin);

                    return;
                }

                string[] host = Config.MySqlHost.Split(':');
                Db = new MySqlConnection
                {
                    ConnectionString = String.Format("Server={0}; Port={1}; Database={2}; Uid={3}; Pwd={4};",
                        host[0],
                        host.Length == 1 ? "3306" : host[1],
                        Config.MySqlDbName,
                        Config.MySqlUsername,
                        Config.MySqlPassword)
                };
            }
            else if (TShock.Config.Settings.StorageType.Equals("sqlite", StringComparison.OrdinalIgnoreCase))
            {
                Db = new SqliteConnection(
                    "Data Source=" + Path.Combine(TShock.SavePath, "essentials.sqlite"));
            }
            else
            {
                throw new InvalidOperationException("无效的存储类型！");
            }

            Mutes = new MuteManager(Db);

            #endregion

            #region Commands

            //允许覆盖已创建的命令。
            Action<Command> Add = c =>
            {
                //找到与新命令匹配的名称和别名的任何命令，并将其删除。
                TShockAPI.Commands.ChatCommands.RemoveAll(c2 => c2.Names.Exists(s2 => c.Names.Contains(s2)));
                //然后添加新命令。
                TShockAPI.Commands.ChatCommands.Add(c);
            };

            Add(new Command(Permissions.Find, Commands.Find, "find", "查找")
            {
                HelpText = "查找具有指定名称的物品和/或NPC。"
            });

            Add(new Command(Permissions.FreezeTime, Commands.FreezeTime, "freezetime", "冻结时间")
            {
                HelpText = "切换冻结时间。"
            });

            Add(new Command(Permissions.HomeDelete, Commands.DeleteHome, "delhome", "删除家点")
            {
                AllowServer = false,
                HelpText = "删除您的一个家点。"
            });
            Add(new Command(Permissions.HomeSet, Commands.SetHome, "sethome", "设置家点")
            {
                AllowServer = false,
                HelpText = "设置您的一个家点。"
            });
            Add(new Command(Permissions.HomeTp, Commands.MyHome, "myhome", "我的家点")
            {
                AllowServer = false,
                HelpText = "传送到您的一个家点。"
            });

            Add(new Command(Permissions.KickAll, Commands.KickAll, "kickall", "踢出所有人")
            {
                HelpText = "踢出服务器上的所有人。"
            });

            Add(new Command(Permissions.LastCommand, Commands.RepeatLast, "=", "重复上一条命令")
            {
                HelpText = "允许您重复上一条命令。"
            });

            Add(new Command(Permissions.More, Commands.More, "more", "最大化堆叠")
            {
                AllowServer = false,
                HelpText = "最大化手持物品的堆叠。"
            });

            //这将覆盖TShock的 'mute' 命令
            Add(new Command(Permissions.Mute, Commands.Mute, "mute", "禁言管理")
            {
                HelpText = "管理禁言。"
            });

            Add(new Command(Permissions.PvP, Commands.PvP, "pvpget2", "切换PvP状态")
            {
                AllowServer = false,
                HelpText = "切换您的PvP状态。"
            });

            Add(new Command(Permissions.Ruler, Commands.Ruler, "ruler", "测量工具")
            {
                AllowServer = false,
                HelpText = "允许您测量两个方块之间的距离。"
            });

            Add(new Command(Permissions.Send, Commands.Send, "send", "广播消息")
            {
                HelpText = "以自定义颜色广播消息。"
            });

            Add(new Command(Permissions.Sudo, Commands.Sudo, "sudo", "代执行")
            {
                HelpText = "允许您以其他用户的身份执行命令。"
            });

            Add(new Command(Permissions.TimeCmd, Commands.TimeCmd, "timecmd", "定时命令")
            {
                HelpText = "在给定时间间隔后执行命令。"
            });

            Add(new Command(Permissions.TpBack, Commands.Back, "eback", "b", "回到")
            {
                AllowServer = false,
                HelpText = "在死亡或传送后将您传送回之前的位置。"
            });
            Add(new Command(Permissions.TpDown, Commands.Down, "down", "向下传送")
            {
                AllowServer = false,
                HelpText = "通过一个方块层向下传送您。"
            });
            Add(new Command(Permissions.TpLeft, Commands.Left, "left", "向左传送")
            {
                AllowServer = false,
                HelpText = "通过一个方块层向左传送您。"
            });
            Add(new Command(Permissions.TpRight, Commands.Right, "right", "向右传送")
            {
                AllowServer = false,
                HelpText = "通过一个方块层向右传送您。"
            });
            Add(new Command(Permissions.TpUp, Commands.Up, "up", "向上传送")
            {
                AllowServer = false,
                HelpText = "通过一个方块层向上传送您。"
            });


            #endregion
        }


        private void OnPostInitialize(EventArgs args)
        {
            Homes = new HomeManager(Db);
        }

        private async void OnJoin(JoinEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }

            TSPlayer player = TShock.Players[e.Who];
            if (player == null)
            {
                return;
            }

            DateTime muteExpiration = Mutes.GetExpiration(player);

            if (DateTime.UtcNow < muteExpiration)
            {
                player.mute = true;
                try
                {
                    await Task.Delay(muteExpiration - DateTime.UtcNow, player.GetPlayerInfo().MuteToken);
                    player.mute = false;
                    player.SendInfoMessage("您已被解除禁言。");
                }
                catch (TaskCanceledException)
                {
                }
            }
        }

        private void OnGetData(GetDataEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }

            TSPlayer tsplayer = TShock.Players[e.Msg.whoAmI];
            if (tsplayer == null)
            {
                return;
            }

            switch (e.MsgID)
            {
                #region Packet 118 - PlayerDeathV2

                case PacketTypes.PlayerDeathV2:
                    if (tsplayer.Group.HasPermission(Permissions.TpBack))
                    {
                        tsplayer.GetPlayerInfo().PushBackHistory(tsplayer.TPlayer.position);
                    }
                    return;

                case PacketTypes.Teleport:
                    {
                        if (tsplayer.Group.HasPermission(Permissions.TpBack))
                        {
                            using (MemoryStream ms = new MemoryStream(e.Msg.readBuffer, e.Index, e.Length))
                            {
                                BitsByte flags = (byte)ms.ReadByte();

                                int type = 0;
                                if (flags[1])
                                {
                                    type = 2;
                                }

                                if (type == 0 && tsplayer.Group.HasPermission(TShockAPI.Permissions.rod))
                                {
                                    tsplayer.GetPlayerInfo().PushBackHistory(tsplayer.TPlayer.position);
                                }
                                else if (type == 2 && tsplayer.Group.HasPermission(TShockAPI.Permissions.wormhole))
                                {
                                    tsplayer.GetPlayerInfo().PushBackHistory(tsplayer.TPlayer.position);
                                }
                            }
                        }
                    }
                    return;

                    #endregion
            }
        }
    }
}
