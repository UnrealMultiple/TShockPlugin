# Economics.WeaponPlus Weapon Enhancement EC Version

- 作者: 枳、少司命
- Source: [github](https://github.com/skywhale-zhi/WeaponPlusCostCoin)
- This is a Tshock server plugin mainly used for weapon enhancement using Economics.
- Enhances damage multiplier, knockback, size, attack speed, and projectile speed of items owned by players via commands.
- Differentiates the property definitions of melee, magic, ranged, summon, and other weapons.
- Stores player weapon enhancement levels in the tshock.sqlite database, ensuring that enhancements are not lost when weapons are discarded or when rejoining the server.
- This plugin supports Chinese and English conversion.

> [!NOTE]\
> Requires the installation of the prerequisite plugin: EconomicsAPI (this repository)

## Commands

| Syntax               |  别名  |            Permission            |              Description             |
| -------------------- | :--: | :------------------------------: | :----------------------------------: |
| /plus help           | None |  weaponplus.plus |   View the weapon enhancement menu   |
| /clearallplayersplus | None | weaponplus.admin | Clear enhanced items for all players |

## Configuration

> Configuration file location: tshock/Economics/WeaponPlus.json

```
{
  "Enable English": false,
  "Enable auto reloading of weapons upon joining": true,
  "Maximum upgrade times": 50,
  "Cost parameter": 1.0,
  "Upgrade cost increase": 0.2,
  "Reset weapon refund multiplier": 0.5,
  "Weapon upgrade attack speed cap multiplier": 60.0,
  "Weapon upgrade projectile speed cap multiplier": 3.0,
  "Weapon upgrade knockback cap multiplier": 3.0,
  "Weapon upgrade size cap multiplier": 2.5,
  "Melee weapon damage cap multiplier": 1.75,
  "Melee weapon upgrade attack speed cap": 8,
  "Ranged weapon damage cap multiplier": 2.0,
  "Ranged weapon upgrade attack speed cap": 4,
  "Magic weapon damage cap multiplier": 2.3,
  "Magic weapon upgrade attack speed cap": 6,
  "Summon weapon damage cap multiplier": 2.5,
  "Summon weapon upgrade attack speed cap": 8,
  "Other weapon damage cap multiplier": 2.0,
  "Other weapon upgrade attack speed cap": 8
}
```

## 更新日志

```
V2.0.0.0
适配多货币

V1.0.0.5
适配新 EconomicsAPI

1.0.3
适配新版EC套件
配置文件支持自动中英文转换
指令方法无任何改变
（羽学重构了配置文件使其更简洁）

1.0.2
此版本为中文论坛发布最新版，作者尚未开源。
羽学对其反编译并重构了Config（移除中文变量）
```

## FeedBack

- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
