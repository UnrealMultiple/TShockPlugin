# ECONOMICS.weaponPlus Weapon Reinforcement EC Edition

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: 少, Shao Si Ming
- - 出处: [github](https://github.com/skywhale-zhi/WeaponPlusCostCoin) 
- This is a TSHOCK server plug -in mainly used to use Economics' economic enhanced weapons
- Through instructions to have items reinforcement damage multiple, repellers, size, attack speed, ejection speed
- The definition of attributes of melee, spells, remote, summoning, and other weapons
- Reinforcement levels to store players' weapons from TSHOCK.SQLITE database will not be strengthened due to discarding weapons or re -entered the server
- This plug -in supports Chinese and English conversion

> [! Note]
> You need to install front plug -in: Economicsapi (this warehouse)

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

|grammar|Alias|Authority|illustrate|
| -------------- |: ----------:|: --------------:|: -------:|
|/plus help|none|weaponplus.plus|View weapon reinforcement menu|
|/Clearallplayersplus|none|weaponplus.admin|Clear the enhanced items of all players|

## Configuration
> Configuration file location: TSHOCK/ECONOMICS/WeaponPlus.json
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
- Priority to ISSUED-> Commonly maintained plug -in library: https://github.com/Controllerdestiny/tshockPlugin
- Two priority: TSHOCK official group: 816771079
- It may not be seen in a high probability, but it can also be: domestic community trhub.cn, bbstr.net, tr.monika.love