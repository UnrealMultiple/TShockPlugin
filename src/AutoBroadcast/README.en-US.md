# AutoBroadcast 

- Authors: Scavenger，Zaicon,GK 小改良，肝帝熙恩更新至1449
- Source: [github](https://github.com/Scavenger3/AutoBroadcast)
- Schedule broadcasts in the server or execute commands.


## Commands
```
None
```

## Config
> Configuration file location：tshock/AutoBroadcast.en-US.json
```json5
{
  "Broadcasts": [
    {
      "Name": "示例广播",
      "Enable": true,
      "Msg": [
        "/say Ciallo～(∠・ω< )⌒★",
        "The auto broadcast executed the server command /say Ciallo～(∠・ω< )⌒★"
      ],
      "Color": [
        255,
        234,
        115
      ],
      "Interval": 600, // This is the interval time in seconds.
      "Delay": 0, // This is the delay before the first broadcast starts.
      "Groups": [], // Define specific groups for this broadcast.
      "TriggerWords": [], // You can set chat message keywords to trigger this broadcast.
      "TriggerToWholeGroup": false // When enabled, this broadcast will be executed for all players in the 'broadcast group'.
    }
  ]
}
```

## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
