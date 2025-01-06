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
- ApmApi Release: [Plugins.zip](http://api.terraria.ink:11434/plugin/get_all_plugins)

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

| Plugin Name | Translation Percentage | Plugin Description | Dependencies |
| :-: | :-: | :-: | :-: |
| [AdditionalPylons](./src/AdditionalPylons/README.en-US.md) | 100.0% | Place more Pylons | [LazyAPI](./src/LazyAPI/README.md) |
| [AnnouncementBoxPlus](./src/AnnouncementBoxPlus/README.md) | 0.0% | Enhance Broadcast Box Functionality | [LazyAPI](./src/LazyAPI/README.md) |
| [AutoAirItem](./src/AutoAirItem/README.en-US.md) | 100.0% | Automatic trash cans | [LazyAPI](./src/LazyAPI/README.md) |
| [AutoBroadcast](./src/AutoBroadcast/README.en-US.md) | 100.0% | Automatic broadcast | [LazyAPI](./src/LazyAPI/README.md) |
| [AutoClear](./src/AutoClear/README.en-US.md) | 100.0% | Intelligent automatic cleaning | [LazyAPI](./src/LazyAPI/README.md) |
| [AutoFish](./src/AutoFish/README.en-US.md) | 100.0% | Automatic fishing | [LazyAPI](./src/LazyAPI/README.md) |
| [AutoPluginManager](./src/AutoPluginManager/README.en-US.md) | 100.0% | Update plugins automatically in one key |  |
| [AutoReset](./src/AutoReset/README.en-US.md) | 75.0% | Fully automatic reset | [LazyAPI](./src/LazyAPI/README.md) |
| [AutoStoreItems](./src/AutoStoreItems/README.en-US.md) | 100.0% | Automatic storage | [LazyAPI](./src/LazyAPI/README.md) |
| [AutoTeam](./src/AutoTeam/README.en-US.md) | 100.0% | Automatic team formation | [LazyAPI](./src/LazyAPI/README.md) |
| [Back](./src/Back/README.en-US.md) | 100.0% | Return to the point of death | [LazyAPI](./src/LazyAPI/README.md) |
| [BagPing](./src/BagPing/README.en-US.md) | 100.0% | Mark treasure bags on the map |  |
| [BanNpc](./src/BanNpc/README.en-US.md) | 100.0% | Prevent monster generation | [LazyAPI](./src/LazyAPI/README.md) |
| [BedSet](./src/BedSet/README.en-US.md) | 100.0% | Set and record respawn points | [LazyAPI](./src/LazyAPI/README.md) |
| [BetterWhitelist](./src/BetterWhitelist/README.en-US.md) | 100.0% | Whitelist plugin | [LazyAPI](./src/LazyAPI/README.md) |
| [BridgeBuilder](./src/BridgeBuilder/README.en-US.md) | 100.0% | Quick bridge building | [LazyAPI](./src/LazyAPI/README.md) |
| [BuildMaster](./src/BuildMaster/README.md) | 0.0% | Red Bean Mini Game·Master Builder Mode | [MiniGamesAPI](./src/MiniGamesAPI/README.md) |
| [CaiBot](./src/CaiBot/README.md) | 0.0% | CaiBot adapter plugin (Only support QQ) |  |
| [CaiCustomEmojiCommand](./src/CaiCustomEmojiCommand/README.en-US.md) | 100.0% | Custom emoji command | [LazyAPI](./src/LazyAPI/README.md) |
| [CaiLib](./src/CaiLib/README.md) | 0.0% | Cai's preload library | [SixLabors.ImageSharp]() |
| [CaiPacketDebug](./src/CaiPacketDebug/README.md) | 0.0% | Cai Packet Debug Tool | [LazyAPI](./src/LazyAPI/README.md) [TrProtocol]() |
| [CaiRewardChest](./src/CaiRewardChest/README.en-US.md) | 100.0% | Convert naturally generated chests into reward chests that everyone can claim once | [linq2db]() [LazyAPI](./src/LazyAPI/README.md) |
| [CGive](./src/CGive/README.en-US.md) | 100.0% | Offline commands |  |
| [Challenger](./src/Challenger/README.en-US.md) | 100.0% | Challenger mode |  |
| [Chameleon](./src/Chameleon/README.en-US.md) | 100.0% | Login before entering the server | [LazyAPI](./src/LazyAPI/README.md) |
| [ChattyBridge](./src/ChattyBridge/README.md) | 0.0% | Used for cross-server chat | [LazyAPI](./src/LazyAPI/README.md) |
| [ChestRestore](./src/ChestRestore/README.en-US.md) | 100.0% | Infinite items in resource servers |  |
| [Chireiden.TShock.Omni](https://github.com/sgkoishi/yaaiomni/blob/master/README.md) | 0.0% | Yet another misc plugin for TShock - the core part |  |
| [Chireiden.TShock.Omni.Misc](https://github.com/sgkoishi/yaaiomni/blob/master/README.md) | 0.0% | Yet another misc plugin for TShock - the miscellaneous part | [Chireiden.TShock.Omni](https://github.com/sgkoishi/yaaiomni/blob/master/README.md) |
| [CNPCShop](./src/CNPCShop/README.md) | 0.0% | Custom NPC shop |  |
| [ConsoleSql](./src/ConsoleSql/README.md) | 0.0% | Execute SQL statements in the console |  |
| [ConvertWorld](./src/ConvertWorld/README.en-US.md) | 100.0% | Convert world items by defeating monsters |  |
| [CreateSpawn](./src/CreateSpawn/README.md) | 0.0% | Spawn point building generation | [LazyAPI](./src/LazyAPI/README.md) |
| [CriticalHit](./src/CriticalHit/README.en-US.md) | 0.0% | Critical hit prompt |  |
| [Crossplay](https://github.com/UnrealMultiple/Crossplay/blob/main/README.md) | 0.0% | Allows for cross-platform play |  |
| [CustomMonster](./src/CustomMonster/README.en-US.md) | 100.0% | Customize, modify, and generate monsters and bosses  |  |
| [DamageRuleLoot](./src/DamageRuleLoot/README.en-US.md) | 100.0% | Determine the drop treasure bag based on the ratio of damage and transfer damage calculation |  |
| [DamageStatistic](./src/DamageStatistic/README.md) | 0.0% | Display damage caused by each player after each boss fight |  |
| [DataSync](./src/DataSync/README.md) | 0.0% | Progress synchronization |  |
| [DeathDrop](./src/DeathDrop/README.md) | 0.0% | Random and custom loot upon monster death |  |
| [DisableMonsLoot](./src/DisableMonsLoot/README.md) | 0.0% | Prohibit monster loot |  |
| [DonotFuck](./src/DonotFuck/README.en-US.md) | 100.0% | Prevent swearing | [LazyAPI](./src/LazyAPI/README.md) |
| [DTEntryBlock](./src/DTEntryBlock/README.md) | 0.0% | Prevent entry into dungeons or temples |  |
| [DumpTerrariaID](./src/DumpTerrariaID/README.md) | 0.0% | Dump Terraria IDs |  |
| [DwTP](./src/DwTP/README.en-US.md) | 100.0% | Positioning Teleport |  |
| [Economics.Deal](./src/Economics.Deal/README.en-US.md) | 95.8% | Trading plugin | [EconomicsAPI](./src/EconomicsAPI/README.en-US.md) |
| [Economics.NPC](./src/Economics.NPC/README.en-US.md) | 0.0% | Custom monster rewards | [EconomicsAPI](./src/EconomicsAPI/README.en-US.md) |
| [Economics.Projectile](./src/Economics.Projectile/README.en-US.md) | 0.0% | Custom projectiles | [EconomicsAPI](./src/EconomicsAPI/README.en-US.md) [Economics.RPG](./src/Economics.RPG/README.en-US.md) |
| [Economics.Regain](./src/Economics.Regain/README.en-US.md) | 92.3% | Item recycling | [EconomicsAPI](./src/EconomicsAPI/README.en-US.md) |
| [Economics.RPG](./src/Economics.RPG/README.en-US.md) | 94.4% | RPG plugin | [EconomicsAPI](./src/EconomicsAPI/README.en-US.md) |
| [Economics.Shop](./src/Economics.Shop/README.en-US.md) | 93.3% | Shop plugin | [EconomicsAPI](./src/EconomicsAPI/README.en-US.md) [Economics.RPG](./src/Economics.RPG/README.en-US.md) |
| [Economics.Skill](./src/Economics.Skill/README.en-US.md) | 100.0% | Skill plugin | [EconomicsAPI](./src/EconomicsAPI/README.en-US.md) [Jint]() [Economics.RPG](./src/Economics.RPG/README.en-US.md) |
| [Economics.Task](./src/Economics.Task/README.en-US.md) | 100.0% | Task plugin | [EconomicsAPI](./src/EconomicsAPI/README.en-US.md) [Economics.RPG](./src/Economics.RPG/README.en-US.md) |
| [Economics.WeaponPlus](./src/Economics.WeaponPlus/README.en-US.md) | 100.0% | Weapon enhancement | [EconomicsAPI](./src/EconomicsAPI/README.en-US.md) |
| [EconomicsAPI](./src/EconomicsAPI/README.en-US.md) | 86.2% | Economic plugin prerequisite |  |
| [EndureBoost](./src/EndureBoost/README.en-US.md) | 100.0% | Grant specified buff when the player has a certain number of items |  |
| [EssentialsPlus](./src/EssentialsPlus/README.en-US.md) | 100.0% | Additional management commands |  |
| [Ezperm](./src/Ezperm/README.en-US.md) | 100.0% | Batch change permissions |  |
| [FishShop](https://github.com/UnrealMultiple/TShockFishShop/blob/master/README.md) | 0.0% | Fish shop |  |
| [GenerateMap](./src/GenerateMap/README.md) | 0.0% | Generate map images | [CaiLib](./src/CaiLib/README.md) |
| [GolfRewards](./src/GolfRewards/README.md) | 0.0% | Golf rewards |  |
| [GoodNight](./src/GoodNight/README.md) | 0.0% | Curfew |  |
| [HardPlayerDrop](./src/HardPlayerDrop/README.md) | 0.0% | Hardcore death drops life crystals |  |
| [HelpPlus](./src/HelpPlus/README.en-US.md) | 100.0% | Fix and enhance the Help command |  |
| [History](./src/History/README.md) | 0.0% | History grid record |  |
| [HouseRegion](./src/HouseRegion/README.md) | 0.0% | Land claiming plugin |  |
| [Invincibility](./src/Invincibility/README.md) | 0.0% | Time-limited invincibility |  |
| [ItemBox](./src/ItemBox/README.md) | 0.0% | Off-line inventory |  |
| [ItemDecoration](./src/ItemDecoration/README.en-US.md) | 0.0% | Floating message display for held items | [LazyAPI](./src/LazyAPI/README.md) |
| [ItemPreserver](./src/ItemPreserver/README.md) | 0.0% | Preserve specified items from consumption |  |
| [JourneyUnlock](./src/JourneyUnlock/README.md) | 0.0% | Unlock journey items |  |
| [Lagrange.XocMat.Adapter](./src/Lagrange.XocMat.Adapter/README.md) | 0.0% | Lagrange.XocMat Bot Adapter Plugin | [SixLabors.ImageSharp]() |
| [LazyAPI](./src/LazyAPI/README.md) | 0.0% | Plugin base library | [linq2db]() |
| [LifemaxExtra](./src/LifemaxExtra/README.en-US.md) | 0.0% | Eat more life fruits/crystals | [LazyAPI](./src/LazyAPI/README.md) |
| [ListPlugins](./src/ListPlugins/README.md) | 0.0% | List installed plugins |  |
| [MapTp](./src/MapTp/README.en-US.md) | 100.0% | Double-click on the map to teleport |  |
| [MiniGamesAPI](./src/MiniGamesAPI/README.md) | 0.0% | Bean paste mini-game API |  |
| [ModifyWeapons](./src/ModifyWeapons/README.en-US.md) | 100.0% | ModifyWeapons | [LazyAPI](./src/LazyAPI/README.md) |
| [MonsterRegen](./src/MonsterRegen/README.md) | 0.0% | Monster progress regeneration |  |
| [MusicPlayer](./src/MusicPlayer/README.md) | 0.0% | Simple music player |  |
| [Noagent](./src/Noagent/README.md) | 0.0% | Prohibit proxy IP from entering |  |
| [NormalDropsBags](./src/NormalDropsBags/README.md) | 0.0% | Drop treasure bags at normal difficulty |  |
| [OnlineGiftPackage](./src/OnlineGiftPackage/README.md) | 0.0% | Online gift package |  |
| [PacketsStop](./src/PacketsStop/README.md) | 0.0% | Packet interception |  |
| [PermaBuff](./src/PermaBuff/README.md) | 0.0% | Permanent buff |  |
| [PerPlayerLoot](./src/PerPlayerLoot/README.en-US.md) | 100.0% | Separate chest for player loot |  |
| [PersonalPermission](./src/PersonalPermission/README.md) | 0.0% | Set permissions individually for players |  |
| [Platform](./src/Platform/README.md) | 0.0% | Determine player device |  |
| [PlayerManager](https://github.com/UnrealMultiple/TShockPlayerManager/blob/master/README.md) | 0.0% | Hufang's player manager |  |
| [PlayerRandomSwapper](./src/PlayerRandomSwapper/README.en-US.md) | 100.0% | Random Player Position Swap | [LazyAPI](./src/LazyAPI/README.md) |
| [PlayerSpeed](./src/PlayerSpeed/README.en-US.md) | 100.0% | Enable players to achieve a two-stage sprint | [LazyAPI](./src/LazyAPI/README.md) |
| [ProgressBag](./src/ProgressBag/README.md) | 0.0% | Progress gift pack |  |
| [ProgressControls](./src/ProgressControls/README.md) | 0.0% | Planbook (Automate server control) |  |
| [ProgressRestrict](./src/ProgressRestrict/README.md) | 0.0% | Super progress detection | [DataSync](./src/DataSync/README.md) |
| [ProxyProtocolSocket](./src/ProxyProtocolSocket/README.md) | 100.0% | Accept proxy protocol connections |  |
| [PvPer](./src/PvPer/README.md) | 0.0% | Duel system |  |
| [RainbowChat](./src/RainbowChat/README.md) | 100.0% | Random chat color |  |
| [RandomBroadcast](./src/RandomBroadcast/README.md) | 0.0% | Random broadcast |  |
| [RandRespawn](./src/RandRespawn/README.en-US.md) | 0.0% | Random spawn point |  |
| [RealTime](./src/RealTime/README.md) | 0.0% | Synchronize server time with real time |  |
| [RebirthCoin](./src/RebirthCoin/README.en-US.md) | 100.0% | Consume designated items to revive player |  |
| [RecipesBrowser](./src/RecipesBrowser/README.md) | 0.0% | Crafting table |  |
| [ReFishTask](./src/ReFishTask/README.en-US.md) | 100.0% | Automatically refresh fisherman tasks |  |
| [RegionView](./src/RegionView/README.md) | 0.0% | Display area boundaries |  |
| [Respawn](./src/Respawn/README.md) | 0.0% | Respawn at the death place |  |
| [RestInventory](./src/RestInventory/README.md) | 0.0% | Provide REST query backpack interface |  |
| [RolesModifying](./src/RolesModifying/README.md) | 0.0% | Modify player backpack |  |
| [Sandstorm](./src/Sandstorm/README.md) | 0.0% | Toggle sandstorm |  |
| [ServerTools](./src/ServerTools/README.en-US.md) | 0.0% | Server management tools | [LazyAPI](./src/LazyAPI/README.md) [linq2db]() |
| [SessionSentinel](./src/SessionSentinel/README.md) | 0.0% | Handle players not sending data packets for a long time |  |
| [ShortCommand](./src/ShortCommand/README.md) | 0.0% | Short command |  |
| [ShowArmors](./src/ShowArmors/README.md) | 0.0% | Display equipment bar |  |
| [SignInSign](./src/SignInSign/README.md) | 0.0% | Signboard login plugin |  |
| [SimultaneousUseFix](./src/SimultaneousUseFix/README.md) | 0.0% | Solve problems like stuck double hammer and star spin machine gun | [Chireiden.TShock.Omni](https://github.com/sgkoishi/yaaiomni/blob/master/README.md) |
| [SmartRegions](./src/SmartRegions/README.md) | 0.0% | Smart regions |  |
| [SpawnInfra](./src/SpawnInfra/README.md) | 0.0% | Generate basic infrastructure |  |
| [SpclPerm](./src/SpclPerm/README.md) | 0.0% | Server owner privileges |  |
| [StatusTextManager](./src/StatusTextManager/README.md) | 0.0% | PC status text management plugin |  |
| [SurfaceBlock](./src/SurfaceBlock/README.en-US.md) | 100.0% | Prohibit surface projectiles | [LazyAPI](./src/LazyAPI/README.md) |
| [SwitchCommands](./src/SwitchCommands/README.md) | 0.0% | Execute commands in region |  |
| [TeleportRequest](./src/TeleportRequest/README.en-US.md) | 100.0% | Teleport request |  |
| [TimeRate](./src/TimeRate/README.en-US.md) | 100.0% | modifying time acceleration using commands, and supporting player sleep to trigger events. |  |
| [TimerKeeper](./src/TimerKeeper/README.en-US.md) | 100.0% | Save timer state |  |
| [TownNPCHomes](./src/TownNPCHomes/README.en-US.md) | 100.0% | NPC quick home |  |
| [TShockConfigMultiLang](./src/TShockConfigMultiLang/README.en-US.md) | 0.0% | TShock configuration language localization | [LazyAPI](./src/LazyAPI/README.md) |
| [UnseenInventory](./src/UnseenInventory/README.md) | 0.0% | Allows the server to generate items that are normally 'unobtainable' |  |
| [VBY.Common](https://github.com/UnrealMultiple/MyPlugin/blob/master/docs/VBY.Common.md) | 0.0% | Foundation library for VBY plugins |  |
| [VBY.GameContentModify](https://github.com/UnrealMultiple/MyPlugin/blob/master/docs/VBY.GameContentModify.md) | 0.0% | Customizable modifications for certain game content (super) | [VBY.Common](https://github.com/UnrealMultiple/MyPlugin/blob/master/docs/VBY.Common.md) |
| [VBY.OtherCommand](https://github.com/UnrealMultiple/MyPlugin/blob/master/docs/VBY.OtherCommand.md) | 0.0% | Provide some other auxiliary commands | [VBY.Common](https://github.com/UnrealMultiple/MyPlugin/blob/master/docs/VBY.Common.md) |
| [VBY.PluginLoader](https://github.com/UnrealMultiple/MyPlugin/blob/master/docs/VBY.PluginLoader.md) | 0.0% | A plugin loader that allows hot reloading |  |
| [VeinMiner](./src/VeinMiner/README.en-US.md) | 100.0% | Chain mining |  |
| [VotePlus](./src/VotePlus/README.en-US.md) | 100.0% | Multi-function voting |  |
| [WeaponPlus](./src/WeaponPlus/README.md) | 0.0% | Weapon enhancement coin version |  |
| [WikiLangPackLoader](./src/WikiLangPackLoader/README.md) | 0.0% | Load Chinese Wiki language pack for server |  |
| [WorldModify](https://github.com/UnrealMultiple/TShockWorldModify/blob/master/README.md) | 0.0% | World editor, can modify most of the world parameters |  |
| [ZHIPlayerManager](./src/ZHIPlayerManager/README.en-US.md) | 92.8% | zZhi's player management plugin |  |

</Details>

## Friendly Links

- [TShock Plugin Development Documentation](https://github.com/ACaiCat/TShockPluginDocument)
- [Tshock Comprehensive Navigation](https://github.com/UnrealMultiple/Tshock-nav)
