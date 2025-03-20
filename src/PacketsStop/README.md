# PacketsStop 数据包拦截

- 作者: 羽学
- 出处: [github](https://github.com/1242509682/PacketsStop/)
- 插件源码来于少司命的游客限制插件，将其处理数据包方法做成了一个独立的功能插件
- 这是一个Tshock服务器插件主要用于： 
- 使用指令开启拦截玩家数据包  
- 使用前必须先给default组一个权限【免拦截】  
- 接着通过修改配置文件添加数据包名使用/reload重载  
- 输入【/拦截 add 名字】将玩家分配到一个新建的【LJ组】  
- 封锁了大部分可用权限并对其数据包拦截。  

## 指令

| 语法          | 权限  |           说明           |
|-------------|:---:|:----------------------:|
| /拦截 或 packetstop       | packetstop.use  |          开启功能          |
| /拦截 add 玩家名 | packetstop.use  | 添加拦截指定玩家并分配到一个自动创建的LJ组 |
| /拦截 del 玩家名 | packetstop.use  |  移除拦截指定玩家并分配回default组  |
| 无           | packetstop.notstop |      不受插件数据包拦截影响       |

## 配置
> 配置文件位置：tshock/数据包拦截.json
```json5
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

## 更新日志

```
- 1.0.5
- 添加英文命令，修改权限
- 1.0.1
- 完善卸载函数
- 2.0
- 修复数据包拦截插件的GetPacket逻辑：原对配置文件内的数据包名以外的全部拦截问题已修复
- 1.0
- 将少司命的游客限制插件处理数据包方法，做成了一个独立的功能插件。
```

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
