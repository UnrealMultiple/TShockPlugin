using Terraria;
using Terraria.ID;
using TShockAPI;

namespace SignInSign;

internal class Command
{
    internal static void SetupCmd(CommandArgs args)
    {
        //获取玩家当前坐标
        var x = args.Player.TileX;
        var y = args.Player.TileY;

        var message = GetString("[告示牌登录]请输入以下指令：\n重置告示牌：[c/42B2CE:/gs r]\n设置传送点：[c/F25E61:/gs s]");

        if (args.Parameters.Count == 0)
        {
            args.Player.SendMessage(message, Microsoft.Xna.Framework.Color.YellowGreen);
            return;
        }

        switch (args.Parameters[0].ToLower())
        {
            case "r":
            case "reset":
            case "reload":
                if (args.Player.HasPermission("signinsign.setup"))
                {
                    ReloadCmd(args);
                    if (SignInSign.Config.Teleport_X > 0 && SignInSign.Config.Teleport_Y > 0)
                    {
                        SignInSign.Config.Teleport_X = 0;
                        SignInSign.Config.Teleport_Y = 0;
                        SignInSign.Config.Write(Configuration.ConfigPath);
                    }
                }
                return;
            case "s":
            case "set":
                if (args.Parameters.Count != 1)
                {
                    args.Player.SendMessage(GetString("[告示牌登录]设置传送点命令无需额外参数，将会使用你当前位置。"), Microsoft.Xna.Framework.Color.Yellow);
                }
                else if (args.Parameters.Count == 1 && (args.Player.HasPermission("signinsign.tp") || args.Player.HasPermission("signinsign.setup"))) //加个玩家设置TP的权限
                {
                    SignInSign.Config.Teleport_X = x;
                    SignInSign.Config.Teleport_Y = y;
                    args.Player.SendMessage(GetString($"已将你所在的位置设置为[c/9487D6:告示牌传送点]，坐标为({x}, {y})"), Microsoft.Xna.Framework.Color.Yellow);
                    Console.WriteLine(GetString($"【告示牌登录】传送点已设置，坐标为({x}, {y})"), Microsoft.Xna.Framework.Color.Yellow);

                    // 确保配置更改被保存
                    SignInSign.Config.Write(Configuration.ConfigPath);
                }
                break;
            default:
                args.Player.SendMessage(message, Microsoft.Xna.Framework.Color.YellowGreen);
                return;
        }
    }

    private static void ReloadCmd(CommandArgs args)
    {
        if (args.Player == null || args == null) { return; }
        //清掉原有的图格
        WorldGen.KillTile(Main.spawnTileX, Main.spawnTileY - 3);

        //检查墙壁是否为空，空则放置回声墙
        if (Main.tile[Main.spawnTileX, Main.spawnTileY - 3].wall == WallID.None)
        {
            Main.tile[Main.spawnTileX, Main.spawnTileY - 3].wall = WallID.EchoWall;
            Main.tile[Main.spawnTileX, Main.spawnTileY - 2].wall = WallID.EchoWall;
            Main.tile[Main.spawnTileX + 1, Main.spawnTileY - 3].wall = WallID.EchoWall;
            Main.tile[Main.spawnTileX + 1, Main.spawnTileY - 2].wall = WallID.EchoWall;
        }

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
        var newSignID = -1;
        for (var i = 0; i < 1000; i++)
        {
            if (Main.sign[i] == null || Main.sign[i].text == "")
            {
                Main.sign[i] = new Sign();
                newSignID = i;
                break;
            }
        }

        if (newSignID == -1)
        {
            newSignID = 999;
        }

        //放下标志信息
        Main.sign[newSignID].text = SignInSign.Config.SignText;
        Main.sign[newSignID].x = Main.spawnTileX;
        Main.sign[newSignID].y = Main.spawnTileY - 3;

        //重载配置文件 保存世界并发送出生点坐标
        TShockAPI.Commands.HandleCommand(TSPlayer.Server, "/save");
        TSPlayer.All.SendTileRect((short) Main.spawnTileX, (short) (Main.spawnTileY - 3), 2, 2);
        TShockAPI.Commands.HandleCommand(args.Player, "/reload");
    }
}