# SwitchCommand

- Author: Johuan ,Cjx，羽学
- Source: None
- This is a Tshock server plug-in mainly used to: give command capabilities to "switches", ignore player permissions, customize cooldown times, explain switch functions and other execution methods, etc.
- You can use the placeholder `$name`, which will be replaced by the player using the switch
- How to use:
- 1.Place the switch on the wall Use command: /kg
- 2.Then use /kg add god
- 3.Last /kg wc
- You will get a command switch that can turn on invincible
- If you need to ignore permissions, you can send it in the second step: /kg qxhl
- The same is true for other instructions, make sure the last step is /kg wc

## Instruction

| Command                                         |        Permissions         |              Description               |
|--------------------------------------------|:-----------------:|:-----------------------------:|
| /switch(switch) <add/list/delete/cool/description/permissions/cancel/rebind/complete> |   switch.admin    |            Use switch command             |
| /kg add list del lq sm qxhl qx cb wc       |   switch.admin    |            Use switch command             |
| /switch ignoreperm  <true/false>           | switch.ignoreperm | Independent permissions. Only when you have this permission can you give the command switch the ability to ignore permissions. |

## Configuration
> Configuration file location: tshock/开关配置表.json
```json5
{
  "是否开启开关保护": true, //Turn on switch protection
  "试图破坏开关的警告": "You do not have permission to destroy the command switch!", //Warnings to try to destroy the switch
  "开关指令表": { //Switch command list
    "X: 1984, Y: 226": {
      "指令": [ //instruction
        "/god"
      ],
      "冷却时间": 0.0, //Cooldown time
      "忽略权限": true, //Ignore permissions
      "开关说明": "I'm invincible! ! !" //Switch instructions
    },
    "X: 1985, Y: 226": {
      "指令": [ //instruction
        "/butcher"
      ],
      "冷却时间": 0.0, //Cooldown time
      "忽略权限": false, //Ignore permissions
      "开关说明": "Let's clear and weird~" //Switch instructions
    }
  }
}
```

## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
