# TShockConfigMultiLang 创建一个本地化语言的config

- Authors: 肝帝熙恩，羽学
- Source: This Repository
- Create a localized language config, modify the config by changing it, and vice versa. After synchronization, it will automatically reload.
- It may conflict with some plugins that operate on TShock's own config. Generally, this can be resolved manually.

## Commands

| Syntax                      |                     Permission                    |                            Description                            |
| --------------------------- | :-----------------------------------------------: | :---------------------------------------------------------------: |
| /configToNewconfig or /ctc  | tshock.cfg.reload | Synchronize the original config file to the localized config file |
| /configToFromconfig or /cfc | tshock.cfg.reload | Synchronize the localized config file to the original config file |

## Config

> Configuration file location: tshock/config.en-US.json

```json5
{
    "ServerPassword": "",
    "ServerPort": 7777,
    "MaxSlots": 8,
    "ReservedSlots": 20,
    "ServerName": "",
    "UseServerName": false,
    "LogPath": "tshock/logs",
    "DebugLogs": false,
    "DisableLoginBeforeJoin": false,
    "IgnoreChestStacksOnLoad": false,
    "WorldTileProvider": "default",
    "AutoSave": true,
    "AnnounceSave": true,
    "ShowBackupAutosaveMessages": true,
    "BackupInterval": 10,
    "BackupKeepFor": 240,
    "SaveWorldOnCrash": true,
    "SaveWorldOnLastPlayerExit": true,
    "InvasionMultiplier": 1,
    "DefaultMaximumSpawns": 5,
    "DefaultSpawnRate": 600,
    "InfiniteInvasion": false,
    "PvPMode": "normal",
    "SpawnProtection": true,
    "SpawnProtectionRadius": 10,
    "RangeChecks": true,
    "ProjRangeChecks": 150,
    "HardcoreOnly": false,
    "MediumcoreOnly": false,
    "SoftcoreOnly": false,
    "DisableBuild": false,
    "DisableHardmode": false,
    "DisableDungeonGuardian": false,
    "DisableClownBombs": false,
    "DisableSnowBalls": false,
    "DisableTombstones": true,
    "DisablePrimeBombs": true,
    "ForceTime": "normal",
    "DisableInvisPvP": false,
    "MaxRangeForDisabled": 10,
    "RegionProtectChests": false,
    "RegionProtectGemLocks": true,
    "IgnoreProjUpdate": false,
    "IgnoreProjKill": false,
    "AllowCutTilesAndBreakables": false,
    "AllowIce": false,
    "AllowCrimsonCreep": true,
    "AllowCorruptionCreep": true,
    "AllowHallowCreep": true,
    "StatueSpawn200": 3,
    "StatueSpawn600": 6,
    "StatueSpawnWorld": 10,
    "PreventBannedItemSpawn": false,
    "PreventDeadModification": true,
    "PreventInvalidPlaceStyle": true,
    "ForceXmas": false,
    "ForceHalloween": false,
    "AllowAllowedGroupsToSpawnBannedItems": false,
    "RespawnSeconds": 0,
    "RespawnBossSeconds": 0,
    "AnonymousBossInvasions": true,
    "MaxHP": 1000,
    "MaxMP": 400,
    "BombExplosionRadius": 5,
    "GiveItemsDirectly": false,
    "DefaultRegistrationGroupName": "default",
    "DefaultGuestGroupName": "guest",
    "RememberLeavePos": false,
    "MaximumLoginAttempts": 3,
    "KickOnMediumcoreDeath": false,
    "MediumcoreKickReason": "Death results in a kick",
    "BanOnMediumcoreDeath": false,
    "MediumcoreBanReason": "因为死亡而被封禁",
    "DisableDefaultIPBan": false,
    "EnableWhitelist": false,
    "WhitelistKickReason": "你不在白名单中。",
    "ServerFullReason": "服务器已满",
    "ServerFullNoReservedReason": "服务器已满（包括预留空间）。",
    "KickOnHardcoreDeath": false,
    "HardcoreKickReason": "因为死亡而被踢出",
    "BanOnHardcoreDeath": false,
    "HardcoreBanReason": "因为死亡而被封禁",
    "KickProxyUsers": true,
    "RequireLogin": false,
    "AllowLoginAnyUsername": true,
    "AllowRegisterAnyUsername": false,
    "MinimumPasswordLength": 4,
    "BCryptWorkFactor": 7,
    "DisableUUIDLogin": false,
    "KickEmptyUUID": false,
    "TilePaintThreshold": 15,
    "KickOnTilePaintThresholdBroken": false,
    "MaxDamage": 1175,
    "MaxProjDamage": 1175,
    "KickOnDamageThresholdBroken": false,
    "TileKillThreshold": 60,
    "KickOnTileKillThresholdBroken": false,
    "TilePlaceThreshold": 32,
    "KickOnTilePlaceThresholdBroken": false,
    "TileLiquidThreshold": 50,
    "KickOnTileLiquidThresholdBroken": false,
    "ProjIgnoreShrapnel": true,
    "ProjectileThreshold": 50,
    "KickOnProjectileThresholdBroken": false,
    "HealOtherThreshold": 50,
    "KickOnHealOtherThresholdBroken": false,
    "SuppressPermissionFailureNotices": false,
    "DisableModifiedZenith": false,
    "DisableCustomDeathMessages": true,
    "CommandSpecifier": "/",
    "CommandSilentSpecifier": ".",
    "DisableSpewLogs": true,
    "DisableSecondUpdateLogs": false,
    "SuperAdminChatRGB": [
      255,
      255,
      255
    ],
    "SuperAdminChatPrefix": "（超级管理员）",
    "SuperAdminChatSuffix": "",
    "EnableGeoIP": false,
    "DisplayIPToAdmins": false,
    "ChatFormat": "{1}{2}{3}: {4}",
    "ChatAboveHeadsFormat": "{2}",
    "EnableChatAboveHeads": false,
    "BroadcastRGB": [
      127,
      255,
      212
    ],
    "StorageType": "sqlite",
    "SqliteDBPath": "tshock.sqlite",
    "MySqlHost": "localhost:3306",
    "MySqlDbName": "",
    "MySqlUsername": "",
    "MySqlPassword": "",
    "UseSqlLogs": false,
    "RevertToTextLogsOnSqlFailures": 10,
    "RestApiEnabled": false,
    "RestApiPort": 7878,
    "LogRest": false,
    "EnableTokenEndpointAuthentication": false,
    "RESTMaximumRequestsPerInterval": 5,
    "RESTRequestBucketDecreaseIntervalMinutes": 1,
    "ApplicationRestTokens": {}
  }
```

## 更新日志

```
暂无
```

## FeedBack

- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
