# Challenger Challenger Mode

- Author: 星夜神花、枳、羽学
- Source: [github](https://github.com/skywhale-zhi/Challenger)
- This is a Tshock server plug-in mainly used to modify the server BOSSAI's modifications and equipment sets, accessories passive skills, etc.
- When the player is injured, a blood pack will be extracted. The blood pack will return blood to nearby enemies (boss priority). If the player can intercept the blood pack, he can return blood to himself.
- And allows you to control the health multiplier of all monsters by modifying the configuration file. Since the author lost the source code of version 1.0.1, Yuxue decompiled the plug-in for secondary open source and update.
- 
- The modified set enhancement effects are:
- Mining set: gives permanent mining and candy sprint buffs, and enables chain mining capabilities
- Angler set: gives permanent buff include: sonar, fishing, crate, and calming
- Ninja set: 1/4 chance to dodge non-lethal damage and release smoke
- Fossil set: Summons an amber ball of light above your head and throws extremely fast lightning bolts at enemies.
- Shadow set: When critical hits, Devouring Monster projectile are generated from around the player to attack surrounding enemies. Cooldown is 1 second.
- Crimson set: When a critical hit occurs, it absorbs a certain amount of blood from each surrounding enemy. As the number of enemies increases, the amount of blood sucked is -1, and the cooldown is 5 seconds.
- Meteor set: Recover some magic power when critical hit, and intermittently drop high-damage falling stars to attack the enemy.  
- Bee set: Gives a permanent honey buff; continuously scatters honey cans around, and the player's blood will be restored after contact and the honey buff will be given for 15 seconds;  
The amount of treatment given to the player itself is slightly lower than that given to other players.   
- Jungle Set/Ancient Cobalt: Intermittently generates damaging spores from around the player
- Necro set: When receiving damage, bones will be scattered around; when attacking, bone arrows will occasionally be fired.
- Obsidian set: Due to the Thief's Blessing, the dropped items will be tried to be dropped twice (only valid for non-boss creatures and non-high HP monsters)
- Molten set: Immunity to magma, gives permanent Hellfire buff
- Spider set: When attacking, inflicts poison and poison debuffs on the enemy. Press the "up" key to generate a poisonous fang potion bottle, which explodes when it hits the enemy."  
- Crystal Assassin set: When an enemy is nearby, it releases crystal fragments; if the player is hit, it releases more powerful fragments
- Frost set: It starts to snow around you (The Snow Queen drops the weapon "Arctic" projectile)  
- Forbidden set: Releases automatically homing soul flames to attack nearby enemies.
- Hallowed Set/Ancient Hallowed: Summons light and dark sword energy when hitting an enemy. Enter "/cf" to switch the sword energy type.
- Chlorophyte set: Release imprecise chlorophyll crystal arrows, and the power of the jungle gives you a higher life limit.  
- Turtle set: Increases the blood limit by 60 and automatically releases explosive fragments nearby
- Tiki set: Increases the health limit by 20 and leaves spores in the path of the whip
- Beetle set: Increases the blood limit by 60, and part of the enemy's damage will heal surrounding teammates and provide buffs;  
Melee weapon damage is doubled when Paladin Shield or its superior compound is equipped.
- Shroomite set: Projectiles will erratically leave behind mushrooms
- Spooky Set: When using the whip, throw out a bat or pumpkin head
- Spectre Set: Increases the blood limit by 40 or the mana limit by 80 depending on the headgear choice; summons 2 ghost curses to surround the player and attack nearby enemies.
- Royal Gel: The sky begins to rain gel
- Shield of Cthulhu: Gain invincibility for a short period of time when sprinting, cooldown 12 seconds
- Brain of Confusion: Enter "/cf" to confuse all surrounding enemies.
- Worm Scarf: Immune to cold, frostfire, ichor and spellfire
- Radio things: Enter "/cf" to listen to the weather forecast. In hard mode, you can listen to the world prophet forecast.  
- Hive Pack: Continuously throws poisonous bee hive around, then explodes to release a bee.
- Volatile Gel: Hitting an enemy has a chance to drop broken magic crystals, pearl stones, gel, etc.
- Magic Quiver: Gives a permanent buff and enables unlimited ammunition

- [Current problem]: The damage caused by the added effect will not be counted as caused by the player, but monsters can still be killed.



## instruction

| Commands | Alias |    Permissions    |                                   Description                                    |
| -------- | :---: | :---------------: | :------------------------------------------------------------------------------: |
| /cenable | none  | challenger.enable |                Enable challenge mode and use it again to disable                 |
| /tips    | none  |  challenger.tips  | Enable content prompts, such as text prompts for various items, use Cancel again |
| /cf      | none  |       none        |             Used to switch or trigger equipment passive skill types              |

## Configuration
> Configuration file location: tshock/ChallengerConfig.json
```
{
  "是否启用挑战模式": true, //Whether to enable challenge mode
  "是否启用BOSS魔改": false, //Whether to enable BOSS modification
  "启用话痨模式": false, //Enable chatterbox mode
  "启用广播话痨模式": false, //Enable broadcast talk mode
  "是否启用怪物吸血": true, //Whether to enable monster blood sucking
  "怪物吸血比率": 0.25, //Monster blood-sucking ratio
  "怪物吸血比率对于Boss": 0.5, //Monster life-stealing ratio for Boss
  "怪物回血上限：小怪>1.5倍则会消失": 1.5, //Monster blood recovery upper limit: mobs will disappear if they are >1.5 times
  "所有怪物血量倍数(仅开启魔改BOSS时生效)": 1.0, //The health multiplier of all monsters (only takes effect when the BOSS modification is turned on)
  "冲刺饰品类的闪避冷却时间/默认12秒": 5, //Dodge cooldown time of sprint accessories/default 12 seconds
  "蜜蜂背包是否扔毒蜂罐": true, //Whether the Hive Pack throws bee hive
  "蜜蜂背包首次弹幕ID": 183, //Hive Pack first Projectile ID
  "蜜蜂背包首次弹幕伤害": 30, //Hive Pack first Projectile Damage
  "蜜蜂背包首次弹幕击退": 10.0, //Hive Pack first Projectile Repulse
  "蜜蜂背包生成弹幕概率/分母值": 3, //Hive Pack generates Projectile probability/denominator value
  "蜜蜂背包弹幕爆炸后的弹幕ID": 566, //The Projectile ID after the Hive Pack Projectile explosion
  "蜜蜂背包弹幕爆炸后的弹幕伤害": 30, //Projectile damage after Hive Pack Projectile explosion
  "蜜蜂背包弹幕爆炸后的弹幕击退": 0.0, //The Projectile knockback after the Hive Pack Projectile explodes
  "蜜蜂背包二次弹幕间隔/帧": 240, /Hive Pack Second Projectile Interval/Frame
  "化石套是否出琥珀光球": true, //Whether the Fossil Set produces projectile balls
  "化石套的弹幕ID": 732, //Fossil Set Projectile ID
  "化石套的弹幕射程": 48.0, //Fossil Set Projectile Range
  "化石套的识敌范围": 3750.0, //Fossil Set enemy Detection Range
  "化石套的弹幕伤害": 25, //Fossil Set Projectile Damage
  "化石套的弹幕击退": 8.0, //Fossil Set Projectile knockback
  "化石套的弹幕间隔/帧": 10.0, //Fossil Set Projectile interval/frame
  "丛林套是否环绕伤害孢子": true, //Whether the Jungle Set surrounds the damage spores
  "丛林套弹幕射程/速率": 1.5, //Jungle Set Projectile Range/Rate
  "丛林套弹幕ID1": 569, //Jungle Set Projectile ID1
  "丛林套弹幕ID2": 572, //Jungle Set Projectile ID2
  "丛林套弹幕伤害": 35, //Jungle Set Projectile Damage
  "丛林套弹幕击退": 8.0, //Jungle Set Projectile Repel
  "丛林套弹幕间隔/帧": 1, //Jungle Set Projectile interval/frame
  "忍者套是否会闪避": true, //Whether the Ninja Set will dodge
  "忍者套闪避概率随机数/0则100%闪避": 2, //Ninja Set dodge probability random number/0 means 100% dodge
  "忍者套闪避释放的弹幕ID": 196, //Projectile ID of Ninja Set dodge release
  "忍者套闪避释放的弹幕伤害": 0, //The Projectile damage released by the Ninja Set dodges
  "忍者套闪避释放的弹幕击退": 0.0, //The Projectile released by the Ninja Set dodges and knockback
  "暗影套的弹幕ID": 307, //Shadow Set Projectile ID
  "暗影套的弹幕伤害": 40, //Shadow Set Projectile Damage
  "暗影套的弹幕击退": 2.0, //Shadow Set Projectile Knockback
  "暗影套的弹幕间隔/帧": 180, //Shadow Set Projectile Interval/Frame
  "猩红套的弹幕ID": 305, //Crimson Set Projectile ID
  "猩红套的弹幕伤害": 0, //Crimson Set Projectile Damage
  "猩红套的弹幕击退": 0.0, //Crimson Set Projectile Knockback
  "猩红套的弹幕间隔/帧": 300, //Crimson Set Projectile interval/frame
  "流星套是否下落星": true, //Whether the Meteor Set is a falling Projectile
  "流星套的弹幕ID": 725, //Meteor Set's Projectile ID
  "流星套的弹幕射程": 1000, //Meteor Set Projectile range
  "流星套的弹幕速度/帧": 8.0, //Meteor Set Projectile speed/frame
  "流星套的弹幕间隔/帧": 120, //Meteor Set Projectile Interval/Frame
  "死灵套是否产生额外弹幕": true, //Whether the Necro Set generates additional Projectile
  "死灵套受到攻击时的弹幕ID": 532, //Projectile ID when Necro Set gets attacked
  "死灵套受到攻击时的弹幕伤害": 20, //Projectile Damage when the Necro Set gets attacked
  "死灵套受到攻击时的弹幕击退": 5.0, //Necro Set Projectile Knockback when gets attacked
  "死灵套的受伤弹幕间隔/帧": 1, //Necro Set Damage Projectile interval/frame
  "死灵套攻击时的弹幕ID": 117, //Projectile ID when attacking with Necro Set
  "死灵套攻击时的弹幕伤害": 20, //Projectile Damage when attacking with Necro Set
  "死灵套攻击时的弹幕击退": 2.0, //Projectile Knockback when attacking with Necro Set
  "死灵套的攻击弹幕间隔/帧": 1, //Necro Set attack Projectile interval/frame
  "黑曜石套是否盗窃双倍掉落物": true, //Whether Obsidian Set steal double drops
  "黑曜石套盗窃的稀有等级": 2, //Obsidian Set stolen rarity level
  "水晶刺客套是否释放水晶碎片": true, //Whether the Crystal Assassin Set releases crystal shards
  "水晶刺客套遇怪自动释放的弹幕ID": 90, //Projectile ID automatically released by Crystal Assassin Set when encountering monsters
  "水晶刺客套受伤释放的弹幕ID": 94, //Crystal Assassin Set Release projectile ID
  "水晶刺客套弹幕间隔/帧": 50, //Crystal Assassin Set Projectile Interval/Frame
  "蜘蛛套给NPC施加什么BUFF1": 70, //What BUFF1 does the Spider Set apply to NPCs
  "蜘蛛套给NPC施加BUFF1时长": 180, //Spider Set applies BUFF1 duration to NPC
  "蜘蛛套给NPC施加什么BUFF2": 20, //What BUFF2 does Spider Set apply to NPC
  "蜘蛛套给NPC施加BUFF2时长": 360, //Spider Set applies BUFF2 duration to NPC
  "禁戒套是否释放弹幕": true, //Whether the Forbidden Set releases Projectile
  "禁戒套释放什么弹幕ID": 950, //What Projectile ID is released by the Forbidden Set
  "禁戒套的弹幕伤害": 125, //Projectile Damage of Forbidden Set
  "禁戒套的弹幕频率/分母值": 15, //Projectile frequency/denominator value of Forbidden Set
  "禁戒套的弹幕范围": 300, //Projectile Range of Forbidden Set
  "寒霜套是否下弹幕": true, //Whether the Frost Set will drop a Projectile
  "寒霜套释放什么弹幕ID": 297, //What Projectile ID is released by Frost Set
  "寒霜套的弹幕伤害": 40, //Frost Set Projectile Damage
  "寒霜套的弹幕击退": 5.0, //Frost Set Projectile Knockback
  "寒霜套的弹幕间隔/帧": 60, //Frost Set Projectile Interval/Frame
  "神圣套额外弹幕多少伤害/默认55%": 0.35, //How much Damage does the Hallowed Set additional Projectile do/default 55%
  "神圣套释放什么弹幕ID": 156, //What Projectile ID is released by the Hallowed Set
  "神圣套释放什么弹幕ID2": 157, //What Projectile ID2 is released by the Hallowed Set
  "神圣套的弹幕间隔/帧": 2, //Hallowed Set Projectile Interval/Frame
  "叶绿套加多少生命上限": 100, //How much life limit does Chloropyte Set increase
  "海龟套加多少生命上限": 120, //How much life limit does Turtle Set increase
  "海龟套的弹幕ID": 338, //Turtle Set Projectile ID
  "海龟套的弹幕伤害": 50, //Turtle Set Projectile Damage
  "海龟套的弹幕间隔/默认3秒": 3, //Turtle Set Projectile interval/default 3 seconds
  "提基套加多少生命上限": 200, //How much life limit is added to Tiki Set
  "提基套的弹幕ID": 523, //Tiki Set cover Projectile ID
  "提基套的弹幕伤害": 1.2, //Tiki Set Projectile Damage
  "提基套的弹幕间隔/帧": 2, //Tiki Set cover Projectile interval/frame
  "阴森套是否出弹幕": true, //Whether the Spooky Set will pop up a Projectile
  "阴森套白天出什么弹幕": 316, //What kind of Spooky Set Projectile is there during the day?
  "阴森套晚上出什么弹幕": 321, //What kind of Spooky Set Projectile is going on at night?
  "阴森套弹幕伤害/默认0.2": 0.8, //Spectre Set Projectile Damage/Default 0.2
  "阴森套的弹幕间隔/帧": 10, //Spectre Set Projectile interval/frame
  "蘑菇套是否出弹幕": true, //Whether Shroomite Set cover will pop up a Projectile
  "蘑菇套的弹幕ID": 819, //Shroomite Set Projectile ID
  "蘑菇套的弹幕伤害倍数": 0.6, //Shroomite Set Projectile Damage Multiplier
  "蘑菇套的弹幕击退": 1.14514, //Shroomite Set Projectile Repulse
  "蘑菇套的弹幕间隔/帧": 1, //Shroomite Set Projectile interval/frame
  "甲虫套加多少生命上限": 180, //How much life limit does the Beetle Set increase
  "甲虫套受到伤害给其他玩家的回血转换比例/默认30%": 0.5, //The conversion ratio of blood recovery to other players when the Beetle Set is damaged/default 30%
  "甲虫套减多少回复量/默认为0": 15, //How much recovery is reduced by the Beetle Set/default is 0
  "甲虫套带骑士盾加多少弹幕伤害/默认90%": 1.5, //How much Projectile Damage does a Beetle Set with a paladin shield add/default 90%
  "甲虫套的弹幕ID": 866, //Beetle Set Projectile ID
  "甲虫套的治疗弹幕间隔/帧": 1, //Beetle Set healing Projectile interval/frame
  "甲虫套的攻击弹幕间隔/帧": 1, //Beetle Set attack Projectile interval/frame
  "皇家凝胶是否下物品雨": true, //Whether Royal Gel rains items
  "皇家凝胶物品雨随机概率": 25, //Royal Gel Item Rain Random Probability
  "皇家凝胶物品雨间隔/帧": 180, //Royal Gel Item Rain Interval/Frame
  "皇家凝胶物品雨表": [ //Royal Gel Item Rainmeter
    75
  ],
  "挥发凝胶击中敌怪掉落物品表": [ //Table of items dropped by Violatile Gel when hitting an enemy
    72,
    75,
    501,
    502
  ],
  "幽灵套加多少生命和魔力上限": 80, //How much life and mana power does the Spectre Set add to the upper limit
  "幽灵兜帽是否出幽灵弹幕": true, //Whether the Spectre Hood emits ghost Projectile
  "幽灵面具是否出幽灵弹幕": false, //Whether the Spectre Mask emits ghost Projectile
  "幽灵套的弹幕ID": 79, //Spectre Set Projectile ID
  "幽灵套的弹幕伤害倍数": 50.0, //Spectre Set Projectile Damage Multiplier
  "幽灵套的弹幕击退": 0.0, //spectre Set Projectile Knockback
  "幽灵套环绕的弹幕ID": 299, //Projectile ID surrounded by Spectre Set
  "幽灵套环绕的弹幕伤害": 0, //Projectile damage surrounded by Spectre Set
  "幽灵套环绕的弹幕击退": 0.0, //Projectile Knockback surrounded by Spectre Set
  "幽灵套的攻击弹幕间隔/帧": 2, //Spectre Set attack Projectile interval/frame
  "幽灵套给什么永久BUFF": [ //What Permanent BUFF does the Spectre Set give
    6,
    7,
    181,
    178
  ],
  "蜜蜂套是否释放弹幕": true, //Whether the Bee Set releases the Projectile
  "蜜蜂套的BUFF时长/帧": 150, //Bee Set BUFF duration/frame
  "蜜蜂套的弹幕间隔/帧": 120, //Bee Set Projectile Interval/Frame
  "蜜蜂套给什么永久BUFF": [ //What Permanent BUFF does the Bee Set give?
    48
  ],
  "狱岩套给什么永久BUFF": [ //What Permanent buff does the Molten Set give?
    1,
    172
  ],
  "钓鱼套包含哪些永久BUFF": [ //What Permanent buffs does the Angler Set contain?
    106,
    123,
    121,
    122
  ],
  "挖矿套是否开启连锁挖矿": true, //Whether the Mining Set enables vein miner
  "挖矿套给什么永久BUFF": [ //What Permanent BUFF does the Mining Set give?
    104,
    192
  ],
  "挖矿套连锁图格ID表": [ //Mining Set vein miner tile ID table
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
  "蠕虫围巾免疫buff是否开启": false, //Whether the Worm Scarf immunity buff is turned on
  "蠕虫围巾遍历前几个buff": 22, //Worm Scarf traverses the first few buffs
  "蠕虫围巾免疫DeBuff列表/遇到会清空所有BUFF": [ //Worm Scarf Immunity DeBuff List/All BUFFs will be cleared when encountered
    39,
    69,
    44,
    46
  ],
  "蠕虫围巾给什么永久BUFF": [ //What permanent BUFF does the Worm Scarf give?
    5,
    114,
    215
  ],
  "箭袋补充开关": true, //Magic Quiver refill switch
  "箭袋补充时间/帧": 20, //Magic Quiver replenishment time/frame
  "箭袋补充物品数量": 99, //Number of Magic Quiver replenishment items
  "箭袋补充物品ID": [ //Magic Quiver replenishment item ID
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
  "箭袋给什么永久BUFF": [ //What permanent buff does the Magic Quiver give?
    16,
    93,
    112
  ]
}
```

## Change log

```
Change log
1.1.7
Add Spanish translation (By itsFrankV22)
Add English README (By Hexagonforce12)

1.1.6
Add English translation

1.1.5
Multi-language support

1.1.4
Fix ServerLeave Null error report

1.1.3
Complete uninstall function

1.1.2 fix
Try to fix null references caused by the following three methods
OnGameUpdate/WormScarf/OnServerLeave

1.1.1
Added random generation probability and interval to [Royal Gel]

1.1.0
The [Projectile Interval] configuration item has been added to the following suits:
hive pack, fossil set, jungle set, shadow set,
Crimson Set, Meteor Set, Crystal Assassin Set, Frost Set, Hallowed Set
Turtle set, Tiki set, Spooky set, Shroomite set, Mining set, Beetle set, Spectre set
Adjusted the default configuration value of the Challenger mode suit, and added an additional [enemy recognition range] considering the particularity of the fossil suit projectile.


1.0.9
Fixed a bug that caused the upper limit of blood volume to be reset when players exit the server while wearing suits.
See the 1.0.5 update log for details. Yu Xue commented out the ServerLeave method.

1.0.8
Fixed the permanent BUFF of the Spectre Set
Added magic quiver enhancement, enabling unlimited ammunition
The amount of ammunition is automatically reset when attacking while wearing  magic quiver accessories, and cannot be stacked (to prevent players from using their characteristics to farm items)
Added the permanent BUFF of the Worm Scarf/Spectre set, the projectile frequency/range of the Forbidden set, and the projectile ID of the Beetle set.
Modified a large number of default configuration item parameters (mainly the bee set)

1.0.7
Optimized the layout order of configuration files
Added the configuration item "Whether the mining set enables chain mining"
The chain mining capability of the mining set has been added, and its buff and chain tile ID can be configured.
Added configuration options for Shadow Set, Crimson Set, Obsidian Set, and Volatile Gel

1.0.6
Added an upper limit for monster blood sucking (to avoid the bug of raising gu)
All sprint accessories are equipped with dodge effects
Added configuration options for hive pack, fossil set, forbidden set, shroomite set, spectre set, and angler set
Fixed incorrect names of some configuration items
The following 3 configuration items have been removed:
  "Monster blood-sucking conditions: How much damage the player receives triggers": 1500,
  "Monster blood-sucking conditions: Return blood to non-BOSS monsters whose health value is less than the upper limit of the health value": 4000.0,
  "Monster barrage blood-sucking conditions: Return blood to non-BOSS monsters whose health value is less than the maximum health limit": 6000.0,

1.0.5
Statement: The problem of the player's health not increasing under the effect of the set is caused by the lack of forced land reclamation.
Fixed the problem of long loading time when the player exits the game and re-enters it.
Fixed a null reference issue that occurred when players exited

1.0.4
A large number of configuration items have been added (there are too many to write down, just look at the configuration file)
Note: There is a bug in the immune debuff function of the worm scarf.
When encountering a DeBUFF that requires immunity, all existing BUFFs will be cleared (issue with package No. 50), which is not recommended.

1.0.3
Configuration items added
  "Whether the spectre hood emits ghost projectiles": true,
  "Whether the spectre mask emits ghost projectiles": false,
  "The conversion ratio of blood recovery to other players when the beetle set is damaged/default 30%": 0.3,
  "How much does the beetle set reduce the recovery amount/default is 0": 10,
Players have reported that unlimited head swapping in version 1.0.2 of the Spectre Set will accumulate barrages, so this bug has been added to fix it.

1.0.2
Yuxue has added a large number of configuration items, allowing you to customize the passive skills of certain accessories or modify them.
Configuration items can be modified to decide whether to enable the magic modification of BOSS_AI

1.0.1
When the player is injured, a blood pack will be drawn out. The blood pack will restore blood to nearby enemies (boss takes priority). If the player can intercept the blood pack, he will be able to restore blood to himself.
```

## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love