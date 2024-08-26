# PacketsStop packet interception

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: feather science
- - 出处: [github](https://github.com/1242509682/PacketsStop/) 
- The source code of the plug-in comes from the tourist restriction plug-in, and its data packet processing method is made into an independent functional plug-in
- This is a Tshock server plug-in mainly used for:
- Use the command to turn on blocking player packets.
- You must give the default group a permission before using it [No Interception].
- Then, by modifying the configuration file, the package name is added using the /reload overload.
- Enter [/intercept add name] to assign players to a newly created [LJ Group].
- Blocked most of the available permissions and intercepted their packets.
## Update log

```
- 1.0.1
- 完善卸载函数
- 2.0
- 修复数据包拦截插件的GetPacket逻辑：原对配置文件内的数据包名以外的全部拦截问题已修复
- 1.0
- 将少司命的游客限制插件处理数据包方法，做成了一个独立的功能插件。
```
## instruction

|grammar|limit of authority|explain|
| -------------- |:-----------------:|:------:|
|/intercept|intercept|Open function|
|/block add player name|intercept|Add intercept specified players and assign them to an automatically created LJ group.|
|/block del player name|intercept|Remove and intercept the specified player and assign it back to the default group.|
|without|Interception free|Not affected by plug-in packet interception|

## deploy
> Configuration file location: tshock/ packet interception.json.
```json
{
   "数据包名可查看": "https://github.com/Pryaxis/TSAPI/blob/general-devel/TerrariaServerAPI/TerrariaApi.Server/PacketTypes.cs",
   "插件指令与权限名": "拦截",
   "第一次使用输这个": "/group addperm default 免拦截",
   "拦截的数据包名": [
     "ConnectRequest",
     "Disconnect",
     "ContinueConnecting",
     "ContinueConnecting2",
     "PlayerInfo",
     "PlayerSlot",
     "TileGetSection",
     "PlayerSpawn",
     "ProjectileNew",
     "ProjectileDestroy",
     "SyncProjectileTrackers" 
  ]
}
```
## feedback
- Give priority to issued-> jointly maintained plug-in library: https://github.com/Controllerdestiny/TShockPlugin.
- Second priority: TShock official group: 816771079
- You can't see it with a high probability, but you can: domestic communities trhub.cn, bbstr.net, tr. monika.love.