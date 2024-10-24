# DamageRuleLoot

- Authors: Yu Xue、West River Kid
- Source: Tshock Official QQ Group 816771079
- Description: This plugin determines whether a loot bag is dropped based on the percentage of damage dealt by players, built upon a damage statistics plugin with secondary creation.

## Changelog

```
v1.3.1
Refactored repeated parts into unified methods.
Fixed the configuration item ["Include Critical Hits"] in the custom damage transfer table.
Added more configuration items for custom damage transfer:
["Monster Name"] Automatically writes the ID of the ["Injured Monster"] when using /reload.
["Minimum Transfer Damage"] The minimum threshold to trigger damage transfer.
["Maximum Transfer Damage"] The maximum cap for damage transfer.
["Broadcast Ranking"] Whether to broadcast the output ranking of the ["Injured Monster"].
Removed the configuration item ["Exclude Giant Rock Head Damage Calculation"].

v1.3.0
Added gradient color to the damage leaderboard title.
All transferred damage is considered as true damage, added a custom damage transfer table.
Added configuration items for real damage dealt to Meat Mountain by attacking Imps and Hungry Souls.
Changed Mechanical Skeletron's limb damage from false value to real damage.
Added statue monster judgment for damage transfer.
Added town NPC, statue monster, dummy judgment for monster kill tables.
Custom damage transfer table has its own damage calculation logic (not actual HP values).

v1.2.3
Added special handling for Martian Saucer.
Added Medusa judgment and special handling.
Added developer-specific critical hit monitoring configuration item.
Added configuration item to ignore Giant Rock head damage.
Added configuration item to calculate Mechanical Skeletron limb damage (false value).
Added configuration item for real damage dealt to Pig Dragon by attacking Sharkron.

v1.2.2
Reconstructed the "Damage Monster Table Method" to make damage closer to accurate values.
Added critical hit continuous statistic broadcast and coward count broadcast.

v1.2.1
Added critical damage counting method to summarize players' real damage.
Moved advertisement content to Config for player customization.

v1.2.0
Reconstructed all code, developed secondarily based on Zhi's damage statistics plugin.
Special processing for individualized bosses' damage output.
Beautified the content of output ranking broadcasts.
Added additional NPCs for the damage leaderboard extension.
Added separate toggle configuration items for punishment and damage leaderboards.

v1.1.0
Removed most unnecessary parameters.
Converted the ["Player Output Table"] into a dictionary key-value format for easier reference.
Optimized to judge loot bag drops even in multi-boss scenarios.

v1.0.0
A damage rule loot plugin created based on a damage statistics plugin.
New players joining the server automatically create a ["Player Data Table"], and if the player is already in the config, it will clear their ["Damage Value"].
Players can only pick up items within ["Item ID"] if their ["Damage Percentage"] dealt to the boss exceeds the ["Pickup Condition"] percentage.

```

## Commands

| Syntax                             | Aliases  |       Permission	       |                   Description                   |
| -------------------------------- | :---: | :--------------: | :--------------------------------------: |
| /reload  | None |   tshock.cfg.reload    |    Reloads the configuration file.    |

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
```json
{
  `Plugin Switch`: true,
  `Whether Punish`: true,
  `Ad Switch`: true,
  `Ad Content`: `[i:3456][C/F2F2C7:Plugin Developed By] [C/BFDFEA:Yuxue] [C/E7A5CC:|] [c/00FFFF:Xijiangxiazi][i:3459]`,
  `Damage Ranking Broadcast`: true,
  `Punishment Ranking Broadcast`: true,
  `Below This Percentage No Loot Bag Drops`: 0.15,
  `Tophaze New Three Kings Stat as Medusa Damage Ranking`: true,
  `Ignore Giant Rock Head Damage Calculation`: false,
  `Real Damage Caused by Attacking Mechanical Skeletron Limbs`: true,
  `Real Damage Caused by Attacking Sharkron to Pig Dragon`: true,
  `Real Damage Caused by Attacking Imps and Hungry Souls to Meat Mountain (Only FTW and Tophaze)`: true,
  `Non-Boss Monster Names Participating in Damage Leaderboard`: [
    `Ice Giant`,
    `Dust Elemental`,
    `Corrupt Chest Demon`,
    `Crimson Chest Demon`,
    `Hallowed Chest Demon`,
    `Dark Caster`,
    `Ogre`,
    `Goblin Sorcerer`,
    `Pirate Ship`,
    `Fear Nautilus`,
    `Plasm Goblin Shark`,
    `Blood Eel`,
    `Pirate Captain`,
    `Martian Saucer`
  ],
  `Monitor Critical Hit Count`: false,
  `Monitor Damage Transfer`: false,
  `Custom Damage Transfer`: true,
  `Custom Damage Transfer Table`: [
    {
      `Monster Name`: `Eye of Cthulhu`,
      `Injured Monster`: 4,
      `Stop Transfer HP`: 600,
      `Minimum Transfer Damage`: 1,
      `Maximum Transfer Damage`: 200,
      `Include Critical Hits`: false,
      `Broadcast Ranking`: true,
      `Damage Value into Ranking`: true,
      `Transferred Damage Monsters`: [
        5
      ]
    },
    {
      `Monster Name`: `King Slime`,
      `Injured Monster`: 50,
      `Stop Transfer HP`: 800,
      `Minimum Transfer Damage`: 1,
      `Maximum Transfer Damage`: 200,
      `Include Critical Hits`: true,
      `Broadcast Ranking`: true,
      `Damage Value into Ranking`: true,
      `Transferred Damage Monsters`: [
        1,
        535
      ]
    },
    {
      `Monster Name`: `Plantera`,
      `Injured Monster`: 262,
      `Stop Transfer HP`: 10000,
      `Minimum Transfer Damage`: 1,
      `Maximum Transfer Damage`: 1000,
      `Include Critical Hits`: true,
      `Broadcast Ranking`: true,
      `Damage Value into Ranking`: true,
      `Transferred Damage Monsters`: [
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