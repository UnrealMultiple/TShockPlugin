using Terraria;
using TShockAPI;

namespace LazyAPI.Extensions;
public static class ItemExtension
{
    public static void Send(this Item item)
    {
        var tuple = LazyAPI.Utility.Utils.FindItemRef(item);
        if (tuple == null)
        {
            return;
        }

        TShock.Players[tuple.Item1].SendData(PacketTypes.PlayerSlot, "", tuple.Item1, tuple.Item2);
    }
}
