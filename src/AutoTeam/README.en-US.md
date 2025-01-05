# AutoTeamPlus

- Authors: 十七，肝帝熙恩
- Source: None
- Automatically assign players from a group to a specific team

## Commands

| 语法               |            Permission           |                                          说明                                         |
| ---------------- | :-----------------------------: | :---------------------------------------------------------------------------------: |
| /autoteam or /at | autoteam.toggle |            Toggle the automatic team assignment feature.            |
| None             |            noautoteam           | Having this permission will not automatically assign you to a team. |

## Config

> Configuration file location：tshock/AutoTeam.en-US.json

- Team Name Reference：

| 中文  | English |
| --- | ------- |
| 无队伍 | none    |
| 红队  | red     |
| 绿队  | green   |
| 蓝队  | blue    |
| 黄队  | yellow  |
| 粉队  | pink    |

- Configuration Example

```json5
{
  "Enable": true,
  "GroupTemp": {
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
v2.4.5
修复reload不重载配置文件
v2.4.3
修复了默认会横插一脚的问题,顺带把指令逻辑改简单了
v2.4.2
添加英文翻译
v2.4.1
补全卸载函数
```

## FeedBack

- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
