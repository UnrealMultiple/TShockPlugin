using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using TShockAPI;
using WorldModify;

namespace WorldModify
{
    internal class ClearTool
    {
        private enum Type
        {
            None,
            All,
            AllBlock,
            Block,
            AllWall,
            Wall,
            Liquid,
            Water,
            Lava,
            Honey,
            Shimmer,
            AllWire,
            Wire1,
            Wire2,
            Wire3,
            Wire4
        }

        private static string Desc(Type type)
        {
            return type switch
            {
                Type.All => "所有",
                Type.AllBlock => "所有方块",
                Type.Block => "指定方块",
                Type.AllWall => "所有墙体",
                Type.Wall => "指定墙体",
                Type.AllWire => "所有电线",
                Type.Wire1 => "红电线",
                Type.Wire2 => "蓝电线",
                Type.Wire3 => "绿电线",
                Type.Wire4 => "黄电线",
                Type.Liquid => "所有液体",
                Type.Water => "水",
                Type.Lava => "岩浆",
                Type.Honey => "蜂蜜",
                Type.Shimmer => "微光",
                _ => "图格",
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
            int result = -1;
            string name = "";
            switch (args.Parameters[0].ToLowerInvariant())
            {
                case "help":
                    Help();
                    return;
                case "all":
                    type = Type.All;
                    break;
                case "block":
                    type = Type.AllBlock;
                    break;
                case "wall":
                case "w":
                case "墙":
                    if (args.Parameters.Count > 1)
                    {
                        WallProp wallByIDOrName = ResHelper.GetWallByIDOrName(args.Parameters[1]);
                        if (wallByIDOrName == null)
                        {
                            op.SendErrorMessage("输入的id或名称无效！");
                            return;
                        }
                        type = Type.Wall;
                        result = wallByIDOrName.id;
                        name = wallByIDOrName.Desc;
                    }
                    else
                    {
                        type = Type.AllWall;
                    }
                    break;
                case "wire":
                case "电线":
                    type = Type.AllWire;
                    break;
                case "red":
                case "红电线":
                    type = Type.Wire1;
                    break;
                case "blue":
                case "蓝电线":
                    type = Type.Wire2;
                    break;
                case "green":
                case "绿电线":
                    type = Type.Wire3;
                    break;
                case "yellow":
                case "黄电线":
                    type = Type.Wire4;
                    break;
                case "liquid":
                case "液体":
                    type = Type.Liquid;
                    break;
                case "water":
                case "水":
                    type = Type.Water;
                    break;
                case "lava":
                case "岩浆":
                    type = Type.Lava;
                    break;
                case "honey":
                case "蜂蜜":
                    type = Type.Honey;
                    break;
                case "shimmer":
                case "微光":
                    type = Type.Shimmer;
                    break;
                default:
                    if (int.TryParse(args.Parameters[0], out result))
                    {
                        if (result < 0 || result >= TileID.Count)
                        {
                            op.SendErrorMessage("输入的图格id无效！");
                            return;
                        }
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
                    List<string> dataToPaginate = new List<string> { "/igen c all，清除所有", "/igen c wall，清除所有墙体", "/igen c wall <id/中文名称>，清除指定墙体", "/igen c block，清除所有方块", "/igen c <id>，清除指定方块", "/igen c liquid，清除所有液体", "/igen c wire，清除所有电线", "/igen c <water/lava/honey/shimmer>，清除水/岩浆/蜂蜜/微光", "/igen c <red/blue/green/yellow>，清除红/蓝/绿/黄电线" };
                    PaginationTools.SendPage(op, pageNumber, dataToPaginate, new PaginationTools.Settings
                    {
                        HeaderFormat = "[c/96FF0A:/igen c]lear 指令用法 ({0}/{1})：",
                        FooterFormat = "输入 /igen c help {{0}} 查看更多".SFormat(Commands.Specifier)
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
                            case Type.All:
                                if (val.active() || val.wall > 0 || val.liquid > 0)
                                {
                                    count++;
                                }
                                val.ClearEverything();
                                break;
                            case Type.AllBlock:
                                if (val.active())
                                {
                                    count++;
                                    val.ClearTile();
                                }
                                break;
                            case Type.Block:
                                if (val.type == id)
                                {
                                    count++;
                                    val.ClearTile();
                                }
                                break;
                            case Type.AllWall:
                                if (val.wall > 0)
                                {
                                    count++;
                                    val.wall = 0;
                                }
                                break;
                            case Type.Wall:
                                if (val.wall == id)
                                {
                                    count++;
                                    val.wall = 0;
                                }
                                break;
                            case Type.AllWire:
                                if (val.wire() || val.wire2() || val.wire3() || val.wire4())
                                {
                                    count++;
                                }
                                val.wire(false);
                                val.wire2(false);
                                val.wire3(false);
                                val.wire4(false);
                                break;
                            case Type.Wire1:
                                if (val.wire())
                                {
                                    count++;
                                    val.wire(false);
                                }
                                break;
                            case Type.Wire2:
                                if (val.wire2())
                                {
                                    count++;
                                    val.wire2(false);
                                }
                                break;
                            case Type.Wire3:
                                if (val.wire3())
                                {
                                    count++;
                                    val.wire3(false);
                                }
                                break;
                            case Type.Wire4:
                                if (val.wire4())
                                {
                                    count++;
                                    val.wire4(false);
                                }
                                break;
                            case Type.Liquid:
                            case Type.Water:
                            case Type.Lava:
                            case Type.Honey:
                            case Type.Shimmer:
                                count += ClearLiquid(i, j, type);
                                break;
                        }
                    }
                }
            }).ContinueWith(delegate
            {
                TileHelper.GenAfter();
                int value = Utils.GetUnixTimestamp - secondLast;
                if (string.IsNullOrEmpty(name))
                {
                    name = Desc(type);
                }
                op.SendSuccessMessage($"已清除{name}{count}格，用时{value}秒。");
            });
        }

