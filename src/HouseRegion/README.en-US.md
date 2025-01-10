# HouseRegion house protection plugin

- author: GK
- Source: None
- House Protection Plugin

## instruction

| Command                        | Alias |            Permissions             |   Description    |
|---------------------------|:--:|:-------------------------:|:-------:|
| /house set 1              | none  |         house.use         |  Tap the top left corner  |
| /house set 2              | none  |         house.use         |  Tap the bottom right corner  |
| /house clear              | none  |         house.use         | Reset temporary hit points |
| /house allow [player] [house]    | none  | `house.use` `house.admin` |  Add owner  |
| /house disallow [player] [house] | none  | `house.use` `house.admin` |  Remove owner  |
| /house adduser [player] [house]  | none  | `house.use` `house.admin` |  Add user  |
| /house deluser [player] [house]  | none  | `house.use` `house.admin` |  Delete user  |
| /house delete [house name]        | none  | `house.use` `house.admin` |  Delete house   |
| /house list [page number]          | none  | `house.use` `house.admin` | View house define list  |
| /house redefine [house name]      | none  | `house.use` `house.admin` | Redefining the house  |
| /house info [house name]          | none  | `house.use` `house.admin` |  Housing information   |
| /house lock [house name]          | none  | `house.use` `house.admin` |   House lock   |

## Configuration
	Configuration file location: tshock/HouseRegion.json
> HouseRegion.json

```json
{
  "进出房屋提示": true, //House Entry/Exit Notification
  "房屋嘴大大小": 1000, //Maximum House Size
  "房屋最小宽度": 30, //Minimum House Width
  "房屋最小高度": 30, //Minimum House Height
  "房屋最大数量": 1, //Maximum House
  "禁止锁房屋": false, //Disabled House Locking
  "保护宝石锁": false, //Protect Gems Lock
  "始终保护箱子": false, //Always Protect Chest
  "冻结警告破坏者": true, //Freeze Warning For Griefer
  "禁止分享所有者7": false, //Disable Sharing Owner
  "禁止分享使用者": false, //Disable Sharing Users
  "禁止所有者修改使用者": true //Prevent Owners From Modifying Users
}
```

## Change log

```
v1.0.0.4
Improve the uninstall function
```

## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
