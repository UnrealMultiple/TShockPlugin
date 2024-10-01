# CaiRewardChest

- Author: Cai
- Source: This repository
- Tired of server chests that can only be claimed once? Try Cai's reward chests.
- After creating a new map, use `/CaiRCInit` to convert all chests on the map into reward chests that each player can claim only once.
- Players can claim the reward by simply interacting with the chest, and they will receive the corresponding chest loot.
- Of course, you can also use `/CaiRCAdd` and `/CaiRCEdit` to set up your own reward chests.

> Note: Please use `/CaiRCInit` after generating the map, otherwise player chests might be mistakenly marked.


## Commands

| Command     | Permission |          Details          |
|---------|:---------------------:|:------------------:|
| /初始化奖励箱 or /caircinit |  CaiRewardChest.Init  | Set all chests on the map as reward chests (only applicable when the server is first started) |
| /添加奖励箱 or /caircadd    |  CaiRewardChest.Add   |      Add a reward chest        |
| /编辑奖励箱 or /caircedit   |  CaiRewardChest.Edit  |      Edit a reward chest       |
| /删除奖励箱 or //caircdelete or /caircdel | CaiRewardChest.Delete |      Delete a reward chest     |
| /清空奖励箱 or /caircclear  | CaiRewardChest.Clear  |      Remove all reward chests  |

## Config

```json    
None
```

## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
