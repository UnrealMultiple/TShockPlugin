using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;


namespace SpawnInfra
{
    [ApiVersion(2, 1)]
    public class Plugin : TerrariaPlugin
    {
        #region 插件信息
        public override string Name => "生成基础建设";
        public override string Author => "羽学";
        public override Version Version => new Version(1, 1, 0);
        public override string Description => "给新世界创建NPC住房、地狱直通车、地表和地狱平台（轨道）";
        #endregion

        #region 注册与释放
        public Plugin(Main game) : base(game) { }
        public override void Initialize()
        {
            LoadConfig();
            GeneralHooks.ReloadEvent += (_) => LoadConfig();
            ServerApi.Hooks.GamePostInitialize.Register(this, OnGamePostInitialize);
            Commands.ChatCommands.Add(new Command("room.use", Comds.Comd, "rm", "基建")
            {
                HelpText = "自动建监狱"
            });
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GeneralHooks.ReloadEvent -= (_) => LoadConfig();
                ServerApi.Hooks.GamePostInitialize.Deregister(this, OnGamePostInitialize);
                Commands.ChatCommands.Remove(new Command("room.use", Comds.Comd, "rm", "监狱"));
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
            TShock.Log.ConsoleInfo("[生成基础建设]重新加载配置完毕。");
        }
        #endregion

        #region  游戏初始化建筑设施（世界平台/轨道+地狱平台/轨道+出生点监狱+直通车）
        private static void OnGamePostInitialize(EventArgs args)
        {
            if (args == null) return;

            if (Config.Enabled)
            {
                foreach (var item in Config.Prison)
                {
                    GenLargeHouse(Main.spawnTileX + item.spawnTileX, Main.spawnTileY + item.spawnTileY, item.Width, item.Height);
                }

                foreach (var item in Config.HellTunnel)
                {
                    HellTunnel(Main.spawnTileX + item.SpawnTileX, Main.spawnTileY + item.SpawnTileY);
                    UnderworldPlatform(Main.UnderworldLayer + item.PlatformY, item.PlatformY);
                }

                foreach (var item in Config.WorldPlatform)
                {
                    WorldPlatform(Main.spawnTileY + item.SpawnTileY, item.Height);
                    BuildOceanPlatforms(item.Wide, item.Height2, item.Interval);
                }

                TShock.Utils.Broadcast(
                    "基础建设已完成，如需重置布局\n" +
                    "请输入指令后重启服务器：/rm reset", 250, 247, 105);

                Commands.HandleCommand(TSPlayer.Server, "/save");

                Config.Enabled = false;
                Config.Write();
            }
        }
        #endregion

