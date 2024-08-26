# Rainbow Chat colorful chat

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: Professor X, NNT upgrade/Chinese, liver Emperor Xien, Yu Xue update 1449
- Source: None
- The color that makes the player speak differently every time.

## Update log

```
1.0.6
完善卸载函数

1.0.5
修复了玩家消息转发不了跨服聊天问题
恢复了对控制台的原始消息打印输出

1.0.4
更新了Config，把没用的几个值移除
加入了进服自动开启渐变色
（默认为关闭，这是影响所有玩家进服后是否自动开启的）
加入了子命令方便玩家在游戏内设置渐变颜色值（只能改自己的）

1.0.3
更新了子命令：/rc 渐变 与 /rc 随机
加入了配置文件专门调整渐变色值用（你直管改RGBA值就行了，A是透明度，RGB是红绿蓝）
```

## instruction

|grammar|Authority|illustrate|
| -------------- |: --------------------:|: -------:|
|/rainbowchat or /rc|rainbowchat.use|View menu|
|/Rainbowchat or /RC gradient|rainbowchat.use|Switch Rainbow Chat [Gradient Color] Function|
|/Rainbowchat or /RC random|rainbowchat.use|Switch Rainbow Chat [Random color] Function|
|/RC gradient starts XXX, XXX, XXX|rainbowchat.use|Change the start value of gradient color|
|/RC Gradient End XXX, XXX, XXX|rainbowchat.use|Change the end of the gradient color|



## Configuration
> Configuration file location: TSHOCK/RAINBOWChat.json
```json
{
   "使用说明": "权限名（rainbowchat.use） /rc 渐变 用指令修改的颜色不会写进配置文件，这里改的是全体默认渐变色，开启【随机】渐变会默认失效",
   "进服自动开启渐变色": false,
   "修改渐变开始颜色": {
     "R": 166,
     "G": 213,
     "B": 234
  },
   "修改渐变结束颜色": {
     "R": 245,
     "G": 247,
     "B": 175
  }
}
```
## feedback
- Priority to ISSUED-> Commonly maintained plug -in library: https://github.com/Controllerdestiny/tshockPlugin
- Two priority: TSHOCK official group: 816771079
- It may not be seen in a high probability, but it can also be: domestic community trhub.cn, bbstr.net, tr.monika.love