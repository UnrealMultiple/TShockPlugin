# PacketsStop

- Author: 羽学 少司命
- Source: [PacketsStop](https://github.com/1242509682/PacketsStop/)
- This is a Tshock server plugin mainly used for intercepting specified player's data packets through commands.
- Use `/pksp add name` to add a specified player to the interception list and then use `/pksp on` to activate interception.
- The source code of this plugin was adapted from ShaoSiMing's visitor restriction plugin, transforming its method of processing data packets into an independent function plugin.

## Commands

| Syntax                             | Alias  |       Permission       |                   Description                   |
| ---------------------------------- | :----: | :--------------------: | :---------------------------------------------: |
| /pksp or packetstop                | None   | packetstop.use         | Displays command menu                           |
| /pksp on                           | None   | packetstop.use         | Enables data packet interception                |
| /pksp off                          | None   | packetstop.use         | Disables data packet interception               |
| /pksp list                         | None   | packetstop.use         | Lists players in the interception list          |
| /pksp add playername               | None   | packetstop.use         | Adds specified player to interception list      |
| /pksp del playername               | None   | packetstop.use         | Removes specified player from interception list |
| /pksp reset                        | None   | packetstop.use         | Clears the interception list                    |
| /reload                            | None   | tshock.cfg.reload      | Reloads configuration file                      |

## Update Log
```
1.0.6
Refactored and fixed codes, no longer adding intercepted targets to a new group but specifying through the list in the configuration file.
Improved some commands for easier addition, deletion, modification, and query.
Added all packet names in preset configurations for easy filtering.
1.0.5
Added English commands, modified permissions.
1.0.1
Enhanced uninstallation function.
2.0
Fixed GetPacket logic in the data packet interception plugin: Resolved issue where everything outside of the packet names listed in the configuration file was being intercepted.
1.0
Adapted the method of processing data packets from ShaoSiMing's visitor restriction plugin into an independent function plugin.
```

## Configuration
> Configuration file position: tshock/数据包拦截.json
```json5
{
  "DataPacketNamesViewableAt": "https://github.com/Pryaxis/TSAPI/blob/general-devel/TerrariaServerAPI/TerrariaApi.Server/PacketTypes.cs",
  "CommandAndPermissionName": "Command:pksp Permission:packetstop.use",
  "FeatureSwitch": false,
  "PlayerInterceptionList": [
    "羽学"
  ],
  "PacketNames": [
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

## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
