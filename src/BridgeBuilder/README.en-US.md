# BridgeBuilder

- Authors: Soofa，肝帝熙恩
- Source: [github](https://github.com/Soof4/BridgeBuilder)
- Quickly pave in the parallel direction facing the player, stopping when encountering a block, and stopping when a wall encounters another wall.
- Customizable bridge blocks are available, with default configurations including: platforms, planter boxes, and team blocks.
- 可以竖直放了
- 科普：<>内是必填，[]内是选填

## Commands

| 语法                                                                                                                                         |              Permission              |               说明               |
| ------------------------------------------------------------------------------------------------------------------------------------------ | :----------------------------------: | :----------------------------: |
| /bridge [up/down] or /桥来 [up/down] | bridgebuilder.bridge | Quick bridge placement command |

## Config

> Configuration file location：tshock/BridgeBuilder.en-US.json

```json
{
  "TilesID": [ // Allow quick placement of tile ID
    19,
    380,
    427,
    435,
    436,
    437,
    438,
    439
  ],
  "WallsID": [], // Allow quick placement of wall ID
  "MaxTile": 256 // The maximum number of tiles that can be placed at once
}
```

## 更新日志

```
1.1.2
准备更新TS 5.2.1,修正文档，初始配置内容更改
1.1.0
i18n和README_EN.md
1.0.9
i18n预备
1.0.8
补全卸载函数
1.0.7
修复 地图边缘搭桥导致崩溃和竖直方向距离没有正常被限制（目前不知道为什么水平少两块，竖直少一块）

1.0.6
添加竖直方向

1.0.5
修改添加了墙壁逻辑，填墙壁id就好了
```

## FeedBack

- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
