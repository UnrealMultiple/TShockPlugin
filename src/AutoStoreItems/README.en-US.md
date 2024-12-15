# AutoStoreItems

- Authors: 羽学 cmgy雱
- Source: None
- Automatically store specified items into storage space based on configured item IDs.
- (Supports automatic money storage, thanks to cmgy雱’s coin stacking algorithm)

---
Configuration Notes
---
1.`Whether to hold (是否手持)` needs to select one of the `held items (持有物品)` to activate the storage function. If disabled: the backpack `contains one of them (含有其中1个)`  to activate.

2.`Storage speed (存物速度)` should not be lower than `60`(recommended`120`), otherwise manually `continuously` quickly placing the `same item into the storage space slot` will cause the item `stack to double`

3.`Item names (物品名)` will be automatically written based on `item IDs (物品ID)` when using the `/Reload` command. `Item stack (物品数量)` is the minimum storage requirement.

4.`Armor accessories (装备饰品)` will only detect `3 Armor slots + 7 accessory slots`, unrelated to `storage speed (存物速度)`，Equipping specified `accessories/armor (饰品/盔甲)` will trigger auto-storage when the player moves or attacks.

5.`Existing BUG` collected items will be ` Unfavorite` (referring to the `Void Bag` potions having the risk of `stacking into the chest`).


## Config
> Configuration file location：tshock/自动储存.json
```json
{
  "装备持有检测": true, // Equipment holding detection
  "背包持有检测": true, // Main Inventory holding detection
  "存物速度(背包)": 120.0, // Storage speed (Main Inventory)
  "存钱速度(背包)": 20.0, // Money storage speed (Main Inventory)
  "使用说明1": "[Whether to hold (是否手持)] needs to select one of the held items (持有物品) to activate the storage function. If disabled: the backpack contains one of them (含有其中1个) to activate",
  "使用说明2": "[Storage speed (存物速度)] should not be lower than 60 (recommended 120), otherwise manually continuously quickly placing the same item into the storage space slot will cause the item stack to double",
  "使用说明3": "[Item names (物品名)] will be automatically written based on item IDs (物品ID) when using the /Reload command. Item stack (物品数量) is the minimum storage requirement.",
  "使用说明4": "[Armor accessories (装备饰品)] will only detect 3 Armor slots + 7 accessory slots, unrelated to storage speed (存物速度)，Equipping specified accessories/armor (饰品/盔甲) will trigger auto-storage when the player moves or attacks.",
  "使用说明5": "[Existing BUG] collected items will be Unfavorite (referring to the Void Bag potions having the risk of stacking into the chest)", 
  "存钱罐": true, // Piggy bank
  "保险箱": true, // Safe
  "虚空袋": true, // Void bag
  "护卫熔炉": true, // Defender's Forge
  "储存物品提示": true, // Storage item prompt
  "是否手持(↓)": false, // Whether to hold (↓)
  "持有物品": [ //Item IDs
    87,
    346,
    3213,
    3813,
    4076,
    4131,
    5325
  ],
  "装备饰品": [ //Armor & accessories
    5126 // Item IDs
  ],
  "储存物品表": [
    {
      "物品名(不用写)": "凝胶", // Item name (no need to write)
      "物品数量": 20, // Item stack
      "物品ID": [ // Item IDs
        23
      ]
    },
    {
      "物品名(不用写)": "护卫奖章, 坠落之星, 礼袋, 礼物", // Item name (no need to write)
      "物品数量": 1, // Item stack
      "物品ID": [ // Item IDs
        3817,
        75,
        1774,
        1869
      ]
    }
  ]
}

```

## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
