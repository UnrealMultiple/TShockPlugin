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
        255.0,
        234.0,
        115.0
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

## 更新日志

```
v1.0.8
准备更新TS 5.2.1,修正文档，初始配置内容更改
v1.0.6
修复了初始配置文件的问题
v1.0.5
i18n 和 README_EN.md
v1.0.4
i18n预定
```

## FeedBack

- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
