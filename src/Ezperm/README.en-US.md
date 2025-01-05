# Ezperm 便捷权限

- Author: 大豆子, 肝帝熙恩优化1449
- Source: Official Chinese TShock Group
- A command to help beginners add missing permissions to the initial server and also allows for batch addition and deletion of permissions.
- Alternatively, you can directly use /group addperm groupname permission1 permission2 permission3

## Commands

| Syntax             |           Permission          |        Description       |
| ------------------ | :---------------------------: | :----------------------: |
| /inperms or /批量改权限 | inperms.admin | Batch modify permissions |

## Configuration

> Configuration file location: tshock/ezperm.json

```json5
{
  "Groups": [
    {
      "组名字": "default",
      "添加的权限": [
        "tshock.world.movenpc",
        "tshock.tp.pylon",
        "tshock.tp.demonconch",
        "tshock.tp.magicconch",
        "tshock.tp.tppotion",
        "tshock.tp.rod",
        "tshock.npc.startdd2",
        "tshock.tp.wormhole",
        "tshock.npc.summonboss",
        "tshock.npc.startinvasion",
        "tshock.npc.spawnpets",
        "tshock.world.time.usesundial"
      ],
      "删除的权限": [
        "tshock.admin"
      ]
    }
  ]
}
```

## 更新日志

```
v1.2.4
默认数据添加召唤城镇宠物的权限
v1.2.3
把这个抽象代码改了，支持没有该组自动创建
v1.2.2
加了几个权限
v1.2.1
优化代码，完善卸载函数
```

## Feedback

- Preferred: Submit an issue -> Commonly maintained plugin repository: https://github.com/UnrealMultiple/TShockPlugin
- Secondary option: Official TShock QQ Group: 816771079
- Least likely to be seen but still possible: Domestic Terraria community forums trhub.cn, bbstr.net, tr.monika.love
