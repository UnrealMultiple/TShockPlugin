# PlayerSpeed

- Authors: 逆光奔跑 羽学
- Source: TShock QQ Group 816771079
- This is a Tshock server PlayerSpeed, mainly used for：

## Commands

| Syntax     |   别名   |                     Permission                    |               Description               |
| ---------- | :----: | :-----------------------------------------------: | :-------------------------------------: |
| /vel on    |  None  |             vel.admin             |        Enable the plugin function       |
| /vel off   |  None  |             vel.admin             |       Disable the plugin function       |
| /vel set   | /vel s |             vel.admin             | Set global dash speed and cooldown time |
| /vel boss  |  None  |             vel.admin             |      Toggle automatic progress mode     |
| /vel mess  |  None  |             vel.admin             |                开启或关闭播报系统                |
| /vel del   |  None  |             vel.admin             |       Delete specified player data      |
| /vel reset |  None  |             vel.admin             |          Reset all player data          |
| /reload    |  None  | tshock.cfg.reload |        Reload configuration file        |

## Configuration

> Configuration file location： tshock/玩家速度.json

```json5
{
  "Enabled": true,
  "Uses": 5,
  "Interval": 2000.0,
  "CoolDown": 25,
  "Speed": 20.0,
  "Height": 5.0,
  "Broadcast": true,
  "Sprint": true,
  "SprintMultiplier": 1.5,
  "Jump": true,
  "JumpDivideMultiplier": 5.0,
  "JumpBoostItems": [
    5107,
    4989
  ],
  "AutoProgress": true,
  "BossProgressList": [
    {
      "Name": "Eye of Cthulhu, King Slime",
      "Defeated": true,
      "SetSpeed": 20.0,
      "SetHeight": 2.5,
      "Uses": 1,
      "CoolDownTime": 60,
      "NPCIds": [
        4,
        50
      ]
    },
    {
      "Name": "Eater of Worlds, Brain of Cthulhu",
      "Defeated": false,
      "SetSpeed": 25.0,
      "SetHeight": 5.0,
      "Uses": 2,
      "CoolDownTime": 45,
      "NPCIds": [
        13,
        266
      ]
    },
    {
      "Name": "Wall of Flesh",
      "Defeated": false,
      "SetSpeed": 30.0,
      "SetHeight": 10.0,
      "Uses": 3,
      "CoolDownTime": 30,
      "NPCIds": [
        113
      ]
    },
    {
      "Name": "Golem, Destroyer, Skeletron Prime, Plantera",
      "Defeated": false,
      "SetSpeed": 40.0,
      "SetHeight": 15.0,
      "Uses": 4,
      "CoolDownTime": 15,
      "NPCIds": [
        125,
        126,
        127,
        134
      ]
    }
  ]
}
```

## 更新日志

```
v1.2.3
Database改utf-8
v1.2.2
加入自动进度模式,根据击败任意BOSS自动设置速度与冷却等相关数值…(开关指令:/vel boss)
使用自动进度模式记得用指令重置击败记录：/vel reset 
拥有vel.admin管理权限不会进入冷却期(触发间隔不免,防止卡服)
允许vel.use权限的玩家使用/vel 查询当前速度状态
加入了跳跃下降除于倍数配置项(用于辅助坐骑冲刺时优化手感)
加入了Y轴上升下降加速逻辑:
当按住左上或左下时按空格会加速上升与下降
反之只按左或右，直接按空格则往前加速
/vel s指令加入了新属性参数：
高度:h

v1.2.1
重构无限冲逻辑避免性能问题：
满足配置中的`次数`进入冷却时间
并给每次动作设定了间隔时间(毫秒)
移除了`上次跳跃`的相关播报
移除了停止时间（ut）属性
/vel s指令加入了新属性参数：
间隔:r
次数:c

v1.2.0
加入了无限冲刺机制：
1.当使用克盾类饰品双击冲刺不断冲刺
2.装备指定物品时不断使用跳跃
可以刷新无限冲刺的间隔
当冲刺间隔超过《停止无限冲时的毫秒》时自动进入冷却期
/vel s指令加入了新属性参数：
停止时间:ut
添加跳跃物品:add
移除跳跃物品:del

v1.1.0
因群友"哨兵"服主的定制要求修改而来
加入了冲刺判断逻辑和冷却机制
重构了大部分代码与指令方法、触发逻辑
玩家使用权限：vel.use
管理员权限：vel.admin
/vel set 指令格式：
/vel s sd 40 t 10

v1.0.0
从逆光奔跑那反编译来的
```

## FeedBack

- Github Issue -> TShockPlayerSpeed Repo: https://github.com/UnrealMultiple/TShockPlayerSpeed
- TShock QQ Group: 816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
