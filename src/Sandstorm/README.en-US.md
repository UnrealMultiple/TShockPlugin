# Sandstorm
- Author: onusai 羽学
- Source: [tshock-onus-utility](https://github.com/onusai/tshock-onus-utility)
- Use the command to switch the start and stop of the sandstorm, and you can define the 'wind speed value' in the configuration file.

## Instruction

| Command  |  Alias  |       Permissions        |     Description     |
|-----|:----:|:---------------:|:----------:|
| /sd | /沙尘暴 | sandstorm.admin | Switch to start and stop the sandstorm |

## Configuration
> Configuration file location: tshock/Sandstorm.json
```json5
{
  "使用说明": "instruction: /sd, permission: sandstorm.admin ", //Instructions for use
  "是否允许指令开启沙尘暴": true, //Allowed to turn on the sandstorm
  "是否开启广播": true, //Enable broadcast
  "广播颜色": [ //Broadcast Color
    255.0,
    234.0,
    115.0
  ],
  "开启沙尘暴的风速目标值": 35, //Turn on sandstorm wind speed target value
  "关闭沙尘暴的风速目标值": 20 //Turn off sandstorm wind speed target value
}
```

## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
