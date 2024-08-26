# HouseRegion enclosure plug-in

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: GK
- Source: None
- Better enclosure plug-in

## Update log

```
暂无
```

## instruction

|grammar|another name|limit of authority|explain|
| ----------------------------- |:--:|:-----------------------:|:------------:|
|/house set 1|without|house.use|Tap the upper left corner|
|/house set 2|without|house.use|Tap the lower right corner|
|/house clear|without|house.use|Reset temporary tapping point|
|/house allow [player] [house]|without|`house.use`  `house.admin`|Add owner|
|/house disallow [player] [house]|without|`house.use`  `house.admin`|Remove owner|
|/house adduser [player] [house]|without|`house.use`  `house.admin`|Add users|
|/house deluser [player] [house]|without|`house.use`  `house.admin`|Delete user|
|/house delete [house name]|without|`house.use`  `house.admin`|Delete a house|
|/house list [page number]|without|`house.use`  `house.admin`|View house list|
|/house redefine [house name]|without|`house.use`  `house.admin`|Redefine the house|
|/house info [house name]|without|`house.use`  `house.admin`|Housing information|
|/门锁[屋名]|无|`house.use`  `house.admin`|房屋锁|

## 配置
配置文件位置:tshock/HouseRegion.json
> 豪斯区域. json

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

## 反馈

- 共同维护的插件库:https://github . com/controller destiny/TShockPlugin
- 国内社区trhub.cn或TShock官方群等