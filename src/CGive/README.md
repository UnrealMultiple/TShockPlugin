# CGive 离线命令

- 作者: Leader
- 出处: 无
- 离线执行命令，玩家登陆游戏时自动执行命令。



## Rest API

| 路径            | 权限 |      说明       |
|---------------|:--:|:-------------:|
| /getWarehouse | 无  | 查询/give命令具体信息 |

## 指令

| 语法                        |     权限      |     说明      |
|---------------------------|:-----------:|:-----------:|
| /cgive personal [命令] [目标] | cgive.admin | 为某个玩家添加一条命令 |
| /cgive all [执行者] [命令]     | cgive.admin |  所有玩家离线命令   |
| /cgive list               | cgive.admin |   离线命令列表    |
| /cgive del [id]           | cgive.admin |   删除离线命令    |
| /cgive reset              | cgive.admin |   重置离线命令    |

## 配置

```json5
暂无
```

## 更新日志

```
v1.0.0.8
- 修正 GetString
v1.0.0.4
- i18n和README_EN.md
v1.0.0.3
- i18n预备
v1.0.0.2
- 完善rest卸载函数
V1.0.0.1
- 优化简化部分代码，完善卸载函数
```

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love