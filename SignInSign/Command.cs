using Terraria;
using Terraria.ID;
using TShockAPI;

namespace SignInSign
{
    internal class Command
    {
        internal static void SetupCmd(CommandArgs args)
        {
            //获取玩家当前坐标
            int x = args.Player.TileX;
            int y = args.Player.TileY;

            const string Message = $"[告示牌登录]请输入以下指令：\n重置告示牌：[c/42B2CE:/gs r]\n设置传送点：[c/F25E61:/gs s]";

            if (args.Parameters.Count == 0)
            {
                args.Player.SendMessage(Message, Microsoft.Xna.Framework.Color.YellowGreen);
                return;
            }

            switch (args.Parameters[0].ToLower())
            {
                case "r":
                case "reload":
                case "重置":
                    ReloadCmd(args);
                    return;
                case "s":
                case "set":
                case "设置":
                    if (args.Parameters.Count != 1)
                    {
                        args.Player.SendMessage("[告示牌登录]设置传送点命令无需额外参数，将会使用你当前位置。", Microsoft.Xna.Framework.Color.Yellow);
                    }
                    else
                    {
                        SignInSign.Config.Teleport_X = x;
                        SignInSign.Config.Teleport_Y = y;
                        args.Player.SendMessage($"已将你所在的位置设置为[c/9487D6:告示牌传送点]，坐标为({x}, {y})", Microsoft.Xna.Framework.Color.Yellow);
                        Console.WriteLine($"【告示牌登录】传送点已设置，坐标为({x}, {y})", Microsoft.Xna.Framework.Color.Yellow);

                        // 确保配置更改被保存
                        SignInSign.Config.Write(Configuration.ConfigPath);
                    }
                    break;
                default:
                    args.Player.SendMessage(Message, Microsoft.Xna.Framework.Color.YellowGreen);
                    return;
            }
        }

        private static void ReloadCmd(CommandArgs args)
        {
            if (args.Player == null || args == null) { return; }
            //清掉原有的图格
            WorldGen.KillTile(Main.spawnTileX, Main.spawnTileY - 3);

            //设置墙壁和图格
            Main.tile[Main.spawnTileX, Main.spawnTileY - 3].wall = WallID.EchoWall;
            Main.tile[Main.spawnTileX, Main.spawnTileY - 2].wall = WallID.EchoWall;
            Main.tile[Main.spawnTileX + 1, Main.spawnTileY - 3].wall = WallID.EchoWall;
            Main.tile[Main.spawnTileX + 1, Main.spawnTileY - 2].wall = WallID.EchoWall;

            Main.tile[Main.spawnTileX, Main.spawnTileY - 3].active(false);
            Main.tile[Main.spawnTileX, Main.spawnTileY - 2].active(false);
            Main.tile[Main.spawnTileX + 1, Main.spawnTileY - 3].active(false);
            Main.tile[Main.spawnTileX + 1, Main.spawnTileY - 2].active(false);

            Main.tile[Main.spawnTileX, Main.spawnTileY - 3].UseBlockColors(new TileColorCache() { Invisible = true });
            Main.tile[Main.spawnTileX, Main.spawnTileY - 2].UseBlockColors(new TileColorCache() { Invisible = true });
            Main.tile[Main.spawnTileX + 1, Main.spawnTileY - 3].UseBlockColors(new TileColorCache() { Invisible = true });
            Main.tile[Main.spawnTileX + 1, Main.spawnTileY - 2].UseBlockColors(new TileColorCache() { Invisible = true });

            //放置
            WorldGen.PlaceSign(Main.spawnTileX, Main.spawnTileY - 3, TileID.Signs, 4);

            //查找空的标志ID
            int newSignID = -1;
            for (int i = 0; i < 1000; i++)
            {
                if (Main.sign[i] == null || Main.sign[i].text == "")
                {
                    Main.sign[i] = new Sign();
                    newSignID = i;
                    break;
                }
            }

            if (newSignID == -1) newSignID = 999;

            //放下标志信息
            Main.sign[newSignID].text = SignInSign.Config.SignText;
            Main.sign[newSignID].x = Main.spawnTileX;
            Main.sign[newSignID].y = Main.spawnTileY - 3;

            //重载配置文件 保存世界并发送出生点坐标
            TShockAPI.Commands.HandleCommand(TSPlayer.Server, "/save");
            TSPlayer.All.SendTileRect((short)Main.spawnTileX, (short)(Main.spawnTileY - 3), 2, 2);
            TShockAPI.Commands.HandleCommand(args.Player, "/reload");
        }
    }

}