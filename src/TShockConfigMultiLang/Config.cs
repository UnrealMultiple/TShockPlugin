using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;
using Rests;
using TShockAPI;

namespace TShockConfigMultiLang;


[Config]
public class Config : JsonConfigBase<Config>
{
    protected override string Filename => "config";

    #region ServerSettings

    // 服务器密码
    [LocalizedPropertyName(CultureType.Chinese, "服务器密码")]
    [LocalizedPropertyName(CultureType.English, "ServerPassword")]
    public string ServerPassword = TShock.Config.Settings.ServerPassword;

    // 服务器端口
    [LocalizedPropertyName(CultureType.Chinese, "服务器端口")]
    [LocalizedPropertyName(CultureType.English, "ServerPort")]
    public int ServerPort = TShock.Config.Settings.ServerPort;

    // 服务器人数上限
    [LocalizedPropertyName(CultureType.Chinese, "服务器人数上限")]
    [LocalizedPropertyName(CultureType.English, "MaxSlots")]
    public int MaxSlots = TShock.Config.Settings.MaxSlots;

    // 服务器人满预留位
    [LocalizedPropertyName(CultureType.Chinese, "服务器人满预留位")]
    [LocalizedPropertyName(CultureType.English, "ReservedSlots")]
    public int ReservedSlots = TShock.Config.Settings.ReservedSlots;

    // 服务器名称
    [LocalizedPropertyName(CultureType.Chinese, "服务器名称")]
    [LocalizedPropertyName(CultureType.English, "ServerName")]
    public string ServerName = TShock.Config.Settings.ServerName;

    // 是否使用服务器名称替代世界名称
    [LocalizedPropertyName(CultureType.Chinese, "是否使用服务器名称")]
    [LocalizedPropertyName(CultureType.English, "UseServerName")]
    public bool UseServerName = TShock.Config.Settings.UseServerName;

    // 服务器日志存放路径
    [LocalizedPropertyName(CultureType.Chinese, "服务器日志存放路径")]
    [LocalizedPropertyName(CultureType.English, "LogPath")]
    public string LogPath = TShock.Config.Settings.LogPath;

    // 是否启用报错日志
    [LocalizedPropertyName(CultureType.Chinese, "是否启用报Debug日志")]
    [LocalizedPropertyName(CultureType.English, "DebugLogs")]
    public bool DebugLogs = TShock.Config.Settings.DebugLogs;

    // 禁用加入后登录
    [LocalizedPropertyName(CultureType.Chinese, "禁用登录后进入")]
    [LocalizedPropertyName(CultureType.English, "DisableLoginBeforeJoin")]
    public bool DisableLoginBeforeJoin = TShock.Config.Settings.DisableLoginBeforeJoin;

    // 允许箱子中物品堆栈超出限制
    [LocalizedPropertyName(CultureType.Chinese, "允许箱子中物品堆叠超出限制")]
    [LocalizedPropertyName(CultureType.English, "IgnoreChestStacksOnLoad")]
    public bool IgnoreChestStacksOnLoad = TShock.Config.Settings.IgnoreChestStacksOnLoad;

    // 世界图格提供器 (heaptile/constileation)
    [LocalizedPropertyName(CultureType.Chinese, "世界图格提供器")]
    [LocalizedPropertyName(CultureType.English, "WorldTileProvider")]
    public string WorldTileProvider = TShock.Config.Settings.WorldTileProvider;

    #endregion

    #region Backup and Save Settings

    // 启用自动保存
    [LocalizedPropertyName(CultureType.Chinese, "启用自动保存")]
    [LocalizedPropertyName(CultureType.English, "AutoSave")]
    public bool AutoSave = TShock.Config.Settings.AutoSave;

    // 自动保存世界时广播提示
    [LocalizedPropertyName(CultureType.Chinese, "自动保存世界广播提示")]
    [LocalizedPropertyName(CultureType.English, "AnnounceSave")]
    public bool AnnounceSave = TShock.Config.Settings.AnnounceSave;

    // 是否显示备份自动保存消息。
    [LocalizedPropertyName(CultureType.Chinese, "是否显示备份自动保存消息")]
    [LocalizedPropertyName(CultureType.English, "ShowBackupAutosaveMessages")]
    public bool ShowBackupAutosaveMessages = TShock.Config.Settings.ShowBackupAutosaveMessages;

    // 备份间隔(分钟)
    [LocalizedPropertyName(CultureType.Chinese, "自动备份间隔")]
    [LocalizedPropertyName(CultureType.English, "BackupInterval")]
    public int BackupInterval = TShock.Config.Settings.BackupInterval;

    // 备份保留时间(分钟)
    [LocalizedPropertyName(CultureType.Chinese, "备份保留时间")]
    [LocalizedPropertyName(CultureType.English, "BackupKeepFor")]
    public int BackupKeepFor = TShock.Config.Settings.BackupKeepFor;

    // 崩溃时是否保存世界
    [LocalizedPropertyName(CultureType.Chinese, "崩溃时保存世界")]
    [LocalizedPropertyName(CultureType.English, "SaveWorldOnCrash")]
    public bool SaveWorldOnCrash = TShock.Config.Settings.SaveWorldOnCrash;

    // 最后一名玩家退出时是否保存世界
    [LocalizedPropertyName(CultureType.Chinese, "最后玩家退出时保存世界")]
    [LocalizedPropertyName(CultureType.English, "SaveWorldOnLastPlayerExit")]
    public bool SaveWorldOnLastPlayerExit = TShock.Config.Settings.SaveWorldOnLastPlayerExit;

