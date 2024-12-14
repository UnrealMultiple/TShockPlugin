# DwTP

- Authors: 羽学
- Source: 无
- This is a Tshock server plugin, mainly used for：
- teleporting to specific landmarks using commands such as Glow Lakes, Dungeons, Temples, Plantera's Bulb, and Boss Loot Bags.

## Update Log

```
v1.0.0
Use the /dw command to teleport to designated landmarks.
- Bulb: Teleports only dw the bulb exists on the world map.
- Dungeon: Teleports only dw there is a Guide or Cultist Archer in the dungeon.
- Temple: Teleports to the entrance if the door is not open, otherwise teleports to the altar.
- Loot Bag: Teleports to the location of the Boss's death (loot bag).
- Glow Lake: Teleports to the glow liquid; places a gray brick above the liquid if it is empty the first time.
```

## Commands

| Syntax                             | Alias  |       Permission       |                   Description                   |
| -------------------------------- | :---: | :--------------: | :--------------------------------------: |
| /dw  | /定位 |   dw.use    |    Command menu    |
| /dw hb | /定位 花苞 |   dw.use    |    Teleport to Plantera's Bulb    |
| /dw dl | /定位 地牢 |   dw.use    |    Teleport to the Guide or Cultist Archer in the dungeon    |
| /dw sm | /定位 神庙 |   dw.use    |    Teleport to the temple entrance or the Jungle Shrine    |
| /dw bag | /定位 宝藏袋 |   dw.use    |    Teleport to the Boss's death location (loot bag)    |
| /dw wgh | /定位 微光湖 |   dw.use    |    Teleport to the Glow Lake (places a gray brick above the liquid if it is empty the first time)    |

## Configuration
```json
None
```
## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love