<div align="center">
  
[![TShockPlugin](https://socialify.git.ci/UnrealMultiple/TShockPlugin/image?description=1&descriptionEditable=A%20TShock%20Chinese%20Plugin%20Collection%20Repository&forks=1&issues=1&language=1&logo=https%3A%2F%2Fgithub.com%2FUnrealMultiple%2FTShockPlugin%2Fblob%2Fmaster%2Ficon.png%3Fraw%3Dtrue&name=1&pattern=Circuit%20Board&pulls=1&stargazers=1&theme=Auto)](https://github.com/UnrealMultiple/TShockPlugin)  
[![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/UnrealMultiple/TShockPlugin/.github%2Fworkflows%2Fbuild.yml)](https://github.com/UnrealMultiple/TShockPlugin/actions)
[![GitHub contributors](https://img.shields.io/github/contributors/UnrealMultiple/TShockPlugin?style=flat)](https://github.com/UnrealMultiple/TShockPlugin/graphs/contributors)
[![NET6](https://img.shields.io/badge/Core-%20.NET_6-blue)](https://dotnet.microsoft.com/zh-cn/)
[![QQ](https://img.shields.io/badge/QQ-EB1923?logo=tencent-qq&logoColor=white)](https://qm.qq.com/cgi-bin/qm/qr?k=54tOesIU5g13yVBNFIuMBQ6AzjgE6f0m&jump_from=webapi&authKey=6jzafzJEqQGzq7b2mAHBw+Ws5uOdl83iIu7CvFmrfm/Xxbo2kNHKSNXJvDGYxhSW)
[![TShock](https://img.shields.io/badge/TShock5.2.0-2B579A.svg?&logo=TShock&logoColor=white)](https://github.com/Pryaxis/TShock)

[简体中文](README.md) | **&gt; English &lt;** | [Spanish/Español](README_ES.md)

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
| [AutoPluginManager](src/AutoPluginManager/README_EN.md) | Yes | Update plugins automatically in one key | None |
| [AdditionalPylons](src/AdditionalPylons/README_EN.md) | Yes | Place more Pylons | None |
| [AnnouncementBoxPlus](src/AnnouncementBoxPlus/README.md) | No | Enhance Broadcast Box Functionality | None |
| [AutoBroadcast](src/AutoBroadcast/README_EN.md) | Yes | Automatic broadcast | None |
| [AutoAirItem](src/AutoAirItem/README_EN.md) | Yes | Automatic trash cans | None |
| [AutoClear](src/Autoclear/README_EN.md) | Yes | Intelligent automatic cleaning | None |
| [AutoReset](src/AutoReset/README_EN.md) | Yes | Fully automatic reset | None |
| [AutoStoreItems](src/AutoStoreItems/README_EN.md) | Yes | Automatic storage | None |
| [AutoTeam](src/AutoTeam/README_EN.md) | Yes | Automatic team formation | None |
| [AutoFish](src/AutoFish/README_EN.md) | Yes | Automatic fishing | None |
| [Back](src/Back/README_EN.md) | Yes | Return to the point of death | None |
| [BagPing](src/BagPing/README_EN.md) | Yes | Mark treasure bags on the map | None |
| [BetterWhitelist](src/BetterWhitelist/README_EN.md) | Yes | Whitelist plugin | None |
| [BanNpc](src/BanNpc/README_EN.md) | Yes | Prevent monster generation | None |
| [BedSet](src/BedSet/README_EN.md) | Yes | Set and record respawn points | None |
| [BridgeBuilder](src/BridgeBuilder/README_EN.md) | Yes | Quick bridge building | None |
| [BuildMaster](src/BuildMaster/README.md) | No | Red Bean Mini Game·Master Builder Mode | [MiniGamesAPI](src/MiniGamesAPI/README.md) |
| [Chireiden.TShock.Omni](https://github.com/sgkoishi/yaaiomni/blob/master/README.md) | Yes | Yet another misc plugin for TShock - the core part | None |
| [Chireiden.TShock.Omni.Misc](https://github.com/sgkoishi/yaaiomni/blob/master/README.md) | Yes | Yet another misc plugin for TShock - the miscellaneous part | [Chireiden.TShock.Omni](https://github.com/sgkoishi/yaaiomni/blob/master/README.md) |
| [CaiBot](src/CaiBot/README.md) | No | CaiBot adapter plugin | Built-in dependencies |
| [CaiPacketDebug](src/CaiPacketDebug/README.md) | No | Cai Packet Debug Tool | [TrProtocol](https://github.com/UnrealMultiple/TrProtocol) |
| [CaiCustomEmojiCommand](src/CaiCustomEmojiCommand/README_EN.md) | Yes | Custom emoji command | None |
| [CaiLib](src/CaiLib/README.md) | No | Cai's preload library | None |
| [CaiRewardChest](src/CaiRewardChest/README_EN.md) | Yes | Convert naturally generated chests into reward chests that everyone can claim once | None |
| [CGive](src/CGive/README_EN.md) | Yes | Offline commands | None |
| [Challenger](src/Challenger/README.md) | Yes | Challenger mode | None |
| [Chameleon](src/Chameleon/README_EN.md) | Yes | Login before entering the server | None |
| [ChattyBridge](src/ChattyBridge/README.md) | No | Used for cross-server chat | None |
| [ChestRestore](src/ChestRestore/README_EN.md) | Yes | Infinite items in resource servers | None |
| [CNPCShop](src/CNPCShop/README.md) | No | Custom NPC shop | None |
| [ConsoleSql](src/ConsoleSql/README.md) | No | Execute SQL statements in the console | None |
| [ConvertWorld](src/ConvertWorld/README_EN.md) | Yes | Convert world items by defeating monsters | None |
| [CustomMonster](src/CustomMonster/README_EN.md) | Yes | Customize, modify, and generate monsters and bosses | None |
| [CreateSpawn](src/CreateSpawn/README.md) | No | Spawn point building generation | None |
| [CriticalHit](src/CriticalHit/README.md) | No | Critical hit prompt | None |
| [DamageRuleLoot](src/DamageRuleLoot/README_EN.md) | Yes | Determine the drop treasure bag based on the ratio of damage and transfer damage calculation | None |
| [DamageStatistic](src/DamageStatistic/README.md) | No | Display damage caused by each player after each boss fight | None |
| [DataSync](src/DataSync/README.md) | No | Progress synchronization | None |
| [DeathDrop](src/DeathDrop/README.md) | No | Random and custom loot upon monster death | None |
| [DisableMonsLoot](src/DisableMonsLoot/README.md) | No | Prohibit monster loot | None |
| [Don't Fuck](src/DonotFuck/README.md) | Yes | Prevent swearing | None |
| [DwTP](src/DwTP/README.md) | Yes | Positioning Teleport | None |
| [DTEntryBlock](src/DTEntryBlock/README.md) | No | Prevent entry into dungeons or temples | None |
| [DumpTerrariaID](src/DumpTerrariaID/README.md) | No | Dump Terraria IDs | None |
| [Economics.Deal](src/Economics.RPG/README_EN.md) | Yes | Trading plugin | [EconomicsAPI](src/EconomicsAPI/README_EN.md) |
| [Economics.NPC](src/Economics.NPC/README_EN.md) | Yes | Custom monster rewards | [EconomicsAPI](src/EconomicsAPI/README_EN.md) |
| [Economics.Projectile](src/Economics.Projectile/README_EN.md) | Yes | Custom projectiles | [EconomicsAPI](src/EconomicsAPI/README_EN.md) [Economics.RPG](src/Economics.RPG/README_EN.md) |
| [Economics.Regain](src/Economics.Regain/README_EN.md) | Yes | Item recycling | [EconomicsAPI](src/EconomicsAPI/README_EN.md) |
| [Economics.RPG](src/Economics.RPG/README_EN.md) | Yes | RPG plugin | [EconomicsAPI](src/EconomicsAPI/README_EN.md) |
| [Economics.Shop](src/Economics.Shop/README_EN.md) | Yes | Shop plugin | [EconomicsAPI](src/EconomicsAPI/README_EN.md) [Economics.RPG](src/Economics.RPG/README_EN.md) |
| [Economics.Task](src/Economics.Task/README_EN.md) | Yes | Task plugin | [EconomicsAPI](src/EconomicsAPI/README_EN.md) [Economics.RPG](src/Economics.RPG/README_EN.md) |
| [Economics.Skill](src/Economics.Skill/README_EN.md) | Yes | Skill plugin | [EconomicsAPI](src/EconomicsAPI/README_EN.md) [Economics.RPG](src/Economics.RPG/README_EN.md) |
| [Economics.WeaponPlus](src/Economics.WeaponPlus/README_EN.md) | Yes | Weapon enhancement | [EconomicsAPI](src/EconomicsAPI/README_EN.md) |
| [EconomicsAPI](src/EconomicsAPI/README_EN.md) | Yes | Economic plugin prerequisite | None |
| [EndureBoost](src/EndureBoost/README_EN.md) | Yes | Grant specified buff when the player has a certain number of items | None |
| [EssentialsPlus](src/EssentialsPlus/README_EN.md) | Yes | Additional management commands | None |
| [Ezperm](src/Ezperm/README.md) | Yes | Batch change permissions | None |
| [FishShop](https://github.com/UnrealMultiple/TShockFishShop/blob/master/README.md) | No | Fish shop | None |
| [GenerateMap](src/GenerateMap/README.md) | No | Generate map images | [CaiLib](src/CaiLib/README.md) |
| [GolfRewards](src/GolfRewards/README.md) | No | Golf rewards | None |
| [GoodNight](src/GoodNight/README.md) | No | Curfew | None |
| [HardPlayerDrop](src/HardPlayerDrop/README.md) | No | Hardcore death drops life crystals | None |
| [HelpPlus](src/HelpPlus/README_EN.md) | Yes | Fix and enhance the Help command | None |
| [History](src/History/README.md) | No | History grid record | None |
| [HouseRegion](src/HouseRegion/README.md) | No | Land claiming plugin | None |
| [Invincibility](src/Invincibility/README.md) | No | Time-limited invincibility | None |
| [ItemPreserver](src/ItemPreserver/README.md) | No | Preserve specified items from consumption | None |
| [ItemBox](src/itemBox/README.md) | No | Off-line inventory | None |
| [ItemDecoration](src/ItemDecoration/README_EN.md) | Yes | Floating message display for held items | [LazyAPI](src/LazyAPI/README.md) |
| [JourneyUnlock](src/JourneyUnlock/README.md) | No | Unlock journey items | None |
| [LazyAPI](src/LazyAPI/README.md) | Yes | Plugin base library | linq2db |
| [LifemaxExtra](src/LifemaxExtra/README.md) | No | Eat more life fruits/crystals | None |
| [ListPlugins](src/ListPlugins/README.md) | No | List installed plugins | None |
| [ModifyWeapons](src/ModifyWeapons/README_EN.md) | Yes | ModifyWeapons | [LazyAPI](src/LazyAPI/README.md) |
| [MapTeleport](src/MapTp/README_EN.md) | Yes | Double-click on the map to teleport | None |
| [MiniGamesAPI](src/MiniGamesAPI/README.md) | No | Bean paste mini-game API | None |
| [MonsterRegen](src/MonsterRegen/README.md) | No | Monster progress regeneration | None |
| [Musicplayer](src/MusicPlayer/README.md) | No | Simple music player | None |
| [Noagent](src/Noagent/README.md) | No | Prohibit proxy IP from entering | None |
| [NormalDropsBags](src/NormalDropsBags/README.md) | No | Drop treasure bags at normal difficulty | None |
| [OnlineGiftPackage](src/OnlineGiftPackage/README.md) | No | Online gift package | None |
| [PlayerSpeed](src/PlayerSpeed/README.md) | Yes | Enable players to achieve a two-stage sprint |  [LazyAPI](src/LazyAPI/README.md)  |
| [PacketsStop](src/PacketsStop/README.md) | No | Packet interception | None |
| [PermaBuff](src/PermaBuff/README.md) | No | Permanent buff | None |
| [PerPlayerLoot](src/PerPlayerLoot/README_EN.md) | Yes | Separate chest for player loot | None |
| [PersonalPermission](src/PersonalPermission/README.md) | No | Set permissions individually for players | None |
| [Platform](src/Platform/README.md) | No | Determine player device | None |
| [PlayerManager](https://github.com/UnrealMultiple/TShockPlayerManager/blob/master/README.md) | No | Hufang's player manager | None |
| [PvPer](src/PvPer/README.md) | No | Duel system | None |
| [ProgressBag](src/ProgressBag/README.md) | No | Progress gift pack | None |
| [ProgressControls](src/ProgressControls/README.md) | No | Planbook (Automate server control) | None |
| [ProgressRestrict](src/ProgressRestrict/README.md) | No | Super progress detection | [DataSync](src/DataSync/README.md) |
| [ProxyProtocolSocket](src/ProxyProtocolSocket/README.md) | Yes | Accept proxy protocol connections | None |
| [RainbowChat](src/RainbowChat/README.md) | Yes | Random chat color | None |
| [RandomBroadcast](src/RandomBroadcast/README.md) | No | Random broadcast | None |
| [RandReSpawn](src/RandRespawn/README.md) | Yes | Random spawn point | None |
| [RealTime](src/RealTime/README.md) | No | Synchronize server time with real time | None |
| [RecipesBrowser](src/RecipesBrowser/README.md) | No | Crafting table | None |
| [RegionView](src/RegionView/README.md) | No | Display area boundaries | None |
| [ReFishTask](src/ReFishTask/README_EN.md) | Yes | Automatically refresh fisherman tasks | None |
| [Respawn](src/Respawn/README.md) | No | Respawn at the death place | None |
| [RebirthCoin](src/RebirthCoin/README_EN.md) | Yes | Consume designated items to revive player | None |
| [RestInventory](src/RestInventory/README.md) | No | Provide REST query backpack interface | None |
| [SurfaceBlock](src/SurfaceBlock/README.md) | Yes | Prohibit surface projectiles |  [LazyAPI](src/LazyAPI/README.md) |
| [Sandstorm](src/Sandstorm/README.md) | No | Toggle sandstorm | None |
| [ServerTools](src/ServerTools/README.md) | No | Server management tools | None |
| [SessionSentinel](src/SessionSentinel/README.md) | No | Handle players not sending data packets for a long time | None |
| [ShortCommand](src/ShortCommand/README.md) | No | Short command | None |
| [ShowArmors](src/ShowArmors/README.md) | No | Display equipment bar | None |
| [SignInSign](src/SignInSign/README.md) | No | Signboard login plugin | None |
| [SimultaneousUseFix](src/SimultaneousUseFix/README.md) | No | Solve problems like stuck double hammer and star spin machine gun | [Chireiden.TShock.Omni](src/https://github.com/sgkoishi/yaaiomni/releases) |
| [SmartRegions](src/SmartRegions/README.md) | No | Smart regions | None |
| [SpawnInfra](src/SpawnInfra/README.md) | No | Generate basic infrastructure | None |
| [SpclPerm](src/SpclPerm/README.md) | No | Server owner privileges | None |
| [StatusTextManager](src/StatusTextManager/README.md) | No | PC status text management plugin | None |
| [SwitchCommands](src/SwitchCommands/README.md) | No | Execute commands in region | None |
| [TeleportRequest](src/TeleportRequest/README_EN.md) | Yes | Teleport request | None |
| [TimerKeeper](src/TimerKeeper/README_EN.md) | Yes | Save timer state | None |
| [TownNPCHomes](src/TownNPCHomes/README_EN.md) | Yes | NPC quick home | None |
| [TimeRate](src/TimeRate/README_EN.md) | Yes | modifying time acceleration using commands, and supporting player sleep to trigger events. | None |
| [UnseenInventory](src/UnseenInventory/README.md) | No | Allows the server to generate items that are normally 'unobtainable' | None |
| [VeinMiner](src/VeinMiner/README.md) | Yes | Chain mining | None |
| [VotePlus](src/VotePlus/README_EN.md) | Yes | Multi-function voting | None |
| [WeaponPlusCostCoin](src/WeaponPlusCostCoin/README.md) | No | Weapon enhancement coin version | None |
| [WikiLangPackLoader](src/WikiLangPackLoader/README.md) | No | Load Chinese Wiki language pack for server | None |
| [WorldModify](https://github.com/UnrealMultiple/TShockWorldModify/blob/master/README.md) | No | World editor, can modify most of the world parameters | None |
| [ZHIPlayerManager](src/ZHIPlayerManager/README.md) | No | zZhi's player management plugin | None |
| [Lagrange.XocMat.Adapter](src/Lagrange.XocMat.Adapter/README.md) | No | Lagrange.XocMat Bot Adapter Plugin | None |

</Details>

## Friendly Links

- [TShock Plugin Development Documentation](https://github.com/ACaiCat/TShockPluginDocument)
- [Tshock Comprehensive Navigation](https://github.com/UnrealMultiple/Tshock-nav)
