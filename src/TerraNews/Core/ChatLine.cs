using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace TerraNews.Core;

/// <summary>One rendered chat line: the text TShock sends plus the RGB to send it in.</summary>
public readonly record struct ChatLine(string Text, byte R, byte G, byte B)
{
    public static readonly ChatLine White = new(string.Empty, 255, 255, 255);
}

/// <summary>
/// Resolves a chat line's base colour so TShock can send real RGB instead of default white.
/// A single tag wrapping the whole line ([c/4FC3F7:...], closing bracket matched from the end
/// so an [i:2451] inside it does not truncate it) is stripped and applied here; several
/// inline tags ([c/FFD966:label] [c/FFFFFF:value]) cannot be told apart from item tags by
/// bracket matching, so the line passes through and the client renders the colours itself.
/// Either way the payload reaches the player intact.
/// </summary>
public static class ChatLineParser
{
    public static ChatLine Parse(string? line)
    {
        if (string.IsNullOrEmpty(line))
            return ChatLine.White;

        if (!line!.StartsWith("[c/", StringComparison.OrdinalIgnoreCase))
            return new ChatLine(line, 255, 255, 255);

        // A colour tag is exactly "[c/" + 6 hex digits + ":".
        if (line.Length < 11 || line[9] != ':')
            return new ChatLine(line, 255, 255, 255);

        // More than one colour tag on the line: leave the inline colours to the client.
        if (line.IndexOf("[c/", 10, StringComparison.OrdinalIgnoreCase) >= 0)
            return new ChatLine(line, 255, 255, 255);

        // The colour tag is the first thing on the line and wraps the whole line, so its
        // closing bracket is the last one - anything between belongs to the payload.
        int close = line.LastIndexOf(']');
        if (close < 0)
            return new ChatLine(line, 255, 255, 255);

        string hex = line.Substring(3, 6);
        if (!int.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int rgb))
            return new ChatLine(line, 255, 255, 255);

        // Only treat it as a colour tag when the tag wraps the whole line; a suffix after
        // the bracket means the bracket does not close it.
        if (close != line.Length - 1)
            return new ChatLine(line, 255, 255, 255);

        string text = line.Substring(10, close - 10);
        return new ChatLine(text,
            (byte)((rgb >> 16) & 0xFF),
            (byte)((rgb >> 8) & 0xFF),
            (byte)(rgb & 0xFF));
    }

    /// <summary>
    /// Rewrites interactive item tags into something a plain text log can represent.
    /// [i:2451] becomes [物品#2451] so the server log stays readable.
    /// </summary>
    public static string ToLogText(string? text) =>
        Regex.Replace(text ?? string.Empty, @"\[i:(\d+)\]", "[物品#$1]");
}
