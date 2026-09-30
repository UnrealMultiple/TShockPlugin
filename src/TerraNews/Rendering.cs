using System.Globalization;
using System.Text.RegularExpressions;

namespace TerraNews;

// 一行渲染好的聊天内容：文本加上发送时使用的 RGB。
public readonly record struct ChatLine(string Text, byte R, byte G, byte B);

// 解析一行的基础颜色，好让 TShock 真正按 RGB 发送，而不是一律走客户端默认白。
// 只有一个颜色标签且包住整行时（[c/4FC3F7:...]，结尾的 ] 从行尾反向匹配，这样行内
// 夹着的 [i:2451] 不会把它截断），剥掉标签并在本层用其颜色；多个行内标签
// （[c/FFD966:标签] [c/FFFFFF:值]）在括号配对上与物品标签无法区分，故原样透传，
// 交由客户端自行渲染行内颜色——原版也是这么做的。
public static class ChatLineParser
{
    private static readonly Regex ItemTag = new(@"\[i:(\d+)\]", RegexOptions.Compiled);

    public static ChatLine Parse(string? line)
    {
        if (string.IsNullOrEmpty(line))
            return new ChatLine(string.Empty, 255, 255, 255);

        // 不是我们托管的行：按管理员写的样子原样发出。
        if (!line!.StartsWith("[c/", StringComparison.OrdinalIgnoreCase))
            return new ChatLine(line, 255, 255, 255);

        // 颜色标签的格式恰好是 "[c/" + 6 位十六进制 + ":"。
        if (line.Length < 11 || line[9] != ':')
            return new ChatLine(line, 255, 255, 255);

        // 有多个颜色标签：行内颜色交给客户端。
        if (line.IndexOf("[c/", 10, StringComparison.OrdinalIgnoreCase) >= 0)
            return new ChatLine(line, 255, 255, 255);

        // 标签包住整行，所以它的右括号就是行尾那个，中间的都算正文。
        // 后面还有内容就说明这个右括号并没有闭合它。
        int close = line.LastIndexOf(']');
        if (close != line.Length - 1)
            return new ChatLine(line, 255, 255, 255);

        if (!int.TryParse(line.AsSpan(3, 6), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int rgb))
            return new ChatLine(line, 255, 255, 255);

        return new ChatLine(line[10..close],
            (byte)((rgb >> 16) & 0xFF),
            (byte)((rgb >> 8) & 0xFF),
            (byte)(rgb & 0xFF));
    }

    // 把 [i:2451] 改写成 [物品#2451]，让纯文本日志仍然可读。
    public static string ToLogText(string? text) =>
        ItemTag.Replace(text ?? string.Empty, "[物品#$1]");
}
