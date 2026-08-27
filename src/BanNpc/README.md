# BanNpc 阻止怪物生成

- 作者: GK，唉唉有新增 Boss 全局滑动窗口速率限制功能
- 出处: GK

**扩展功能：Boss 全局滑动窗口限流**：针对指定 Boss NPC，配置多段时间窗口召唤次数阈值；任意窗口触发阈值后，全局锁死该 Boss 召唤，锁死冷却结束后重新开始统计；仅拦截玩家发起的 Boss 召唤，雕像、其他插件生成的 NPC 不做拦截。

## 指令

| 语法           |     权限     |     说明     |
|--------------|:----------:|:----------:|
| /bm add [ID] | bannpc.use | 添加禁止生成 NPC |
| /bm del [ID] | bannpc.use | 移除禁止生成 NPC |
| /bm list     | bannpc.use |  查看禁止生成表   |

## 配置
> 配置文件位置：tshock/BanNPC.zh-CN.json
```json5
{
  "阻止怪物生成表": [],
  "Boss速率限制配置": [
    {
      "NpcId": 146,//需要限流的 Boss NPC ID
      "PlayerCooldownSeconds": 0,
      "GlobalMaxAlive": 2,//该 Boss 同时存活最大数量，超出阻止召唤
      "DenyMessage": "召唤过于频繁，已暂时封禁，请稍后再尝试！",//触发限流时发送的提示文本
      "BroadcastDeny": true,
      "RateWindows": [
        {"WindowSeconds":60,"MaxCount":30},//`WindowSeconds`窗口时长 (秒)`MaxCount`该窗口内允许最大召唤次数
        {"WindowSeconds":300,"MaxCount":180}
      ]
    }
  ]
}

```

## 更新日志

### v1.0.0.5
- 新增 Boss 全局多段滑动窗口速率限制；触发阈值全局锁死 Boss 召唤；成功召唤才计入统计；区分玩家召唤与其他来源生成
- 
### v1.0.0.4
- 准备更新TS 5.2.1
- 修正文档
- 初始配置内容更改
### v1.0.0.3
- i18n
- README_EN.md 
### v1.0.0.2
- i18n预备
### v1.0.0.1
- 补全卸载函数

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
