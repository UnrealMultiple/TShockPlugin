# AutoReset / OneKeyReset

- Authors: cc04 & Leader & 棱镜 & Cai & 肝帝熙恩
- Fully automatic reset plugin, fully customizable reset, freeing your hands.

## Commands

| Command     | permission |            Details            |
|-------------| :-----------------: |:-----------------------------:|
| /reset	     | reset.admin      | Generating map and reset data |
| /resetdata	 | reset.admin      |        Only reset data        |
| /rs	        | reset.admin      |         Reset Setting         |


## Config
> Configuration file location：tshock/AutoReset/reset_config.json
```json
{
  "替换文件": { //Replace Files
    "/tshock/原神.json": "原神.json", // Replace /tshock/原神.json with 原神.json
    "/tshock/XSB数据缓存.json": "" // Delete /tshock/XSB数据缓存.json
  },
  "击杀重置": { // Kill to reset setting
    "击杀重置开关": false, // Enable or disable the kill to reset feature
    "已击杀次数": 0, // Current kill count
    "生物ID": 50, // NPC ID of the creature to be killed
    "需要击杀次数": 50 // Number of kills required to trigger the reset
  },
  "重置后指令": [ // Commands to execute after reset
    "/reload", // Reload the server
    "/初始化进度补给箱", 
    "/rpg reset"
  ],
  "重置前指令": [ // Commands to execute before reset
    "/结算金币" 
  ],
  "重置后SQL命令": [ // SQL commands to execute after reset
    "DELETE FROM tsCharacter" // Delete all characters from the tsCharacter table
  ],
  "地图预设": { // Map presets
    "地图名": null, // Name of the map
    "地图种子": null // Seed of the map
  },
  "重置提醒": false, // Enable or disable CaiBot reset reminders (CaiBot is a QQ Group BOT)
  "CaiBot服务器令牌": "西江超级可爱喵" // CaiBot server token
}
```

## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
