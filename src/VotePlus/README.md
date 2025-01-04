# VotePlus 多功能投票

- 作者: Cai
- 出处: 无
- 一个多功能投票插件

## 指令

| 语法                       | 别名 |    权限    |       说明        |
|--------------------------|:--:|:--------:|:---------------:|
| /vote kick <玩家>          | 无  | vote.use |    投票踢出指定玩家     |
| /vote ban <玩家>           | 无  | vote.use |    投票封禁指定玩家     |
| /vote mute <玩家>          | 无  | vote.use |    投票禁言指定玩家     |
| /vote clearboss <BOSS名称> | 无  | vote.use | 投票清理指定BOSS（不掉落） |
| /vote clearevent         | 无  | vote.use |   投票清理地图中所有事件   |
| /vote day                | 无  | vote.use |    投票将时间改为白天    |
| /vote night              | 无  | vote.use |     将时间改为黑夜     |
| /vote rain               | 无  | vote.use |    投票下雨或停止下雨    |
| /vote topic <主题>         | 无  | vote.use |     自由主题投票      |
| /vote clearkick <玩家>     | 无  | vote.use |   将玩家从踢出状态移除    |
| /vote kicklist           | 无  | vote.use |   查看处于踢出状态的玩家   |
| /agree                   | 同意 | vote.use |     同意当前投票      |
| /disagree                | 反对 | vote.use |     反对当前投票      |

## 配置
> 配置文件位置：tshock/VotePlus.json
```json5
{
  "启用投票踢出": true,
  "踢出持续时间(秒)": 600,
  "启用投票封禁": false,
  "启用投票禁言": true,
  "启用投票清除BOSS": true,
  "启用投票关闭事件": true,
  "启用投票修改时间": true,
  "启用投票修改天气": true,
  "启用自由投票": true,
  "最小投票人数": 3,
  "投票通过率": 60
}
```

## 更新日志

```
暂无
```

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/Controllerdestiny/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love