    #endregion

    #region World Settings

    // 入侵事件的大小，由公式 //(100 + 乘数*血量>200的在线玩家)
    [LocalizedPropertyName(CultureType.Chinese, "事件入侵乘数")]
    [LocalizedPropertyName(CultureType.English, "InvasionMultiplier")]
    public int InvasionMultiplier = TShock.Config.Settings.InvasionMultiplier;

    // 默认刷怪生成上限
    [LocalizedPropertyName(CultureType.Chinese, "默认刷怪生成上限")]
    [LocalizedPropertyName(CultureType.English, "DefaultMaximumSpawns")]
    public int DefaultMaximumSpawns = TShock.Config.Settings.DefaultMaximumSpawns;

    // 刷怪间隔(数值越低，敌人生成越频繁)
    [LocalizedPropertyName(CultureType.Chinese, "默认刷怪率")]
    [LocalizedPropertyName(CultureType.English, "DefaultSpawnRate")]
    public int DefaultSpawnRate = TShock.Config.Settings.DefaultSpawnRate;

    // 是否启用无限制入侵事件(仍需手动触发) (/invade)
    [LocalizedPropertyName(CultureType.Chinese, "事件无限入侵")]
    [LocalizedPropertyName(CultureType.English, "InfiniteInvasion")]
    public bool InfiniteInvasion = TShock.Config.Settings.InfiniteInvasion;

    // 设置 PvP 模式(normal/always/disabled/pvpwithnoteam)
    [LocalizedPropertyName(CultureType.Chinese, "PVP模式")]
    [LocalizedPropertyName(CultureType.English, "PvPMode")]
    public string PvPMode = TShock.Config.Settings.PvPMode;

    // 防止在默认生成点的范围内放置方块
    [LocalizedPropertyName(CultureType.Chinese, "启用出生点保护")]
    [LocalizedPropertyName(CultureType.English, "SpawnProtection")]
    public bool SpawnProtection = TShock.Config.Settings.SpawnProtection;

    // 默认生成点周围的保护范围(单位：格)
    [LocalizedPropertyName(CultureType.Chinese, "出生点保护范围")]
    [LocalizedPropertyName(CultureType.English, "SpawnProtectionRadius")]
    public int SpawnProtectionRadius = TShock.Config.Settings.SpawnProtectionRadius;

    // 启用或禁用基于玩家与方块放置距离的反作弊检查
    [LocalizedPropertyName(CultureType.Chinese, "启用放置物块范围检查")]
    [LocalizedPropertyName(CultureType.English, "RangeChecks")]
    public bool RangeChecks = TShock.Config.Settings.RangeChecks;

    // 禁止非硬核玩家连接
    [LocalizedPropertyName(CultureType.Chinese, "启用强制硬核角色")]
    [LocalizedPropertyName(CultureType.English, "HardcoreOnly")]
    public bool HardcoreOnly = TShock.Config.Settings.HardcoreOnly;

    // 禁止软核玩家连接
    [LocalizedPropertyName(CultureType.Chinese, "启用强制中核角色")]
    [LocalizedPropertyName(CultureType.English, "MediumcoreOnly")]
    public bool MediumcoreOnly = TShock.Config.Settings.MediumcoreOnly;

    // 禁止非软核玩家连接
    [LocalizedPropertyName(CultureType.Chinese, "启用强制软核角色")]
    [LocalizedPropertyName(CultureType.English, "SoftcoreOnly")]
    public bool SoftcoreOnly = TShock.Config.Settings.SoftcoreOnly;

    // 禁用方块放置或移除
    [LocalizedPropertyName(CultureType.Chinese, "禁止建筑")]
    [LocalizedPropertyName(CultureType.English, "DisableBuild")]
    public bool DisableBuild = TShock.Config.Settings.DisableBuild;

    // 如果启用，困难模式将不会通过肉山或 /starthardmode 命令激活
    [LocalizedPropertyName(CultureType.Chinese, "禁止困难模式")]
    [LocalizedPropertyName(CultureType.English, "DisableHardmode")]
    public bool DisableHardmode = TShock.Config.Settings.DisableHardmode;

    // 禁止地牢守卫生成，取而代之的是将玩家发送回生成点
    [LocalizedPropertyName(CultureType.Chinese, "禁止生成地牢守卫")]
    [LocalizedPropertyName(CultureType.English, "DisableDungeonGuardian")]
    public bool DisableDungeonGuardian = TShock.Config.Settings.DisableDungeonGuardian;

    // 禁用小丑炸弹生成
    [LocalizedPropertyName(CultureType.Chinese, "禁止血月小丑炸弹")]
    [LocalizedPropertyName(CultureType.English, "DisableClownBombs")]
    public bool DisableClownBombs = TShock.Config.Settings.DisableClownBombs;

    // 禁用雪球炸弹生成
    [LocalizedPropertyName(CultureType.Chinese, "禁止雪人雪块弹幕")]
    [LocalizedPropertyName(CultureType.English, "DisableSnowBalls")]
    public bool DisableSnowBalls = TShock.Config.Settings.DisableSnowBalls;

    // 禁用死亡时掉落墓碑
    [LocalizedPropertyName(CultureType.Chinese, "禁止玩家死亡生成墓碑")]
    [LocalizedPropertyName(CultureType.English, "DisableTombstones")]
    public bool DisableTombstones = TShock.Config.Settings.DisableTombstones;

