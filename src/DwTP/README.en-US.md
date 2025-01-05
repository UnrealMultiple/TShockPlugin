# DwTP 定位传送插件

- Authors: 羽学
- Source: 无
- This is a Tshock server plugin, mainly used for：

## Commands

| Syntax  |    别名   |       Permission       |                                                    Description                                                    |
| ------- | :-----: | :--------------------: | :---------------------------------------------------------------------------------------------------------------: |
| /dw     |   /定位   | dw.use |                                                    Command menu                                                   |
| /dw hb  |  /定位 花苞 | dw.use |                                                     传送到世纪之花花苞                                                     |
| /dw dl  |  /定位 地牢 | dw.use |                               Teleport to the Guide or Cultist Archer in the dungeon                              |
| /dw sm  |  /定位 神庙 | dw.use |                                Teleport to the temple entrance or the Jungle Shrine                               |
| /dw bag | /定位 宝藏袋 | dw.use |                        Teleport to the Boss's death location (loot bag)                        |
| /dw wgh | /定位 微光湖 | dw.use | Teleport to the Glow Lake (places a gray brick above the liquid if it is empty the first time) |

## Configuration

```json
None
```

## 更新日志

```
v1.0.0
使用/dw指令传送到定位地标
定位花苞：只在世界图格上存在花苞时才会传送
定位地牢：只在世界存在地牢老人或邪教徒弓箭手时才会传送
定位神庙：神庙门没打开时传送到门前，打开后传送到丛林蜥蜴祭坛
定位宝藏袋：只在有BOSS死亡后才会传送到其死亡地点
定位微光湖：传送到微光液体，第一次会判断液体上方没有方块时放置灰砖
```

## FeedBack

- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
