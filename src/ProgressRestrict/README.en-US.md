# ProgressRestrict

- Author: 少司命 & 恋恋魔改三合一
- Source: github
- Exceeded progress detection
- Items, projectile, and buff can be detected over progress

> [!NOTE]  
> Need to install the pre-plug-in: DataSync (this repository) 

## Instruction

| Command |             Permissions             |       Description        |
|----|:--------------------------:|:---------------:|
| none  |    progress.item.white     |   Exceed the progress item to check white name permissions   |
| none  | progress.projecttile.white |   Exceed progress barrage check white name permissions   |
| none  |    progress.buff.white     | Exceed progress Buff Check white name permissions |

## Configuration
> Configuration file location: tshock/超进度检测.json
```json5
{
  "惩罚违规": true, //Punishment of violation
  "惩罚Debuff时长": 5, //Duration of punishment for Debuff
  "公示违规者": true, //Public violation notification
  "写入日志": true, //Write to log
  "清除违规物品": true, //Clear illegal items
  "清除违规状态": true, //Clear illegal buff
  "踢出违规玩家": false, //Kick out the illegal player
  "限制列表": [ //Restriction list
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "史莱姆王", //Corresponding progress //King Slime
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "克苏鲁之眼", //Corresponding progress //Eye of Cthulhu
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "邪恶Boss", //Corresponding progress //Evil Boss
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "蜂后", //Corresponding progress //Queen Bee
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "骷髅王", //Corresponding progress //Skeletron
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "肉山", //Corresponding progress //Wall of Flesh
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "任一三王", //Corresponding progress //Any of mech boss
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "双子魔眼", //Corresponding progress //Twins
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "机械毁灭者", //Corresponding progress //Destroyer
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "机械骷髅", //Corresponding progress //Skeletron Prime
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "世纪之花", //Corresponding progress //Plantera
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "石巨人", //Corresponding progress //Golem
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "猪鲨公爵", //Corresponding progress //Duke Fishron
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "拜月教徒", //Corresponding progress //Lunatic Cultist
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "月球领主", //Corresponding progress //Moon Lord
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "日耀塔", //Corresponding progress //Solar Pillar
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "星旋塔", //Corresponding progress //Vortex Pillar
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "星云塔", //Corresponding progress //Nebula Pillar
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "星尘塔", //Corresponding progress //Stardust Pillar
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "冰雪女王", //Corresponding progress //Ice Queen
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "圣诞坦克", //Corresponding progress //Christmas Tank
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "永恒尖啸", //Corresponding progress //PumpKing
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "哀嚎之木", //Corresponding progress //Mourning Wood
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "史莱姆皇后", //Corresponding progress //Queen Slime
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "独眼巨鹿", //Corresponding progress //Deerclops
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "光之女皇", //Corresponding progress //Empress of Light
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "血月", //Corresponding progress //Blood Moon
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "困难血月", //Corresponding progress //Hardmode Blood Moon
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "日食", //Corresponding progress //Solar Eclips
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "三王后日食", //Corresponding progress //Solar Eclips Before Plantera
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "花后日食", //Corresponding progress //Solar Eclipse After Plantera
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "南瓜月", //Corresponding progress //Pumpkin Moon
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "霜月", //Corresponding progress //Frost Moon
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "火星暴乱", //Corresponding progress //Martians Madness
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "旧日军团", //Corresponding progress //Old On Army
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "哥布林军团", //Corresponding progress //Goblin army
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "困难哥布林军团", //Corresponding progress //Hardmode Goblin Army
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "海盗军团", //Corresponding progress //Pirate Invasion
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "雪人军团", //Corresponding progress //Frost Legion
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "一阶旧日军团", //Corresponding progress //Tier 1 Old On Army Pre Hardmode
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "二阶旧日军团", //Corresponding progress //Tier 2 Old On Army After Mech Boss Defeated
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "三阶旧日军团", //Corresponding progress //Tier 3 Old On Army After Golem Defeated
      "跨服解禁": false //Cross-server ban
    },
    {
      "限制物品": [], //Restricted items
      "限制弹幕": [], //Restricted projectile
      "限制状态": [], //Restricted buff
      "对应进度": "不存在的进度", //Corresponding progress //Non-existent progress
      "跨服解禁": false //Cross-server ban
    }
  ]
}
```

## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
