# History 记录图格操作

- 作者: Cracker64 & Zaicon & Cai
- 出处: 无
- 记录每一位玩家对图格的操作
- 这使得你可以用指令来恢复图格
- 或复现玩家的操作
- 避免某些情况因玩家的故意破坏导致游戏无法正常进行

## 更新日志

```
暂无
```

## 指令

| 语法                             | 别名  |       权限       |                   说明                   |
| -------------------------------- | :---: | :--------------: | :--------------------------------------: |
| /history [账号名] [时间] [范围]  | /历史 |   history.get    |    还原一个用户在时间跨度和半径内行为    |
| /prunehist [时间]                | /删除 |  history.prune   |      删除在一个时间跨度上的历史记录      |
| /reenact [账号名] [时间] [范围]  | /复现 | history.reenact  | 在时间跨度和半径内重新创建玩家的所有动作 |
| /rollback [账号名] [时间] [范围] | /回溯 | history.rollback |   在时间跨度和半径内还原玩家的所有动作   |
| /rollback [账号名] [时间] [范围] | /撤销 | history.rollback |   在时间跨度和半径内还原玩家的所有动作   |
| /hreset |/重置历史| history.admin |   重置数据库表   |

## 配置

```json
暂无
```
## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/Controllerdestiny/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love