# RainbowChat 彩虹聊天

- 作者: Professor X制作,nnt升级/汉化,肝帝熙恩、羽学更新1449
- 出处: 无
- 使玩家每次说话的颜色不一样

## 指令

| 语法                             |               别名               |        权限         |          说明          |
|--------------------------------|:------------------------------:|:-----------------:|:--------------------:|
| /rc                            |          /rainbowchat          |  rainbowchat.use  |         查看菜单         |
| /rc gradient                   |        /rainbowchat 渐变         |  rainbowchat.use  | 玩家自己的开关彩虹聊天【渐变色】功能开关 |
| /rc random                     |        /rainbowchat 随机         |  rainbowchat.use  |  玩家自己的彩虹聊天【随机色】功能开关  |
| /rc gradient start xxx,xxx,xxx | /rainbowchat 渐变 开始 RRR,GGG,BBB |  rainbowchat.use  |       更改渐变色开始值       |
| /rc gradient stop xxx,xxx,xxx  | /rainbowchat 渐变 结束 RRR,GGG,BBB |  rainbowchat.use  |       更改渐变色结束值       |
| /rc on 或 off                   |      /rainbowchat 开启 或 关闭      | rainbowchat.admin |      更改插件的全局开关       |
| /rc rswitch                    |       /rainbowchat 随机开关        | rainbowchat.admin |     更改插件的随机色全局开关     |
| /rc gswitch                    |       /rainbowchat 渐变开关        | rainbowchat.admin |     更改插件的渐变色全局开关     |



## 配置
> 配置文件位置：tshock/RainbowChat.json
```json5
{
  "使用说明": "权限名（rainbowchat.use） /rc 渐变 用指令修改的颜色不会写进配置文件，这里改的是全体默认渐变色，开启【随机色】渐变会默认失效",
  "插件开关": true,
  "错误提醒": true,
  "进服自动开启渐变色": true,
  "全局随机色开关": true,
  "全局渐变色开关": true,
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

## 更新日志


### v1.0.7
- 修复了指令屏蔽问题
- 加入了全局控制开关的指令与对应配置项
- 修复了渐变色聊天时的物品图标显示
- 加入了当玩家使用指令开启渐变时默认会关闭随机色的逻辑
- 关于随机色的图标修复工作：
- 羽学表示放弃，甚至对这个功能是否有存在的必要性，都持有怀疑态度。

### v1.0.6
- 完善卸载函数

### v1.0.5
- 修复了玩家消息转发不了跨服聊天问题
- 恢复了对控制台的原始消息打印输出

### v1.0.4
- 更新了Config，把没用的几个值移除
- 加入了进服自动开启渐变色 （默认为关闭，这是影响所有玩家进服后是否自动开启的）
- 加入了子命令方便玩家在游戏内设置渐变颜色值（只能改自己的）

### v1.0.3
- 更新了子命令：/rc 渐变 与 /rc 随机
- 加入了配置文件专门调整渐变色值用（你直管改RGBA值就行了，A是透明度，RGB是红绿蓝）

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
