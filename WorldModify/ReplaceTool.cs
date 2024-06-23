using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using TShockAPI;

namespace WorldModify
{
    internal class ReplaceTool
    {
        private enum Type
        {
            None,
            Block,
            Wall,
            Random,
            Config,
            Ice,
            Helt,
            Lava,
            Truffle,
            Desert,
            Purify,
            Corruption,
            Crimson,
            Hallow
        }

        private static string TypeDesc(Type type)
        {
            return type switch
            {
                Type.Block => "替换方块",
                Type.Wall => "替换墙",
                Type.Random => "随机图格",
                Type.Config => "按配置替换",
                Type.Ice => "冰河化",
                Type.Helt => "冰融化",
                Type.Lava => "岩浆化",
                Type.Truffle => "蘑菇化",
                Type.Desert => "沙漠化",
                Type.Purify => "净化",
                Type.Corruption => "腐化",
                Type.Crimson => "猩红化",
                Type.Hallow => "神圣化",
                _ => "替换图格",
            };
        }

        public static void Manage(CommandArgs args)
        {
            args.Parameters.RemoveAt(0);
            TSPlayer op = args.Player;
            if (args.Parameters.Count == 0)
            {
                Help();
                return;
            }
            Type type = Type.None;
            string text = args.Parameters[0].ToLowerInvariant();
            int result = -1;
            int result2 = -1;
            string name = "";
            switch (text)
            {
                case "help":
                    Help();
                    break;
                default:
                    if (int.TryParse(text, out result))
                    {
                        if (args.Parameters.Count < 2 || !int.TryParse(args.Parameters[1], out result2))
                        {
                            op.SendErrorMessage("指令用法：/igen r <目标id> <替换后id>");
                            return;
                        }
                        if (result >= TileID.Count || result < 0)
                        {
                            op.SendErrorMessage("输入的第一个id无效！");
                            return;
                        }
                        if (result2 >= TileID.Count || result2 < 0)
                        {
                            op.SendErrorMessage("输入的第二个id无效！");
                            return;
                        }
                        type = Type.Block;
                    }
                    else
                    {
                        Help();
                    }
                    break;
                case "wall":
                case "w":
                case "墙":
                    {
                        if (args.Parameters.Count < 3)
                        {
                            op.SendErrorMessage("指令用法：/igen r wall <目标id> <替换后的id>");
                            return;
                        }
                        WallProp wallByIDOrName = ResHelper.GetWallByIDOrName(args.Parameters[1]);
                        if (wallByIDOrName == null)
                        {
                            op.SendErrorMessage("输入的第一个id无效！");
                            return;
                        }
                        WallProp wallByIDOrName2 = ResHelper.GetWallByIDOrName(args.Parameters[2]);
                        if (wallByIDOrName2 == null)
                        {
                            op.SendErrorMessage("输入的第二个id无效！");
                            return;
                        }
                        type = Type.Wall;
                        result = wallByIDOrName.id;
                        result2 = wallByIDOrName2.id;
                        name = wallByIDOrName.Desc + " → " + wallByIDOrName2.Desc;
                        break;
                    }
                case "config":
                case "con":
                    if (RetileTool.FirstCreate())
                    {
                        op.SendErrorMessage(RetileTool.SaveFile + " 已创建，按格式编辑后，再次执行本指令");
                        return;
                    }
                    RetileTool.Init();
                    type = Type.Config;
                    break;
                case "desert":
                    type = Type.Desert;
                    break;
                case "ice":
                    type = Type.Ice;
                    break;
                case "melt":
                case "mel":
                    type = Type.Helt;
                    break;
                case "lava":
                    type = Type.Lava;
                    break;
                case "purify":
                case "pur":
                    type = Type.Purify;
                    break;
                case "corruption":
                case "cor":
                    type = Type.Corruption;
                    break;
                case "crimson":
                case "cri":
                    type = Type.Crimson;
                    break;
                case "hallow":
                case "hal":
                    type = Type.Hallow;
                    break;
                case "mushroom":
                case "mus":
                    type = Type.Truffle;
                    break;
                case "random":
                case "ran":
                    type = Type.Random;
                    break;
            }
            if (type != 0)
            {
                Action(op, type, result, result2, name);
            }
            void Help()
            {
                if (PaginationTools.TryParsePageNumber(args.Parameters, 1, op, out var pageNumber))
                {
                    List<string> dataToPaginate = new List<string>
                {
                    "/igen r <id> <id>，替换方块", "/igen r wall <id> <id>，替换墙体", "/igen r ice，水 → 薄冰", "/igen r lava，水 → 岩浆", "/igen r melt，薄冰 → 水", "/igen r purify，净化", "/igen r corruption，腐化", "/igen r crimson，猩红化", "/igen r hallow，神圣化", "/igen r mushroom，蘑菇化",
                    "/igen r desert，沙漠化", "/igen r random，随机", "/igen r config，按配置替换"
                };
                    PaginationTools.SendPage(op, pageNumber, dataToPaginate, new PaginationTools.Settings
                    {
                        HeaderFormat = "[c/96FF0A:/igen r]eplace 指令用法 ({0}/{1})：",
                        FooterFormat = "输入 /igen r help {{0}} 查看更多".SFormat(Commands.Specifier)
                    });
                }
            }
        }

