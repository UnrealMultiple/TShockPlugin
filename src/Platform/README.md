# Platform 判断玩家设备

- 作者: Cai、cc04
- 出处: [github](https://github.com/ACaiCat/CaiPlugins)
- 判断玩家是用什么设备进来的（只能判断类型）

> [!NOTE]
> 此插件是用于作为前置插件供其他插件调用的(可用于聊天前缀、计分板判定等)  
> 目前支持区分的的PE、Stadia、XBOX、PSN、Editor、Switch、PC  

## 更新日志

### v2026.02.15.0
- 支持Terraria 1.4.5

### v2025.3.30.1 
- 支持在聊天格式中显示设备

## 聊天前缀
可以在`tshock/config.json`聊天格式`ChatFormat`中使用占位符`%platform%`/`%device%`来添加设备前缀
```json5
"ChatFormat": "[%platform%]{1}{2}{3}: {4}", 
"ChatAboveHeadsFormat": "{1}{2}{3}", //不支持
"EnableChatAboveHeads": false, //不支持, 请保持false
```

## 指令

| 语法              |      权限      |    说明    |
|-----------------|:------------:|:--------:|
| /platform <玩家名> | platform.use | 查看玩家游玩设备 |

## 配置

```
暂无
```
## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
