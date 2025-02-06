using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace SpawnInfra;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    #region 插件信息
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!; public override string Author => "羽学";
    public override Version Version => new Version(1, 5, 9);
    public override string Description => GetString("给新世界创建NPC住房、仓库、洞穴刷怪场、地狱/微光直通车、地表和地狱世界级平台（轨道）");
    #endregion

    #region 注册与释放
    public Plugin(Main game) : base(game)
    {
        this._reloadHandler = (_) => LoadConfig();
    }
    private readonly GeneralHooks.ReloadEventD _reloadHandler;
    public override void Initialize()
    {
        LoadConfig();
        GeneralHooks.ReloadEvent += this._reloadHandler;
        //提高优先级避免覆盖CreateSpawn插件
        ServerApi.Hooks.GamePostInitialize.Register(this, OnGamePostInitialize, 20);
        GetDataHandlers.PlayerUpdate.Register(this.PlayerUpdate);
        Commands.ChatCommands.Add(new Command("room.use", Comds.Comd, "rm", "基建")
        {
            HelpText = GetString("生成基础建设")
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GeneralHooks.ReloadEvent -= this._reloadHandler;
            ServerApi.Hooks.GamePostInitialize.Deregister(this, OnGamePostInitialize);
            GetDataHandlers.PlayerUpdate.UnRegister(this.PlayerUpdate);
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == Comds.Comd);
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
        TShock.Log.ConsoleInfo(GetString("[生成基础建设]重新加载配置完毕。"));
    }
    #endregion

    #region  游戏初始化建筑设施（世界平台/轨道+地狱平台/轨道+出生点监狱+直通车、刷怪场、仓库、微光湖）
    private static void OnGamePostInitialize(EventArgs args)
    {
        if (args == null)
        {
            return;
        }

        //太空层
        var sky = Main.worldSurface * 0.3499999940395355;
        //临近地下层
        var surface = Main.worldSurface * 0.7;

        if (Config.Enabled)
        {
            foreach (var cmd in Config.CommandList)
            {
                Commands.HandleCommand(TSPlayer.Server, cmd);
            }

            //监狱
            foreach (var item in Config.Prison)
            {
                GenLargeHouse(Main.spawnTileX + item.spawnTileX, Main.spawnTileY + item.spawnTileY, item.BigHouseWidth, item.BigHouseHeight);
            }


            //箱子
            foreach (var item in Config.Chests)
            {
                SpawnChest(Main.spawnTileX + item.spawnTileX, Main.spawnTileY + item.spawnTileY, item.ClearHeight, item.ChestWidth, item.ChestCount, item.ChestLayers);
            }



            //自建微光湖
            foreach (var item in Config.SpawnShimmerBiome)
            {
                if (item.SpawnShimmerBiome)
                {
                    //天顶、颠倒以地狱为起点往上建微光湖
                    if (Main.zenithWorld || Main.remixWorld || !Main.tenthAnniversaryWorld)
                    {
                        WorldGen.ShimmerMakeBiome(Main.spawnTileX + item.TileX, Main.UnderworldLayer - item.TileY);
                    }

                    //普通地图按出生点为起点往下建
                    else
                    {
                        WorldGen.ShimmerMakeBiome(Main.spawnTileX + item.TileX, Main.spawnTileY + item.TileY); //坐标为出生点
                    }
                }
            }


            //地狱直通车与刷怪场
            foreach (var item in Config.HellTunnel)
            {
                //微光湖直通车 天顶
                if (Main.zenithWorld)
                {
                    ZenithShimmerBiome(item.ShimmerBiomeTunnelWidth, (int) sky - Config.WorldPlatform[0].WorldPlatformY);
                }
                //十周年
                else if (Main.tenthAnniversaryWorld)
                {
                    ZenithShimmerBiome(item.ShimmerBiomeTunnelWidth, Main.spawnTileY + Config.WorldPlatform[0].WorldPlatformY + 1);
                }
                //其他世界
                else
                {
                    ShimmerBiome(Main.spawnTileY + Config.WorldPlatform[0].WorldPlatformY + 1, item.ShimmerBiomeTunnelWidth);
                }

                if (Main.zenithWorld || Main.remixWorld) //是颠倒种子
                {
                    if (Config.HellTunnel[0].HellTrunnelCoverBrushMonst)//直通车是否贯穿刷怪场
                    {
                        //刷怪场 = 地狱层 除一半 补上自定义高度 
                        RockTrialField((Main.rockLayer / 2) + item.BrushMonstHeight, item.BrushMonstHeight, item.BrushMonstWidth, item.BrushMonstCenter);
                        //直通车 = 地狱 - 世界平台高度
                        HellTunnel(Main.spawnTileX + item.SpawnTileX, (int) sky - Config.WorldPlatform[0].WorldPlatformY, item.HellTrunnelWidth);
                    }
                    else //没啥不同就换个先后顺序
                    {
                        HellTunnel(Main.spawnTileX + item.SpawnTileX, (int) sky - Config.WorldPlatform[0].WorldPlatformY, item.HellTrunnelWidth);
                        RockTrialField((Main.rockLayer / 2) + item.BrushMonstHeight, item.BrushMonstHeight, item.BrushMonstWidth, item.BrushMonstCenter);
                    }

                    //左海平台挖到地下层为止 避免失重不好打猪鲨
                    OceanPlatforms((int) surface,
                        Config.WorldPlatform[0].OceanPlatformWide,
                        Config.WorldPlatform[0].OceanPlatformHeight,
                        Config.WorldPlatform[0].OceanPlatformInterval,
                        Config.WorldPlatform[0].OceanPlatformInterval - 1,
                        Config.WorldPlatform[0].OceanPlatformFormSpwanXLimit);
                }
                else //普通世界
                {
                    if (Config.HellTunnel[0].HellTrunnelCoverBrushMonst)
                    {
                        RockTrialField(Main.rockLayer, item.BrushMonstHeight, item.BrushMonstWidth, item.BrushMonstCenter);
                        HellTunnel(Main.spawnTileX + item.SpawnTileX, Main.spawnTileY + item.SpawnTileY, item.HellTrunnelWidth);
                    }
                    else
                    {
                        HellTunnel(Main.spawnTileX + item.SpawnTileX, Main.spawnTileY + item.SpawnTileY, item.HellTrunnelWidth);
                        RockTrialField(Main.rockLayer, item.BrushMonstHeight, item.BrushMonstWidth, item.BrushMonstCenter);
                    }

                    //不超过海平面就行
                    OceanPlatforms((int) sky,
                        Config.WorldPlatform[0].OceanPlatformWide,
                        Config.WorldPlatform[0].OceanPlatformHeight,
                        Config.WorldPlatform[0].OceanPlatformInterval,
                        Config.WorldPlatform[0].OceanPlatformInterval - 1,
                        Config.WorldPlatform[0].OceanPlatformFormSpwanXLimit);
                }
                //地狱平台
                UnderworldPlatform(Main.UnderworldLayer + item.HellPlatformY, item.HellPlatformY);
            }

            //世界平台
            foreach (var item in Config.WorldPlatform)
            {
                if (Main.zenithWorld || Main.remixWorld) //天顶 颠倒以天空层往下算
                {
                    WorldPlatform((int) sky - item.WorldPlatformY, item.WorldPlatformClearY);
                }
                else //正常世界从出生点往上算
                {
                    WorldPlatform(Main.spawnTileY + item.WorldPlatformY, item.WorldPlatformClearY);
                }
            }

            TShock.Utils.Broadcast(
                GetString("基础建设已完成，如需重置布局\n") +
                GetString("请输入指令后重启服务器：/rm reset"), 250, 247, 105);

            Commands.HandleCommand(TSPlayer.Server, "/save");
            Commands.HandleCommand(TSPlayer.Server, "/clear i 9999");
            Configuration.Read();
            Config.Enabled = false;
            Config.Write();
        }
    }
    #endregion

    #region 提高刷怪场中的刷怪率方法
    private void PlayerUpdate(object? sender, GetDataHandlers.PlayerUpdateEventArgs e)
    {
        if (e == null || !Config.Enabled || !Config.HellTunnel[0].SpawnRate ||
            !Config.HellTunnel[0].BrushMonstEnabled || !Main.hardMode)
        {
            return;
        }

        if (Main.zenithWorld || Main.remixWorld)//颠倒种子
        {
            MonsterRegion((Main.rockLayer / 2) + Config.HellTunnel[0].BrushMonstHeight,
                Config.HellTunnel[0].BrushMonstHeight, Config.HellTunnel[0].BrushMonstWidth, Config.HellTunnel[0].BrushMonstCenter);
        }
        else //普通世界
        {
            MonsterRegion(Main.rockLayer, Config.HellTunnel[0].BrushMonstHeight,
                Config.HellTunnel[0].BrushMonstWidth, Config.HellTunnel[0].BrushMonstCenter);
        }
    }

    private static void MonsterRegion(double posY, int height, int width, int centerVal)
    {
        if (InRegion(posY, height, width, centerVal))
        {
            NPC.defaultSpawnRate = Config.HellTunnel[0].defaultSpawnRate;
            NPC.defaultMaxSpawns = Config.HellTunnel[0].defaultMaxSpawns;
        }
        else
        {
            NPC.defaultSpawnRate = TShock.Config.Settings.DefaultSpawnRate;
            NPC.defaultMaxSpawns = TShock.Config.Settings.DefaultMaximumSpawns;
        }
    }
    #endregion

    #region 判断所有玩家在刷怪区方法
    public static bool InRegion(double posY, int height, int width, int centerVal)
    {
        var region = (int) posY - height;
        var top = region + (height * 2);
        var bottom = (int) posY + (height * 2);
        var middle = ((top + bottom) / 2) + centerVal;

        var centerTop = middle + 8 + centerVal - 11;
        var centerBottom = middle + 8 + centerVal + 3;
        var centerLeft = Main.spawnTileX - 8 - centerVal;
        var centerRight = Main.spawnTileX + 8 + centerVal;

        for (var i = 0; i < Main.maxPlayers; i++) //所有在线玩家
        {
            var plr = Main.player[i];
            if (plr.active)
            {
                var plrX = (int) (plr.position.X / 16);
                var plrY = (int) (plr.position.Y / 16);
                if (!(plrX >= centerLeft && plrX <= centerRight
                    && plrY >= centerTop && plrY <= centerBottom))
                {
                    return false;
                }
            }
        }
        return true;
    }
    #endregion

    #region 刷怪场
    private static void RockTrialField(double posY, int Height, int Width, int CenterVal)
    {
        var clear = (int) posY - Height;

        // 计算顶部、底部和中间位置
        var top = clear + (Height * 2);
        var bottom = (int) posY + (Height * 2);
        var middle = ((top + bottom) / 2) + CenterVal;

        var left = Math.Max(Main.spawnTileX - Width, 0);
        var right = Math.Min(Main.spawnTileX + Width, Main.maxTilesX);

        var CenterLeft = Main.spawnTileX - 8 - CenterVal;
        var CenterRight = Main.spawnTileX + 8 + CenterVal;

        if (Config.HellTunnel[0].BrushMonstEnabled && !Config.HellTunnel[0].ClearRegionEnabled)
        {
            for (var y = (int) posY; y > clear; y--)
            {
                for (var x = left; x < right; x++)
                {
                    Main.tile[x, y + (Height * 2)].ClearEverything(); // 清除方块

                    WorldGen.PlaceTile(x, top, Config.HellTunnel[0].Hell_BM_TileID, false, true, -1, 0); // 在清理顶部放1层（防液体流进刷怪场）

                    WorldGen.PlaceTile(x, bottom, Config.HellTunnel[0].Hell_BM_TileID, false, true, -1, 0); //刷怪场底部放1层

                    //如果不是天顶 或 开启直通车贯穿刷怪场 则放岩浆在底部禁止刷怪
                    if ((!Main.zenithWorld || !Main.remixWorld) && Config.HellTunnel[0].HellTrunnelCoverBrushMonst && Config.HellTunnel[0].Lava)
                    {
                        WorldGen.PlaceLiquid(x, bottom - 1, 1, 1); //岩浆高于底部一格
                    }

                    WorldGen.PlaceTile(x, middle, Config.HellTunnel[0].Hell_BM_TileID, false, true, -1, 0); //刷怪场中间放1层（刷怪用）

                    WorldGen.PlaceTile(x, middle + 8 + CenterVal, Config.HellTunnel[0].PlatformID, false, true, -1, Config.HellTunnel[0].PlatformStyle);//中间下8格放一层方便站人

                    for (var wallY = middle + 3; wallY <= middle + 7 + CenterVal; wallY++)
                    {
                        Main.tile[x, wallY].wall = 155; // 放置墙壁
                    }

                    //定刷怪区
                    for (var i = 1; i <= 3; i++)
                    {
                        for (var j = 61; j <= 83; j++)
                        {
                            WorldGen.PlaceWall(CenterLeft - j + 8 + CenterVal, middle + i, 164, false); // 左 62 - 84格刷怪区 放红玉晶莹墙警示
                            WorldGen.PlaceWall(CenterRight + j - 8 - CenterVal, middle + i, 164, false); // 右 62 - 84格刷怪区 放红玉晶莹墙警示
                        }
                    }
                    if (Main.tile[x, y].wall != Terraria.ID.WallID.None)
                    {
                        continue;
                    }
                    WorldGen.PlaceTile(x, middle + 11 + CenterVal, Config.HellTunnel[0].Hell_BM_TileID, false, true, -1, 0); //中间下11格放箱子的实体块

                    WorldGen.PlaceTile(x, middle + 10 + CenterVal, Config.Chests[0].ChestTileID, false, true, -1, Config.Chests[0].ChestStyle); //中间下10格放箱子

                    WorldGen.PlaceTile(x, middle + 2, Config.HellTunnel[0].Hell_BM_TileID, false, true, -1, 0); //放计时器的平台

                    WorldGen.PlaceTile(x, middle + 4, Config.HellTunnel[0].Hell_BM_TileID, false, true, -1, 0); //防掉怪下来

                    // 如果x值在中心范围内，放置10格高的方块
                    if (x >= CenterLeft && x <= CenterRight)
                    {
                        for (var wallY = middle - 10 - CenterVal; wallY <= middle - 1; wallY++)
                        {
                            // 创建矩形判断
                            if (wallY >= middle - 10 - CenterVal && wallY <= middle - 1 && x >= CenterLeft + 1 && x <= CenterRight - 1)
                            {
                                // 挖空方块
                                Main.tile[x, wallY].ClearEverything();
                            }
                            else
                            {
                                // 在矩形范围外放置方块
                                WorldGen.PlaceTile(x, wallY, Config.HellTunnel[0].Hell_BM_TileID, false, true, -1, 0);

                                //放一层半砖平台让怪穿进中心区
                                WorldGen.PlaceTile(CenterLeft - 1, wallY, 19, false, true, -1, 43);
                                Main.tile[CenterLeft - 1, wallY].slope(2); // 设置为右斜坡
                                WorldGen.PlaceTile(CenterRight + 1, wallY, 19, false, true, -1, 43);
                                Main.tile[CenterRight + 1, wallY].slope(1); // 设置为左斜坡
                            }

                            // 检查是否在中间位置，如果是则放置岩浆
                            if (wallY == middle - 10 - CenterVal)
                            {
                                //是否放岩浆
                                if (Config.HellTunnel[0].Lava)
                                {
                                    Main.tile[x - 1, wallY + 2].liquid = 60;  //设置1格液体
                                    Main.tile[x - 1, wallY + 2].liquidType(1); // 设置为岩浆
                                    WorldGen.SquareTileFrame(x - 1, wallY + 2, false);
                                }

                                //是否放尖球
                                if (Config.HellTunnel[0].Trap)
                                {
                                    //加一排尖球
                                    for (var j = 1; j <= 5 + CenterVal; j++)
                                    {
                                        //电线
                                        WorldGen.PlaceWire(x - j, wallY - j);
                                        WorldGen.PlaceWire(x + j, wallY - j);
                                        //尖球
                                        WorldGen.PlaceTile(x - j, wallY - j, 137, true, false, -1, 3);
                                        WorldGen.PlaceTile(x + j, wallY - j, 137, true, false, -1, 3);
                                        //给尖球虚化
                                        Main.tile[x - j, wallY - j].inActive(true);
                                        Main.tile[x + j, wallY - j].inActive(true);
                                    }
                                    //给尖球加电线
                                    for (var j = 2; j <= 10 + CenterVal; j++)
                                    {
                                        WorldGen.PlaceWire(CenterLeft - 1, middle - j);
                                        WorldGen.PlaceWire(CenterRight + 1, middle - j);
                                    }
                                }
                            }
                        }
                    }
                    else //不在中心 左右生成半砖推怪平台
                    {
                        Main.tile[x, middle - 1].type = Config.HellTunnel[0].Hell_BM_TileID;
                        Main.tile[x, middle - 1].active(true);
                        Main.tile[x, middle - 1].halfBrick(false);

                        // 根据x值确定斜坡方向
                        if (x < Main.spawnTileX)
                        {
                            Main.tile[x, middle - 1].slope(3); // 设置为右斜坡
                            WorldGen.PlaceTile(x, middle, 421, false, true, -1, 0); //刷怪场中间左边放1层传送带（刷怪取物品用）
                        }
                        else
                        {
                            Main.tile[x, middle - 1].slope(4); // 设置为左斜坡
                            WorldGen.PlaceTile(x, middle, 422, false, true, -1, 0); //刷怪场中间右边放1层传送带（刷怪取物品用） 
                        }

                        // 把半砖替换成推怪平台
                        WorldGen.PlaceTile(x, middle - 1, Config.HellTunnel[0].PlatformID, false, true, -1, Config.HellTunnel[0].PlatformStyle);
                        //平台加电线+制动器
                        WorldGen.PlaceWire(x, middle - 1);
                        WorldGen.PlaceActuator(x, middle - 1);

                        // 在电线的末端放置1/4秒计时器并连接
                        WorldGen.PlaceWire(CenterLeft - 1, middle + 1);
                        WorldGen.PlaceWire(CenterRight + 1, middle + 1);
                        WorldGen.PlaceWire(CenterLeft - 1, middle);
                        WorldGen.PlaceWire(CenterRight + 1, middle);
                        WorldGen.PlaceTile(CenterLeft - 1, middle + 1, 144, false, true, -1, 4);
                        WorldGen.PlaceTile(CenterRight + 1, middle + 1, 144, false, true, -1, 4);

                        //在1/4秒计时器下面连接开关
                        for (var i = 2; i <= 5; i++)
                        {
                            WorldGen.PlaceWire(CenterLeft - 1, middle + i);
                            WorldGen.PlaceWire(CenterRight + 1, middle + i);
                            WorldGen.PlaceTile(CenterLeft - 1, middle + 5, 136, false, true, -1, 0);
                            WorldGen.PlaceTile(CenterRight + 1, middle + 5, 136, false, true, -1, 0);
                        }

                        //左边 放篝火、影烛、水蜡烛、花园侏儒、心灯、巴斯特雕像
                        WorldGen.PlaceObject(CenterLeft - 2, middle + 7 + CenterVal, 215, false, 0, 0, -1, -1);
                        WorldGen.PlaceObject(CenterLeft - 4, middle + 7 + CenterVal, 646, false, 0, 0, -1, -1);
                        WorldGen.PlaceObject(CenterLeft - 5, middle + 7 + CenterVal, 49, false, 0, 0, -1, -1);
                        WorldGen.PlaceGnome(CenterLeft - 6, middle + 7 + CenterVal, 0);
                        WorldGen.PlaceObject(CenterLeft - 6, middle + 3 + CenterVal, 42, false, 9, 0, -1, -1);
                        WorldGen.PlaceObject(CenterLeft - 8, middle + 7 + CenterVal, 506, false, 0, 0, -1, -1);

                        //右边 放环境火把
                        for (var j = 5; j <= 23; j++)
                        {
                            WorldGen.PlaceObject(CenterRight + j, middle + 3 + CenterVal, 4, false, j, -1, -1);
                        }

                        //是否放飞镖
                        if (Config.HellTunnel[0].Dart)
                        {
                            //每30格 放一个 一共6个
                            for (var k = 1; k <= 6; k++)
                            {
                                //给电线从中心y0拉线到下位3
                                for (var l = 0; l <= 3; l++)
                                {
                                    //输出1 3 忽略偶数（刷怪层为0 ，下1和3格放飞镖）
                                    for (var m = 1; m <= 3; m += 2)
                                    {
                                        //右飞镖
                                        WorldGen.PlaceTile(CenterRight + (30 * k), middle + m, 137, false, true, -1, 5);
                                        //电线
                                        WorldGen.PlaceWire(CenterRight + (30 * k), middle + l);
                                        //把飞镖机关虚化
                                        Main.tile[CenterRight + (30 * k), middle + m].inActive(true);

                                        //左飞镖
                                        WorldGen.PlaceTile(CenterLeft - (30 * k), middle + m, 137, false, true, -1, 5);
                                        //给飞镖换个方向
                                        var obj = Main.tile[CenterLeft - (30 * k), middle + m];
                                        obj.frameX = 18;
                                        //放电线
                                        WorldGen.PlaceWire(CenterLeft - (30 * k), middle + l);
                                        //把飞镖机关虚化
                                        Main.tile[CenterLeft - (30 * k), middle + m].inActive(true);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // 左右各放一列把刷怪场封闭起来
            for (var y2 = top; y2 < bottom + 1; y2++)
            {
                WorldGen.PlaceTile(left - 1, y2, Config.HellTunnel[0].Hell_BM_TileID, false, true, -1, 0);
                WorldGen.PlaceTile(right, y2, Config.HellTunnel[0].Hell_BM_TileID, false, true, -1, 0);

                //是否放置飞镖
                if (Config.HellTunnel[0].Dart)
                {
                    // 避免核心区放置边界飞镖
                    var placeTop = middle + 8 + CenterVal - 11; // 平台上方11格开始
                    var placeBottom = middle + 8 + CenterVal + 3; // 平台下方3格结束
                    if (y2 < placeTop || y2 > placeBottom)
                    {
                        //右飞镖
                        WorldGen.PlaceTile(right - 1, y2, 137, false, true, -1, 5);
                        //左飞镖
                        WorldGen.PlaceTile(left, y2, 137, false, true, -1, 5);
                        //给左飞镖换朝向
                        var obj = Main.tile[left, y2];
                        obj.frameX = 18;
                        //把飞镖机关虚化
                        Main.tile[right - 1, y2].inActive(true);
                        Main.tile[left, y2].inActive(true);
                    }
                    //放电线
                    WorldGen.PlaceWire(right - 1, y2);
                    WorldGen.PlaceWire(left, y2);
                }
            }
        }
        //只清场地不建刷怪场 帮定中心点与刷怪范围
        else
        {
            for (var y = (int) posY; y > clear; y--)
            {
                for (var x = left; x < right; x++)
                {
                    Main.tile[x, y + (Height * 2)].ClearEverything(); // 清除方块

                    WorldGen.PlaceTile(x, top, Config.HellTunnel[0].Hell_BM_TileID, false, true, -1, 0); // 在清理顶部放1层（防液体流进刷怪场）

                    WorldGen.PlaceTile(x, bottom, Config.HellTunnel[0].Hell_BM_TileID, false, true, -1, 0); //刷怪场底部放1层

                    //如果不是天顶 或 开启直通车贯穿刷怪场 则放岩浆禁止刷怪
                    if ((!Main.zenithWorld || !Main.remixWorld) && Config.HellTunnel[0].HellTrunnelCoverBrushMonst && Config.HellTunnel[0].Lava)
                    {
                        WorldGen.PlaceLiquid(x, bottom - 1, 1, 1);  //岩浆高于底部一格
                    }

                    //定中心
                    WorldGen.PlaceTile(CenterLeft, middle, 267, false, true, -1, 0);
                    WorldGen.PlaceTile(CenterRight, middle, 267, false, true, -1, 0);

                    //定刷怪区
                    for (var i = 1; i <= 3; i++)
                    {
                        for (var j = 61; j <= 83; j++)
                        {
                            WorldGen.PlaceWall(CenterLeft - 60 + 8 + CenterVal, middle + i, 155, false); // 左 61格 放钻石晶莹墙
                            WorldGen.PlaceWall(CenterLeft - j + 8 + CenterVal, middle + i, 164, false); // 左 62 - 84格刷怪区 放红玉晶莹墙警示
                            WorldGen.PlaceWall(CenterLeft - 84 + 8 + CenterVal, middle + i, 155, false); // 左 85格 放钻石晶莹墙

                            WorldGen.PlaceWall(CenterRight + 60 - 8 - CenterVal, middle + i, 155, false);  //  右 61格 放钻石晶莹墙
                            WorldGen.PlaceWall(CenterRight + j - 8 - CenterVal, middle + i, 164, false); // 右 62 - 84格刷怪区 放红玉晶莹墙警示
                            WorldGen.PlaceWall(CenterRight + 84 - 8 - CenterVal, middle + i, 155, false); // 右 85格 放钻石晶莹墙
                        }
                    }
                }
            }

            // 左右各放一列把刷怪场封闭起来
            for (var y2 = top; y2 < bottom + 1; y2++)
            {
                WorldGen.PlaceTile(left - 1, y2, Config.HellTunnel[0].Hell_BM_TileID, false, true, -1, 0);
                WorldGen.PlaceTile(right, y2, Config.HellTunnel[0].Hell_BM_TileID, false, true, -1, 0);
            }
        }
    }
    #endregion

    #region 构造监狱集群方法
    public static void GenLargeHouse(int startX, int startY, int width, int height)
    {
        if (!Config.Prison[0].BigHouseEnabled)
        {
            return;
        }

        var roomsAcross = width / 5;
        var roomsDown = height / 9;

        for (var row = 0; row < roomsDown; row++)
        {
            for (var col = 0; col < roomsAcross; col++)
            {
                var isRight = col == roomsAcross;
                GenRoom(startX + (col * 5), startY + (row * 9), isRight);
            }
        }
    }
    #endregion

    #region 创建小房间方法
    public static void GenRoom(int posX, int posY, bool isRight = true)
    {
        var roomTheme = new RoomTheme();
        roomTheme.SetGlass();
        var tile = roomTheme.tile;
        var wall = roomTheme.wall;
        var platform = roomTheme.platform;
        var chair = roomTheme.chair;
        var bench = roomTheme.bench;
        var torch = roomTheme.torch;
        var num = posX;
        var num2 = 6;
        var num3 = 10;
        if (!isRight)
        {
            num += 6;
        }
        for (var i = num; i < num + num2; i++)
        {
            for (var j = posY - num3; j < posY; j++)
            {
                var val = Main.tile[i, j];
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
            var obj = Main.tile[num + 1, posY - 6];
            obj.frameX += 18;
            var obj2 = Main.tile[num + 1, posY - 7];
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
        var secondLast = Utils.GetUnixTimestamp;
        return Task.Run(delegate
        {
            var num = 5;
            var num2 = 1 + (total * num);
            var num3 = needCenter ? posX - (num2 / 2) : posX;
            for (var i = 0; i < total; i++)
            {
                GenRoom(num3, posY, isRight);
                num3 += num;
            }
        }).ContinueWith(delegate
        {
            TileHelper.InformPlayers();
            var value = Utils.GetUnixTimestamp - secondLast;
            plr.SendSuccessMessage(GetString($"已生成{total}个小房子，用时{value}秒。"));
        });
    }

    #endregion

    #region 箱子集群方法
    private static void SpawnChest(int posX, int posY, int hight, int width, int count, int layers)
    {
        if (!Config.Chests[0].SpawnChestEnabled)
        {
            return;
        }

        var ClearHeight = hight + ((layers - 1) * (width + Config.Chests[0].LayerHeight));

        for (var x = posX; x < posX + (width * count); x++)
        {
            for (var y = posY - ClearHeight; y <= posY; y++)
            {
                Main.tile[x, y].ClearEverything();
            }
        }

        for (var layer = 0; layer < layers; layer++)
        {
            for (var i = 0; i < count; i++)
            {
                var currentXPos = posX + (i * width);
                var currentYPos = posY - (layer * (width + Config.Chests[0].LayerHeight));

                for (var wx = currentXPos; wx < currentXPos + width; wx++)
                {
                    WorldGen.PlaceTile(wx, currentYPos + 1, Config.Chests[0].ChestPlatformID, false, true, -1, Config.Chests[0].ChestPlatformStyle);
                }

                WorldGen.PlaceTile(currentXPos, currentYPos, Config.Chests[0].ChestTileID, false, true, -1, Config.Chests[0].ChestStyle);
            }
        }
    }
    #endregion

    #region 左海平台
    private static void OceanPlatforms(int sky, int wide, int height, int interval, int IntlClear, int Radius)
    {
        if (!Config.WorldPlatform[0].OceanPlatformEnabled)
        {
            return;
        }

        var clear = Math.Max(0, height);

        for (var y = Main.oceanBG; y < sky + clear; y += interval)
        {
            for (var top = y - IntlClear; top < y; top++)
            {
                for (var x = 0; x < wide; x++)
                {
                    // 停止放置平台的条件
                    if (x >= Main.maxTilesX || x < 0 || y < sky)
                    {
                        continue;
                    }

                    if (Math.Abs(x - Main.spawnTileX) <= Radius)
                    {
                        continue;
                    }

                    // 清理方块和墙（保留液体方法）
                    WorldGen.KillTile(x, y);
                    WorldGen.KillWall(x, y);
                    WorldGen.KillTile(x, top);
                    WorldGen.KillWall(x, top);

                    // 正常放置平台
                    WorldGen.PlaceTile(x, y, Config.WorldPlatform[0].WorldPlatformID, false, true, -1, Config.WorldPlatform[0].WorldPlatformStyle);

                    //是否让平台生成液体（怕海水太多把平台阉了）
                    if (Config.WorldPlatform[0].PlaceLiquid)
                    {
                        //从每层平台的位置生成水 避免原来的海水下落体积过大游戏引擎过载（液体碰撞就会流动了）
                        WorldGen.PlaceLiquid(x, y, 0, 255);
                    }

                    // 开启实体块包围左海平台
                    if (Config.WorldPlatform[0].Enclose)
                    {
                        // 清理顶部上面一层
                        for (var j = 1; j < clear; j++)
                        {
                            if (sky - clear + height - j >= 0 && sky - clear + height - j < Main.maxTilesY)
                            {
                                Main.tile[x, sky - clear + height - j].ClearTile();
                            }
                        }

                        // 放置左海平台顶部格栅
                        if (sky - clear + height >= 0 && sky - clear + height < Main.maxTilesY)
                        {
                            WorldGen.PlaceTile(x, sky - clear + height, 546, false, false, -1, 0);
                        }

                        // 清理底部上面一层
                        for (var j = 1; j < IntlClear; j++)
                        {
                            if (sky + clear - j >= 0 && sky + clear - j < Main.maxTilesY)
                            {
                                Main.tile[x, sky + clear - j].ClearEverything();
                            }
                        }

                        // 放置左海平台底部
                        if (sky + clear >= 0 && sky + clear < Main.maxTilesY)
                        {
                            WorldGen.PlaceTile(x, sky + clear, 38, false, true, -1, 0);
                        }

                        // 放置左边一列特定方块
                        var leftX = 0;
                        var leftY = x + y - IntlClear;
                        if (leftY >= 0 && leftY < Main.maxTilesY)
                        {
                            WorldGen.PlaceTile(leftX, leftY, Config.HellTunnel[0].Hell_BM_TileID, false, true, -1, 0);
                        }
                    }
                }
            }
        }
    }
    #endregion

    #region 世界平台
    private static void WorldPlatform(int posY, int hight)
    {
        var clear = Math.Max(3, hight);

        for (var x = 0; x < Main.maxTilesX; x++)
        {
            if (Config.WorldPlatform[0].OceanPlatformEnabled)
            {
                if (x - clear <= Main.oceanBG + Config.WorldPlatform[0].WorldPlatformFromOceanLimit)
                {
                    continue;
                }
            }

            for (var y = posY - clear; y <= posY; y++)
            {
                if (x < 0 || x >= Main.maxTilesX || y < 0 || y >= Main.maxTilesY)
                {
                    continue;
                }

                Main.tile[x, y].ClearEverything();
            }

            if (Config.WorldPlatform[0].WorldPlatformEnabled)
            {
                WorldGen.PlaceTile(x, posY, Config.WorldPlatform[0].WorldPlatformID, false, true, -1, Config.WorldPlatform[0].WorldPlatformStyle);
            }

            if (Config.WorldPlatform[0].WorldTrackEnabled)
            {
                WorldGen.PlaceTile(x, posY - 1, 314, false, true, -1, 0);
            }
        }
    }
    #endregion

    #region 地狱直通车
    private static int HellTunnel(int posX, int posY, int Width)
    {
        var hell = 0;

        if (Config.HellTunnel[0].HellTunnelEnabled)
        {
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
            var num = hell;
            var Xstart = posX - 2;
            Parallel.For(Xstart, Xstart + Width, delegate (int cx)
            {
                Parallel.For(posY, hell, delegate (int cy)
                {
                    var val = Main.tile[cx, cy];
                    val.ClearEverything();
                    if (cx == Xstart + (Width / 2))
                    {
                        val.type = Config.HellTunnel[0].Cord_TileID; //绳子
                        val.active(true);
                        val.slope(0);
                        val.halfBrick(false);
                    }
                    else if (cx == Xstart || cx == Xstart + Width - 1)
                    {
                        val.type = Config.HellTunnel[0].Hell_BM_TileID; //边界方块
                        val.active(true);
                        val.slope(0);
                        val.halfBrick(false);
                    }
                });
            });

            var platformStart = Xstart + 1;
            var platformEnd = Xstart + Width - 2;
            //确保平台与直通车等宽
            for (var px = platformStart; px <= platformEnd; px++)
            {
                WorldGen.PlaceTile(px, posY, Config.HellTunnel[0].PlatformID, false, true, -1, Config.HellTunnel[0].PlatformStyle);
                for (var cy = posY + 1; cy <= hell; cy++)
                {
                    Main.tile[px, cy].wall = 155; // 放置墙壁
                }
            }
        }
        return hell;
    }
    #endregion

    #region 地狱平台
    private static void UnderworldPlatform(int posY, int hight)
    {
        var Clear = posY - hight;
        for (var y = posY; y > Clear; y--)
        {
            for (var x = 0; x < Main.maxTilesX; x++)
            {
                Main.tile[x, y].ClearEverything(); // 清除方块

                if (Config.HellTunnel[0].HellPlatformEnabled)
                {
                    WorldGen.PlaceTile(x, posY, Config.HellTunnel[0].PlatformID, false, true, -1, Config.HellTunnel[0].PlatformStyle); //地狱平台
                }

                if (Config.HellTunnel[0].HellTrackEnabled)
                {
                    WorldGen.PlaceTile(x, posY - 1, 314, false, true, -1, 0); //地狱轨道
                }
            }
        }
    }
    #endregion

    #region 普通世界微光湖直通车
    private static void ShimmerBiome(int spawnY, int Width)
    {
        //开启出生点微光湖则返回
        if (!Config.HellTunnel[0].ShimmerBiomeTunnelEnabled || Config.SpawnShimmerBiome[0].SpawnShimmerBiome)
        {
            return;
        }

        //西江的判断微光湖位置方法
        var skipTile = new bool[Main.maxTilesX, Main.maxTilesY];
        for (var x = 0; x < Main.maxTilesX; x++)
        {
            for (var y = 0; y < Main.maxTilesY; y++)
            {
                var tile = Main.tile[x, y];
                if (tile is null || skipTile[x, y])
                {
                    continue;
                }

                if (tile.shimmer())
                {
                    var Right = x;
                    var Bottom = y;
                    for (var right = x; right < Main.maxTilesX; right++)
                    {
                        if (!Main.tile[right, y].shimmer())
                        {
                            Right = right;
                            break;
                        }
                    }

                    for (var right = x; right < Right; right++)
                    {
                        for (var bottom = y; bottom < Main.maxTilesY; bottom++)
                        {
                            if (!Main.tile[right, bottom].shimmer())
                            {
                                if (bottom > Bottom)
                                {
                                    Bottom = bottom;
                                }
                                break;
                            }
                        }
                    }

                    for (var start = x - 2; start < Right + 2; start++)
                    {
                        for (var end = y; end < Bottom + 2; end++)
                        {
                            skipTile[start, end] = true;
                        }
                    }

                    #region 微光湖直通车
                    // 找到微光湖的中心点
                    var CenterX = (x + Right) / 2;
                    //深度到为中心湖面
                    var CenterY = ((y + Bottom) / 2) - 8;

                    // 从微光湖中心点向上挖通道直至地表
                    for (var TunnelY = CenterY; TunnelY >= spawnY; TunnelY--)
                    {
                        for (var TunnelX = CenterX - Width; TunnelX <= CenterX + Width; TunnelX++)
                        {
                            if (TunnelX >= 0 && TunnelX < Main.maxTilesX)
                            {
                                Main.tile[TunnelX, TunnelY].ClearEverything();
                                //直通车的两侧的方块
                                if (TunnelX == CenterX - Width || TunnelX == CenterX + Width)
                                {
                                    WorldGen.PlaceTile(TunnelX, TunnelY, Config.HellTunnel[0].Hell_BM_TileID, false, true, -1, 0);
                                }

                                //微光湖底部放一层雨云 防摔
                                else if (TunnelY == CenterY)
                                {
                                    WorldGen.PlaceTile(TunnelX, TunnelY, 460, false, true, -1, 0);
                                }
                            }
                        }
                    }
                    #endregion
                }
            }
        }
    }
    #endregion

    #region 天顶微光直通车
    private static readonly int MaxTunnels = 1; // 全局最大隧道数
    private static int CurrentTunnels = 0; // 当前已创建的隧道数
    private static void ZenithShimmerBiome(int Width, int Height)
    {
        //开启出生点微光湖则返回
        if (!Config.HellTunnel[0].ShimmerBiomeTunnelEnabled || CurrentTunnels >= MaxTunnels || Config.SpawnShimmerBiome[0].SpawnShimmerBiome)
        {
            return;
        }

        const int MinLakeSize = 200; // 只为大于等于200个瓷砖的微光湖创建隧道
        var skipTile = new bool[Main.maxTilesX, Main.maxTilesY];
        var ConedLakes = new List<Tuple<int, int>>();
        var labelMap = new int[Main.maxTilesX * Main.maxTilesY]; // 用于标记连通组件

        // 使用连通组件分析检测微光湖
        for (var x = 0; x < Main.maxTilesX; x++)
        {
            for (var y = 0; y < Main.maxTilesY; y++)
            {
                if (Main.tile[x, y]?.shimmer() == true && !skipTile[x, y])
                {
                    // 发现新的微光湖，开始标记
                    var label = ConedLakes.Count + 1;
                    FloodFill(x, y, label, ref labelMap, ref skipTile);
                    ConedLakes.Add(Tuple.Create(x, y));
                }
            }
        }

        // 从连通组件中选择最大的微光湖
        Tuple<int, int> SCenter = null!;
        foreach (var lake in ConedLakes)
        {
            var label = ConedLakes.IndexOf(lake) + 1;
            var size = CountSize(labelMap, label);
            if (size >= MinLakeSize)
            {
                // 更新选定的微光湖中心点
                if (SCenter == null || size > CountSize(labelMap, ConedLakes.FindIndex(l => l.Equals(SCenter)) + 1))
                {
                    SCenter = lake;
                }
            }
        }

        // 创建隧道
        if (SCenter != null)
        {
            CreateTunnel(SCenter.Item1, SCenter.Item2, Width, Height);
            CurrentTunnels++;
        }
    }



    //大量填充，根据邻近点合并，统计连通区域的大小（主打一个扫雷）
    private static void FloodFill(int x, int y, int label, ref int[] labelMap, ref bool[,] skipTile)
    {
        if (x < 0 || x >= Main.maxTilesX || y < 0 || y >= Main.maxTilesY || !Main.tile[x, y].shimmer() || skipTile[x, y])
        {
            return;
        }

        var index = (y * Main.maxTilesX) + x;
        labelMap[index] = label;
        skipTile[x, y] = true;

        FloodFill(x + 1, y, label, ref labelMap, ref skipTile);
        FloodFill(x - 1, y, label, ref labelMap, ref skipTile);
        FloodFill(x, y + 1, label, ref labelMap, ref skipTile);
        FloodFill(x, y - 1, label, ref labelMap, ref skipTile);
    }

    //计数大小
    private static int CountSize(int[] labelMap, int label)
    {
        var count = 0;
        for (var i = 0; i < labelMap.Length; i++)
        {
            if (labelMap[i] == label)
            {
                count++;
            }
        }
        return count;
    }

    //创建隧道
    private static void CreateTunnel(int CenterX, int CenterY, int Width, int Height)
    {
        var CenterYOffset = -8; // 调整到微光湖中心偏上的位置

        // 开始挖掘隧道
        for (var y = CenterY + CenterYOffset; y >= Height; y--)
        {
            for (var x = CenterX - Width; x <= CenterX + Width; x++)
            {
                if (x >= 0 && x < Main.maxTilesX)
                {
                    Main.tile[x, y].ClearEverything();
                    if (x == CenterX - Width || x == CenterX + Width)
                    {
                        WorldGen.PlaceTile(x, y, Config.HellTunnel[0].Hell_BM_TileID, false, true, -1, 0); // 两侧放置特殊方块
                    }
                    else if (y == CenterY + CenterYOffset)
                    {
                        WorldGen.PlaceTile(x, y, 460, false, true, -1, 0); // 底部放置雨云
                    }
                }
            }
        }
    }
    #endregion

}