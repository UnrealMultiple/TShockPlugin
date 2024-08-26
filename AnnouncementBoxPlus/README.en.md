# AnnouncementBoxPlus

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: Cai
- Source: this warehouse
- Broadcast box strengthening and management control

## introduce

1. Broadcast box interpolation placeholder and formatting
2. You can add the right to edit the broadcast box.
4. The broadcast box can be switched on and off
5. You can set the effective range of the broadcast box (in pixels).


## deploy
> Configuration file location: tshock/announcementboxplus.json.
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
## order

|Command name|explain|
| -------------- |:-----------------:
|/reload|Overloaded configuration file
## Broadcast box format placeholder

|placeholder|explain|
| -------------- |:-----------------:
|% Player Group Name%|Name of the player group
|% player name%|Player name
|% current time%|Current real time
|% content%|Original content of broadcast box

## Broadcast box content placeholder

|placeholder|explain|
| -------------- |:-----------------:
|% Player Group Name%|Name of the player group
|% player name%|Player name
|% current time%|Current real time
|% The number of people currently online on the server%|Get the number of online users of the current server.
|% Fisherman Task Fish Name%|Fisherman's task fish name
|% fisherman task fish ID%|Fisherman task fish ID
|% fisherman's mission fish location%|Fisherman's mission fish location
|% map name%|Current map name
|% Player HP%|Player blood volume
|% Player Magic%|Player magic
|% Maximum player's HP%|Maximum player blood volume
|% Player Magic Maximum%|Maximum player magic
|% Player Lucky Value%|Player's lucky value
|% player x coordinate%|X coordinate of player graph
|% player y coordinate%|Y coordinate of player graph lattice
|% player's area%|Region where the player is located
|% Player Death Status%|Player death state
|% current environment%|Player's current environment
|% Server Online List%|Online list of servers (/who)
|% fisherman's task fish completed%|Does the player complete the fisherman's task fish?


## Update log

```
v1.0.1 完善卸载函数
```

## feedback
- Give priority to issued-> jointly maintained plug-in library: https://github.com/Controllerdestiny/TShockPlugin.
- Second priority: TShock official group: 816771079
- You can't see it with a high probability, but you can: domestic communities trhub.cn, bbstr.net, tr. monika.love.