    // 禁用 Skeletron Prime 炸弹生成，防止破坏 “for the worthy” 世界
    [LocalizedPropertyName(CultureType.Chinese, "禁止机械骷髅王炸弹")]
    [LocalizedPropertyName(CultureType.English, "DisablePrimeBombs")]
    public bool DisablePrimeBombs = TShock.Config.Settings.DisablePrimeBombs;

    // 强制世界时间为白天或黑夜(normal/day/night)
    [LocalizedPropertyName(CultureType.Chinese, "强制世界时间")]
    [LocalizedPropertyName(CultureType.English, "ForceTime")]
    public string ForceTime = TShock.Config.Settings.ForceTime;

    // 当PvP启用时，禁用隐身药水的效果，使玩家对其他玩家可见。
    [LocalizedPropertyName(CultureType.Chinese, "禁止PVP隐身药水")]
    [LocalizedPropertyName(CultureType.English, "DisableInvisPvP")]
    public bool DisableInvisPvP = TShock.Config.Settings.DisableInvisPvP;

    // 禁用玩家的移动范围(单位：方块)。
    [LocalizedPropertyName(CultureType.Chinese, "未登录时禁止移动范围")]
    [LocalizedPropertyName(CultureType.English, "MaxRangeForDisabled")]
    public int MaxRangeForDisabled = TShock.Config.Settings.MaxRangeForDisabled;

    // 是否对箱子应用区域保护。
    [LocalizedPropertyName(CultureType.Chinese, "保护区域箱子")]
    [LocalizedPropertyName(CultureType.English, "RegionProtectChests")]
    public bool RegionProtectChests = TShock.Config.Settings.RegionProtectChests;

    // 是否对宝石锁应用区域保护。
    [LocalizedPropertyName(CultureType.Chinese, "保护区域内宝石锁")]
    [LocalizedPropertyName(CultureType.English, "RegionProtectGemLocks")]
    public bool RegionProtectGemLocks = TShock.Config.Settings.RegionProtectGemLocks;

    // 忽略检查玩家是否可以更新弹幕。
    [LocalizedPropertyName(CultureType.Chinese, "忽略检查玩家弹幕更新")]
    [LocalizedPropertyName(CultureType.English, "IgnoreProjUpdate")]
    public bool IgnoreProjUpdate = TShock.Config.Settings.IgnoreProjUpdate;

    // 忽略检查玩家是否可以销毁弹幕。
    [LocalizedPropertyName(CultureType.Chinese, "忽略检查玩家弹幕销毁")]
    [LocalizedPropertyName(CultureType.English, "IgnoreProjKill")]
    public bool IgnoreProjKill = TShock.Config.Settings.IgnoreProjKill;

    // 允许玩家破坏临时方块(草、罐子等)，即使在通常不能建造的地方。
    [LocalizedPropertyName(CultureType.Chinese, "允许玩家破坏易碎方块")]
    [LocalizedPropertyName(CultureType.English, "AllowCutTilesAndBreakables")]
    public bool AllowCutTilesAndBreakables = TShock.Config.Settings.AllowCutTilesAndBreakables;

    // 允许在通常不能建造的地方放置冰块。
    [LocalizedPropertyName(CultureType.Chinese, "允许玩家保护区域释放冰块")]
    [LocalizedPropertyName(CultureType.English, "AllowIce")]
    public bool AllowIce = TShock.Config.Settings.AllowIce;

    // 允许猩红在困难模式下蔓延。
    [LocalizedPropertyName(CultureType.Chinese, "允许猩红蔓延")]
    [LocalizedPropertyName(CultureType.English, "AllowCrimsonCreep")]
    public bool AllowCrimsonCreep = TShock.Config.Settings.AllowCrimsonCreep;

    // 允许腐化在困难模式下蔓延。
    [LocalizedPropertyName(CultureType.Chinese, "允许腐化蔓延")]
    [LocalizedPropertyName(CultureType.English, "AllowCorruptionCreep")]
    public bool AllowCorruptionCreep = TShock.Config.Settings.AllowCorruptionCreep;

    // 允许神圣在困难模式下蔓延。
    [LocalizedPropertyName(CultureType.Chinese, "允许神圣蔓延")]
    [LocalizedPropertyName(CultureType.English, "AllowHallowCreep")]
    public bool AllowHallowCreep = TShock.Config.Settings.AllowHallowCreep;

    // 统计200格之间的雕像生成NPC数量。
    [LocalizedPropertyName(CultureType.Chinese, "雕像停止生成前200像素(12.5格)内该NPC数量")]
    [LocalizedPropertyName(CultureType.English, "StatueSpawn200")]
    public int StatueSpawn200 = TShock.Config.Settings.StatueSpawn200;

    // 统计600格之间的雕像生成NPC数量。
    [LocalizedPropertyName(CultureType.Chinese, "雕像停止生成前600格(37.5格)内该NPC数量")]
    [LocalizedPropertyName(CultureType.English, "StatueSpawn600")]
    public int StatueSpawn600 = TShock.Config.Settings.StatueSpawn600;

    // 整个世界雕像停止生成前该NPC数量"。
    [LocalizedPropertyName(CultureType.Chinese, "整个世界雕像停止生成前该NPC数量")]
    [LocalizedPropertyName(CultureType.English, "StatueSpawnWorld")]
    public int StatueSpawnWorld = TShock.Config.Settings.StatueSpawnWorld;

    // 阻止禁用物品生成或指令获取。
    [LocalizedPropertyName(CultureType.Chinese, "阻止禁用物品生成或指令获取")]
    [LocalizedPropertyName(CultureType.English, "PreventBannedItemSpawn")]
    public bool PreventBannedItemSpawn = TShock.Config.Settings.PreventBannedItemSpawn;

