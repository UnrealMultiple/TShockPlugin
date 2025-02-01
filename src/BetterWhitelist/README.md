# BetterWhitelist 更好的白名单

- 作者: 豆沙，肝帝熙恩修,Cai改
- 出处: [gitee](https://gitee.com/Crafty/BetterWhitelist)
- 将玩家名字加入白名单，仅在白名单内的玩家可进入游戏


## 指令

| 语法                | 权限        | 说明          |
|-------------------|-----------|-------------|
| `/bwl help`       | `bwl.use` | 显示帮助信息      |
| `/bwl add {name}` | `bwl.use` | 添加玩家名到白名单中  |
| `/bwl del {name}` | `bwl.use` | 将玩家移出白名单    |
| `/bwl list`       | `bwl.use` | 显示白名单上的全部玩家 |
| `/bwl true`       | `bwl.use` | 启用插件        |
| `/bwl false`      | `bwl.use` | 关闭插件        |

## 配置

> 配置文件位置：tshock/BetterWhitelist.zh-CN.json
```json5
{
  "白名单玩家": [],
  "插件开关": false,
  "连接时不在白名单提示": "你不在服务器白名单中！"
}
```

## 更新日志

```
v2.6.4
添加 GetString
v2.6.2
修复本插件根本无效的bug
v2.6.1
准备更新TS 5.2.1,修正文档，初始配置内容更改
v2.6
添加英文翻译
```

## 反馈

- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
