# CustomMonster 

- Authors: GK & 羽学
- Source: QQ Group 232109072 [CustomMonster](https://github.com/1242509682/CustomMonster)
- This is a Tshock server plugin mainly used for:
- Modifying monsters on the Tshock server,
- Customize modifications to monsters and BOSSes including AI, health,
- bullet patterns, drop items, area-of-effect buffs,
- summons, pulling players,
- defenses, invincibility, magma immunity,
- trap immunity, controlling specified monster counts, NPC protection, etc.
- (Comes with built-in configuration as a reference)

## Update Log

```
December 16, 2024
Integrated the latest version 1.0.4.38 by GK

October 14, 2024
Integrated the latest version 1.0.4.36 by GK

June 12, 2024
Removed the no-config version and added the embedded version source code by Yu Xue

June 6, 2024
Fixed error prompts in the no-config version
And added to both the embedded version and the no-config version: /reload reloads only reporting line numbers of errors

June 5, 2024
Added ["Custom Forced Hidden Config Items"]:
When ["Hide Unused Config Items"] is enabled, it triggers.
Without custom hidden items, it will only ignore empty and default values,
But if you write a custom hidden item, even if it has been modified, it will be forcibly hidden after /reload, and the specified configuration item will revert to its default value (there's a risk of breaking configurations, not recommended to change this)
For example, if you set 【Monster Health】to 20000, and then add 【Monster Health】as a keyword in the forced hidden items, after /reload it will reset to 0.

Adjustments in the embedded version by Yu Xue:
Modified the "Shopkeeper" configuration file,
Removed Four Eyes Demon, added Enhanced Slime, Spiked Slime, Shadow Flame Devil, Rain Cloud Fiend modification
Adjusted all BOSS individual health to 1.5-2 times
Fixed issues caused by the original plugin preset configuration automatically filling unmodified settings such as knockback range, pull range, etc.
Fixed issues where setting 【Player Kill Coefficient -10】/【Spawn Duration 600】caused BOSSes to not spawn or disappear prematurely.

June 4, 2024
Added ["Hide Unused Config Items"] switch
Added command: /editmonster to control ["Hide Unused Config Items"]:
This switch can hide unused config items with empty or default values.
Added the "Shopkeeper" configuration file
Related NPC modifications: Light Maiden, Wyvern, Queen Moon's Four Eyes Demon, Snow Giant,
Dust Imp, Armored Phantom, Demon Eye, Cursed Hammer, Crimson Axe, Enchanted Sword, Bone Serpent,
Holy Chest Monster, Snow Queen, Flying Eye, Goblin Mage, Goblin Summoner

Version 1.0.4.9  
Added death execution commands, pull range, and other configuration items
This version's source code is for the no-embedded-config-file version, suitable for direct writing.
```

## Commands

| Command Syntax	                             | Alias  |       Permission       |                   Description                   |
| -------------------------------- | :---: | :--------------: | :--------------------------------------: |
| /ggw  | None  |   CustomMonster    |    Hide extra configuration items and reload    |
| /smc  |  None  |   CustomMonster    |    Summon a flagged monster    |
| /reload  | None |   CustomMonster    |    Reload the configuration file    |

## Configuration
> Configuration file location： tshock/CustomMonster.json
```json
{
  "是否隐藏没用到配置项的指令/ggw": false,
  "自定义强制隐藏哪些配置项的指令/Reload": [
    "覆盖原血量",
    "不低于正常",
    "不小于正常",
    "覆盖原强化",
    "出没率子",
    "出没率母",
    "智慧机制",
    "出场放音",
    "死亡放音",
    "播放声音"
  ],
  "启动错误报告": false,
  "启动死亡队友视角": false,
  "队友视角仅BOSS时": true,
  "队友视角流畅度": -1,
  "队友视角等待范围": 18,
  "统一对怪物伤害修正": 1.0,
  "统一怪物弹幕伤害修正": 1.0,
  "统一初始怪物玩家系数": 0,
  "统一初始玩家系数不低于人数": true,
  "统一初始怪物强化系数": 0.0,
  "统一怪物血量倍数": 1.0,
  "统一血量不低于正常": true,
  "统一怪物强化倍数": 1.0,
  "统一强化不低于正常": true,
  "统一免疫熔岩类型": 1,
  "统一免疫陷阱类型": 0,
  "统一设置例外怪物": [
    1,
    2,
    3
  ],
  "启动动态血量上限": true,
  "启动怪物时间限制": true,
  "启动动态时间限制": true,
  "怪物节集": [
    {
      "标志": "1",
      "怪物ID": 0,
      "再匹配": [
        1
      ],
      "初始属性玩家系数": 0,
      "初始属性强化系数": 0.0,
      "初始属性对怪物伤害修正": 1.0,
      "怪物血量": 0,
      "玩家系数": 0,
      "开服系数": 0,
      "杀数系数": 0,
      "开服时间型": 0,
      "覆盖原血量": true,
      "不低于正常": true,
      "强化系数": 0.0,
      "不小于正常": true,
      "玩家强化系数": 0.0,
      "开服强化系数": 0.0,
      "杀数强化系数": 0.0,
      "覆盖原强化": true,
      "玩家复活时间": -1,
      "阻止传送器": 0,
      "出没秒数": 0,
      "人秒系数": 0,
      "开服系秒": 0,
      "杀数系秒": 0,
      "出没率子": 1,
      "出没率母": 1,
      "杀数条件": 0,
      "数量条件": 0,
      "人数条件": 0,
      "昼夜条件": 0,
      "肉山条件": 0,
      "巨人条件": 0,
      "血月条件": 0,
      "月总条件": 0,
      "开服条件": 0,
      "怪物条件": [
        {
          "怪物ID": 0,
          "查标志": "1",
          "指示物": [
            {
              "名称": "1",
              "数量": 0
            }
          ],
          "范围内": 0,
          "血量比": 0,
          "符合数": 0
        }
      ],
      "智慧机制": -1,
      "免疫熔岩": 0,
      "免疫陷阱": 0,
      "能够穿墙": 0,
      "无视重力": 0,
      "设为老怪": 0,
      "修改防御": false,
      "怪物防御": 0,
      "怪物无敌": 0,
      "自定缀称": "",
      "出场喊话": "",
      "死亡喊话": "",
      "不宣读信息": false,
      "状态范围": 100,
      "周围状态": [
        {
          "状态ID": 0,
          "状态起始范围": 0,
          "状态结束范围": 0,
          "头顶提示": "1"
        }
      ],
      "死状范围": 100,
      "死亡状态": {
        "1": 1
      },
      "出场弹幕": [
        {
          "弹幕ID": 0,
          "X轴偏移": 0.0,
          "Y轴偏移": 0.0,
          "X轴速度": 0.0,
          "Y轴速度": 0.0,
          "弹幕伤害": 0,
          "弹幕击退": 0,
          "弹幕Ai0": 0.0,
          "速度注入AI0": false,
          "弹幕Ai1": 0.0,
          "弹幕Ai2": 0.0,
          "角度偏移": 0.0,
          "指示物数量注入X轴偏移名": "",
          "指示物数量注入X轴偏移系数": 0.0,
          "指示物数量注入Y轴偏移名": "",
          "指示物数量注入Y轴偏移系数": 0.0,
          "指示物数量注入X轴速度名": "",
          "指示物数量注入X轴速度系数": 0.0,
          "指示物数量注入Y轴速度名": "",
          "指示物数量注入Y轴速度系数": 0.0,
          "指示物数量注入角度名": "",
          "指示物数量注入角度系数": 0.0,
          "怪面向X偏移修正": 0.0,
          "怪面向Y偏移修正": 0.0,
          "怪面向X速度修正": 0.0,
          "怪面向Y速度修正": 0.0,
          "不射原始": false,
          "差度位始角": 0,
          "差度位射数": 0,
          "差度位射角": 0,
          "差度位半径": 0,
          "不射差度位": false,
          "差度射数": 0,
          "差度射角": 0.0,
          "差位射数": 0,
          "差位偏移X": 0.0,
          "差位偏移Y": 0.0,
          "锁定范围": 0,
          "锁定速度": 0.0,
          "以弹为位": false,
          "持续时间": -1,
          "计入仇恨": false,
          "锁定血少": false,
          "锁定低防": false,
          "仅攻击对象": false,
          "逆仇恨锁定": false,
          "逆血量锁定": false,
          "逆防御锁定": false,
          "仅扇形锁定": false,
          "扇形半偏角": 60,
          "以锁定为点": false,
          "最大锁定数": 1
        }
      ],
      "死亡弹幕": [
        {
          "弹幕ID": 0,
          "X轴偏移": 0.0,
          "Y轴偏移": 0.0,
          "X轴速度": 0.0,
          "Y轴速度": 0.0,
          "弹幕伤害": 0,
          "弹幕击退": 0,
          "弹幕Ai0": 0.0,
          "速度注入AI0": false,
          "弹幕Ai1": 0.0,
          "弹幕Ai2": 0.0,
          "角度偏移": 0.0,
          "指示物数量注入X轴偏移名": "",
          "指示物数量注入X轴偏移系数": 0.0,
          "指示物数量注入Y轴偏移名": "",
          "指示物数量注入Y轴偏移系数": 0.0,
          "指示物数量注入X轴速度名": "",
          "指示物数量注入X轴速度系数": 0.0,
          "指示物数量注入Y轴速度名": "",
          "指示物数量注入Y轴速度系数": 0.0,
          "指示物数量注入角度名": "",
          "指示物数量注入角度系数": 0.0,
          "怪面向X偏移修正": 0.0,
          "怪面向Y偏移修正": 0.0,
          "怪面向X速度修正": 0.0,
          "怪面向Y速度修正": 0.0,
          "不射原始": false,
          "差度位始角": 0,
          "差度位射数": 0,
          "差度位射角": 0,
          "差度位半径": 0,
          "不射差度位": false,
          "差度射数": 0,
          "差度射角": 0.0,
          "差位射数": 0,
          "差位偏移X": 0.0,
          "差位偏移Y": 0.0,
          "锁定范围": 0,
          "锁定速度": 0.0,
          "以弹为位": false,
          "持续时间": -1,
          "计入仇恨": false,
          "锁定血少": false,
          "锁定低防": false,
          "仅攻击对象": false,
          "逆仇恨锁定": false,
          "逆血量锁定": false,
          "逆防御锁定": false,
          "仅扇形锁定": false,
          "扇形半偏角": 60,
          "以锁定为点": false,
          "最大锁定数": 1
        }
      ],
      "出场放音": [
        {
          "声音ID": -1,
          "声音规模": -1.0,
          "高音补偿": -1.0
        }
      ],
      "死亡放音": [
        {
          "声音ID": -1,
          "声音规模": -1.0,
          "高音补偿": -1.0
        }
      ],
      "出场伤怪": [
        {
          "怪物ID": 0,
          "范围内": 0,
          "造成伤害": 0,
          "直接伤害": false,
          "直接清除": false
        }
      ],
      "死亡伤怪": [
        {
          "怪物ID": 0,
          "范围内": 0,
          "造成伤害": 0,
          "直接伤害": false,
          "直接清除": false
        }
      ],
      "出场命令": [
        "/who"
      ],
      "死亡命令": [
        "/who"
      ],
      "血事件限": 1,
      "血量事件": [
        {
          "血量剩余比例": 0,
          "可触发次": 0,
          "触发率子": 1,
          "触发率母": 1,
          "杀数条件": 0,
          "人数条件": 0,
          "杀死条件": 0,
          "昼夜条件": 0,
          "耗时条件": 0,
          "ID条件": 0,
          "肉山条件": 0,
          "巨人条件": 0,
          "血月条件": 0,
          "月总条件": 0,
          "开服条件": 0,
          "X轴条件": 0,
          "Y轴条件": 0,
          "面向条件": 0,
          "跳出事件": false,
          "怪物条件": [
            {
              "怪物ID": 0,
              "查标志": "1",
              "指示物": [
                {
                  "名称": "1",
                  "数量": 0
                }
              ],
              "范围内": 0,
              "血量比": 0,
              "符合数": 0
            }
          ],
          "玩家条件": [
            {
              "范围起": 0,
              "范围内": 0,
              "符合数": 0
            }
          ],
          "直接撤退": false,
          "玩家复活时间": -2,
          "切换智慧": -1,
          "能够穿墙": 0,
          "无视重力": 0,
          "修改防御": false,
          "怪物防御": 0,
          "恢复血量": 0,
          "比例回血": 0,
          "怪物无敌": 0,
          "拉取起始": 0,
          "拉取范围": 0,
          "拉取止点": 0,
          "拉取点X轴偏移": 0.0,
          "拉取点Y轴偏移": 0.0,
          "杀伤范围": 0,
          "杀伤伤害": 0,
          "击退范围": 30,
          "击退力度": 5,
          "释放弹幕": [
            {
              "弹幕ID": 0,
              "X轴偏移": 0.0,
              "Y轴偏移": 0.0,
              "X轴速度": 0.0,
              "Y轴速度": 0.0,
              "弹幕伤害": 0,
              "弹幕击退": 0,
              "弹幕Ai0": 0.0,
              "速度注入AI0": false,
              "弹幕Ai1": 0.0,
              "弹幕Ai2": 0.0,
              "角度偏移": 0.0,
              "指示物数量注入X轴偏移名": "",
              "指示物数量注入X轴偏移系数": 0.0,
              "指示物数量注入Y轴偏移名": "",
              "指示物数量注入Y轴偏移系数": 0.0,
              "指示物数量注入X轴速度名": "",
              "指示物数量注入X轴速度系数": 0.0,
              "指示物数量注入Y轴速度名": "",
              "指示物数量注入Y轴速度系数": 0.0,
              "指示物数量注入角度名": "",
              "指示物数量注入角度系数": 0.0,
              "怪面向X偏移修正": 0.0,
              "怪面向Y偏移修正": 0.0,
              "怪面向X速度修正": 0.0,
              "怪面向Y速度修正": 0.0,
              "不射原始": false,
              "差度位始角": 0,
              "差度位射数": 0,
              "差度位射角": 0,
              "差度位半径": 0,
              "不射差度位": false,
              "差度射数": 0,
              "差度射角": 0.0,
              "差位射数": 0,
              "差位偏移X": 0.0,
              "差位偏移Y": 0.0,
              "锁定范围": 0,
              "锁定速度": 0.0,
              "以弹为位": false,
              "持续时间": -1,
              "计入仇恨": false,
              "锁定血少": false,
              "锁定低防": false,
              "仅攻击对象": false,
              "逆仇恨锁定": false,
              "逆血量锁定": false,
              "逆防御锁定": false,
              "仅扇形锁定": false,
              "扇形半偏角": 60,
              "以锁定为点": false,
              "最大锁定数": 1
            }
          ],
          "状态范围": 0,
          "周围状态": [
            {
              "状态ID": 0,
              "状态起始范围": 0,
              "状态结束范围": 0,
              "头顶提示": "1"
            }
          ],
          "杀伤怪物": [
            {
              "怪物ID": 0,
              "范围内": 0,
              "造成伤害": 0,
              "直接伤害": false,
              "直接清除": false
            }
          ],
          "召唤怪物": {
            "1": 1
          },
          "喊话": "1"
        }
      ],
      "时事件限": 3,
      "时间事件": [
        {
          "消耗时间": 0.0,
          "循环执行": false,
          "延迟秒数": 0.0,
          "可触发次": 0,
          "触发率子": 1,
          "触发率母": 1,
          "杀数条件": 0,
          "人数条件": 0,
          "昼夜条件": 0,
          "血比条件": 0,
          "杀死条件": 0,
          "怪数条件": 0,
          "耗时条件": 0,
          "ID条件": 0,
          "AI条件": {
            "1": 1.0
          },
          "肉山条件": 0,
          "巨人条件": 0,
          "血月条件": 0,
          "月总条件": 0,
          "开服条件": 0,
          "X轴条件": 0,
          "Y轴条件": 0,
          "面向条件": 0,
          "跳出事件": false,
          "怪物条件": [
            {
              "怪物ID": 0,
              "查标志": "",
              "指示物": [
                {
                  "名称": "1",
                  "数量": 0
                }
              ],
              "范围内": 0,
              "血量比": 0,
              "符合数": 0
            }
          ],
          "玩家条件": [
            {
              "范围起": 0,
              "范围内": 0,
              "符合数": 0
            }
          ],
          "指示物条件": [
            {
              "名称": "1",
              "数量": 0
            }
          ],
          "指示物修改": [
            {
              "清除": false,
              "名称": "1",
              "数量": 0
            }
          ],
          "直接撤退": false,
          "玩家复活时间": -2,
          "阻止传送器": 0,
          "切换智慧": -1,
          "能够穿墙": 0,
          "无视重力": 0,
          "修改防御": false,
          "怪物防御": 0,
          "AI赋值": {
            "1": 1.0
          },
          "恢复血量": 0,
          "比例回血": 0,
          "怪物无敌": 0,
          "拉取起始": 50,
          "拉取范围": 100,
          "拉取止点": 0,
          "拉取点X轴偏移": 0.0,
          "拉取点Y轴偏移": 0.0,
          "击退范围": 30,
          "击退力度": 5,
          "杀伤范围": 0,
          "杀伤伤害": 0,
          "反射范围": 0,
          "释放弹幕": [
            {
              "弹幕ID": 0,
              "X轴偏移": 0.0,
              "Y轴偏移": 0.0,
              "X轴速度": 0.0,
              "Y轴速度": 0.0,
              "弹幕伤害": 0,
              "弹幕击退": 0,
              "弹幕Ai0": 0.0,
              "速度注入AI0": false,
              "弹幕Ai1": 0.0,
              "弹幕Ai2": 0.0,
              "角度偏移": 0.0,
              "指示物数量注入X轴偏移名": "",
              "指示物数量注入X轴偏移系数": 0.0,
              "指示物数量注入Y轴偏移名": "",
              "指示物数量注入Y轴偏移系数": 0.0,
              "指示物数量注入X轴速度名": "",
              "指示物数量注入X轴速度系数": 0.0,
              "指示物数量注入Y轴速度名": "",
              "指示物数量注入Y轴速度系数": 0.0,
              "指示物数量注入角度名": "",
              "指示物数量注入角度系数": 0.0,
              "怪面向X偏移修正": 0.0,
              "怪面向Y偏移修正": 0.0,
              "怪面向X速度修正": 0.0,
              "怪面向Y速度修正": 0.0,
              "不射原始": false,
              "差度位始角": 0,
              "差度位射数": 0,
              "差度位射角": 0,
              "差度位半径": 0,
              "不射差度位": false,
              "差度射数": 0,
              "差度射角": 0.0,
              "差位射数": 0,
              "差位偏移X": 0.0,
              "差位偏移Y": 0.0,
              "锁定范围": 0,
              "锁定速度": 0.0,
              "以弹为位": false,
              "持续时间": -1,
              "计入仇恨": false,
              "锁定血少": false,
              "锁定低防": false,
              "仅攻击对象": false,
              "逆仇恨锁定": false,
              "逆血量锁定": false,
              "逆防御锁定": false,
              "仅扇形锁定": false,
              "扇形半偏角": 60,
              "以锁定为点": false,
              "最大锁定数": 1
            }
          ],
          "状态范围": 0,
          "周围状态": [
            {
              "状态ID": 0,
              "状态起始范围": 0,
              "状态结束范围": 0,
              "头顶提示": "1"
            }
          ],
          "杀伤怪物": [
            {
              "怪物ID": 0,
              "范围内": 0,
              "造成伤害": 0,
              "直接伤害": false,
              "直接清除": false
            }
          ],
          "召唤怪物": {
            "1": 1
          },
          "播放声音": [
            {
              "声音ID": -1,
              "声音规模": -1.0,
              "高音补偿": -1.0
            }
          ],
          "释放命令": [
            "/reload"
          ],
          "喊话": "1"
        }
      ],
      "随从怪物": {
        "1": 1
      },
      "遗言怪物": {
        "1": 1
      },
      "掉落组限": 1,
      "额外掉落": [
        {
          "掉落率子": 1,
          "掉落率母": 1,
          "杀数条件": 0,
          "人数条件": 0,
          "杀死条件": 0,
          "昼夜条件": 0,
          "耗时条件": 0,
          "肉山条件": 0,
          "巨人条件": 0,
          "血月条件": 0,
          "月总条件": 0,
          "开服条件": 0,
          "跳出掉落": false,
          "怪物条件": [
            {
              "怪物ID": 0,
              "查标志": "1",
              "指示物": [
                {
                  "名称": "1",
                  "数量": 0
                }
              ],
              "范围内": 0,
              "血量比": 0,
              "符合数": 0
            }
          ],
          "掉落物品": [
            {
              "物品ID": 0,
              "物品数量": 0,
              "物品前缀": -1,
              "独立掉落": false
            }
          ],
          "备注": "1"
        }
      ],
      "备注": "1"
    }
  ]
}
```
## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
