# DTEntryBlock Block access to dungeon or temple

- author: 肝帝熙恩
- source: None
- Prevent players from entering dungeons/temples before defeating Skeletron/Plantera

## Instruction

| Command |        Permissions        |    Description    |
|----|:----------------:|:--------:|
| none  | skullking.bypass | Ignore the restrictions and enter the dungeon |
| none  |   Plant.bypass   | Ignore the restrictions and enter the temple |

## Configuration
> Configuration file path: tshock/阻止进入地牢或神庙.json
```json
{
  "阻止玩家进入地牢总开关": true, //Prevent players from entering dungeon
  "击杀未击败骷髅王进入地牢的玩家": true, //Kill players who enter the dungeon without defeating skeletron
  "传送未击败骷髅王进入地牢的玩家": true, //Teleport players who enter the dungeon without defeating skeletron
  "阻止玩家进入神庙总开关": true, //Prevent players from entering the temple
  "击杀未击败世纪之花进入神庙的玩家": true, //Kill players who enter the temple without defeating plantera
  "传送未击败世纪之花进入神庙的玩家": true //Teleport players who enter the temple without defeating plantera
}
```

## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
