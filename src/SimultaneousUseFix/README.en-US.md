# SimultaneousUseFix

- Author: 肝帝熙恩，恋恋
- Source: None
- Solve problems such as card double hammer star rotary machine gun
- Due to some testing reasons, the last bucket will be judged, so it is added without testing
> [!NOTE]  
> Requires preset: https://github.com/sgkoishi/yaaiomni/releases

## Instruction

| Command |         Permissions         |   Description   |
|----|:------------------:|:------:|
| None  | SimultaneousUseFix | Completely exempt from inspection |

## Configuration
    Configuration file location: tshock/解决卡双锤卡星旋机枪之类的问题.json
```json
{
  "免检测物品列表": [ //List of items without detection
    205,
    206,
    207,
    1128
  ],
  "是否杀死": true, //Whether to kill player
  "是否上buff": false, //Whether to buff player
  "buff时长(s)": 60, //Buff duration (s)
  "上什么buff": [ //What buff to do
    163,
    149,
    23,
    164
  ],
  "是否踢出": false //Whether to kick out player
}
```

## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
