# DTEntryBlock prevents access to dungeons or temples.

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: Gan Di Xi En
- Source: None
- Prevent players from entering the dungeon/temple before defeating the Skull King/Flower of the Century.

## Update log

```
v1.1.5
补充对醉酒世界的支持

v1.1.4
补全卸载函数
```

## instruction

|grammar|limit of authority|explain|
| -------------- |:-----------------:|:------:|
|without|skullking.bypass|Ignore and stop entering the dungeon|
|without|Plant.bypass|Ignore and stop entering the temple|

## deploy
> Configuration file path: tshock/ block access to dungeons or temples. json
```json
{
   "阻止玩家进入地牢总开关": true,
   "击杀未击败骷髅王进入地牢的玩家": true,
   "传送未击败骷髅王进入地牢的玩家": true,
   "阻止玩家进入神庙总开关": true,
   "击杀未击败世纪之花进入神庙的玩家": true,
   "传送未击败世纪之花进入神庙的玩家": true
}
```
## feedback
- Co-maintained plug-in library: https://github.com/THEXN/TShockPlugin/
- Domestic community, trhub.cn or TShock official group, etc.