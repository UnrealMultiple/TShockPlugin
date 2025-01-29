# DataSync 进度同步插件

- 作者: 恋恋
- 出处: github
- 当两个服务端公用一个数据库时实现进度同步

## 更新日志



## Rest API

| 路径        |    权限    |  说明  |
|-----------|:--------:|:----:|
| /DataSync | DataSync | 查询进度 |

## 指令

| 语法                        |      权限      |  说明  |
|---------------------------|:------------:|:----:|
| /重置进度同步                   |   DataSync   |  无   |
| /进度 [进度名] [true or false] | DataSync.set | 设置进度 |
| /进度 local                 |   DataSync   | 进度查询 |
| /进度                       |   DataSync   | 进度查询 |

## 配置
> 配置文件位置：tshock/DataSync.json
```json5
{
  "KingSlime": false,
  "EyeOfCthulhu": false,
  "PrehardmodeBoss2": false,
  "QueenBee": false,
  "Skeletron": false,
  "WallOfFlesh": false,
  "MechBoss": false,
  "TheTwins": false,
  "TheDestroyer": false,
  "SkeletronPrime": false,
  "Plantera": false,
  "Golem": false,
  "DukeFishron": false,
  "LunaticCultist": false,
  "MoonLord": false,
  "SolarPillar": false,
  "VortexPillar": false,
  "NebulaPillar": false,
  "StardustPillar": false,
  "ChristmasIceQueen": false,
  "ChristmasSantank": false,
  "ChristmasTree": false,
  "HalloweenTree": false,
  "QueenSlime": false,
  "Deerclops": false,
  "EmpressOfLight": false,
  "BloodMoon": false,
  "BloodMoonHardmode": false,
  "SolarEclipse": false,
  "SolarEclipseMech": false,
  "SolarEclipsePlantera": false,
  "PumpkinMoon": false,
  "FrostMoon": false,
  "MartianMadness": false,
  "OldOnesArmy": false,
  "GoblinsArmy": false,
  "GoblinsArmyHardmode": false,
  "PiratesArmy": false,
  "FrostLegion": false,
  "DD2Mage": false,
  "DD2Orge": false,
  "DD2Betsy": false,
  "Unreachable": false
}
```

```
2025.1.29
添加 GetString
2024.07.29
咋没版本，补全卸载函数
```

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
