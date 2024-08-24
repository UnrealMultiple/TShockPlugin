<div align = "center">
  
[![TShockPlugin](https://socialify.git.ci/Controllerdestiny/TShockPlugin/image?description=1&descriptionEditable=A%20TShock%20Chinese%20Plugin%20Collection%20Repository&forks=1&issues=1&language=1&logo=https%3A%2F%2Fgithub.com%2FControllerdestiny%2FTShockPlugin%2Fblob%2Fmaster%2Ficon.png%3Fraw%3Dtrue&name=1&pattern=Circuit%20Board&pulls=1&stargazers=1&theme=Auto)](https://github.com/Controllerdestiny/TShockPlugin)  
[![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/Controllerdestiny/TShockPlugin/.github%2Fworkflows%2Fplugins_publish.yml)](https://github.com/Controllerdestiny/TShockPlugin/actions)
[![GitHub contributors](https://img.shields.io/github/contributors/Controllerdestiny/TShockPlugin?style=flat)](https://github.com/Controllerdestiny/TShockPlugin/graphs/contributors)
[![NET6](https://img.shields.io/badge/Core-%20.NET_6-blue)](https://dotnet.microsoft.com/zh-cn/)
[![QQ](https://img.shields.io/badge/QQ-EB1923?logo=tencent-qq&logoColor=white)](https://qm.qq.com/cgi-bin/qm/qr?k=54tOesIU5g13yVBNFIuMBQ6AzjgE6f0m&jump_from=webapi&authKey=6jzafzJEqQGzq7b2mAHBw+Ws5uOdl83iIu7CvFmrfm/Xxbo2kNHKSNXJvDGYxhSW)
[![TShock](https://img.shields.io/badge/TShock5.2.0-2B579A.svg?&logo=TShock&logoColor=white)](https://github.com/Pryaxis/TShock)

[简体中文](README.md) | **&gt; English &lt;**

</div>

## Preface
- This is a repository dedicated to collecting and integrating `TShock` plugins.
- Some of the plugins in the library are collected from the internet and decompiled.
- Due to the special nature of the project, it may cause infringement. If there is any infringement, please contact us to resolve it.
- We will continue to collect high-quality `TShock` plugins, update them in a timely manner, and keep up with the latest version of `TShock`.
- If you wish to join us, follow the `Developer Notes` and submit a `Pull Request` to this repository.


## User Notes

- Note that some plugins may require preconditions, please check the list below to install the preconditions.
- Each plugin has a usage note, click on the hyperlink in the list below to view the specific instructions.
- It is said that people who like to star repositories, their plugins are not easy to raise errors.

## Download

- Chinese mirror: [Plugins.zip](https://github.moeyy.xyz/https://github.com/Controllerdestiny/TShockPlugin/releases/download/V1.0.0.0/Plugins.zip)
- This repository: [Plugins.zip](https://github.com/Controllerdestiny/TShockPlugin/releases/tag/V1.0.0.0)

## How to Install TShock
- If you are a Windows user, you can choose to install it using the following command (run this command in pwsh):
```powershell
curl -o install.ps1 https://github.moeyy.xyz/https://raw.githubusercontent.com/UnrealMultiple/TShockPlugin/master/InstallTShock.ps1
powershell -ExecutionPolicy ByPass -File ./install.ps1 -verb runas
```

## Developer Notes

> Coding Standards

- Do not use Chinese variable names.
- Do not use dangerous features.
- Avoid using multithreading where possible.
- Do not leave backdoors in plugins.
- Please include a README.md file with each plugin project.

## Feedback

> Any feedback, suggestions, or improvements on this code library will be considered as public contributions and may be included in this code library unless otherwise explicitly stated.

- If there is a bug, please provide the relevant system and TShock version bug reproduction process in a standardized `issue`.

### Collected Plugins

> Click on the blue link to view the detailed description of the plugin

<Details>
<Summary>Plugin List</Summary>

| Plugin Name                                                      |                                 Plugin Description                                 |                                                                   Precondition                                                                   |
|------------------------------------------------------------------|:----------------------------------------------------------------------------------:|:------------------------------------------------------------------------------------------------------------------------------------------------:|
| [ChattyBridge](ChattyBridge/README.md)                           |                            Used for cross-server chat.                             |                                                                        No                                                                        |
| [EconomicsAPI](EconomicsAPI/README.md)                           |                            Economic plugin prerequisite                            |                                                                        No                                                                        |
| [Economics.RPG](Economics.RPG/README.md)                         |                                        RPG                                         |                                                      [EconomicsAPI](EconomicsAPI/README.md)                                                      |
| [Economics.WeaponPlus](Economics.WeaponPlus/README.md)           |                                  Enhance weapons                                   |                                                      [EconomicsAPI](EconomicsAPI/README.md)                                                      |
| [Economics.Deal](Economics.RPG/README.md)                        |                                   Trading plugin                                   |                                                      [EconomicsAPI](EconomicsAPI/README.md)                                                      |
| [Economics.Shop](Economics.Shop/README.md)                       |                                    Store plugin                                    | [EconomicsAPI](EconomicsAPI/README.md)<br>[Economics.RPG](https://github.com/Controllerdestiny/TShockPlugin/blob/master/Economics.RPG/README.md) |
| [Economics.Skill](Economics.Skill/README.md)                     |                                    Skill plugin                                    | [EconomicsAPI](EconomicsAPI/README.md)<br>[Economics.RPG](https://github.com/Controllerdestiny/TShockPlugin/blob/master/Economics.RPG/README.md) |
| [Economics.Regain](Economics.Regain/README.md)                   |                                   Item recycling                                   |                                                      [EconomicsAPI](EconomicsAPI/README.md)                                                      |
| [Economics.Projectile](Economics.Projectile/README.md)           |                                 Custom projectile                                  |                                [EconomicsAPI](EconomicsAPI/README.md)<br>[Economics.RPG](Economics.RPG/README.md)                                |
| [Economics.NPC](Economics.NPC/README.md)                         |                                Custom monster loot                                 |                                                      [EconomicsAPI](EconomicsAPI/README.md)                                                      |
| [Economics.Task](Economics.Task/README.md)                       |                                    Task plugin                                     | [EconomicsAPI](EconomicsAPI/README.md)<br>[Economics.RPG](https://github.com/Controllerdestiny/TShockPlugin/blob/master/Economics.RPG/README.md) |
| [CreateSpawn](CreateSpawn/README.md)                             |                          Spawn point building generation                           |                                                                        No                                                                        |
| [AutoBroadcast](AutoBroadcast/README.md)                         |                                Automatic broadcast                                 |                                                                        No                                                                        |
| [AutoTeam](AutoTeam/README.md)                                   |                                      AutoTeam                                      |                                                                        No                                                                        |
| [BridgeBuilder](BridgeBuilder/README.md)                         |                                Quick bridge laying                                 |                                                                        No                                                                        |
| [OnlineGiftPackage](OnlineGiftPackage/README.md)                 |                                  Online gift pack                                  |                                                                        No                                                                        |
| [LifemaxExtra](LifemaxExtra/README.md)                           |                         Eat more Life Fruits/Life Crystal                          |                                                                        No                                                                        |
| [DisableMonsLoot](DisableMonsLoot/README.md)                     |                           Prohibit monster drop rewards                            |                                                                        No                                                                        |
| [PermaBuff](PermaBuff/README.md)                                 |                                   Permanent Buff                                   |                                                                        No                                                                        |
| [ShortCommand](ShortCommand/README.md)                           |                                   Short Command                                    |                                                                        No                                                                        |
| [ProgressBag](ProgressBag/README.md)                             |                                 Progress gift pack                                 |                                                                        No                                                                        |
| [CriticalHit](CriticalHit/README.md)                             |                                     Hit prompt                                     |                                                                        No                                                                        |
| [Back](Back/README.md)                                           |                            Return to the point of death                            |                                                                        No                                                                        |
| [BanNpc](BanNpc/README.md)                                       |                             Prevent monster generation                             |                                                                        No                                                                        |
| [MapTeleport](MapTp/README.md)                                   |                         Double-click the map to teleport.                          |                                                                        No                                                                        |
| [RandReSpawn](RandRespawn/README.md)                             |                                 Random spawn point                                 |                                                                        No                                                                        |
| [CGive](CGive/README.md)                                         |                                  Offline commands                                  |                                                                        No                                                                        |
| [RainbowChat](RainbowChat/README.md)                             |                                 Random chat color                                  |                                                                        No                                                                        |
| [NormalDropsBags](NormalDropsBags/README.md)                     |                      Drop Treasure Bags at normal difficulty.                      |                                                                        No                                                                        |
| [DisableSurfaceProjectiles](DisableSurfaceProjectiles/README.md) |                            Prohibit surface projectiles                            |                                                                        No                                                                        |
| [RecipesBrowser](RecipesBrowser/README.md)                       |                                   Crafting Table                                   |                                                                        No                                                                        |
| [DisableGodMod](DisableGodMod/README.md)                         |                        Prevent player from being invincible                        |                                                                        No                                                                        |
| [TownNPCHomes](TownNPCHomes/README.md)                           |                                   NPC quick home                                   |                                                                        No                                                                        |
| [RegionView](RegionView/README.md)                               |                              Display area boundaries                               |                                                                        No                                                                        |
| [Noagent](Noagent/README.md)                                     |                          Prohibit proxy IP from entering                           |                                                                        No                                                                        |
| [SwitchCommands](SwitchCommands/README.md)                       |                             Execute commands in region                             |                                                                        No                                                                        |
| [GolfRewards](GolfRewards/README.md)                             |                                    Golf Rewards                                    |                                                                        No                                                                        |
| [DataSync](DataSync/README.md)                                   |                              Progress synchronization                              |                                                                        No                                                                        |
| [ProgressRestrict](ProgressRestrict/README.md)                   |                              Super progress detection                              |                                                          [DataSync](DataSync/README.md)                                                          |
| [PacketsStop](PacketsStop/README.md)                             |                                Packet interception                                 |                                                                        No                                                                        |
| [DeathDrop](DeathDrop/README.md)                                 |                      Random and custom loot for monster death                      |                                                                        No                                                                        |
| [DTEntryBlock](DTEntryBlock/README.md)                           |                       Prevent entry into dungeons or temples                       |                                                                        No                                                                        |
| [PerPlayerLoot](PerPlayerLoot/README.md)                         |                           Separate chest for player loot                           |                                                                        No                                                                        |
| [PvPer](PvPer/README.md)                                         |                                    Duel system                                     |                                                                        No                                                                        |
| [DumpTerrariaID](DumpTerrariaID/README.md)                       |                                 Query Terraria ID                                  |                                                                        No                                                                        |
| [DamageStatistic](DamageStatistic/README.md)                     |                                 Damage statistics                                  |                                                                        No                                                                        |
| [AdditionalPylons](AdditionalPylons/README.md)                   |                             Place more crystal towers                              |                                                                        No                                                                        |
| [History](History/README.md)                                     |                                History grid record                                 |                                                                        No                                                                        |
| [Invincibility](Invincibility/README.md)                         |                             Time-limited invincibility                             |                                                                        No                                                                        |
| [Ezperm](Ezperm/README.md)                                       |                              Batch change permissions                              |                                                                        No                                                                        |
| [AutoClear](Autoclear/README.md)                                 |                           Intelligent automatic cleaning                           |                                                                        No                                                                        |
| [EssentialsPlus](EssentialsPlus/README.md)                       |                              More management commands                              |                                                                        No                                                                        |
| [ShowArmors](ShowArmors/README.md)                               |                               Display equipment bar                                |                                                                        No                                                                        |
| [VeinMiner](VeinMiner/README.md)                                 |                                    Chain mining                                    |                                                                        No                                                                        |
| [PersonalPermission](PersonalPermission/README.md)               |                      Set permissions individually for players                      |                                                                        No                                                                        |
| [ItemPreserver](ItemPreserver/README.md)                         |                           Specified items do not consume                           |                                                                        No                                                                        |
| [SimultaneousUseFix](SimultaneousUseFix/README.md)               |         Solve problems like stuck double hammer and star spin machine gun          |                                      [Chireiden.TShock.Omni](https://github.com/sgkoishi/yaaiomni/releases)                                      |
| [Challenger](Challenger/README.md)                               |                                  Challenger mode                                   |                                                                        No                                                                        |
| [MiniGamesAPI](MiniGamesAPI/README.md)                           |                              Bean paste mini game API                              |                                                                        No                                                                        |
| [BuildMaster](BuildMaster/README.md)                             |                       Red Bean Mini Game·Master Builder Mode                       |                                                      [MiniGamesAPI](MiniGamesAPI/README.md)                                                      |
| [journeyUnlock](journeyUnlock/README.md)                         |                                Unlock Journey Items                                |                                                                        No                                                                        |
| [ListPlugins](ListPlugins/README.md)                             |                               List Installed Plugins                               |                                                                        No                                                                        |
| [BagPing](BagPing/README.md)                                     |                              Mark Treasure Bag on Map                              |                                                                        No                                                                        |
| [ServerTools](ServerTools/README.md)                             |                              Server Management Tools                               |                                                                        No                                                                        |
| [Platform](Platform/README.md)                                   |                              Determine Player Device                               |                                                                        No                                                                        |
| [CaiLib](CaiLib/README.md)                                       |                               Cai’s Preload Library                                |                                                                        No                                                                        |
| [GenerateMap](GenerateMap/README.md)                             |                                 Generate Map Image                                 |                                                            [CaiLib](CaiLib/README.md)                                                            |
| [RestInventory](RestInventory/README.md)                         |                       Provide REST Query Backpack Interface                        |                                                                        No                                                                        |
| [WikiLangPackLoader](WikiLangPackLoader/README.md)               |                     Load Chinese Wiki Language Pack for Server                     |                                                                        No                                                                        |
| [HelpPlus](HelpPlus/README.md)                                   |                            Fix and Enhance Help Command                            |                                                                        No                                                                        |
| [CaiBot](CaiBot/README.md)                                       |                             CaiBot(QQ) Adapter Plugin                              |                                                              Built-in Precondition                                                               |
| [HouseRegion](HouseRegion/README.md)                             |                                Land Claiming Plugin                                |                                                                        No                                                                        |
| [SignInSign](SignInSign/README.md)                               |                               Signboard Login Plugin                               |                                                                        No                                                                        |
| [WeaponPlusCostCoin](WeaponPlusCostCoin/README.md)               |                          Weapon Enhancement Coin Version                           |                                                                        No                                                                        |
| [Respawn](Respawn/README.md)                                     |                            Respawn at the Deadth Place                             |                                                                        No                                                                        |
| [EndureBoost](EndureBoost/README.md)                             |                  Long Duration Buff After Certain Amount of Items                  |                                                                        No                                                                        |
| [AnnouncementBoxPlus](AnnouncementBoxPlus/README.md)             |                        Enhance Broadcast Box Functionality                         |                                                                        No                                                                        |
| [ConsoleSql](ConsoleSql/README.md)                               |                 Allow You to Execute SQL Statements in the Console                 |                                                                        No                                                                        |
| [ProgressControl](ProgressControls/README.md)                    |                         Planbook (Automate Server Control)                         |                                                                        No                                                                        |
| [RealTime](RealTime/README.md)                                   |                       Synchronize Server Time with Real Time                       |                                                                        No                                                                        |
| [GoodNight](GoodNight/README.md)                                 |                                       Curfew                                       |                                                                        No                                                                        |
| [Musicplayer](musicplayer/README.md)                             |                                Simple Music Player                                 |                                                                        No                                                                        |
| [TimerKeeper](TimerKeeper/README.md)                             |                                  Save Timer State                                  |                                                                        No                                                                        |
| [Chameleon](Chameleon/README.md)                                 |                          Login Before Entering the Server                          |                                                                        No                                                                        |
| [AutoPluginManager](AutoPluginManager/README.md)                 |                         One-Click Automatic Plugin Update                          |                                                                        No                                                                        |
| [SpclPerm](SpclPerm/README.md)                                   |                              Server Owner Privileges                               |                                                                        No                                                                        |
| [MonsterRegen](MonsterRegen/README.md)                           |                           Monster Progress Regeneration                            |                                                                        No                                                                        |
| [HardPlayerDrop](HardPlayerDrop/README.md)                       |                         Hardcore Death Drops Life Crystals                         |                                                                        No                                                                        |
| [ReFishTask](ReFishTask/README.md)                               |                       Automatically Refresh Fisherman Tasks                        |                                                                        No                                                                        |
| [Sandstorm](Sandstorm/README.md)                                 |                                  Toggle Sandstorm                                  |                                                                        No                                                                        |
| [RandomBroadcast](RandomBroadcast/README.md)                     |                                  Random Broadcast                                  |                                                                        No                                                                        |
| [BedSet](BedSet/README.md)                                       |                            Set and Record Respawn Point                            |                                                                        No                                                                        |
| [ConvertWorld](ConvertWorld/README.md)                           |                       Defeat Monsters to Convert World Items                       |                                                                        No                                                                        |
| [AutoStoreItems](AutoStoreItems/README.md)                       |                                   Auto Save Item                                   |                                                                        No                                                                        |
| [ZHIPlayerManager](ZHIPlayerManager/README.md)                   |                          zZhi's Player Management Plugin                           |                                                                        No                                                                        |
| [SpawnInfra](SpawnInfra/README.md)                               |                           Generate Basic Infrastructure                            |                                                                        No                                                                        |
| [CNPCShop](CNPCShop/README.md)                                   |                                  Custom NPC Shop                                   |                                                                        No                                                                        |
| [SessionSentinel](SessionSentinel/README.md)                     |              Handle Players Not Sending Data Packets for a Long Time               |                                                                        No                                                                        |
| [TeleportRequest](TeleportRequest/README.md)                     |                                  Teleport Request                                  |                                                                        No                                                                        |
| [CaiRewardChest](CaiRewardChest/README.md)                       | Convert Naturally Generated Chests into Reward Chests that Everyone Can Claim Once |                                                                        No                                                                        |
| [ProxyProtocolSocket](ProxyProtocolSocket/README.md)             |                         Accept proxy protocol connections                          |                                                                        No                                                                        |
| [UnseenInventory](UnseenInventory/README.md)                     |       Allows the server to generate items that are normally 'unobtainable'.        |      
       No                                                                        |
| [ChestRestore](ChestRestore/README.md)                           |                              Infinite chest items                                  |       
        No                                                                       |

</Details>

## Contributors

[![Contributors](https://stats.deeptrain.net/contributor/Controllerdestiny/TShockPlugin)](https://github.com/Controllerdestiny/TShockPlugin/graphs/contributors)

## Friendly Links

- [TShock Plugin Development Documentation](https://github.com/ACaiCat/TShockPluginDocument)
- [Tshock Comprehensive Navigation](https://github.com/UnrealMultiple/Tshock-nav)
