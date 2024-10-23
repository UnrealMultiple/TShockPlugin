# EssentialsPlus

- Authors: WhiteX等人，Average,Cjx，肝帝熙恩适配与修改,Cai更新
- Source: [github](https://github.com/QuiCM/EssentialsPlus)
- Essentials+ is a combination of things from Essentials and things from MoreAdminCommands made better. All commands run asynchronously. It does not include Sign Commands.


## Commands

- **/find** -> Includes several subcommands:
    - **-command** -> Searches for specific commands based on input and returns matching commands and their permissions.
    - **-item** -> Searches for specific items based on input and returns matching items and their IDs.
    - **-tile** -> Searches for specific tiles based on input and returns matching tiles and their IDs.
    - **-wall** -> Searches for specific walls based on input and returns matching walls and their IDs.
- **/freezetime** -> Freezes or unfreezes time.
- **/delhome** <home name> -> Deletes one of your home points.
- **/sethome** <home name> -> Sets one of your home points.
- **/myhome** <home name> -> Teleports you to one of your home points.
- **/kickall** <reason> -> Kicks all players from the server.
- **/=** -> Repeats the last command you entered (does not include other iterations of /=).
- **/more** -> Maximizes the stack of the item in your hand. Subcommand:
    - **-all** -> Maximizes all stackable items in the player's inventory.
- **/mute** -> Overrides TShock's /mute command. Includes subcommands:
    - **add** <name> <time> -> Adds a mute for the user named <name>, for a time of <time>.
    - **delete** <name> -> Removes the mute for the user named <name>.
    - **help** -> Shows command information.
- **/pvpget2** -> Toggles your PvP status.
- **/ruler** [1|2] -> Measures the distance between point 1 and point 2.
- **/send** -> Broadcasts a message with a custom color.
- **/sudo** -> Attempts to make <player> execute <command>. Includes subcommands:
    - **-force** -> Executes the command forcibly, without the <player>'s permission restrictions.
- **/timecmd** -> Executes a command after a given time interval. Includes subcommands:
    - **-repeat** -> Repeats the execution of <command> every <time>.
- **/eback** [steps] -> Takes you back to the previous location. If [steps] is provided, tries to take you back to the position [steps] before.
- **/down** [levels] -> Attempts to move your position on the map downward. If [levels] is specified, tries to move downward [levels] times.
- **/left** [levels] -> Similar to /down [levels], but moves left.
- **/right** [levels] -> Similar to /down [levels], but moves right.
- **/up** [levels] -> Similar to /down [levels], but moves upward.



## Permissions

- `essentials.find` -> Grants access to the `/find` command.
- `essentials.freezetime` -> Grants access to the `/freezetime` command.
- `essentials.home.delete` -> Grants access to the `/delhome` and `/sethome` commands.
- `essentials.home.tp` -> Grants access to the `/myhome` command.
- `essentials.kickall` -> Grants access to the `/kickall` command.
- `essentials.lastcommand` -> Grants access to the `/=` command.
- `essentials.more` -> Grants access to the `/more` command.
- `essentials.mute` -> Grants access to the improved `/mute` command.
- `essentials.pvp` -> Grants access to the `/pvp` command.
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
```json
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
## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
