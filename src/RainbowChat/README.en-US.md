# RainbowChat

- Author: Professor X制作,nnt升级/汉化,肝帝熙恩、羽学更新1449
- Source: None
- Make the color of the player chat differently every time

## Instruction

| Command                             |               Alias               |        Permissions         |          Description          |
|--------------------------------|:------------------------------:|:-----------------:|:--------------------:|
| /rc                            |          /rainbowchat          |  rainbowchat.use  |         View command menu         |
| /rc gradient                   |        /rainbowchat 渐变         |  rainbowchat.use  | Player's own switch Rainbow Chat【Gradient color】function switch |
| /rc random                     |        /rainbowchat 随机         |  rainbowchat.use  |  Players' own Rainbow Chat【Random color】function switch  |
| /rc gradient start RRR,GGG,BBB | /rainbowchat 渐变 开始 RRR,GGG,BBB |  rainbowchat.use  |       Change the gradient color start value       |
| /rc gradient stop RRR,GGG,BBB  | /rainbowchat 渐变 结束 RRR,GGG,BBB |  rainbowchat.use  |       Change the gradient color end value       |
| /rc on or off                   |      /rainbowchat 开启 或 关闭      | rainbowchat.admin |      Change the plugin's global switch       |
| /rc rswitch                    |       /rainbowchat 随机开关        | rainbowchat.admin |     Change the random color global switch of the plugin     |
| /rc gswitch                    |       /rainbowchat 渐变开关        | rainbowchat.admin |     Change the plugin's gradient color global switch     |



## Configuration
> Configuration file location: tshock/RainbowChat.json
```json5
{
  "使用说明": "Permission name（rainbowchat.use） /rc Gradient The colors modified with instructions will not be written into the configuration file. What is changed here is the default gradient color of all. Turning on [Random Color] Gradient will be invalid by default", //Instructions for use
  "插件开关": true, //Plug-in switch
  "错误提醒": true, //Error report
  "进服自动开启渐变色": true, //Automatically turn on the gradient color when entering the server
  "全局随机色开关": true, //Global random color switch
  "全局渐变色开关": true, //Global gradient switch
  "修改渐变开始颜色": { //Modify the gradient start color
    "R": 166,
    "G": 213,
    "B": 234
  },
  "修改渐变结束颜色": { //Modify the gradient end color
    "R": 245,
    "G": 247,
    "B": 175
  }
}
```

## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
