# AutoTeamPlus 更好的自动队伍

- 作者: 十七，肝帝熙恩
- 出处: 无
- 自动分配一个组的玩家到特定队伍


## 指令

| 语法              |       权限        |      说明       |
|-----------------|:---------------:|:-------------:|
| /autoteam 或 /at | autoteam.toggle |   开关自动队伍功能    |
| 无               |   noautoteam    | 有该权限不会被自动分配队伍 |

## 配置
> 配置文件位置：tshock/AutoTeam.zh-CN.json
- 中文英文队伍名字参考如下：

| 中文  | English |
|-----|---------|
| 无队伍 | none    |
| 红队  | red     |
| 绿队  | green   |
| 蓝队  | blue    |
| 黄队  | yellow  |
| 粉队  | pink    |

- 配置示例
```json5
{
  "开启插件": true, 
  "组对应的队伍": {
    "guest": "pink",
    "default": "蓝队",
    "owner": "红队",
    "admin": "green",
    "vip": "none"
  }
}
```

## 更新日志
```
v2.4.8
添加 GetString
v2.4.6
修复一些逻辑，没有配置的现在可以自由切换了，配置错误的会锁在上一个的队伍
v2.4.5
修复reload不重载配置文件
v2.4.3
修复了默认会横插一脚的问题,顺带把指令逻辑改简单了
v2.4.2
添加英文翻译
v2.4.1
补全卸载函数
```


## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
