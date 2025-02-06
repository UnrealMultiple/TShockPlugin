using Terraria;
using Terraria.ID;
using Terraria.Map;
using TShockAPI;

namespace SpawnInfra;

internal class TileHelper
{
    public static bool isTaskRunning { get; set; }

    public static bool NeedWaitTask(TSPlayer op)
    {
        if (isTaskRunning)
        {
            op?.SendErrorMessage(GetString("另一个创建任务正在执行，请稍后再操作"));
        }
        return isTaskRunning;
    }

    public static void InformPlayers()
    {
        var players = TShock.Players;
        foreach (var tSPlayer in players)
        {
            if (tSPlayer == null || !tSPlayer.Active)
            {
                continue;
            }
            for (var j = 0; j < 255; j++)
            {
                for (var k = 0; k < Main.maxSectionsX; k++)
                {
                    for (var l = 0; l < Main.maxSectionsY; l++)
                    {
                        Netplay.Clients[j].TileSections[k, l] = false;
                    }
                }
            }
        }
    }

    public static void ClearTile(int x, int y, int w = 1, int h = 1)
    {
        for (var i = 0; i < w; i++)
        {
            for (var j = 0; j < h; j++)
            {
                ClearTile(x + i, y + j);
            }
        }
    }

    public static void ClearTile(int x, int y)
    {
        Main.tile[x, y].ClearTile();
        NetMessage.SendTileSquare(-1, x, y, (TileChangeType) 0);
    }

    public static string GetTileName(int type, int style)
    {
        return Lang._mapLegendCache[MapHelper.TileToLookup(type, style)].Value;
    }
}