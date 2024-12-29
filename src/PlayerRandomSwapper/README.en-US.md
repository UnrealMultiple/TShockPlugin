# PlayerRandomSwapper

- Authors: 肝帝熙恩 少司命
- Source: 无
- Randomly swap players' positions, with options for both multiplayer and duo modes.
- In multiplayer mode, all players' positions are randomly swapped. In duo mode, two players are randomly selected to swap positions.
- Only players with the `playerswap` permission and not in a dead state will be included in the teleportation list. If there are fewer than two players in the list, no teleportation will occur.
- Supports random teleportation time intervals.


## Commands

| Syntax                             | Alias                      | Permission           | Description                                   |
| ---------------------------------- | :------------------------: | :------------------: | :--------------------------------------------: |
| /swaptoggle as                     | /swaptoggle allowself       | swapplugin.toggle    | Toggle allowing players to swap positions with themselves in duo mode |
| /swaptoggle en                     | /swaptoggle enable          | swapplugin.toggle    | Toggle random position swapping                |
| /swaptoggle i <teleport interval>  | /swaptoggle interval        | swapplugin.toggle    | Set teleport interval time (in seconds)        |
| /swaptoggle maxi <max interval>    | /swaptoggle maxinterval     | swapplugin.toggle    | Set maximum teleport interval time (in seconds)|
| /swaptoggle mini <min interval>    | /swaptoggle mininterval     | swapplugin.toggle    | Set minimum teleport interval time (in seconds)|
| /swaptoggle ri                     | /swaptoggle randominterval  | swapplugin.toggle    | Toggle random teleport interval                |
| /swaptoggle swap                   | /swaptoggle swap            | swapplugin.toggle    | Toggle broadcasting player position swaps      |
| /swaptoggle timer [broadcast countdown threshold] | /swaptoggle timer     | swapplugin.toggle | Toggle the broadcast remaining teleport time status or set the broadcast countdown threshold. |
|                                    |                            | playerswap           | Required permission to be included in teleport list |

## Configuration
> Configuration file location：tshock/PlayerRandomSwapper.en-US.json
```json
{
  "PluginEnabled": true,
  "IntervalSeconds": 10,
  "RandomIntervalSeconds": false,
  "MaxRandomIntervalSeconds": 30,
  "MinRandomIntervalSeconds": 10,
  "AllowSamePlayerSwap": true,
  "Multi-PlayerMode": false,
  "BroadcastRemainingTime": true,
  "BroadcastRemainingTimeThreshold": 5,
  "BroadcastPlayerSwap": true
}
```

## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
