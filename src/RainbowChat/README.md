# Rainbow Chat 五彩斑斓聊天

- 作者: Professor X制作,nnt升级/汉化,肝帝熙恩、羽学更新1449
- 出处: 无
- 使玩家每次说话的颜色不一样.

## 更新日志

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

## 指令

| 语法           |        权限         |   说明   |
| -------------- | :-----------------: | :------: |
| /rainbowchat 或 /rc | rainbowchat.use  |   查看菜单   |
| /rainbowchat 或 /rc 渐变| rainbowchat.use  |   开关彩虹聊天【渐变色】功能   |
| /rainbowchat 或 /rc 随机| rainbowchat.use  |   开关彩虹聊天【随机色】功能   |
| /rc 渐变 开始 xxx,xxx,xxx| rainbowchat.use  |   更改渐变色开始值   |
| /rc 渐变 结束 xxx,xxx,xxx| rainbowchat.use  |   更改渐变色结束值   |



## 配置
> 配置文件位置：tshock/RainbowChat.json
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
## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
