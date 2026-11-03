using System.Globalization;
using System.Text.RegularExpressions;
using TShockAPI;

namespace TerraNews;

// 一行渲染好的聊天内容：文本加上发送时使用的 RGB。
public readonly record struct ChatLine(string Text, byte R, byte G, byte B);

// 解析一行的基础颜色，好让 TShock 真正按 RGB 发送，而不是一律走客户端默认白。
//
// 客户端用一条正则从头扫到尾：
//     \[(?<tag>[a-zA-Z]{1,10})(\/(?<options>[^:]+))?:(?<text>.+?)(?<!\\)\]
// text 组是非贪婪的，所以颜色标签只会吞到它后面的第一个 ] 。这意味着
// [c/FFFFFF:[i:2451]] 里的物品标签会被颜色标签当成正文吃掉，图标就再也不会
// 变成图标，而是原样显示成 [i:2451] 这串字符。
//
// 因此规则只有一条：含物品标签的行只能用「一个颜色标签包住整行」这种写法。
// 这种行在这里就被剥掉标签、改用真正的 RGB 发出去，客户端收到的是不含任何
// 标签的纯文本，物品图标正常渲染。行内多段颜色的写法留给不含物品标签的行，
// 那时原样透传给客户端由它自己渲染，与原版行为一致。
public static class ChatLineParser
{
    private static readonly Regex ItemTag = new(@"\[i:(\d+)\]", RegexOptions.Compiled);

    // 同一个模板只提醒一次，免得刷屏。
    private static readonly HashSet<string> Warned = new(StringComparer.Ordinal);

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

        // 有多个颜色标签：行内颜色交给客户端，但物品标签会因此失效，得提醒一声。
        if (line.IndexOf("[c/", 10, StringComparison.OrdinalIgnoreCase) >= 0)
        {
            WarnAboutItemTag(line);
            return new ChatLine(line, 255, 255, 255);
        }

        // 单个标签包住整行，剥掉它并用它的颜色发送。它的右括号就是行尾那个，
        // 中间的都算正文。
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

    // 占位符可能整段变空（功能被关掉、当天没货、旅商货架是空的），于是拼出一串
    // "泰拉日报：满月；渔夫；；旅商："。这里按「；」切成段，丢掉只剩店名没有货的残段
    // 和空段，再重新接上，所以不会留下悬空的分号。
    //
    // 必须在剥掉颜色标签之后调用：模板开头那半个标签正好以「：」收尾，先清就会把
    // 颜色标签本身当成残段丢掉。行内还留着颜色标签的写法（Parse 会原样透传，那种
    // 写法本来就配不了物品图标）就整行跳过，免得把标签拆散。
    //
    // 代价：段末以全角冒号收尾的正文会被当成残段丢掉。默认模板不会写出这种段，
    // 自定义模板若确实需要，记得在冒号后面接点东西。
    public static string TidySeparators(string? text)
    {
        text ??= string.Empty;
        if (text.Contains("[c/", StringComparison.OrdinalIgnoreCase))
            return text;

        string[] parts = text.Split('；');
        var kept = new List<string>(parts.Length);

        foreach (string part in parts)
        {
            string trimmed = part.Trim();
            if (trimmed.Length == 0 || trimmed.EndsWith('：'))
                continue;
            kept.Add(trimmed);
        }

        return string.Join('；', kept);
    }

    private static void WarnAboutItemTag(string line)
    {
        if (!line.Contains("[i:", StringComparison.Ordinal) || !Warned.Add(line))
            return;

        TShock.Log.Warn($"[TerraNews] 模板 {line} 把物品标签 [i:ID] 放在了颜色标签里面，"
            + "客户端会把它当纯文本显示。请改成「一个颜色标签包住整行」的写法，例如 "
            + "[c/FFD966:任务鱼 {icon}]。");
    }
}