        #region 构造监狱集群方法
        public static void GenLargeHouse(int startX, int startY, int width, int height)
        {
            if (!Config.Enabled2) return;

            int roomsAcross = width / 5;
            int roomsDown = height / 9;

            for (int row = 0; row < roomsDown; row++)
            {
                for (int col = 0; col < roomsAcross; col++)
                {
                    bool isRight = col == roomsAcross;
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
                    if (i == num && j > posY - 5 || i == num + num2 - 1 && j > posY - 5 || j == posY - 1)
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

        //指令方法 用来给玩家自己建晶塔房用
        public static Task AsyncGenRoom(TSPlayer plr, int posX, int posY, int total = 1, bool isRight = true, bool needCenter = false)
        {
            int secondLast = Utils.GetUnixTimestamp;
            return Task.Run(delegate
            {
                int num = 5;
                int num2 = 1 + total * num;
                int num3 = needCenter ? posX - num2 / 2 : posX;
                for (int i = 0; i < total; i++)
                {
                    GenRoom(num3, posY, isRight);
                    num3 += num;
                }
            }).ContinueWith(delegate
            {
                TileHelper.InformPlayers();
                int value = Utils.GetUnixTimestamp - secondLast;
                plr.SendSuccessMessage($"已生成{total}个小房子，用时{value}秒。");
            });
        }

        #endregion

        #region 左海平台
        private static void BuildOceanPlatforms(int wide, int hight, int interval)
        {
            if (!Config.Enabled4_1) return;

            //第一层比间隔层高10格
            int y = Main.oceanBG + interval - 10;

            while (y < Main.worldSurface - hight)
            {
                for (int x = 0; x < wide; x++)
                {
                    if (x >= Main.maxTilesX || x < 0) continue;
                    WorldGen.PlaceTile(x, y, Config.WorldPlatform[0].ID, false, true, -1, Config.WorldPlatform[0].Style);
                }
                y += interval;
            }
        }
        #endregion

        #region 世界平台
        private static void WorldPlatform(int posY, int hight)
        {
            int clear = Math.Max(3, hight);

            for (int x = 0; x < Main.maxTilesX; x++)
            {
                //防止世界平台搭进左海
                if (x - clear <= Main.oceanBG + 200) continue;

                for (int y = posY - clear; y <= posY; y++)
                {
                    Main.tile[x, y].ClearEverything();
                }

                if (Config.Enabled3)
                    WorldGen.PlaceTile(x, posY, Config.WorldPlatform[0].ID, false, true, -1, Config.WorldPlatform[0].Style);

                if (Config.Enabled4)
                    WorldGen.PlaceTile(x, posY - 1, 314, false, true, -1, 0);
            }
        }
        #endregion

        #region 地狱直通车
        private static int HellTunnel(int posX, int posY)
        {
            int hell = 0;
            int xtile;
            for (hell = Main.UnderworldLayer + 10; hell <= Main.maxTilesY - 100; hell++)
            {
                xtile = posX;
                Parallel.For(posX, posX + 8, delegate (int cwidth, ParallelLoopState state)
                {
                    if (Main.tile[cwidth, hell].active() && !Main.tile[cwidth, hell].lava())
                    {
                        state.Stop();
                        xtile = cwidth;
                    }
                });
                if (!Main.tile[xtile, hell].active())
                {
                    break;
                }
            }
            int Width = 5;
            int num = hell;
            int Xstart = posX - 2;
            Parallel.For(Xstart, Xstart + Width, delegate (int cx)
            {
                Parallel.For(posY, hell, delegate (int cy)
                {
                    ITile val = Main.tile[cx, cy];
                    val.ClearEverything();
                    if (cx == Xstart + Width / 2)
                    {
                        val.type = Config.HellTunnel[0].ID2;
                        val.active(true);
                        val.slope(0);
                        val.halfBrick(false);
                    }
                    else if (cx == Xstart || cx == Xstart + Width - 1)
                    {
                        val.type = Config.HellTunnel[0].ID;
                        val.active(true);
                        val.slope(0);
                        val.halfBrick(false);
                    }
                });
            });

            if (Config.Enabled5)
            {
                WorldGen.PlaceTile(Xstart + 1, posY, Config.HellTunnel[0].PlatformID, false, true, -1, Config.HellTunnel[0].PlatformStyle);
                WorldGen.PlaceTile(Xstart + 2, posY, Config.HellTunnel[0].PlatformID, false, true, -1, Config.HellTunnel[0].PlatformStyle);
                WorldGen.PlaceTile(Xstart + 3, posY, Config.HellTunnel[0].PlatformID, false, true, -1, Config.HellTunnel[0].PlatformStyle);
            }
            return hell;
        }
        #endregion

        #region 地狱平台
        private static void UnderworldPlatform(int posY, int hight)
        {
            int Clear = posY - hight;
            for (int y = posY; y > Clear; y--)
                for (int x = 0; x < Main.maxTilesX; x++)
                {
                    Main.tile[x, y].ClearEverything(); // 清除方块

                    if (Config.Enabled6)
                        WorldGen.PlaceTile(x, posY, Config.HellTunnel[0].PlatformID, false, true, -1, Config.HellTunnel[0].PlatformStyle); //地狱平台

                    if (Config.Enabled7)
                        WorldGen.PlaceTile(x, posY - 1, 314, false, true, -1, 0); //地狱轨道
                }
        }
        #endregion


    }
}