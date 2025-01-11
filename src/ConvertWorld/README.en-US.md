# ConvertWorld 
- Author: onusai 羽学
- Source: tshock-bless-world
- Defeating the specified monster converts all world tiles and items in chests


## Commands
```
None
``` 
  
## Config

---
Configuration Notes
---
1. The `World Tile Replacement Table` is for tile IDs, and the `Chest Item Replacement Table` is for item IDs.

2. The `Monster Name` will be automatically filled in when you use /reload based on the `Monster ID`.

3. Multiple `Monster IDs` can be listed.

4. For `Defeat All`, all monsters listed in the `Monster ID` must be defeated.
Once all specified monsters are defeated, if there are any `specified tiles` or `items in chests` in the world, killing any one of them will trigger the conversion.
If there are no convertible IDs, there will be no notification.



> Configuration file location：tshock/击败怪物替换世界物品.json
```json5
{
  "使用说明": "击败指定NPC将世界所有指定图格与箱子内物品对比1:1转换",  // Usage Instructions: Defeating the specified NPC will convert all specified world tiles and items in chests on a 1:1 basis
  "插件开关": true,  // Plugin Switch: Enables or disables the plugin
  "击败所有": false,  // Defeat All: Whether to defeat all listed monsters before conversion
  "击杀转换表": [  // Kill Conversion Table: List of monsters and their corresponding tile and item replacements
    {
      "怪物名": "血肉墙",  // Monster Name: The name of the monster
      "怪物ID": [  // Monster ID: The ID(s) of the monster
        113
      ],
      "世界图格替换表": {  // World Tile Replacement Table: Mapping of old tile IDs to new tile IDs
        "7": 58,
        "166": 58,
        "6": 107,
        "167": 221,
        "9": 108,
        "168": 222,
        "8": 111,
        "169": 223
      },
      "箱子物品替换表": {  // Chest Item Replacement Table: Mapping of old item IDs to new item IDs
        "9": 621,
        "188": 499,
        "189": 500,
        "964": 534,
        "848": 857
      }
    },
    {
      "怪物名": "世纪之花",  // Monster Name: The name of the monster
      "怪物ID": [  // Monster ID: The ID(s) of the monster
        262
      ],
      "世界图格替换表": {  // World Tile Replacement Table: Mapping of old tile IDs to new tile IDs
        "12": 236,
        "22": 211,
        "204": 211
      },
      "箱子物品替换表": {  // Chest Item Replacement Table: Mapping of old item IDs to new item IDs
        "953": 976,
        "975": 976,
        "29": 1291
      }
    }
  ]
}
```
## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