        private static int ClearLiquid(int x, int y, Type type)
        {
            ITile val = Main.tile[x, y];
            if (val.liquid > 0)
            {
                bool flag = false;
                switch (type)
                {
                    case Type.Liquid:
                        flag = true;
                        break;
                    case Type.Water:
                        flag = val.liquidType() == 0;
                        break;
                    case Type.Lava:
                        flag = val.liquidType() == 1;
                        break;
                    case Type.Honey:
                        flag = val.liquidType() == 2;
                        break;
                    case Type.Shimmer:
                        flag = val.liquidType() == 3;
                        break;
                }
                if (flag)
                {
                    val.liquid = 0;
                    return 1;
                }
            }
            return 0;
        }

        public static void Hole(TSPlayer op)
        {
            Rectangle val = new Rectangle(op.TileX - 3, op.TileY - 5, 7, 7);
            int num = 0;
            for (int i = val.X; i < val.Right; i++)
            {
                for (int j = val.Y; j < val.Bottom; j++)
                {
                    if (j == val.Y || j == val.Y + 6)
                    {
                        if (i == val.X || i == val.X + 1 || i == val.X + 5 || i == val.X + 6)
                        {
                            continue;
                        }
                    }
                    else if ((j == val.Y + 1 || j == val.Y + 5) && (i == val.X || i == val.X + 6))
                    {
                        continue;
                    }
                    ITile val2 = Main.tile[i, j];
                    if (val2.active() || val2.wall > 0 || val2.liquid > 0)
                    {
                        num++;
                    }
                    val2.ClearEverything();
                }
            }
            if (num > 0)
            {
                NetMessage.SendTileSquare(-1, op.TileX, op.TileY, 14, (TileChangeType)0);
                op.SendSuccessMessage($"打洞完成，已清空{num}格。");
            }
            else
            {
                op.SendSuccessMessage("打洞完成，未清空任何东西。");
            }
        }
    }
}
