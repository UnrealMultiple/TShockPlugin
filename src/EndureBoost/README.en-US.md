# EndureBoost

- Authors: 肝帝熙恩
- Source: here
- Grant specified buff when the player has a certain number of potions or specific items in their inventory
- Refresh on player join, command use, death, and recall


## Commands

| Command                                    | Permission  |                   Details                    |
|--------------------------------------------|:-----------:|:--------------------------------------------:|
| /ebbuff or /ldbuff or /loadbuff or /刷新buff | EndureBoost | Immediately refresh the plugin's buff status |

## Config
> Configuration file location：tshock/EndureBoost.json
```json5
{
  "猪猪储钱罐": true, // Piggy Bank
  "保险箱": false, // Safe
  "护卫熔炉": false, // Defender's Forge
  "虚空宝藏袋": true, // Void Bag
  "持续时间(s)": 3600, // Buff duration in seconds
  "自动更新频率(s)": 5, // Automatic update frequency in seconds

  "药水": [ // Potions
    {
      "药水id": [ // Potion ID
        288,
        289
      ],
      "药水数量": 30 // Required potion count
    },
    {
      "药水id": [ // Potion ID
        290
      ],
      "药水数量": 200 // Required potion count
    }
  ],

  "背包": [ // Items in inventory
    {
      "同时拥有时触发": false, // When true, all items must be present, and each must meet the required quantity to trigger
      "物品id": [ // Item IDs
        2,
        3
      ],
      "物品数量": 3, // Required item count
      "给buff的id": [ // Buff IDs to grant
        9,
        11
      ]
    },
    {
      "同时拥有时触发": false, // When true, all items must be present, and each must meet the required quantity to trigger
      "物品id": [ // Item ID
        5
      ],
      "物品数量": 10, // Required item count
      "给buff的id": [ // Buff IDs to grant
        11
      ]
    }
  ],

  "配饰": [ // Equipment and accessories, including social slots
    {
      "同时拥有时触发": true, // When true, all items must be present, and each must meet the required quantity to trigger
      "配饰id": [ // Accessory IDs
        2763,
        2764,
        2765
      ],
      "物品数量": 1, // Required item count
      "给buff的id": [ // Buff IDs to grant
        181
      ],
      "非本装备栏也触发": true, // Trigger across all loadouts
      "社交栏也触发": true // Trigger with social slots
    },
    {
      "同时拥有时触发": false, // When true, all items must be present, and each must meet the required quantity to trigger
      "配饰id": [ // Accessory IDs
        13,
        14
      ],
      "物品数量": 1, // Required item count
      "给buff的id": [ // Buff IDs to grant
        178
      ],
      "非本装备栏也触发": false, // Do not trigger across other loadouts
      "社交栏也触发": false // Do not trigger with social slots
    }
  ],

  "染料": [ // Dyes
    {
      "同时拥有时触发": true, // Trigger when all dyes are present
      "染料id": [ // Dye IDs
        2871
      ],
      "物品数量": 2, // Required item count
      "给buff的id": [ // Buff IDs to grant
        10
      ],
      "非本装备栏也触发": false // Do not trigger across other loadouts
    },
    {
      "同时拥有时触发": true, // Trigger when all dyes are present
      "染料id": [ // Dye IDs
        1007,
        1008
      ],
      "物品数量": 1, // Required item count
      "给buff的id": [ // Buff IDs to grant
        12
      ],
      "非本装备栏也触发": false // Do not trigger across other loadouts
    }
  ],

  "全部物品": [ // All items across inventory and storage
    {
      "同时拥有时触发": true, // Trigger when all items are present
      "物品id": [ // Item IDs
        20,
        30
      ],
      "物品数量": 5, // Required item count
      "给buff的id": [ // Buff IDs to grant
        26,
        27,
        29
      ]
    }
  ]
}
```

## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
