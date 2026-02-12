using Terraria;

namespace ItemBox;

internal class Utils
{
    public static void GiveItem(int acId, int itemId, int prefix, int stack)
    {
        List<Item> list;
        list = DB.GetUserInfo(acId);
        var item = new Item();
        item.type = itemId;
        item.prefix = (byte) prefix;
        item.stack = stack;
        list.Add(item);
        DB.UpdataInentory(acId, list);
    }

    public static void GiveItem(int acID, List<Item> items)
    {
        List<Item> list;
        list = DB.GetUserInfo(acID);
        items = list.Concat(items).ToList();
        DB.UpdataInentory(acID, items);
    }

    public static List<Item> GetItems(int acID)
    {
        return DB.GetUserInfo(acID);
    }
}