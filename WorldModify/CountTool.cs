using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using TShockAPI;

namespace WorldModify
{
    internal class CountTool
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
                Type.AllBlock => "方块",
                Type.Block => "指定方块",
                Type.AllWall => "墙体",
                Type.Wall => "指定墙体",
                Type.AllWire => "电线",
                Type.Wire1 => "红电线",
                Type.Wire2 => "蓝电线",
                Type.Wire3 => "绿电线",
                Type.Wire4 => "黄电线",
                Type.Liquid => "液体",
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
                            op.SendErrorMessage("输入的id/中文名称无效！");
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
                            op.SendErrorMessage("输入的id无效！");
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
                    List<string> dataToPaginate = new List<string> { "/igen count block，统计方块的数量", "/igen count <id>，统计指定方块的数量", "/igen count wall，统计墙的数量", "/igen count wall <id/中文名称>，统计指定墙的数量", "/igen count liquid，统计液体的数量", "/igen count <water/lava/honey/shimmer>，统计水/岩浆/蜂蜜/微光的数量", "/igen count wire，统计电线的数量", "/igen count <red/blue/green/yellow>，统计红/蓝/绿/黄电线的数量" };
                    PaginationTools.SendPage(op, pageNumber, dataToPaginate, new PaginationTools.Settings
                    {
                        HeaderFormat = "/igen count 指令用法 ({0}/{1})：",
                        FooterFormat = "输入 /igen count help {{0}} 查看更多".SFormat(Commands.Specifier)
                    });
                }
            }
        }

        private static async void Action(TSPlayer op, Type type, int id, string name)
        {
            Rectangle rect = (op.RealPlayer ? SelectionTool.GetSelection(op.Index) : Utils.GetWorldArea());
            int count = 0;
            int wallNum = 0;
            int liquidNum = 0;
            int wireNum = 0;
            await Task.Run(delegate
            {
                for (int i = rect.X; i < ((Rectangle)( rect)).Right; i++)
                {
                    for (int j = rect.Y; j < ((Rectangle)( rect)).Bottom; j++)
                    {
                        ITile val = Main.tile[i, j];
                        switch (type)
                        {
                            case Type.All:
                                if (val.active())
                                {
                                    count++;
                                }
                                if (val.wall > 0)
                                {
                                    wallNum++;
                                }
                                if (val.liquid == byte.MaxValue)
                                {
                                    liquidNum++;
                                }
                                if (val.wire() || val.wire2() || val.wire3() || val.wire4())
                                {
                                    wireNum++;
                                }
                                break;
                            case Type.AllBlock:
                                if (val.active())
                                {
                                    count++;
                                }
                                break;
                            case Type.Block:
                                if (val.active() && val.type == id)
                                {
                                    count++;
                                }
                                break;
                            case Type.AllWall:
                                if (val.wall > 0)
                                {
                                    count++;
                                }
                                break;
                            case Type.Wall:
                                if (val.wall == id)
                                {
                                    count++;
                                }
                                break;
                            case Type.AllWire:
                                if (val.wire() || val.wire2() || val.wire3() || val.wire4())
                                {
                                    count++;
                                }
                                break;
                            case Type.Wire1:
                                if (val.wire())
                                {
                                    count++;
                                }
                                break;
                            case Type.Wire2:
                                if (val.wire2())
                                {
                                    count++;
                                }
                                break;
                            case Type.Wire3:
                                if (val.wire3())
                                {
                                    count++;
                                }
                                break;
                            case Type.Wire4:
                                if (val.wire4())
                                {
                                    count++;
                                }
                                break;
                            case Type.Liquid:
                                if (val.liquid > 0)
                                {
                                    count++;
                                }
                                break;
                            case Type.Water:
                                if (val.liquid > 0 && val.liquidType() == 0)
                                {
                                    count++;
                                }
                                break;
                            case Type.Lava:
                                if (val.liquid > 0 && val.liquidType() == 1)
                                {
                                    count++;
                                }
                                break;
                            case Type.Honey:
                                if (val.liquid > 0 && val.liquidType() == 2)
                                {
                                    count++;
                                }
                                break;
                            case Type.Shimmer:
                                if (val.liquid > 0 && val.liquidType() == 3)
                                {
                                    count++;
                                }
                                break;
                        }
                    }
                }
            }).ContinueWith(delegate
            {
                if (type == Type.All)
                {
                    op.SendInfoMessage($"{Desc(Type.AllBlock)}: {count}格");
                    op.SendInfoMessage($"{Desc(Type.AllWall)}: {wallNum}格");
                    op.SendInfoMessage($"{Desc(Type.Liquid)}: {liquidNum}格");
                    op.SendInfoMessage($"{Desc(Type.AllWire)}: {wireNum}根");
                }
                else
                {
                    if (string.IsNullOrEmpty(name))
                    {
                        name = Desc(type);
                    }
                    op.SendInfoMessage($"{name}: {count}格");
                }
            });
        }
    }
}
