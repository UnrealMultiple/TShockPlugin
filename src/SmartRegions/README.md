# Smart Regions 智能区域

- 作者: GameRoom，肝帝熙恩汉化修复
- 出处: [Github](https://github.com/ZakFahey/SmartRegions)
- 当玩家进入一个区域时，这个插件会运行你想要的任何命令
- 你可以用它来设置一个玩家的团队，治疗他们，给他们物品，或者任何你想要的东西。可能性是无穷无尽的
- 可以使用占位符 `[PLAYERNAME]`，插件会将其替换为该区域中的玩家
- 兼容旧版本，但是sqlite和json二选一
- 以 -- 结束区域名称，并且仅当它具有所有重叠区域中最高的 Z 值时，它才会执行


## 指令

| 语法           |         权限          |   说明   |
|--------------|:-------------------:|:------:|
| /smartregion | SmartRegions.manage | 智能区域管理 |
| /replace     | SmartRegions.manage |  区域替换  |

## 配置
> 配置文件位置：tshock/SmartRegions/SmartRegions.sqlite或config.json

## 更新日志

```
v1.4.3
完善卸载函数

v1.4.2
修复初始化顺序错误
```


## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
