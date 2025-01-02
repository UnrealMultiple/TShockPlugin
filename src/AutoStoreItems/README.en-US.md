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
> Configuration file location：tshock/AutoStoreItems.en-US.json
```json5
{
  "Enable": true, // Enables equipment holding detection and main inventory holding detection features.
  "Optimize": true, // Optimization settings, this may include setting the storage speed (Main Inventory) to the recommended value of 120, as well as money storage speed settings, etc.
  "Piggiy": true, // Enables the piggy bank (money storage) feature.
  "Safe": true, // Enables the safe storage feature.
  "Forge": true, // Enables the Defender's Forge feature.
  "VoidVault": true, // Enables the Void Bag feature.
  "StoragePrompt": true, // Shows a prompt message when items are stored.
  "HoldItemToActivate": false, // Whether a specific item needs to be held to activate the storage function; if disabled, having one of the specified items in the inventory will suffice to activate it.
  "TargetItems": [ // List of item IDs that need to be detected for holding. When the player holds these items, it can trigger the auto-storage function.
    87,
    346,
    3213,
    3813,
    4076,
    4131,
    5098, // Note: Here ID `5098` seems to replace the original `5325`, possibly an update or correction.
    5325 // It appears both IDs are included now. Please verify if this is intentional.
  ],
  "LoadoutItem": [ // List of item IDs for armor and accessories. Equipping these items will trigger auto-storage when the player moves or attacks.
    88,
    410,
    411,
    489,
    490,
    491,
    855,
    935,
    1301,
    2220,
    2998,
    3034,
    3035,
    3061,
    3068,
    4008,
    4056,
    4989,
    5098,
    5107,
    5126
  ]
}
```

## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
