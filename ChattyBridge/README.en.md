# ChattyBridge chat bridge

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: Shao Siming
- Source: None
- Let multiple servers chat with each other.

## Update log

```
v1.0.0.2
补全卸载函数

v1.0.0.1
添加配置 `是否转发指令` 
```

## instruction

without

## deploy
> Configuration file location: tshock/ChattyBridge.json
```json
{
   "是否转发指令": false,
   "Rest地址": [
     "127.0.0.1:5000" 
  ],
   "服务器名称": "玄荒",
   "验证令牌": "", //该令牌不是Rest令牌
   "消息设置": {
     "聊天格式": "[{0}]{1}: {2}",  // 0为服务器名 1为玩家名 2为消息
     "离开格式": "[{0}]{1}离开服务器",
     "加入格式": "[{0}]{1}加入服务器" 
  }
}
```
## feedback
- Give priority to issued-> jointly maintained plug-in library: https://github.com/Controllerdestiny/TShockPlugin.
- Second priority: TShock official group: 816771079
- You can't see it with a high probability, but you can: domestic communities trhub.cn, bbstr.net, tr. monika.love.