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
> Configuration file location: tshock/Dummy.zh-CN.json
```json5
{
  "dummy": [
    {
      "登陆密码": "", //login password //leave blank not to log in
      "皮肤": 0, //skin
      "头发": 0, //hair
      "名字": "熙恩", //name
      "染发": 0, //hair dye
      "隐藏设置": 0, //hidden setting
      "头发颜色": { //hair color
        "R": 239,
        "G": 211,
        "B": 211
      },
      "皮肤颜色": { //skin color
        "R": 239,
        "G": 202,
        "B": 202
      },
      "眼睛颜色": { //eye color
        "R": 51,
        "G": 9,
        "B": 9
      },
      "上衣颜色": { //top color
        "R": 239,
        "G": 211,
        "B": 211
      },
      "内衣颜色": { //underwear color
        "R": 239,
        "G": 211,
        "B": 211
      },
      "裤子颜色": { //pants color
        "R": 239,
        "G": 211,
        "B": 211
      },
      "鞋子颜色": { //shoe color
        "R": 239,
        "G": 211,
        "B": 211
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