        private static async void Action(TSPlayer op, Type type, int beforeID = -1, int afterID = -1, string name = "")
        {
            Rectangle rect = SelectionTool.GetSelection(op.Index);
            int secondLast = Utils.GetUnixTimestamp;
            int count = 0;
            List<ReTileInfo> replaceInfo = new List<ReTileInfo>();
            switch (type)
            {
                case Type.Config:
                    replaceInfo = RetileTool.Con.replace;
                    break;
                case Type.Desert:
                    replaceInfo = ReTileTheme.GetDesert();
                    break;
            }
            await Task.Run(delegate
            {
                for (int i = rect.X; i < rect.Right; i++)
                {
                    for (int j = rect.Y; j < rect.Bottom; j++)
                    {
                        switch (type)
                        {
                            case Type.Random:
                                count += RandomTool.RandomTile(i, j);
                                break;
                            case Type.Config:
                            case Type.Desert:
                                count += Replace(i, j, replaceInfo);
                                break;
                            case Type.Block:
                                count += RelaceBlock(i, j, beforeID, afterID);
                                break;
                            case Type.Wall:
                                count += RelaceWall(i, j, beforeID, afterID);
                                break;
                            case Type.Ice:
                                count += IceAgeTile(i, j);
                                break;
                            case Type.Helt:
                                count += IceMeltTile(i, j);
                                break;
                            case Type.Lava:
                                count += LavaLiquid(i, j);
                                break;
                            case Type.Purify:
                                WorldGen.Convert(i, j, 0, 4);
                                break;
                            case Type.Corruption:
                                WorldGen.Convert(i, j, 1, 4);
                                break;
                            case Type.Crimson:
                                WorldGen.Convert(i, j, 4, 4);
                                break;
                            case Type.Hallow:
                                WorldGen.Convert(i, j, 2, 4);
                                break;
                            case Type.Truffle:
                                WorldGen.Convert(i, j, 3, 4);
                                break;
                        }
                    }
                }
            }).ContinueWith(delegate
            {
                TileHelper.FinishGen();
                int value = Utils.GetUnixTimestamp - secondLast;
                if (string.IsNullOrEmpty(name))
                {
                    name = TypeDesc(type);
                }
                Type type2 = type;
                Type type3 = type2;
                if (type3 == Type.Truffle || (uint)(type3 - 10) <= 3u)
                {
                    op.SendSuccessMessage($"已{name}，用时{value}秒。");
                }
                else
                {
                    op.SendSuccessMessage($"已{name}{count}格，用时{value}秒。");
                }
            });
        }

