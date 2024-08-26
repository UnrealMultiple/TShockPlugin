# Economics.WeaponPlus weapon enhanced EC version

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Authors: Fructus Aurantii, Shao Siming
- - 出处: [github](https://github.com/skywhale-zhi/WeaponPlusCostCoin) 
- This is a Tshock server plug-in, which is mainly used to use the economic strengthening weapon of Economics.
- Strengthen the damage multiple, repulsion, size, attack speed and bounce speed of the items owned by the player through instructions.
- The attribute definitions of melee, spell, ranged, summoning and other weapons are distinguished.
- Storing player's weapon enhancement level through tshock.sqlite database will not be enhanced due to discarding weapons or re-entering the server.
- This plugin supports Chinese-English conversion.

> [! NOTE]
> Need to install the pre-plug-in: EconomicsAPI (this warehouse)

## Update log

```
更新日志
1.0.3
适配新版EC套件
配置文件支持自动中英文转换
指令方法无任何改变
（羽学重构了配置文件使其更简洁）

1.0.2
此版本为中文论坛发布最新版，作者尚未开源。
羽学对其反编译并重构了Config（移除中文变量）
```

## instruction

|grammar|another name|limit of authority|explain|
| -------------- |:---------:|:------------:|:------:|
|/plus help|without|weaponplus.plus|View weapon enhancement menu|
|/clearallplayersplus|without|weaponplus.admin|Clear all players' enhancement items.|

## deploy
> Configuration file location: tshock/economics/weaponplus.json.
```
{
   "启用英文": false,
   "进服时是否开启自动重读武器": true,
   "最多升级次数": 50,
   "花费参数": 1.0,
   "升级花费增加": 0.2,
   "重置武器返还倍数": 0.5,
   "武器升级攻速上限倍数": 60.0,
   "武器升级射弹飞速上限倍数": 3.0,
   "武器升级击退上限倍数": 3.0,
   "武器升级尺寸上限倍数": 2.5,
   "近战武器伤害上限倍数": 1.75,
   "近战武器升级攻速上限": 8,
   "远程武器伤害上限倍数": 2.0,
   "远程武器升级攻速上限": 4,
   "魔法武器伤害上限倍数": 2.3,
   "魔法武器升级攻速上限": 6,
   "召唤武器伤害上限倍数": 2.5,
   "召唤武器升级攻速上限": 8,
   "其他武器伤害上限倍数": 2.0,
   "其他武器升级攻速上限": 8
}
```

## feedback
- Give priority to issued-> jointly maintained plug-in library: https://github.com/Controllerdestiny/TShockPlugin.
- Second priority: TShock official group: 816771079
- You can't see it with a high probability, but you can: domestic communities trhub.cn, bbstr.net, tr. monika.love.