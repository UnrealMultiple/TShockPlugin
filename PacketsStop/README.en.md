# PacketsStop packet interception

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: Yu Xue
- - 出处: [github](https://github.com/1242509682/PacketsStop/) 
- The source code of the plug -in is to the tourists who are in Shaoshi's life to restrict the plug -in, and make the data packet method into an independent functional plug -in
- This is a TSHOCK server plug -in mainly used for:
- Use the instruction to open the intercepting player packet
- Before use, you must give the Default group a permissions [free interception]
- Then modify the configuration file to add the data packet name to use/RELOAD overload
- Enter the [/intercept ADD name] to allocate the player to a new [LJ group]
- Most of the availability limit is blocked and intercepted its data packets.
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

|grammar|Authority|illustrate|
| -------------- |: --------------------:|: -------:|
|/Block|Intercept|Open function|
|/Intercept ADD player name|Intercept|Add an intercept the designated player and allocate it to an automatic LJ group|
|/Intercept Del player name|Intercept|Remove and intercept the designated player and allocate back to the DEFAULT group|
|none|Exempt|Not affected by the interception of the plug -in packet|

## Configuration
> Configuration file location: TSHOCK/Data Pack Intercept.json
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
- Priority to ISSUED-> Commonly maintained plug -in library: https://github.com/Controllerdestiny/tshockPlugin
- Two priority: TSHOCK official group: 816771079
- It may not be seen in a high probability, but it can also be: domestic community trhub.cn, bbstr.net, tr.monika.love