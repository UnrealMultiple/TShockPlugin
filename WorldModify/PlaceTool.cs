using Terraria;
using Terraria.ID;
using TShockAPI;

namespace WorldModify
{
    internal class PlaceTool
    {
        public static void Manage(CommandArgs args)
        {
            args.Parameters.RemoveAt(0);
            TSPlayer op = args.Player;
            if (args.Parameters.Count == 0)
            {
                Help();
                return;
            }
            string text = args.Parameters[0].ToLowerInvariant();
            int tileX = op.TileX;
            int num = op.TileY + 2;
            switch (text)
            {
                case "help":
                    Help();
                    return;
                case "生命水晶":
                case "crystal":
                    WorldGen.AddLifeCrystal(tileX, num);
                    NetMessage.SendTileSquare(-1, tileX, num, 3, (TileChangeType)0);
                    return;
                case "暗影珠":
                case "orb":
                    {
                        bool crimson = WorldGen.crimson;
                        WorldGen.crimson = false;
                        WorldGen.AddShadowOrb(tileX, num);
                        WorldGen.crimson = crimson;
                        NetMessage.SendTileSquare(-1, tileX, num, 3, (TileChangeType)0);
                        return;
                    }
                case "猩红之心":
                case "heart":
                    {
                        bool crimson = WorldGen.crimson;
                        WorldGen.crimson = true;
                        WorldGen.AddShadowOrb(tileX, num);
                        WorldGen.crimson = crimson;
                        NetMessage.SendTileSquare(-1, tileX, num, 3, (TileChangeType)0);
                        return;
                    }
                case "恶魔祭坛":
                case "demon":
                    WorldGen.Place3x2(tileX, num, (ushort)26, 0);
                    NetMessage.SendTileSquare(-1, tileX, num, 3, (TileChangeType)0);
                    return;
                case "猩红祭坛":
                case "crimson":
                    WorldGen.Place3x2(tileX, num, (ushort)26, 1);
                    NetMessage.SendTileSquare(-1, tileX, num, 3, (TileChangeType)0);
                    return;
                case "祭坛":
                case "altar":
                    if (!WorldGen.crimson)
                    {
                        WorldGen.Place3x2(tileX, num, (ushort)26, 0);
                    }
                    else
                    {
                        WorldGen.Place3x2(tileX, num, (ushort)26, 1);
                    }
                    NetMessage.SendTileSquare(-1, tileX, num, 3, (TileChangeType)0);
                    return;
                case "附魔剑":
                case "sword":
                    WorldGen.PlaceTile(tileX, num, 187, true, false, -1, 17);
                    NetMessage.SendTileSquare(-1, tileX, num, 3, (TileChangeType)0);
                    return;
                case "罐子":
                case "pot":
                    {
                        int num2 = WorldGen.genRand.Next(0, 4);
                        int num3 = 0;
                        int num4 = 0;
                        if (num < Main.maxTilesY - 5)
                        {
                            num3 = Main.tile[tileX, num + 1].type;
                            num4 = Main.tile[tileX, num].wall;
                        }
                        if (num3 == 147 || num3 == 161 || num3 == 162)
                        {
                            num2 = WorldGen.genRand.Next(4, 7);
                        }
                        if (num3 == 60)
                        {
                            num2 = WorldGen.genRand.Next(7, 10);
                        }
                        if (Main.wallDungeon[Main.tile[tileX, num].wall])
                        {
                            num2 = WorldGen.genRand.Next(10, 13);
                        }
                        if (num3 == 41 || num3 == 43 || num3 == 44 || num3 == 481 || num3 == 482 || num3 == 483)
                        {
                            num2 = WorldGen.genRand.Next(10, 13);
                        }
                        if (num3 == 22 || num3 == 23 || num3 == 25)
                        {
                            num2 = WorldGen.genRand.Next(16, 19);
                        }
                        if (num3 == 199 || num3 == 203 || num3 == 204 || num3 == 200)
                        {
                            num2 = WorldGen.genRand.Next(22, 25);
                        }
                        if (num3 == 367)
                        {
                            num2 = WorldGen.genRand.Next(31, 34);
                        }
                        if (num3 == 226)
                        {
                            num2 = WorldGen.genRand.Next(28, 31);
                        }
                        if (num4 == 187 || num4 == 216)
                        {
                            num2 = WorldGen.genRand.Next(34, 37);
                        }
                        if (num > Main.UnderworldLayer)
                        {
                            num2 = WorldGen.genRand.Next(13, 16);
                        }
                        if (!WorldGen.oceanDepths(tileX, num) && !Main.tile[tileX, num].shimmer())
                        {
                            WorldGen.PlacePot(tileX, num, (ushort)28, num2);
                            NetMessage.SendTileSquare(-1, tileX, num, 3, (TileChangeType)0);
                        }
                        else
                        {
                            op.SendErrorMessage("未能放置罐子");
                        }
                        return;
                    }
            }
            int result = 0;
            if (int.TryParse(text, out var result2))
            {
                if (result2 < 0 || result2 >= TileID.Count)
                {
                    op.SendErrorMessage("输入的图格id无效！");
                    return;
                }
                if (args.Parameters.Count > 1 && !int.TryParse(args.Parameters[1], out result))
                {
                    op.SendErrorMessage("图格样式输入错误！");
                    return;
                }
                tileX = op.TileX;
                num = op.TileY + 2;
                WorldGen.PlaceTile(tileX, num, result2, false, true, -1, result);
                NetMessage.SendTileSquare(-1, tileX, num, 20, (TileChangeType)0);
                op.SendSuccessMessage($"放置完成 id={result2} 样式={result}");
            }
            else
            {
                Help();
            }
            void Help()
            {
                op.SendErrorMessage("[c/96FF0A:/igen p]lace 指令用法：\n/igen place <id> [style]，放置\n/igen place <名称>, 目前支持 生命水晶、暗影珠、猩红之心、恶魔祭坛、猩红祭坛 和 附魔剑。");
            }
        }
    }
}
