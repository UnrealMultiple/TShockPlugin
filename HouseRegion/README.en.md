# Houseregg circular plug -in

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: GK
- Source: None
- Better circle plug -in

## Update log

```
暂无
```

## instruction

|grammar|Alias|Authority|illustrate|
| ----------------------------- |:-:|: -----------------------:|: --------------:|
|/House set 1|none|House.use|Tap the upper left corner|
|/House set 2|none|House.use|Tap the lower right corner|
|/House Clear|none|House.use|Reset the temporary knocking point|
|/House Allow [Player] [House]|none|`house.use`  `house.admin`|Add|
|/House disallow [Player] [House]|none|`house.use`  `house.admin`|Eliminator|
|/House Adduser [Player] [House]|none|`house.use`  `house.admin`|Add user|
|/House Deluser [Player] [House]|none|`house.use`  `house.admin`|Delete user|
|/House Delete [House Name]|none|`house.use`  `house.admin`|Delete a house|
|/House list [page number]|none|`house.use`  `house.admin`|View house list|
|/House Redefine [House Name]|none|`house.use`  `house.admin`|Redefine the house|
|/House Info [House Name]|none|`house.use`  `house.admin`|House information|
|/House Lock [House name]|none|`house.use`  `house.admin`|House lock|

## Configuration
Configuration file location: TSHOCK/HOUSEREGION.JSON
> Houseregg.json

```json
{
   "进出房屋提示": true,
   "房屋嘴大大小": 1000,
   "房屋最小宽度": 30,
   "房屋最小高度": 30,
   "房屋最大数量": 1,
   "禁止锁房屋": false,
   "保护宝石锁": false,
   "始终保护箱子": false,
   "冻结警告破坏者": true,
   "禁止分享所有者7": false,
   "禁止分享使用者": false,
   "禁止所有者修改使用者": true
}
```

## feedback

- Co -maintained plug -in library: https://github.com/Controllerdestiny/tshockplugin
- The official group of Trhub.cn or Tshock in the domestic community