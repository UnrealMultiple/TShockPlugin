using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Utilities;
using TShockAPI;

namespace WorldModify
{
    internal class iGen
    {
        private static UnifiedRandom genRand = WorldGen.genRand;

        public static async void Manage(CommandArgs args)
        {
            TSPlayer op = args.Player;
            if (args.Parameters.Count == 0)
            {
                Help();
                return;
            }
            int num;
            switch (args.Parameters[0].ToLowerInvariant())
            {
                case "help":
                    Help();
                    break;
                default:
                    op.SendErrorMessage("语法错误，请输入 /igen help 查询帮助");
                    break;
                case "world":
                case "w":
                    WorldTool.Manage(args);
                    break;
                case "info":
                    {
                        if (NeedInGame())
                        {
                            break;
                        }
                        int cx = op.TileX;
                        int cy = op.TileY + 3;
                        ITile tile = Main.tile[cx, cy];
                        List<string> li = new List<string>
                        {
                            "脚下一格的信息：",
                            $"坐标：{cx},{cy}({Utils.GetLocationDesc(cx, cy)})({op.TPlayer.position.X},{op.TPlayer.position.Y})",
                            $"图格：{tile.type}",
                            $"frameXY：{tile.frameX},{tile.frameY}",
                            $"blockType：{tile.blockType()}",
                            $"斜面：{tile.slope()}"
                        };
                        List<string> li2 = new List<string>();
                        if (tile.wall > 0)
                        {
                            WallProp wp = ResHelper.GetWallByID(tile.wall);
                            string wallDesc = wp != null ? wp.Desc : tile.wall.ToString();
                            li2.Add(wallDesc);
                        }
                        if (tile.liquid > 0)
                        {
                            if (tile.lava())
                            {
                                li2.Add("岩浆");
                            }
                            else if (tile.honey())
                            {
                                li2.Add("蜂蜜");
                            }
                            else if (tile.shimmer())
                            {
                                li2.Add("微光");
                            }
                            else if (tile.liquidType() == 0)
                            {
                                li2.Add("水");
                            }
                        }
                        if (tile.wire())
                        {
                            li2.Add("红电线");
                        }
                        if (tile.wire2())
                        {
                            li2.Add("蓝电线");
                        }
                        if (tile.wire3())
                        {
                            li2.Add("绿电线");
                        }
                        if (tile.wire4())
                        {
                            li2.Add("黄电线");
                        }
                        if (li2.Any())
                        {
                            li.Add("其它：" + string.Join(", ", li2));
                        }
                        op.SendInfoMessage(string.Join("\n", li));
                        break;
                    }
                case "room":
                    {
                        if (NeedInGame() || NeedWaitTask())
                        {
                            break;
                        }
                        int total = 3;
                        if (args.Parameters.Count > 1)
                        {
                            if (!int.TryParse(args.Parameters[1], out total))
                            {
                                op.SendErrorMessage("输入的房间数量不对");
                                break;
                            }
                            if (total < 1 || total > 1000)
                            {
                                total = 3;
                            }
                        }
                        await AsyncGenRoom(isRight: op.TPlayer.direction != -1, op: op, posX: op.TileX, posY: op.TileY + 3, total: total, needCenter: true);
                        break;
                    }
                case "hotel":
                    if (!NeedInGame() && !NeedWaitTask())
                    {
                        await AsyncGenHotel(isRight: op.TPlayer.direction != -1, total: Math.Max(3, NPCHelper.GetRelive().Count), op: op, posX: op.TileX, posY: op.TileY + 3, needCenter: true);
                    }
                    break;
                case "pond":
                    {
                        if (NeedInGame() || NeedWaitTask())
                        {
                            break;
                        }
                        int type = 0;
                        if (args.Parameters.Count > 1)
                        {
                            switch (args.Parameters[1].ToLowerInvariant())
                            {
                                case "水":
                                case "water":
                                    type = 0;
                                    break;
                                case "岩浆":
                                case "lava":
                                    type = 1;
                                    break;
                                case "蜂蜜":
                                case "honey":
                                    type = 2;
                                    break;
                                case "微光":
                                case "shimmer":
                                    type = 3;
                                    break;
                                default:
                                    op.SendErrorMessage("鱼池风格不对");
                                    return;
                            }
                        }
                        await AsyncGenPond(op, op.TileX, op.TileY + 3, type);
                        break;
                    }
                case "shieldmachine":
                case "sm":
                    if (!NeedInGame() && !NeedWaitTask())
                    {
                        bool isRight = op.TPlayer.direction != -1;
                        int w = 61;
                        int h = 34;
                        if (args.Parameters.Count > 1 && int.TryParse(args.Parameters[1], out num))
                        {
                            w = Math.Max(3, num);
                        }
                        if (args.Parameters.Count > 2 && int.TryParse(args.Parameters[2], out num))
                        {
                            h = Math.Max(3, num);
                        }
                        await AsyncGenShieldMachine(op, op.TileX, op.TileY + 3, w, h, isRight);
                    }
                    break;
                case "drill":
                    if (!NeedInGame() && !NeedWaitTask())
                    {
                        bool isRight = op.TPlayer.direction != -1;
                        int w = 3;
                        int h = 34;
                        if (Utils.TryParseInt(args.Parameters, 1, out num))
                        {
                            w = Math.Max(3, num);
                        }
                        if (Utils.TryParseInt(args.Parameters, 2, out num))
                        {
                            h = Math.Max(34, num);
                        }
                        await AsyncDigArea(op, op.TileX, op.TileY + 3, w, h, isRight);
                    }
                    break;
                case "hell":
                    if (!NeedInGame() && !NeedWaitTask())
                    {
                        await AsyncGenHellevator(op, op.TileX, op.TileY + 3);
                    }
                    break;
                case "random":
                    if (!NeedWaitTask())
                    {
                        if (args.Parameters.Count > 1 && args.Parameters[1].ToLowerInvariant() == "true")
                        {
                            RandomTool.RandomAll(args);
                        }
                        else
                        {
                            op.SendErrorMessage("本操作比较危险，将对全图的图格和背景墙进行随机，如确定此操作，请输入 /igen random true");
                        }
                    }
                    break;
                case "dirt":
                    if (!NeedInGame() && !NeedWaitTask())
                    {
                        await AsyncPlaceDirt(op);
                    }
                    break;
                case "hole":
                    if (!NeedInGame() && !NeedWaitTask())
                    {
                        ClearTool.Hole(op);
                    }
                    break;
                case "place":
                case "p":
                    if (!NeedInGame() && !NeedWaitTask())
                    {
                        PlaceTool.Manage(args);
                    }
                    break;
                case "selection":
                case "s":
                    if (!NeedInGame())
                    {
                        SelectionTool.Mange(args);
                    }
                    break;
                case "replace":
                case "r":
                    if (!NeedInGame() && !NeedWaitTask())
                    {
                        ReplaceTool.Manage(args);
                    }
                    break;
                case "fill":
                case "f":
                    if (!NeedInGame() && !NeedWaitTask())
                    {
                        FillTool.Manage(args);
                    }
                    break;
                case "clear":
                case "c":
                    if (!NeedInGame() && !NeedWaitTask())
                    {
                        ClearTool.Manage(args);
                    }
                    break;
                case "count":
                    CountTool.Manage(args);
                    break;
            }
            void Help()
            {
                if (PaginationTools.TryParsePageNumber(args.Parameters, 1, op, out var pageNumber))
                {
                    List<string> dataToPaginate = new List<string>
                {
                    "/igen room [数量]，小房间（默认生成3个）", "/igen hotel，NPC小旅馆", "/igen hell，地狱直通车", "/igen pond [water/lava/honey/shimmer]，鱼池", "/igen dirt，填土", "/igen sm [宽] [高]，盾构机（默认宽61高34）", "/igen drill [宽] [高]，钻井机（默认宽3高34）", "/igen place help，放置（实验性功能）", "/igen selection help，选区工具", "/igen replace help，替换工具",
                    "/igen fill help，填充工具", "/igen clear help，清除工具", "/igen count help，统计工具", "/igen world help，重建世界", "/igen random，全图随机", "", ""
                };
                    PaginationTools.SendPage(op, pageNumber, dataToPaginate, new PaginationTools.Settings
                    {
                        HeaderFormat = "/igen 指令用法 ({0}/{1})：",
                        FooterFormat = "输入 /igen help {{0}} 查看更多".SFormat(Commands.Specifier)
                    });
                }
            }
            bool NeedInGame()
            {
                return Utils.NeedInGame(op);
            }
            bool NeedWaitTask()
            {
                return TileHelper.NeedWaitTask(op);
            }
        }

