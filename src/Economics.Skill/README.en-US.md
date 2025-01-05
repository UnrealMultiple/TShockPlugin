# Economics.Skill Skill Plugin

- Author: Shaosiming
- Source: None
- A plugin that allows you to release skills.

> [!NOTE]
> requires pre-installed plugins: EconomicsAPI, Economics.RPG (this repository)
> using the AI style function may cause a large amount of bandwidth usage.

## Configuration Notes

- In the trigger mode `击杀` `击打` `主动` three modes, any two of which can not be combined, hit Hit Hit itself is the embodiment of the initiative.

## Commands

| 语法                                                                                                                              |                           权限                          |                       说明                      |
| ------------------------------------------------------------------------------------------------------------------------------- | :---------------------------------------------------: | :-------------------------------------------: |
| /skill buy [技能索引]                                                           |  economics.skill.use  |                      购买技能                     |
| /skill del [技能索引]                                                           |  economics.skill.use  |                      解绑技能                     |
| /skill ms                                                                                                                       |  economics.skill.use  |                    查看已绑定技能                    |
| /skill delall                                                                                                                   |  economics.skill.use  |                  解绑手持武器的所有技能                  |
| /skill clear                                                                                                                    |  economics.skill.use  |                     解绑所有技能                    |
| /skill reset                                                                                                                    | economics.skill.admin |                      重置技能                     |
| /skill give [玩家] [技能索引] | economics.skill.admin | 给予玩家技能(不安全的，此指令不会检查玩家技能情况) |
| /skill del [玩家] [技能索引]  | economics.skill.admin |                    移除目标玩家技能                   |
| /skill clearh [玩家]                                                          | economics.skill.admin |                   移除玩家所有隐藏技能                  |

## Configuration

> configuration file location: tshock/Economics/Skill.json

```json5
{
  "购买主动技能最大数量": 1,
  "单武器绑定最大技能数量": 1,
  "被动绑定最大技能数量": 4,
  "禁止拉怪表": [],
  "最大显示页": 20,
  "技能列表": [
    {
      "名称": "",
      "喊话": "",
      "技能唯一": false,
      "全服唯一": false,
      "技能价格": 0,
      "限制等级": [],
      "限制进度": [],
      "触发设置": {
        "触发模式": [
          "主动" //触发模式 CD 主动 打击 击杀 死亡 血量 蓝量 冲刺 装备 跳跃
        ],
        "冷却": 0,
        "血量": 0,
        "蓝量": 0,
        "物品条件": []
      },
      "伤害敌怪": {
        "伤害": 0,
        "范围": 0
      },
      "范围命令": {
        "命令": [],
        "范围": 0
      },
      "治愈": {
        "血量": 0,
        "魔力": 0,
        "范围": 0
      },
      "清理弹幕": {
        "启用": false,
        "范围": 0
      },
      "拉怪": {
        "X轴调整": 0,
        "Y轴调整": 0,
        "范围": 0
      },
      "传送": {
        "启用": false,
        "X轴位置": 0,
        "Y轴位置": 0
      },
      "无敌": {
        "启用": true, //无敌帧，不保证完全无敌，算是我留的小坑，而且不想改，觉得这样挺好。
        "时长": 2000
      },
      "范围Buff": {
        "Buff列表": [],
        "范围": 0
      },
      "弹幕": [
        {
          "弹幕ID": 132,
          "伤害": 40,
          "击退": 1.0,
          "起始角度": 2,
          "X轴起始位置": 0,
          "Y轴起始位置": 0,
          "X轴速度": 0.0,
          "Y轴速度": 0.0,
          "自动方向": true,
          "持续时间": -1,
          "AI": [
            0.0,
            0.0,
            0.0
          ],
          "AI样式": 0, //目前只有0有效
          "射速": 10.0, 
          "锁定怪物配置": {
            "启用": true,
            "弹幕锁定敌怪": true, 
            "以锁定敌怪为中心": true,
            "锁定血量最少": true, //不开启则锁定距离最近 锁定方式: Boss > 小怪
            "范围": 200
          },
          "延迟": 0,
          "弹幕循环": {
            "配置": [
              {
                "次数": 5,
                "X递增": 0,
                "Y递增": 0,
                "角度递增": 20,
                "圆面半径": 20, //把老版本画圆挪过来了
                "反向发射": false,
                "延迟": 100,
                "跟随玩家位置": false,
                "根据角度计算新的点": true //配合画圆使用
              }
            ]
          }
        }
      ]
    }
  ]
}
```

## 更新日志

```
V2.0.0.0
适配多货币

V1.2.1.6
添加隐藏技能，隐藏技能无法被主动购买。
添加新指令:
/skill give 给玩家添加技能，此指令不是一个安全的指令，它不会检查玩家技能状态。
/skill del 这个指令可以删除目标玩家技能
/skill clearh 移除目标玩家身上的隐藏技能

V1.2.1.5
适配新 EconomicsAPI

V1.1.0.1
添加 无敌帧，锁定怪物，AI样式，传送玩家，移除画圆配置，改用循环实现
修复: 弹幕AI无法生效，持续时间无法生效

V1.0.0.1
修复:物品消耗
```

## FeedBack

- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- 国内社区 trhub.cn 或 TShock 官方群等
