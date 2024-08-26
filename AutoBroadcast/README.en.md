# Autobroadcast automatic broadcast

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: scavenger, zaicon, GK small improvement, liver Emperor Xien updated to 1449
- Source: None
- Broadcast or execute commands in the server regularly.

## Update log

```
暂无
```

## instruction

none

## Configuration
> Configuration file location: TSHOCK/AutobroadCast.json
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
## feedback
- Priority to ISSUED-> Commonly maintained plug -in library: https://github.com/Controllerdestiny/tshockPlugin
- Two priority: TSHOCK official group: 816771079
- It may not be seen in a high probability, but it can also be: domestic community trhub.cn, bbstr.net, tr.monika.love