    // 阻止玩家死后与世界互动。
    [LocalizedPropertyName(CultureType.Chinese, "阻止玩家死后与世界互动")]
    [LocalizedPropertyName(CultureType.English, "PreventDeadModification")]
    public bool PreventDeadModification = TShock.Config.Settings.PreventDeadModification;

    // 阻止玩家放置无效风格的方块。
    [LocalizedPropertyName(CultureType.Chinese, "阻止玩家放置无效特殊值方块")]
    [LocalizedPropertyName(CultureType.English, "PreventInvalidPlaceStyle")]
    public bool PreventInvalidPlaceStyle = TShock.Config.Settings.PreventInvalidPlaceStyle;

    // 强制圣诞节事件全年发生。
    [LocalizedPropertyName(CultureType.Chinese, "强制圣诞节")]
    [LocalizedPropertyName(CultureType.English, "ForceXmas")]
    public bool ForceXmas = TShock.Config.Settings.ForceXmas;

    // 强制万圣节事件全年发生。
    [LocalizedPropertyName(CultureType.Chinese, "强制万圣节")]
    [LocalizedPropertyName(CultureType.English, "ForceHalloween")]
    public bool ForceHalloween = TShock.Config.Settings.ForceHalloween;

    // 允许管理员获取禁用物品。
    [LocalizedPropertyName(CultureType.Chinese, "允许可使用禁用物品的组生成禁用物品")]
    [LocalizedPropertyName(CultureType.English, "AllowAllowedGroupsToSpawnBannedItems")]
    public bool AllowAllowedGroupsToSpawnBannedItems = TShock.Config.Settings.AllowAllowedGroupsToSpawnBannedItems;

    // 玩家复活时间(秒)。
    [LocalizedPropertyName(CultureType.Chinese, "玩家复活时间")]
    [LocalizedPropertyName(CultureType.English, "RespawnSeconds")]
    public int RespawnSeconds = TShock.Config.Settings.RespawnSeconds;

    // 玩家BOSS战复活时间(秒)。
    [LocalizedPropertyName(CultureType.Chinese, "玩家BOSS战复活时间")]
    [LocalizedPropertyName(CultureType.English, "RespawnBossSeconds")]
    public int RespawnBossSeconds = TShock.Config.Settings.RespawnBossSeconds;

    // 是否提示BOSS生成或事件入侵。
    [LocalizedPropertyName(CultureType.Chinese, "不显示召唤BOSS或事件入侵的玩家")]
    [LocalizedPropertyName(CultureType.English, "AnonymousBossInvasions")]
    public bool AnonymousBossInvasions = TShock.Config.Settings.AnonymousBossInvasions;

    // 检测玩家血量上限(超过会被网住)。
    [LocalizedPropertyName(CultureType.Chinese, "玩家血量上限")]
    [LocalizedPropertyName(CultureType.English, "MaxHP")]
    public int MaxHP = TShock.Config.Settings.MaxHP;

    // 检测玩家蓝量上限(惩罚同上)。
    [LocalizedPropertyName(CultureType.Chinese, "玩家蓝量上限")]
    [LocalizedPropertyName(CultureType.English, "MaxMP")]
    public int MaxMP = TShock.Config.Settings.MaxMP;

    // 爆炸影响范围(单位：方块)。
    [LocalizedPropertyName(CultureType.Chinese, "爆炸影响范围")]
    [LocalizedPropertyName(CultureType.English, "BombExplosionRadius")]
    public int BombExplosionRadius = TShock.Config.Settings.BombExplosionRadius;

    // 是否直接给予玩家物品到其背包中(需要SSC支持)。
    [LocalizedPropertyName(CultureType.Chinese, "给予物品直接插入玩家背包(需SSC)")]
    [LocalizedPropertyName(CultureType.English, "GiveItemsDirectly")]
    public bool GiveItemsDirectly = TShock.Config.Settings.GiveItemsDirectly;

    #endregion

    #region Login and Ban Settings

    // 默认将新注册的用户放入的组名。
    [LocalizedPropertyName(CultureType.Chinese, "玩家注册后的用户组")]
    [LocalizedPropertyName(CultureType.English, "DefaultRegistrationGroupName")]
    public string DefaultRegistrationGroupName = TShock.Config.Settings.DefaultRegistrationGroupName;

    // 默认将未注册玩家放入的组名。
    [LocalizedPropertyName(CultureType.Chinese, "玩家注册前的用户组")]
    [LocalizedPropertyName(CultureType.English, "DefaultGuestGroupName")]
    public string DefaultGuestGroupName = TShock.Config.Settings.DefaultGuestGroupName;

    // 记住玩家离开的位置，基于玩家的 IP。此设置不会在服务器重启后保存。
    [LocalizedPropertyName(CultureType.Chinese, "进服传回离线时的位置")]
    [LocalizedPropertyName(CultureType.English, "RememberLeavePos")]
    public bool RememberLeavePos = TShock.Config.Settings.RememberLeavePos;

    // 最大登录失败尝试次数，超过此次数后踢出玩家。
    [LocalizedPropertyName(CultureType.Chinese, "尝试登录次数上限")]
    [LocalizedPropertyName(CultureType.English, "MaximumLoginAttempts")]
    public int MaximumLoginAttempts = TShock.Config.Settings.MaximumLoginAttempts;

