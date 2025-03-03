<div align="center">

[![TShockPlugin](https://socialify.git.ci/UnrealMultiple/TShockPlugin/image?description=1&descriptionEditable=A%20TShock%20Chinese%20Plugin%20Collection%20Repository&forks=1&issues=1&language=1&logo=https%3A%2F%2Fgithub.com%2FUnrealMultiple%2FTShockPlugin%2Fblob%2Fmaster%2Ficon.png%3Fraw%3Dtrue&name=1&pattern=Circuit%20Board&pulls=1&stargazers=1&theme=Auto)](https://github.com/UnrealMultiple/TShockPlugin)
[![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/UnrealMultiple/TShockPlugin/.github%2Fworkflows%2Fbuild.yml)](https://github.com/UnrealMultiple/TShockPlugin/actions)
[![GitHub contributors](https://img.shields.io/github/contributors/UnrealMultiple/TShockPlugin?style=flat)](https://github.com/UnrealMultiple/TShockPlugin/graphs/contributors)
[![NET6](https://img.shields.io/badge/Core-%20.NET_6-blue)](https://dotnet.microsoft.com/zh-cn/)
[![QQ](https://img.shields.io/badge/QQ-EB1923?logo=tencent-qq&logoColor=white)](https://qm.qq.com/cgi-bin/qm/qr?k=54tOesIU5g13yVBNFIuMBQ6AzjgE6f0m&jump_from=webapi&authKey=6jzafzJEqQGzq7b2mAHBw+Ws5uOdl83iIu7CvFmrfm/Xxbo2kNHKSNXJvDGYxhSW)
[![TShock](https://img.shields.io/badge/TShock5.2.0-2B579A.svg?&logo=TShock&logoColor=white)](https://github.com/Pryaxis/TShock)

**&gt; 简体中文 &lt;** | [English](README.en-US.md) | [Spanish/Español](README.es-ES.md)

</div>

## 前言
- 这是一个致力于收集整合 `TShock` 插件的仓库。
- 库中部分插件内容来源于网络收集以及反编译。
- 因项目的特殊性，可能会造成侵权行为，如有侵权请联系我们解决。
- 我们将持续收集优质的 `TShock` 插件，进行及时的更新。并跟进`TShock`的最新版本。
- 如果你也想加入我们，请按照下方`开发者注意事项`的规定对本仓库`Pull Request`。


## 使用者注意事项
- 插件文档： [TShock中文插件库文档](http://docs.terraria.ink/zh/)
- 注意有些插件可能需要安装依赖，请查看下方列表安装依赖插件。
- 每个插件都有一个使用说明，在下方列表点击超链接查看具体说明事项。
- 听说喜欢给仓库点星星的人，插件都不容易报错

## 下载

- ApmApi(国内推荐): [Plugins.zip](http://api.terraria.ink:11434/plugin/get_all_plugins)
- Github发布: [Plugins.zip](https://github.com/UnrealMultiple/TShockPlugin/releases/download/V1.0.0.0/Plugins.zip)

#### AutoPluginManager插件 (本仓库推出的插件管理器)  
     /apm l 列出所有插件  
     /apm i <插件名> 一键安装插件  
     /apm u [插件名] 检查并且更新插件  

## 开发者注意事项

> 代码规范

- 禁止使用中文变量
- 禁止使用危险功能
- 尽量避免使用多线程
- 禁止为插件留有后门
- 请给每个插件项目附带一个 README.md 说明文件。

## 反馈

> 对于本代码库的任何反馈、建议或改进，将被视为公共贡献，并可能被纳入本代码库，除非明确声明其他意图。

- 如有 bug，请在 GitHub 的 `issue` 页提供相关系统信息、 TShock 版本 以及 bug 复现流程。

### 已收集插件

> 点击超链接可查看插件详细说明

<Details>
<Summary>插件列表</Summary>

| 名称 | 插件说明 | 依赖 |
| :-: | :-: | :-: |
| [AdditionalPylons](./src/AdditionalPylons/README.md) | 放置更多晶塔 | [LazyAPI](./src/LazyAPI/README.md) |
| [AIChatPlugin](./src/AIChatPlugin/README.md) | AI聊天插件 |  |
| [AnnouncementBoxPlus](./src/AnnouncementBoxPlus/README.md) | 广播盒功能强化 | [LazyAPI](./src/LazyAPI/README.md) |
| [AutoAirItem](./src/AutoAirItem/README.md) | 自动垃圾桶插件 | [LazyAPI](./src/LazyAPI/README.md) |
| [AutoBroadcast](./src/AutoBroadcast/README.md) | 自动广播 | [LazyAPI](./src/LazyAPI/README.md) |
| [AutoClear](./src/AutoClear/README.md) | 智能自动扫地 | [LazyAPI](./src/LazyAPI/README.md) |
| [AutoFish](./src/AutoFish/README.md) | 自动钓鱼 | [LazyAPI](./src/LazyAPI/README.md) |
| [AutoPluginManager](./src/AutoPluginManager/README.md) | 一键自动更新插件 |  |
| [AutoReset](./src/AutoReset/README.md) | 完全自动重置 | [LazyAPI](./src/LazyAPI/README.md) |
| [AutoStoreItems](./src/AutoStoreItems/README.md) | 自动储存 | [LazyAPI](./src/LazyAPI/README.md) |
| [AutoTeam](./src/AutoTeam/README.md) | 自动队伍 | [LazyAPI](./src/LazyAPI/README.md) |
| [Back](./src/Back/README.md) | 死亡回溯 | [LazyAPI](./src/LazyAPI/README.md) |
| [BagPing](./src/BagPing/README.md) | 地图上标记宝藏袋 |  |
| [BanNpc](./src/BanNpc/README.md) | 阻止怪物生成 | [LazyAPI](./src/LazyAPI/README.md) |
| [BedSet](./src/BedSet/README.md) | 设置并记录重生点 | [LazyAPI](./src/LazyAPI/README.md) |
| [BetterWhitelist](./src/BetterWhitelist/README.md) | 白名单插件 | [LazyAPI](./src/LazyAPI/README.md) |
| [BridgeBuilder](./src/BridgeBuilder/README.md) | 快速铺桥 | [LazyAPI](./src/LazyAPI/README.md) |
| [BuildMaster](./src/BuildMaster/README.md) | 豆沙小游戏·建筑大师模式 | [MiniGamesAPI](./src/MiniGamesAPI/README.md) |
| [CaiBot](./src/CaiBot/README.md) | CaiBot 适配插件 |  |
| [CaiBotLite](./src/CaiBotLite/README.md) | CaiBot 官方机器人适配插件 |  |
| [CaiCustomEmojiCommand](./src/CaiCustomEmojiCommand/README.md) | 自定义表情命令 | [LazyAPI](./src/LazyAPI/README.md) |
| [CaiLib](./src/CaiLib/README.md) | Cai 的前置库 | [SixLabors.ImageSharp]() |
| [CaiPacketDebug](./src/CaiPacketDebug/README.md) | Cai数据包调试工具 | [LazyAPI](./src/LazyAPI/README.md) [TrProtocol]() |
| [CaiRewardChest](./src/CaiRewardChest/README.md) | 将自然生成的箱子变为所有人都可以领一次的奖励箱 | [linq2db]() [LazyAPI](./src/LazyAPI/README.md) |
| [CGive](./src/CGive/README.md) | 离线命令 |  |
| [Challenger](./src/Challenger/README.md) | 挑战者模式 |  |
| [Chameleon](./src/Chameleon/README.md) | 进服前登录 | [LazyAPI](./src/LazyAPI/README.md) |
| [ChattyBridge](./src/ChattyBridge/README.md) | 用于跨服聊天 | [LazyAPI](./src/LazyAPI/README.md) |
| [ChestRestore](./src/ChestRestore/README.md) | 资源服无限物品 |  |
| [Chireiden.TShock.Omni](https://github.com/sgkoishi/yaaiomni/blob/master/README.md) | 恋恋工具箱核心,用于修复各种TShock问题 (建议安装) |  |
| [Chireiden.TShock.Omni.Misc](https://github.com/sgkoishi/yaaiomni/blob/master/README.md) | 恋恋工具箱扩展 | [Chireiden.TShock.Omni](https://github.com/sgkoishi/yaaiomni/blob/master/README.md) |
| [CNPCShop](./src/CNPCShop/README.md) | 自定义NPC商店 |  |
| [ConsoleSql](./src/ConsoleSql/README.md) | 允许你在控制台执行SQL语句 |  |
| [ConvertWorld](./src/ConvertWorld/README.md) | 击败怪物转换世界物品 |  |
| [CreateSpawn](./src/CreateSpawn/README.md) | 出生点建筑生成 | [LazyAPI](./src/LazyAPI/README.md) |
| [CriticalHit](./src/CriticalHit/README.md) | 击打提示 |  |
| [Crossplay](https://github.com/UnrealMultiple/Crossplay/blob/main/README.md) | 跨版本游玩 |  |
| [CustomMonster](./src/CustomMonster/README.md) | 自定义怪物插件 |  |
| [DamageRuleLoot](./src/DamageRuleLoot/README.md) | 伤害规则掉落 |  |
| [DamageStatistic](./src/DamageStatistic/README.md) | 在每次 Boss 战后显示每个玩家造成的伤害 |  |
| [DataSync](./src/DataSync/README.md) | 进度同步 |  |
| [DeathDrop](./src/DeathDrop/README.md) | 怪物死亡随机和自定义掉落物品 |  |
| [DisableMonsLoot](./src/DisableMonsLoot/README.md) | 禁怪物掉落 |  |
| [DonotFuck](./src/DonotFuck/README.md) | 禁止脏话 | [LazyAPI](./src/LazyAPI/README.md) |
| [DTEntryBlock](./src/DTEntryBlock/README.md) | 阻止进入地牢或神庙 |  |
| [Dummy](./src/Dummy/README.md) | 服务器假人 | [LazyAPI](./src/LazyAPI/README.md) [TrProtocol]() |
| [DumpTerrariaID](./src/DumpTerrariaID/README.md) | 输出 ID |  |
| [DwTP](./src/DwTP/README.md) | 定位传送 |  |
| [Economics.Deal](./src/Economics.Deal/README.md) | 交易插件 | [EconomicsAPI](./src/EconomicsAPI/README.md) |
| [Economics.NPC](./src/Economics.NPC/README.md) | 自定义怪物奖励 | [EconomicsAPI](./src/EconomicsAPI/README.md) |
| [Economics.Projectile](./src/Economics.Projectile/README.md) | 自定义弹幕 | [EconomicsAPI](./src/EconomicsAPI/README.md) [Economics.RPG](./src/Economics.RPG/README.md) |
| [Economics.Regain](./src/Economics.Regain/README.md) | 物品回收 | [EconomicsAPI](./src/EconomicsAPI/README.md) |
| [Economics.RPG](./src/Economics.RPG/README.md) | RPG | [EconomicsAPI](./src/EconomicsAPI/README.md) |
| [Economics.Shop](./src/Economics.Shop/README.md) | 商店插件 | [EconomicsAPI](./src/EconomicsAPI/README.md) [Economics.RPG](./src/Economics.RPG/README.md) |
| [Economics.Skill](./src/Economics.Skill/README.md) | 技能插件 | [EconomicsAPI](./src/EconomicsAPI/README.md) [Jint]() [Economics.RPG](./src/Economics.RPG/README.md) |
| [Economics.Task](./src/Economics.Task/README.md) | 任务插件 | [EconomicsAPI](./src/EconomicsAPI/README.md) [Economics.RPG](./src/Economics.RPG/README.md) |
| [Economics.WeaponPlus](./src/Economics.WeaponPlus/README.md) | 强化武器 | [EconomicsAPI](./src/EconomicsAPI/README.md) |
| [EconomicsAPI](./src/EconomicsAPI/README.md) | 经济插件前置 |  |
| [EndureBoost](./src/EndureBoost/README.md) | 拥有指定数量物品给指定buff |  |
| [EssentialsPlus](./src/EssentialsPlus/README.md) | 更多管理指令 | [LazyAPI](./src/LazyAPI/README.md) |
| [Ezperm](./src/Ezperm/README.md) | 批量改权限 | [LazyAPI](./src/LazyAPI/README.md) |
| [FishShop](https://github.com/UnrealMultiple/TShockFishShop/blob/master/README.md) | 鱼店 |  |
| [GenerateMap](./src/GenerateMap/README.md) | 生成地图图片 | [CaiLib](./src/CaiLib/README.md) |
| [GolfRewards](./src/GolfRewards/README.md) | 高尔夫奖励 |  |
| [GoodNight](./src/GoodNight/README.md) | 宵禁 |  |
| [HardPlayerDrop](./src/HardPlayerDrop/README.md) | 硬核死亡掉生命水晶 |  |
| [HelpPlus](./src/HelpPlus/README.md) | 修复和增强 Help 命令 |  |
| [History](./src/History/README.md) | 历史图格记录 |  |
| [HouseRegion](./src/HouseRegion/README.md) | 圈地插件 | [LazyAPI](./src/LazyAPI/README.md) |
| [Invincibility](./src/Invincibility/README.md) | 限时无敌 |  |
| [ItemBox](./src/ItemBox/README.md) | 离线背包系统，物品盒子 |  |
| [ItemDecoration](./src/ItemDecoration/README.md) | 手持物品浮动消息显示 | [LazyAPI](./src/LazyAPI/README.md) |
| [ItemPreserver](./src/ItemPreserver/README.md) | 指定物品不消耗 |  |
| [JourneyUnlock](./src/JourneyUnlock/README.md) | 解锁旅途物品 |  |
| [Lagrange.XocMat.Adapter](./src/Lagrange.XocMat.Adapter/README.md) | Lagrange.XocMat的适配插件 | [SixLabors.ImageSharp]() |
| [LazyAPI](./src/LazyAPI/README.md) | 插件基础库 | [linq2db]() |
| [LifemaxExtra](./src/LifemaxExtra/README.md) | 吃更多生命果/水晶 | [LazyAPI](./src/LazyAPI/README.md) |
| [ListPlugins](./src/ListPlugins/README.md) | 查已装插件 |  |
| [MapTp](./src/MapTp/README.md) | 双击大地图传送 |  |
| [MiniGamesAPI](./src/MiniGamesAPI/README.md) | 豆沙小游戏 API |  |
| [ModifyWeapons](./src/ModifyWeapons/README.md) | 修改武器 | [LazyAPI](./src/LazyAPI/README.md) |
| [MonsterRegen](./src/MonsterRegen/README.md) | 怪物进度回血 |  |
| [MusicPlayer](./src/MusicPlayer/README.md) | 简易音乐播放器 |  |
| [Noagent](./src/Noagent/README.md) | 禁止代理 ip 进入 |  |
| [NormalDropsBags](./src/NormalDropsBags/README.md) | 普通难度宝藏袋 |  |
| [NoteWall](./src/NoteWall/README.md) | 留言墙 | [LazyAPI](./src/LazyAPI/README.md) [linq2db]() |
| [OnlineGiftPackage](./src/OnlineGiftPackage/README.md) | 在线礼包 |  |
| [PacketsStop](./src/PacketsStop/README.md) | 数据包拦截 |  |
| [PermaBuff](./src/PermaBuff/README.md) | 永久 Buff |  |
| [PerPlayerLoot](./src/PerPlayerLoot/README.md) | 玩家战利品单独箱子 |  |
| [PersonalPermission](./src/PersonalPermission/README.md) | 为玩家单独设置权限 |  |
| [Platform](./src/Platform/README.md) | 判断玩家设备 |  |
| [PlayerManager](https://github.com/UnrealMultiple/TShockPlayerManager/blob/master/README.md) | Hufang的玩家管理器 |  |
| [PlayerRandomSwapper](./src/PlayerRandomSwapper/README.md) | 玩家位置随机交换 | [LazyAPI](./src/LazyAPI/README.md) |
| [PlayerSpeed](./src/PlayerSpeed/README.md) | 玩家速度 | [LazyAPI](./src/LazyAPI/README.md) |
| [ProgressBag](./src/ProgressBag/README.md) | 进度礼包 | [LazyAPI](./src/LazyAPI/README.md) |
| [ProgressControls](./src/ProgressControls/README.md) | 计划书（自动化控制服务器） |  |
| [ProgressRestrict](./src/ProgressRestrict/README.md) | 超进度检测 | [DataSync](./src/DataSync/README.md) |
| [ProxyProtocolSocket](./src/ProxyProtocolSocket/README.md) | 接受 proxy protocol 协议 |  |
| [PvPer](./src/PvPer/README.md) | 决斗系统 |  |
| [RainbowChat](./src/RainbowChat/README.md) | 每次说话颜色不一样 |  |
| [RandomBroadcast](./src/RandomBroadcast/README.md) | 随机广播 |  |
| [RandRespawn](./src/RandRespawn/README.md) | 随机出生点 |  |
| [RealTime](./src/RealTime/README.md) | 使服务器内时间同步现实时间 |  |
| [RebirthCoin](./src/RebirthCoin/README.md) | 复活币 |  |
| [RecipesBrowser](./src/RecipesBrowser/README.md) | 合成表 |  |
| [ReFishTask](./src/ReFishTask/README.md) | 自动刷新渔夫任务 |  |
| [RegionView](./src/RegionView/README.md) | 显示区域边界 |  |
| [Respawn](./src/Respawn/README.md) | 原地复活 |  |
| [RestInventory](./src/RestInventory/README.md) | 提供 REST 查询背包接口 |  |
| [ReverseWorld](./src/ReverseWorld/README.md) | 世界反转和放置地雷 |  |
| [RolesModifying](./src/RolesModifying/README.md) | 修改玩家背包 |  |
| [Sandstorm](./src/Sandstorm/README.md) | 切换沙尘暴 |  |
| [ServerTools](./src/ServerTools/README.md) | 服务器管理工具 | [LazyAPI](./src/LazyAPI/README.md) [linq2db]() |
| [SessionSentinel](./src/SessionSentinel/README.md) | 处理长时间不发送数据包的玩家 |  |
| [ShortCommand](./src/ShortCommand/README.md) | 简短指令 |  |
| [ShowArmors](./src/ShowArmors/README.md) | 展示装备栏 |  |
| [SignInSign](./src/SignInSign/README.md) | 告示牌登录插件 |  |
| [SimultaneousUseFix](./src/SimultaneousUseFix/README.md) | 解决卡双锤卡星旋机枪之类的问题 | [Chireiden.TShock.Omni](https://github.com/sgkoishi/yaaiomni/blob/master/README.md) |
| [SmartRegions](./src/SmartRegions/README.md) | 智能区域 |  |
| [SpawnInfra](./src/SpawnInfra/README.md) | 生成基础建设 |  |
| [SpclPerm](./src/SpclPerm/README.md) | 服主特权 |  |
| [StatusTextManager](./src/StatusTextManager/README.md) | PC端模板文本管理插件 |  |
| [SurfaceBlock](./src/SurfaceBlock/README.md) | 禁地表弹幕插件 | [LazyAPI](./src/LazyAPI/README.md) |
| [SurvivalCrisis](./src/SurvivalCrisis/README.md) | 类among us小游戏 |  |
| [SwitchCommands](./src/SwitchCommands/README.md) | 区域执行指令 |  |
| [TeleportRequest](./src/TeleportRequest/README.md) | 传送请求 |  |
| [TimeRate](./src/TimeRate/README.md) | 时间加速插件 |  |
| [TimerKeeper](./src/TimerKeeper/README.md) | 保存计时器状态 |  |
| [TownNPCHomes](./src/TownNPCHomes/README.md) | NPC 快速回家 |  |
| [TShockConfigMultiLang](./src/TShockConfigMultiLang/README.md) | TShock config文件语言本土化 | [LazyAPI](./src/LazyAPI/README.md) |
| [UnseenInventory](./src/UnseenInventory/README.md) | 允许服务器端生成“无法获取”的物品 |  |
| [VBY.Common](https://github.com/UnrealMultiple/MyPlugin/blob/master/docs/VBY.Common.md) | VBY插件的基础库 |  |
| [VBY.GameContentModify](https://github.com/UnrealMultiple/MyPlugin/blob/master/docs/VBY.GameContentModify.md) | 对一些游戏内容的可自定义修改 (超强) | [VBY.Common](https://github.com/UnrealMultiple/MyPlugin/blob/master/docs/VBY.Common.md) |
| [VBY.OtherCommand](https://github.com/UnrealMultiple/MyPlugin/blob/master/docs/VBY.OtherCommand.md) | 提供一些其他的辅助命令 | [VBY.Common](https://github.com/UnrealMultiple/MyPlugin/blob/master/docs/VBY.Common.md) |
| [VBY.PluginLoader](https://github.com/UnrealMultiple/MyPlugin/blob/master/docs/VBY.PluginLoader.md) | 一个允许热重载的插件加载器 |  |
| [VBY.PluginLoaderAutoReload](https://github.com/UnrealMultiple/MyPlugin/blob/master/docs/VBY.PluginLoaderAutoReload.md) | VBY.PluginLoader的扩展, 自动热重载插件 | [VBY.PluginLoader](https://github.com/UnrealMultiple/MyPlugin/blob/master/docs/VBY.PluginLoader.md) |
| [VeinMiner](./src/VeinMiner/README.md) | 连锁挖矿 |  |
| [VotePlus](./src/VotePlus/README.md) | 多功能投票 |  |
| [WeaponPlus](./src/WeaponPlus/README.md) | 武器强化钱币版 |  |
| [WikiLangPackLoader](./src/WikiLangPackLoader/README.md) | 为服务器加载 Wiki 语言包 |  |
| [WorldModify](https://github.com/UnrealMultiple/TShockWorldModify/blob/master/README.md) | 世界编辑器,可以修改大部分的世界参数 |  |
| [ZHIPlayerManager](./src/ZHIPlayerManager/README.md) | zhi的玩家管理插件 |  |

</Details>

## 翻译

- 如果你想要参与翻译工作，欢迎访问我们的 [Crowdin](https://zh.crowdin.com/project/tshock-chinese-plugin) 链接

## 友情链接

- [TShock 插件开发文档](https://github.com/ACaiCat/TShockPluginDocument)
- [Tshock 相关内容大全导航](https://github.com/UnrealMultiple/Tshock-nav)
