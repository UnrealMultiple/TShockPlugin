# GolfRewards

- author: GK 阁下 由 鸽子 定制，肝帝熙恩更新适配1449
- source: TShock Chinese official group
- Players hit the ball in the golf hole and receive corresponding rewards based on the number of shots (number of strokes).
- The plugin supports custom reward tables, including item rewards and executed commands.


## Instruction

| Command    |  Permissions  |     Description      |
|-------|:----:|:-----------:|
| /物块坐标 或 /blockpos | blockpos | Auxiliary command to determine block coordinates |

## Configuration
> Configuration file location: tshock/高尔夫奖励.json
Each reward node (`Bonus Section`) contains the following properties:

- `Hole coordinates X`: The coordinates of the hole on the X-axis.
- `Hole coordinate Y`: The coordinate of the hole on the Y axis.
- `Minimum number of strokes`: The minimum number of strokes required for a player to hit the ball into the hole.
- `Maximum number of strokes`: The maximum number of strokes required for a player to hit the hole.
- `Only receive this reward at this location`: If it is `true`, the player can only receive the reward once at this location.
- `Item Section`: A list containing the item rewards available to the player. Each item reward contains the item ID, quantity and item prefix.
- `Command section`: A list containing the commands that the player may execute after a successful shot.

```json5
{
  "奖励表": [ //Reward table
    {
      "球洞坐标X": 0, //Hole coordinates X
      "球洞坐标Y": 0, //Hole coordinate Y
      "最少杆数": 0, //Minimum strokes
      "最多杆数": 999, //Maximum strokes
      "本位置仅领取该奖励": false, //Only receive this reward at this location
      "物品节": [ //Item Section
        {
          "物品ID": 757, //Item ID
          "物品数量": 1, //Item quantity
          "物品前缀": 0 //Item prefix
        }
      ],
      "指令节": [ //Command section
        "/time noon", //Commands to execute, such as setting the game time to noon
        "/time night" //Another possible command to set the game time to night
      ],
      "命中提示": "高尔夫击中奖励球" //Hit prompt //Prompt message after the player successfully hits the ball
    }
  ]
}
```

## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