    // 是否在中核玩家死亡时将其踢出。
    [LocalizedPropertyName(CultureType.Chinese, "踢出死亡后的中核玩家")]
    [LocalizedPropertyName(CultureType.English, "KickOnMediumcoreDeath")]
    public bool KickOnMediumcoreDeath = TShock.Config.Settings.KickOnMediumcoreDeath;

    // 如果中核玩家死亡，将给出的踢出原因。
    [LocalizedPropertyName(CultureType.Chinese, "踢出死亡后的中核玩家信息")]
    [LocalizedPropertyName(CultureType.English, "MediumcoreKickReason")]
    public string MediumcoreKickReason = TShock.Config.Settings.MediumcoreKickReason;

    // 是否在中核玩家死亡时将其封禁。
    [LocalizedPropertyName(CultureType.Chinese, "封禁死亡后的中核玩家")]
    [LocalizedPropertyName(CultureType.English, "BanOnMediumcoreDeath")]
    public bool BanOnMediumcoreDeath = TShock.Config.Settings.BanOnMediumcoreDeath;

    // 如果中核玩家死亡，将给出的封禁原因。
    [LocalizedPropertyName(CultureType.Chinese, "封禁死亡后的中核玩家信息")]
    [LocalizedPropertyName(CultureType.English, "MediumcoreBanReason")]
    public string MediumcoreBanReason = TShock.Config.Settings.MediumcoreBanReason;

    // 默认情况下，禁用 IP 封禁(如果没有传递参数到封禁命令)。
    [LocalizedPropertyName(CultureType.Chinese, "关闭默认封禁IP")]
    [LocalizedPropertyName(CultureType.English, "DisableDefaultIPBan")]
    public bool DisableDefaultIPBan = TShock.Config.Settings.DisableDefaultIPBan;

    // 根据 whitelist.txt 文件中的 IP 地址启用或禁用白名单。
    [LocalizedPropertyName(CultureType.Chinese, "启用IP白名单")]
    [LocalizedPropertyName(CultureType.English, "EnableWhitelist")]
    public bool EnableWhitelist = TShock.Config.Settings.EnableWhitelist;

    // 玩家尝试加入时，如果不在白名单中，将给出的踢出原因。
    [LocalizedPropertyName(CultureType.Chinese, "不在白名单被踢出的信息")]
    [LocalizedPropertyName(CultureType.English, "WhitelistKickReason")]
    public string WhitelistKickReason = TShock.Config.Settings.WhitelistKickReason;

    // 当服务器已满，玩家尝试加入时的踢出原因。
    [LocalizedPropertyName(CultureType.Chinese, "服务器人满提示信息")]
    [LocalizedPropertyName(CultureType.English, "ServerFullReason")]
    public string ServerFullReason = TShock.Config.Settings.ServerFullReason;

    // 当服务器已满且没有保留插槽时，玩家尝试加入时的踢出原因。
    [LocalizedPropertyName(CultureType.Chinese, "服务器人满且没有保留位的提示信息")]
    [LocalizedPropertyName(CultureType.English, "ServerFullNoReservedReason")]
    public string ServerFullNoReservedReason = TShock.Config.Settings.ServerFullNoReservedReason;

    // 是否在硬核玩家死亡时将其踢出。
    [LocalizedPropertyName(CultureType.Chinese, "踢出死亡后的硬核玩家")]
    [LocalizedPropertyName(CultureType.English, "KickOnHardcoreDeath")]
    public bool KickOnHardcoreDeath = TShock.Config.Settings.KickOnHardcoreDeath;

    // 硬核玩家死亡时的踢出原因。
    [LocalizedPropertyName(CultureType.Chinese, "踢出死亡后的硬核玩家信息")]
    [LocalizedPropertyName(CultureType.English, "HardcoreKickReason")]
    public string HardcoreKickReason = TShock.Config.Settings.HardcoreKickReason;

    // 是否在硬核玩家死亡时将其封禁。
    [LocalizedPropertyName(CultureType.Chinese, "封禁死亡后的硬核玩家")]
    [LocalizedPropertyName(CultureType.English, "BanOnHardcoreDeath")]
    public bool BanOnHardcoreDeath = TShock.Config.Settings.BanOnHardcoreDeath;

    // 硬核玩家死亡时的封禁原因。
    [LocalizedPropertyName(CultureType.Chinese, "封禁死亡后的硬核玩家信息")]
    [LocalizedPropertyName(CultureType.English, "HardcoreBanReason")]
    public string HardcoreBanReason = TShock.Config.Settings.HardcoreBanReason;

    // 如果启用了 GeoIP，将踢出被识别为使用代理的用户。
    [LocalizedPropertyName(CultureType.Chinese, "踢出代理IP用户")]
    [LocalizedPropertyName(CultureType.English, "KickProxyUsers")]
    public bool KickProxyUsers = TShock.Config.Settings.KickProxyUsers;

    // 是否要求所有玩家在游戏前注册或登录。
    [LocalizedPropertyName(CultureType.Chinese, "用户必须登录")]
    [LocalizedPropertyName(CultureType.English, "RequireLogin")]
    public bool RequireLogin = TShock.Config.Settings.RequireLogin;

    // 允许用户登录任何账号，即使用户名与角色名不匹配。
    [LocalizedPropertyName(CultureType.Chinese, "允许玩家登录账号与角色名不符")]
    [LocalizedPropertyName(CultureType.English, "AllowLoginAnyUsername")]
    public bool AllowLoginAnyUsername = TShock.Config.Settings.AllowLoginAnyUsername;

