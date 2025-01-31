# Ezperm 便捷权限

- 作者: 大豆子,肝帝熙恩优化1449
- 出处: TShock中文官方群
- 一个指令帮助小白给初始服务器添加缺失的权限，还可以批量添删权限
- 其实你也可以直接/group addperm 组名字 权限1 权限2 权限3

## 指令

| 语法                |      权限       |  说明   |
|-------------------|:-------------:|:-----:|
| /inperms 或 /批量改权限 | inperms.admin | 批量改权限 |

## 配置
> 配置文件位置：tshock/ezperm.json
```json5
{
  "组列表": [
    {
      "组名字": "default",
      "父组": "guest",
      "添加的权限": [
        "tshock.world.movenpc",
        "tshock.world.time.usesundial",
        "tshock.tp.pylon",
        "tshock.tp.demonconch",
        "tshock.tp.magicconch",
        "tshock.tp.tppotion",
        "tshock.tp.rod",
        "tshock.tp.wormhole",
        "tshock.npc.startdd2",
        "tshock.npc.spawnpets",
        "tshock.npc.summonboss",
        "tshock.npc.startinvasion",
        "tshock.npc.hurttown"
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
v1.2.9
使用lazyapi，添加父组预设
v1.2.8
添加 GetString
v1.2.7
添加权限：
v1.2.4
默认数据添加召唤城镇宠物的权限
v1.2.3
把这个抽象代码改了，支持没有该组自动创建
v1.2.2
加了几个权限
v1.2.1
优化代码，完善卸载函数
```

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
