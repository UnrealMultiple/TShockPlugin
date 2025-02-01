# AutoBroadcast 自动广播

- 作者: Scavenger，Zaicon,GK 小改良，肝帝熙恩更新至1449
- 出处: [github](https://github.com/Scavenger3/AutoBroadcast)
- 定时在服务器中广播，或执行命令。


## 指令
```
无
```
## 配置
> 配置文件位置：tshock/AutoBroadcast.zh-CN.json
```json5
{
  "广播列表": [
    {
      "广播名称": "示例广播",
      "启用": true,
      "广播消息": [
        "/say Ciallo～(∠・ω< )⌒★",
        "自动广播执行了服务器指令/say Ciallo～(∠・ω< )⌒★"
      ],
      "RGB颜色": [
        255,
        234,
        115
      ],
      "时间间隔": 600, //计时广播间隔,设为0则不启用计时广播
      "延迟执行": 0, //计时广播延迟时间,设为0则不延迟
      "广播组": [], //不设置默认所有组均可触发
      "触发词语": [], //触发词语
      "触发整个组": false //触发词语时，向整个组发送广播
    }
  ]
}
```

## 更新日志

```
v1.1.2
为ColorRgbBytes添加[JsonIgnore]
v1.1.1
几乎完全重写逻辑
v1.0.9
修复了启用实际上是关闭的问题
v1.0.8
准备更新TS 5.2.1,修正文档，初始配置内容更改
v1.0.6
修复了初始配置文件的问题
v1.0.5
i18n 和 README_EN.md
v1.0.4
i18n预定
```

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
