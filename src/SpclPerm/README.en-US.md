# SpclPerm Server Privileges
- Author: 羽学
- Source: None
- Automatically execute on the server or resurrection according to the list: Invincible, BUFF, Items, Commands, Set Inventory,
- The user group name ignores all switches. The switch is only valid for the privilege list. You need to close the user group permissions and delete the group name directly.

## Instruction

```
None
```

## Configuration
> Configuration file location: tshock/服主特权.json
```json5
{
  "使用说明": "根据名单进服或复活自动执行：无敌、BUFF、物品、命令、设置背包，用户组名无视所有开关，开关只对特权名单有效，需关用户组权限直接删组名", //Instructions for use
  "进服给无敌": true, //Being invincible
  "进服用命令": false, //Take the command
  "进服给物品": false, //Entering the item
  "进服给BUFF": true, //Get BUFF
  "设置SSC背包": true, //Set SSC inventory
  "特权名单/受以上开关影响": [ //Privileges/affected by the above switches
    "羽学",
    "灵乐"
  ],
  "所有特权的用户组名": "admin,owner", //All privileged user group names
  "无敌权限的用户组名": "admin,owner", //User group name with invincible permissions
  "物品权限的用户组名": "admin,owner", //User group name for item permissions
  "BUFF权限的用户组名": "admin,owner", //User group name with BUFF permissions
  "命令权限的用户组名": "admin,owner", //User group name for command permissions
  "特权背包的用户组名": "admin,owner", //User group name of privileged inventory
  "命令表": [ //Command list
    "/clear i 9999",
    ".clear p 9999"
  ],
  "物品表（ID:数量）": { //Item list (ID: Amount)
    "4346": 1
  },
  "Buff表（ID:分钟）": { //Buff list (ID: minutes)
    "1": 10,
    "11": -1,
    "12": -1,
    "3": -1,
    "192": -1,
    "165": -1,
    "207": -1,
    "274": -1,
    "63": -1,
    "257": -1
  },
  "特权背包表": [ //Privileged inventory table
    {
      "初始血量": 200, //Initial health
      "初始魔力": 100, //Initial mana
      "初始物品": [ //Initial Items
        {
          "netID": 4956,
          "prefix": 81,
          "stack": 1
        },
        {
          "netID": 4346,
          "prefix": 0,
          "stack": 1
        },
        {
          "netID": 5437,
          "prefix": 0,
          "stack": 1
        }
      ]
    }
  ],
  "非特权背包表": [ //Non-privileged inventory table
    {
      "初始血量": 100, //Initial health
      "初始魔力": 20, //Initial mana
      "初始物品": [ //Initial Items
        {
          "netID": -15,
          "prefix": 0,
          "stack": 1
        },
        {
          "netID": -13,
          "prefix": 0,
          "stack": 1
        },
        {
          "netID": -16,
          "prefix": 0,
          "stack": 1
        }
      ]
    }
  ]
}
```

## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
