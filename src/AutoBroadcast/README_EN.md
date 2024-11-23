# AutoBroadcast 

- Authors: Scavenger，Zaicon,GK 小改良，肝帝熙恩更新至1449
- Source: [github](https://github.com/Scavenger3/AutoBroadcast)
- Schedule broadcasts in the server or execute commands.


## Commands
```
None
```

## Config
> Configuration file location：tshock/AutoBroadcast.json
```json
{
  "广播列表": [ 
    {
      "广播名称": "实例广播",
      "启用": false,
      "广播消息": [
        "这是一条广播",
        "每五分钟执行一次",
        "可以执行命令",
        "/time noon"
      ],
      "RGB颜色": [255.0, 0.0, 0.0],
      "时间间隔": 300,
      "延迟执行": 60,
      "广播组": [],
      "触发词语": [], //可以设置聊天消息关键词触发此广播
      "触发整个组": false //开启后将会对'广播组'内所有玩家执行此广播
    }
  ]
}

// You can refer to this:
{
  "Broadcasts": [
    {
      "Name": "Example Broadcast",
      "Enabled": false,
      "Messages": [
        "This is an example broadcast",
        "It will broadcast every 5 minutes",
        "Broadcasts can also execute commands",
        "/time noon"
      ],
      "ColorRGB": [255.0, 0.0, 0.0],
      "Interval": 300,
      "StartDelay": 60,
      "Groups"": [],
      "TriggerWords": [], // You can set chat message keywords to trigger this broadcast.
      "TriggerToWholeGroup": false // When enabled, this broadcast will be executed for all players in the 'broadcast group'
    }
  ]
}
```

## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
