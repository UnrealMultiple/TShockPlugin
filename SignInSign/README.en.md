# SIGNINSIGN notice board login

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: SOOFA, Yu Xue, Shao Si Ming
- - 出处: [github](https://github.com/Soof4/SignInSign) 
+ This is a TSHOCK server plug -in mainly used for:
+ Players can enter the service to display the notice sign, you can register login, obtain items, BUFF, transmit, record the player password and other functions
+ Note:
+ Instant this plug -in player will not be able to destroy all the notices in the server unless there is Sign.edit permissions.
+ The plug -in will build a hidden notice on the birth point according to the content of the configuration file.
+ If you need to change the notice content:
+ 1. Use the login character in the server to dig out the notice sign
+ 2. Modify the "Create the Content of the Creation Sign" in the configuration file
+ 3. Input instructions:/gs R [permissions: signinsign.setup]

## Update log
```
1.0.5
完善卸载函数

1.0.4
加入了对放置区域墙壁是否为空的判定
降低插件放置告示牌的优先级（避让CreateSpawn插件）
给/gs s指令加个权限方便玩家自用
把点击告示牌执行命令的身份改为玩家本人（临时超管组）
使用/gs r会清空传送坐标 避免重置服务器沿用

1.0.3
少司命修复了多人进服告示牌不弹窗问题，加入了阻止破坏修改告示牌图格
羽学优化了指令，使用/gs s可快速设置当前位置为传送点，使用/gs r 可重设告示牌（自动执行/reload）
对告示牌传送加入了对：玩家登录、打开传送开关、传送点坐标非0的判定

1.0.2
修复了一些空引用的BUG
加了Reload方法与大量配置项
```
## instruction

|grammar|Authority|illustrate|
| -------------- |: --------------------:|: -------:|
|/GS R|signinsign.setup|Reset the notice sign|
|/GS S|signinsign.tp|Set the notice sign transfer point and automatically write the configuration file|
|none|sign.editit|Allow destruction notice permissions|


## Configuration
> Configuration file location: TSHOCK/Notice Sign Login.json
```json
{
   "是否开启注册登录功能": true,
   "记录角色密码": false,
   "对登录玩家显示告示牌": true,
   "是否允许点击告示牌": true,
   "点击告示牌是否发广播": false,
   "创建告示牌的内容,重设指令:/gs r": "欢迎来到开荒服！！\n本服支持PE/PC跨平台联机游玩\n每25分钟清理世界与Boss战排名统计\n更多指令教学请输入/help\n点击告示牌可进行传送\n\nTShock官方群：816771079\n",
   "点击告示牌的广播/仅使用者可见": "在本告示牌依序输入2次：\n[c/F7CCF0:123456]  进行注册登录。",
   "试图破坏告示牌的广播": "此告示牌不可被修改!",
   "点击告示牌执行什么指令": [],
   "点击告示牌给什么BUFF": [],
   "点击告示牌BUFF时长/分钟": 10,
   "点击告示牌给什么物品": [],
   "点击告示牌给物品数量": 1,
   "点击告示牌是否传送,设置指令:/gs s": false,
   "点击告示牌传送坐标.X": 0.0,
   "点击告示牌传送坐标.Y": 0.0,
   "点击告示牌传送特效": 10
}
```
## feedback
- Priority to ISSUED-> Commonly maintained plug -in library: https://github.com/Controllerdestiny/tshockPlugin
- Two priority: TSHOCK official group: 816771079
- It may not be seen in a high probability, but it can also be: domestic community trhub.cn, bbstr.net, tr.monika.love