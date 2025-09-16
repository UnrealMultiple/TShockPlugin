# Lagrange.XocMat BOT适配插件

- 作者: 少司命
- 仓库: 此仓库

## Lagrange.XocMat 地址

- [Lagrange.XocMat](https://github.com/UnrealMultiple/XocMat)

## 指令

```
暂无  
```

## 配置

> 配置文件位置：tshock/Lagrange.XocMat.Adapter.json

```json5
{
  "阻止未注册进入": true,
  "阻止语句": ["未注禁止进入服务器！"],
  "Socket": {
    "套字节地址": "127.0.0.1",
    "服务器名称": "玄荒", // 名称要求相同
    "端口": 6000,
    "心跳包间隔": 60000,
    "重连间隔": 5000,
    "空指令注册": [
      "购买",
      "抽"
    ],
    "验证令牌": "" //与Lagrange.XocMat配置中相同
  },
  "重置设置": {
    "删除地图": true,
    "删除日志": true,
    "执行命令": [
      "/skill reset",
      "/deal reset",
      "/礼包 重置",
      "/level reset",
      "/clearallplayersplus"
    ],
    "删除表": [
      "boss数据统计",
      "economics",
      "economicsskill",
      "learnt",
      "OnlineDuration",
      "BotOnlineDuration",
      "BotDeath",
      "onlybaniplist",
      "permabuff",
      "permabuffs",
      "regions",
      "user",
      "Death",
      "rememberedpos",
      "research",
      "stronger",
      "synctable",
      "tscharacter",
      "users",
      "warps",
      "weapons",
      "使用日志"
    ]
  }
}
```

## 更新日志

### v1.0.0.1
- 阻止语句改string[]
### v1.0.0.7
- 防止没有Active的情况下转发消息

## 反馈

- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love

