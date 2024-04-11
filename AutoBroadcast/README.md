# AutoBroadcast 自动广播

- 作者: Scavenger
- 出处: 无
- 定时在服务器中广播，或执行命令。

## 更新日志

```
暂无
```

## 指令

无

## 配置

```json
{
  "Broadcasts": [
    {
      "Name": "E实例广播",
      "Enabled": false,
      "Messages": [
        "这是一条广播",
        "每五分钟执行一次",
        "可以执行命令",
        "/time noon"
      ],
      "ColorRGB": [255.0, 0.0, 0.0],
      "Interval": 300,
      "StartDelay": 60,
      "Groups": [],
      "TriggerWords": [],
      "TriggerToWholeGroup": false
    }
  ]
}
```
