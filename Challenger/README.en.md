# Challenger challenger mode

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: Star Night God Flower, Tale, Yu Xue
- - 出处: [github](https://github.com/skywhale-zhi/Challenger) 
- This is a TSHOCK server plug -in mainly used to modify the magic reform and equipment set of the server BOSSAI, the passive skills of accessories, etc.
- And the player will be pulled out when injured, and the blood bag will give the nearby enemy monsters (BOSS priority). If the player can intercept the blood bag, he can return the blood to himself.
- And allow the blood volume multiple of all monsters by modifying the configuration file. Due to the author's loss of version 1.0.1, Yu Xue compiled the plug -in for secondary open source and update.
- 
- The modified set enhancement effect is:
- Bagding jewelry: Give permanent buff, enable unlimited ammunition
- Mining set: Give permanent mining, candy sprint buff, enable chain mining capabilities
- Fishing set: Give permanent sound, fishing, treasure box, calm buff
- Ninja set: a quarter of probability to avoid non -fatal damage and release smoke
- Fossil set: Call a amber light ball on the head, throw a very fast lightning vector to the enemy
- Shadow set: During the crit, the surrounding of monster missiles to attack the surrounding enemies from the player, cooling for 1 second
- Scarlet set: During the crit, the amount of blood from each enemy around the surrounding surroundings is absorbed by the number of blood volume with the number of enemy monsters.
- Meteorite: Restore a little magic during the crit, intermittently drops high damage to the star to attack the enemy
- Bee condom: Gives permanent honey gain; sprinkle bee sugar cans around uninterruptedly, players return blood and give 15 seconds of honey gain after contact;
The amount of treatment to the player is slightly lower than that of other players
- Jungle cover (ancient cobalt): generate harmful spores from the players intermittently
- Naddonic set: When injured, splash bones around the surroundings; occasionally launch bone arrows when attacking
- Obsidian set: Because of the blessings of the thief, the drop -off will try to drop twice (only effective for non -BOSS creatures and non -high -blood monsters)
- Lava set: immune magma, giving permanent hell fire gain
- Spider sleeve: During the attack, give the enemy poisoning and toxic reducing benefits, press the "UP" key to generate a toothproof drug water bottle, and explode when hitting the enemy "
- Crystal Assassin: When an enemy is nearby, the crystal fragments are released; if the player is hit, it releases a stronger fragment
- Cold frost set: start snowing around (the "Arctic" barrage of the ice and snow queen drops the weapon "Arctic")
- Forbidden ring set: Release the automatic search of the spiritual flame soul fire attack nearby enemies
- Sacred set (ancient sacred): Summon light and dark sword gas when hitting the enemy, enter "/cf" to switch sword air type
- Leaf green sleeve: release inaccurate leaf green crystal vessels, the power of the jungle gives you a higher life limit
- Turtle set: increase the upper limit of 60 blood, automatically release explosion fragments nearby
- Paramia: increase the upper limit of 20 blood, leave spores on the trajectory of the whip
- Beetle cover: increase the 60 blood limit, part of the enemy's injury will treat the surrounding teammates and give BUFF;
When equipped with the Paladin Shield or its superior synthesizer, the damage of melee weapons is doubled
- Mushroom Set: Firemons will leave mushrooms unstablely
- Yinyan: When using a whip, throw out the bat or pumpkin head
- Ghost Set: Add 40 blood maximum or 80 magic upper limit according to the headdress; summon two ghost curse surround players and attack nearby enemies
- Royal Gel: The sky began to undergo light rain in the sky
- Kuuru's shield: Get a short period of invincible time during sprint, cool for 12 seconds
- The chaotic brain: enter "/cf" all the enemy monsters around the chaos
- Porch scarf: immune cold, frost fire, spiritual fluid and spell fire
- Radio parts: Enter "/CF" to listen to the weather forecast, you can listen to the world prophet forecast in the difficult mode
- Bee backpack: throw poison bee tanks around uninterrupted, release a bee after explosion
- Virgin gel: The enemy has a probability of falling down the magic crystal, pearls, gel, etc.
- 
- [Current problems]: The damage caused by the increase will not be considered as caused by players, but it can still kill monsters

## Update log

```
更新日志
1.1.3
补全卸载函数

1.1.2 fix
尝试修复以下三个方法引起的空引用
OnGameUpdate/WormScarf/OnServerLeave

1.1.1
给【皇家凝胶】加了随机生成概率与间隔

1.1.0
给以下套装加入了【弹幕间隔】的配置项：
蜜蜂背包、化石套、丛林套、暗影套、
猩红套、流星套、水晶刺客套、寒霜套、神圣套
海龟套、提基套、阴森套、蘑菇矿套、甲虫套、幽灵套
调整挑战者模式套装默认配置数值，考虑化石套弹幕的特殊性额外加入个【识敌范围】


1.0.9
修复玩家穿套装时退服会刷血量上限BUG
详情看1.0.5更新日志，羽学注释掉了ServerLeave方法引起的

1.0.8
修复了幽灵套永久BUFF
加入了箭袋饰品加强，启用无限弹药
佩戴箭袋类饰品攻击时自动重置弹药数量，无法进行分堆（避免玩家利用特性刷物品）
添加了蠕虫围巾/幽灵套永久BUFF、禁戒套的弹幕频率/范围、甲虫套的弹幕ID
修改了大量默认的配置项参数（主要是肉后的套装）

1.0.7
优化了配置文件的排版次序
加入了“挖矿套是否开启连锁挖矿”配置项
添加了挖矿套的连锁挖矿能力，可配置其buff与连锁图格ID
添加了暗影套、猩红套、黑曜石套、挥发凝胶的配置项

1.0.6
给怪物吸血加入了个上限值（避免养蛊bug）
给冲刺类饰品都配备了闪避效果
增加了蜜蜂背包、化石套、禁戒套、蘑菇套、幽灵套、渔夫套的配置项
修正了一些配置项的错误名字
移除了以下3个配置项：
   "怪物吸血条件:玩家受到多少伤害触发": 1500,
   "怪物吸血条件:给非BOSS小于多少生命值上限的怪回血": 4000.0,
   "怪物弹幕吸血条件:给非BOSS小于多少生命值上限的怪回血": 6000.0,

1.0.5
声明：套装效果下玩家血量不加的问题是没开强制开荒导致的
修复了玩家退出游戏重进时加载时间过长问题
修复了玩家退出时出现的空引用问题

1.0.4
加入了大量配置项（实在太多了 写不过来 直接看配置文件吧）
注意：蠕虫围巾的免疫debuff功能存在BUG，
当遇到需免疫DeBUFF时会清空所有现有BUFF（50号包问题），不建议用

1.0.3
配置项加入了
   "幽灵兜帽是否出幽灵弹幕": true,
   "幽灵面具是否出幽灵弹幕": false,
   "甲虫套受到伤害给其他玩家的回血转换比例/默认30%": 0.3,
   "甲虫套减多少回复量/默认为0": 10,
经玩家反馈1.0.2版幽灵套无限换头会累积弹幕，故此添加修复此BUG

1.0.2
羽学添加了大量配置项，允许自定义开启或修改某些饰品的被动技能
可修改配置项来决定是否开启BOSS_AI的魔改

1.0.1
玩家受伤时会被抽出血包，血包会给附近的敌怪回血（boss优先），若玩家能拦截住血包就能给自己回血
```

## instruction

|grammar|Alias|Authority|illustrate|
| -------------- |: ----------:|: --------------:|: -------:|
|/Finable|none|challenger.enable|Enable the challenge mode and use it again|
|/TIPS|none|challenger.tips|Enable the content prompt, such as the text prompts of various items, use the cancellation again|
|/CF|none|none|Used to switch or trigger equipment passive skills types|

## Configuration
> Configuration file location: TSHOCK/ChallengerConfig.json
```
{
   "是否启用挑战模式": true,
   "是否启用BOSS魔改": false,
   "启用话痨模式": false,
   "启用广播话痨模式": false,
   "是否启用怪物吸血": true,
   "怪物吸血比率": 0.25,
   "怪物吸血比率对于Boss": 0.5,
   "怪物回血上限：小怪>1.5倍则会消失": 1.5,
   "所有怪物血量倍数(仅开启魔改BOSS时生效)": 1.0,
   "冲刺饰品类的闪避冷却时间/默认12秒": 5,
   "蜜蜂背包是否扔毒蜂罐": true,
   "蜜蜂背包首次弹幕ID": 183,
   "蜜蜂背包首次弹幕伤害": 30,
   "蜜蜂背包首次弹幕击退": 10.0,
   "蜜蜂背包生成弹幕概率/分母值": 3,
   "蜜蜂背包弹幕爆炸后的弹幕ID": 566,
   "蜜蜂背包弹幕爆炸后的弹幕伤害": 30,
   "蜜蜂背包弹幕爆炸后的弹幕击退": 0.0,
   "蜜蜂背包二次弹幕间隔/帧": 240,
   "化石套是否出琥珀光球": true,
   "化石套的弹幕ID": 732,
   "化石套的弹幕射程": 48.0,
   "化石套的识敌范围": 3750.0,
   "化石套的弹幕伤害": 25,
   "化石套的弹幕击退": 8.0,
   "化石套的弹幕间隔/帧": 10.0,
   "丛林套是否环绕伤害孢子": true,
   "丛林套弹幕射程/速率": 1.5,
   "丛林套弹幕ID1": 569,
   "丛林套弹幕ID2": 572,
   "丛林套弹幕伤害": 35,
   "丛林套弹幕击退": 8.0,
   "丛林套弹幕间隔/帧": 1,
   "忍者套是否会闪避": true,
   "忍者套闪避概率随机数/0则100%闪避": 2,
   "忍者套闪避释放的弹幕ID": 196,
   "忍者套闪避释放的弹幕伤害": 0,
   "忍者套闪避释放的弹幕击退": 0.0,
   "暗影套的弹幕ID": 307,
   "暗影套的弹幕伤害": 40,
   "暗影套的弹幕击退": 2.0,
   "暗影套的弹幕间隔/帧": 180,
   "猩红套的弹幕ID": 305,
   "猩红套的弹幕伤害": 0,
   "猩红套的弹幕击退": 0.0,
   "猩红套的弹幕间隔/帧": 300,
   "流星套是否下落星": true,
   "流星套的弹幕ID": 725,
   "流星套的弹幕射程": 1000,
   "流星套的弹幕速度/帧": 8.0,
   "流星套的弹幕间隔/帧": 120,
   "死灵套是否产生额外弹幕": true,
   "死灵套受到攻击时的弹幕ID": 532,
   "死灵套受到攻击时的弹幕伤害": 20,
   "死灵套受到攻击时的弹幕击退": 5.0,
   "死灵套的受伤弹幕间隔/帧": 1,
   "死灵套攻击时的弹幕ID": 117,
   "死灵套攻击时的弹幕伤害": 20,
   "死灵套攻击时的弹幕击退": 2.0,
   "死灵套的攻击弹幕间隔/帧": 1,
   "黑曜石套是否盗窃双倍掉落物": true,
   "黑曜石套盗窃的稀有等级": 2,
   "水晶刺客套是否释放水晶碎片": true,
   "水晶刺客套遇怪自动释放的弹幕ID": 90,
   "水晶刺客套受伤释放的弹幕ID": 94,
   "水晶刺客套弹幕间隔/帧": 50,
   "蜘蛛套给NPC施加什么BUFF1": 70,
   "蜘蛛套给NPC施加BUFF1时长": 180,
   "蜘蛛套给NPC施加什么BUFF2": 20,
   "蜘蛛套给NPC施加BUFF2时长": 360,
   "禁戒套是否释放弹幕": true,
   "禁戒套释放什么弹幕ID": 950,
   "禁戒套的弹幕伤害": 125,
   "禁戒套的弹幕频率/分母值": 15,
   "禁戒套的弹幕范围": 300,
   "寒霜套是否下弹幕": true,
   "寒霜套释放什么弹幕ID": 297,
   "寒霜套的弹幕伤害": 40,
   "寒霜套的弹幕击退": 5.0,
   "寒霜套的弹幕间隔/帧": 60,
   "神圣套额外弹幕多少伤害/默认55%": 0.35,
   "神圣套释放什么弹幕ID": 156,
   "神圣套释放什么弹幕ID2": 157,
   "神圣套的弹幕间隔/帧": 2,
   "叶绿套加多少生命上限": 100,
   "海龟套加多少生命上限": 120,
   "海龟套的弹幕ID": 338,
   "海龟套的弹幕伤害": 50,
   "海龟套的弹幕间隔/默认3秒": 3,
   "提基套加多少生命上限": 200,
   "提基套的弹幕ID": 523,
   "提基套的弹幕伤害": 1.2,
   "提基套的弹幕间隔/帧": 2,
   "阴森套是否出弹幕": true,
   "阴森套白天出什么弹幕": 316,
   "阴森套晚上出什么弹幕": 321,
   "阴森套弹幕伤害/默认0.2": 0.8,
   "阴森套的弹幕间隔/帧": 10,
   "蘑菇套是否出弹幕": true,
   "蘑菇套的弹幕ID": 819,
   "蘑菇套的弹幕伤害倍数": 0.6,
   "蘑菇套的弹幕击退": 1.14514,
   "蘑菇套的弹幕间隔/帧": 1,
   "甲虫套加多少生命上限": 180,
   "甲虫套受到伤害给其他玩家的回血转换比例/默认30%": 0.5,
   "甲虫套减多少回复量/默认为0": 15,
   "甲虫套带骑士盾加多少弹幕伤害/默认90%": 1.5,
   "甲虫套的弹幕ID": 866,
   "甲虫套的治疗弹幕间隔/帧": 1,
   "甲虫套的攻击弹幕间隔/帧": 1,
   "皇家凝胶是否下物品雨": true,
   "皇家凝胶物品雨表": [
    75
  ],
   "挥发凝胶击中敌怪掉落物品表": [
    72,
    75,
    501,
    502
  ],
   "幽灵套加多少生命和魔力上限": 80,
   "幽灵兜帽是否出幽灵弹幕": true,
   "幽灵面具是否出幽灵弹幕": false,
   "幽灵套的弹幕ID": 79,
   "幽灵套的弹幕伤害倍数": 50.0,
   "幽灵套的弹幕击退": 0.0,
   "幽灵套环绕的弹幕ID": 299,
   "幽灵套环绕的弹幕伤害": 0,
   "幽灵套环绕的弹幕击退": 0.0,
   "幽灵套的攻击弹幕间隔/帧": 2,
   "幽灵套给什么永久BUFF": [
    6,
    7,
    181,
    178
  ],
   "蜜蜂套是否释放弹幕": true,
   "蜜蜂套的BUFF时长/帧": 150,
   "蜜蜂套的弹幕间隔/帧": 120,
   "蜜蜂套给什么永久BUFF": [
    48
  ],
   "狱岩套给什么永久BUFF": [
    1,
    172
  ],
   "钓鱼套包含哪些永久BUFF": [
    106,
    123,
    121,
    122
  ],
   "挖矿套是否开启连锁挖矿": true,
   "挖矿套给什么永久BUFF": [
    104,
    192
  ],
   "挖矿套连锁图格ID表": [
    6,
    7,
    8,
    9,
    166,
    167,
    168,
    169,
    22,
    221,
    222,
    223,
    224,
    232,
    37,
    404,
    408,
    48,
    481,
    482,
    483,
    56,
    571,
    58,
    63,
    64,
    65,
    66,
    67,
    68,
    107,
    108,
    111,
    123,
    178,
    204,
    211,
    229,
    230
  ],
   "蠕虫围巾免疫buff是否开启": false,
   "蠕虫围巾遍历前几个buff": 22,
   "蠕虫围巾免疫DeBuff列表/遇到会清空所有BUFF": [
    39,
    69,
    44,
    46
  ],
   "蠕虫围巾给什么永久BUFF": [
    5,
    114,
    215
  ],
   "箭袋补充开关": true,
   "箭袋补充时间/帧": 20,
   "箭袋补充物品数量": 99,
   "箭袋补充物品ID": [
    40,
    41,
    47,
    51,
    516,
    545,
    988,
    1235,
    1334,
    1341,
    3003,
    3568,
    5348
  ],
   "箭袋给什么永久BUFF": [
    16,
    93,
    112
  ]
}
```

## feedback
- Priority to ISSUED-> Commonly maintained plug -in library: https://github.com/Controllerdestiny/tshockPlugin
- Two priority: TSHOCK official group: 816771079
- It may not be seen in a high probability, but it can also be: domestic community trhub.cn, bbstr.net, tr.monika.love