    // 允许用户注册与角色名不匹配的用户名。
    [LocalizedPropertyName(CultureType.Chinese, "允许玩家注册账号与角色名不符")]
    [LocalizedPropertyName(CultureType.English, "AllowRegisterAnyUsername")]
    public bool AllowRegisterAnyUsername = TShock.Config.Settings.AllowRegisterAnyUsername;

    // 新用户账户的最小密码长度。不能低于 4。
    [LocalizedPropertyName(CultureType.Chinese, "密码最少长度")]
    [LocalizedPropertyName(CultureType.English, "MinimumPasswordLength")]
    public int MinimumPasswordLength = TShock.Config.Settings.MinimumPasswordLength;

    // 确定 BCrypt 工作因子。增加此因子后，所有密码将在验证时升级为新工作因子。计算轮数是 2^n。增加时请谨慎。范围：5-31。
    [LocalizedPropertyName(CultureType.Chinese, "使用的BCrypt工作因子")]
    [LocalizedPropertyName(CultureType.English, "BCryptWorkFactor")]
    public int BCryptWorkFactor = TShock.Config.Settings.BCryptWorkFactor;

    // 禁止用户使用客户端 UUID 登录。
    [LocalizedPropertyName(CultureType.Chinese, "禁止UUID自动登录")]
    [LocalizedPropertyName(CultureType.English, "DisableUUIDLogin")]
    public bool DisableUUIDLogin = TShock.Config.Settings.DisableUUIDLogin;

    // 踢出未发送 UUID 的客户端。
    [LocalizedPropertyName(CultureType.Chinese, "踢出不发送UUID到服务器的玩家")]
    [LocalizedPropertyName(CultureType.English, "KickEmptyUUID")]
    public bool KickEmptyUUID = TShock.Config.Settings.KickEmptyUUID;

    // 如果在 1 秒钟内绘制的瓷砖数量超过此数值，将禁用玩家。
    [LocalizedPropertyName(CultureType.Chinese, "1秒内刷油漆的格数上限")]
    [LocalizedPropertyName(CultureType.English, "TilePaintThreshold")]
    public int TilePaintThreshold = TShock.Config.Settings.TilePaintThreshold;

    // 是否在超出 TilePaint 阈值时踢出玩家。
    [LocalizedPropertyName(CultureType.Chinese, "踢出1秒内刷油漆超出格数上限的玩家")]
    [LocalizedPropertyName(CultureType.English, "KickOnTilePaintThresholdBroken")]
    public bool KickOnTilePaintThresholdBroken = TShock.Config.Settings.KickOnTilePaintThresholdBroken;

    // 玩家或 NPC 可造成的最大伤害。
    [LocalizedPropertyName(CultureType.Chinese, "玩家最大伤害上限")]
    [LocalizedPropertyName(CultureType.English, "MaxDamage")]
    public int MaxDamage = TShock.Config.Settings.MaxDamage;

    // 投射物可造成的最大伤害。
    [LocalizedPropertyName(CultureType.Chinese, "玩家最大弹幕伤害上限")]
    [LocalizedPropertyName(CultureType.English, "MaxProjDamage")]
    public int MaxProjDamage = TShock.Config.Settings.MaxProjDamage;

    // 是否在超出 MaxDamage 阈值时踢出玩家。
    [LocalizedPropertyName(CultureType.Chinese, "踢出超出伤害上限的玩家")]
    [LocalizedPropertyName(CultureType.English, "KickOnDamageThresholdBroken")]
    public bool KickOnDamageThresholdBroken = TShock.Config.Settings.KickOnDamageThresholdBroken;

    // 如果在 1 秒内杀死的瓷砖数量超过此值，将禁用玩家并撤销其操作。
    [LocalizedPropertyName(CultureType.Chinese, "1秒内破坏方块的格数上限")]
    [LocalizedPropertyName(CultureType.English, "TileKillThreshold")]
    public int TileKillThreshold = TShock.Config.Settings.TileKillThreshold;

    // 是否在超出 TileKill 阈值时踢出玩家。
    [LocalizedPropertyName(CultureType.Chinese, "踢出1秒内破坏方块超出格数上限的玩家")]
    [LocalizedPropertyName(CultureType.English, "KickOnTileKillThresholdBroken")]
    public bool KickOnTileKillThresholdBroken = TShock.Config.Settings.KickOnTileKillThresholdBroken;

    // 如果在 1 秒内放置的瓷砖数量超过此值，将禁用玩家并撤销其操作。
    [LocalizedPropertyName(CultureType.Chinese, "1秒内放置方块的格数上限")]
    [LocalizedPropertyName(CultureType.English, "TilePlaceThreshold")]
    public int TilePlaceThreshold = TShock.Config.Settings.TilePlaceThreshold;

    // 是否在超出 TilePlace 阈值时踢出玩家。
    [LocalizedPropertyName(CultureType.Chinese, "踢出1秒内放置方块超出格数上限的玩家")]
    [LocalizedPropertyName(CultureType.English, "KickOnTilePlaceThresholdBroken")]
    public bool KickOnTilePlaceThresholdBroken = TShock.Config.Settings.KickOnTilePlaceThresholdBroken;

    // 如果在 1 秒内超出此液体设置数值，将禁用玩家。
    [LocalizedPropertyName(CultureType.Chinese, "1秒内放置液体的格数上限")]
    [LocalizedPropertyName(CultureType.English, "TileLiquidThreshold")]
    public int TileLiquidThreshold = TShock.Config.Settings.TileLiquidThreshold;