        private static Task AsyncGenHotel(TSPlayer op, int posX, int posY, int total = 1, bool isRight = true, bool needCenter = false)
        {
            int secondLast = Utils.GetUnixTimestamp;
            return Task.Run(delegate
            {
                int num = 5;
                int num2 = 6;
                int num3 = 1 + Math.Min(num2, total) * num;
                int num4 = (needCenter ? (posX - num3 / 2) : posX);
                for (int i = 0; i < total; i++)
                {
                    int posX2 = num4 + i % num2 * num;
                    int posY2 = posY - (int)Math.Floor((float)(i / num2)) * 10;
                    GenRoom(posX2, posY2, isRight);
                }
                int num5 = num4 - 1;
                int num6 = num4 + num3;
                int num7 = posY - 1;
                WorldGen.PlaceTile(num5, num7, 19, false, true, -1, 14);
                WorldGen.PlaceTile(num6, num7, 19, false, true, -1, 14);
            }).ContinueWith(delegate
            {
                TileHelper.GenAfter();
                int value = Utils.GetUnixTimestamp - secondLast;
                op.SendSuccessMessage($"已生成NPC小旅馆，内含{total}个小房间，用时{value}秒。");
            });
        }

        private static Task AsyncGenRoom(TSPlayer op, int posX, int posY, int total = 1, bool isRight = true, bool needCenter = false)
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
                TileHelper.GenAfter();
                int value = Utils.GetUnixTimestamp - secondLast;
                op.SendSuccessMessage($"已生成{total}个小房间，用时{value}秒。");
            });
        }

        private static void GenRoom(int posX, int posY, bool isRight = true)
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
                num += 2;
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
                        val.slope((byte)0);
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

        private static Task AsyncGenPond(TSPlayer op, int posX, int posY, int style)
        {
            int secondLast = Utils.GetUnixTimestamp;
            return Task.Run(delegate
            {
                GenPond(posX, posY, style);
            }).ContinueWith(delegate
            {
                TileHelper.GenAfter();
                int value = Utils.GetUnixTimestamp - secondLast;
                string value2 = style switch
                {
                    1 => "岩浆",
                    2 => "蜂蜜",
                    3 => "微光",
                    _ => "普通",
                };
                op.SendSuccessMessage($"已生成{value2}鱼池，用时{value}秒。");
            });
        }

        private static void GenPond(int posX, int posY, int style)
        {
            PondTheme pondTheme = new PondTheme();
            switch (style)
            {
                case 1:
                    pondTheme.SetObsidian();
                    break;
                case 2:
                    pondTheme.SetHoney();
                    break;
                case 3:
                    pondTheme.SetGray();
                    break;
                default:
                    pondTheme.SetGlass();
                    break;
            }
            ushort tile = pondTheme.tile;
            TileInfo platform = pondTheme.platform;
            int num = posX - 6;
            int num2 = 13;
            int num3 = 32;
            for (int i = num; i < num + num2; i++)
            {
                for (int j = posY; j < posY + num3; j++)
                {
                    ITile val = Main.tile[i, j];
                    val.ClearEverything();
                    if (i == num || i == num + num2 - 1 || j == posY + num3 - 1)
                    {
                        val.type = tile;
                        val.active(true);
                        val.slope((byte)0);
                        val.halfBrick(false);
                    }
                }
                WorldGen.PlaceTile(i, posY, platform.id, false, true, -1, platform.style);
            }
            for (int k = num + 1; k < num + num2 - 1; k++)
            {
                for (int l = posY + 1; l < posY + num3 - 1; l++)
                {
                    ITile val2 = Main.tile[k, l];
                    val2.active(false);
                    val2.liquid = byte.MaxValue;
                    switch (style)
                    {
                        case 1:
                            val2.lava(true);
                            break;
                        case 2:
                            val2.honey(true);
                            break;
                        case 3:
                            val2.shimmer(true);
                            break;
                    }
                }
            }
        }

        private static Task AsyncGenHellevator(TSPlayer op, int posX, int posY)
        {
            int secondLast = Utils.GetUnixTimestamp;
            int height = 0;
            return Task.Run(delegate
            {
                height = GenHellevator(posX, posY);
            }).ContinueWith(delegate
            {
                TileHelper.GenAfter();
                int value = Utils.GetUnixTimestamp - secondLast;
                op.SendSuccessMessage($"已生成地狱直通车，高{height}格，用时{value}秒。");
            });
        }

        private static int GenHellevator(int posX, int posY)
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
            int Ystart = posY;
            Parallel.For(Xstart, Xstart + Width, delegate (int cx)
            {
                Parallel.For(Ystart, hell, delegate (int cy)
                {
                    ITile val = Main.tile[cx, cy];
                    val.ClearEverything();
                    if (cx == Xstart + Width / 2)
                    {
                        val.type = 365;
                        val.active(true);
                        val.slope((byte)0);
                        val.halfBrick(false);
                    }
                    else if (cx == Xstart || cx == Xstart + Width - 1)
                    {
                        val.type = 75;
                        val.active(true);
                        val.slope((byte)0);
                        val.halfBrick(false);
                    }
                });
            });
            WorldGen.PlaceTile(Xstart + 1, Ystart, 19, false, true, -1, 13);
            WorldGen.PlaceTile(Xstart + 2, Ystart, 19, false, true, -1, 13);
            WorldGen.PlaceTile(Xstart + 3, Ystart, 19, false, true, -1, 13);
            return hell;
        }

        private static Task AsyncPlaceDirt(TSPlayer op)
        {
            int secondLast = Utils.GetUnixTimestamp;
            return Task.Run(delegate
            {
                PlaceDirt(op);
            }).ContinueWith(delegate
            {
                TileHelper.GenAfter();
                int value = Utils.GetUnixTimestamp - secondLast;
                op.SendSuccessMessage($"填土完成，用时{value}秒。");
            });
        }

        private static void PlaceDirt(TSPlayer op)
        {
            Rectangle screen = Utils.GetScreen(op);
            for (int i = screen.X; i < screen.Right; i++)
            {
                for (int j = screen.Y; j < screen.Bottom; j++)
                {
                    Main.tile[i, j].ClearEverything();
                }
                Main.tile[i, screen.Y + 35].type = 2;
                Main.tile[i, screen.Y + 35].active(true);
                for (int k = screen.Y + 36; k < screen.Bottom; k++)
                {
                    Main.tile[i, k].type = 0;
                    Main.tile[i, k].active(true);
                }
            }
        }

        private static Task AsyncGenShieldMachine(TSPlayer op, int posX, int posY, int w, int h, bool isRight = true)
        {
            int secondLast = Utils.GetUnixTimestamp;
            return Task.Run(delegate
            {
                GenShieldMachine(posX, posY, w, h, isRight);
            }).ContinueWith(delegate
            {
                TileHelper.GenAfter();
                int value = Utils.GetUnixTimestamp - secondLast;
                op.SendSuccessMessage($"已盾构出{w}x{h}区域，用时{value}秒。");
            });
        }

        private static void GenShieldMachine(int posX, int posY, int w, int h, bool isRight = true)
        {
            int num = posX;
            int Ystart = posY;
            int num2 = Math.Max(3, w);
            int height = Math.Max(3, h);
            int toExclusive;
            if (isRight)
            {
                toExclusive = num + num2;
            }
            else
            {
                toExclusive = num + 2;
                num -= num2;
            }
            Parallel.For(num, toExclusive, delegate (int cx)
            {
                Parallel.For(Ystart - height, Ystart, delegate (int cy)
                {
                    Main.tile[cx, cy].ClearEverything();
                });
                WorldGen.PlaceTile(cx, Ystart, 19, false, true, -1, 43);
            });
        }

        private static Task AsyncDigArea(TSPlayer op, int posX, int posY, int w, int h, bool isRight = true, bool isHell = false)
        {
            int secondLast = Utils.GetUnixTimestamp;
            return Task.Run(delegate
            {
                DigArea(posX, posY, w, h, isRight, isHell);
            }).ContinueWith(delegate
            {
                TileHelper.GenAfter();
                int value = Utils.GetUnixTimestamp - secondLast;
                op.SendSuccessMessage($"已挖掘出{w}x{h}区域，用时{value}秒。");
            });
        }

        private static void DigArea(int posX, int posY, int w, int h, bool isRight = true, bool isHell = false)
        {
            int num = Math.Max(3, w);
            int height = Math.Max(3, h);
            int num2 = posX - num / 2 + 1;
            int Ystart = posY;
            Parallel.For(num2, num2 + num, delegate (int cx)
            {
                Parallel.For(Ystart, Ystart + height, delegate (int cy)
                {
                    Main.tile[cx, cy].ClearEverything();
                });
            });
            Parallel.For(posX, posX + 2, delegate (int cx)
            {
                WorldGen.PlaceTile(cx, Ystart, 19, false, true, -1, 43);
            });
        }

        public static async void AsyncDesertWorld(TSPlayer op)
        {
            await Task.Run(delegate
            {
                for (int i = 0; i < 10; i++)
                {
                    WorldGen.makeTemple(op.TileX, op.TileY);
                }
            }).ContinueWith(delegate
            {
                TileHelper.FinishGen();
                TileHelper.InformPlayers();
                op.SendSuccessMessage("创建沙漠地形完成");
            });
        }
    }
}
