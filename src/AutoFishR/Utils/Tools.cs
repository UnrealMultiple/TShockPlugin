using System.Globalization;
using System.Text;
using Terraria;
using TShockAPI;

namespace AutoFish.Utils;

public class Tools
{
    /// <summary>
    ///     检查指定玩家是否有任意活跃的浮漂。
    /// </summary>
    public static bool BobbersActive(int whoAmI)
    {
        return Main.projectile.Any(p => p.active && p.owner == whoAmI && p.bobber);
    }

    /// <summary>
    ///     将当前使用的贵重鱼饵与背包中最末尾的可用鱼饵交换，以避免消耗贵重鱼饵。
    /// </summary>
    /// <returns>是否发生交换。</returns>
    public static bool TrySwapValuableBaitToBack(TSPlayer player, int baitType, ICollection<int> valuableBaitIds,
        out int currentSlot, out int targetSlot, out int currentItemType, out int targetItemType)
    {
        currentSlot = -1;
        targetSlot = -1;
        currentItemType = 0;
        targetItemType = 0;

        if (player?.TPlayer?.inventory is null) return false;
        var inv = player.TPlayer.inventory;

        for (var i = 0; i < inv.Length; i++)
        {
            if (inv[i].bait <= 0 || inv[i].type != baitType) continue;
            currentSlot = i;
            currentItemType = inv[i].type;
            break;
        }

        if (currentSlot == -1) return false;

        // 优先选择末尾的非贵重鱼饵，找不到时才回退到任何末尾鱼饵。
        for (var i = inv.Length - 1; i >= 0; i--)
        {
            if (inv[i].bait <= 0) continue;
            if (valuableBaitIds.Contains(inv[i].type)) continue;
            targetSlot = i;
            targetItemType = inv[i].type;
            break;
        }

        if (targetSlot == -1)
            for (var i = inv.Length - 1; i >= 0; i--)
            {
                if (inv[i].bait <= 0 || i == currentSlot) continue;
                targetSlot = i;
                targetItemType = inv[i].type;
                break;
            }

        if (targetSlot == -1 || targetSlot == currentSlot) return false;

        (inv[currentSlot], inv[targetSlot]) = (inv[targetSlot], inv[currentSlot]);
        return true;
    }

    /// <summary>
    ///     给玩家发送一个简单的渐变色消息。
    /// </summary>
    public static void SendGradientMessage(TSPlayer player, string text, string startHex = "F3A6FF",
        string endHex = "7CC7FF")
    {
        if (player == null || string.IsNullOrEmpty(text)) return;

        var (sr, sg, sb) = ParseHex(startHex);
        var (er, eg, eb) = ParseHex(endHex);

        var len = text.Length;
        var sbMsg = new StringBuilder(len * 12);
        for (var i = 0; i < len; i++)
        {
            var t = len <= 1 ? 0f : (float)i / (len - 1);
            var r = (int)MathF.Round(sr + (er - sr) * t);
            var g = (int)MathF.Round(sg + (eg - sg) * t);
            var b = (int)MathF.Round(sb + (eb - sb) * t);
            sbMsg.Append("[c/");
            sbMsg.Append(r.ToString("X2"));
            sbMsg.Append(g.ToString("X2"));
            sbMsg.Append(b.ToString("X2"));
            sbMsg.Append(":");
            sbMsg.Append(text[i]);
            sbMsg.Append(']');
        }

        player.SendInfoMessage(sbMsg.ToString());
    }

    private static (int r, int g, int b) ParseHex(string hex)
    {
        if (string.IsNullOrWhiteSpace(hex)) return (255, 255, 255);
        hex = hex.TrimStart('#');
        if (hex.Length is not 6) return (255, 255, 255);
        var r = int.Parse(hex.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        var g = int.Parse(hex.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        var b = int.Parse(hex.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        return (r, g, b);
    }

    /// <summary>
    ///     根据在线玩家数量动态调整钓鱼次数上限。
    /// </summary>
    public static int GetLimit(int plrs)
    {
        return plrs <= 5 ? 100 : plrs <= 10 ? 50 : plrs <= 20 ? 25 : 10;
    }
}