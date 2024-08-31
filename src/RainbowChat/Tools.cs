using Microsoft.Xna.Framework;
using System.Text;
using Terraria;
using Terraria.ID;

namespace RainbowChat;

public static class Tools
{
    // 方法：TextColor，给定文本字符串和指定颜色，返回包裹了颜色代码的格式化字符串
    public static string TextColor(string text, Color color)
    {
        return $"[c/{color.Hex3()}:{text}]";
    }

    // 方法：TextGradient，给定文本字符串、起始颜色和结束颜色，返回包裹了渐变颜色代码的格式化字符串
    public static string TextGradient(string text, Color color, Color otherColor)
    {
        StringBuilder gradientedText = new();

        var index = 0;
        foreach (var c in text)
        {
            var ratio = (float) index / (text.Length - 1);
            var thisColor = Color.Lerp(color, otherColor, ratio);
            gradientedText.Append($"[c/{thisColor.Hex3()}:{c}]");
            index++;
        }
        return gradientedText.ToString();
    }

    // 方法：ItemIcon，根据给定的物品对象返回插入物品图标的格式化字符串
    public static string ItemIcon(Item item)
    {
        return ItemIcon(item.type);
    }

    // 方法：ItemIcon，根据给定的物品ID返回插入物品图标的格式化字符串
    public static string ItemIcon(ItemID itemID)
    {
        return ItemIcon(itemID);
    }

    // 方法：ItemIcon，根据给定的物品整型ID返回插入物品图标的格式化字符串
    public static string ItemIcon(int itemID)
    {
        return $"[i:{itemID}]";
    }
}