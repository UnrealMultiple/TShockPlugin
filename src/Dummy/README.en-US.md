# Dummy dummy

- author: 少司命
- Source: 此仓库
- This plugin provides a fake person to enter your server, who is essentially a player.


## Description
- Adding too many dummies to the server may cause server lag
- This plugin is in its infancy and waiting to be expanded
- Plugins are at risk of crashing


## Instruction
| Command           |        Permissions         |   Description   |
| -------------- | :-----------------: | :------: |
| /dummy list | dummy.client.use   | view dummy|
| /dummy remove [index] | dummy.client.use   | remove dummy|
| /dummy reconnect [index] | dummy.client.use   | reconnect |

## Configuration
> Configuration file location: tshock/Dummy.en-US.json
```json5
{
  "Dummys": [
    {
      "LoginPassword": "", //leave blank not to log in
      "SkinVariant": 0,
      "Hair": 0,
      "Name": "熙恩",
      "HairDye": 0,
      "HideMisc": 0,
      "HairColor": {
        "R": 0,
        "G": 0,
        "B": 0
      },
      "SkinClolor": {
        "R": 0,
        "G": 0,
        "B": 0
      },
      "EyeColor": {
        "R": 0,
        "G": 0,
        "B": 0
      },
      "ShirtColor": {
        "R": 0,
        "G": 0,
        "B": 0
      },
      "UnderShirtColor": {
        "R": 0,
        "G": 0,
        "B": 0
      },
      "PantColor": {
        "R": 0,
        "G": 0,
        "B": 0
      },
      "ShoeColor": {
        "R": 0,
        "G": 0,
        "B": 0
      }
    }
  ]
}
```

## Change log

```
none
```

## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
