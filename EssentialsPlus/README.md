# EssentialsPlus

- 作者: WhiteX等人，Average,Cjx，肝帝熙恩适配与修改
- 出处: [github](https://github.com/QuiCM/EssentialsPlus)
- 提供一些管理指令

## 更新日志

```
暂无
```

## 指令 ##

- **/find** 或 **/查找** -> 包含多个子命令：
    - **-command** 或 **-命令** -> 根据输入搜索特定命令，返回匹配的命令及其权限。
    - **-item** 或 **-物品** -> 根据输入搜索特定物品，返回匹配的物品及其 ID。
    - **-tile** 或 **-方块** -> 根据输入搜索特定方块，返回匹配的方块及其 ID。
    - **-wall** 或 **-墙壁** -> 根据输入搜索特定墙壁，返回匹配的墙壁及其 ID。
- **/freezetime** 或 **/冻结时间** -> 冻结或解冻时间。
- **/delhome** 或 **/删除家点** <家名称> -> 删除您的一个家点。
- **/sethome** 或 **/设置家点** <家名称> -> 设置您的一个家点。
- **/myhome** 或 **/我的家点** <家名称> -> 传送到您的一个家点。
- **/kickall** 或 **/踢所有人** <原因> -> 踢出服务器上的所有人。
- **/=** 或 **/重复** -> 重复您最后输入的命令（不包括其他 /= 的迭代）。
- **/more** 或 **/堆叠** -> 最大化手持物品的堆叠。子命令：
    - **-all** 或 **-全部** -> 最大化玩家背包中所有可堆叠的物品。
- **/mute** 或 **/静音管理** -> 覆盖 TShock 的 /mute。包含子命令：
    - **add** <名称> <时间> -> 为名称为 <名称> 的用户添加静音，时间为 <时间>。
    - **delete** <名称> -> 删除名称为 <名称> 的用户的静音。
    - **help** 或 **帮助** -> 输出命令信息。
- **/pvpget2** 或 **/切换PvP状态** -> 切换您的PvP状态。
- **/ruler** 或 **/测量工具** [1|2] -> 测量点 1 和点 2 之间的距离。
- **/send** 或 **/广播消息** -> 以自定义颜色广播消息。
- **/sudo** 或 **/代执行** -> 尝试让 <玩家> 执行 <命令>。包含子命令：
    - **-force** -> 强制运行命令，不受 <玩家> 的权限限制。
- **/timecmd** 或 **/定时命令** -> 在给定时间间隔后执行命令。包含子命令：
    - **-repeat** -> 每隔 <时间> 重复执行 <命令>。
- **/eback** 或 **/b** 或 **/返回** [步数] -> 将您带回到上一个位置。如果提供了 [步数]，则尝试将您带回 [步数] 步之前的位置。
- **/down** 或 **/下** [层数] -> 尝试向下移动您在地图上的位置。如果指定了 [层数]，则尝试向下移动 [层数] 次。
- **/left** 或 **/左** [层数] -> 与 /down [层数] 相同，但向左移动。
- **/right** 或 **/右** [层数] -> 与 /down [层数] 相同，但向右移动。
- **/up** 或 **/上** [层数] -> 与 /down [层数] 相同，但向上移动。



## 权限 ##

- essentials.find -> 允许使用 /find 命令。
- essentials.freezetime -> 允许使用 /freezetime 命令。
- essentials.home.delete -> 允许使用 /delhome 和 /sethome 命令。
- essentials.home.tp -> 允许使用 /myhome 命令。
- essentials.kickall -> 允许使用 /kickall 命令。
- essentials.lastcommand -> 允许使用 /= 命令。
- essentials.more -> 允许使用 /more 命令。
- essentials.mute -> 允许使用 /mute 命令。
- essentials.pvp -> 允许使用 /pvpget2 命令。
- essentials.ruler -> 允许使用 /ruler 命令。
- essentials.send -> 允许使用 /send 命令。
- essentials.sudo -> 允许使用 /sudo 命令。
- essentials.timecmd -> 允许使用 /timecmd 命令。
- essentials.tp.eback -> 允许使用 /eback 命令。
- essentials.tp.down -> 允许使用 /down 命令。
- essentials.tp.left -> 允许使用 /left 命令。
- essentials.tp.right -> 允许使用 /right 命令。
- essentials.tp.up -> 允许使用 /up 命令。

## 配置
    配置文件位置：tshock/EssentialsPlus.json
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
## 反馈
- 共同维护的插件库：https://github.com/Controllerdestiny/TShockPlugin
- 国内社区trhub.cn 或 TShock官方群等