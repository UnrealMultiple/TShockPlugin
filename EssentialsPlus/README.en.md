# ESSENTIALSPLUS

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: Whitex et al.
- - 出处: [github](https://github.com/QuiCM/EssentialsPlus) 
- Provide some management instructions

## Update log
```
1.0.2
修复数据库错误
1.0.1 
修复重启无法获取禁言的BUG, 重命名一些方法
```

## Instruction##

- **/find** or **/Search** -> 包含多个子命令:
    - **-command** or **-命令** -> 根据输入搜索特定命令,返回匹配的命令及其权限.
    - **-item** or **-物品** -> 根据输入搜索特定物品,返回匹配的物品及其 ID.
    - **-tile** or **-方块** -> 根据输入搜索特定方块,返回匹配的方块及其 ID.
    - **-wall** or **-墙壁** -> 根据输入搜索特定墙壁,返回匹配的墙壁及其 ID.
- **/freezetime** or **/Freezing time** -> 冻结或解冻时间.
- **/Delhome** or **/Delete home**<Home Name> -> Delete one of your home points.
- **/sethome** or **/Set home points**<Home Name> -> Set a home point of you.
- **/MyHome** or **/My home**<Home Name> -> Transmit to a home point.
- **/kickall** or **/Kick everyone**<Reasonable> -> Kick out everyone on the server.
- **/=** or **/repeat** -> 重复您最后输入的命令(不包括其他 /= 的迭代).
- **/more** or **/Stack** -> 最大化手持物品的堆叠.子命令:
    - **-all** or **-全部** -> 最大化玩家背包中所有可堆叠的物品.
- **/mute** or **/Mute management** -> 覆盖 TShock 的 /mute.包含子命令:
    - **ADD**<Name> <Time> -> adds silence as a user with the name <imser>, and the time is <Time>.
    - **delete**<Name> -> Delete the silence of users with names.
    - **help** or **help** -> 输出命令信息.
- **/PVPGET2** or **/Switch PVP status** -> 切换您的PvP状态.
- **/Ruler** or **/Measurement tool**[1|2] -> The distance between the measurement point 1 and point 2.
- **/send** or **/Broadcast message** -> 以自定义颜色广播消息.
- **/Sudo** or **/Active execution** -> 尝试让 <玩家> 执行 <命令>.包含子命令:
    - **-force** -> 强制运行命令,不受 <玩家> 的权限限制.
- **/timecmd** or **/Timing command** -> 在给定时间间隔后执行命令.包含子命令:
    - **-repeat** -> 每隔 <时间> 重复执行 <命令>.
- **/eBack** or **/B** or **/return**[Step Number] -> Bring you back to the previous position. If [step] is provided, try to bring you back to the position before the [step] step.
- **/don** or **/Down**[Layer] -> Try to move your position on the map down. If [layer] is specified, try to move down [layer number] times.
- **/left** or **/Left**[Layer] -> Like /DOWN [layer number], but moving left.
- **/Right** or **/right**[Section] -> Like /DOWN [layer number], but move to the right.
- **/up** or **/superior**[Layer] -> Like /DOWN [layer number], but moving upward.



## Authority##

- Essentials.find-> Allow the /find command.
- Essentials.freezetime-> Allows the /freezetime command.
- Essentials.home.delete-> Allows the use of /delhome and /setHome commands.
- Essentials.home.tp-> Allow the /MyHome command.
- Essentials.kickall-> Allow the /Kickall command.
- Essentials.lastcommand-> Allows /= command.
- Essentials.more-> Allow the /more command.
- Essentials.mute-> Allow the /Mute command.
- Essentials.pvp-> Allows the /PVPGET2 command.
- Essentials.ruler-> Allow the /Ruler command.
- Essentials.send-> Allow the /Send command.
- Essentials.sudo-> Allow the /Sudo command.
- Essentials.Timecmd-> Allow the /Timecmd command.
- Essentials.tp.eBack-> Allows the /eBack command.
- Essentials.tp.down-> Allow the /don command.
- Essentials.tp.left-> Allow the /left command.
- Essentials.tp.richt-> Allow the /Right command.
- Essentials.tp.up-> Allow the /up command.

## Configuration
> Configuration file location: TSHOCK/ESSENTIALSPLUS.JSON
```json
{
   "Pvp禁用命令": [
     "eback" 
  ],
   "回退位置历史记录": 10,
   "MySql主机": "如使用Mysql这里需要配置完整信息",
   "MySql数据库名称": "",
   "MySql用户": "",
   "MySql密码": "" 
}
```
## feedback
- Priority to ISSUED-> Commonly maintained plug -in library: https://github.com/Controllerdestiny/tshockPlugin
- Two priority: TSHOCK official group: 816771079
- It may not be seen in a high probability, but it can also be: domestic community trhub.cn, bbstr.net, tr.monika.love