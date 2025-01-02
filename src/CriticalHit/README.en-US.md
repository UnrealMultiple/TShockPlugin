# CriticalHit

- Authors: White,Stone·Free,肝帝熙恩
- Source: TShock Official QQ Group
- When attacking NPCs, if a critical hit occurs, special floating text will appear (similar to damage numbers).
- Different text will appear based on the weapon type.
- It can be set to display text even if it’s not a critical hit.

## Config
> Configuration file location：tshock/CriticalConfig.json
```json5
{
  "总开关": true, // Main switch
  "仅暴击显示": true, // Only show critical hits
  "消息分类": {
    "近战": { // Melee weapon critical hit
      "详细消息设置": { // Critical Hit Texts
        "Boom!": [ //Color
          255, 
          120,
          0
        ]
      }
    },
    "爆炸": { // Explosion
      "详细消息设置": {
        "Boom!": [
          255,
          0,
          0
        ]
      }
    },
    "远程": { // Ranged weapon critical hit
      "详细消息设置": {
        "Biu biu!": [
          50,
          255,
          10
        ]
      }
    },
    "魔法": { // Magic weapon critical hit
      "详细消息设置": {
        "Whoomph!": [
          0,
          200,
          255
        ]
      }
    }
  }
}

```

## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
