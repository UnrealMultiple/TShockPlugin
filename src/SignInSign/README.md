# SignInSign 告示板登录

- 作者: Soofa、羽学、少司命
- 出处: [github](https://github.com/Soof4/SignInSign)  
+ 这是一个Tshock服务器插件主要用于：  
+ 玩家进服可弹窗显示告示牌，可通过告示牌注册登录、获取物品、BUFF、传送、记录玩家密码等功能
+ 注意：
+ 装了此插件玩家将无法破坏服务器内的所有告示牌，除非拥有sign.edit权限。
+ 插件会根据配置文件的内容在出生点建一个隐藏告示牌，
+ 如需更改告示牌内容：
+ 1.在服务器里使用已登录角色挖掉告示牌
+ 2.修改【告示牌登录.json】配置文件中的“创建告示牌的内容”
+ 3.输入指令：/gs r [权限：signinsign.setup]

## 更新日志
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
## 指令

| 语法           |        权限         |   说明   |
| -------------- | :-----------------: | :------: |
| /gs r |  signinsign.setup  | 重设告示牌 |
| /gs s | signinsign.tp   |设置告示牌传送点并自动写入配置文件|
| 无 | sign.edit    |允许破坏告示牌权限|


## 配置
> 配置文件位置：tshock/告示牌登录.json
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
## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love