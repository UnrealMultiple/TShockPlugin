<div align = "center">
  
[![TShockPlugin](https://socialify.git.ci/UnrealMultiple/TShockPlugin/image?description=1&descriptionEditable=%E4%B8%80%E4%B8%AATShock%E6%94%B6%E9%9B%86%E4%BB%93%E5%BA%93&font=Inter&forks=1&issues=1&language=1&logo=https%3A%2F%2Fgithub.com%2FUnrealMultiple%2FTShockPlugin%2Fblob%2Fmaster%2Ficon.png%3Fraw%3Dtrue&name=1&pattern=Circuit%20Board&pulls=1&stargazers=1&theme=Auto)](https://github.com/UnrealMultiple/TShockPlugin)  
[![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/UnrealMultiple/TShockPlugin/.github%2Fworkflows%2Fplugins_publish.yml)](https://github.com/UnrealMultiple/TShockPlugin/actions)
[![GitHub contributors](https://img.shields.io/github/contributors/UnrealMultiple/TShockPlugin?style=flat)](https://github.com/UnrealMultiple/TShockPlugin/graphs/contributors)
[![NET6](https://img.shields.io/badge/Core-%20.NET_6-blue)](https://dotnet.microsoft.com/zh-cn/)
[![QQ](https://img.shields.io/badge/QQ-EB1923?logo=tencent-qq&logoColor=white)](https://qm.qq.com/cgi-bin/qm/qr?k=54tOesIU5g13yVBNFIuMBQ6AzjgE6f0m&jump_from=webapi&authKey=6jzafzJEqQGzq7b2mAHBw+Ws5uOdl83iIu7CvFmrfm/Xxbo2kNHKSNXJvDGYxhSW)
[![TShock](https://img.shields.io/badge/TShock5.2.0-2B579A.svg?&logo=TShock&logoColor=white)](https://github.com/Pryaxis/TShock)

**&gt; 简体中文 &lt;** | [English](README_en.md)

</div>

## 前言
- 这是一个致力于收集整合 `TShock` 插件的仓库。
- 库中内插件容部分来源于网络收集以及反编译。
- 因项目的特殊性，可能会造成侵权行为，如有侵权请联系我们解决。
- 我们将持续收集优质的 `TShock` 插件，进行及时的更新。并跟进`TShock`的最新版本。
- 如果你也想加入我们，请按照下方`开发者注意事项`的规定对本仓库`Pull Request`。


## 使用者注意事项

- 注意有些插件可能需要前置，请查看下方列表安装前置插件。
- 每个插件都有一个使用说明，在下方列表点击超链查看具体说明事项。
- 听说喜欢给仓库点星星的人，插件都不容易报错

## 下载

- Gitee发布(国内推荐): [Plugins.zip](https://gitee.com/kksjsj/TShockPlugin/releases/download/V1.0.0.0/Plugins.zip)
- Github发布: [Plugins.zip](https://github.com/UnrealMultiple/TShockPlugin/releases/download/V1.0.0.0/Plugins.zip)


## 如何安装TShock
- 如果你是Window用户可以选择使用以下命令一次性安装(在pwsh中运行此命令即可)
```
curl -o install.ps1 https://gitee.com/kksjsj/TShockPlugin/raw/master/InstallTShock.ps1
powershell -ExecutionPolicy ByPass -File ./install.ps1 -verb runas
```

## 开发者注意事项

> 代码规范

- 禁止使用中文变量
- 禁止使用危险功能
- 尽量避免使用多线程
- 禁止为插件留有后门
- 请给每个插件项目附带一个 README.md 说明文件。

## 反馈

> 对于本代码库的任何反馈、建议或改进，将被视为公共贡献，并可能被纳入本代码库，除非明确声明其他意图。

- 如有 bug，请规范`issue`提供相关系统以及 TShock 版本 bug 复现流程。

### 已收集插件

> 点击蓝链可查看插件详细说明

<Details>
<Summary>插件列表</Summary>

| 名称                                                               |          插件说明           |                                                                        前置                                                                        |
|------------------------------------------------------------------|:-----------------------:|:------------------------------------------------------------------------------------------------------------------------------------------------:|
| [ChattyBridge](src/ChattyBridge/README.md)                           |         用于跨服聊天          |                                                                        无                                                                         |
| [EconomicsAPI](src/EconomicsAPI/README.md)                           |         经济插件前置          |                                                                        无                                                                         |
| [Economics.RPG](src/Economics.RPG/README.md)                         |           RPG           |                                                      [EconomicsAPI](src/EconomicsAPI/README.md)                                                      |
| [Economics.WeaponPlus](src/Economics.WeaponPlus/README.md)           |          强化武器           |                                                      [EconomicsAPI](src/EconomicsAPI/README.md)                                                      |
| [Economics.Deal](src/Economics.RPG/README.md)                        |          交易插件           |                                                      [EconomicsAPI](src/EconomicsAPI/README.md)                                                      |
| [Economics.Shop](src/Economics.Shop/README.md)                       |          商店插件           | [EconomicsAPI](src/EconomicsAPI/README.md)<br>[Economics.RPG](src/https://github.com/UnrealMultiple/TShockPlugin/blob/master/Economics.RPG/README.md) |
| [Economics.Skill](src/Economics.Skill/README.md)                     |          技能插件           | [EconomicsAPI](src/EconomicsAPI/README.md)<br>[Economics.RPG](src/https://github.com/UnrealMultiple/TShockPlugin/blob/master/Economics.RPG/README.md) |
| [Economics.Regain](src/Economics.Regain/README.md)                   |          物品回收           |                                                      [EconomicsAPI](src/EconomicsAPI/README.md)                                                      |
| [Economics.Projectile](src/Economics.Projectile/README.md)           |          自定义弹幕          |                                [EconomicsAPI](src/EconomicsAPI/README.md)<br>[Economics.RPG](src/Economics.RPG/README.md)                                |
| [Economics.NPC](src/Economics.NPC/README.md)                         |         自定义怪物奖励         |                                                      [EconomicsAPI](src/EconomicsAPI/README.md)                                                      |
| [Economics.Task](src/Economics.Task/README.md)                       |          任务插件           | [EconomicsAPI](src/EconomicsAPI/README.md)<br>[Economics.RPG](src/https://github.com/UnrealMultiple/TShockPlugin/blob/master/Economics.RPG/README.md) |
| [CreateSpawn](src/CreateSpawn/README.md)                             |         出生点建筑生成         |                                                                        无                                                                         |
| [AutoBroadcast](src/AutoBroadcast/README.md)                         |          自动广播           |                                                                        无                                                                         |
| [AutoTeam](src/AutoTeam/README.md)                                   |          自动队伍           |                                                                        无                                                                         |
| [BridgeBuilder](src/BridgeBuilder/README.md)                         |          快速铺桥           |                                                                        无                                                                         |
| [OnlineGiftPackage](src/OnlineGiftPackage/README.md)                 |          在线礼包           |                                                                        无                                                                         |
| [LifemaxExtra](src/LifemaxExtra/README.md)                           |        吃更多生命果/水晶        |                                                                        无                                                                         |
| [DisableMonsLoot](src/DisableMonsLoot/README.md)                     |          禁怪物掉落          |                                                                        无                                                                         |
| [PermaBuff](src/PermaBuff/README.md)                                 |         永久 Buff         |                                                                        无                                                                         |
| [ShortCommand](src/ShortCommand/README.md)                           |          简短指令           |                                                                        无                                                                         |
| [ProgressBag](src/ProgressBag/README.md)                             |          进度礼包           |                                                                        无                                                                         |
| [CriticalHit](src/CriticalHit/README.md)                             |          击打提示           |                                                                        无                                                                         |
| [Back](src/Back/README.md)                                           |          死亡回溯           |                                                                        无                                                                         |
| [BanNpc](src/BanNpc/README.md)                                       |         阻止怪物生成          |                                                                        无                                                                         |
| [MapTeleport](src/MapTp/README.md)                                   |         双击大地图传送         |                                                                        无                                                                         |
| [RandReSpawn](src/RandRespawn/README.md)                             |          随机出生点          |                                                                        无                                                                         |
| [CGive](src/CGive/README.md)                                         |          离线命令           |                                                                        无                                                                         |
| [RainbowChat](src/RainbowChat/README.md)                             |        每次说话颜色不一样        |                                                                        无                                                                         |
| [NormalDropsBags](src/NormalDropsBags/README.md)                     |         普通难度宝藏袋         |                                                                        无                                                                         |
| [DisableSurfaceProjectiles](src/DisableSurfaceProjectiles/README.md) |          禁地表弹幕          |                                                                        无                                                                         |
| [RecipesBrowser](src/RecipesBrowser/README.md)                       |           合成表           |                                                                        无                                                                         |
| [DisableGodMod](src/DisableGodMod/README.md)                         |         阻止玩家无敌          |                                                                        无                                                                         |
| [TownNPCHomes](src/TownNPCHomes/README.md)                           |        NPC 快速回家         |                                                                        无                                                                         |
| [RegionView](src/RegionView/README.md)                               |         显示区域边界          |                                                                        无                                                                         |
| [Noagent](src/Noagent/README.md)                                     |       禁止代理 ip 进入        |                                                                        无                                                                         |
| [SwitchCommands](src/SwitchCommands/README.md)                       |         区域执行指令          |                                                                        无                                                                         |
| [GolfRewards](src/GolfRewards/README.md)                             |          高尔夫奖励          |                                                                        无                                                                         |
| [DataSync](src/DataSync/README.md)                                   |          进度同步           |                                                                        无                                                                         |
| [ProgressRestrict](src/ProgressRestrict/README.md)                   |          超进度检测          |                                                          [DataSync](src/DataSync/README.md)                                                          |
| [PacketsStop](src/PacketsStop/README.md)                             |          数据包拦截          |                                                                        无                                                                         |
| [DeathDrop](src/DeathDrop/README.md)                                 |     怪物死亡随机和自定义掉落物品      |                                                                        无                                                                         |
| [DTEntryBlock](src/DTEntryBlock/README.md)                           |        阻止进入地牢或神庙        |                                                                        无                                                                         |
| [PerPlayerLoot](src/PerPlayerLoot/README.md)                         |        玩家战利品单独箱子        |                                                                        无                                                                         |
| [PvPer](src/PvPer/README.md)                                         |          决斗系统           |                                                                        无                                                                         |
| [DumpTerrariaID](src/DumpTerrariaID/README.md)                       |          输出 ID          |                                                                        无                                                                         |
| [DamageStatistic](src/DamageStatistic/README.md)                     |          伤害统计           |                                                                        无                                                                         |
| [AdditionalPylons](src/AdditionalPylons/README.md)                   |         放置更多晶塔          |                                                                        无                                                                         |
| [History](src/History/README.md)                                     |         历史图格记录          |                                                                        无                                                                         |
| [Invincibility](src/Invincibility/README.md)                         |          限时无敌           |                                                                        无                                                                         |
| [Ezperm](src/Ezperm/README.md)                                       |          批量改权限          |                                                                        无                                                                         |
| [AutoClear](src/Autoclear/README.md)                                 |         智能自动扫地          |                                                                        无                                                                         |
| [EssentialsPlus](src/EssentialsPlus/README.md)                       |         更多管理指令          |                                                                        无                                                                         |
| [ShowArmors](src/ShowArmors/README.md)                               |          展示装备栏          |                                                                        无                                                                         |
| [VeinMiner](src/VeinMiner/README.md)                                 |          连锁挖矿           |                                                                        无                                                                         |
| [PersonalPermission](src/PersonalPermission/README.md)               |        为玩家单独设置权限        |                                                                        无                                                                         |
| [ItemPreserver](src/ItemPreserver/README.md)                         |         指定物品不消耗         |                                                                        无                                                                         |
| [SimultaneousUseFix](src/SimultaneousUseFix/README.md)               |     解决卡双锤卡星旋机枪之类的问题     |                                      [Chireiden.TShock.Omni](src/https://github.com/sgkoishi/yaaiomni/releases)                                      |
| [Challenger](src/Challenger/README.md)                               |          挑战者模式          |                                                                        无                                                                         |
| [MiniGamesAPI](src/MiniGamesAPI/README.md)                           |        豆沙小游戏 API        |                                                                        无                                                                         |
| [BuildMaster](src/BuildMaster/README.md)                             |      豆沙小游戏·建筑大师模式       |                                                      [MiniGamesAPI](src/MiniGamesAPI/README.md)                                                      |
| [JourneyUnlock](src/JourneyUnlock/README.md)                         |         解锁旅途物品          |                                                                        无                                                                         |
| [ListPlugins](src/ListPlugins/README.md)                             |          查已装插件          |                                                                        无                                                                         |
| [BagPing](src/BagPing/README.md)                                     |        地图上标记宝藏袋         |                                                                        无                                                                         |
| [ServerTools](src/ServerTools/README.md)                             |         服务器管理工具         |                                                                        无                                                                         |
| [Platform](src/Platform/README.md)                                   |         判断玩家设备          |                                                                        无                                                                         |
| [CaiLib](src/CaiLib/README.md)                                       |        Cai 的前置库         |                                                                        无                                                                         |
| [GenerateMap](src/GenerateMap/README.md)                             |         生成地图图片          |                                                            [CaiLib](src/CaiLib/README.md)                                                            |
| [RestInventory](src/RestInventory/README.md)                         |     提供 REST 查询背包接口      |                                                                        无                                                                         |
| [WikiLangPackLoader](src/WikiLangPackLoader/README.md)               |     为服务器加载 Wiki 语言包     |                                                                        无                                                                         |
| [HelpPlus](src/HelpPlus/README.md)                                   |      修复和增强 Help 命令      |                                                                        无                                                                         |
| [CaiBot](src/CaiBot/README.md)                                       |       CaiBot 适配插件       |                                                                       自带前置                                                                       |
| [HouseRegion](src/HouseRegion/README.md)                             |          圈地插件           |                                                                        无                                                                         |
| [SignInSign](src/SignInSign/README.md)                               |         告示牌登录插件         |                                                                        无                                                                         |
| [WeaponPlusCostCoin](src/WeaponPlusCostCoin/README.md)               |         武器强化钱币版         |                                                                        无                                                                         |
| [Respawn](src/Respawn/README.md)                                     |          原地复活           |                                                                        无                                                                         |
| [EndureBoost](src/EndureBoost/README.md)                             |     物品一定数量后长时间buff      |                                                                        无                                                                         |
| [AnnouncementBoxPlus](src/AnnouncementBoxPlus/README.md)             |         广播盒功能强化         |                                                                        无                                                                         |
| [ConsoleSql](src/ConsoleSql/README.md)                               |     允许你在控制台执行SQL语句      |                                                                        无                                                                         |
| [ProgressControl](src/ProgressControls/README.md)                    |      计划书（自动化控制服务器）      |                                                                        无                                                                         |
| [RealTime](src/RealTime/README.md)                                   |      使服务器内时间同步现实时间      |                                                                        无                                                                         |
| [GoodNight](src/GoodNight/README.md)                                 |           宵禁            |                                                                        无                                                                         |
| [Musicplayer](src/musicplayer/README.md)                             |         简易音乐播放器         |                                                                        无                                                                         |
| [TimerKeeper](src/TimerKeeper/README.md)                             |         保存计时器状态         |                                                                        无                                                                         |
| [Chameleon](src/Chameleon/README.md)                                 |          进服前登录          |                                                                        无                                                                         |
| [AutoPluginManager](src/AutoPluginManager/README.md)                 |        一键自动更新插件         |                                                                        无                                                                         |
| [SpclPerm](src/SpclPerm/README.md)                                   |          服主特权           |                                                                        无                                                                         |
| [MonsterRegen](src/MonsterRegen/README.md)                           |         怪物进度回血          |                                                                        无                                                                         |
| [HardPlayerDrop](src/HardPlayerDrop/README.md)                       |        硬核死亡掉生命水晶        |                                                                        无                                                                         |
| [ReFishTask](src/ReFishTask/README.md)                               |        自动刷新渔夫任务         |                                                                        无                                                                         |
| [Sandstorm](src/Sandstorm/README.md)                                 |          切换沙尘暴          |                                                                        无                                                                         |
| [RandomBroadcast](src/RandomBroadcast/README.md)                     |          随机广播           |                                                                        无                                                                         |
| [BedSet](src/BedSet/README.md)                                       |        设置并记录重生点         |                                                                        无                                                                         |
| [ConvertWorld](src/ConvertWorld/README.md)                           |       击败怪物转换世界物品        |                                                                        无                                                                         |
| [AutoStoreItems](src/AutoStoreItems/README.md)                       |          自动储存           |                                                                        无                                                                         |
| [ZHIPlayerManager](src/ZHIPlayerManager/README.md)                   |       zhi的玩家管理插件        |                                                                        无                                                                         |
| [SpawnInfra](src/SpawnInfra/README.md)                               |         生成基础建设          |                                                                        无                                                                         |
| [CNPCShop](src/CNPCShop/README.md)                                   |        自定义NPC商店         |                                                                        无                                                                         |
| [SessionSentinel](src/SessionSentinel/README.md)                     |     处理长时间不发送数据包的玩家      |                                                                        无                                                                         |
| [TeleportRequest](src/TeleportRequest/README.md)                     |          传送请求           |                                                                        无                                                                         |
| [CaiRewardChest](src/CaiRewardChest/README.md)                       | 将自然生成的箱子变为所有人都可以领一次的奖励箱 |                                                                        无                                                                         |
| [CaiCustomEmojiCommand](src/CaiCustomEmojiCommand/README.md)         |         自定义表情命令         |                                                                        无                                                                         |
| [BetterWhitelist](src/BetterWhitelist/README.md)                     |          白名单插件          |                                                                        无                                                                         |
| [AutoReset](src/AutoReset/README.md)                                 |         完全自动重置          |                                                                        无                                                                         |
| [SmartRegions](src/SmartRegions/README.md)                           |          智能区域           |                                                                        无                                                                         |
| [ProxyProtocolSocket](src/ProxyProtocolSocket/README.md)             |  接受 proxy protocol 协议   |                                                                        无                                                                         |
| [UnseenInventory](src/UnseenInventory/README.md)                     |  允许服务器端生成“无法获取”的物品   |                                                                        无                                                                         |
| [ChestRestore](src/ChestRestore/README.md)                           |  资源服无限物品   |                                                                        无                                                                         |

</Details>

## 代码贡献

[![Contributors](https://stats.deeptrain.net/contributor/UnrealMultiple/TShockPlugin)](https://github.com/UnrealMultiple/TShockPlugin/graphs/contributors)

## 友情链接

- [TShock 插件开发文档](https://github.com/ACaiCat/TShockPluginDocument)
- [Tshock 相关内容大全导航](https://github.com/UnrealMultiple/Tshock-nav)
