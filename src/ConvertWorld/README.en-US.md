# ConvertWorld 击败怪物替换世界物品

- Author: onusai 羽学
- Source: tshock-bless-world
- Defeating the specified monster converts all world tiles and items in chests

## Commands

```
None
```

---

## Configuration Notes

The `World Tile Replacement Table` is for tile IDs, and the `Chest Item Replacement Table` is for item IDs.

The `Monster Name` will be automatically filled in when you use /reload based on the `Monster ID`.

Multiple `Monster IDs` can be listed.

For `Defeat All`, all monsters listed in the `Monster ID` must be defeated.
Once all specified monsters are defeated, if there are any `specified tiles` or `items in chests` in the world, killing any one of them will trigger the conversion.
If there are no convertible IDs, there will be no notification.

## Config

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

## 更新日志

```
v1.0.2
i18n和README_EN.md
v1.0.1
i18n预定
v1.0.0
修改BlessWorld插件，将bless指令与服务器初始化时自动转换功能移除     
加入了对击杀指定NPC实现转换设定      
加入了从《怪物ID》取值，通过/reload事件重载，自动赋名给《怪物名》    
```

## FeedBack

- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
