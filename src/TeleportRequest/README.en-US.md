# TeleportRequest

- Authors: MarioE, 修改者: Dr.Toxic，肝帝熙恩
- Source: [github](https://github.com/MarioE/TeleportRequest)
- The teleportation request needs to be accepted or denied by the recipient before the teleportation occurs, which is referred to as tpa.

## Commands

| Command               |     Permission     | Details                                                           |
|-----------------------|:------------------:|:------------------------------------------------------------------|
| `/atp`                | `tprequest.gettpr` | Accept teleport request                                           |
| `/autodeny` `         | `tprequest.tpauto` | 	Automatically deny all teleport requests                         |
| `/autoaccept` `       | `tprequest.tpauto` | 	Automatically accept all teleport requests                       |
| `/dtp`                | `tprequest.gettpr` | 	Deny teleport request                                            |
| `/tpahere <player>` ` |  `tprequest.tpat`  | Request to teleport the specified player to your current location |
| `/tpa <player>` `     |  `tprequest.tpat`  | Request to teleport to the specified player’s current location    |


## Config
> Configuration file location：tshock/tpconfig.json
```json5
{
  "间隔秒数": 3, //Interval Seconds
  "超时次数": 3 //Timeout Count
}
```
## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
