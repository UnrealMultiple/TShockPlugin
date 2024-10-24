# DamageRuleLoot

- Authors: Yu Xue、West River Kid
- Source: Tshock Official QQ Group 816771079
- Description: This plugin determines whether a loot bag is dropped based on the percentage of damage dealt by players, built upon a damage statistics plugin with secondary creation.

## Changelog

```
v1.3.1
对代码做了重复部分做成统一方法整理归纳
修复了自定义转移伤害表里的【涵盖暴击】配置项
给自定义转移伤害加了更多配置项：
【怪物名称】/reload时自动根据【受伤怪物】的ID写入
【最低转伤】触发转发伤害的最低下限
【最高转伤】触发转发伤害的最高上限
【播报排名】是否播报【受伤怪物】的输出排名
移除了【是否排除计算石巨人头部伤害】配置项

v1.3.0
给伤害榜标题加了渐变色
所有转移伤害都视为真伤，加入了自定义转移伤害表
加入了攻击小鬼与饿鬼给肉山造成真实伤害配置项
将机械骷髅王计入四肢伤害视为虚值改为造成真伤
给转移伤害加了雕像怪判断
给杀怪建表加了城镇npc、雕像怪、假人判断
自定义转移伤害表有自己的伤害统计逻辑（非真实血量数值）

v1.2.3
加入了对火星飞船的特殊处理
加入了美杜莎的判定与特殊处理
加入了开发者专用的暴击监控配置项
加入忽略石巨人头部伤害配置项
加入计算机械骷髅王四肢伤害配置项（虚标）
加入攻击鲨鱼龙给猪鲨造成真实伤害配置项

v1.2.2
再次重构《伤怪建表法》，使伤害更接近准确数值
加入对暴击连续统计播报与怯战人数播报

v1.2.1
加入对暴击伤害计数法来归纳玩家的真实伤害
将广告内容放到了Config方便玩家自定义

v1.2.0
重构全部代码，以枳的伤害统计插件作为基础二次开发

对各别分体化的BOSS伤害输出做了特殊处理
美化了输出榜播报内容
加入了额外伤害榜NPC扩展项
加入了惩罚榜与伤害榜的独立开关配置项

v1.1.0
移除了大部分不需要的参数
把《玩家输出表》转换成了字典键值方便参考观看
优化了多BOSS场景下也能判断宝藏袋掉落

v1.0.0
从伤害统计插件基础上进行二创的伤害规则掉落插件
新玩家进服会自动创建【玩家数据表】，如果玩家已经在配置里则会清空【伤害值】
玩家对BOSS的【伤害百分比】超过【领取条件】的百分比才能捡到【物品ID】内的物品

```

## Commands

| Syntax                             | Aliases  |       Permission	       |                   Description                   |
| -------------------------------- | :---: | :--------------: | :--------------------------------------: |
| /reload  | None |   tshock.cfg.reload    |    Reloads the configuration file.    |

---
Main Configuration Notes
---
1.Players must exceed the `Below This Percentage No Loot Bag Drops` for their `Damage Percentage` against the boss to pick up the `Loot Bag`.
2.Monsters listed under `Non-Boss Monster Names Participating in Damage Leaderboard` do not participate in punishment ranking broadcasts; 
3.the `Martian Saucer` and `Pirate Ship` have been handled separately, do not delete them.
4.The `Punishment Ranking` broadcast is only associated with bosses that drop loot bags.
5.`Monitor Critical Hit Count` will monitor all players generating critical hits, while `Monitor Damage Transfer` monitors the status of damage transfer; this feature is developer-specific and should not be enabled, otherwise, it will flood the screen.
6.`Tophaze New Three Kings Stat as Medusa Damage Ranking` is only valid in the Tophaze world; if disabled, it will separately broadcast the damage received by each of the new three kings in the Tophaze world. Conversely, it broadcasts overall damage to Medusa; this setting does not affect normal worlds.
7.`Real Damage Caused by Attacking Mechanical Skeletron Limbs` will cause damage numbers to exceed the boss's HP but also allows limb damage.
8.`Real Damage Caused by Attacking Imps and Hungry Souls to Meat Mountain` only works in FTW and Tophaze seeds, causing damage numbers to exceed the boss's HP.
9.If no punishment participation is required, you can disable `Whether Punish`.

---
Custom Damage Transfer Table Notes
---
The `Custom Damage Transfer Table` is located at the end of the list []. Enter , {} and use the /reload command to get a new format (with preset parameters).

`Damage Transfer` only triggers when any player hits the boss once.

`Monster Name` automatically writes the NPC ID of the `Injured Monster` when using the /reload command, no need to fill manually.

`Stop Transfer HP` is based on the `Injured Monster` reaching a certain HP level before stopping the transfer of damage.

`Minimum Transfer Damage` is the lowest threshold for triggering damage transfer.

`Maximum Transfer Damage` is the upper limit for intercepting damage transfer.

`Include Critical Hits` transfers all damage dealt to the `Transferred Damage Monster`; disable to exclude all critical hit damage from transfer.

`Broadcast Ranking` provides ranking information based on the `Injured Monster`.

`Damage Value into Ranking` includes the damage dealt to the `Transferred Damage Monster` into the output leaderboard.

## Configuration
> Configuration file location：tshock/伤害规则掉落.json
```json
{
  "插件开关": true,
  "是否惩罚": true,
  "广告开关": true,
  "广告内容": "[i:3456][C/F2F2C7:插件开发] [C/BFDFEA:by]  羽学 [C/E7A5CC:|] [c/00FFFF:西江小子][i:3459]",
  "伤害榜播报": true,
  "惩罚榜播报": true,
  "低于多少不掉宝藏袋": 0.15,
  "天顶新三王统计为美杜莎伤害榜": true,
  "忽略计算石巨人头部输出榜伤害": false,
  "攻击机械吴克四肢造成真实伤害": true,
  "攻击鲨鱼龙给猪鲨造成真实伤害": true,
  "攻击小鬼与饿鬼给肉山造成真伤(仅FTW与天顶)": true,
  "参与伤害榜的非BOSS怪名称": [
    "冰雪巨人",
    "沙尘精",
    "腐化宝箱怪",
    "猩红宝箱怪",
    "神圣宝箱怪",
    "黑暗魔法师",
    "食人魔",
    "哥布林术士",
    "荷兰飞盗船",
    "恐惧鹦鹉螺",
    "血浆哥布林鲨鱼",
    "血鳗鱼",
    "海盗船长",
    "火星飞碟"
  ],
  "监控暴击次数": false,
  "监控转移伤害": false,
  "自定义转移伤害": true,
  "自定义转移伤害表": [
    {
      "怪物名称": "克苏鲁之眼",
      "受伤怪物": 4,
      "停转血量": 600,
      "最低转伤": 1,
      "最高转伤": 200,
      "涵盖暴击": false,
      "播报排名": true,
      "伤值进榜": true,
      "转伤怪物": [
        5
      ]
    },
    {
      "怪物名称": "史莱姆王",
      "受伤怪物": 50,
      "停转血量": 800,
      "最低转伤": 1,
      "最高转伤": 200,
      "涵盖暴击": true,
      "播报排名": true,
      "伤值进榜": true,
      "转伤怪物": [
        1,
        535
      ]
    },
    {
      "怪物名称": "世纪之花",
      "受伤怪物": 262,
      "停转血量": 10000,
      "最低转伤": 1,
      "最高转伤": 1000,
      "涵盖暴击": true,
      "播报排名": true,
      "伤值进榜": true,
      "转伤怪物": [
        264
      ]
    }
  ]
}
```
## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love