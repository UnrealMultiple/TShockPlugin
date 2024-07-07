using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;


namespace Plugin
{
    [ApiVersion(2, 1)]
    public class Plugin : TerrariaPlugin
    {
        #region 插件信息
        public override string Name => "出生点自动建监狱";
        public override string Author => "羽学";
        public override Version Version => new Version(1, 0, 0);
        public override string Description => "涡轮增压不蒸鸭";
        #endregion

        #region 注册与释放
        public Plugin(Main game) : base(game) { }
        public override void Initialize()
        {
            LoadConfig();
            GeneralHooks.ReloadEvent += (_) => LoadConfig();
            ServerApi.Hooks.GamePostInitialize.Register(this, OnGamePostInitialize);
            Commands.ChatCommands.Add(new Command("room.admin", Comds.Comd, "rm", "监狱")
            {
                HelpText = "出生点监狱"
            });
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GeneralHooks.ReloadEvent -= (_) => LoadConfig();
                ServerApi.Hooks.GamePostInitialize.Deregister(this, OnGamePostInitialize);
                Commands.ChatCommands.Remove(new Command("room.admin", Comds.Comd, "rm", "监狱"));
            }
            base.Dispose(disposing);
        }
        #endregion

        #region 配置重载读取与写入方法
        internal static Configuration Config = new();
        private static void LoadConfig()
        {
            Config = Configuration.Read();
            Config.Write();
            TShock.Log.ConsoleInfo("[出生点监狱]重新加载配置完毕。");
        }
        #endregion

        #region  游戏初始化建大房子方法
        private static void OnGamePostInitialize(EventArgs args)
        {
            if (args == null) return;

            if (Config.Enabled)
            {
                GenLargeHouse(Main.spawnTileX + Config.spawnTileX, Main.spawnTileY + Config.spawnTileY, Config.width, Config.height);

                TShock.Utils.Broadcast(
                    "出生点监狱已建成，如需重置监狱集群" +
                    "\n请输入指令后重启服务器：/rm reset", 250, 247, 105);

                TShockAPI.Commands.HandleCommand(TSPlayer.Server, "/save");

                Config.Enabled = false;
                Config.Write();
            }
        }
        #endregion

        #region 构造大房间方法
        public static void GenLargeHouse(int startX, int startY, int width, int height)
        {
            // 计算需要多少个小房间
            int roomsAcross = width / 5;
            int roomsDown = height / 9;

            // 生成每一行的房间
            for (int row = 0; row < roomsDown; row++)
            {
                // 生成每一列的房间
                for (int col = 0; col < roomsAcross; col++)
                {
                    // 根据当前列确定是否为右侧
                    bool isRight = (col == roomsAcross);

                    GenRoom(startX + col * 5, startY + row * 9, isRight);
                }
            }
        }
        #endregion

        #region 创建小房间方法
        public static void GenRoom(int posX, int posY, bool isRight = true)
        {
            RoomTheme roomTheme = new RoomTheme();
            roomTheme.SetGlass();
            ushort tile = roomTheme.tile;
            ushort wall = roomTheme.wall;
            TileInfo platform = roomTheme.platform;
            TileInfo chair = roomTheme.chair;
            TileInfo bench = roomTheme.bench;
            TileInfo torch = roomTheme.torch;
            int num = posX;
            int num2 = 6;
            int num3 = 10;
            if (!isRight)
            {
                num += 6;
            }
            for (int i = num; i < num + num2; i++)
            {
                for (int j = posY - num3; j < posY; j++)
                {
                    ITile val = Main.tile[i, j];
                    val.ClearEverything();
                    if (i > num && j < posY - 5 && i < num + num2 - 1 && j > posY - num3)
                    {
                        val.wall = wall;
                    }
                    if ((i == num && j > posY - 5) || (i == num + num2 - 1 && j > posY - 5) || j == posY - 1)
                    {
                        WorldGen.PlaceTile(i, j, platform.id, false, true, -1, platform.style);
                    }
                    else if (i == num || i == num + num2 - 1 || j == posY - num3 || j == posY - 5)
                    {
                        val.type = tile;
                        val.active(true);
                        val.slope(0);
                        val.halfBrick(false);
                    }
                }
            }
            if (isRight)
            {
                WorldGen.PlaceTile(num + 1, posY - 6, chair.id, false, true, 0, chair.style);
                ITile obj = Main.tile[num + 1, posY - 6];
                obj.frameX += 18;
                ITile obj2 = Main.tile[num + 1, posY - 7];
                obj2.frameX += 18;
                WorldGen.PlaceTile(num + 2, posY - 6, bench.id, false, true, -1, bench.style);
                WorldGen.PlaceTile(num + 4, posY - 5, torch.id, false, true, -1, torch.style);
            }
            else
            {
                WorldGen.PlaceTile(num + 4, posY - 6, chair.id, false, true, 0, chair.style);
                WorldGen.PlaceTile(num + 2, posY - 6, bench.id, false, true, -1, bench.style);
                WorldGen.PlaceTile(num + 1, posY - 5, torch.id, false, true, -1, torch.style);
            }
        }

        public static Task AsyncGenRoom(TSPlayer plr, int posX, int posY, int total = 1, bool isRight = true, bool needCenter = false)
        {
            int secondLast = Utils.GetUnixTimestamp;
            return Task.Run(delegate
            {
                int num = 5;
                int num2 = 1 + total * num;
                int num3 = (needCenter ? (posX - num2 / 2) : posX);
                for (int i = 0; i < total; i++)
                {
                    GenRoom(num3, posY, isRight);
                    num3 += num;
                }
            }).ContinueWith(delegate
            {
                TileHelper.InformPlayers();
                int value = Utils.GetUnixTimestamp - secondLast;
                plr.SendSuccessMessage($"已生成{total}个监狱，用时{value}秒。");
            });
        }

        #endregion

    }
}