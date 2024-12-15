﻿using Newtonsoft.Json;
using LazyAPI.ConfigFiles;
using LazyAPI;
using System.Collections.Generic;
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
    public string ServerPassword  ;

    // 服务器端口
    [LocalizedPropertyName(CultureType.Chinese, "服务器端口")]
    [LocalizedPropertyName(CultureType.English, "ServerPort")]
    public int ServerPort  ;

    // 服务器人数上限
    [LocalizedPropertyName(CultureType.Chinese, "服务器人数上限")]
    [LocalizedPropertyName(CultureType.English, "MaxSlots")]
    public int MaxSlots  ;

    // 服务器人满预留位
    [LocalizedPropertyName(CultureType.Chinese, "服务器人满预留位")]
    [LocalizedPropertyName(CultureType.English, "ReservedSlots")]
    public int ReservedSlots  ;

    // 服务器名称
    [LocalizedPropertyName(CultureType.Chinese, "服务器名称")]
    [LocalizedPropertyName(CultureType.English, "ServerName")]
    public string ServerName  ;

    // 是否使用服务器名称替代世界名称
    [LocalizedPropertyName(CultureType.Chinese, "是否使用服务器名称")]
    [LocalizedPropertyName(CultureType.English, "UseServerName")]
    public bool UseServerName  ;

    // 服务器日志存放路径
    [LocalizedPropertyName(CultureType.Chinese, "服务器日志存放路径")]
    [LocalizedPropertyName(CultureType.English, "LogPath")]
    public string LogPath  ;

    // 是否启用报错日志
    [LocalizedPropertyName(CultureType.Chinese, "是否启用报错日志")]
    [LocalizedPropertyName(CultureType.English, "DebugLogs")]
    public bool DebugLogs  ;

    // 禁用加入后登录
    [LocalizedPropertyName(CultureType.Chinese, "禁用加入后登录")]
    [LocalizedPropertyName(CultureType.English, "DisableLoginBeforeJoin")]
    public bool DisableLoginBeforeJoin  ;

    // 允许箱子中物品堆栈超出限制
    [LocalizedPropertyName(CultureType.Chinese, "允许箱子中物品堆叠超出限制")]
    [LocalizedPropertyName(CultureType.English, "IgnoreChestStacksOnLoad")]
    public bool IgnoreChestStacksOnLoad  ;

    // 世界图格提供器
    [LocalizedPropertyName(CultureType.Chinese, "世界图格提供器(heaptile/constileation)")]
    [LocalizedPropertyName(CultureType.English, "WorldTileProvider")]
    public string WorldTileProvider  ;

    #endregion

    #region Backup and Save Settings

    // 启用自动保存
    [LocalizedPropertyName(CultureType.Chinese, "启用自动保存")]
    [LocalizedPropertyName(CultureType.English, "AutoSave")]
    public bool AutoSave  ;

    // 自动保存世界时广播提示
    [LocalizedPropertyName(CultureType.Chinese, "自动保存世界广播提示")]
    [LocalizedPropertyName(CultureType.English, "AnnounceSave")]
    public bool AnnounceSave  ;

    // 是否显示备份自动保存消息。
    [LocalizedPropertyName(CultureType.Chinese, "是否显示备份自动保存消息")]
    [LocalizedPropertyName(CultureType.English, "ShowBackupAutosaveMessages")]
    public bool ShowBackupAutosaveMessages  ;

    // 备份间隔(分钟)
    [LocalizedPropertyName(CultureType.Chinese, "自动备份间隔(分钟)")]
    [LocalizedPropertyName(CultureType.English, "BackupInterval")]
    public int BackupInterval  ;

    // 备份保留时间(分钟)
    [LocalizedPropertyName(CultureType.Chinese, "备份保留时间(分钟)")]
    [LocalizedPropertyName(CultureType.English, "BackupKeepFor")]
    public int BackupKeepFor  ;

    // 崩溃时是否保存世界
    [LocalizedPropertyName(CultureType.Chinese, "崩溃时保存世界")]
    [LocalizedPropertyName(CultureType.English, "SaveWorldOnCrash")]
    public bool SaveWorldOnCrash  ;

    // 最后一名玩家退出时是否保存世界
    [LocalizedPropertyName(CultureType.Chinese, "最后玩家退出时保存世界")]
    [LocalizedPropertyName(CultureType.English, "SaveWorldOnLastPlayerExit")]
    public bool SaveWorldOnLastPlayerExit  ;

    #endregion

    #region World Settings

    // 入侵事件的大小，由公式 100 + (倍数 * (200血量以上活跃玩家数量)) 计算得出
    [LocalizedPropertyName(CultureType.Chinese, "事件入侵乘数(100 + 乘数*血量>200的在线玩家)")]
    [LocalizedPropertyName(CultureType.English, "InvasionMultiplier")]
    public int InvasionMultiplier  ;

    // 默认刷怪生成上限
    [LocalizedPropertyName(CultureType.Chinese, "默认刷怪生成上限")]
    [LocalizedPropertyName(CultureType.English, "DefaultMaximumSpawns")]
    public int DefaultMaximumSpawns  ;

    // 刷怪间隔(数值越低，敌人生成越频繁)
    [LocalizedPropertyName(CultureType.Chinese, "默认刷怪生成间隔(帧)")]
    [LocalizedPropertyName(CultureType.English, "DefaultSpawnRate")]
    public int DefaultSpawnRate  ;

    // 是否启用无限制入侵事件(仍需手动触发)
    [LocalizedPropertyName(CultureType.Chinese, "事件无限入侵(/invade)")]
    [LocalizedPropertyName(CultureType.English, "InfiniteInvasion")]
    public bool InfiniteInvasion  ;

    // 设置 PvP 模式(有效选项：normal、always、pvpwithnoteam、disabled)
    [LocalizedPropertyName(CultureType.Chinese, "设置PVP模式(normal/always/disabled/pvpwithnoteam)")]
    [LocalizedPropertyName(CultureType.English, "PvPMode")]
    public string PvPMode  ;

    // 防止在默认生成点的范围内放置方块
    [LocalizedPropertyName(CultureType.Chinese, "启用出生点保护")]
    [LocalizedPropertyName(CultureType.English, "SpawnProtection")]
    public bool SpawnProtection  ;

    // 默认生成点周围的保护范围(单位：格)
    [LocalizedPropertyName(CultureType.Chinese, "出生点保护范围(格)")]
    [LocalizedPropertyName(CultureType.English, "SpawnProtectionRadius")]
    public int SpawnProtectionRadius  ;

    // 启用或禁用基于玩家与方块放置距离的反作弊检查
    [LocalizedPropertyName(CultureType.Chinese, "启用放置物块范围检查")]
    [LocalizedPropertyName(CultureType.English, "RangeChecks")]
    public bool RangeChecks  ;

    // 禁止非硬核玩家连接
    [LocalizedPropertyName(CultureType.Chinese, "启用强制硬核角色")]
    [LocalizedPropertyName(CultureType.English, "HardcoreOnly")]
    public bool HardcoreOnly  ;

    // 禁止软核玩家连接
    [LocalizedPropertyName(CultureType.Chinese, "启用强制中核角色")]
    [LocalizedPropertyName(CultureType.English, "MediumcoreOnly")]
    public bool MediumcoreOnly  ;

    // 禁止非软核玩家连接
    [LocalizedPropertyName(CultureType.Chinese, "启用强制软核角色")]
    [LocalizedPropertyName(CultureType.English, "SoftcoreOnly")]
    public bool SoftcoreOnly  ;

    // 禁用方块放置或移除
    [LocalizedPropertyName(CultureType.Chinese, "禁止建筑")]
    [LocalizedPropertyName(CultureType.English, "DisableBuild")]
    public bool DisableBuild  ;

    // 如果启用，硬模式将不会通过魔眼或 /starthardmode 命令激活
    [LocalizedPropertyName(CultureType.Chinese, "禁止困难模式")]
    [LocalizedPropertyName(CultureType.English, "DisableHardmode")]
    public bool DisableHardmode  ;

    // 禁止副本守护神生成，取而代之的是将玩家发送回生成点
    [LocalizedPropertyName(CultureType.Chinese, "禁止生成地牢守卫")]
    [LocalizedPropertyName(CultureType.English, "DisableDungeonGuardian")]
    public bool DisableDungeonGuardian  ;

    // 禁用小丑炸弹生成
    [LocalizedPropertyName(CultureType.Chinese, "禁止血月小丑炸弹")]
    [LocalizedPropertyName(CultureType.English, "DisableClownBombs")]
    public bool DisableClownBombs  ;

    // 禁用雪球炸弹生成
    [LocalizedPropertyName(CultureType.Chinese, "禁止雪块弹幕")]
    [LocalizedPropertyName(CultureType.English, "DisableSnowBalls")]
    public bool DisableSnowBalls  ;

    // 禁用死亡时掉落墓碑
    [LocalizedPropertyName(CultureType.Chinese, "禁止玩家死亡生成墓碑")]
    [LocalizedPropertyName(CultureType.English, "DisableTombstones")]
    public bool DisableTombstones  ;

    // 禁用 Skeletron Prime 炸弹生成，防止破坏 “for the worthy” 世界
    [LocalizedPropertyName(CultureType.Chinese, "禁止炸弹")]
    [LocalizedPropertyName(CultureType.English, "DisablePrimeBombs")]
    public bool DisablePrimeBombs  ;

    // 强制世界时间为白天或黑夜
    [LocalizedPropertyName(CultureType.Chinese, "强制世界时间(normal/day/night)")]
    [LocalizedPropertyName(CultureType.English, "ForceTime")]
    public string ForceTime  ;

    // 当PvP启用时，禁用隐身药水的效果，使玩家对其他玩家可见。
    [LocalizedPropertyName(CultureType.Chinese, "禁止PVP隐身药水")]
    [LocalizedPropertyName(CultureType.English, "DisableInvisPvP")]
    public bool DisableInvisPvP  ;

    // 禁用玩家的移动范围(单位：方块)。
    [LocalizedPropertyName(CultureType.Chinese, "未登录时禁止移动范围")]
    [LocalizedPropertyName(CultureType.English, "MaxRangeForDisabled")]
    public int MaxRangeForDisabled  ;

    // 是否对箱子应用区域保护。
    [LocalizedPropertyName(CultureType.Chinese, "保护区域箱子")]
    [LocalizedPropertyName(CultureType.English, "RegionProtectChests")]
    public bool RegionProtectChests  ;

    // 是否对宝石锁应用区域保护。
    [LocalizedPropertyName(CultureType.Chinese, "保护区域箱子是否上锁")]
    [LocalizedPropertyName(CultureType.English, "RegionProtectGemLocks")]
    public bool RegionProtectGemLocks  ;

    // 忽略检查玩家是否可以更新弹幕。
    [LocalizedPropertyName(CultureType.Chinese, "忽略检查玩家弹幕更新")]
    [LocalizedPropertyName(CultureType.English, "IgnoreProjUpdate")]
    public bool IgnoreProjUpdate  ;

    // 忽略检查玩家是否可以销毁弹幕。
    [LocalizedPropertyName(CultureType.Chinese, "忽略检查玩家弹幕销毁")]
    [LocalizedPropertyName(CultureType.English, "IgnoreProjKill")]
    public bool IgnoreProjKill  ;

    // 允许玩家破坏临时方块(草、罐子等)，即使在通常不能建造的地方。
    [LocalizedPropertyName(CultureType.Chinese, "允许玩家破坏易碎方块")]
    [LocalizedPropertyName(CultureType.English, "AllowCutTilesAndBreakables")]
    public bool AllowCutTilesAndBreakables  ;

    // 允许在通常不能建造的地方放置冰块。
    [LocalizedPropertyName(CultureType.Chinese, "允许玩家保护区域释放冰块")]
    [LocalizedPropertyName(CultureType.English, "AllowIce")]
    public bool AllowIce  ;

    // 允许猩红在困难模式下蔓延。
    [LocalizedPropertyName(CultureType.Chinese, "允许困难模式猩红蔓延")]
    [LocalizedPropertyName(CultureType.English, "AllowCrimsonCreep")]
    public bool AllowCrimsonCreep  ;

    // 允许腐化在困难模式下蔓延。
    [LocalizedPropertyName(CultureType.Chinese, "允许困难模式腐化蔓延")]
    [LocalizedPropertyName(CultureType.English, "AllowCorruptionCreep")]
    public bool AllowCorruptionCreep  ;

    // 允许神圣在困难模式下蔓延。
    [LocalizedPropertyName(CultureType.Chinese, "允许困难模式神圣蔓延")]
    [LocalizedPropertyName(CultureType.English, "AllowHallowCreep")]
    public bool AllowHallowCreep  ;

    // 统计200格之间的雕像生成NPC数量。
    [LocalizedPropertyName(CultureType.Chinese, "200格之间雕像生成NPC数量")]
    [LocalizedPropertyName(CultureType.English, "StatueSpawn200")]
    public int StatueSpawn200  ;

    // 统计600格之间的雕像生成NPC数量。
    [LocalizedPropertyName(CultureType.Chinese, "600格之间雕像生成NPC数量")]
    [LocalizedPropertyName(CultureType.English, "StatueSpawn600")]
    public int StatueSpawn600  ;

    // 统计整个世界雕像生成NPC数量。
    [LocalizedPropertyName(CultureType.Chinese, "整个世界雕像生成NPC数量")]
    [LocalizedPropertyName(CultureType.English, "StatueSpawnWorld")]
    public int StatueSpawnWorld  ;

    // 阻止禁用物品生成或指令获取。
    [LocalizedPropertyName(CultureType.Chinese, "阻止禁用物品生成或指令获取")]
    [LocalizedPropertyName(CultureType.English, "PreventBannedItemSpawn")]
    public bool PreventBannedItemSpawn  ;

    // 阻止玩家死后与世界互动。
    [LocalizedPropertyName(CultureType.Chinese, "阻止玩家死后与世界互动")]
    [LocalizedPropertyName(CultureType.English, "PreventDeadModification")]
    public bool PreventDeadModification  ;

    // 阻止玩家放置无效风格的方块。
    [LocalizedPropertyName(CultureType.Chinese, "阻止玩家放置无效方块")]
    [LocalizedPropertyName(CultureType.English, "PreventInvalidPlaceStyle")]
    public bool PreventInvalidPlaceStyle  ;

    // 强制圣诞节事件全年发生。
    [LocalizedPropertyName(CultureType.Chinese, "强制圣诞节")]
    [LocalizedPropertyName(CultureType.English, "ForceXmas")]
    public bool ForceXmas  ;

    // 强制万圣节事件全年发生。
    [LocalizedPropertyName(CultureType.Chinese, "强制万圣节")]
    [LocalizedPropertyName(CultureType.English, "ForceHalloween")]
    public bool ForceHalloween  ;

    // 允许管理员获取禁用物品。
    [LocalizedPropertyName(CultureType.Chinese, "允许管理员获取禁用物品")]
    [LocalizedPropertyName(CultureType.English, "AllowAllowedGroupsToSpawnBannedItems")]
    public bool AllowAllowedGroupsToSpawnBannedItems  ;

    // 玩家复活时间(秒)。
    [LocalizedPropertyName(CultureType.Chinese, "玩家复活时间/秒")]
    [LocalizedPropertyName(CultureType.English, "RespawnSeconds")]
    public int RespawnSeconds  ;

    // 玩家BOSS战复活时间(秒)。
    [LocalizedPropertyName(CultureType.Chinese, "玩家BOSS战复活时间/秒")]
    [LocalizedPropertyName(CultureType.English, "RespawnBossSeconds")]
    public int RespawnBossSeconds  ;

    // 是否提示BOSS生成或事件入侵。
    [LocalizedPropertyName(CultureType.Chinese, "提示BOSS生成或事件入侵")]
    [LocalizedPropertyName(CultureType.English, "AnonymousBossInvasions")]
    public bool AnonymousBossInvasions  ;

    // 检测玩家血量上限(超过会被网住)。
    [LocalizedPropertyName(CultureType.Chinese, "检测玩家血量上限(超过会被网住)")]
    [LocalizedPropertyName(CultureType.English, "MaxHP")]
    public int MaxHP  ;

    // 检测玩家蓝量上限(惩罚同上)。
    [LocalizedPropertyName(CultureType.Chinese, "检测玩家蓝量上限(惩罚同上)")]
    [LocalizedPropertyName(CultureType.English, "MaxMP")]
    public int MaxMP  ;

    // 爆炸影响范围(单位：方块)。
    [LocalizedPropertyName(CultureType.Chinese, "爆炸影响范围")]
    [LocalizedPropertyName(CultureType.English, "BombExplosionRadius")]
    public int BombExplosionRadius  ;

    // 是否直接给予玩家物品到其背包中(需要SSC支持)。
    [LocalizedPropertyName(CultureType.Chinese, "强制开荒下给玩家物品方式")]
    [LocalizedPropertyName(CultureType.English, "GiveItemsDirectly")]
    public bool GiveItemsDirectly  ;

    #endregion

    #region Login and Ban Settings

    // 默认将新注册的用户放入的组名。
    [LocalizedPropertyName(CultureType.Chinese, "玩家注册后的用户组")]
    [LocalizedPropertyName(CultureType.English, "DefaultRegistrationGroupName")]
    public string DefaultRegistrationGroupName  ;

    // 默认将未注册玩家放入的组名。
    [LocalizedPropertyName(CultureType.Chinese, "玩家注册前的用户组")]
    [LocalizedPropertyName(CultureType.English, "DefaultGuestGroupName")]
    public string DefaultGuestGroupName  ;

    // 记住玩家离开的位置，基于玩家的 IP。此设置不会在服务器重启后保存。
    [LocalizedPropertyName(CultureType.Chinese, "进服传回离线时的位置")]
    [LocalizedPropertyName(CultureType.English, "RememberLeavePos")]
    public bool RememberLeavePos  ;

    // 最大登录失败尝试次数，超过此次数后踢出玩家。
    [LocalizedPropertyName(CultureType.Chinese, "尝试登录次数上限/超过则踢出")]
    [LocalizedPropertyName(CultureType.English, "MaximumLoginAttempts")]
    public int MaximumLoginAttempts  ;

    // 是否在中核玩家死亡时将其踢出。
    [LocalizedPropertyName(CultureType.Chinese, "踢出死亡后的中核玩家")]
    [LocalizedPropertyName(CultureType.English, "KickOnMediumcoreDeath")]
    public bool KickOnMediumcoreDeath  ;

    // 如果中核玩家死亡，将给出的踢出原因。
    [LocalizedPropertyName(CultureType.Chinese, "踢出死亡后的中核玩家信息")]
    [LocalizedPropertyName(CultureType.English, "MediumcoreKickReason")]
    public string MediumcoreKickReason  ;

    // 是否在中核玩家死亡时将其封禁。
    [LocalizedPropertyName(CultureType.Chinese, "封禁死亡后的中核玩家")]
    [LocalizedPropertyName(CultureType.English, "BanOnMediumcoreDeath")]
    public bool BanOnMediumcoreDeath  ;

    // 如果中核玩家死亡，将给出的封禁原因。
    [LocalizedPropertyName(CultureType.Chinese, "封禁死亡后的中核玩家信息")]
    [LocalizedPropertyName(CultureType.English, "MediumcoreBanReason")]
    public string MediumcoreBanReason  ;

    // 默认情况下，禁用 IP 封禁(如果没有传递参数到封禁命令)。
    [LocalizedPropertyName(CultureType.Chinese, "封禁默认只封IP")]
    [LocalizedPropertyName(CultureType.English, "DisableDefaultIPBan")]
    public bool DisableDefaultIPBan  ;

    // 根据 whitelist.txt 文件中的 IP 地址启用或禁用白名单。
    [LocalizedPropertyName(CultureType.Chinese, "启用白名单(推荐用其他白名单插件)")]
    [LocalizedPropertyName(CultureType.English, "EnableWhitelist")]
    public bool EnableWhitelist  ;

    // 玩家尝试加入时，如果不在白名单中，将给出的踢出原因。
    [LocalizedPropertyName(CultureType.Chinese, "不在白名单被踢出的信息")]
    [LocalizedPropertyName(CultureType.English, "WhitelistKickReason")]
    public string WhitelistKickReason  ;

    // 当服务器已满，玩家尝试加入时的踢出原因。
    [LocalizedPropertyName(CultureType.Chinese, "服务器人满提示信息")]
    [LocalizedPropertyName(CultureType.English, "ServerFullReason")]
    public string ServerFullReason  ;

    // 当服务器已满且没有保留插槽时，玩家尝试加入时的踢出原因。
    [LocalizedPropertyName(CultureType.Chinese, "服务器人满且没有保留位的提示信息")]
    [LocalizedPropertyName(CultureType.English, "ServerFullNoReservedReason")]
    public string ServerFullNoReservedReason  ;

    // 是否在硬核玩家死亡时将其踢出。
    [LocalizedPropertyName(CultureType.Chinese, "踢出死亡后的硬核玩家")]
    [LocalizedPropertyName(CultureType.English, "KickOnHardcoreDeath")]
    public bool KickOnHardcoreDeath  ;

    // 硬核玩家死亡时的踢出原因。
    [LocalizedPropertyName(CultureType.Chinese, "踢出死亡后的硬核玩家信息")]
    [LocalizedPropertyName(CultureType.English, "HardcoreKickReason")]
    public string HardcoreKickReason  ;

    // 是否在硬核玩家死亡时将其封禁。
    [LocalizedPropertyName(CultureType.Chinese, "封禁死亡后的硬核玩家")]
    [LocalizedPropertyName(CultureType.English, "BanOnHardcoreDeath")]
    public bool BanOnHardcoreDeath  ;

    // 硬核玩家死亡时的封禁原因。
    [LocalizedPropertyName(CultureType.Chinese, "封禁死亡后的硬核玩家信息")]
    [LocalizedPropertyName(CultureType.English, "HardcoreBanReason")]
    public string HardcoreBanReason  ;

    // 如果启用了 GeoIP，将踢出被识别为使用代理的用户。
    [LocalizedPropertyName(CultureType.Chinese, "踢出代理IP用户")]
    [LocalizedPropertyName(CultureType.English, "KickProxyUsers")]
    public bool KickProxyUsers  ;

    // 是否要求所有玩家在游戏前注册或登录。
    [LocalizedPropertyName(CultureType.Chinese, "启用注册登录系统务必开启")]
    [LocalizedPropertyName(CultureType.English, "RequireLogin")]
    public bool RequireLogin  ;

    // 允许用户登录任何账号，即使用户名与角色名不匹配。
    [LocalizedPropertyName(CultureType.Chinese, "允许玩家登录账号与角色名不符")]
    [LocalizedPropertyName(CultureType.English, "AllowLoginAnyUsername")]
    public bool AllowLoginAnyUsername  ;

    // 允许用户注册与角色名不匹配的用户名。
    [LocalizedPropertyName(CultureType.Chinese, "允许玩家注册账号与角色名不符")]
    [LocalizedPropertyName(CultureType.English, "AllowRegisterAnyUsername")]
    public bool AllowRegisterAnyUsername  ;

    // 新用户账户的最小密码长度。不能低于 4。
    [LocalizedPropertyName(CultureType.Chinese, "密码最少长度(最少4位)")]
    [LocalizedPropertyName(CultureType.English, "MinimumPasswordLength")]
    public int MinimumPasswordLength  ;

    // 确定 BCrypt 工作因子。增加此因子后，所有密码将在验证时升级为新工作因子。计算轮数是 2^n。增加时请谨慎。范围：5-31。
    [LocalizedPropertyName(CultureType.Chinese, "使用的BCrypt工作因子")]
    [LocalizedPropertyName(CultureType.English, "BCryptWorkFactor")]
    public int BCryptWorkFactor  ;

    // 禁止用户使用客户端 UUID 登录。
    [LocalizedPropertyName(CultureType.Chinese, "禁止UUID自动登录")]
    [LocalizedPropertyName(CultureType.English, "DisableUUIDLogin")]
    public bool DisableUUIDLogin  ;

    // 踢出未发送 UUID 的客户端。
    [LocalizedPropertyName(CultureType.Chinese, "踢出不发送UUID到服务器的玩家")]
    [LocalizedPropertyName(CultureType.English, "KickEmptyUUID")]
    public bool KickEmptyUUID  ;

    // 如果在 1 秒钟内绘制的瓷砖数量超过此数值，将禁用玩家。
    [LocalizedPropertyName(CultureType.Chinese, "1秒内刷油漆的格数上限")]
    [LocalizedPropertyName(CultureType.English, "TilePaintThreshold")]
    public int TilePaintThreshold  ;

    // 是否在超出 TilePaint 阈值时踢出玩家。
    [LocalizedPropertyName(CultureType.Chinese, "踢出1秒内刷油漆超出格数上限的玩家")]
    [LocalizedPropertyName(CultureType.English, "KickOnTilePaintThresholdBroken")]
    public bool KickOnTilePaintThresholdBroken  ;

    // 玩家或 NPC 可造成的最大伤害。
    [LocalizedPropertyName(CultureType.Chinese, "玩家最大伤害上限")]
    [LocalizedPropertyName(CultureType.English, "MaxDamage")]
    public int MaxDamage  ;

    // 投射物可造成的最大伤害。
    [LocalizedPropertyName(CultureType.Chinese, "玩家最大弹幕伤害上限")]
    [LocalizedPropertyName(CultureType.English, "MaxProjDamage")]
    public int MaxProjDamage  ;

    // 是否在超出 MaxDamage 阈值时踢出玩家。
    [LocalizedPropertyName(CultureType.Chinese, "踢出超出伤害上限的玩家")]
    [LocalizedPropertyName(CultureType.English, "KickOnDamageThresholdBroken")]
    public bool KickOnDamageThresholdBroken  ;

    // 如果在 1 秒内杀死的瓷砖数量超过此值，将禁用玩家并撤销其操作。
    [LocalizedPropertyName(CultureType.Chinese, "1秒内破坏方块的格数上限")]
    [LocalizedPropertyName(CultureType.English, "TileKillThreshold")]
    public int TileKillThreshold  ;

    // 是否在超出 TileKill 阈值时踢出玩家。
    [LocalizedPropertyName(CultureType.Chinese, "踢出1秒内破坏方块超出格数上限的玩家")]
    [LocalizedPropertyName(CultureType.English, "KickOnTileKillThresholdBroken")]
    public bool KickOnTileKillThresholdBroken  ;

    // 如果在 1 秒内放置的瓷砖数量超过此值，将禁用玩家并撤销其操作。
    [LocalizedPropertyName(CultureType.Chinese, "1秒内放置方块的格数上限")]
    [LocalizedPropertyName(CultureType.English, "TilePlaceThreshold")]
    public int TilePlaceThreshold  ;

    // 是否在超出 TilePlace 阈值时踢出玩家。
    [LocalizedPropertyName(CultureType.Chinese, "踢出1秒内放置方块超出格数上限的玩家")]
    [LocalizedPropertyName(CultureType.English, "KickOnTilePlaceThresholdBroken")]
    public bool KickOnTilePlaceThresholdBroken  ;

    // 如果在 1 秒内超出此液体设置数值，将禁用玩家。
    [LocalizedPropertyName(CultureType.Chinese, "1秒内放置液体的格数上限")]
    [LocalizedPropertyName(CultureType.English, "TileLiquidThreshold")]
    public int TileLiquidThreshold  ;

    // 是否在超出 TileLiquid 阈值时踢出玩家。
    [LocalizedPropertyName(CultureType.Chinese, "踢出1秒内放置液体超出格数上限的玩家")]
    [LocalizedPropertyName(CultureType.English, "KickOnTileLiquidThresholdBroken")]
    public bool KickOnTileLiquidThresholdBroken  ;

    // 是否忽略水晶子弹的碎片在投射物阈值统计中。
    [LocalizedPropertyName(CultureType.Chinese, "弹幕数量是否包含水晶子弹")]
    [LocalizedPropertyName(CultureType.English, "ProjIgnoreShrapnel")]
    public bool ProjIgnoreShrapnel  ;

    // 如果在 1 秒内超出此投射物创建数，将禁用玩家。
    [LocalizedPropertyName(CultureType.Chinese, "1秒内释放弹幕的数量上限")]
    [LocalizedPropertyName(CultureType.English, "ProjectileThreshold")]
    public int ProjectileThreshold  ;

    // 是否在超出 Projectile 阈值时踢出玩家。
    [LocalizedPropertyName(CultureType.Chinese, "踢出1秒内释放弹幕超出数量上限的玩家")]
    [LocalizedPropertyName(CultureType.English, "KickOnProjectileThresholdBroken")]
    public bool KickOnProjectileThresholdBroken  ;

    // 如果在 1 秒内发送的 HealOtherPlayer 包超过此数值，将禁用玩家。
    [LocalizedPropertyName(CultureType.Chinese, "1秒内治疗其他玩家的数值上限")]
    [LocalizedPropertyName(CultureType.English, "HealOtherThreshold")]
    public int HealOtherThreshold  ;

    // 是否在超出 HealOther 阈值时踢出玩家。
    [LocalizedPropertyName(CultureType.Chinese, "踢出1秒内治疗其他玩家超出数值上限的玩家")]
    [LocalizedPropertyName(CultureType.English, "KickOnHealOtherThresholdBroken")]
    public bool KickOnHealOtherThresholdBroken  ;

    /// <summary>
    /// 描述: 区域与出生点提示无权建筑。
    /// </summary>
    [LocalizedPropertyName(CultureType.Chinese, "区域与出生点提示无权建筑")]
    [LocalizedPropertyName(CultureType.English, "SuppressPermissionFailureNotices")]
    public bool SuppressPermissionFailureNotices  ;

    /// <summary>
    /// 描述: 禁止修改后的天顶剑。
    /// </summary>
    [LocalizedPropertyName(CultureType.Chinese, "禁止第一分形")]
    [LocalizedPropertyName(CultureType.English, "DisableModifiedZenith")]
    public bool DisableModifiedZenith  ;

    /// <summary>
    /// 描述: 禁用自定义死亡信息。
    /// </summary>
    [LocalizedPropertyName(CultureType.Chinese, "禁用自定义死亡信息")]
    [LocalizedPropertyName(CultureType.English, "DisableCustomDeathMessages")]
    public bool DisableCustomDeathMessages  ;

    #endregion

    #region Chat Settings

    /// <summary>
    /// 指定用于启动命令的字符串。
    /// 注意：如果字符串长度大于 1，可能无法正常工作。
    /// </summary>
    [LocalizedPropertyName(CultureType.Chinese, "指令通用符")]
    [LocalizedPropertyName(CultureType.English, "CommandSpecifier")]
    public string CommandSpecifier  ;

    /// <summary>
    /// 指定用于静默启动命令的字符串。
    /// 注意：如果字符串长度大于 1，可能无法正常工作。
    /// </summary>
    [LocalizedPropertyName(CultureType.Chinese, "隐藏指令通用符")]
    [LocalizedPropertyName(CultureType.English, "CommandSilentSpecifier")]
    public string CommandSilentSpecifier  ;

    /// <summary>
    /// 禁用将日志作为消息发送给具有日志权限的玩家。
    /// </summary>
    [LocalizedPropertyName(CultureType.Chinese, "禁止发送日志信息给有日志权限的玩家")]
    [LocalizedPropertyName(CultureType.English, "DisableSpewLogs")]
    public bool DisableSpewLogs  ;

    /// <summary>
    /// 阻止 OnSecondUpdate 检查写入日志文件。
    /// </summary>
    [LocalizedPropertyName(CultureType.Chinese, "禁止每秒更新检查写入日志路径")]
    [LocalizedPropertyName(CultureType.English, "DisableSecondUpdateLogs")]
    public bool DisableSecondUpdateLogs  ;

    /// <summary>
    /// 超级管理员组的聊天颜色。
    /// </summary>
    [LocalizedPropertyName(CultureType.Chinese, "超级管理员的聊天颜色")]
    [LocalizedPropertyName(CultureType.English, "SuperAdminChatRGB")]
    public int[] SuperAdminChatRGB  ;

    /// <summary>
    /// 超级管理员的聊天前缀。
    /// </summary>
    [LocalizedPropertyName(CultureType.Chinese, "超管的聊天前缀")]
    [LocalizedPropertyName(CultureType.English, "SuperAdminChatPrefix")]
    public string SuperAdminChatPrefix  ;

    /// <summary>
    /// 超级管理员的聊天后缀。
    /// </summary>
    [LocalizedPropertyName(CultureType.Chinese, "超管的聊天后缀")]
    [LocalizedPropertyName(CultureType.English, "SuperAdminChatSuffix")]
    public string SuperAdminChatSuffix  ;

    /// <summary>
    /// 是否在玩家加入时基于其 IP 地址宣布玩家的地理位置。
    /// </summary>
    [LocalizedPropertyName(CultureType.Chinese, "显示加入服务器的玩家IP国籍")]
    [LocalizedPropertyName(CultureType.English, "EnableGeoIP")]
    public bool EnableGeoIP  ;

    /// <summary>
    /// 是否在玩家加入时向具有日志权限的用户显示玩家的 IP 地址。
    /// </summary>
    [LocalizedPropertyName(CultureType.Chinese, "显示加入服务器的玩家IP")]
    [LocalizedPropertyName(CultureType.English, "DisplayIPToAdmins")]
    public bool DisplayIPToAdmins  ;

    /// <summary>
    /// 更改游戏内聊天格式：{0}  组名，{1}  组前缀，{2}  玩家名称，{3}  组后缀，{4}  聊天消息。
    /// </summary>
    [LocalizedPropertyName(CultureType.Chinese, "聊天格式(组名0/前缀1/玩家名字2/后缀3/内容4)")]
    [LocalizedPropertyName(CultureType.English, "ChatFormat")]
    public string ChatFormat  ;

    /// <summary>
    /// 更改使用头顶聊天时的玩家名称。以玩家名称包裹在括号中，符合 Terraria 的格式。\n与 ChatFormat 相同的格式，但不包含消息。
    /// </summary>
    [LocalizedPropertyName(CultureType.Chinese, "聊天栏内玩家名字的占位符")]
    [LocalizedPropertyName(CultureType.English, "ChatAboveHeadsFormat")]
    public string ChatAboveHeadsFormat  ;

    /// <summary>
    /// 是否在玩家头顶显示聊天消息。
    /// </summary>
    [LocalizedPropertyName(CultureType.Chinese, "是否在玩家头顶显示聊天消息")]
    [LocalizedPropertyName(CultureType.English, "EnableChatAboveHeads")]
    public bool EnableChatAboveHeads  ;

    /// <summary>
    /// 用于广播消息的颜色的 RGB 值。
    /// </summary>
    [LocalizedPropertyName(CultureType.Chinese, "系统广播文字颜色")]
    [LocalizedPropertyName(CultureType.English, "BroadcastRGB")]
    public int[] BroadcastRGB  ;

    #endregion

    #region MySQL Settings

    /// <summary>
    /// 存储数据时使用的数据库类型: sqlite/mysql。
    /// </summary>
    [LocalizedPropertyName(CultureType.Chinese, "数据库类型(sqlite/mysql)")]
    [LocalizedPropertyName(CultureType.English, "StorageType")]
    public string StorageType  ;

    /// <summary>
    /// 本服务器的数据库路径。
    /// </summary>
    [LocalizedPropertyName(CultureType.Chinese, "本服务器的数据库路径")]
    [LocalizedPropertyName(CultureType.English, "SqliteDBPath")]
    public string SqliteDBPath  ;

    /// <summary>
    /// Mysql连接的ip和端口。
    /// </summary>
    [LocalizedPropertyName(CultureType.Chinese, "Mysql连接的ip和端口")]
    [LocalizedPropertyName(CultureType.English, "MySqlHost")]
    public string MySqlHost  ;

    /// <summary>
    /// Mysql的数据库名称。
    /// </summary>
    [LocalizedPropertyName(CultureType.Chinese, "Mysql的数据库名称")]
    [LocalizedPropertyName(CultureType.English, "MySqlDbName")]
    public string MySqlDbName  ;

    /// <summary>
    /// Mysql的用户名。
    /// </summary>
    [LocalizedPropertyName(CultureType.Chinese, "Mysql的用户名")]
    [LocalizedPropertyName(CultureType.English, "MySqlUsername")]
    public string MySqlUsername  ;

    /// <summary>
    /// Mysql的密码。
    /// </summary>
    [LocalizedPropertyName(CultureType.Chinese, "Mysql的密码")]
    [LocalizedPropertyName(CultureType.English, "MySqlPassword")]
    public string MySqlPassword  ;

    /// <summary>
    /// 是否把日志存入数据库。
    /// </summary>
    [LocalizedPropertyName(CultureType.Chinese, "是否把日志存入数据库")]
    [LocalizedPropertyName(CultureType.English, "UseSqlLogs")]
    public bool UseSqlLogs  ;

    /// <summary>
    /// Sql日志返回文本日志之前连接次数。
    /// </summary>
    [LocalizedPropertyName(CultureType.Chinese, "Sql日志返回文本日志之前连接次数")]
    [LocalizedPropertyName(CultureType.English, "RevertToTextLogsOnSqlFailures")]
    public int RevertToTextLogsOnSqlFailures  ;

    #endregion

    #region REST API Settings

    /// <summary>
    /// 开启 Rest API。
    /// </summary>
    [LocalizedPropertyName(CultureType.Chinese, "开启 Rest API")]
    [LocalizedPropertyName(CultureType.English, "RestApiEnabled")]
    public bool RestApiEnabled  ;

    /// <summary>
    /// Rest的端口。
    /// </summary>
    [LocalizedPropertyName(CultureType.Chinese, "Rest的端口")]
    [LocalizedPropertyName(CultureType.English, "RestApiPort")]
    public int RestApiPort  ;

    /// <summary>
    /// 记录Rest连线。
    /// </summary>
    [LocalizedPropertyName(CultureType.Chinese, "记录Rest连线")]
    [LocalizedPropertyName(CultureType.English, "LogRest")]
    public bool LogRest  ;

    /// <summary>
    /// 开启对Rest的权限认证。
    /// </summary>
    [LocalizedPropertyName(CultureType.Chinese, "开启对Rest的权限认证")]
    [LocalizedPropertyName(CultureType.English, "EnableTokenEndpointAuthentication")]
    public bool EnableTokenEndpointAuthentication  ;

    /// <summary>
    /// Rest连接失败的请求次数。
    /// </summary>
    [LocalizedPropertyName(CultureType.Chinese, "Rest连接失败的请求次数")]
    [LocalizedPropertyName(CultureType.English, "RESTMaximumRequestsPerInterval")]
    public int RESTMaximumRequestsPerInterval  ;

    /// <summary>
    /// Rest连接请求次数间隔/分钟。
    /// </summary>
    [LocalizedPropertyName(CultureType.Chinese, "Rest连接请求次数间隔/分钟")]
    [LocalizedPropertyName(CultureType.English, "RESTRequestBucketDecreaseIntervalMinutes")]
    public int RESTRequestBucketDecreaseIntervalMinutes  ;

    /// <summary>
    /// Rest外部应用权限表。
    /// </summary>
    [LocalizedPropertyName(CultureType.Chinese, "Rest外部应用权限表")]
    [LocalizedPropertyName(CultureType.English, "ApplicationRestTokens")]
    public Dictionary<string, SecureRest.TokenData> ApplicationRestTokens  ;

    #endregion
}