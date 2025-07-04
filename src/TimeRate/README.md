# TimeRate 时间加速插件

- 作者: 羽学
- 出处: 无
- 这是一个Tshock服务器插件，主要用于:使用指令修改时间加速

## 指令

| 语法            |     别名      |     权限      |     说明      |
|---------------|:-----------:|:-----------:|:-----------:|
| /times        |    /时间加速    | times.admin |  时间加速指令菜单   |
| /times all    |  /时间加速 all  | times.admin | 开启或关闭全员睡觉加速 |
| /times one    |  /时间加速 one  | times.admin | 开启或关闭单人睡觉加速 |
| /times on     |  /时间加速 on   | times.admin |  开启指令时间加速   |
| /times off    |  /时间加速 off  | times.admin |  关闭指令时间加速   |
| /times set 数值 | /times s 数值 | times.admin |   设置加速速率    |

---
配置注意事项
---
1.`指令加速`可以用`/times on`和`/times off`来控制 
  
2.`全员睡觉加速`开启时会关闭`单人睡觉加速` ，起床则恢复默认流速

3.`视觉流畅优化`决定会不会发TimeSet数据包来改善时间流逝效果

4.所有配置项均由`加速速率`决定，附魔日晷的流速是`60`，正常为`1`

## 配置
> 配置文件位置：tshock/时间加速.json
```json5
{
  "指令加速": false,
  "全员睡觉加速": true,
  "单人睡觉加速": false,
  "视觉流畅优化": true,
  "加速速率": 180
}
```

## 更新日志

### v1.2.0
- 修复当服务器没人且/times all正在开启时，
- 第一个玩家进入服务器导致时间快进1到2分钟的BUG。

### v1.1.0
- 美化了菜单
- 优化了性能（不再发WorldInfo包）
- 加入了视觉流畅优化配置（是否发TimeSet包）
- 加入了睡觉也会提升加速的逻辑与相对开关指令

### v1.0.0
- 给修改时间流速加入了指令控制配置项的功能

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/Controllerdestiny/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love