# DataSync progress synchronization plugin

- author: 恋恋
- source: github
- Implement progress synchronization when two servers share a database

## Rest API

| Command        |    Permission    |  Description  |
|-----------|:--------:|:----:|
| /DataSync | DataSync | Query progress |

## Instruction

| Command                        |      Permission      |  Description  |
|---------------------------|:------------:|:----:|
| /重置进度同步                   |   DataSync   |  None   |
| /进度 [progress name] [true or false] | DataSync.set | Set progress |
| /进度 local                 |   DataSync   | Progress query |
| /进度                       |   DataSync   | Progress query |

## Configuration
> Configuration file location: tshock/DataSync.json
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

## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
