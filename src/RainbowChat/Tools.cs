using Microsoft.Xna.Framework;
using System.Reflection;
using System.Text;
using Terraria;
using Terraria.ID;

namespace RainbowChat;

public static class Tools
{

    #region 渐变着色方法 + 物品图标解析
    public static string TextGradient(string text, Color color, Color otherColor)
    {
        // 创建一个字符串构建器来构建最终的字符串
        var Text = new StringBuilder();
        var index = 0;

        // 获取文本长度
        var textLength = text.Length;
        while (index < textLength)
        {
            // 获取当前字符
            var c = text[index];

            // 检查是否是图标标签的开始
            if (c == '[' && index < textLength - 1 && text[index + 1] == 'i')
            {
                // 寻找对应图标标签的结束位置
                var end = text.IndexOf(']', index);
                if (end != -1)
                {
                    // 提取图标标签
                    var tag = text.Substring(index, end - index + 1);

                    // 检查是否是有效的图标标签
                    if (tag.StartsWith("[i/") || tag.StartsWith("[i:"))
                    {
                        // 提取标签内的内容
                        var content = tag[3..^1];
                        int itemID;

                        // 检查是否包含冒号
                        if (content.Contains(':'))
                        {
                            // 按冒号分割
                            var parts = content.Split(':');

                            // 尝试解析物品ID
                            if (int.TryParse(parts[0], out itemID))
                            {
                                // 插入图标
                                Text.Append(ItemIcon(itemID));

                                // 移动索引到标签后
                                index = end + 1;
                            }
                            else
                            {
                                // 如果解析失败，则保留原标签
                                Text.Append(tag);
                                index = end + 1;
                            }
                        }

                        // 直接解析整个内容为物品ID
                        else if (int.TryParse(content, out itemID))
                        {
                            // 插入图标
                            Text.Append(ItemIcon(itemID));
                            index = end + 1;
                        }
                        else
                        {
                            // 如果解析失败，则保留原标签
                            Text.Append(tag);
                            index = end + 1;
                        }
                    }
                    else
                    {
                        // 如果不是认识的图标标签，则保留原标签
                        Text.Append(tag);
                        index = end + 1;
                    }
                }
                else
                {
                    // 如果没有找到闭合的括号，则保留当前字符
                    Text.Append(c);
                    index++;
                }
            }
            else
            {
                // 计算当前字符的位置比例
                var ratio = (float) index / (textLength - 1);

                // 根据比例插值颜色
                var thisColor = Color.Lerp(color, otherColor, ratio);

                // 添加带颜色的字符
                Text.Append($"[c/{thisColor.Hex3()}:{c}]");

                // 移动索引
                index++;
            }
        }

        // 返回构建后的字符串
        return Text.ToString();
    }
    #endregion

    #region 返回物品图标方法
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
    #endregion

    #region 原随机色方法（实在不想改了 - - 因为感觉不好看 压根就没用过）
    public static List<Color> GetColors()
    {
        var list = new List<Color>();
        var properties = typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.Public);
        foreach (var propertyInfo in properties)
        {
            if (propertyInfo.PropertyType == typeof(Color) && propertyInfo.CanRead)
            {
                list.Add((Color) propertyInfo.GetValue(null)!);
            }
        }
        return list;
    }
    #endregion
}