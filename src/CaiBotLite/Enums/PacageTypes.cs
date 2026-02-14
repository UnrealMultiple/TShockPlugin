namespace CaiBotLite.Enums;

[Serializable]
public enum PackageType
{
    Hello,
    Whitelist,
    PlayerList,
    Progress,
    LookBag,
    WorldFile,
    MapFile,
    MapImage,
    SelfKick,
    CallCommand,
    UnbindServer,
    Heartbeat,
    PluginList,
    RankData,
    ShopCondition,
    ShopBuy,
    Error,
    Unknown
}

public static class PackageTypeExtension
{
    public static Version GetVersion(this PackageType packageType)
    {
        return packageType switch
        {
            PackageType.SelfKick => new Version(2025, 7, 18),
            PackageType.Hello => new Version(2025, 7, 18),
            PackageType.PlayerList => new Version(2025, 7, 18),
            PackageType.Progress => new Version(2025, 7, 18),
            PackageType.LookBag => new Version(2025, 7, 18),
            PackageType.WorldFile => new Version(2025, 7, 18),
            PackageType.MapFile => new Version(2025, 7, 18),
            PackageType.MapImage => new Version(2025, 7, 18),
            PackageType.CallCommand => new Version(2025, 7, 18),
            PackageType.Unknown => new Version(2025, 7, 18),
            PackageType.Whitelist => new Version(2025, 7, 18),
            PackageType.UnbindServer => new Version(2025, 7, 25),
            PackageType.Heartbeat => new Version(2025, 7, 25),
            PackageType.RankData => new Version(2025, 7, 25),
            PackageType.PluginList => new Version(2025, 7, 25),
            PackageType.ShopBuy => new Version(2025, 7, 25),
            PackageType.ShopCondition => new Version(2025, 7, 25),
            PackageType.Error => new Version(2026, 2, 14),
            _ => new Version(2007, 5, 24)
        };
    }
}