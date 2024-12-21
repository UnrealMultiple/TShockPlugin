# BridgeBuilder 

- Authors: Soofa，肝帝熙恩
- Source: [github](https://github.com/Soof4/BridgeBuilder)
- Quickly pave in the parallel direction facing the player, stopping when encountering a block, and stopping when a wall encounters another wall.
- Customizable bridge blocks are available, with default configurations including: platforms, planter boxes, and team blocks.
- It can now be placed vertically, and walls are allowed.
- Did you know: `<>` indicates required fields, while `[]` indicates optional fields.


## Commands

| Command     | Permission |          Details          |
| -------------- | :-----------------: | :------: |
| /bridge [up/down] or /桥来 [up/down]|  bridgebuilder.bridge  | Quick bridge placement command |

## Config
> Configuration file location：tshock/BridgeBuilder.en-US.json
```json
{
  "TilesID": [ // Allow quick placement of tile ID
    19,
    380,
    427,
    435,
    436,
    437,
    438,
    439
  ],
  "WallsID": [], // Allow quick placement of wall ID
  "MaxTile": 256 // The maximum number of tiles that can be placed at once
}
```

## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
