using System;

namespace TerraNews.Core;

/// <summary>
/// Moon phase name and its effect on fishing power.
///
/// Main.moonPhase is a 0-7 int; the vanilla order runs Full -> waning -> Empty -> waxing.
/// The multipliers are taken verbatim from Player.Fishing_GetPowerMultiplier() in Terraria
/// 1.4.5, which is the only place the game applies the moon bonus, so the news never
/// disagrees with what a player sees in the fishing power tooltip.
/// </summary>
public static class MoonPhases
{
    public const int Count = 8;

    /// <summary>Chinese name of a moon phase index, clamped to the valid 0-7 range.</summary>
    public static string Name(int phase)
    {
        return Wrap(phase) switch
        {
            0 => "满月",
            1 => "亏凸月",
            2 => "下弦月",
            3 => "残月",
            4 => "新月",
            5 => "娥眉月",
            6 => "上弦月",
            _ => "盈凸月"
        };
    }

    /// <summary>
    /// Fishing power multiplier the game applies for this phase.
    /// Full +10%, three quarters +5%, half +0%, quarter -5%, new moon -10%.
    /// </summary>
    public static double FishingMultiplier(int phase)
    {
        return Wrap(phase) switch
        {
            0 => 1.10,
            1 or 7 => 1.05,
            2 or 6 => 1.00,
            3 or 5 => 0.95,
            _ => 0.90
        };
    }

    /// <summary>"钓鱼力 ×1.10" / "钓鱼力 ×0.90" with a plus/minus hint.</summary>
    public static string FishingBonusText(int phase)
    {
        double multiplier = FishingMultiplier(phase);
        int percent = (int)Math.Round((multiplier - 1.0) * 100.0);

        if (percent > 0)
            return $"钓鱼力 ×{multiplier:0.00}（+{percent}%）";
        if (percent < 0)
            return $"钓鱼力 ×{multiplier:0.00}（{percent}%）";
        return $"钓鱼力 ×{multiplier:0.00}";
    }

    /// <summary>Whether this phase is one of the two the Angler likes best (full / new).</summary>
    public static bool IsNotable(int phase)
    {
        int p = Wrap(phase);
        return p == 0 || p == 4;
    }

    private static int Wrap(int phase) => ((phase % Count) + Count) % Count;
}
