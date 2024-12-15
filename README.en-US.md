<div align="center">
  
[![TShockPlugin](https://socialify.git.ci/UnrealMultiple/TShockPlugin/image?description=1&descriptionEditable=A%20TShock%20Chinese%20Plugin%20Collection%20Repository&forks=1&issues=1&language=1&logo=https%3A%2F%2Fgithub.com%2FUnrealMultiple%2FTShockPlugin%2Fblob%2Fmaster%2Ficon.png%3Fraw%3Dtrue&name=1&pattern=Circuit%20Board&pulls=1&stargazers=1&theme=Auto)](https://github.com/UnrealMultiple/TShockPlugin)  
[![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/UnrealMultiple/TShockPlugin/.github%2Fworkflows%2Fbuild.yml)](https://github.com/UnrealMultiple/TShockPlugin/actions)
[![GitHub contributors](https://img.shields.io/github/contributors/UnrealMultiple/TShockPlugin?style=flat)](https://github.com/UnrealMultiple/TShockPlugin/graphs/contributors)
[![NET6](https://img.shields.io/badge/Core-%20.NET_6-blue)](https://dotnet.microsoft.com/zh-cn/)
[![QQ](https://img.shields.io/badge/QQ-EB1923?logo=tencent-qq&logoColor=white)](https://qm.qq.com/cgi-bin/qm/qr?k=54tOesIU5g13yVBNFIuMBQ6AzjgE6f0m&jump_from=webapi&authKey=6jzafzJEqQGzq7b2mAHBw+Ws5uOdl83iIu7CvFmrfm/Xxbo2kNHKSNXJvDGYxhSW)
[![TShock](https://img.shields.io/badge/TShock5.2.0-2B579A.svg?&logo=TShock&logoColor=white)](https://github.com/Pryaxis/TShock)

[简体中文](README.md) | **&gt; English &lt;** | [Spanish/Español](README.es-ES.md)

</div>

## Preface
- This is a repository dedicated to collecting and integrating `TShock` plugins.
- Some of the plugins in the library are collected from the internet and decompiled.
- Due to the special nature of the project, it may cause infringement. If there is any infringement, please contact us to resolve it.
- We will continue to collect high-quality `TShock` plugins, update them in a timely manner, and keep up with the latest version of `TShock`.
- If you wish to join us, follow the `Developer Notes` and submit a `Pull Request` to this repository.


## User Notes

- Note that some plugins may require dependencies, please check the list below to install the dependencies.
- Each plugin has a usage note, click on the hyperlink in the list below to view the specific instructions.
- It is said that people who like to star repositories, their plugins are not easy to raise errors.

## Download

- Github Release: [Plugins.zip](https://github.com/UnrealMultiple/TShockPlugin/releases/download/V1.0.0.0/Plugins.zip)
- Gitee Release: [Plugins.zip](https://gitee.com/kksjsj/TShockPlugin/releases/download/V1.0.0.0/Plugins.zip)

#### AutoPluginManager
    /apm l List all plugins
    /apm i <plugin name> One-click install plugin
    /apm u [plugin name] Check and update plugin

## Developer Notes

> Coding Standards

- Do not use Chinese variable names.
- Do not use dangerous features.
- Avoid using multithreading where possible.
- Do not leave backdoors in plugins.
- Please include a README.md file with each plugin project.

## Feedback

> Any feedback, suggestions, or improvements on this code library will be considered as public contributions and may be included in this code library unless otherwise explicitly stated.

- If there is a bug, please provide the relevant system information, TShock version and bug reproduction process in the `issue` page of GitHub.

### Collected Plugins

> Click on the hyperlinks to view the detailed description of the plugin

> [!NOTE]
> The English plugin documentation might not be updated as promptly as the Chinese plugin documentation.
> Please try to refer to the Chinese documentation whenever possible.

<Details>
<Summary>Plugin List</Summary>

| Plugin Name | English Available | Plugin Description | Dependencies |
| :-: | :-: | :-: | :-: |
| [AdditionalPylons](./src/AdditionalPylons/README.en-US.md) | Yes | Place more Pylons | [LazyAPI](./src/LazyAPI/README.md) |
| [AnnouncementBoxPlus](./src/AnnouncementBoxPlus/README.md) | No | Enhance Broadcast Box Functionality | [LazyAPI](./src/LazyAPI/README.md) |
| [AutoAirItem](./src/AutoAirItem/README.en-US.md) | Yes | Automatic trash cans | [LazyAPI](./src/LazyAPI/README.md) |
| [AutoBroadcast](./src/AutoBroadcast/README.en-US.md) | Yes | Automatic broadcast | [LazyAPI](./src/LazyAPI/README.md) |
| [AutoClear](./src/AutoClears/README.en-US.md) | Yes | Intelligent automatic cleaning | [LazyAPI](./src/LazyAPI/README.md) |
| [AutoFish](./src/AutoFish/README.en-US.md) | Yes | Automatic fishing | [LazyAPI](./src/LazyAPI/README.md) |
| [AutoPluginManager](./src/AutoPluginManager/README.en-US.md) | Yes | Update plugins automatically in one key |  |
| [AutoReset](./src/AutoReset/README.en-US.md) | Yes | Fully automatic reset | [LazyAPI](./src/LazyAPI/README.md) |
| [AutoStoreItems](./src/AutoStoreItems/README.en-US.md) | Yes | Automatic storage | [LazyAPI](./src/LazyAPI/README.md) |
| [AutoTeam](./src/AutoTeam/README.en-US.md) | Yes | Automatic team formation | [LazyAPI](./src/LazyAPI/README.md) |
| [Back](./src/Back/README.en-US.md) | Yes | Return to the point of death | [LazyAPI](./src/LazyAPI/README.md) |
| [BagPing](./src/BagPing/README.en-US.md) | Yes | Mark treasure bags on the map |  |
| [BanNpc](./src/BanNpc/README.en-US.md) | Yes | Prevent monster generation | [LazyAPI](./src/LazyAPI/README.md) |
| [BedSet](./src/BedSet/README.en-US.md) | Yes | Set and record respawn points | [LazyAPI](./src/LazyAPI/README.md) |
| [BetterWhitelist](./src/BetterWhitelist/README.en-US.md) | Yes | Whitelist plugin | [LazyAPI](./src/LazyAPI/README.md) |
| [BridgeBuilder](./src/BridgeBuilder/README.en-US.md) | Yes | Quick bridge building | [LazyAPI](./src/LazyAPI/README.md) |
| [BuildMaster](./src/BuildMaster/README.md) | No | Red Bean Mini Game·Master Builder Mode | [MiniGamesAPI](./src/MiniGamesAPI/README.md) |
| [CaiBot](./src/CaiBot/README.md) | No | CaiBot adapter plugin |  |
| [CaiCustomEmojiCommand](./src/CaiCustomEmojiCommand/README.en-US.md) | Yes | Custom emoji command | [LazyAPI](./src/LazyAPI/README.md) |
| [CaiLib](./src/CaiLib/README.md) | No | Cai's preload library | [SixLabors.ImageSharp]() |
| [CaiPacketDebug](./src/CaiPacketDebug/README.md) | No | Cai Packet Debug Tool | [LazyAPI](./src/LazyAPI/README.md) [TrProtocol]() |
| [CaiRewardChest](./src/CaiRewardChest/README.en-US.md) | Yes | Convert naturally generated chests into reward chests that everyone can claim once | [linq2db]() [LazyAPI](./src/LazyAPI/README.md) |
| [CGive](./src/CGive/README.en-US.md) | Yes | Offline commands |  |
| [Challenger](./src/Challenger/README.md) | Yes | Challenger mode |  |
| [Chameleon](./src/Chameleon/README.en-US.md) | Yes | Login before entering the server | [LazyAPI](./src/LazyAPI/README.md) |
| [ChattyBridge](./src/ChattyBridge/README.md) | No | Used for cross-server chat | [LazyAPI](./src/LazyAPI/README.md) |
| [ChestRestore](./src/ChestRestore/README.en-US.md) | Yes | Infinite items in resource servers |  |
| [Chireiden.TShock.Omni](https://github.com/sgkoishi/yaaiomni/blob/master/README.md) | No | Yet another misc plugin for TShock - the core part |  |
| [Chireiden.TShock.Omni.Misc](https://github.com/sgkoishi/yaaiomni/blob/master/README.md) | No | Yet another misc plugin for TShock - the miscellaneous part | [Chireiden.TShock.Omni](https://github.com/sgkoishi/yaaiomni/blob/master/README.md) |
| [CNPCShop](./src/CNPCShop/README.md) | No | Custom NPC shop |  |
| [ConsoleSql](./src/ConsoleSql/README.md) | No | Execute SQL statements in the console |  |
| [ConvertWorld](./src/ConvertWorld/README.en-US.md) | Yes | Convert world items by defeating monsters |  |
| [CreateSpawn](./src/CreateSpawn/README.md) | No | Spawn point building generation | [LazyAPI](./src/LazyAPI/README.md) |
| [CriticalHit](./src/CriticalHit/README.en-US.md) | No | Critical hit prompt |  |
| [Crossplay](https://github.com/UnrealMultiple/Crossplay/blob/main/README.md) | No | Allows for cross-platform play |  |
| [DamageRuleLoot](./src/DamageRuleLoot/README.en-US.md) | Yes | Determine the drop treasure bag based on the ratio of damage and transfer damage calculation |  |
| [DamageStatistic](./src/DamageStatistic/README.md) | No | Display damage caused by each player after each boss fight |  |
| [DataSync](./src/DataSync/README.md) | No | Progress synchronization |  |
| [DeathDrop](./src/DeathDrop/README.md) | No | Random and custom loot upon monster death |  |
| [DisableMonsLoot](./src/DisableMonsLoot/README.md) | No | Prohibit monster loot |  |
| [DisableSurfaceProjectiles](./src/DisableSurfaceProjectiles/README.md) | No | Prohibit surface projectiles |  |
| [DonotFuck](./src/DonotFuck/README.en-US.md) | Yes | Prevent swearing | [LazyAPI](./src/LazyAPI/README.md) |
| [DTEntryBlock](./src/DTEntryBlock/README.md) | No | Prevent entry into dungeons or temples |  |
| [DumpTerrariaID](./src/DumpTerrariaID/README.md) | No | Dump Terraria IDs |  |
| [DwTP](./src/DwTP/README.en-US.md) | Yes | Positioning Teleport |  |
| [Economics.Deal](./src/Economics.Deal/README.en-US.md) | Yes | Trading plugin | [EconomicsAPI](./src/EconomicsAPI/README.en-US.md) |
| [Economics.NPC](./src/Economics.NPC/README.en-US.md) | No | Custom monster rewards | [EconomicsAPI](./src/EconomicsAPI/README.en-US.md) |
| [Economics.Projectile](./src/Economics.Projectile/README.en-US.md) | No | Custom projectiles | [EconomicsAPI](./src/EconomicsAPI/README.en-US.md) [Economics.RPG](./src/Economics.RPG/README.en-US.md) |
| [Economics.Regain](./src/Economics.Regain/README.en-US.md) | Yes | Item recycling | [EconomicsAPI](./src/EconomicsAPI/README.en-US.md) |
| [Economics.RPG](./src/Economics.RPG/README.en-US.md) | Yes | RPG plugin | [EconomicsAPI](./src/EconomicsAPI/README.en-US.md) |
| [Economics.Shop](./src/Economics.Shop/README.en-US.md) | Yes | Shop plugin | [EconomicsAPI](./src/EconomicsAPI/README.en-US.md) [Economics.RPG](./src/Economics.RPG/README.en-US.md) |
| [Economics.Skill](./src/Economics.Skill/README.md) | Yes | Skill plugin | [EconomicsAPI](./src/EconomicsAPI/README.en-US.md) [Jint]() [Economics.RPG](./src/Economics.RPG/README.en-US.md) |
| [Economics.Task](./src/Economics.Task/README.en-US.md) | Yes | Task plugin | [EconomicsAPI](./src/EconomicsAPI/README.en-US.md) [Economics.RPG](./src/Economics.RPG/README.en-US.md) |
| [Economics.WeaponPlus](./src/Economics.WeaponPlus/README.en-US.md) | Yes | Weapon enhancement | [EconomicsAPI](./src/EconomicsAPI/README.en-US.md) |
| [EconomicsAPI](./src/EconomicsAPI/README.en-US.md) | Yes | Economic plugin prerequisite |  |
| [EndureBoost](./src/EndureBoost/README.en-US.md) | Yes | Grant specified buff when the player has a certain number of items |  |
| [EssentialsPlus](./src/EssentialsPlus/README.en-US.md) | Yes | Additional management commands |  |
| [Ezperm](./src/Ezperm/README.md) | Yes | Batch change permissions |  |
| [FishShop](https://github.com/UnrealMultiple/TShockFishShop/blob/master/README.md) | No | Fish shop |  |
| [GenerateMap](./src/GenerateMap/README.md) | No | Generate map images | [CaiLib](./src/CaiLib/README.md) |
| [GolfRewards](./src/GolfRewards/README.md) | No | Golf rewards |  |
| [GoodNight](./src/GoodNight/README.md) | No | Curfew |  |
| [HardPlayerDrop](./src/HardPlayerDrop/README.md) | No | Hardcore death drops life crystals |  |
| [HelpPlus](./src/HelpPlus/README.en-US.md) | Yes | Fix and enhance the Help command |  |
| [History](./src/History/README.md) | No | History grid record |  |
| [HouseRegion](./src/HouseRegion/README.md) | No | Land claiming plugin |  |
| [Invincibility](./src/Invincibility/README.md) | No | Time-limited invincibility |  |
| [ItemBox](./src/ItemBox/README.md) | No | Off-line inventory |  |
| [ItemDecoration](./src/ItemDecoration/README.en-US.md) | No | Floating message display for held items | [LazyAPI](./src/LazyAPI/README.md) |
| [ItemPreserver](./src/ItemPreserver/README.md) | No | Preserve specified items from consumption |  |
| [JourneyUnlock](./src/JourneyUnlock/README.md) | No | Unlock journey items |  |
| [Lagrange.XocMat.Adapter](./src/Lagrange.XocMat.Adapter/README.md) | No | Lagrange.XocMat Bot Adapter Plugin | [SixLabors.ImageSharp]() |
| [LazyAPI](./src/LazyAPI/README.md) | No | Plugin base library | [linq2db]() |
| [LifemaxExtra](./src/LifemaxExtra/README.md) | No | Eat more life fruits/crystals |  |
| [ListPlugins](./src/ListPlugins/README.md) | No | List installed plugins |  |
| [MapTp](./src/MapTp/README.en-US.md) | Yes | Double-click on the map to teleport |  |
| [MiniGamesAPI](./src/MiniGamesAPI/README.md) | No | Bean paste mini-game API |  |
| [ModifyWeapons](./src/ModifyWeapons/README.en-US.md) | Yes | ModifyWeapons | [LazyAPI](./src/LazyAPI/README.md) |
| [MonsterRegen](./src/MonsterRegen/README.md) | No | Monster progress regeneration |  |
| [MusicPlayer](./src/MusicPlayer/README.md) | No | Simple music player |  |
| [Noagent](./src/Noagent/README.md) | No | Prohibit proxy IP from entering |  |
| [NormalDropsBags](./src/NormalDropsBags/README.md) | No | Drop treasure bags at normal difficulty |  |
| [OnlineGiftPackage](./src/OnlineGiftPackage/README.md) | No | Online gift package |  |
| [PacketsStop](./src/PacketsStop/README.md) | No | Packet interception |  |
| [PermaBuff](./src/PermaBuff/README.md) | No | Permanent buff |  |
| [PerPlayerLoot](./src/PerPlayerLoot/README.en-US.md) | Yes | Separate chest for player loot |  |
| [PersonalPermission](./src/PersonalPermission/README.md) | No | Set permissions individually for players |  |
| [Platform](./src/Platform/README.md) | No | Determine player device |  |
| [PlayerManager](https://github.com/UnrealMultiple/TShockPlayerManager/blob/master/README.md) | No | Hufang's player manager |  |
| [PlayerSpeed](./src/PlayerSpeed/README.en-US.md) | Yes | Enable players to achieve a two-stage sprint | [LazyAPI](./src/LazyAPI/README.md) |
| [ProgressBag](./src/ProgressBag/README.md) | No | Progress gift pack |  |
| [ProgressControls](./src/ProgressControls/README.md) | No | Planbook (Automate server control) |  |
| [ProgressRestrict](./src/ProgressRestrict/README.md) | No | Super progress detection | [DataSync](./src/DataSync/README.md) |
| [ProxyProtocolSocket](./src/ProxyProtocolSocket/README.md) | Yes | Accept proxy protocol connections |  |
| [PvPer](./src/PvPer/README.md) | No | Duel system |  |
| [RainbowChat](./src/RainbowChat/README.md) | Yes | Random chat color |  |
| [RandomBroadcast](./src/RandomBroadcast/README.md) | No | Random broadcast |  |
| [RandRespawn](./src/RandRespawn/README.en-US.md) | No | Random spawn point |  |
| [RealTime](./src/RealTime/README.md) | No | Synchronize server time with real time |  |
| [RebirthCoin](./src/RebirthCoin/README.en-US.md) | Yes | Consume designated items to revive player |  |
| [RecipesBrowser](./src/RecipesBrowser/README.md) | No | Crafting table |  |
| [ReFishTask](./src/ReFishTask/README.en-US.md) | Yes | Automatically refresh fisherman tasks |  |
| [RegionView](./src/RegionView/README.md) | No | Display area boundaries |  |
| [Respawn](./src/Respawn/README.md) | No | Respawn at the death place |  |
| [RestInventory](./src/RestInventory/README.md) | No | Provide REST query backpack interface |  |
| [RolesModifying](./src/RolesModifying/README.md) | No | Modify player backpack |  |
| [Sandstorm](./src/Sandstorm/README.md) | No | Toggle sandstorm |  |
| [ServerTools](./src/ServerTools/README.en-US.md) | No | Server management tools |  |
| [SessionSentinel](./src/SessionSentinel/README.md) | No | Handle players not sending data packets for a long time |  |
| [ShortCommand](./src/ShortCommand/README.md) | No | Short command |  |
| [ShowArmors](./src/ShowArmors/README.md) | No | Display equipment bar |  |
| [SignInSign](./src/SignInSign/README.md) | No | Signboard login plugin |  |
| [SimultaneousUseFix](./src/SimultaneousUseFix/README.md) | No | Solve problems like stuck double hammer and star spin machine gun | [Chireiden.TShock.Omni](https://github.com/sgkoishi/yaaiomni/blob/master/README.md) |
| [SmartRegions](./src/SmartRegions/README.md) | No | Smart regions |  |
| [SpawnInfra](./src/SpawnInfra/README.md) | No | Generate basic infrastructure |  |
| [SpclPerm](./src/SpclPerm/README.md) | No | Server owner privileges |  |
| [StatusTextManager](./src/StatusTextManager/README.md) | No | PC status text management plugin |  |
| [SwitchCommands](./src/SwitchCommands/README.md) | No | Execute commands in region |  |
| [TeleportRequest](./src/TeleportRequest/README.en-US.md) | Yes | Teleport request |  |
| [TimeRate](./src/TimeRate/README.en-US.md) | Yes | modifying time acceleration using commands, and supporting player sleep to trigger events. |  |
| [TimerKeeper](./src/TimerKeeper/README.en-US.md) | Yes | Save timer state |  |
| [TownNPCHomes](./src/TownNPCHomes/README.en-US.md) | Yes | NPC quick home |  |
| [UnseenInventory](./src/UnseenInventory/README.md) | No | Allows the server to generate items that are normally 'unobtainable' |  |
| [VeinMiner](./src/VeinMiner/README.en-US.md) | Yes | Chain mining |  |
| [VotePlus](./src/VotePlus/README.en-US.md) | Yes | Multi-function voting |  |
| [WeaponPlus](./src/WeaponPlusCostCoin/README.md) | No | Weapon enhancement coin version |  |
| [WikiLangPackLoader](./src/WikiLangPackLoader/README.md) | No | Load Chinese Wiki language pack for server |  |
| [WorldModify](https://github.com/UnrealMultiple/TShockWorldModify/blob/master/README.md) | No | World editor, can modify most of the world parameters |  |
| [ZHIPlayerManager](./src/ZHIPlayerManager/README.md) | No | zZhi's player management plugin |  |

</Details>

## Friendly Links

- [TShock Plugin Development Documentation](https://github.com/ACaiCat/TShockPluginDocument)
- [Tshock Comprehensive Navigation](https://github.com/UnrealMultiple/Tshock-nav)
