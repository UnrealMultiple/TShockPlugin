# Chattybridge Chat Qiaoqiao

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: Shao Si Ming
- Source: None
- Let multiple servers chat with each other

## Update log

```
v1.0.0.2
补全卸载函数

v1.0.0.1
添加配置 `是否转发指令` 
```

## instruction

none

## Configuration
> Configuration file location: TSHOCK/COTTYBRIDGE.JSON
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
- Priority to ISSUED-> Commonly maintained plug -in library: https://github.com/Controllerdestiny/tshockPlugin
- Two priority: TSHOCK official group: 816771079
- It may not be seen in a high probability, but it can also be: domestic community trhub.cn, bbstr.net, tr.monika.love