# AutoReset / OneKeyReset

- Authors: cc04 & Leader & 棱镜 & Cai & 肝帝熙恩
- Fully automatic reset plugin, fully customizable reset, freeing your hands.

## Commands

| Command     | Permission  |            Details            |
|-------------|:-----------:|:-----------------------------:|
| /reset	     | reset.admin | Generating map and reset data |
| /resetdata	 | reset.admin |        Only reset data        |
| /rs	        | reset.admin |         Reset Setting         |


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

## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
