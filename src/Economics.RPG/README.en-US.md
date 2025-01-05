# Economics.RPG Upgrade Career Plugin

- 作者: 少司命
- Source: None
- RPG Upgrade Plugin

> [!NOTE]
> requires the pre-installed plugin: EconomicsAPI (this repository).

## Commands

| Syntax                                                                      |                      Permission                     |  Description |
| --------------------------------------------------------------------------- | :-------------------------------------------------: | :----------: |
| /rank [profession name] |  economics.rpg.rank |    upgrade   |
| /reset level                                                                | economics.rpg.reset | reset career |
| /level reset                                                                | economics.rpg.admin |     reset    |

## Configuration

> configuration file location: tshock/Economics/RPG.json

```json5
{
  "RPG信息": {
    "战士": {
      "聊天前缀": "[战士]",
      "聊天颜色": [0, 238, 0],
      "聊天后缀": "",
      "聊天格式": "{0}{1}{2}: {3}",
      "升级广播": "恭喜玩家{0}升级至{1}!",
      "进度限制": [],
      "升级指令": [],
      "附加权限": [],
      "升级奖励": [],
      "升级消耗": 1000,
      "父等级": "萌新"
    },
    "战士2": {
      "聊天前缀": "[战士2]",
      "聊天颜色": [0, 238, 0],
      "聊天后缀": "",
      "聊天格式": "{0}{1}{2}: {3}",
      "升级广播": "恭喜玩家{0}升级至{1}!",
      "进度限制": [],
      "升级指令": [],
      "附加权限": [],
      "升级奖励": [],
      "升级消耗": 1000,
      "父等级": "战士"
    },
    "重置职业执行命令": [],
    "重置职业广播": "玩家{0}重新选择了职业",
    "重置后踢出": false,
    "默认等级": "萌新"
  }
}
```

## 更新日志

```
V2.0.0.0
适配多货币

V1.0.0.6
添加配置 "手持武器"填写后玩家升级必须手持指定的武器

V1.0.0.5
适配新 EconomicsAPI

V1.0.0.3
- 支持多语言

V1.0.0.2
- 添加权限economics.rpg.chat，拥有此权限不会改变玩家聊天格式。

V1.0.0.1
- 增加显示信息
- 添加/level reset指令
- 添加自定义消息玩家组
- 添加RPG聊天渐变色
```

## FeedBack

- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
