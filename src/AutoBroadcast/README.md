# AutoBroadcast 自动广播

- 作者: Scavenger，Zaicon,GK 小改良，肝帝熙恩更新至1449
- 出处: [github](https://github.com/Scavenger3/AutoBroadcast)
- 定时在服务器中广播，或执行命令。

## 更新日志

```
v1.0.5
i18n 和 README_EN.md
v1.0.4
i18n预定
```

## 指令

无

## 配置
> 配置文件位置：tshock/AutoBroadcast.json
```json
{
  "广播列表": [
    {
      "广播名称": "实例广播",
      "启用": false,
      "广播消息": [
        "这是一条广播",
        "每五分钟执行一次",
        "可以执行命令",
        "/time noon"
      ],
      "RGB颜色": [255.0, 0.0, 0.0],
      "时间间隔": 300,
      "延迟执行": 60,
      "广播组": [],
      "触发词语": [], //可以设置聊天消息关键词触发此广播
      "触发整个组": false //开启后将会对'广播组'内所有玩家执行此广播
    }
  ]
}
```
## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
