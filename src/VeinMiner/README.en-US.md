# VeinMiner

- Authors: Megghy|YSpoof|Maxthegreat99|肝帝熙恩|Cai
- Source: [github](https://github.com/Maxthegreat99/TSHockVeinMiner)
- 连锁挖矿，字面意思
- 可以连锁挖一堆矿然后爆指定物品

> [!IMPORTANT]
> To enable vein mining, you need the `veinminer` permission.
> Authorization command: `/group addperm default veinminer` (default is the default group, you can replace it with the group you need)

## Commands

| 语法                                                                 | Permission |                    说明                   |
| ------------------------------------------------------------------ | :--------: | :-------------------------------------: |
| /vm                                                                |  veinminer |            Toggle vein mining           |
| /vm [Any agrs] |  veinminer | Toggle vein mining notification message |

## Config

> Configuration file location：tshock/VeinMiner.json

```json5
{
  "启用": true, //Enable
  "广播": true, //Broadcast
  "放入背包": true, //Put ores into player's inventory
  "矿石物块ID": [  //TileID which will be mined by VeinMiner
    7,
    166,
    6,
    167,
    9,
    168,
    8
  ],
  "忽略挖掘表面方块ID": [    // When these items are above the ore, the ore will not be mined.
    21,                 //Exchange rules
    26,          
    88           
  ],
  "奖励规则": [ 
    {
      "仅给予物品": false, //Only give item
      "最小尺寸": 0, //Min size
      "矿石物块ID": 0, //Tile ID
      "奖励物品": null //Item
    }
  ]
}
```

### Example

```json5
{
  "启用": true, //Enable
  "广播": true, //Broadcast
  "放入背包": true, //Put ores into player's inventory
  "矿石物块ID": [ //TileID which will be mined by VeinMiner
    7,
    166,
    6,
    167,
    9,
    168
  ],
  "忽略挖掘表面方块ID": [
    21,
    26,
    88
  ],
  "奖励规则": [ //Exchange rules
    {
      "仅给予物品": true, //Item
      "最小尺寸": 10,  //Min size
      "矿石物块ID": 168, //Tile ID
      "奖励物品": {
        "666": 1, //"ItemID": stack
        "669": 1
      }
    },
    {
      "仅给予物品": true, 
      "最小尺寸": 10,
      "矿石物块ID": 8,
      "奖励物品": {
        "662": 5,
        "219": 1
      }
    }
  ]
}
```

## 更新日志

```
v1.6.0.6
修复：背包满的时候去挖达到可获得奖励数量的矿，只会挖掉一个矿，但却会给予一个奖励物品，然后就可以用这个刷奖励
完善消息提示逻辑

v1.6.0.5
修复刷矿，添加英文翻译

v1.6.0.4
完善卸载函数

v1.6.0.3
添加配置，当矿石上方有指定方块时，不会触发连锁挖矿（避免刷矿）
```

## FeedBack

- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
