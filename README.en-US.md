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
- We will continue to collect high-quality TShock plugins for timely updates. And follow up on the latest version of `TShock`.
- If you also want to join us, please submit PR to this repository in accordance with the provisions of `Developer Notes` in the repository.
- If you want to participate in the translation work, please visit our [Crowdin](https://zh.crowdin.com/project/tshock-chinese-plugin) link

## User Notes
- Before installing the plugin, please check the documentation of the corresponding plugin: [TShock Chinese plugin library document](http://docs.terraria.ink/zh/)
- Note that some plugins may require `install dependencies`, please check the list below to install dependent plugins.
- Each plugin has a instructions for use. Click the link in the list below to view specific instructions.

> [!IMPORTANT]
> Many plugins rely on `LazyAPI`, while `LazyAPI` depends on `linq2db`. Before using this repository plugin, it is recommended to install `LazyAPI.dll` and `linq2db.dll` first.
> It is recommended to use `AutoPluginManager` to install this plugin library plugin, which will automatically fill in the corresponding dependencies.

## Download (recommended to use APM to install plugin)

- APM Mirror (domestic recommendation): [Plugins.zip](http://api.terraria.ink:11434/plugin/get_all_plugins)
- Github Release: [Plugins.zip](https://github.com/UnrealMultiple/TShockPlugin/releases/download/V1.0.0.0/Plugins.zip)

#### AutoPluginManager Plugin (Recommended)
     /apm l List all plugins
     /apm i <Plugin Name> One-click installation plugins
     /apm u [Plugin Name] Check and update plugin

## Feedback

- [Feedback BUG](https://github.com/UnrealMultiple/TShockPlugin/issues/new?template=report_bug.yaml)  

- [New Feature Suggestions](https://github.com/UnrealMultiple/TShockPlugin/issues/new?template=feature_request.yaml)

- [Blank Issue](https://github.com/UnrealMultiple/TShockPlugin/issues/new)

> [!IMPORTANT]
> When feedbacking the bug, be sure to fill in the template as required to provide detailed details. If necessary, please install the `pdb` file.


### Collected Plugins

> Click on the `link` to view the `plugin document`

> [!NOTE]
> The English plugin documentation might not be updated as promptly as the Chinese plugin documentation.
> Please try to refer to the Chinese documentation whenever possible.

<Details>
<Summary>Plugin List</Summary>

| Plugin Name | Translation Percentage | Plugin Description | Dependencies |
| :-: | :-: | :-: | :-: |
| [AdditionalPylons](./src/AdditionalPylons/README.en-US.md) | 100.0% | Place more Pylons | [LazyAPI](./src/LazyAPI/README.en-US.md) |
| [AIChatPlugin](./src/AIChatPlugin/README.en-US.md) | 100.0% | AIChatPlugin |  |
| [AnnouncementBoxPlus](./src/AnnouncementBoxPlus/README.en-US.md) | 100.0% | Enhance Broadcast Box Functionality | [LazyAPI](./src/LazyAPI/README.en-US.md) |
| [AutoAirItem](./src/AutoAirItem/README.en-US.md) | 100.0% | Automatic trash cans | [LazyAPI](./src/LazyAPI/README.en-US.md) |
| [AutoBroadcast](./src/AutoBroadcast/README.en-US.md) | 100.0% | Automatic broadcast | [LazyAPI](./src/LazyAPI/README.en-US.md) |
| [AutoClassificationQuickStack](./src/AutoClassificationQuickStack/README.md) | 0.0% | Auto Classification Quick Stack |  |
| [AutoClear](./src/AutoClear/README.en-US.md) | 75.0% | Intelligent automatic cleaning | [LazyAPI](./src/LazyAPI/README.en-US.md) |
| [AutoFish](./src/AutoFish/README.en-US.md) | 100.0% | Automatic fishing | [LazyAPI](./src/LazyAPI/README.en-US.md) |
| [AutoPluginManager](./src/AutoPluginManager/README.en-US.md) | 93.2% | Update plugins automatically in one key |  |
| [AutoReset](./src/AutoReset/README.en-US.md) | 100.0% | Fully automatic reset | [LazyAPI](./src/LazyAPI/README.en-US.md) |
| [AutoStoreItems](./src/AutoStoreItems/README.en-US.md) | 100.0% | Automatic storage | [LazyAPI](./src/LazyAPI/README.en-US.md) |
| [AutoTeam](./src/AutoTeam/README.en-US.md) | 100.0% | Automatic team formation | [LazyAPI](./src/LazyAPI/README.en-US.md) |
| [Back](./src/Back/README.en-US.md) | 100.0% | Return to the point of death | [LazyAPI](./src/LazyAPI/README.en-US.md) |
| [BagPing](./src/BagPing/README.en-US.md) | 33.3% | Mark treasure bags on the map |  |
| [BanNpc](./src/BanNpc/README.en-US.md) | 100.0% | Prevent monster generation | [LazyAPI](./src/LazyAPI/README.en-US.md) |
| [BedSet](./src/BedSet/README.en-US.md) | 100.0% | Set and record respawn points | [LazyAPI](./src/LazyAPI/README.en-US.md) |
| [BetterWhitelist](./src/BetterWhitelist/README.en-US.md) | 100.0% | Whitelist plugin | [LazyAPI](./src/LazyAPI/README.en-US.md) |
| [BossLock](./src/BossLock/README.md) | 0.0% | 进度锁插件 |  |
| [BridgeBuilder](./src/BridgeBuilder/README.en-US.md) | 100.0% | Quick bridge building | [LazyAPI](./src/LazyAPI/README.en-US.md) |
| [BuildMaster](./src/BuildMaster/README.en-US.md) | 100.0% | Red Bean Mini Game·Master Builder Mode | [MiniGamesAPI](./src/MiniGamesAPI/README.en-US.md) |
| [CaiBotLite](./src/CaiBotLite/README.md) | 100.0% | CaiBot adapter plugin (Only support QQ) |  |
| [CaiCustomEmojiCommand](./src/CaiCustomEmojiCommand/README.en-US.md) | 100.0% | Custom emoji command | [LazyAPI](./src/LazyAPI/README.en-US.md) |
| [CaiPacketDebug](./src/CaiPacketDebug/README.en-US.md) | 100.0% | Cai Packet Debug Tool | [LazyAPI](./src/LazyAPI/README.en-US.md) [TrProtocol]() |
| [CaiRewardChest](./src/CaiRewardChest/README.en-US.md) | 100.0% | Convert naturally generated chests into reward chests that everyone can claim once | [linq2db]() [LazyAPI](./src/LazyAPI/README.en-US.md) |
| [CGive](./src/CGive/README.en-US.md) | 92.9% | Offline commands |  |
| [Challenger](./src/Challenger/README.en-US.md) | 100.0% | Challenger mode |  |
| [Chameleon](./src/Chameleon/README.en-US.md) | 100.0% | Login before entering the server | [LazyAPI](./src/LazyAPI/README.en-US.md) |
| [ChattyBridge](./src/ChattyBridge/README.en-US.md) | 33.3% | Used for cross-server chat | [LazyAPI](./src/LazyAPI/README.en-US.md) |
| [ChestRestore](./src/ChestRestore/README.en-US.md) | 80.0% | Infinite items in resource servers |  |
| [Chireiden.TShock.Omni](https://github.com/sgkoishi/yaaiomni/blob/master/README.md) | 0.0% | Yet another misc plugin for TShock - the core part |  |
| [Chireiden.TShock.Omni.Misc](https://github.com/sgkoishi/yaaiomni/blob/master/README.md) | 0.0% | Yet another misc plugin for TShock - the miscellaneous part | [Chireiden.TShock.Omni](https://github.com/sgkoishi/yaaiomni/blob/master/README.md) |
| [CNPCShop](./src/CNPCShop/README.en-US.md) | 100.0% | Custom NPC shop |  |
| [ConsoleSql](./src/ConsoleSql/README.en-US.md) | 100.0% | Execute SQL statements in the console |  |
| [ConvertWorld](./src/ConvertWorld/README.en-US.md) | 100.0% | Convert world items by defeating monsters |  |
| [CreateSpawn](./src/CreateSpawn/README.en-US.md) | 10.3% | Spawn point building generation | [LazyAPI](./src/LazyAPI/README.en-US.md) |
| [CriticalHit](./src/CriticalHit/README.en-US.md) | 100.0% | Critical hit prompt |  |
| [Crossplay](https://github.com/UnrealMultiple/Crossplay/blob/main/README.md) | 0.0% | Allows for cross-platform play |  |
| [CustomMonster](./src/CustomMonster/README.en-US.md) | 100.0% | Customize, modify, and generate monsters and bosses  |  |
| [DamageRuleLoot](./src/DamageRuleLoot/README.en-US.md) | 100.0% | Determine the drop treasure bag based on the ratio of damage and transfer damage calculation |  |
| [DamageStatistic](./src/DamageStatistic/README.en-US.md) | 100.0% | Display damage caused by each player after each boss fight |  |
| [DataSync](./src/DataSync/README.en-US.md) | 100.0% | Progress synchronization |  |
| [DeathDrop](./src/DeathDrop/README.en-US.md) | 100.0% | Random and custom loot upon monster death |  |
| [DisableMonsLoot](./src/DisableMonsLoot/README.en-US.md) | 100.0% | Prohibit monster loot |  |
| [DonotFuck](./src/DonotFuck/README.en-US.md) | 100.0% | Prevent swearing | [LazyAPI](./src/LazyAPI/README.en-US.md) |
| [DTEntryBlock](./src/DTEntryBlock/README.en-US.md) | 100.0% | Prevent entry into dungeons or temples |  |
| [Dummy](./src/Dummy/README.en-US.md) | 100.0% | Dummy client | [LazyAPI](./src/LazyAPI/README.en-US.md) [TrProtocol]() |
| [DumpTerrariaID](./src/DumpTerrariaID/README.en-US.md) | 100.0% | Dump Terraria IDs |  |
| [DwTP](./src/DwTP/README.en-US.md) | 100.0% | Positioning Teleport |  |
| [Economics.Core](./src/Economics.Core/README.en-US.md) | 92.9% | Economic plugin prerequisite |  |
| [Economics.Deal](./src/Economics.Deal/README.en-US.md) | 100.0% | Trading plugin | [Economics.Core](./src/Economics.Core/README.en-US.md) |
| [Economics.NPC](./src/Economics.NPC/README.en-US.md) | 100.0% | Custom monster rewards | [Economics.Core](./src/Economics.Core/README.en-US.md) |
| [Economics.Projectile](./src/Economics.Projectile/README.en-US.md) | 100.0% | Custom projectiles | [Economics.Core](./src/Economics.Core/README.en-US.md) [Economics.RPG](./src/Economics.RPG/README.en-US.md) |
| [Economics.Regain](./src/Economics.Regain/README.en-US.md) | 100.0% | Item recycling | [Economics.Core](./src/Economics.Core/README.en-US.md) |
| [Economics.RPG](./src/Economics.RPG/README.en-US.md) | 100.0% | RPG plugin | [Economics.Core](./src/Economics.Core/README.en-US.md) |
| [Economics.Shop](./src/Economics.Shop/README.en-US.md) | 100.0% | Shop plugin | [Economics.Core](./src/Economics.Core/README.en-US.md) [Economics.RPG](./src/Economics.RPG/README.en-US.md) |
| [Economics.Skill](./src/Economics.Skill/README.en-US.md) | 72.5% | Skill plugin | [Economics.Core](./src/Economics.Core/README.en-US.md) [Economics.RPG](./src/Economics.RPG/README.en-US.md) |
| [Economics.Task](./src/Economics.Task/README.en-US.md) | 100.0% | Task plugin | [Economics.Core](./src/Economics.Core/README.en-US.md) [Economics.RPG](./src/Economics.RPG/README.en-US.md) |
| [Economics.WeaponPlus](./src/Economics.WeaponPlus/README.en-US.md) | 100.0% | Weapon enhancement | [Economics.Core](./src/Economics.Core/README.en-US.md) |
| [EndureBoost](./src/EndureBoost/README.en-US.md) | 100.0% | Grant specified buff when the player has a certain number of items |  |
| [EssentialsPlus](./src/EssentialsPlus/README.en-US.md) | 95.6% | Additional management commands | [LazyAPI](./src/LazyAPI/README.en-US.md) |
| [Ezperm](./src/Ezperm/README.en-US.md) | 100.0% | Batch change permissions | [LazyAPI](./src/LazyAPI/README.en-US.md) |
| [FishShop](https://github.com/UnrealMultiple/TShockFishShop/blob/master/README.md) | 0.0% | Fish shop |  |
| [GenerateMap](./src/GenerateMap/README.en-US.md) | 100.0% | Generate map | [CaiLib]() |
| [GolfRewards](./src/GolfRewards/README.en-US.md) | 100.0% | Golf rewards |  |
| [GoodNight](./src/GoodNight/README.md) | 4.9% | Curfew |  |
| [HardPlayerDrop](./src/HardPlayerDrop/README.en-US.md) | 100.0% | Hardcore death drops life crystals |  |
| [HelpPlus](./src/HelpPlus/README.en-US.md) | 100.0% | Fix and enhance the Help command |  |
| [History](./src/History/README.en-US.md) | 100.0% | History grid record |  |
| [HouseRegion](./src/HouseRegion/README.en-US.md) | 100.0% | Land claiming plugin | [LazyAPI](./src/LazyAPI/README.en-US.md) |
| [Invincibility](./src/Invincibility/README.en-US.md) | 100.0% | Time-limited invincibility |  |
| [ItemBox](./src/ItemBox/README.en-US.md) | 100.0% | Off-line inventory |  |
| [ItemDecoration](./src/ItemDecoration/README.en-US.md) | 100.0% | Floating message display for held items | [LazyAPI](./src/LazyAPI/README.en-US.md) |
| [ItemPreserver](./src/ItemPreserver/README.en-US.md) | 100.0% | Preserve specified items from consumption |  |
| [JourneyUnlock](./src/JourneyUnlock/README.en-US.md) | 100.0% | Unlock journey items |  |
| [Lagrange.XocMat.Adapter](./src/Lagrange.XocMat.Adapter/README.en-US.md) | 100.0% | Lagrange.XocMat Bot Adapter Plugin |  |
| [LazyAPI](./src/LazyAPI/README.en-US.md) | 100.0% | Plugin base library | [linq2db]() |
| [LifemaxExtra](./src/LifemaxExtra/README.en-US.md) | 100.0% | Eat more life fruits/crystals | [LazyAPI](./src/LazyAPI/README.en-US.md) |
| [ListPlugins](./src/ListPlugins/README.en-US.md) | 100.0% | List installed plugins |  |
| [MapTp](./src/MapTp/README.en-US.md) | 100.0% | Double-click on the map to teleport |  |
| [MiniGamesAPI](./src/MiniGamesAPI/README.en-US.md) | 100.0% | Bean paste mini-game API |  |
| [ModifyWeapons](./src/ModifyWeapons/README.en-US.md) | 100.0% | ModifyWeapons | [LazyAPI](./src/LazyAPI/README.en-US.md) |
| [MonsterRegen](./src/MonsterRegen/README.md) | 100.0% | Monster progress regeneration |  |
| [MusicPlayer](./src/MusicPlayer/README.en-US.md) | 100.0% | Simple music player |  |
| [Noagent](./src/Noagent/README.en-US.md) | 100.0% | Prohibit proxy IP from entering |  |
| [NormalDropsBags](./src/NormalDropsBags/README.en-US.md) | 100.0% | Drop treasure bags at normal difficulty |  |
| [NoteWall](./src/NoteWall/README.en-US.md) | 100.0% | Players can leave and view notes here. | [LazyAPI](./src/LazyAPI/README.en-US.md) [linq2db]() |
| [OnlineGiftPackage](./src/OnlineGiftPackage/README.en-US.md) | 100.0% | Online gift package |  |
| [PacketsStop](./src/PacketsStop/README.en-US.md) | 100.0% | Packet interception |  |
| [PermaBuff](./src/PermaBuff/README.md) | 100.0% | Permanent buff |  |
| [PerPlayerLoot](./src/PerPlayerLoot/README.en-US.md) | 100.0% | Separate chest for player loot |  |
| [PersonalPermission](./src/PersonalPermission/README.en-US.md) | 100.0% | Set permissions individually for players |  |
| [Platform](./src/Platform/README.en-US.md) | 100.0% | Determine player device |  |
| [PlayerManager](https://github.com/UnrealMultiple/TShockPlayerManager/blob/master/README.md) | 0.0% | Hufang's player manager |  |
| [PlayerRandomSwapper](./src/PlayerRandomSwapper/README.en-US.md) | 100.0% | Random Player Position Swap | [LazyAPI](./src/LazyAPI/README.en-US.md) |
| [PlayerSpeed](./src/PlayerSpeed/README.en-US.md) | 100.0% | Enable players to achieve a two-stage sprint | [LazyAPI](./src/LazyAPI/README.en-US.md) |
| [ProgressBag](./src/ProgressBag/README.en-US.md) | 100.0% | Progress gift pack | [LazyAPI](./src/LazyAPI/README.en-US.md) |
| [ProgressControls](./src/ProgressControls/README.md) | 0.4% | Planbook (Automate server control) |  |
| [ProgressRestrict](./src/ProgressRestrict/README.en-US.md) | 100.0% | Super progress detection | [DataSync](./src/DataSync/README.en-US.md) |
| [ProxyProtocolSocket](./src/ProxyProtocolSocket/README.en-US.md) | 100.0% | Accept proxy protocol connections |  |
| [PvPer](./src/PvPer/README.md) | 1.3% | Duel system |  |
| [QRCoder](./src/QRCoder/README.en-US.md) | 100.0% | QR Code Generator |  |
| [RainbowChat](./src/RainbowChat/README.en-US.md) | 100.0% | Random chat color |  |
| [RandomBroadcast](./src/RandomBroadcast/README.md) | 100.0% | Random broadcast |  |
| [RandRespawn](./src/RandRespawn/README.en-US.md) | 100.0% | Random spawn point |  |
| [RealTime](./src/RealTime/README.en-US.md) | 100.0% | Synchronize server time with real time |  |
| [RebirthCoin](./src/RebirthCoin/README.en-US.md) | 100.0% | Consume designated items to revive player |  |
| [RecipesBrowser](./src/RecipesBrowser/README.en-US.md) | 100.0% | Crafting table |  |
| [ReFishTask](./src/ReFishTask/README.en-US.md) | 100.0% | Automatically refresh fisherman tasks |  |
| [RegionView](./src/RegionView/README.en-US.md) | 100.0% | Display area boundaries |  |
| [Respawn](./src/Respawn/README.en-US.md) | 100.0% | Respawn at the death place |  |
| [RestInventory](./src/RestInventory/README.en-US.md) | 100.0% | Provide REST query backpack interface |  |
| [ReverseWorld](./src/ReverseWorld/README.en-US.md) | 100.0% | World Reversal and Landmine Placement |  |
| [RolesModifying](./src/RolesModifying/README.en-US.md) | 100.0% | Modify player backpack |  |
| [Sandstorm](./src/Sandstorm/README.en-US.md) | 100.0% | Toggle sandstorm |  |
| [ServerTools](./src/ServerTools/README.en-US.md) | 83.0% | Server management tools | [LazyAPI](./src/LazyAPI/README.en-US.md) [linq2db]() |
| [SessionSentinel](./src/SessionSentinel/README.en-US.md) | 100.0% | Handle players not sending data packets for a long time |  |
| [ShortCommand](./src/ShortCommand/README.en-US.md) | 100.0% | Short command |  |
| [ShowArmors](./src/ShowArmors/README.en-US.md) | 100.0% | Display equipment bar |  |
| [SignInSign](./src/SignInSign/README.en-US.md) | 100.0% | Signboard login plugin |  |
| [SimultaneousUseFix](./src/SimultaneousUseFix/README.en-US.md) | 100.0% | Solve problems like stuck double hammer and star spin machine gun | [Chireiden.TShock.Omni](https://github.com/sgkoishi/yaaiomni/blob/master/README.md) |
| [SmartRegions](./src/SmartRegions/README.en-US.md) | 100.0% | Smart regions |  |
| [SpawnInfra](./src/SpawnInfra/README.md) | 100.0% | Generate basic infrastructure |  |
| [SpclPerm](./src/SpclPerm/README.en-US.md) | 100.0% | Server owner privileges |  |
| [StatusTextManager](./src/StatusTextManager/README.en-US.md) | 100.0% | PC status text management plugin |  |
| [SurfaceBlock](./src/SurfaceBlock/README.en-US.md) | 100.0% | Prohibit surface projectiles | [LazyAPI](./src/LazyAPI/README.en-US.md) |
| [SurvivalCrisis](./src/SurvivalCrisis/README.md) | 0.0% | 'Among Us' like game' |  |
| [SwitchCommands](./src/SwitchCommands/README.en-US.md) | 100.0% | Execute commands in region |  |
| [TeleportRequest](./src/TeleportRequest/README.en-US.md) | 100.0% | Teleport request |  |
| [TimeRate](./src/TimeRate/README.en-US.md) | 100.0% | modifying time acceleration using commands, and supporting player sleep to trigger events. |  |
| [TimerKeeper](./src/TimerKeeper/README.en-US.md) | 100.0% | Save timer state |  |
| [TownNPCHomes](./src/TownNPCHomes/README.en-US.md) | 100.0% | NPC quick home |  |
| [TransferPatch](./src/TransferPatch/README.en-US.md) | 50.0% | Transfer Plugin Config Patch |  |
| [UnseenInventory](./src/UnseenInventory/README.en-US.md) | 100.0% | Allows the server to generate items that are normally 'unobtainable' |  |
| [VBY.Common](https://github.com/UnrealMultiple/MyPlugin/blob/master/docs/VBY.Common.md) | 0.0% | Foundation library for VBY plugins |  |
| [VBY.GameContentModify](https://github.com/UnrealMultiple/MyPlugin/blob/master/docs/VBY.GameContentModify.md) | 0.0% | Customizable modifications for certain game content (super) | [VBY.Common](https://github.com/UnrealMultiple/MyPlugin/blob/master/docs/VBY.Common.md) |
| [VBY.OtherCommand](https://github.com/UnrealMultiple/MyPlugin/blob/master/docs/VBY.OtherCommand.md) | 0.0% | Provide some other auxiliary commands | [VBY.Common](https://github.com/UnrealMultiple/MyPlugin/blob/master/docs/VBY.Common.md) |
| [VBY.PluginLoader](https://github.com/UnrealMultiple/MyPlugin/blob/master/docs/VBY.PluginLoader.md) | 0.0% | A plugin loader that allows hot reloading |  |
| [VBY.PluginLoaderAutoReload](https://github.com/UnrealMultiple/MyPlugin/blob/master/docs/VBY.PluginLoaderAutoReload.md) | 0.0% | VBY.PluginLoader的扩展, 自动热重载插件 | [VBY.PluginLoader](https://github.com/UnrealMultiple/MyPlugin/blob/master/docs/VBY.PluginLoader.md) |
| [VeinMiner](./src/VeinMiner/README.en-US.md) | 100.0% | Chain mining |  |
| [VotePlus](./src/VotePlus/README.en-US.md) | 100.0% | Multi-function voting |  |
| [WeaponPlus](./src/WeaponPlus/README.en-US.md) | 100.0% | Weapon enhancement coin version |  |
| [WikiLangPackLoader](./src/WikiLangPackLoader/README.en-US.md) | 100.0% | Load Chinese Wiki language pack for server |  |
| [WorldModify](https://github.com/UnrealMultiple/TShockWorldModify/blob/master/README.md) | 0.0% | World editor, can modify most of the world parameters |  |
| [ZHIPlayerManager](./src/ZHIPlayerManager/README.en-US.md) | 100.0% | zZhi's player management plugin |  |

</Details>


## Friendly links

- [TShock Plugin Development Documentation](https://github.com/ACaiCat/TShockPluginDocument)
- [Tshock Related Content Navigation](https://github.com/UnrealMultiple/Tshock-nav)
