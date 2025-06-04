# WeaponPlusCostCoin

- Author: 枳
- Source: [github](https://github.com/skywhale-zhi/WeaponPlusCostCoin)
- This is a Tshock server plugin mainly used to enhance weapons using in-game coins
- Through the command to enhance damage multiple, knockback, size, attack speed, and bullet speed
- The attribute definitions of melee, magic, ranged, summoned, and other weapons are distinguished.
- Storing players' weapon enhancement levels through the tshock.sqlite database will not lose any enhancement due to discarding weapons or re-entering the server.
- This plug-in supports Chinese and English conversion


## Instruction

| Command                   | Alias |        Permissions        |      Description      |
|----------------------|:--:|:----------------:|:------------:|
| /plus help           | none  | weaponplus.plus  |   View the Weapon Enhancement Menu   |
| /clearallplayersplus | none  | weaponplus.admin | Clear all players' enhanced items |

## Configuration
> Configuration file location: tshock/WeaponPlus.json
```json5
{
  "启用英文": false, //Enable English
  "进服时是否开启自动重读武器": true, //Enable automatic rereading weapons when entering the server
  "最多升级次数": 50, //Maximum number of upgrades
  "花费参数": 1.0, //Spend parameters
  "升级花费增加": 0.2, //Increased upgrade cost
  "重置武器返还倍数": 0.5, //Reset weapon return multiple
  "武器升级攻速上限倍数": 60.0, //Weapon upgrade attack speed maximum multiple
  "武器升级射弹飞速上限倍数": 3.0, //Weapon upgrade projectile speed maximum multiple
  "武器升级击退上限倍数": 3.0, //Weapon upgrade knockback maximum multiple
  "武器升级尺寸上限倍数": 2.5, //Weapon upgrade size upper limit multiple
  "近战武器伤害上限倍数": 1.75, //Melee weapon damage maximum multiple
  "近战武器升级攻速上限": 8, //Melee weapon upgrade attack speed limit
  "远程武器伤害上限倍数": 2.0, //Ranged weapon damage maximum multiple
  "远程武器升级攻速上限": 4, //Ranged weapon upgrade attack speed limit
  "魔法武器伤害上限倍数": 2.3, //Magic weapon damage upper limit multiple
  "魔法武器升级攻速上限": 6, //Magic weapon upgrade attack speed limit
  "召唤武器伤害上限倍数": 2.5, //Summoning weapon damage maximum multiple
  "召唤武器升级攻速上限": 8, //Summoning weapons upgrade attack speed limit
  "其他武器伤害上限倍数": 2.0, //Other weapons damage maximum multiples
  "其他武器升级攻速上限": 8 //Other weapons upgraded attack speed limit
}
```

## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
