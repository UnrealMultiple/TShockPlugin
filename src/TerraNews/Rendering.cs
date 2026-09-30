using System.Globalization;
using System.Text.RegularExpressions;

namespace TerraNews;

/// <summary>One rendered chat line: the text to send plus the RGB to send it in.</summary>
public readonly record struct ChatLine(string Text, byte R, byte G, byte B);

/// <summary>
/// Resolves a chat line's base colour so TShock can send real RGB instead of default white.
/// A single tag wrapping the whole line ([c/4FC3F7:...], closing bracket matched from the end
/// so an [i:2451] inside it does not truncate it) is stripped and applied here; several
/// inline tags ([c/FFD966:label] [c/FFFFFF:value]) cannot be told apart from item tags by
/// bracket matching, so the line passes through and the client renders the colours itself.
/// </summary>
public static class ChatLineParser
{
    private static readonly Regex ItemTag = new(@"\[i:(\d+)\]", RegexOptions.Compiled);

    public static ChatLine Parse(string? line)
    {
        if (string.IsNullOrEmpty(line))
            return new ChatLine(string.Empty, 255, 255, 255);

        // Not a line we own: leave it exactly as the admin wrote it.
        if (!line!.StartsWith("[c/", StringComparison.OrdinalIgnoreCase))
            return new ChatLine(line, 255, 255, 255);

        // A colour tag is exactly "[c/" + 6 hex digits + ":".
        if (line.Length < 11 || line[9] != ':')
            return new ChatLine(line, 255, 255, 255);

        // More than one colour tag: leave the inline colours to the client.
        if (line.IndexOf("[c/", 10, StringComparison.OrdinalIgnoreCase) >= 0)
            return new ChatLine(line, 255, 255, 255);

        // The tag wraps the whole line, so its closing bracket is the last one and anything
        // between belongs to the payload. A suffix after it means it does not close here.
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

    /// <summary>Rewrites [i:2451] to [物品#2451] so a plain text log stays readable.</summary>
    public static string ToLogText(string? text) =>
        ItemTag.Replace(text ?? string.Empty, "[物品#$1]");
}