    // 是否在超出 TileLiquid 阈值时踢出玩家。
    [LocalizedPropertyName(CultureType.Chinese, "踢出1秒内放置液体超出格数上限的玩家")]
    [LocalizedPropertyName(CultureType.English, "KickOnTileLiquidThresholdBroken")]
    public bool KickOnTileLiquidThresholdBroken = TShock.Config.Settings.KickOnTileLiquidThresholdBroken;

    // 是否忽略水晶子弹的碎片在投射物阈值统计中。
    [LocalizedPropertyName(CultureType.Chinese, "弹幕数量是否包含水晶子弹")]
    [LocalizedPropertyName(CultureType.English, "ProjIgnoreShrapnel")]
    public bool ProjIgnoreShrapnel = TShock.Config.Settings.ProjIgnoreShrapnel;

    // 如果在 1 秒内超出此投射物创建数，将禁用玩家。
    [LocalizedPropertyName(CultureType.Chinese, "1秒内释放弹幕的数量上限")]
    [LocalizedPropertyName(CultureType.English, "ProjectileThreshold")]
    public int ProjectileThreshold = TShock.Config.Settings.ProjectileThreshold;

    // 是否在超出 Projectile 阈值时踢出玩家。
    [LocalizedPropertyName(CultureType.Chinese, "踢出1秒内释放弹幕超出数量上限的玩家")]
    [LocalizedPropertyName(CultureType.English, "KickOnProjectileThresholdBroken")]
    public bool KickOnProjectileThresholdBroken = TShock.Config.Settings.KickOnProjectileThresholdBroken;

    // 如果在 1 秒内发送的 HealOtherPlayer 包超过此数值，将禁用玩家。
    [LocalizedPropertyName(CultureType.Chinese, "1秒内治疗其他玩家的数值上限")]
    [LocalizedPropertyName(CultureType.English, "HealOtherThreshold")]
    public int HealOtherThreshold = TShock.Config.Settings.HealOtherThreshold;

    // 是否在超出 HealOther 阈值时踢出玩家。
    [LocalizedPropertyName(CultureType.Chinese, "踢出1秒内治疗其他玩家超出数值上限的玩家")]
    [LocalizedPropertyName(CultureType.English, "KickOnHealOtherThresholdBroken")]
    public bool KickOnHealOtherThresholdBroken = TShock.Config.Settings.KickOnHealOtherThresholdBroken;
[LocalizedPropertyName(CultureType.Chinese, "不提示受保护区域无权建筑信息")]
    [LocalizedPropertyName(CultureType.English, "SuppressPermissionFailureNotices")]
    public bool SuppressPermissionFailureNotices = TShock.Config.Settings.SuppressPermissionFailureNotices;
[LocalizedPropertyName(CultureType.Chinese, "禁止修改的天顶剑")]
    [LocalizedPropertyName(CultureType.English, "DisableModifiedZenith")]
    public bool DisableModifiedZenith = TShock.Config.Settings.DisableModifiedZenith;
[LocalizedPropertyName(CultureType.Chinese, "禁用自定义死亡信息")]
    [LocalizedPropertyName(CultureType.English, "DisableCustomDeathMessages")]
    public bool DisableCustomDeathMessages = TShock.Config.Settings.DisableCustomDeathMessages;

    #endregion

    #region Chat Settings
[LocalizedPropertyName(CultureType.Chinese, "指令前缀")]
    [LocalizedPropertyName(CultureType.English, "CommandSpecifier")]
    public string CommandSpecifier = TShock.Config.Settings.CommandSpecifier;
[LocalizedPropertyName(CultureType.Chinese, "隐藏指令前缀")]
    [LocalizedPropertyName(CultureType.English, "CommandSilentSpecifier")]
    public string CommandSilentSpecifier = TShock.Config.Settings.CommandSilentSpecifier;
[LocalizedPropertyName(CultureType.Chinese, "不将日志作为聊天信息发送给有日志权限的玩家")]
    [LocalizedPropertyName(CultureType.English, "DisableSpewLogs")]
    public bool DisableSpewLogs = TShock.Config.Settings.DisableSpewLogs;
[LocalizedPropertyName(CultureType.Chinese, "不将每秒的更新检查写入日志")]
    [LocalizedPropertyName(CultureType.English, "DisableSecondUpdateLogs")]
    public bool DisableSecondUpdateLogs = TShock.Config.Settings.DisableSecondUpdateLogs;
[LocalizedPropertyName(CultureType.Chinese, "超级管理员的聊天颜色")]
    [LocalizedPropertyName(CultureType.English, "SuperAdminChatRGB")]
    public int[] SuperAdminChatRGB = TShock.Config.Settings.SuperAdminChatRGB;
[LocalizedPropertyName(CultureType.Chinese, "超管的聊天前缀")]
    [LocalizedPropertyName(CultureType.English, "SuperAdminChatPrefix")]
    public string SuperAdminChatPrefix = TShock.Config.Settings.SuperAdminChatPrefix;
[LocalizedPropertyName(CultureType.Chinese, "超管的聊天后缀")]
    [LocalizedPropertyName(CultureType.English, "SuperAdminChatSuffix")]
    public string SuperAdminChatSuffix = TShock.Config.Settings.SuperAdminChatSuffix;
[LocalizedPropertyName(CultureType.Chinese, "显示加入服务器的玩家IP地理位置")]
    [LocalizedPropertyName(CultureType.English, "EnableGeoIP")]
    public bool EnableGeoIP = TShock.Config.Settings.EnableGeoIP;
[LocalizedPropertyName(CultureType.Chinese, "向有日志权限的管理显示进入玩家的IP")]
    [LocalizedPropertyName(CultureType.English, "DisplayIPToAdmins")]
    public bool DisplayIPToAdmins = TShock.Config.Settings.DisplayIPToAdmins;
[LocalizedPropertyName(CultureType.Chinese, "聊天格式")]
    [LocalizedPropertyName(CultureType.English, "ChatFormat")]
    public string ChatFormat = TShock.Config.Settings.ChatFormat;
[LocalizedPropertyName(CultureType.Chinese, "聊天栏内玩家名字")]
    [LocalizedPropertyName(CultureType.English, "ChatAboveHeadsFormat")]
    public string ChatAboveHeadsFormat = TShock.Config.Settings.ChatAboveHeadsFormat;
[LocalizedPropertyName(CultureType.Chinese, "是否在玩家头顶显示聊天消息")]
    [LocalizedPropertyName(CultureType.English, "EnableChatAboveHeads")]
    public bool EnableChatAboveHeads = TShock.Config.Settings.EnableChatAboveHeads;
[LocalizedPropertyName(CultureType.Chinese, "系统广播文字颜色")]
    [LocalizedPropertyName(CultureType.English, "BroadcastRGB")]
    public int[] BroadcastRGB = TShock.Config.Settings.BroadcastRGB;

