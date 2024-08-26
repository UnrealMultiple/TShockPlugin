# PerPlayerLoot player booty separate box

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: Codian, Gan Di Xi En Han Hua 1449
- - 出处: [github](https://github.com/xxcodianxx/PerPlayerLoot) 
- This plug-in was originally designed to solve the problem of insufficient map resources. When the map loaded with this plug-in is started for the first time, each trophy box will be given an inventory independent of other players, ensuring that each player can get exclusive trophies when opening the box, even if others have opened the same box before.
- In order to avoid mistaken identification of the box placed by the player, this plug-in should be installed at the beginning of server startup. If installed during the game, all existing boxes will be regarded as generated trophy boxes, and their inventory will be copied for each player.
- Box database: in the tshock folder directory `perplayerloot.sqlite` 

## Update log

```
暂无
```

## instruction

|grammar|limit of authority|explain|
| -------------- |:-----------------:|:------:|
|/ppltoggle|perplayerloot.toggle|Packet hook function of global switching plug-in|
- **warn**: Using this command may cause the Main.chest array to be out of sync with the internal state of the plug-in, which is not recommended for normal game environment. When this command is enabled, the box placed by the player will become a trophy box, showing the real inventory instead of personalized inventory. Debugging purposes only

## deploy

```
暂无
```

## feedback
- Give priority to issued-> jointly maintained plug-in library: https://github.com/Controllerdestiny/TShockPlugin.
- Second priority: TShock official group: 816771079
- You can't see it with a high probability, but you can: domestic communities trhub.cn, bbstr.net, tr. monika.love.