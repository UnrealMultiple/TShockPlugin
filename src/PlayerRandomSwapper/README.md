# PlayerRandomSwapper 玩家位置随机交换
- 作者: 肝帝熙恩 少司命
- 出处: 无
- 随机交换玩家的位置，分为多人模式和双人模式
- 多人模式下，所有玩家随机交换位置。双人模式下，随机选取两名玩家交换位置
- 有权限`playerswap`并且没有在死亡状态的玩家才会被列入传送列表，当玩家列表少于2人时，不会传送
- 支持随机传送时间间隔

## 指令

| 语法                             | 别名                  | 权限              | 说明                                   |
| -------------------------------- | :-------------------: | :---------------: | :------------------------------------: |
| /swaptoggle as                   | /swaptoggle allowself | swapplugin.toggle | 切换允许双人模式玩家和自己交换位置的状态 |
| /swaptoggle en                   | /swaptoggle enable    | swapplugin.toggle | 切换随机位置互换的状态                 |
| /swaptoggle i <传送间隔秒>       | /swaptoggle interval  | swapplugin.toggle | 设置传送间隔时间（秒）                 |
| /swaptoggle maxi <最大传送间隔秒> | /swaptoggle maxinterval | swapplugin.toggle | 设置最大传送间隔时间（秒）            |
| /swaptoggle mini <最小传送间隔秒> | /swaptoggle mininterval | swapplugin.toggle | 设置最小传送间隔时间（秒）           |
| /swaptoggle ri                   | /swaptoggle randominterval | swapplugin.toggle | 切换随机传送间隔的状态              |
| /swaptoggle swap                 | /swaptoggle swap      | swapplugin.toggle | 切换广播玩家交换位置信息的状态         |
| /swaptoggle timer [广播交换倒计时阈值]    | /swaptoggle timer     | swapplugin.toggle | 切换广播剩余传送时间的状态或设置广播交换倒计时阈值            |
|                       |                       | playerswap | 有这个权限才会被传送            |

## 配置
> 配置文件位置：tshock/PlayerRandomSwapper.zh-CN.json
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

## 更新日志

### v1.0.0
- 添加插件

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
