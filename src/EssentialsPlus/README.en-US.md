# EssentialsPlus

- Authors: WhiteX等人，Average,Cjx，肝帝熙恩适配与修改,Cai更新
- Source: [github](https://github.com/QuiCM/EssentialsPlus)
- Essentials+ is a combination of things from Essentials and things from MoreAdminCommands made better. All commands run asynchronously. It does not include Sign Commands.所有命令都是异步执行的。不包括 Flag 命令。

## Commands

- **/find** -> Takes a variety of subcommands:
  - **-command** -> Searches for a specific command based on input, returning matching commands and their permissions.
  - **-item** -> Searches for a specific item based on input, returning matching items and their IDs.
  - **-tile** -> Searches for a specific tile based on input, returning matching tiles and their IDs.
  - **-wall** -> Searches for a specific wall based on input, returning matching walls and their IDs.
- **/freezetime** -> Freezes and unfreezes time.
- **/delhome** `<home name>` -> Deletes a home specified by `<home name>`.
- **/sethome** `<home name>` -> Sets a home named `<home name>`.
- **/myhome** `<home name>` -> Teleports you to your home named `<home name>`.
- **/kickall** 或 **/踢所有人** `<原因>` -> 踢出服务器上的所有人。**/kickall** `<flag> <reason>` -> Kicks every player for `<reason>`. Valid flag: `-nosave` -> The kick doesn't save SSC inventory.
- **/=** -> Repeats your last entered command (not including other instances of /=).
- **/more** -> Maximizes the item stack of the held item. Includes subcommands:子命令：
  - **-all** -> Maximizes all stackable items in the player's inventory.
- **/mute** -> Overwrites TShock's /mute. Includes subcommands:包含子命令：
  - **add** `<名称> <时间>` -> 为名称为 `<名称>` 的用户添加静音，时间为 `<时间>`。
  - **delete** `<name>` -> Removes the mute for the player `<name>`.
  - **help** -> Displays command info.
- **/pvpget** -> Enables or disables your PvP status.
- **/ruler** `[1|2]` -> Measures the distance between points 1 and 2.
- **/sudo** `[flag] <player> <command>` -> Makes `<player>` execute `<command>`. Valid flag: `-force` -> Forces the player to run the command, ignoring permission checks. Players with the `essentials.sudo.super` permission can use /sudo on anyone.有效标志：`-force` -> 强制执行命令，忽略 `<玩家>` 的权限限制。拥有 `essentials.sudo.super` 权限的玩家可以对任何人使用 /sudo。
- **/timecmd** `[flag] <time> <command>` -> Executes `<command>` after `<time>`. Valid flag: `-repeat` -> Repeats `<command>` every `<time>`.有效标志：`-repeat` -> 每隔 `<时间>` 重复执行 `<命令>`。
- **/eback** `[steps]` -> Returns you to a previous location. If `[steps]` is provided, it takes you back to your position `[steps]` steps ago.如果提供了 `[步数]`，则尝试将您带回 `[步数]` 步之前的位置。
- **/down** `[levels]` -> Moves you down on the map. If `[levels]` is specified, it attempts to move you down `[levels]` levels.如果指定了 `[层数]`，则尝试向下移动 `[层数]` 次。
- **/left** `[levels]` -> Similar to `/down [levels]`, but moves left.
- **/right** `[levels]` -> Similar to `/down [levels]`, but moves right.
- **/up** `[levels]` -> Similar to `/down [levels]`, but moves upwards.

## Permissions

- `essentials.find` -> Grants access to the `/find` command.
- `essentials.freezetime` -> Grants access to the `/freezetime` command.
- `essentials.home.delete` -> Grants access to the `/delhome` and `/sethome` commands.
- `essentials.home.tp` -> Grants access to the `/myhome` command.
- `essentials.kickall` -> Grants access to the `/kickall` command.
- `essentials.lastcommand` -> Grants access to the `/=` command.
- `essentials.more` -> Grants access to the `/more` command.
- `essentials.mute` -> Grants access to the improved `/mute` command.
- `essentials.pvp` -> Grants access to the `/pvpget` command.
- `essentials.ruler` -> Grants access to the `/ruler` command.
- `essentials.send` -> Grants access to the `/send` command.
- `essentials.sudo` -> Grants access to the `/sudo` command.
- `essentials.sudo.force` -> Extends the capabilities of `sudo`.
- `essentials.sudo.super` -> Allows `sudo` to be used on anyone.
- `essentials.sudo.invisible` -> Causes `sudo`'d commands to be executed invisibly.
- `essentials.timecmd` -> Grants access to the `/timecmd` command.
- `essentials.tp.back` -> Grants access to the `/back` command.
- `essentials.tp.down` -> Grants access to the `/down` command.
- `essentials.tp.left` -> Grants access to the `/left` command.
- `essentials.tp.right` -> Grants access to the `/right` command.
- `essentials.tp.up` -> Grants access to the `/up` command.

## Config

> Configuration file location：tshock/EssentialsPlus.json

```json5
{
  // List of commands that disable PvP (Player vs Player) when used.
  "Pvp禁用命令": [
    "eback"
  ],

  // The number of previous locations to store in the history for rollback or similar features.
  "回退位置历史记录": 10,

  // MySQL server host information. Required if using MySQL as the database.
  "MySql主机": "如使用Mysql这里需要配置完整信息",  // Host address, e.g., "localhost" or an IP address.

  // Name of the MySQL database to be used.
  "MySql数据库名称": "",  // Database name, e.g., "mydatabase".

  // MySQL user with access to the specified database.
  "MySql用户": "",  // Username for database access, e.g., "dbuser".

  // Password for the MySQL user.
  "MySql密码": ""  // Password for the specified user, e.g., "password123".
}
```

## 更新日志

```
1.0.4
添加西班牙语，修正部分内容
1.0.3
i18n完成，且预置es-EN
1.0.2
修复数据库错误
1.0.1 
修复重启无法获取禁言的BUG, 重命名一些方法
```

## FeedBack

- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
