# pvper 决斗系统

- 作者: Soofa、羽学  
- 出处: [github](https://github.com/Soof4/PvPer/)  
+ 这是一个Tshock服务器插件主要用于：  
- 实现了玩家间的PvP战斗功能，插件使用SQLite数据库存储决斗数据，  
- 通过事件钩子监听游戏事件，对玩家行为进行限制与管理，确保决斗规则得到遵循。  
- 同时，插件支持配置文件加载与重载。  
- 修改配置文件中的【Player1与Player2】设置双方玩家位置  
- 修改配置文件中的【ArenaPos1与ArenaPos2】设置双方竞技场位置  
- 注意：玩家如果脱离竞技范围则会被直接杀死判输。  
## 更新日志

```
1.0.2
羽学汉化并修改了所有指令与其回馈信息，  
并加入了个新指令权限用于重置玩家数据表
```
## 指令

| 语法           |        权限         |   说明   |
| -------------- | :-----------------: | :------: |
| /pvp add 玩家名 |  pvper.use  | 邀请玩家参加决斗 |
| /pvp yes | pvper.use    |同意决斗|
| /pvp no | pvper.use    |拒绝决斗|
| /pvp list | pvper.use   |  排行榜 |
| /pvp data | pvper.use   | 战绩查询 |
| /决斗重置 | pvper.admin   |  重置玩家的数据库表 |

## 配置

```json
{
  "Player1PositionX": 0, //玩家1号位置
  "Player1PositionY": 0,
  "Player2PositionX": 0, //玩家2号位置
  "Player2PositionY": 0,
  "ArenaPosX1": 0, //玩家1号竞技场范围
  "ArenaPosY1": 0, 
  "ArenaPosX2": 0, //玩家2号竞技场范围
  "ArenaPosY2": 0
}
```
## 反馈
- 共同维护的插件库：https://github.com/THEXN/TShockPlugin/
- 国内社区trhub.cn 或 TShock官方群等
