using Terraria;
using TShockAPI;

namespace VeinMiner;

public static class Expansion
{
    public static int GetBlankSlot(this TSPlayer tsp)
    {
        var num = 0;
        tsp.TPlayer.inventory.ForEach(s => { if (s.netID == 0) { num++; } });
        return num;
    }

    public static bool IsSpaceEnough(this TSPlayer tsp, int id, int stack)
    {
        var available = 0;
        var item = new Item();
        item.SetDefaults(id);
        Item s;
        for (var i = 0; i < 50; i++)
        {
            s = tsp.TPlayer.inventory[i];
            if (available < stack)
            {
                if (s.netID == id)
                {
                    available += s.maxStack - s.stack;
                }
                else if (s.netID == 0)
                {
                    available += item.maxStack;
                }
            }
            else
            {
                break;
            }
        }
        return available >= stack;
    }
}