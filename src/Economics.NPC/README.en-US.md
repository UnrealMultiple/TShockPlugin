# Economics.NPC Plugin Custom Monster Rewards

- Author: Shaosiming
- Source: None
- Configuration NPC Exclusive Rewards

> [!NOTE]
>  requires the pre-installed plugin: EconomicsAPI (this repository)


## Commands

None

## Configuration
>  configuration file location: tshock/Economics/NPC.json
```json5
{
  "开启提示": true,
  "提示内容": "你因击杀{0},获得额外奖励{1}{2}个",
  "额外奖励列表": [
    {
      "怪物ID": 390,
      "怪物名称": "猪鲨",
      "奖励货币": 100000,
      "按输出瓜分": true // false 时每个人发10000奖励
    }
  ],
  "转换率更改": {
    "50": 1.3 //id 和 转换率
  }
}
```

## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love