        private static int Replace(int x, int y, List<ReTileInfo> replaceInfo)
        {
            ITile tile = Main.tile[x, y];
            bool flag = false;
            IEnumerable<ReTileInfo> enumerable;
            if (tile.active())
            {
                enumerable = replaceInfo.Where((ReTileInfo info) => info.before.type == 0 && info.before.id == tile.type);
                foreach (ReTileInfo item in enumerable)
                {
                    RelaceBlock(x, y, item);
                    flag = true;
                }
            }
            enumerable = replaceInfo.Where((ReTileInfo info) => info.before.type == 1 && info.before.id == tile.wall);
            foreach (ReTileInfo item2 in enumerable)
            {
                if (item2.after.type == 1)
                {
                    tile.wall = (ushort)item2.after.id;
                    flag = true;
                }
            }
            if (tile.liquid > 0)
            {
                int liquidType = tile.liquidType() + 1;
                enumerable = replaceInfo.Where((ReTileInfo info) => info.before.type == 2 && info.before.id == liquidType);
                foreach (ReTileInfo item3 in enumerable)
                {
                    if (item3.after.type == 0)
                    {
                        if (tile.active())
                        {
                            tile.liquid = 0;
                        }
                        else
                        {
                            tile.type = (ushort)item3.after.id;
                            tile.active(true);
                            tile.slope((byte)0);
                            tile.halfBrick(false);
                            tile.liquid = 0;
                        }
                        flag = true;
                    }
                }
            }
            if (flag)
            {
                NetMessage.SendTileSquare(-1, x, y, (TileChangeType)0);
                return 1;
            }
            return 0;
        }

        private static int RelaceBlock(int x, int y, ReTileInfo info2)
        {
            ITile val = Main.tile[x, y];
            int type = info2.after.type;
            int id = info2.after.id;
            int style = info2.after.style;
            if (type == 0)
            {
                if (id == -1)
                {
                    val.ClearTile();
                }
                else
                {
                    val.type = (ushort)id;
                }
                return 1;
            }
            return 0;
        }

        private static int RelaceBlock(int x, int y, int before, int after)
        {
            ITile val = Main.tile[x, y];
            if (val.type == before)
            {
                val.type = (ushort)after;
                NetMessage.SendTileSquare(-1, x, y, (TileChangeType)0);
                return 1;
            }
            return 0;
        }

        private static int RelaceWall(int x, int y, int before, int after)
        {
            ITile val = Main.tile[x, y];
            if (val.wall == before)
            {
                val.wall = (ushort)after;
                NetMessage.SendTileSquare(-1, x, y, (TileChangeType)0);
                return 1;
            }
            return 0;
        }

        public static int IceAgeTile(int x, int y)
        {
            ITile val = Main.tile[x, y];
            if (val.liquid > 0)
            {
                switch (val.liquidType())
                {
                    case 0:
                        if (!val.active())
                        {
                            val.type = 162;
                            val.active(true);
                            val.slope((byte)0);
                            val.halfBrick(false);
                        }
                        val.liquid = 0;
                        return 1;
                }
            }
            return 0;
        }

        public static int LavaLiquid(int x, int y)
        {
            ITile val = Main.tile[x, y];
            if (val.liquid > 0)
            {
                switch (val.liquidType())
                {
                    case 0:
                        if (!val.active())
                        {
                            val.lava(true);
                            val.liquid = byte.MaxValue;
                            WorldGen.SquareTileFrame(x, y, true);
                            NetMessage.SendTileSquare(-1, x, y, (TileChangeType)0);
                        }
                        return 1;
                }
            }
            return 0;
        }

        public static int IceMeltTile(int tileX, int tileY)
        {
            ITile val = Main.tile[tileX, tileY];
            if (val.type != 162)
            {
                return 0;
            }
            val.active(false);
            val.liquid = byte.MaxValue;
            WorldGen.SquareTileFrame(tileX, tileY, true);
            return 1;
        }
    }
}
