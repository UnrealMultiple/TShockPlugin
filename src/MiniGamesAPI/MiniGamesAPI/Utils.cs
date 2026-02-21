using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using TShockAPI;

namespace MiniGamesAPI;

public static class Utils
{
    public static readonly string EndLine_10 = new string('\n', 10);

    public static readonly string EndLine_15 = new string('\n', 15);

    public static readonly string EndLine_8 = new string('\n', 8);

    public static int DropItem(float x, float y, int netid, int stack, byte prefix)
    {
        var itemById = TShock.Utils.GetItemById(netid);
        var num = Item.NewItem(new EntitySource_DebugCommand(), (int) x, (int) y, itemById.width, itemById.height, netid, stack, false, prefix, false);
        TSPlayer.All.SendData((PacketTypes) 21, "", num, 0f, 0f, 0f, 0);
        return num;
    }

    public static bool CompareOneAndTwo(Point first, Point main)
    {
        if (main.X - 1 == first.X && main.Y - 1 == first.Y)
        {
            return true;
        }
        if (main.X == first.X && main.Y - 1 == first.Y)
        {
            return true;
        }
        if (main.X + 1 == first.X && main.Y - 1 == first.Y)
        {
            return true;
        }
        if (main.X - 1 == first.X && main.Y == first.Y)
        {
            return true;
        }
        if (main.X == first.X && main.Y == first.Y)
        {
            return true;
        }
        if (main.X + 1 == first.X && main.Y == first.Y)
        {
            return true;
        }
        if (main.X - 1 == first.X && main.Y + 1 == first.Y)
        {
            return true;
        }
        return main.X == first.X && main.Y + 1 == first.Y ? true : main.X + 1 == first.X && main.Y + 1 == first.Y;
    }

    public static void ClearItem(int id)
    {
        Main.item[id].TurnToAir();
        NetMessage.SendData(21, -1, -1, null, id, 0f, 0f, 0f, 0, 0, 0);
    }

    public static IEnumerable<float> TopTen(List<float> records)
    {
        return records.OrderByDescending((float p) => p);
    }

    public static void ClearRangeItem(Vector2 position, int id, int radius)
    {
        for (var i = 0; i < 400; i++)
        {
            var val = Main.item[i];
            var num = val.position.X - position.X;
            var num2 = val.position.Y - position.Y;
            if (val.active && val.type == id && (num * num) + (num2 * num2) <= radius * radius * 256f)
            {
                Main.item[i].TurnToAir();
                TSPlayer.All.SendData((PacketTypes) 21, "", i, 0f, 0f, 0f, 0);
            }
        }
    }
}