using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace PacketsStop
{
    [ApiVersion(2, 1)]
    public class PacketsStop : TerrariaPlugin
    {

        public override string Name => "数据包拦截 PacketsStop";
        public override Version Version => new Version(1, 0, 0);
        public override string Author => "羽学 感谢少司命";
        public override string Description => "拦截没有指定权限的用户组数据包";

        #region 配置方法的工具
        private readonly Dictionary<string, Dictionary<PacketTypes, DateTime>> countDictionary = new Dictionary<string, Dictionary<PacketTypes, DateTime>>();
        private const double PacketInterval = 1000.0;
        private bool _Enabled = false;
        internal static Configuration Config;
        private HashSet<PacketTypes> Packets;
        #endregion

        public PacketsStop(Main game) : base(game)
        {
            Config = new Configuration();
        }

        public override void Initialize()
        {
            LoadConfig();
            Packets = GetPackets();
            ServerApi.Hooks.NetGetData.Register(this, OnGetData, int.MaxValue);
            Commands.ChatCommands.Add(new Command("拦截", Command, "拦截"));
            GeneralHooks.ReloadEvent += LoadConfig;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
            }
            base.Dispose(disposing);
        }

        #region 配置文件创建与重读加载方法
        private static void LoadConfig(ReloadEventArgs args = null)
        {
            if (!File.Exists(Configuration.FilePath))
            {
                var defaultConfig = new Configuration();
                defaultConfig.Write(Configuration.FilePath);
            }
            else
            {
                Config = Configuration.Read(Configuration.FilePath);
            }

            if (args != null && args.Player != null)
            {
                args.Player.SendSuccessMessage("[数据包拦截]重新加载配置完毕。");
            }
        }
        #endregion

        #region 指令


        private void Command(CommandArgs args)
        {
            if (args.Player != null && !args.Player.HasPermission("拦截"))
            {
                args.Player.SendErrorMessage("你没有使用数据包拦截的权限");
                return;
            }
            else
            {
                _Enabled = !_Enabled;
                TSPlayer.All.SendInfoMessage($"[数据包拦截]已{(_Enabled ? "启用" : "禁用")}");
            }


            if (args.Parameters.Count != 2)
            {
                args.Player.SendInfoMessage("/拦截 add 玩家名 - 将玩家添加到LJ组(不存在自动创建)。\n/拦截 del 玩家名 - 将玩家从LJ组移除并设为default组。");
                return;
            }

            string Action = args.Parameters[0];
            string Name = args.Parameters[1];
            var Account = TShock.UserAccounts.GetUserAccountByName(Name);

            if (Account == null)
            {
                args.Player.SendInfoMessage($"无法找到名为'{Name}'的在线玩家。");
                return;
            }

            switch (Action.ToLower())
            {
                case "add":
                    if (!TShock.Groups.GroupExists("LJ"))
                    {
                        TShock.Groups.AddGroup("LJ", "", "tshock.canchat,tshock,tshock.partychat,tshock.sendemoji", "045,235,203");
                        args.Player.SendSuccessMessage("LJ组已创建。");
                    }

                    try
                    {
                        TShock.UserAccounts.SetUserGroup(Account, "LJ");
                        args.Player.SendSuccessMessage($"{Name}已被设为LJ组成员。");
                    }
                    catch (Exception ex)
                    {
                        args.Player.SendErrorMessage($"无法将{Name}设为LJ组成员。错误信息: \n{ex.Message}");
                    }
                    break;
                case "del":
                    try
                    {
                        TShock.UserAccounts.SetUserGroup(Account, "default");
                        args.Player.SendSuccessMessage($"{Name}已从LJ组移除，并被设为default组。");
                    }
                    catch (Exception ex)
                    {
                        args.Player.SendErrorMessage($"无法将{Name}从LJ组移除或设为default组。错误信息: \n{ex.Message}");
                    }
                    break;
                default:
                    args.Player.SendInfoMessage("无效的子命令。使用 'add' 或 'del'。");
                    break;
            }
        }
        #endregion

        #region 获取数据包方法
        private void OnGetData(GetDataEventArgs args)
        {
            TSPlayer player = TShock.Players[args.Msg.whoAmI];

            if (!_Enabled ||! Packets.Contains(args.MsgID))
            {
                return;
            }

            if (!player.HasPermission("免拦截"))
            {

                HandlePacket(player, args.MsgID);
            }
        }

        private HashSet<PacketTypes> GetPackets()
        {
            HashSet<PacketTypes> Packets = new HashSet<PacketTypes>();
            foreach (string packetName in Config.Packets)
            {
                if (Enum.TryParse(packetName, out PacketTypes packetType))
                {
                    Packets.Add(packetType);
                }
                else
                {
                    TShock.Log.Error($"无法识别的数据包类型名称: {packetName}");
                }
            }
            return Packets;
        }
        #endregion

        #region 处理数据包方法
        private void HandlePacket(TSPlayer args, PacketTypes packetType)
        {
            if (_Enabled)
            {
                DateTime now = DateTime.Now;
                if (args.Name != null)
                {
                    if (!countDictionary.TryGetValue(args.Name, out var packetDictionary))
                    {
                        packetDictionary = new Dictionary<PacketTypes, DateTime>();
                        countDictionary[args.Name] = packetDictionary;
                    }
                    if (packetDictionary.TryGetValue(packetType, out DateTime lastPacketTime))
                    {
                        if ((now - lastPacketTime).TotalMilliseconds >= PacketInterval)
                        {
                            packetDictionary[packetType] = now;
                        }
                    }
                    else
                    {
                        packetDictionary[packetType] = now;
                    }
                }
            }
        }
        #endregion
    }
}