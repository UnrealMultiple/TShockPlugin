# EndureBoost

- Authors: 肝帝熙恩
- Source: here
- Grant specified buff when the player has a certain number of potions or specific items in their inventory
- Refresh on player join, command use, death, and recall


## Commands

| Command          |   Permission    |          Details          |
| -------------- | :-----------------: | :------: |
| /ebbuff or /ldbuff or /loadbuff or /刷新buff| EndureBoost| Immediately refresh the plugin's buff status|

## Config
> Configuration file location：tshock/EndureBoost.json
```json
{
  "猪猪储钱罐": false, // Piggy Bank
  "保险箱": false, // Safe
  "护卫熔炉": false, // Defender's Forge
  "虚空宝藏袋": true, // Void Bag
  "持续时间(s)": 3600, // Duration (s)
  "药水": [ // Potions
    {
      "药水id": [ // Potion ID
        288,
        289
      ],
      "药水数量": 30 // Potion count
    },
    {
      "药水id": [ // Potion ID
        290
      ],
      "药水数量": 200 // Potion count
    }
  ],
    "其他物品": [ // Other items
    {
      "物品id": [ // Item ID
        2,
        3
      ],
      "物品数量": 3, // Item count
      "给buff的id": [ // Grant buff ID
        1,
        2
      ]
    },
    {
      "物品id": [ // Item ID
        5
      ],
      "物品数量": 3, // Item count
      "给buff的id": [ // Grant buff ID
        3
      ]
    }
  ]
}
```

## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
