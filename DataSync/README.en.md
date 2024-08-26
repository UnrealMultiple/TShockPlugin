# DataSync progress synchronization plug-in

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: love
- Source: github
- When two servers share a database, progress synchronization is realized.

## Update log

```
20240729
咋没版本，补全卸载函数
```

## Rest APO

|path|limit of authority|explain|
| --------- |:------:|:------:|
|/DataSync|DataSync|Query progress|

## instruction

|grammar|limit of authority|explain|
| ------------------------------ |:----------:|:------:|
|/Reset progress synchronization|DataSync|without|
|/progress [progress name] [true or false]|DataSync.set|Set the schedule|
|/progress local|DataSync|Progress query|
|/progress|DataSync|Progress query|

## deploy
> Configuration file location: tshock/DataSync.json
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
- Give priority to issued-> jointly maintained plug-in library: https://github.com/Controllerdestiny/TShockPlugin.
- Second priority: TShock official group: 816771079
- You can't see it with a high probability, but you can: domestic communities trhub.cn, bbstr.net, tr. monika.love.