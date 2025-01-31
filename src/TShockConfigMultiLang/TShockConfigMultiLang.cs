using LazyAPI;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace TShockConfigMultiLang;
[ApiVersion(2, 1)]
public class TShockConfigMultiLang : LazyPlugin
{
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override string Author => "肝帝熙恩，羽学";
    public override string Description => GetString("创建一个本地化语言的config");
    public override Version Version => new Version(1, 0, 3);

    public TShockConfigMultiLang(Main game) : base(game)
    {
        this.Order = 1;
    }

    public override void Initialize()
    {
        Commands.ChatCommands.Add(new Command(Permissions.cfgreload, this.CTC, "configToNewconfig", "ctc", "原版同步给本土"));
        Commands.ChatCommands.Add(new Command(Permissions.cfgreload, this.CFC, "configFromnNewconfig", "cfc", "本土同步给原版"));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == this.CTC || x.CommandDelegate == this.CFC);
        }
        base.Dispose(disposing);
    }
    private void OnReload(TSPlayer player)
    {
        TShock.Utils.Reload();
        TShockAPI.Hooks.GeneralHooks.OnReloadEvent(player);
        player.SendSuccessMessage(GetString("[TShockConfigMultiLang] 同步已完成，已自动重载所有配置"));
    }

    private void CFC(CommandArgs args)
    {
        try
        {
            this.OnReload(args.Player);
            // 应用服务器设置
            #region Apply ServerSettings
            TShock.Config.Settings.ServerPassword = Config.Instance.ServerPassword;
            TShock.Config.Settings.ServerPort = Config.Instance.ServerPort;
            TShock.Config.Settings.MaxSlots = Config.Instance.MaxSlots;
            TShock.Config.Settings.ReservedSlots = Config.Instance.ReservedSlots;
            TShock.Config.Settings.ServerName = Config.Instance.ServerName;
            TShock.Config.Settings.UseServerName = Config.Instance.UseServerName;
            TShock.Config.Settings.LogPath = Config.Instance.LogPath;
            TShock.Config.Settings.DebugLogs = Config.Instance.DebugLogs;
            TShock.Config.Settings.DisableLoginBeforeJoin = Config.Instance.DisableLoginBeforeJoin;
            TShock.Config.Settings.IgnoreChestStacksOnLoad = Config.Instance.IgnoreChestStacksOnLoad;
            TShock.Config.Settings.WorldTileProvider = Config.Instance.WorldTileProvider;
            #endregion

            // 应用备份和保存设置
            #region Apply Backup and Save Settings
            TShock.Config.Settings.AutoSave = Config.Instance.AutoSave;
            TShock.Config.Settings.AnnounceSave = Config.Instance.AnnounceSave;
            TShock.Config.Settings.ShowBackupAutosaveMessages = Config.Instance.ShowBackupAutosaveMessages;
            TShock.Config.Settings.BackupInterval = Config.Instance.BackupInterval;
            TShock.Config.Settings.BackupKeepFor = Config.Instance.BackupKeepFor;
            TShock.Config.Settings.SaveWorldOnCrash = Config.Instance.SaveWorldOnCrash;
            TShock.Config.Settings.SaveWorldOnLastPlayerExit = Config.Instance.SaveWorldOnLastPlayerExit;
            #endregion

            #region Apply World Settings

            // 入侵事件的大小乘数
            TShock.Config.Settings.InvasionMultiplier = Config.Instance.InvasionMultiplier;

            // 默认刷怪生成上限
            TShock.Config.Settings.DefaultMaximumSpawns = Config.Instance.DefaultMaximumSpawns;

            // 刷怪间隔（数值越低，敌人生成越频繁）
            TShock.Config.Settings.DefaultSpawnRate = Config.Instance.DefaultSpawnRate;

            // 是否启用无限制入侵事件（仍需手动触发）
            TShock.Config.Settings.InfiniteInvasion = Config.Instance.InfiniteInvasion;

            // 设置 PvP 模式
            TShock.Config.Settings.PvPMode = Config.Instance.PvPMode;

            // 启用出生点保护
            TShock.Config.Settings.SpawnProtection = Config.Instance.SpawnProtection;

            // 出生点保护范围（单位：格）
            TShock.Config.Settings.SpawnProtectionRadius = Config.Instance.SpawnProtectionRadius;

            // 启用放置物块范围检查
            TShock.Config.Settings.RangeChecks = Config.Instance.RangeChecks;

            // 禁止非硬核玩家连接
            TShock.Config.Settings.HardcoreOnly = Config.Instance.HardcoreOnly;

            // 禁止软核玩家连接
            TShock.Config.Settings.MediumcoreOnly = Config.Instance.MediumcoreOnly;

            // 禁止非软核玩家连接
            TShock.Config.Settings.SoftcoreOnly = Config.Instance.SoftcoreOnly;

            // 禁用方块放置或移除
            TShock.Config.Settings.DisableBuild = Config.Instance.DisableBuild;

            // 禁止困难模式
            TShock.Config.Settings.DisableHardmode = Config.Instance.DisableHardmode;

            // 禁止生成地牢守卫
            TShock.Config.Settings.DisableDungeonGuardian = Config.Instance.DisableDungeonGuardian;

            // 禁用小丑炸弹生成
            TShock.Config.Settings.DisableClownBombs = Config.Instance.DisableClownBombs;

            // 禁用雪球炸弹生成
            TShock.Config.Settings.DisableSnowBalls = Config.Instance.DisableSnowBalls;

            // 禁用死亡时掉落墓碑
            TShock.Config.Settings.DisableTombstones = Config.Instance.DisableTombstones;

            // 禁用 Skeletron Prime 炸弹生成
            TShock.Config.Settings.DisablePrimeBombs = Config.Instance.DisablePrimeBombs;

            // 强制世界时间为白天或黑夜
            TShock.Config.Settings.ForceTime = Config.Instance.ForceTime;

            // 当PvP启用时，禁用隐身药水的效果
            TShock.Config.Settings.DisableInvisPvP = Config.Instance.DisableInvisPvP;

            // 禁止移动范围（未登录时）
            TShock.Config.Settings.MaxRangeForDisabled = Config.Instance.MaxRangeForDisabled;

            // 保护区域箱子
            TShock.Config.Settings.RegionProtectChests = Config.Instance.RegionProtectChests;

            // 保护区域箱子是否上锁
            TShock.Config.Settings.RegionProtectGemLocks = Config.Instance.RegionProtectGemLocks;

            // 忽略检查玩家是否可以更新弹幕
            TShock.Config.Settings.IgnoreProjUpdate = Config.Instance.IgnoreProjUpdate;

            // 忽略检查玩家是否可以销毁弹幕
            TShock.Config.Settings.IgnoreProjKill = Config.Instance.IgnoreProjKill;

            // 允许玩家破坏易碎方块
            TShock.Config.Settings.AllowCutTilesAndBreakables = Config.Instance.AllowCutTilesAndBreakables;

            // 允许在通常不能建造的地方放置冰块
            TShock.Config.Settings.AllowIce = Config.Instance.AllowIce;

            // 允许猩红在困难模式下蔓延
            TShock.Config.Settings.AllowCrimsonCreep = Config.Instance.AllowCrimsonCreep;

            // 允许腐化在困难模式下蔓延
            TShock.Config.Settings.AllowCorruptionCreep = Config.Instance.AllowCorruptionCreep;

            // 允许神圣在困难模式下蔓延
            TShock.Config.Settings.AllowHallowCreep = Config.Instance.AllowHallowCreep;

            // 统计200格之间的雕像生成NPC数量
            TShock.Config.Settings.StatueSpawn200 = Config.Instance.StatueSpawn200;

            // 统计600格之间的雕像生成NPC数量
            TShock.Config.Settings.StatueSpawn600 = Config.Instance.StatueSpawn600;

            // 统计整个世界雕像生成NPC数量
            TShock.Config.Settings.StatueSpawnWorld = Config.Instance.StatueSpawnWorld;

            // 阻止禁用物品生成或指令获取
            TShock.Config.Settings.PreventBannedItemSpawn = Config.Instance.PreventBannedItemSpawn;

            // 阻止玩家死后与世界互动
            TShock.Config.Settings.PreventDeadModification = Config.Instance.PreventDeadModification;

            // 阻止玩家放置无效风格的方块
            TShock.Config.Settings.PreventInvalidPlaceStyle = Config.Instance.PreventInvalidPlaceStyle;

            // 强制圣诞节事件全年发生
            TShock.Config.Settings.ForceXmas = Config.Instance.ForceXmas;

            // 强制万圣节事件全年发生
            TShock.Config.Settings.ForceHalloween = Config.Instance.ForceHalloween;

            // 允许管理员获取禁用物品
            TShock.Config.Settings.AllowAllowedGroupsToSpawnBannedItems = Config.Instance.AllowAllowedGroupsToSpawnBannedItems;

            // 玩家复活时间（秒）
            TShock.Config.Settings.RespawnSeconds = Config.Instance.RespawnSeconds;

            // 玩家BOSS战复活时间（秒）
            TShock.Config.Settings.RespawnBossSeconds = Config.Instance.RespawnBossSeconds;

            // 是否提示BOSS生成或事件入侵
            TShock.Config.Settings.AnonymousBossInvasions = Config.Instance.AnonymousBossInvasions;

            // 检测玩家血量上限（超过会被网住）
            TShock.Config.Settings.MaxHP = Config.Instance.MaxHP;

            // 检测玩家蓝量上限（惩罚同上）
            TShock.Config.Settings.MaxMP = Config.Instance.MaxMP;

            // 爆炸影响范围（单位：方块）
            TShock.Config.Settings.BombExplosionRadius = Config.Instance.BombExplosionRadius;

            // 是否直接给予玩家物品到其背包中（需要SSC支持）
            TShock.Config.Settings.GiveItemsDirectly = Config.Instance.GiveItemsDirectly;

            #endregion

            #region Apply Login and Ban Settings

            // 默认将新注册的用户放入的组名。
            TShock.Config.Settings.DefaultRegistrationGroupName = Config.Instance.DefaultRegistrationGroupName;

            // 默认将未注册玩家放入的组名。
            TShock.Config.Settings.DefaultGuestGroupName = Config.Instance.DefaultGuestGroupName;

            // 记住玩家离开的位置，基于玩家的 IP。此设置不会在服务器重启后保存。
            TShock.Config.Settings.RememberLeavePos = Config.Instance.RememberLeavePos;

            // 最大登录失败尝试次数，超过此次数后踢出玩家。
            TShock.Config.Settings.MaximumLoginAttempts = Config.Instance.MaximumLoginAttempts;

            // 是否在中核玩家死亡时将其踢出。
            TShock.Config.Settings.KickOnMediumcoreDeath = Config.Instance.KickOnMediumcoreDeath;

            // 如果中核玩家死亡，将给出的踢出原因。
            TShock.Config.Settings.MediumcoreKickReason = Config.Instance.MediumcoreKickReason;

            // 是否在中核玩家死亡时将其封禁。
            TShock.Config.Settings.BanOnMediumcoreDeath = Config.Instance.BanOnMediumcoreDeath;

            // 如果中核玩家死亡，将给出的封禁原因。
            TShock.Config.Settings.MediumcoreBanReason = Config.Instance.MediumcoreBanReason;

            // 默认情况下，禁用 IP 封禁（如果没有传递参数到封禁命令）。
            TShock.Config.Settings.DisableDefaultIPBan = Config.Instance.DisableDefaultIPBan;

            // 根据 whitelist.txt 文件中的 IP 地址启用或禁用白名单。
            TShock.Config.Settings.EnableWhitelist = Config.Instance.EnableWhitelist;

            // 玩家尝试加入时，如果不在白名单中，将给出的踢出原因。
            TShock.Config.Settings.WhitelistKickReason = Config.Instance.WhitelistKickReason;

            // 当服务器已满，玩家尝试加入时的踢出原因。
            TShock.Config.Settings.ServerFullReason = Config.Instance.ServerFullReason;

            // 当服务器已满且没有保留插槽时，玩家尝试加入时的踢出原因。
            TShock.Config.Settings.ServerFullNoReservedReason = Config.Instance.ServerFullNoReservedReason;

            // 是否在硬核玩家死亡时将其踢出。
            TShock.Config.Settings.KickOnHardcoreDeath = Config.Instance.KickOnHardcoreDeath;

            // 硬核玩家死亡时的踢出原因。
            TShock.Config.Settings.HardcoreKickReason = Config.Instance.HardcoreKickReason;

            // 是否在硬核玩家死亡时将其封禁。
            TShock.Config.Settings.BanOnHardcoreDeath = Config.Instance.BanOnHardcoreDeath;

            // 硬核玩家死亡时的封禁原因。
            TShock.Config.Settings.HardcoreBanReason = Config.Instance.HardcoreBanReason;

            // 如果启用了 GeoIP，将踢出被识别为使用代理的用户。
            TShock.Config.Settings.KickProxyUsers = Config.Instance.KickProxyUsers;

            // 是否要求所有玩家在游戏前注册或登录。
            TShock.Config.Settings.RequireLogin = Config.Instance.RequireLogin;

            // 允许用户登录任何账号，即使用户名与角色名不匹配。
            TShock.Config.Settings.AllowLoginAnyUsername = Config.Instance.AllowLoginAnyUsername;

            // 允许用户注册与角色名不匹配的用户名。
            TShock.Config.Settings.AllowRegisterAnyUsername = Config.Instance.AllowRegisterAnyUsername;

            // 新用户账户的最小密码长度。不能低于 4。
            TShock.Config.Settings.MinimumPasswordLength = Config.Instance.MinimumPasswordLength;

            // 确定 BCrypt 工作因子。增加此因子后，所有密码将在验证时升级为新工作因子。计算轮数是 2^n。增加时请谨慎。范围：5-31。
            TShock.Config.Settings.BCryptWorkFactor = Config.Instance.BCryptWorkFactor;

            // 禁止用户使用客户端 UUID 登录。
            TShock.Config.Settings.DisableUUIDLogin = Config.Instance.DisableUUIDLogin;

            // 踢出未发送 UUID 的客户端。
            TShock.Config.Settings.KickEmptyUUID = Config.Instance.KickEmptyUUID;

            // 如果在 1 秒钟内绘制的瓷砖数量超过此数值，将禁用玩家。
            TShock.Config.Settings.TilePaintThreshold = Config.Instance.TilePaintThreshold;

            // 是否在超出 TilePaint 阈值时踢出玩家。
            TShock.Config.Settings.KickOnTilePaintThresholdBroken = Config.Instance.KickOnTilePaintThresholdBroken;

            // 玩家或 NPC 可造成的最大伤害。
            TShock.Config.Settings.MaxDamage = Config.Instance.MaxDamage;

            // 投射物可造成的最大伤害。
            TShock.Config.Settings.MaxProjDamage = Config.Instance.MaxProjDamage;

            // 是否在超出 MaxDamage 阈值时踢出玩家。
            TShock.Config.Settings.KickOnDamageThresholdBroken = Config.Instance.KickOnDamageThresholdBroken;

            // 如果在 1 秒内杀死的瓷砖数量超过此值，将禁用玩家并撤销其操作。
            TShock.Config.Settings.TileKillThreshold = Config.Instance.TileKillThreshold;

            // 是否在超出 TileKill 阈值时踢出玩家。
            TShock.Config.Settings.KickOnTileKillThresholdBroken = Config.Instance.KickOnTileKillThresholdBroken;

            // 如果在 1 秒内放置的瓷砖数量超过此值，将禁用玩家并撤销其操作。
            TShock.Config.Settings.TilePlaceThreshold = Config.Instance.TilePlaceThreshold;

            // 是否在超出 TilePlace 阈值时踢出玩家。
            TShock.Config.Settings.KickOnTilePlaceThresholdBroken = Config.Instance.KickOnTilePlaceThresholdBroken;

            // 如果在 1 秒内超出此液体设置数值，将禁用玩家。
            TShock.Config.Settings.TileLiquidThreshold = Config.Instance.TileLiquidThreshold;

            // 是否在超出 TileLiquid 阈值时踢出玩家。
            TShock.Config.Settings.KickOnTileLiquidThresholdBroken = Config.Instance.KickOnTileLiquidThresholdBroken;

            // 是否忽略水晶子弹的碎片在投射物阈值统计中。
            TShock.Config.Settings.ProjIgnoreShrapnel = Config.Instance.ProjIgnoreShrapnel;

            // 如果在 1 秒内超出此投射物创建数，将禁用玩家。
            TShock.Config.Settings.ProjectileThreshold = Config.Instance.ProjectileThreshold;

            // 是否在超出 Projectile 阈值时踢出玩家。
            TShock.Config.Settings.KickOnProjectileThresholdBroken = Config.Instance.KickOnProjectileThresholdBroken;

            // 如果在 1 秒内发送的 HealOtherPlayer 包超过此数值，将禁用玩家。
            TShock.Config.Settings.HealOtherThreshold = Config.Instance.HealOtherThreshold;

            // 是否在超出 HealOther 阈值时踢出玩家。
            TShock.Config.Settings.KickOnHealOtherThresholdBroken = Config.Instance.KickOnHealOtherThresholdBroken;

            // 区域与出生点提示无权建筑。
            TShock.Config.Settings.SuppressPermissionFailureNotices = Config.Instance.SuppressPermissionFailureNotices;

            // 禁止修改后的天顶剑。
            TShock.Config.Settings.DisableModifiedZenith = Config.Instance.DisableModifiedZenith;

            // 禁用自定义死亡信息。
            TShock.Config.Settings.DisableCustomDeathMessages = Config.Instance.DisableCustomDeathMessages;

            #endregion

            #region Apply Chat Settings

            // 指定用于启动命令的字符串。
            TShock.Config.Settings.CommandSpecifier = Config.Instance.CommandSpecifier;

            // 指定用于静默启动命令的字符串。
            TShock.Config.Settings.CommandSilentSpecifier = Config.Instance.CommandSilentSpecifier;

            // 禁用将日志作为消息发送给具有日志权限的玩家。
            TShock.Config.Settings.DisableSpewLogs = Config.Instance.DisableSpewLogs;

            // 阻止 OnSecondUpdate 检查写入日志文件。
            TShock.Config.Settings.DisableSecondUpdateLogs = Config.Instance.DisableSecondUpdateLogs;

            // 超级管理员组的聊天颜色。
            TShock.Config.Settings.SuperAdminChatRGB = Config.Instance.SuperAdminChatRGB;

            // 超级管理员的聊天前缀。
            TShock.Config.Settings.SuperAdminChatPrefix = Config.Instance.SuperAdminChatPrefix;

            // 超级管理员的聊天后缀。
            TShock.Config.Settings.SuperAdminChatSuffix = Config.Instance.SuperAdminChatSuffix;

            // 是否在玩家加入时基于其 IP 地址宣布玩家的地理位置。
            TShock.Config.Settings.EnableGeoIP = Config.Instance.EnableGeoIP;

            // 是否在玩家加入时向具有日志权限的用户显示玩家的 IP 地址。
            TShock.Config.Settings.DisplayIPToAdmins = Config.Instance.DisplayIPToAdmins;

            // 更改游戏内聊天格式。
            TShock.Config.Settings.ChatFormat = Config.Instance.ChatFormat;

            // 更改使用头顶聊天时的玩家名称格式。
            TShock.Config.Settings.ChatAboveHeadsFormat = Config.Instance.ChatAboveHeadsFormat;

            // 是否在玩家头顶显示聊天消息。
            TShock.Config.Settings.EnableChatAboveHeads = Config.Instance.EnableChatAboveHeads;

            // 用于广播消息的颜色的 RGB 值。
            TShock.Config.Settings.BroadcastRGB = Config.Instance.BroadcastRGB;

            #endregion

            #region Apply MySQL Settings

            // 存储数据时使用的数据库类型: sqlite/mysql。
            TShock.Config.Settings.StorageType = Config.Instance.StorageType;

            // 本服务器的数据库路径。
            TShock.Config.Settings.SqliteDBPath = Config.Instance.SqliteDBPath;

            // Mysql连接的ip和端口。
            TShock.Config.Settings.MySqlHost = Config.Instance.MySqlHost;

            // Mysql的数据库名称。
            TShock.Config.Settings.MySqlDbName = Config.Instance.MySqlDbName;

            // Mysql的用户名。
            TShock.Config.Settings.MySqlUsername = Config.Instance.MySqlUsername;

            // Mysql的密码。
            TShock.Config.Settings.MySqlPassword = Config.Instance.MySqlPassword;

            // 是否把日志存入数据库。
            TShock.Config.Settings.UseSqlLogs = Config.Instance.UseSqlLogs;

            // Sql日志返回文本日志之前连接次数。
            TShock.Config.Settings.RevertToTextLogsOnSqlFailures = Config.Instance.RevertToTextLogsOnSqlFailures;

            #endregion

            #region Apply REST API Settings

            // 开启 Rest API。
            TShock.Config.Settings.RestApiEnabled = Config.Instance.RestApiEnabled;

            // Rest的端口。
            TShock.Config.Settings.RestApiPort = Config.Instance.RestApiPort;

            // 记录Rest连线。
            TShock.Config.Settings.LogRest = Config.Instance.LogRest;

            // 开启对Rest的权限认证。
            TShock.Config.Settings.EnableTokenEndpointAuthentication = Config.Instance.EnableTokenEndpointAuthentication;

            // Rest连接失败的请求次数。
            TShock.Config.Settings.RESTMaximumRequestsPerInterval = Config.Instance.RESTMaximumRequestsPerInterval;

            // Rest连接请求次数间隔/分钟。
            TShock.Config.Settings.RESTRequestBucketDecreaseIntervalMinutes = Config.Instance.RESTRequestBucketDecreaseIntervalMinutes;

            // Rest外部应用权限表。
            foreach (var tokenEntry in Config.Instance.ApplicationRestTokens)
            {
                if (!TShock.Config.Settings.ApplicationRestTokens.ContainsKey(tokenEntry.Key))
                {
                    TShock.Config.Settings.ApplicationRestTokens[tokenEntry.Key] = tokenEntry.Value;
                }
                else
                {
                    // 如果键已经存在，可以选择更新或保持现有值不变，这里选择更新
                    TShock.Config.Settings.ApplicationRestTokens[tokenEntry.Key] = tokenEntry.Value;
                }
            }

            #endregion
            TShock.Config.Write(Path.Combine(TShock.SavePath, "config.json"));
            this.OnReload(args.Player);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"同步配置时出错: {ex.Message}");
        }
    }

    private void CTC(CommandArgs args)
    {
        try
        {
            this.OnReload(args.Player);//先重载
            // 从服务器当前配置更新到自定义配置文件
            #region Sync ServerSettings to Config
            Config.Instance.ServerPassword = TShock.Config.Settings.ServerPassword;
            Config.Instance.ServerPort = TShock.Config.Settings.ServerPort;
            Config.Instance.MaxSlots = TShock.Config.Settings.MaxSlots;
            Config.Instance.ReservedSlots = TShock.Config.Settings.ReservedSlots;
            Config.Instance.ServerName = TShock.Config.Settings.ServerName;
            Config.Instance.UseServerName = TShock.Config.Settings.UseServerName;
            Config.Instance.LogPath = TShock.Config.Settings.LogPath;
            Config.Instance.DebugLogs = TShock.Config.Settings.DebugLogs;
            Config.Instance.DisableLoginBeforeJoin = TShock.Config.Settings.DisableLoginBeforeJoin;
            Config.Instance.IgnoreChestStacksOnLoad = TShock.Config.Settings.IgnoreChestStacksOnLoad;
            Config.Instance.WorldTileProvider = TShock.Config.Settings.WorldTileProvider;
            #endregion
            // 应用备份和保存设置
            #region Apply Backup and Save Settings

            Config.Instance.AutoSave = TShock.Config.Settings.AutoSave;
            Config.Instance.AnnounceSave = TShock.Config.Settings.AnnounceSave;
            Config.Instance.ShowBackupAutosaveMessages = TShock.Config.Settings.ShowBackupAutosaveMessages;
            Config.Instance.BackupInterval = TShock.Config.Settings.BackupInterval;
            Config.Instance.BackupKeepFor = TShock.Config.Settings.BackupKeepFor;
            Config.Instance.SaveWorldOnCrash = TShock.Config.Settings.SaveWorldOnCrash;
            Config.Instance.SaveWorldOnLastPlayerExit = TShock.Config.Settings.SaveWorldOnLastPlayerExit;

            #endregion
            #region Apply World Settings

            // 入侵事件的大小乘数
            Config.Instance.InvasionMultiplier = TShock.Config.Settings.InvasionMultiplier;

            // 默认刷怪生成上限
            Config.Instance.DefaultMaximumSpawns = TShock.Config.Settings.DefaultMaximumSpawns;

            // 刷怪间隔（数值越低，敌人生成越频繁）
            Config.Instance.DefaultSpawnRate = TShock.Config.Settings.DefaultSpawnRate;

            // 是否启用无限制入侵事件（仍需手动触发）
            Config.Instance.InfiniteInvasion = TShock.Config.Settings.InfiniteInvasion;

            // 设置 PvP 模式
            Config.Instance.PvPMode = TShock.Config.Settings.PvPMode;

            // 启用出生点保护
            Config.Instance.SpawnProtection = TShock.Config.Settings.SpawnProtection;

            // 出生点保护范围（单位：格）
            Config.Instance.SpawnProtectionRadius = TShock.Config.Settings.SpawnProtectionRadius;

            // 启用放置物块范围检查
            Config.Instance.RangeChecks = TShock.Config.Settings.RangeChecks;

            // 禁止非硬核玩家连接
            Config.Instance.HardcoreOnly = TShock.Config.Settings.HardcoreOnly;

            // 禁止软核玩家连接
            Config.Instance.MediumcoreOnly = TShock.Config.Settings.MediumcoreOnly;

            // 禁止非软核玩家连接
            Config.Instance.SoftcoreOnly = TShock.Config.Settings.SoftcoreOnly;

            // 禁用方块放置或移除
            Config.Instance.DisableBuild = TShock.Config.Settings.DisableBuild;

            // 禁止困难模式
            Config.Instance.DisableHardmode = TShock.Config.Settings.DisableHardmode;

            // 禁止生成地牢守卫
            Config.Instance.DisableDungeonGuardian = TShock.Config.Settings.DisableDungeonGuardian;

            // 禁用小丑炸弹生成
            Config.Instance.DisableClownBombs = TShock.Config.Settings.DisableClownBombs;

            // 禁用雪球炸弹生成
            Config.Instance.DisableSnowBalls = TShock.Config.Settings.DisableSnowBalls;

            // 禁用死亡时掉落墓碑
            Config.Instance.DisableTombstones = TShock.Config.Settings.DisableTombstones;

            // 禁用 Skeletron Prime 炸弹生成
            Config.Instance.DisablePrimeBombs = TShock.Config.Settings.DisablePrimeBombs;

            // 强制世界时间为白天或黑夜
            Config.Instance.ForceTime = TShock.Config.Settings.ForceTime;

            // 当PvP启用时，禁用隐身药水的效果
            Config.Instance.DisableInvisPvP = TShock.Config.Settings.DisableInvisPvP;

            // 禁止移动范围（未登录时）
            Config.Instance.MaxRangeForDisabled = TShock.Config.Settings.MaxRangeForDisabled;

            // 保护区域箱子
            Config.Instance.RegionProtectChests = TShock.Config.Settings.RegionProtectChests;

            // 保护区域箱子是否上锁
            Config.Instance.RegionProtectGemLocks = TShock.Config.Settings.RegionProtectGemLocks;

            // 忽略检查玩家是否可以更新弹幕
            Config.Instance.IgnoreProjUpdate = TShock.Config.Settings.IgnoreProjUpdate;

            // 忽略检查玩家是否可以销毁弹幕
            Config.Instance.IgnoreProjKill = TShock.Config.Settings.IgnoreProjKill;

            // 允许玩家破坏易碎方块
            Config.Instance.AllowCutTilesAndBreakables = TShock.Config.Settings.AllowCutTilesAndBreakables;

            // 允许在通常不能建造的地方放置冰块
            Config.Instance.AllowIce = TShock.Config.Settings.AllowIce;

            // 允许猩红在困难模式下蔓延
            Config.Instance.AllowCrimsonCreep = TShock.Config.Settings.AllowCrimsonCreep;

            // 允许腐化在困难模式下蔓延
            Config.Instance.AllowCorruptionCreep = TShock.Config.Settings.AllowCorruptionCreep;

            // 允许神圣在困难模式下蔓延
            Config.Instance.AllowHallowCreep = TShock.Config.Settings.AllowHallowCreep;

            // 统计200格之间的雕像生成NPC数量
            Config.Instance.StatueSpawn200 = TShock.Config.Settings.StatueSpawn200;

            // 统计600格之间的雕像生成NPC数量
            Config.Instance.StatueSpawn600 = TShock.Config.Settings.StatueSpawn600;

            // 统计整个世界雕像生成NPC数量
            Config.Instance.StatueSpawnWorld = TShock.Config.Settings.StatueSpawnWorld;

            // 阻止禁用物品生成或指令获取
            Config.Instance.PreventBannedItemSpawn = TShock.Config.Settings.PreventBannedItemSpawn;

            // 阻止玩家死后与世界互动
            Config.Instance.PreventDeadModification = TShock.Config.Settings.PreventDeadModification;

            // 阻止玩家放置无效风格的方块
            Config.Instance.PreventInvalidPlaceStyle = TShock.Config.Settings.PreventInvalidPlaceStyle;

            // 强制圣诞节事件全年发生
            Config.Instance.ForceXmas = TShock.Config.Settings.ForceXmas;

            // 强制万圣节事件全年发生
            Config.Instance.ForceHalloween = TShock.Config.Settings.ForceHalloween;

            // 允许管理员获取禁用物品
            Config.Instance.AllowAllowedGroupsToSpawnBannedItems = TShock.Config.Settings.AllowAllowedGroupsToSpawnBannedItems;

            // 玩家复活时间（秒）
            Config.Instance.RespawnSeconds = TShock.Config.Settings.RespawnSeconds;

            // 玩家BOSS战复活时间（秒）
            Config.Instance.RespawnBossSeconds = TShock.Config.Settings.RespawnBossSeconds;

            // 是否提示BOSS生成或事件入侵
            Config.Instance.AnonymousBossInvasions = TShock.Config.Settings.AnonymousBossInvasions;

            // 检测玩家血量上限（超过会被网住）
            Config.Instance.MaxHP = TShock.Config.Settings.MaxHP;

            // 检测玩家蓝量上限（惩罚同上）
            Config.Instance.MaxMP = TShock.Config.Settings.MaxMP;

            // 爆炸影响范围（单位：方块）
            Config.Instance.BombExplosionRadius = TShock.Config.Settings.BombExplosionRadius;

            // 是否直接给予玩家物品到其背包中（需要SSC支持）
            Config.Instance.GiveItemsDirectly = TShock.Config.Settings.GiveItemsDirectly;

            #endregion
            #region Apply Login and Ban Settings

            // 默认将新注册的用户放入的组名。
            Config.Instance.DefaultRegistrationGroupName = TShock.Config.Settings.DefaultRegistrationGroupName;

            // 默认将未注册玩家放入的组名。
            Config.Instance.DefaultGuestGroupName = TShock.Config.Settings.DefaultGuestGroupName;

            // 记住玩家离开的位置，基于玩家的 IP。此设置不会在服务器重启后保存。
            Config.Instance.RememberLeavePos = TShock.Config.Settings.RememberLeavePos;

            // 最大登录失败尝试次数，超过此次数后踢出玩家。
            Config.Instance.MaximumLoginAttempts = TShock.Config.Settings.MaximumLoginAttempts;

            // 是否在中核玩家死亡时将其踢出。
            Config.Instance.KickOnMediumcoreDeath = TShock.Config.Settings.KickOnMediumcoreDeath;

            // 如果中核玩家死亡，将给出的踢出原因。
            Config.Instance.MediumcoreKickReason = TShock.Config.Settings.MediumcoreKickReason;

            // 是否在中核玩家死亡时将其封禁。
            Config.Instance.BanOnMediumcoreDeath = TShock.Config.Settings.BanOnMediumcoreDeath;

            // 如果中核玩家死亡，将给出的封禁原因。
            Config.Instance.MediumcoreBanReason = TShock.Config.Settings.MediumcoreBanReason;

            // 默认情况下，禁用 IP 封禁（如果没有传递参数到封禁命令）。
            Config.Instance.DisableDefaultIPBan = TShock.Config.Settings.DisableDefaultIPBan;

            // 根据 whitelist.txt 文件中的 IP 地址启用或禁用白名单。
            Config.Instance.EnableWhitelist = TShock.Config.Settings.EnableWhitelist;

            // 玩家尝试加入时，如果不在白名单中，将给出的踢出原因。
            Config.Instance.WhitelistKickReason = TShock.Config.Settings.WhitelistKickReason;

            // 当服务器已满，玩家尝试加入时的踢出原因。
            Config.Instance.ServerFullReason = TShock.Config.Settings.ServerFullReason;

            // 当服务器已满且没有保留插槽时，玩家尝试加入时的踢出原因。
            Config.Instance.ServerFullNoReservedReason = TShock.Config.Settings.ServerFullNoReservedReason;

            // 是否在硬核玩家死亡时将其踢出。
            Config.Instance.KickOnHardcoreDeath = TShock.Config.Settings.KickOnHardcoreDeath;

            // 硬核玩家死亡时的踢出原因。
            Config.Instance.HardcoreKickReason = TShock.Config.Settings.HardcoreKickReason;

            // 是否在硬核玩家死亡时将其封禁。
            Config.Instance.BanOnHardcoreDeath = TShock.Config.Settings.BanOnHardcoreDeath;

            // 硬核玩家死亡时的封禁原因。
            Config.Instance.HardcoreBanReason = TShock.Config.Settings.HardcoreBanReason;

            // 如果启用了 GeoIP，将踢出被识别为使用代理的用户。
            Config.Instance.KickProxyUsers = TShock.Config.Settings.KickProxyUsers;

            // 是否要求所有玩家在游戏前注册或登录。
            Config.Instance.RequireLogin = TShock.Config.Settings.RequireLogin;

            // 允许用户登录任何账号，即使用户名与角色名不匹配。
            Config.Instance.AllowLoginAnyUsername = TShock.Config.Settings.AllowLoginAnyUsername;

            // 允许用户注册与角色名不匹配的用户名。
            Config.Instance.AllowRegisterAnyUsername = TShock.Config.Settings.AllowRegisterAnyUsername;

            // 新用户账户的最小密码长度。不能低于 4。
            Config.Instance.MinimumPasswordLength = TShock.Config.Settings.MinimumPasswordLength;

            // 确定 BCrypt 工作因子。增加此因子后，所有密码将在验证时升级为新工作因子。计算轮数是 2^n。增加时请谨慎。范围：5-31。
            Config.Instance.BCryptWorkFactor = TShock.Config.Settings.BCryptWorkFactor;

            // 禁止用户使用客户端 UUID 登录。
            Config.Instance.DisableUUIDLogin = TShock.Config.Settings.DisableUUIDLogin;

            // 踢出未发送 UUID 的客户端。
            Config.Instance.KickEmptyUUID = TShock.Config.Settings.KickEmptyUUID;

            // 如果在 1 秒钟内绘制的瓷砖数量超过此数值，将禁用玩家。
            Config.Instance.TilePaintThreshold = TShock.Config.Settings.TilePaintThreshold;

            // 是否在超出 TilePaint 阈值时踢出玩家。
            Config.Instance.KickOnTilePaintThresholdBroken = TShock.Config.Settings.KickOnTilePaintThresholdBroken;

            // 玩家或 NPC 可造成的最大伤害。
            Config.Instance.MaxDamage = TShock.Config.Settings.MaxDamage;

            // 投射物可造成的最大伤害。
            Config.Instance.MaxProjDamage = TShock.Config.Settings.MaxProjDamage;

            // 是否在超出 MaxDamage 阈值时踢出玩家。
            Config.Instance.KickOnDamageThresholdBroken = TShock.Config.Settings.KickOnDamageThresholdBroken;

            // 如果在 1 秒内杀死的瓷砖数量超过此值，将禁用玩家并撤销其操作。
            Config.Instance.TileKillThreshold = TShock.Config.Settings.TileKillThreshold;

            // 是否在超出 TileKill 阈值时踢出玩家。
            Config.Instance.KickOnTileKillThresholdBroken = TShock.Config.Settings.KickOnTileKillThresholdBroken;

            // 如果在 1 秒内放置的瓷砖数量超过此值，将禁用玩家并撤销其操作。
            Config.Instance.TilePlaceThreshold = TShock.Config.Settings.TilePlaceThreshold;

            // 是否在超出 TilePlace 阈值时踢出玩家。
            Config.Instance.KickOnTilePlaceThresholdBroken = TShock.Config.Settings.KickOnTilePlaceThresholdBroken;

            // 如果在 1 秒内超出此液体设置数值，将禁用玩家。
            Config.Instance.TileLiquidThreshold = TShock.Config.Settings.TileLiquidThreshold;

            // 是否在超出 TileLiquid 阈值时踢出玩家。
            Config.Instance.KickOnTileLiquidThresholdBroken = TShock.Config.Settings.KickOnTileLiquidThresholdBroken;

            // 是否忽略水晶子弹的碎片在投射物阈值统计中。
            Config.Instance.ProjIgnoreShrapnel = TShock.Config.Settings.ProjIgnoreShrapnel;

            // 如果在 1 秒内超出此投射物创建数，将禁用玩家。
            Config.Instance.ProjectileThreshold = TShock.Config.Settings.ProjectileThreshold;

            // 是否在超出 Projectile 阈值时踢出玩家。
            Config.Instance.KickOnProjectileThresholdBroken = TShock.Config.Settings.KickOnProjectileThresholdBroken;

            // 如果在 1 秒内发送的 HealOtherPlayer 包超过此数值，将禁用玩家。
            Config.Instance.HealOtherThreshold = TShock.Config.Settings.HealOtherThreshold;

            // 是否在超出 HealOther 阈值时踢出玩家。
            Config.Instance.KickOnHealOtherThresholdBroken = TShock.Config.Settings.KickOnHealOtherThresholdBroken;

            // 区域与出生点提示无权建筑。
            Config.Instance.SuppressPermissionFailureNotices = TShock.Config.Settings.SuppressPermissionFailureNotices;

            // 禁止修改后的天顶剑。
            Config.Instance.DisableModifiedZenith = TShock.Config.Settings.DisableModifiedZenith;

            // 禁用自定义死亡信息。
            Config.Instance.DisableCustomDeathMessages = TShock.Config.Settings.DisableCustomDeathMessages;

            #endregion
            #region Apply MySQL Settings

            // 存储数据时使用的数据库类型: sqlite/mysql。
            Config.Instance.StorageType = TShock.Config.Settings.StorageType;

            // 本服务器的数据库路径。
            Config.Instance.SqliteDBPath = TShock.Config.Settings.SqliteDBPath;

            // Mysql连接的ip和端口。
            Config.Instance.MySqlHost = TShock.Config.Settings.MySqlHost;

            // Mysql的数据库名称。
            Config.Instance.MySqlDbName = TShock.Config.Settings.MySqlDbName;

            // Mysql的用户名。
            Config.Instance.MySqlUsername = TShock.Config.Settings.MySqlUsername;

            // Mysql的密码。
            Config.Instance.MySqlPassword = TShock.Config.Settings.MySqlPassword;

            // 是否把日志存入数据库。
            Config.Instance.UseSqlLogs = TShock.Config.Settings.UseSqlLogs;

            // Sql日志返回文本日志之前连接次数。
            Config.Instance.RevertToTextLogsOnSqlFailures = TShock.Config.Settings.RevertToTextLogsOnSqlFailures;

            #endregion

            #region Apply REST API Settings

            // 开启 Rest API。
            Config.Instance.RestApiEnabled = TShock.Config.Settings.RestApiEnabled;

            // Rest的端口。
            Config.Instance.RestApiPort = TShock.Config.Settings.RestApiPort;

            // 记录Rest连线。
            Config.Instance.LogRest = TShock.Config.Settings.LogRest;

            // 开启对Rest的权限认证。
            Config.Instance.EnableTokenEndpointAuthentication = TShock.Config.Settings.EnableTokenEndpointAuthentication;

            // Rest连接失败的请求次数。
            Config.Instance.RESTMaximumRequestsPerInterval = TShock.Config.Settings.RESTMaximumRequestsPerInterval;

            // Rest连接请求次数间隔/分钟。
            Config.Instance.RESTRequestBucketDecreaseIntervalMinutes = TShock.Config.Settings.RESTRequestBucketDecreaseIntervalMinutes;

            // Rest外部应用权限表。
            foreach (var tokenEntry in TShock.Config.Settings.ApplicationRestTokens)
            {
                if (!Config.Instance.ApplicationRestTokens.ContainsKey(tokenEntry.Key))
                {
                    Config.Instance.ApplicationRestTokens[tokenEntry.Key] = tokenEntry.Value;
                }
                else
                {
                    // 如果键已经存在，可以选择更新或保持现有值不变，这里选择更新
                    Config.Instance.ApplicationRestTokens[tokenEntry.Key] = tokenEntry.Value;
                }
            }

            #endregion
            Config.Save();
            this.OnReload(args.Player);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"同步配置时出错: {ex.Message}");
        }
    }
}