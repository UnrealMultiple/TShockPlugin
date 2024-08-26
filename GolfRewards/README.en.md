# Golfrewards golf reward

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: GK will be customized by pigeons, liver Emperor Xien updates and adapts 1449
- Source: TSHOCK Chinese official group
- Players hit the ball in the golf hole and get the corresponding reward based on the number of hitting times (number of rods).
- The plug -in supports the custom reward form, including the item rewards and execution commands.

## Update log

```
v1.0.7
完善卸载函数

v1.0.6
完善卸载函数
```

## instruction

|grammar|Authority|illustrate|
| -------------- |: --------------------:|: -------:|
|/Block coordinates|Block coordinates|Auxiliary instructions, determine the square coordinates|

## Configuration
> Configuration file location: TSHOCK/Golf reward.json
Each reward node (`Reward`) Including the following attributes:

- `Cave coordinates x`: The coordinates of the ball on the X -axis.
- `Cave coordinates y`: The coordinates of the ball on the y -axis.
- `Minimum rod`: The minimum number of poles needed for players to hit the ball.
- `Maximum rod`: The maximum number of rods needed for players to enter the hole.
- `This location only receives the reward`: If it is `true` then players can only receive once rewards at this location.
- `Festival`: A list, which contains the item rewards that the player can get. Each item reward includes items ID, quantity and prefix.
- `Instruction`: A list contains the commands that the player may execute after the player is successful.

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
- Priority to ISSUED-> Commonly maintained plug -in library: https://github.com/Controllerdestiny/tshockPlugin
- Two priority: TSHOCK official group: 816771079
- It may not be seen in a high probability, but it can also be: domestic community trhub.cn, bbstr.net, tr.monika.love