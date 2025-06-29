# PacketsStop 数据包拦截

- 作者: 羽学 少司命
- 出处: [PacketsStop](https://github.com/1242509682/PacketsStop/)
- 这是一个TShock服务器插件主要用于： 
- 使用指令开启拦截指定玩家的数据包  
- 输入【/pksp add 名字】将指定玩家添加到拦截名单后输入【/pksp on】即可开启拦截 
- 插件源码来于少司命的游客限制插件，将其处理数据包方法做成了一个独立的功能插件

## 指令

| 语法                             | 别名  |       权限       |                   说明                   |
| -------------------------------- | :---: | :--------------: | :--------------------------------------: |
| /pksp 或 packetstop | 无   | packetstop.use  |          指令菜单          |
| /pksp on | 无   | packetstop.use  |          开启数据包拦截          |
| /pksp off | 无  | packetstop.use  |          关闭数据包拦截          |
| /pksp list | 无    | packetstop.use  |          列出拦截名单          |
| /pksp add 玩家名 | 无 | packetstop.use  | 将指定玩家添加到拦截名单 |
| /pksp del 玩家名 | 无 | packetstop.use  |  将指定玩家从拦截名单移除  |
| /pksp reset | 无 | packetstop.use  |  清空拦截名单  |
| /reload | 无 |   tshock.cfg.reload    |    重载配置文件    |


## 配置
> 配置文件位置：tshock/数据包拦截.json
```json5
{
  "数据包名可查看": "https://github.com/Pryaxis/TSAPI/blob/general-devel/TerrariaServerAPI/TerrariaApi.Server/PacketTypes.cs",
  "插件指令与权限名": "指令：pksp 权限：packetstop.use",
  "功能开关": false,
  "拦截玩家名单": [
    "羽学"
  ],
  "拦截数据包名": [
    "ConnectRequest",
    "Disconnect",
    "ContinueConnecting",
    "PlayerInfo",
    "PlayerSlot",
    "ContinueConnecting2",
    "WorldInfo",
    "TileGetSection",
    "Status",
    "TileSendSection",
    "TileFrameSection",
    "PlayerSpawn",
    "PlayerUpdate",
    "PlayerActive",
    "PlayerHp",
    "Tile",
    "TimeSet",
    "DoorUse",
    "TileSendSquare",
    "ItemDrop",
    "ItemOwner",
    "NpcUpdate",
    "NpcItemStrike",
    "ProjectileNew",
    "NpcStrike",
    "ProjectileDestroy",
    "TogglePvp",
    "ChestGetContents",
    "ChestItem",
    "ChestOpen",
    "PlaceChest",
    "EffectHeal",
    "Zones",
    "PasswordRequired",
    "PasswordSend",
    "RemoveItemOwner",
    "NpcTalk",
    "PlayerAnimation",
    "PlayerMana",
    "EffectMana",
    "PlayerTeam",
    "SignRead",
    "SignNew",
    "LiquidSet",
    "PlayerSpawnSelf",
    "PlayerBuff",
    "NpcSpecial",
    "ChestUnlock",
    "NpcAddBuff",
    "NpcUpdateBuff",
    "PlayerAddBuff",
    "UpdateNPCName",
    "UpdateGoodEvil",
    "PlayHarp",
    "HitSwitch",
    "UpdateNPCHome",
    "SpawnBossorInvasion",
    "PlayerDodge",
    "PaintTile",
    "PaintWall",
    "Teleport",
    "PlayerHealOther",
    "Placeholder",
    "ClientUUID",
    "ChestName",
    "CatchNPC",
    "ReleaseNPC",
    "TravellingMerchantInventory",
    "TeleportationPotion",
    "AnglerQuest",
    "CompleteAnglerQuest",
    "NumberOfAnglerQuestsCompleted",
    "CreateTemporaryAnimation",
    "ReportInvasionProgress",
    "PlaceObject",
    "SyncPlayerChestIndex",
    "CreateCombatText",
    "LoadNetModule",
    "NpcKillCount",
    "PlayerStealth",
    "ForceItemIntoNearestChest",
    "UpdateTileEntity",
    "PlaceTileEntity",
    "TweakItem",
    "PlaceItemFrame",
    "UpdateItemDrop",
    "EmoteBubble",
    "SyncExtraValue",
    "SocialHandshake",
    "KillPortal",
    "PlayerTeleportPortal",
    "NotifyPlayerNpcKilled",
    "NotifyPlayerOfEvent",
    "UpdateMinionTarget",
    "NpcTeleportPortal",
    "UpdateShieldStrengths",
    "NebulaLevelUp",
    "MoonLordCountdown",
    "NpcShopItem",
    "GemLockToggle",
    "PoofOfSmoke",
    "SmartTextMessage",
    "WiredCannonShot",
    "MassWireOperation",
    "MassWireOperationPay",
    "ToggleParty",
    "TreeGrowFX",
    "CrystalInvasionStart",
    "CrystalInvasionWipeAll",
    "MinionAttackTargetUpdate",
    "CrystalInvasionSendWaitTime",
    "PlayerHurtV2",
    "PlayerDeathV2",
    "CreateCombatTextExtended",
    "Emoji",
    "TileEntityDisplayDollItemSync",
    "RequestTileEntityInteraction",
    "WeaponsRackTryPlacing",
    "TileEntityHatRackItemSync",
    "SyncTilePicking",
    "SyncRevengeMarker",
    "RemoveRevengeMarker",
    "LandGolfBallInCup",
    "FinishedConnectingToServer",
    "FishOutNPC",
    "TamperWithNPC",
    "PlayLegacySound",
    "FoodPlatterTryPlacing",
    "UpdatePlayerLuckFactors",
    "DeadPlayer",
    "SyncCavernMonsterType",
    "RequestNPCBuffRemoval",
    "ClientSyncedInventory",
    "SetCountsAsHostForGameplay",
    "SetMiscEventValues",
    "RequestLucyPopup",
    "SyncProjectileTrackers",
    "CrystalInvasionRequestedToSkipWaitTime",
    "RequestQuestEffect",
    "SyncItemsWithShimmer",
    "ShimmerActions",
    "SyncLoadout",
    "SyncItemCannotBeTakenByEnemies"
  ]
}
```


## 更新日志

### v1.0.6
- 重构并修复代码，不再将拦截对象加入到一个新的组
- 而是通过配置文件中的名单进行指定
- 完善了部分指令用于更方便的增删改查
- 预设配置中加入了全部的数据包名，方便筛选
### v1.0.5
- 添加英文命令，修改权限
### v1.0.1
- 完善卸载函数
### v2.0
- 修复数据包拦截插件的GetPacket逻辑：原对配置文件内的数据包名以外的全部拦截问题已修复
### v1.0
- 将少司命的游客限制插件处理数据包方法，做成了一个独立的功能插件


## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
