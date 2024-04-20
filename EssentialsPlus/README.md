# EssentialsPlus

- 作者: 未知,Cjx适配与修改
- 出处: 网络
- 提供一些管理指令

## 更新日志

```
暂无
```

## 指令

| 语法           |        权限         |   说明   |
| -------------- | :-----------------: | :------: |
| /find |  essentials.find  | 查找指令权限,物品ID等信息|
| /freezetime |  essentials.freezetime  | 冻结时间|
| /kickall |  essentials.freezetime  | 踢出所有在线玩家（管理权限除外）|
| /pvp |  essentials.pvp  | 开启自己的PVP模式|
| /sudo -f <玩家名称> <指令>|  essentials.sudo  | 让玩家执行内容,-f（force）为强制（忽略权限）|
| ...|  ...  | ... |

## 配置

```json
{
  "DisabledCommandsInPvp": [
    "back"
  ],
  "BackPositionHistory": 10,
  "MySqlHost": "如使用Mysql这里需要配置完整信息",
  "MySqlDbName": "",
  "MySqlUsername": "",
  "MySqlPassword": ""
}
```
## 反馈
- 共同维护的插件库：https://github.com/THEXN/TShockPlugin/
- 国内社区trhub.cn 或 TShock官方群等