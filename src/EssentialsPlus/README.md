# EssentialsPlus

- 作者: WhiteX等人，Average,Cjx，肝帝熙恩适配与修改,Cai更新
- 出处: [github](https://github.com/QuiCM/EssentialsPlus)
- Essentials+ 是一种组合，用于改进和优化 Essentials 和 MoreAdminCommands 中的某些功能。所有命令都是异步执行的。不包括 Flag 命令。

## 指令

- **/find** 或 **/查找** -> 包含多个子命令：
    - **-command** 或 **-命令** -> 根据输入搜索特定命令，返回匹配的命令及其权限。
    - **-item** 或 **-物品** -> 根据输入搜索特定物品，返回匹配的物品及其 ID。
    - **-tile** 或 **-方块** -> 根据输入搜索特定方块，返回匹配的方块及其 ID。
    - **-wall** 或 **-墙壁** -> 根据输入搜索特定墙壁，返回匹配的墙壁及其 ID。
- **/freezetime** 或 **/冻结时间** -> 冻结或解冻时间。
- **/delhome** 或 **/删除家点** `<家名称>` -> 删除您的一个家点。
- **/sethome** 或 **/设置家点** `<家名称>` -> 设置您的一个家点。
- **/myhome** 或 **/我的家点** `<家名称>` -> 传送到您的一个家点。
- **/kickall** 或 **/踢所有人** `<原因>` -> 踢出服务器上的所有人。有效标志：`-nosave` -> 踢出时不会保存 SSC 背包。
- **/=** 或 **/重复** -> 重复您最后输入的命令（不包括其他 /= 的迭代）。
- **/more** 或 **/堆叠** -> 最大化手持物品的堆叠。子命令：
    - **-all** 或 **-全部** -> 最大化玩家背包中所有可堆叠的物品。
- **/mute** 或 **/静音管理** -> 覆盖 TShock 的 /mute。包含子命令：
    - **add** `<名称> <时间>` -> 为名称为 `<名称>` 的用户添加静音，时间为 `<时间>`。
    - **delete** `<名称>` -> 删除名称为 `<名称>` 的用户的静音。
    - **help** 或 **帮助** -> 输出命令信息。
- **/pvpget** 或 **/切换PvP状态** -> 切换您的 PvP 状态。
- **/ruler** 或 **/测量工具** `[1|2]` -> 测量点 1 和点 2 之间的距离。
- **/sudo** 或 **/代执行** `[flag] <玩家> <命令>` -> 让 `<玩家>` 执行 `<命令>`。有效标志：`-force` -> 强制执行命令，忽略 `<玩家>` 的权限限制。拥有 `essentials.sudo.super` 权限的玩家可以对任何人使用 /sudo。
- **/timecmd** 或 **/定时命令** `[flag] <时间> <命令>` -> 在 `<时间>` 后执行 `<命令>`。有效标志：`-repeat` -> 每隔 `<时间>` 重复执行 `<命令>`。
- **/back** 或 **/返回** `[步数]` -> 将您带回到上一个位置。如果提供了 `[步数]`，则尝试将您带回 `[步数]` 步之前的位置。
- **/down** 或 **/下** `[层数]` -> 尝试向下移动您在地图上的位置。如果指定了 `[层数]`，则尝试向下移动 `[层数]` 次。
- **/left** 或 **/左** `[层数]` -> 类似于 /down `[层数]`，但向左移动。
- **/right** 或 **/右** `[层数]` -> 类似于 /down `[层数]`，但向右移动。
- **/up** 或 **/上** `[层数]` -> 类似于 /down `[层数]`，但向上移动。

## 权限

- `essentials.find` -> 授予访问 `/find` 命令的权限。
- `essentials.freezetime` -> 授予访问 `/freezetime` 命令的权限。
- `essentials.home.delete` -> 授予访问 `/delhome` 和 `/sethome` 命令的权限。
- `essentials.home.tp` -> 授予访问 `/myhome` 命令的权限。
- `essentials.kickall` -> 授予访问 `/kickall` 命令的权限。
- `essentials.lastcommand` -> 授予访问 `/=` 命令的权限。
- `essentials.more` -> 授予访问 `/more` 命令的权限。
- `essentials.mute` -> 授予访问改进后的 `/mute` 命令的权限。
- `essentials.pvp` -> 授予访问 `/pvpget` 命令的权限。
- `essentials.ruler` -> 授予访问 `/ruler` 命令的权限。
- `essentials.send` -> 授予访问 `/send` 命令的权限。
- `essentials.sudo` -> 授予访问 `/sudo` 命令的权限。
- `essentials.sudo.force` -> 扩展 `sudo` 的功能。
- `essentials.sudo.super` -> 允许对任何人使用 `sudo`。
- `essentials.sudo.invisible` -> 使通过 `sudo` 执行的命令不可见。
- `essentials.timecmd` -> 授予访问 `/timecmd` 命令的权限。
- `essentials.tp.back` -> 授予访问 `/back` 命令的权限。
- `essentials.tp.down` -> 授予访问 `/down` 命令的权限。
- `essentials.tp.left` -> 授予访问 `/left` 命令的权限。
- `essentials.tp.right` -> 授予访问 `/right` 命令的权限。
- `essentials.tp.up` -> 授予访问 `/up` 命令的权限。

## 配置
> 配置文件位置：tshock/EssentialsPlus.json
```json5
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

## 更新日志
```
1.0.7
EssentialsPlus的config使用lazyapi，禁言判断uuid，名字，ip
1.0.4
添加西班牙语，修正部分内容
1.0.3
i18n完成，且预置es-EN
1.0.2
修复数据库错误
1.0.1 
修复重启无法获取禁言的BUG, 重命名一些方法
```

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
