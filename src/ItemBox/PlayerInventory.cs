using Terraria;

namespace ItemBox;

public class PlayerInventory
{
    public List<Item> Inventory { get; set; } = new();

    public void Clear()
    {
        this.Inventory.Clear();
    }

    public string GetString()
    {
        var text = "";
        foreach (var item in this.Inventory)
        {
            text = text + item.type + "-" + item.prefix + "-" + item.stack + ",";
        }

        return text;
    }

    public bool Load(string itemString)
    {
        var array = itemString.Split(",");
        foreach (var text in array)
        {
            var array2 = text.Split("-");
            var item = new Item();
            item.type = int.Parse(array2[0]);
            item.prefix = byte.Parse(array2[1]);
            item.stack = byte.Parse(array2[2]);
            this.Inventory.Add(item);
        }

        return true;
    }

    public static List<Item> Prase(string itemString)
    {
        var list = new List<Item>();
        if (string.IsNullOrEmpty(itemString))
        {
            return new List<Item>();
        }

        var array = itemString.Split(",");
        foreach (var text in array)
        {
            var array2 = text.Split("-");
            var item = new Item();
            item.type = int.Parse(array2[0]);
            item.prefix = byte.Parse(array2[1]);
            item.stack = int.Parse(array2[2]);
            list.Add(item);
        }

        return list;
    }

    public static string ToString(List<Item> items)
    {
        var text = "";
        foreach (var item in items)
        {
            text = text + item.type + "-" + item.prefix + "-" + item.stack + ",";
        }

        return text.Substring(0, text.Length - 1);
    }
}