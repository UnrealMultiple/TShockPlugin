using Terraria;
using Terraria.ID;
using TShockAPI;

namespace SignInSign;

public static class Utils
{
    public static int GetSignIdByPos(int tileX, int tileY)
    {
        for (var i = 0; i < Main.sign.Length; i++)
        {
            if (Main.sign[i] != null && Main.sign[i].x == tileX && Main.sign[i].y == tileY)
            {
                return i;
            }
        }
        Console.WriteLine(GetString($"未在位置({tileX}, {tileY})找到告示牌。"));
        return -1;
    }

    public static int SpawnSign(int x, int y)
    {
        WorldGen.KillTile(x, y);
        if (Main.tile[x, y].wall == WallID.None) //检查墙壁是否为空，空则放置隐形墙
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

        WorldGen.PlaceSign(Main.spawnTileX, Main.spawnTileY - 3, TileID.Signs, 4);
        WorldGen.PlaceObject(x, y, TileID.Signs, false, 4, -1, -1);

        //查找空的标志ID
        var newSignID = 999;
        for (var i = 0; i < 1000; i++)
        {
            if (Main.sign[i] == null || Main.sign[i].text == "")
            {
                Main.sign[i] = new Sign();
                newSignID = i;
                break;
            }
        }
        //放下标志信息
        Main.sign[newSignID].text = SignInSign.Config.SignText;
        Main.sign[newSignID].x = x;
        Main.sign[newSignID].y = y;
        TSPlayer.All.SendTileRect((short) x, (short) y);
        return newSignID;
    }

    public static string ReadPassword(string text)
    {
        return string.IsNullOrEmpty(text)
            ? throw new ArgumentException(GetString("输入的密码不能为空"))
            : text[SignInSign.Config.SignText.Length..].Trim();
    }
}