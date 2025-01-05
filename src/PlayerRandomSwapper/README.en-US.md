# PlayerRandomSwapper

- Authors: 肝帝熙恩 少司命
- Source: 无
- Randomly swap players' positions, with options for both multiplayer and duo modes.
- 多人模式下，所有玩家随机交换位置。In multiplayer mode, all players' positions are randomly swapped. In duo mode, two players are randomly selected to swap positions.
- Only players with the `playerswap` permission and not in a dead state will be included in the teleportation list. If there are fewer than two players in the list, no teleportation will occur.
- Supports random teleportation time intervals.

## 更新日志

```
```

## Commands

| Syntax                                                                                                |             别名             |             Permission            |                                                  Description                                                  |
| ----------------------------------------------------------------------------------------------------- | :------------------------: | :-------------------------------: | :-----------------------------------------------------------------------------------------------------------: |
| /swaptoggle as                                                                                        |    /swaptoggle allowself   | swapplugin.toggle |                     Toggle allowing players to swap positions with themselves in duo mode                     |
| /swaptoggle en                                                                                        |     /swaptoggle enable     | swapplugin.toggle |                                        Toggle random position swapping                                        |
| /swaptoggle i <teleport interval>                                                                     |    /swaptoggle interval    | swapplugin.toggle |                           Set teleport interval time (in seconds)                          |
| /swaptoggle maxi <max interval>                                                                       |   /swaptoggle maxinterval  | swapplugin.toggle |                       Set maximum teleport interval time (in seconds)                      |
| /swaptoggle mini <min interval>                                                                       |   /swaptoggle mininterval  | swapplugin.toggle |                       Set minimum teleport interval time (in seconds)                      |
| /swaptoggle ri                                                                                        | /swaptoggle randominterval | swapplugin.toggle |                                        Toggle random teleport interval                                        |
| /swaptoggle swap                                                                                      |      /swaptoggle swap      | swapplugin.toggle |                                   Toggle broadcasting player position swaps                                   |
| /swaptoggle timer [broadcast countdown threshold] |      /swaptoggle timer     | swapplugin.toggle | Toggle the broadcast remaining teleport time status or set the broadcast countdown threshold. |
|                                                                                                       |                            |             playerswap            |                                                   有这个权限才会被传送                                                  |

## Configuration

> Configuration file location：tshock/PlayerRandomSwapper.en-US.json

```json
{
  "总开关": true,
  "传送间隔秒": 10,
  "随机传送间隔": false,
  "传送间隔秒最大值": 30,
  "传送间隔秒最小值": 10,
  "双人模式允许玩家和自己交换": true,
  "多人打乱模式": false,
  "广播剩余传送时间": true,
  "广播交换倒计时阈值": 5,
  "广播玩家交换位置信息": true
}
```

## FeedBack

- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
