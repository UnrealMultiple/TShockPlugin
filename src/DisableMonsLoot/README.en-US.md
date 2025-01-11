# DisableMonsLoot prevents monster drops

- author: 羽学
- source: [github](https://gitee.com/Crafty/bean-points)
- The original development intention of the magic modification <monster does not drop money plugin> is to cooperate with the random dropping of items to open up wasteland.
- This plugin cleans up items in the current progress through monster death events.
- If you defeat any monster in [Monster ID], the [Cleaning Function] will be automatically turned off.
- If [Kill All] is turned on, you need to defeat all monsters with [Monster ID] before it will be automatically turned off.
- Type /kdm reset to turn on all [Clean] switches so that they can be restored when resetting the server.

## Instruction

| Command   |       Permissions       |     Description     |
|------|:--------------:|:----------:|
| /kdm | killitem.admin | View the prevents monster drop command menu |

## Configuration
> Configuration file location: tshock/禁怪物掉落.json
```json5
{
  "使用说明": "你只管改‘ID’就行，‘清理’和‘怪物名’不用动，击杀其中1个怪物ID自动关闭清理功能。指令：/kdm (权限killitem.admin)", //Instructions for use //"You only need to change the 'ID'. Leave 'Clean' and 'Monster Name' untouched. Killing one of the monster IDs will automatically turn off the cleaning function. Command:/kdm (permission killitem.admin)
  "插件开关": true, //Plugin switch
  "是否全杀": false, //Whether to kill all
  "清理范围": 35, //Clean range
  "清理列表": [ //Clean list
    {
      "怪物名": "蓝史莱姆, 恶魔眼, 僵尸", //Monster name
      "清理": true, //Clean
      "怪物ID": [ //Monster ID
        1,
        2,
        3
      ],
      "物品ID": [ //Item ID
        71,
        72,
        73,
        74
      ]
    }
  ]
}
```

## Feedback
-- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
￼Enter
