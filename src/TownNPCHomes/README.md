# TownNPCHomes NPC快速回家

- 作者: 棱镜、羽学
- 出处: [github](https://www.bbstr.net/r/72/)
- 这是一个Tshock服务器插件主要用于NPC能在允许回家的情况下快速回家，  
- 如果搭配西江小子的VBY.GameContentModify插件可以不输入指令直接分配房屋即可传送NPC回家。
- VBY.GameContentModify：[github](https://github.com/xuyuwtu/MyPlugin/tree/master/src/VBY)

## 指令

| 语法       |          权限          |          说明           |
|----------|:--------------------:|:---------------------:|
| /npchome | tshock.world.movenpc | 该权限为原版TShock分配NPC入住权限 |

## 配置

```json5
暂无
```

## 更新日志


### v1.1.2
- 添加英文翻译 
### v1.1.1
- 完善卸载函数
### v1.1.0
- 修复了原插件分配住所后城镇NPC会人间蒸发的BUG
- 现在即使不输入命令只要npc处于晚上或雨天都会秒传回家

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love