# AutoReset / OneKeyReset

- Authors: cc04 & Leader & 棱镜 & Cai & 肝帝熙恩
- 完全自动重置插件,自定义要重置什么

## Commands

| 语法          |          Permission         |        说明       |
| ----------- | :-------------------------: | :-------------: |
| /reset      | reset.admin |       重置世界      |
| /resetdata  | reset.admin | Only reset data |
| /rs 或 /重置设置 | reset.admin |  Reset Setting  |

## Config

> Configuration file location：tshock/AutoReset/AutoReset.en-US.json.json
> Replace Files: tshock/AutoReset/ReplaceFiles

```json5
{
  "ReplaceFiles": { //Replace Files
    "/tshock/原神.json": "原神.json", // Replace /tshock/原神.json with 原神.json
    "/tshock/XSB数据缓存.json": "" // Delete /tshock/XSB数据缓存.json
  },
  "KillToReset": { // Kill to reset setting
    "KillResetEnable": false, // Enable or disable the kill to reset feature
    "CurrentKillCount": 0, // Current kill count
    "NpcId": 50, // NPC ID of the creature to be killed
    "NeedKillCount": 50 // Number of kills required to trigger the reset
  },
  "AfterResetCommand": [ // Commands to execute after reset
    "/reload", // Reload the server
    "/初始化进度补给箱", 
    "/rpg reset"
  ],
  "BeforeResetCommand": [ // Commands to execute before reset
    "/结算金币" 
  ],
  "AfterResetSQL": [ // SQL commands to execute after reset
    "DELETE FROM tsCharacter" // Delete all characters from the tsCharacter table
  ],
  "WorldSetting": { // Map presets
    "WorldName": null, // Name of the map
    "WorldSeed": null // Seed of the map
  },
  "CaiBotResetCaution": false, // Enable or disable CaiBot reset reminders (CaiBot is a QQ Group BOT)
  "CaiBotToken": "西江超级可爱喵" // CaiBot server token "西江 is absolutely adorable!" 
}
```

## 更新日志

```
v2024.12.8.1 修复配置文件位置错误，在世界名称显示进度
v2024.9.1 添加英文翻译，添加resetdata以重置数据不生成地图
v2024.8.24 尝试完善卸载函数
```

## FeedBack

- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
