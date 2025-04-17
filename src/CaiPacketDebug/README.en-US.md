# CaiPacketDebug Packet debugging

- Author: Cai
- Source: This repository
- This plug-in is used to view the data packet content of the client and server side

> [!NOTE]  
> Need to install the preset: TrProtocol


## Instruction

| Command       |         Permissions         |    Description     |
|----------|:------------------:|:---------:|
| /cpd     | CaiPacketDebug.Use | Check debug enabled status  |
| /cpd stc | CaiPacketDebug.Use | S->C packet debugging |
| /cpd cts | CaiPacketDebug.Use | C->S packet debugging |


## Configuration
> 配置文件位置：tshock/CaiPacketDebug..json
```json5   
{
  "C->S": { //Data packets sent by the client to the server
    "自启动": false, //Self-start //Automatically enable debugging when opening the server
    "白名单模式": false, //Whitelist mode //Show only the data packets of the whitelist list
    "白名单模式数据包": [ //Whitelist mode packets
      1,
      2,
      3
    ],
    "排除数据包": [ //Exclude the following data packets
      114,
      514
    ]
  },
  "S->C": { //Data packets sent by the server to the client
    "自启动": false, //Self-start
    "白名单模式": false, //Whitelist mode
    "白名单模式数据包": [ //Whitelist mode packets
      1,
      2,
      3
    ],
    "排除数据包": [ //Exclude data packets
      23
    ]
  }
}
```

## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
