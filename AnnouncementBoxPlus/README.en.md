# Announdmentboxplus

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: CAI
- Source: This warehouse
- Broadcast box enhancement and management control

## introduce

1. Insert bits and formatting in the broadcast box
2. You can add permissions of editing broadcast boxes
4. Can switch to broadcast boxes
5. You can set the effective range of broadcast boxes (unit: pixel)


## Configuration
> Configuration file location: TSHOCK/AnnoundcementBoxplus.json
```json
{
   "禁用广播盒": false,
   "广播内容仅触发者可见": true,
   "广播范围(像素)(0为无限制)": 50,
   "启用广播盒权限(AnnouncementBoxPlus.Edit)": true,
   "启用插件广播盒发送格式": false,
   "广播盒发送格式": "%当前时间% %玩家组名% %玩家名%:%内容% #详细可查阅文档",
   "启用广播盒占位符(详细查看文档)": true
}
```
## Order

|Order name|illustrate|
| -------------- |: --------------------:
|/regoad|Heavy load configuration file
## Broadcast box format placement symbol

|Occupy|illustrate|
| -------------- |: --------------------:
|% Player group name%|The name of the player group
|% Player name%|Player name
|% Current time%|Current actual time
|%content%|Broadcast box original content

## Broadcast box content placement symbol

|Occupy|illustrate|
| -------------- |: --------------------:
|% Player group name%|The name of the player group
|% Player name%|Player name
|% Current time%|Current actual time
|%Of the current server online%%%|Get the number of online servers online
|%Fisherman task fish name%%|Fisherman task fish name
|%Fisherman task fish ID%|Fisherman task fish ID
|%Fisherman task fish location%%|Fisherman task fish location
|%Map name%|Current map name
|%Of player blood%%%|Player blood volume
|%Player Magic%|Player magic
|%Player's maximum blood volume%%%|Player's maximum blood volume
|%Player Magic Most value%%%|Player Magic Most Value
|%Player lucky value%%|Player lucky value
|%Player x coordinates%|Player graphm X coordinates
|%Player Y coordinates%|Player graphm y coordinates
|%Of the area where the player is located|Region region where the player is located
|%Player Death Status%%|Player death state
|%Current environment%|Player's current environment
|%Server online list%|Server online list (/who)
|%Fisherman task fish complete%|Whether the player completes the fisherman task fish


## Update log

```
v1.0.1 完善卸载函数
```

## feedback
- Priority to ISSUED-> Commonly maintained plug -in library: https://github.com/Controllerdestiny/tshockPlugin
- Two priority: TSHOCK official group: 816771079
- It may not be seen in a high probability, but it can also be: domestic community trhub.cn, bbstr.net, tr.monika.love