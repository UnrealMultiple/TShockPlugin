# EssentialsPlus

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Authors: WhiteX et al., Average,Cjx, adaptation and modification of Gan Di Xi 'en, Cai update.
- - 出处: [github](https://github.com/QuiCM/EssentialsPlus) 
- Provide some management instructions

## Update log
```
1.0.2
修复数据库错误
1.0.1 
修复重启无法获取禁言的BUG, 重命名一些方法
```

## instruction##

- **/find** or **/search** -> 包含多个子命令:
    - **-command** or **-命令** -> 根据输入搜索特定命令,返回匹配的命令及其权限.
    - **-item** or **-物品** -> 根据输入搜索特定物品,返回匹配的物品及其 ID.
    - **-tile** or **-方块** -> 根据输入搜索特定方块,返回匹配的方块及其 ID.
    - **-wall** or **-墙壁** -> 根据输入搜索特定墙壁,返回匹配的墙壁及其 ID.
- **/freezetime** or **/freeze time** -> 冻结或解冻时间.
- **/delhome** or **/Delete Home Point**< home name >-> delete one of your home points.
- **/sethome** or **/Set Home Point**< home name >-> set one of your home points.
- **/myhome** or **/My Home Point**< home name >-> sent to one of your home points.
- **/kickall** or **/Kick everyone**< Reason >-> Kick out everyone on the server.
- **/=** or **/repeat** -> 重复您最后输入的命令(不包括其他 /= 的迭代).
- **/more** or **/stack** -> 最大化手持物品的堆叠.子命令:
    - **-all** or **-全部** -> 最大化玩家背包中所有可堆叠的物品.
- **/mute** or **/silent management** -> 覆盖 TShock 的 /mute.包含子命令:
    - **add**< name > < time >-> mute the user named < name > for < time >.
    - **delete**< name >-> mute the user with name < name >.
    - **help** or **help** -> 输出命令信息.
- **/pvpget2** or **/Toggle PvP state** -> 切换您的PvP状态.
- **/ruler** or **/measuring tool**[1|2]-> measure the distance between point 1 and point 2.
- **/send** or **/broadcast message** -> 以自定义颜色广播消息.
- **/sudo** or **/generation execution** -> 尝试让 <玩家> 执行 <命令>.包含子命令:
    - **-force** -> 强制运行命令,不受 <玩家> 的权限限制.
- **/timecmd** or **/timing command** -> 在给定时间间隔后执行命令.包含子命令:
    - **-repeat** -> 每隔 <时间> 重复执行 <命令>.
- **/eback** or **/b** or **/return**[steps]-> takes you back to the previous position. If [Steps] is provided, try to take you back to the position before [Steps].
- **/down** or **/down**[number of layers]-> try to move your position down on the map. If [Number of Layers] is specified, try to move down [Number of Layers] times.
- **/left** or **/Left**[number of layers]-> is the same as /down [number of layers], but moves to the left.
- **/right** or **/Right**[number of layers]-> same as /down [number of layers], but moved to the right.
- **/up** or **/Shang**[number of layers]-> is the same as /down [number of layers], but moves up.



## limit of authority##

- Essentials.find-> allows the /find command.
- Essentials.freezetime-> allows the use of the /freezetime command.
- Essentials.home.delete-> allows /delhome and /sethome commands.
- essentials . home . TP-->允许使用/我的家命令。
- essentials . kickall-->允许使用/kickall命令。
- essentials . last command-->允许使用 /= 命令。
- essentials.more ->允许使用/更多命令。
- essentials . mute-->允许使用/静音命令。
- essentials . PVP-->允许使用/pvpget2命令。
- essentials.ruler ->允许使用/标尺命令。
- essentials.send ->允许使用/发送命令。
- essentials . sudo-->允许使用/须藤命令。
- essentials . time cmd-->允许使用/timecmd命令。
- essentials . TP . eback-->允许使用/eback命令。
- essentials.tp.down ->允许使用/向下命令。
- essentials.tp.left ->允许使用/向左命令。
- essentials . TP . right-->允许使用/对命令。
- Essentials.tp.up-> allows the /up command.

## deploy
> Configuration file location: tshock/EssentialsPlus.json
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
- Give priority to issued-> jointly maintained plug-in library: https://github.com/Controllerdestiny/TShockPlugin.
- Second priority: TShock official group: 816771079
- You can't see it with a high probability, but you can: domestic communities trhub.cn, bbstr.net, tr. monika.love.