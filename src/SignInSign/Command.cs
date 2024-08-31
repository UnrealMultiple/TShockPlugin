using Terraria;
using Terraria.ID;
using TShockAPI;

namespace SignInSign;

internal class Command
{
    internal static void SetupCmd(CommandArgs args)
    {
        //��ȡ��ҵ�ǰ����
        var x = args.Player.TileX;
        var y = args.Player.TileY;

        const string Message = $"[��ʾ�Ƶ�¼]����������ָ�\n���ø�ʾ�ƣ�[c/42B2CE:/gs r]\n���ô��͵㣺[c/F25E61:/gs s]";

        if (args.Parameters.Count == 0)
        {
            args.Player.SendMessage(Message, Microsoft.Xna.Framework.Color.YellowGreen);
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
                    args.Player.SendMessage("[��ʾ�Ƶ�¼]���ô��͵���������������������ʹ���㵱ǰλ�á�", Microsoft.Xna.Framework.Color.Yellow);
                }
                else if (args.Parameters.Count == 1 && (args.Player.HasPermission("signinsign.tp") || args.Player.HasPermission("signinsign.setup"))) //�Ӹ��������TP��Ȩ��
                {
                    SignInSign.Config.Teleport_X = x;
                    SignInSign.Config.Teleport_Y = y;
                    args.Player.SendMessage($"�ѽ������ڵ�λ������Ϊ[c/9487D6:��ʾ�ƴ��͵�]������Ϊ({x}, {y})", Microsoft.Xna.Framework.Color.Yellow);
                    Console.WriteLine($"����ʾ�Ƶ�¼�����͵������ã�����Ϊ({x}, {y})", Microsoft.Xna.Framework.Color.Yellow);

                    // ȷ�����ø��ı�����
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
        //���ԭ�е�ͼ��
        WorldGen.KillTile(Main.spawnTileX, Main.spawnTileY - 3);

        //���ǽ���Ƿ�Ϊ�գ�������û���ǽ
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

        //����
        WorldGen.PlaceSign(Main.spawnTileX, Main.spawnTileY - 3, TileID.Signs, 4);

        //���ҿյı�־ID
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

        //���±�־��Ϣ
        Main.sign[newSignID].text = SignInSign.Config.SignText;
        Main.sign[newSignID].x = Main.spawnTileX;
        Main.sign[newSignID].y = Main.spawnTileY - 3;

        //���������ļ� �������粢���ͳ���������
        TShockAPI.Commands.HandleCommand(TSPlayer.Server, "/save");
        TSPlayer.All.SendTileRect((short) Main.spawnTileX, (short) (Main.spawnTileY - 3), 2, 2);
        TShockAPI.Commands.HandleCommand(args.Player, "/reload");
    }
}