# CaiPacketDebug 数据包调试

- 作者: Cai
- 出处: 本仓库
- 本插件用于查看客户端、服务端的数据包内容

> [!NOTE]  
> 需要安装前置：TrProtocol


## 指令

| 语法       |         权限         |    说明     |
|----------|:------------------:|:---------:|
| /cpd     | CaiPacketDebug.Use | 查看调试启用状态  |
| /cpd stc | CaiPacketDebug.Use | S->C数据包调试 |
| /cpd cts | CaiPacketDebug.Use | C->S数据包调试 |


## 配置
> 配置文件位置：tshock/CaiPacketDebug.zh-CN.json
```json5   
{
  "C->S": { //客户端向服务器发送的数据包
    "自启动": false, //打开服务器时自动启用调试
    "白名单模式": false, //只显示白名单列表的数据包
    "白名单模式数据包": [
      1,
      2,
      3
    ],
    "排除数据包": [ //排除以下数据包
      114,
      514
    ]
  },
  "S->C": { //服务器向客户端发送的数据包
    "自启动": false,
    "白名单模式": false,
    "白名单模式数据包": [
      1,
      2,
      3
    ],
    "排除数据包": [
      23
    ]
  }
}
```

## 更新日志

```
2025.1.25.2 添加 GetString
2025.1.25.1 提高插件优先级，显示客户端索引, 修复无法解析NetManager发送的数据包
2024.11.30.0 使用lazyapi,i18n预备
2024.11.10.0 添加插件
```

## 反馈

- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
