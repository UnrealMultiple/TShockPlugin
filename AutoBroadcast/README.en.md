# AutoBroadcast automatic broadcast

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Authors: Scavenger, Zaicon,GK minor improvement, Gan Di Xi 'en updated to 1449.
- Source: None
- Broadcast in the server regularly, or execute commands.

## Update log

```
暂无
```

## instruction

without

## deploy
> Configuration file location: tshock/AutoBroadcast.json
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
- Give priority to issued-> jointly maintained plug-in library: https://github.com/Controllerdestiny/TShockPlugin.
- Second priority: TShock official group: 816771079
- You can't see it with a high probability, but you can: domestic communities trhub.cn, bbstr.net, tr. monika.love.