# CaiRewardChest

- Author: Cai
- Source: This repository
- Tired of server chests that can only be claimed once? Try Cai's reward chests.
- After creating a new map, use `/CaiRCInit` to convert all chests on the map into reward chests that each player can
  claim only once.
- Players can claim the reward by simply interacting with the chest, and they will receive the corresponding chest loot.
- Of course, you can also use `/CaiRCAdd` and `/CaiRCEdit` to set up your own reward chests.

> Note: Please use `/CaiRCInit` after generating the map, otherwise player chests might be mistakenly marked.

## Commands

| Command             |      Permission       |                                            Details                                            |
|---------------------|:---------------------:|:---------------------------------------------------------------------------------------------:|
| /rcinit             |  CaiRewardChest.Init  | Set all chests on the map as reward chests (only applicable when the server is first started) |
| /rcadd              |  CaiRewardChest.Add   |                                      Add a reward chest                                       |
| /rcedit             |  CaiRewardChest.Edit  |                                      Edit a reward chest                                      |
| /rcdelete or /rcdel | CaiRewardChest.Delete |                                     Delete a reward chest                                     |
| /rcclear            | CaiRewardChest.Clear  |                                   Remove all reward chests                                    |

## Config

```json5 
None
```

## FeedBack

- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
