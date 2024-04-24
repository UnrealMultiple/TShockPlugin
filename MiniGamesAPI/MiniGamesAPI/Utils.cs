using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using TShockAPI;

namespace MiniGamesAPI
{
    public static class Utils
    {
        public static readonly string EndLine_10 = new string('\n', 10);

        public static readonly string EndLine_15 = new string('\n', 15);

        public static readonly string EndLine_8 = new string('\n', 8);

        public static int DropItem(float x, float y, int netid, int stack, byte prefix)
        {
            Item itemById = TShock.Utils.GetItemById(netid);
            int num = Item.NewItem((IEntitySource)new EntitySource_DebugCommand(), (int)x, (int)y, ((Entity)itemById).width, ((Entity)itemById).height, netid, stack, false, (int)prefix, false, false);
            TSPlayer.All.SendData((PacketTypes)21, "", num, 0f, 0f, 0f, 0);
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
            if (main.X == first.X && main.Y + 1 == first.Y)
            {
                return true;
            }
            if (main.X + 1 == first.X && main.Y + 1 == first.Y)
            {
                return true;
            }
            return false;
        }

        public static void ClearItem(int id)
        {
            ((Entity)Main.item[id]).active = false;
            NetMessage.SendData(21, -1, -1, (NetworkText)null, id, 0f, 0f, 0f, 0, 0, 0);
        }

        public static IEnumerable<float> TopTen(List<float> records)
        {
            return records.OrderByDescending((float p) => p);
        }

        public static void ClearRangeItem(Vector2 position, int id, int radius)
        {
            for (int i = 0; i < 400; i++)
            {
                Item val = Main.item[i];
                float num = ((Entity)val).position.X - position.X;
                float num2 = ((Entity)val).position.Y - position.Y;
                if (((Entity)val).active && val.netID == id && num * num + num2 * num2 <= (float)(radius * radius) * 256f)
                {
                    ((Entity)Main.item[i]).active = false;
                    TSPlayer.All.SendData((PacketTypes)21, "", i, 0f, 0f, 0f, 0);
                }
            }
        }
    }
}
