using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace WorldModify
{
    internal class FillTool
    {
        private enum Type
        {
            None,
            Block,
            Wall,
            Dirt,
            Mud,
            Water,
            Honey,
            Lava,
            Shimmer,
            Wire1,
            Wire2,
            Wire3,
            Wire4
        }

        private static readonly string[] Desc = new string[13]
        {
        "", "方块", "墙体", "土块", "泥块", "水", "蜂蜜", "岩浆", "微光", "红电线",
        "蓝电线", "绿电线", "黄电线"
        };

        public static void Manage(CommandArgs args)
        {
            args.Parameters.RemoveAt(0);
            TSPlayer op = args.Player;
            if (args.Parameters.Count == 0 || args.Parameters[0].ToLowerInvariant() == "help")
            {
                Help();
                return;
            }
            Type type = Type.None;
            int result = -1;
            string name = "";
            string text = args.Parameters[0].ToLowerInvariant();
            switch (text)
            {
                case "dirt":
                case "土":
                case "土块":
                    type = Type.Dirt;
                    result = 0;
                    break;
                case "mud":
                case "泥":
                case "泥块":
                    type = Type.Mud;
                    result = 59;
                    break;
                case "water":
                case "水":
                    type = Type.Water;
                    break;
                case "honey":
                case "蜂蜜":
                    type = Type.Honey;
                    break;
                case "lava":
                case "岩浆":
                    type = Type.Lava;
                    break;
                case "shimmer":
                case "微光":
                    type = Type.Shimmer;
                    break;
                case "wall":
                case "w":
                case "墙":
                    {
                        if (args.Parameters.Count < 2)
                        {
                            op.SendErrorMessage("需要输入墙体ID，/igen f wall <id/中文名称>");
                            return;
                        }
                        WallProp wallByIDOrName = ResHelper.GetWallByIDOrName(args.Parameters[1]);
                        if (wallByIDOrName == null)
                        {
                            op.SendErrorMessage("输入的id或中文名称无效！");
                            return;
                        }
                        type = Type.Wall;
                        result = wallByIDOrName.id;
                        name = wallByIDOrName.Desc;
                        break;
                    }
                case "red":
                case "红电线":
                case "电线":
                    type = Type.Wire1;
                    result = 1;
                    break;
                case "blue":
                case "蓝电线":
                    type = Type.Wire2;
                    result = 2;
                    break;
                case "green":
                case "绿电线":
                    type = Type.Wire3;
                    result = 3;
                    break;
                case "yellow":
                case "黄电线":
                    type = Type.Wire4;
                    result = 4;
                    break;
                default:
                    if (int.TryParse(text, out result))
                    {
                        type = Type.Block;
                    }
                    else
                    {
                        Help();
                    }
                    break;
            }
            if (type != 0)
            {
                Action(op, type, result, name);
            }
            void Help()
            {
                if (PaginationTools.TryParsePageNumber(args.Parameters, 1, op, out var pageNumber))
                {
                    List<string> dataToPaginate = new List<string> { "/igen f <id>，填充图格", "/igen f wall <id/中文名>，铺墙", "/igen f <dirt/mud>，填充土块/泥块", "/igen f <water/lava/honey/shimmer>，填充水/岩浆/蜂蜜/微光", "/igen f <red/blue/green/yellow>，铺设红/蓝/绿/黄电线" };
                    PaginationTools.SendPage(op, pageNumber, dataToPaginate, new PaginationTools.Settings
                    {
                        HeaderFormat = "[c/96FF0A:/igen f]ill 指令用法 ({0}/{1})：",
                        FooterFormat = "输入 /igen f help {{0}} 查看更多".SFormat(Commands.Specifier)
                    });
                }
            }
        }

        private static async void Action(TSPlayer op, Type type, int id, string name)
        {
            Rectangle rect = SelectionTool.GetSelection(op.Index);
            int secondLast = Utils.GetUnixTimestamp;
            int count = 0;
            await Task.Run(delegate
            {
                for (int i = rect.X; i < rect.Right; i++)
                {
                    for (int j = rect.Y; j < rect.Bottom; j++)
                    {
                        ITile val = Main.tile[i, j];
                        switch (type)
                        {
                            case Type.Block:
                            case Type.Dirt:
                            case Type.Mud:
                                if (!val.active() && val.liquid == 0)
                                {
                                    count++;
                                    Fill(i, j, type, id);
                                }
                                break;
                            case Type.Wall:
                                if (val.wall == 0)
                                {
                                    count++;
                                    val.wall = (ushort)id;
                                }
                                break;
                            case Type.Water:
                            case Type.Honey:
                            case Type.Lava:
                            case Type.Shimmer:
                                if (val.liquid == 0 && val.wall == 0 && !val.active())
                                {
                                    count++;
                                    FillLiquid(i, j, type);
                                }
                                break;
                        }
                    }
                }
                Type type4 = type;
                Type type5 = type4;
                if ((uint)(type5 - 9) <= 3u)
                {
                    for (int k = rect.X; k < rect.Right; k++)
                    {
                        ITile val2 = Main.tile[k, rect.Bottom - 1];
                        if (id == 1)
                        {
                            val2.wire(true);
                        }
                        else if (id == 2)
                        {
                            val2.wire2(true);
                        }
                        else if (id == 3)
                        {
                            val2.wire3(true);
                        }
                        else if (id == 4)
                        {
                            val2.wire4(true);
                        }
                        count++;
                    }
                    for (int l = rect.Y; l < rect.Bottom; l++)
                    {
                        ITile val3 = Main.tile[rect.Right - 1, l];
                        if (id == 1)
                        {
                            val3.wire(true);
                        }
                        else if (id == 2)
                        {
                            val3.wire2(true);
                        }
                        else if (id == 3)
                        {
                            val3.wire3(true);
                        }
                        else if (id == 4)
                        {
                            val3.wire4(true);
                        }
                        count++;
                    }
                }
            }).ContinueWith(delegate
            {
                TileHelper.GenAfter();
                int value = Utils.GetUnixTimestamp - secondLast;
                if (string.IsNullOrEmpty(name))
                {
                    name = Desc[(int)type];
                }
                string value2 = "";
                Type type2 = type;
                Type type3 = type2;
                if ((uint)(type3 - 5) <= 3u)
                {
                    value2 = "，如液体不流动，可输入[c/96FF0A:/settle]进行平衡";
                }
                op.SendSuccessMessage($"已填充{name}{count}格，用时{value}秒{value2}。");
            });
        }

        private static void Fill(int x, int y, Type type, int replaceID = -1)
        {
            ITile val = Main.tile[x, y];
            int num = -1;
            switch (type)
            {
                case Type.Block:
                    if (replaceID != -1)
                    {
                        num = replaceID;
                    }
                    break;
                case Type.Dirt:
                    num = 0;
                    break;
                case Type.Mud:
                    num = 59;
                    break;
            }
            if (num != -1)
            {
                val.type = (ushort)num;
                val.active(true);
            }
        }

        private static void FillLiquid(int x, int y, Type type)
        {
            ITile val = Main.tile[x, y];
            if (!val.active() && val.liquid <= 0)
            {
                int num = -1;
                switch (type)
                {
                    case Type.Water:
                        num = 0;
                        break;
                    case Type.Lava:
                        num = 1;
                        break;
                    case Type.Honey:
                        num = 2;
                        break;
                    case Type.Shimmer:
                        num = 3;
                        break;
                }
                if (num != -1)
                {
                    val.liquid = byte.MaxValue;
                    val.liquidType(num);
                }
            }
        }
    }
}
