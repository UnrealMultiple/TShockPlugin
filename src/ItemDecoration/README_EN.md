# ItemDeco Plugin

- Author: FrankV22
- Source: [Github](https://github.com/itsFrankV22/ItemsDeco-Plugin)
- **Show Item Name**: Whenever a player switches the item they are holding, the name of the item will appear as a floating message above their head and in the chat. This feature also supports showing the item's damage value, which can be toggled in the config.
- **Color Customization**: The floating message color is by default white (RGB 255, 255, 255) but can be customized.
- **Compatible with** [Floating-MessageChat](https://github.com/itsFrankV22/FloatingText-Chat)

## Changelog

```
No updates yet
```

## Commands
```
No commands yet
```

## Configuration
> Configuration file path: tshock/ItemDeco/ItemChatConfig.json
```json
{
  "CONFIGURATION": {    
    "ShowItem": true, // Whether to show the item name
    "ShowDamage": true, // Whether to show the item’s damage value
    "ItemColor": {  // Color for the item name
      "R": 255,
      "G": 255,
      "B": 255
    },
    "DamageColor": {  // Color for the damage value
      "R": 0,
      "G": 255,
      "B": 255
    }
  }
}
```
> Configuration file path: tshock/ItemDeco/ItemTextConfig.json
```json
{
  "ShowName": true, // Whether to show the item name
  "ShowDamage": true, // Whether to show the item’s damage value
  "DamageText": "Damage", // The prefix text displayed with damage
  "DefaultColor": { // Default color
    "r": 255, // Red component
    "g": 255, // Green component
    "b": 255  // Blue component
  },
  "RarityColors": { // Color for each rarity level
    "-1": { // Gray (169, 169, 169)
      "r": 169,
      "g": 169,
      "b": 169
    },
    "0": { // White (255, 255, 255)
      "r": 255,
      "g": 255,
      "b": 255
    },
    "1": { // Green (0, 128, 0)
      "r": 0,
      "g": 128,
      "b": 0
    },
    "2": { // Blue (0, 112, 221)
      "r": 0,
      "g": 112,
      "b": 221
    },
    "3": { // Purple (128, 0, 128)
      "r": 128,
      "g": 0,
      "b": 128
    },
    "4": { // Orange (255, 128, 0)
      "r": 255,
      "g": 128,
      "b": 0
    },
    "5": { // Red (255, 0, 0)
      "r": 255,
      "g": 0,
      "b": 0
    },
    "6": { // Gold (255, 215, 0)
      "r": 255,
      "g": 215,
      "b": 0
    },
    "7": { // Pink (255, 105, 180)
      "r": 255,
      "g": 105,
      "b": 180
    },
    "8": { // Gold (255, 215, 0)
      "r": 255,
      "g": 215,
      "b": 0
    },
    "9": { // Cyan (0, 255, 255)
      "r": 0,
      "g": 255,
      "b": 255
    },
    "10": { // Pink (255, 105, 180)
      "r": 255,
      "g": 105,
      "b": 180
    },
    "11": { // Dark Purple (75, 0, 130)
      "r": 75,
      "g": 0,
      "b": 130
    }
  }
}
```

## Feedback
- The plugin’s primary repository: [Github](https://github.com/itsFrankV22/ItemsDeco-Plugin)
- Preferred feedback through issues -> Shared plugin repository: https://github.com/UnrealMultiple/TShockPlugin
- Secondary: TShock official group: 816771079
- Alternative: Community sites such as trhub.cn, bbstr.net, tr.monika.love
