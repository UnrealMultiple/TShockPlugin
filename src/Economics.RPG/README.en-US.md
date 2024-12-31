# Economics.RPG Upgrade Career Plugin

- Author: Shao Shi Ming
- Source: None
- RPG Upgrade Plugin

> [!NOTE]
>  requires the pre-installed plugin: EconomicsAPI (this repository).

## Commands

| Syntax                  |     Permission      | Description  |
|-------------------------|:-------------------:|:------------:|
| /rank [profession name] | economics.rpg.rank  |   upgrade    |
| /reset level            | economics.rpg.reset | reset career |
| /level reset            | economics.rpg.admin |    reset     |

## Configuration
>  configuration file location: tshock/Economics/RPG.json
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
## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love