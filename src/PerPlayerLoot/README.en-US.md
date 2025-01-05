# PerPlayerLoot

- Authors: Codian,肝帝熙恩汉化1449
- Source: [github](https://github.com/xxcodianxx/PerPlayerLoot)
- 本插件设计之初是用于解决地图资源不足问题，装载该插件的地图初次启动，会赋予每个战利品箱一个独立于其他玩家的库存，确保每位玩家在打开箱子时都能获得专属的战利品，即便其他人在之前已开过同一箱子。
- 为避免玩家放置的箱子被误识别，应在服务器开启之初即安装本插件。若在游戏过程中安装，所有现有箱子将被视为生成的战利品箱，其库存将为每位玩家复制。
- 箱子数据库：在tshock文件夹目录中的`perplayerloot.sqlite`

## Commands

| 语法         |              Permission              |                    说明                   |
| ---------- | :----------------------------------: | :-------------------------------------: |
| /ppltoggle | perplayerloot.toggle | Toggle the plugin packet hooks globally |

- **警告**：使用此命令可能导致 `Main.chest` 数组与插件内部状态不同步，不推荐在正常游戏环境中使用。启用此命令后，玩家放置的箱子将成为战利品箱，显示真实库存而非个人化库存。仅限调试目的

## Config

```
None
```

## 更新日志

```
v2.0.3
完成i18n和README_EN.md
v2.0.2
修复：初始箱子都保存的问题
v2.0.1
完善卸载函数
```

## FeedBack

- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
