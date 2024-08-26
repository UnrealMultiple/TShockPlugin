# GolfRewards golf award

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: GK is customized by pigeons, updated and adapted by Gan Di Xi 'en 1449.
- Source: TShock Chinese official group
- Players will be rewarded according to the number of strokes (the number of strokes) when they hit the ball in the golf hole.
- Plug-ins support custom reward tables, including item rewards and executed commands.

## Update log

```
v1.0.7
完善卸载函数

v1.0.6
完善卸载函数
```

## instruction

|grammar|limit of authority|explain|
| -------------- |:-----------------:|:------:|
|/block coordinates|Block coordinates|Auxiliary command, determine the box coordinates.|

## deploy
> Profile location: tshock/ golf award.json
Each reward node (`Bonus Festival`) contains the following properties:

- `Hole coordinate x`: the coordinates of the hole on the x axis.
- `Hole coordinate y`: the coordinates of the hole on the y axis.
- `Minimum number of rods`: The minimum number of strokes required for a player to hit the ball into the hole.
- `Maximum number of rods`: The maximum number of strokes a player needs to hit the ball into the hole.
- `This position only receives this reward.`: If it is `true`, the player can only receive a reward once in this position.
- `Goods festival`: A list of items that players can get. Each item reward includes item ID, quantity and item prefix.
- `Instruction section`: A list containing the commands that the player may execute after hitting the ball successfully.

```json
{
   "奖励表": [
    {
       "球洞坐标X": 0,
       "球洞坐标Y": 0,
       "最少杆数": 0,
       "最多杆数": 999,
       "本位置仅领取该奖励": false,
       "物品节": [
        {
           "物品ID": 757, // 物品的ID，例如757是泰拉刀
           "物品数量": 1, // 物品的数量
           "物品前缀": 0 // 物品的前缀，0表示无前缀
        }
      ],
       "指令节": [
         "/time noon", // 执行的命令，例如设置游戏时间为中午
         "/time night"  // 另一个可能的命令，设置游戏时间为夜晚
      ],
       "命中提示": "高尔夫击中奖励球" // 玩家击球成功后的提示信息
    }
  ]
}
```
## feedback
- Give priority to issued-> jointly maintained plug-in library: https://github.com/Controllerdestiny/TShockPlugin.
- Second priority: TShock official group: 816771079
- You can't see it with a high probability, but you can: domestic communities trhub.cn, bbstr.net, tr. monika.love.