    #endregion

    #region MySQL Settings
[LocalizedPropertyName(CultureType.Chinese, "数据库类型")]
    [LocalizedPropertyName(CultureType.English, "StorageType")]
    public string StorageType = TShock.Config.Settings.StorageType;
[LocalizedPropertyName(CultureType.Chinese, "数据库路径")]
    [LocalizedPropertyName(CultureType.English, "SqliteDBPath")]
    public string SqliteDBPath = TShock.Config.Settings.SqliteDBPath;
[LocalizedPropertyName(CultureType.Chinese, "Mysql连接的ip和端口")]
    [LocalizedPropertyName(CultureType.English, "MySqlHost")]
    public string MySqlHost = TShock.Config.Settings.MySqlHost;
[LocalizedPropertyName(CultureType.Chinese, "Mysql的数据库名称")]
    [LocalizedPropertyName(CultureType.English, "MySqlDbName")]
    public string MySqlDbName = TShock.Config.Settings.MySqlDbName;
[LocalizedPropertyName(CultureType.Chinese, "Mysql的用户名")]
    [LocalizedPropertyName(CultureType.English, "MySqlUsername")]
    public string MySqlUsername = TShock.Config.Settings.MySqlUsername;
[LocalizedPropertyName(CultureType.Chinese, "Mysql的密码")]
    [LocalizedPropertyName(CultureType.English, "MySqlPassword")]
    public string MySqlPassword = TShock.Config.Settings.MySqlPassword;
[LocalizedPropertyName(CultureType.Chinese, "是否把日志存入数据库")]
    [LocalizedPropertyName(CultureType.English, "UseSqlLogs")]
    public bool UseSqlLogs = TShock.Config.Settings.UseSqlLogs;
[LocalizedPropertyName(CultureType.Chinese, "Sql日志连接失败指定次数后变回文本日志")]
    [LocalizedPropertyName(CultureType.English, "RevertToTextLogsOnSqlFailures")]
    public int RevertToTextLogsOnSqlFailures = TShock.Config.Settings.RevertToTextLogsOnSqlFailures;

    #endregion

    #region REST API Settings
[LocalizedPropertyName(CultureType.Chinese, "开启 Rest API")]
    [LocalizedPropertyName(CultureType.English, "RestApiEnabled")]
    public bool RestApiEnabled = TShock.Config.Settings.RestApiEnabled;
[LocalizedPropertyName(CultureType.Chinese, "Rest的端口")]
    [LocalizedPropertyName(CultureType.English, "RestApiPort")]
    public int RestApiPort = TShock.Config.Settings.RestApiPort;
[LocalizedPropertyName(CultureType.Chinese, "记录Rest连线")]
    [LocalizedPropertyName(CultureType.English, "LogRest")]
    public bool LogRest = TShock.Config.Settings.LogRest;
[LocalizedPropertyName(CultureType.Chinese, "开启对Rest的权限认证")]
    [LocalizedPropertyName(CultureType.English, "EnableTokenEndpointAuthentication")]
    public bool EnableTokenEndpointAuthentication = TShock.Config.Settings.EnableTokenEndpointAuthentication;
[LocalizedPropertyName(CultureType.Chinese, "Rest最大请求次数")]
    [LocalizedPropertyName(CultureType.English, "RESTMaximumRequestsPerInterval")]
    public int RESTMaximumRequestsPerInterval = TShock.Config.Settings.RESTMaximumRequestsPerInterval;
[LocalizedPropertyName(CultureType.Chinese, "Rest允许连接请求加一间隔")]
    [LocalizedPropertyName(CultureType.English, "RESTRequestBucketDecreaseIntervalMinutes")]
    public int RESTRequestBucketDecreaseIntervalMinutes = TShock.Config.Settings.RESTRequestBucketDecreaseIntervalMinutes;
[LocalizedPropertyName(CultureType.Chinese, "Rest外部应用令牌字典")]
    [LocalizedPropertyName(CultureType.English, "ApplicationRestTokens")]
    public Dictionary<string, SecureRest.TokenData> ApplicationRestTokens = TShock.Config.Settings.ApplicationRestTokens;

    #endregion
}