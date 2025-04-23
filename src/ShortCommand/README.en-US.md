# ShortCommand

- Author: GK、少司命、羽学
- Source: QQ Group（232109072）
- This is a Tshock server plugin, mainly used to map the plugin or native Tshock command into a command you want
- And you can completely disable a command through【阻止原始】in the configuration file, adding the Reload reload configuration file method
- It is equipped with a permission name【免检指令】to prevent specific user groups from being affected by [Block Original]


## Instruction

| Command |  Permissions  |      Description       |
|----|:----:|:-------------:|
| None  | 免禁指令 | The plugin does not prohibit the use of original commands for it |

## Configuration
> Configuration file location: tshock/简短指令.json
```json5
{
  "命令表": [ //Command table
    {
      "原始命令": "执行", //Original command
      "新的命令": "重置玩家数据", //New Command
      "余段补充": true, //Supplementary
      "阻止原始": true, //Block the original
      "Restrictions": 0, //Restrictions
      "冷却秒数": 0, //Cooldown second
      "冷却共享": false //Cooldown sharing
    },
    {
      "原始命令": "pskill list", //Original command
      "新的命令": "被动书店", //New command
      "余段补充": true, //Supplementary
      "阻止原始": false, //Block original
      "限制条件": 0, //Restrictions
      "冷却秒数": 0, //Cooldown second
      "冷却共享": false //Cooldown sharing
    }
  ]
}
```

## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
