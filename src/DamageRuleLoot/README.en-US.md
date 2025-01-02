# DamageRuleLoot

- Authors: Yu Xue、West River Kid
- Source: Tshock Official QQ Group 816771079
- Description: This plugin determines whether a loot bag is dropped based on the percentage of damage dealt by players, built upon a damage statistics plugin with secondary creation.


## Commands

| Syntax  | Aliases |    Permission	    |           Description           |
|---------|:-------:|:-----------------:|:-------------------------------:|
| /reload |  None   | tshock.cfg.reload | Reloads the configuration file. |

---
Main Configuration Notes
---
1.Players must exceed the `Below This Percentage No Loot Bag Drops` for their `Damage Percentage` against the boss to pick up the `Loot Bag`.
2.Monsters listed under `Non-Boss Monster Names Participating in Damage Leaderboard` do not participate in punishment ranking broadcasts; 
3.the `Martian Saucer` and `Pirate Ship` have been handled separately, do not delete them.
4.The `Punishment Ranking` broadcast is only associated with bosses that drop loot bags.
5.`Monitor Critical Hit Count` will monitor all players generating critical hits, while `Monitor Damage Transfer` monitors the status of damage transfer; this feature is developer-specific and should not be enabled, otherwise, it will flood the screen.
6.`Tophaze New Three Kings Stat as Medusa Damage Ranking` is only valid in the Tophaze world; if disabled, it will separately broadcast the damage received by each of the new three kings in the Tophaze world. Conversely, it broadcasts overall damage to Medusa; this setting does not affect normal worlds.
7.`Real Damage Caused by Attacking Mechanical Skeletron Limbs` will cause damage numbers to exceed the boss's HP but also allows limb damage.
8.`Real Damage Caused by Attacking Imps and Hungry Souls to Meat Mountain` only works in FTW and Tophaze seeds, causing damage numbers to exceed the boss's HP.
9.If no punishment participation is required, you can disable `Whether Punish`.

---
Custom Damage Transfer Table Notes
---
The `Custom Damage Transfer Table` is located at the end of the list []. Enter , {} and use the /reload command to get a new format (with preset parameters).

`Damage Transfer` only triggers when any player hits the boss once.

`Monster Name` automatically writes the NPC ID of the `Injured Monster` when using the /reload command, no need to fill manually.

`Stop Transfer HP` is based on the `Injured Monster` reaching a certain HP level before stopping the transfer of damage.

`Minimum Transfer Damage` is the lowest threshold for triggering damage transfer.

`Maximum Transfer Damage` is the upper limit for intercepting damage transfer.

`Include Critical Hits` transfers all damage dealt to the `Transferred Damage Monster`; disable to exclude all critical hit damage from transfer.

`Broadcast Ranking` provides ranking information based on the `Injured Monster`.

`Damage Value into Ranking` includes the damage dealt to the `Transferred Damage Monster` into the output leaderboard.

## Configuration
> Configuration file location：tshock/伤害规则掉落.json
```json5
{
  "插件开关": true, // Plugin switch: whether the plugin is on or off
  "是否惩罚": true, // Whether punishment (for rule violations) is enabled
  "广告开关": true, // Ad switch: whether ads are shown
  "广告内容": "[i:3456][C/F2F2C7:插件开发] [C/BFDFEA:by]  羽学 [C/E7A5CC:|] [c/00FFFF:西江小子][i:3459]", // Ad content: the message that will be displayed as an ad
  "伤害榜播报": true, // Damage ranking announcement: whether to announce damage rankings
  "惩罚榜播报": true, // Punishment ranking announcement: whether to announce punishment rankings
  "低于多少不掉宝藏袋": 0.15, // Below which percentage of HP a boss does not drop a treasure bag
  "天顶新三王统计为美杜莎伤害榜": true, // New three kings in Celestial Towers counted for Mechdusa's damage board
  "攻击机械吴克四肢造成真实伤害": true, // Attacking Skeletron Prime's limbs deals true damage
  "攻击鲨鱼龙给猪鲨造成真实伤害": true, // Attacking Sharkrons deal true damage to Duke Fishron
  "攻击小鬼与饿鬼给肉山造成真伤(仅FTW与天顶)": true, // Attacking Fire Imps and The Hungry deals true damage to Wall of Flesh (only in FTW and Get fixed boi)
  "参与伤害榜的非BOSS怪ID": [ // Non-boss monsters' IDs that participate in the damage rankings
    243, // Ice Golem
    541, // Sand Elemental
    473, // Corrupt Mimic
    474, // Crimson Mimic
    475, // Hallowed Mimic
    564, // Dark Mage
    565, // Dark Mage
    576, // Ogre
    577, // Ogre
    471, // Goblin Warlock
    491, // Flying Dutchman
    618, // Dreadnautilus
    620, // Hemogoblin Shark
    621, // Blood Eel
    622, // Blood Eel
    623, // Blood Eel
    216, // Pirate Captain
    392  // Martian Saucer
  ],
  "监控暴击次数": false, // Monitor critical hit counts
  "监控转移伤害": false, // Monitor transferred damage
  "自定义转移伤害": true, // Customized transferred damage
  "自定义转移伤害表": [ // Custom transferred damage table
    {
      "怪物名称": "克苏鲁之眼", // Monster name: Eye of Cthulhu
      "受伤怪物": 4, // ID of the monster taking damage
      "停转血量": 600, // Stop transferring damage when the monster's health drops below this value
      "最低转伤": 1, // Minimum transferred damage
      "最高转伤": 200, // Maximum transferred damage
      "涵盖暴击": false, // Whether to include critical hits in the transfer
      "播报排名": true, // Whether to announce the ranking
      "伤值进榜": true, // Whether the damage value goes into the ranking
      "转伤怪物": [ // IDs of the monsters to which the damage is transferred
        5
      ]
    },
    {
      "怪物名称": "史莱姆王", // Monster name: King Slime
      "受伤怪物": 50, // ID of the monster taking damage
      "停转血量": 800, // Stop transferring damage when the monster's health drops below this value
      "最低转伤": 1, // Minimum transferred damage
      "最高转伤": 200, // Maximum transferred damage
      "涵盖暴击": false, // Whether to include critical hits in the transfer
      "播报排名": true, // Whether to announce the ranking
      "伤值进榜": true, // Whether the damage value goes into the ranking
      "转伤怪物": [ // IDs of the monsters to which the damage is transferred
        535
      ]
    },
    {
      "怪物名称": "世纪之花", // Monster name: Plantera
      "受伤怪物": 262, // ID of the monster taking damage
      "停转血量": 10000, // Stop transferring damage when the monster's health drops below this value
      "最低转伤": 1, // Minimum transferred damage
      "最高转伤": 1000, // Maximum transferred damage
      "涵盖暴击": true, // Whether to include critical hits in the transfer
      "播报排名": true, // Whether to announce the ranking
      "伤值进榜": true, // Whether the damage value goes into the ranking
      "转伤怪物": [ // IDs of the monsters to which the damage is transferred
        264
      ]
    }
  ]
}
```
## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love