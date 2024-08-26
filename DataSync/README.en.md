# DataSync progress synchronization plug -in

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: love
- Source: github
- The progress is synchronized when the two server -side databases use a database

## Update log

```
20240729
咋没版本，补全卸载函数
```

## REST APO

|Path|Authority|illustrate|
| --------- |: -------:|: -------:|
|/Datasync|DataSync|Query progress|

## instruction

|grammar|Authority|illustrate|
| ------------------------------ |: -----------:|: -------:|
|/Reset progress synchronization|DataSync|none|
|/Progress [Progress Name] [True Or False]|Datasenc.set|Set progress|
|/Progress Local|DataSync|Progress query|
|Progress|DataSync|Progress query|

## Configuration
> Configuration file location: tshock/datasync.json
```json
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
## feedback
- Priority to ISSUED-> Commonly maintained plug -in library: https://github.com/Controllerdestiny/tshockPlugin
- Two priority: TSHOCK official group: 816771079
- It may not be seen in a high probability, but it can also be: domestic community trhub.cn, bbstr.net, tr.monika.love