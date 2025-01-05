# CaiRewardChest

- Author: Cai
- 出处: 本仓库
- Tired of server chests that can only be claimed once? Try Cai's reward chests.
- After creating a new map, use `/CaiRCInit` to convert all chests on the map into reward chests that each player can
  claim only once.
- 而且只要点一下就能领取，并且会给予玩家对应的箱子凋落物
- Of course, you can also use `/CaiRCAdd` and `/CaiRCEdit` to set up your own reward chests.

> Note: Please use `/CaiRCInit` after generating the map, otherwise player chests might be mistakenly marked.

## Commands

| 语法                  |               Permission              |                                                        说明                                                        |
| ------------------- | :-----------------------------------: | :--------------------------------------------------------------------------------------------------------------: |
| /初始化奖励箱 或 /rcinit   |  CaiRewardChest.Init  | Set all chests on the map as reward chests (only applicable when the server is first started) |
| /rcadd              |   CaiRewardChest.Add  |                                                Add a reward chest                                                |
| /rcedit             |  CaiRewardChest.Edit  |                                                Edit a reward chest                                               |
| /rcdelete or /rcdel | CaiRewardChest.Delete |                                               Delete a reward chest                                              |
| /rcclear            |  CaiRewardChest.Clear |                                             Remove all reward chests                                             |

## Config

```json5
None
```

## 更新日志

```
2024.12.8.1 修复数据库错误, 添加奖励箱ID缓存以防止卡服
2024.10.1.2 简化英文命令
2024.10.1.1 修复英文指令不能用的问题
2024.9.29.1 i18n和README_EN.md
2024.9.28.1 i18n预备
2024.9.22.1 细分命令权限，初始化奖励箱时绕过空箱子(梳妆台)
2024.7.29.1 补全卸载函数
2024.7.24.1 禁止快速堆叠，修复快速堆叠刷物品
1.0.0 添加插件

```

## FeedBack

- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
