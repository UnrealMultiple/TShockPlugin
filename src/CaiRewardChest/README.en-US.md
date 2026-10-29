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

## Changelog

### v2026.10.29.1
- Adapt reward drops to Terraria 1.4.5 WorldItem ownership synchronization, including reservation duration.
- Skip empty slots and non-positive stacks; validate spawned item indices instead of sending sentinel slot 400.
- Keep claim limits, commands, database format and the reward-chest quick-stack prohibition unchanged.
- Verified on an isolated Terraria 1.4.5.8 / TShock 6.2.1 server: empty slots, item indices, reservation duration, stacks/prefixes, claims across accounts and duplicate rejection, and blocked quick stacking. Actual client pickup still requires in-game verification.

## FeedBack

- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
