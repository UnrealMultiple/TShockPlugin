# ProxyProtocolSocket 反代真实IP

- 作者: [LaoSparrow](https://github.com/LaoSparrow)
- 出处: https://github.com/LaoSparrow/ProxyProtocolSocket
- 让 TShock 支持 `proxy protocol`, 获取用户真实IP（让内网穿透可获取玩家真实IP）
- 需要搭配开启了 `proxy protocol` 功能的 `frp`, `nginx` 等反代软件使用
- 安装此插件后游戏客户端只能通过反代连接 TShock, 直接连接 TShock 会被拒绝接入

## 指令

| 语法 | 权限 | 说明 |
|:--:|:--:|:--:|
| 无  | 无  | 无  |

## 配置
> 配置文件路径：tshock/ProxyProtocolSocket.json
```json5
{
  "Settings": {
    "log_level": "Info",
    "timeout": 1000
  }
}
```

| 参数        | 说明                                              |                值范围                | 默认值  |
|:----------|:------------------------------------------------|:---------------------------------:|:----:|
| log_level | 终端日志等级                                          | None, Error, Warning, Info, Debug | Info |
| timeout   | 超时毫秒数(接收到 proxy protocol 数据前等待的时间), -1代表不启用超时功能 |             -1 或 正整数              | 1000 |


## 更新日志

### v2.1
- 重构 Parsers

### v2.0
- 适配 TShock 5.2

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
