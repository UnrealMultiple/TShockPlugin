# FullHealthRespawn 复活满血

- 作者: 不是现在
- 出处: [TShockPlugin 插件库](https://github.com/UnrealMultiple/TShockPlugin)
- 玩家死亡复活时自动恢复到当前最大生命值

## 功能说明

- 仅在玩家**死亡后复活**时触发，恢复到当前最大生命值（`statLifeMax2`，含生命水晶/果实提升与临时增益）
- 服务器端修改血量并发包同步，SSC 存档同样保存满血，非纯视觉恢复
- 对所有玩家开放，无需权限，无需配置

> 协议说明：原版魔镜/记忆药水/海螺/回归药水等回城，在协议层与死亡复活共用同一个
> Spawn 包（仅上下文不同，回城为 `RecallFromItem`，复活为 `ReviveFromDeath`）。
> v1.2.1 起按上下文过滤，**回城不再触发满血**；同时移除了进服（ServerJoin）满血。

## 指令

```
暂无
```

## 配置

```
暂无
```

## 更新日志

### v1.2.1
- 仅在死亡复活（`ReviveFromDeath`）时恢复满血：魔镜/海螺等回城不再触发
- 移除进服满血（ServerJoin 钩子），残血上线不再被自动拉满
- 整理 README，补充触发条件说明

### v1.2.0
- 使用 ConcurrentQueue 替代 Task.Delay，在 GameUpdate 中处理恢复
- 添加 manifest.json 文件

### v1.1.0
- 等待时间改为0.5秒
- 简化代码，移除不必要的功能

### v1.0.0
- 添加插件

## 致谢

- 学习了 [SpclPerm](https://github.com/UnrealMultiple/TShockPlugin/tree/master/src/SpclPerm) 服主特权插件的实现思路
- SpclPerm 作者: 羽学

## 反馈

- 优先发 issue -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区 trhub.cn ，bbstr.net , tr.monika.love
