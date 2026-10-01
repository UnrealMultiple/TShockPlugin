namespace TerraNews;

// 月相名称与钓鱼力加成，读取 Main.moonPhase（0-7）。
public static class MoonPhases
{
    private static readonly string[] Names =
    {
        "满月", "亏凸月", "下弦月", "残月", "新月", "娥眉月", "上弦月", "盈凸月"
    };

    // 逐字取自 Terraria 1.4.5 的 Player.Fishing_GetPowerMultiplier()。
    private static readonly double[] Multipliers = { 1.05, 1.05, 1.00, 1.00, 1.00, 1.00, 1.00, 1.05 };

    public static string Name(int phase) => Names[Wrap(phase)];

    public static string FishingBonusText(int phase)
    {
        double multiplier = Multipliers[Wrap(phase)];
        int percent = (int)Math.Round((multiplier - 1.0) * 100);

        if (percent > 0)
            return $"钓鱼力 ×{multiplier:0.00}（+{percent}%）";
        if (percent < 0)
            return $"钓鱼力 ×{multiplier:0.00}（{percent}%）";
        return $"钓鱼力 ×{multiplier:0.00}";
    }

    private static int Wrap(int phase) => ((phase % Names.Length) + Names.Length) % Names.Length;
}
