# Lagrange.XocMat BOT adapter plug-in

- Author: 少司命
- Source: This repository

## Lagrange.XocMat address

- [Lagrange.XocMat](https://github.com/UnrealMultiple/XocMat)

## Instruction

```
None  
```

## Configuration

> Configuration file location: tshock/Lagrange.XocMat.Adapter.json

```json5
{
  "阻止未注册进入": true, //Ban unregistered player join
  "阻止语句": ["未注禁止进入服务器！"], //Ban statement
  "Socket": {
    "套字节地址": "127.0.0.1", //Set address
    "服务器名称": "玄荒", //Server name //The name requirements are the same
    "端口": 6000, //Port
    "心跳包间隔": 60000, //Heartbeat compartment interval
    "重连间隔": 5000, //Reconnect interval
    "空指令注册": [ //Register for empty command
      "购买",
      "抽"
    ],
    "验证令牌": "" //Verify token //Same as in Lagrange.XocMat configuration
  },
  "重置设置": { //Reset settings
    "删除地图": true, //Delete map
    "删除日志": true, //Delete logs
    "执行命令": [ //Execute command
      "/skill reset",
      "/deal reset",
      "/礼包 重置",
      "/level reset",
      "/clearallplayersplus"
    ],
    "删除表": [ //Delete list
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

## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
