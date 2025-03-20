# PacketsStop Packet interception

- author: 羽学
- source: [github](https://github.com/1242509682/PacketsStop/)
- The plugin source code comes from Shaosiming's visitor restriction plug-in, and its data packet processing method is made into an independent functional plugin.
- This is a Tshock server plugin mainly used for:
- Use the instruction to open the intercepting player packet  
- Before use, you must first give the default group permission【免拦截】 
- Then modify the configuration file to add the data package name and use /reload to reload  
- Enter 【/拦截 add name】 to assign players to a new【LJ Group】
- Blocks most available permissions and intercepts their packets.  

## Instruction

| Command          | Permissions  |           Description           |
|-------------|:---:|:----------------------:|
| /packetstop         | packetstop.use  |          Open function          |
| /packetstop add player name | packetstop.use  | Add interception of specified players and assign them to an automatically created LJ group |
| /packetstop del Player name | packetstop.use  |  Remove and intercept the designated player and allocate back to the default group  |
| none           | packetstop.notstop |      Not affected by plugin packet interception       |

## Configuration
> Configuration file position: tshock/数据包拦截.json
```json5
{
  "数据包名可查看": "https://github.com/Pryaxis/TSAPI/blob/general-devel/TerrariaServerAPI/TerrariaApi.Server/PacketTypes.cs", //The data package name can be viewed
  "插件指令与权限名": "拦截", //Plugin instructions and permission names
  "第一次使用输这个": "/group addperm default 免拦截", //This is the first time you use it
  "拦截的数据包名": [ //Intercepted packet name
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

## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
