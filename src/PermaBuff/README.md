# PermaBuff 永久Buff

- 作者: 少司命
- 出处: 无
- 让玩家永久拥有 buff

## 指令

| 语法                          |       权限       |          说明           |
|-----------------------------|:--------------:|:---------------------:|
| /permabuff [buffID]         | permabuff.use  |  给予自身永久 buff(再次使用移除)  |
| /gpermabuff [玩家名称] [buffid] | gpermabuff.use | 给予玩家一个永久 buff(再次使用移除) |
| /clearbuffs                 | clearbuffs.use |      清空自身所有 buff      |

## 配置
> 配置文件位置：tshock/permbuff.json
```json5
{
  "不可添加buff": []
}
```
## 更新日志

```
v.1.0.1
- 完善卸载函数
```

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
