using Terraria;
using Terraria.ID;
using TShockAPI;

namespace CreateSpawn;

internal class TileHelper
{
    public static bool IsTaskRunning { get; set; }

    public static void StartGen()
    {
        IsTaskRunning = true;
    }

    public static void FinishGen()
    {
        IsTaskRunning = false;
        TShock.Utils.SaveWorld();
    }

    public static bool NeedWaitTask(TSPlayer op)
    {
        if (IsTaskRunning)
        {
            op?.SendErrorMessage(GetString("另一个创建任务正在执行，请稍后再操作"));
        }

        return IsTaskRunning;
    }

    public static void ClearEverything(int x, int y)
    {
        Main.tile[x, y].ClearEverything();
        NetMessage.SendTileSquare(-1, x, y, TileChangeType.None);
    }


    public static void InformPlayers()
    {
        for (var j = 0; j < 255; j++)
        {
            if (!Netplay.Clients[j].IsActive)
            {
                continue;
            }

            for (var k = 0; k < Main.maxSectionsX; k++)
            {
                for (var l = 0; l < Main.maxSectionsY; l++)
                {
                    Netplay.Clients[j].TileSections[k, l] = false;
                }
            }
        }
    }

    public static void GenAfter()
    {
        InformPlayers();
        FinishGen();
    }
}
