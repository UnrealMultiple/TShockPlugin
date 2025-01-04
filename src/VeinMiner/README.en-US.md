# VeinMiner

- Authors: Megghy|YSpoof|Maxthegreat99|肝帝熙恩|Cai
- Source: [github](https://github.com/Maxthegreat99/TSHockVeinMiner)
- To quickly mine veins of ore
  
> [!IMPORTANT]
> To enable vein mining, you need the `veinminer` permission.
> Authorization command: `/group addperm default veinminer` (default is the default group, you can replace it with the group you need)

## Commands

| Command        | Permission |                 Details                 |
|----------------|:----------:|:---------------------------------------:|
| /vm            | veinminer  |           Toggle vein mining            |
| /vm [Any agrs] | veinminer  | Toggle vein mining notification message |

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

## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
