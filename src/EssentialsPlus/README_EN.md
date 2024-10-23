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

- essentials.find -> Allows use of the /find command.
- essentials.freezetime -> Allows use of the /freezetime command.
- essentials.home.delete -> Allows use of the /delhome and /sethome commands.
- essentials.home.tp -> Allows use of the /myhome command.
- essentials.kickall -> Allows use of the /kickall command.
- essentials.lastcommand -> Allows use of the /= command.
- essentials.more -> Allows use of the /more command.
- essentials.mute -> Allows use of the /mute command.
- essentials.pvp -> Allows use of the /pvpget2 command.
- essentials.ruler -> Allows use of the /ruler command.
- essentials.send -> Allows use of the /send command.
- essentials.sudo -> Allows use of the /sudo command.
- essentials.timecmd -> Allows use of the /timecmd command.
- essentials.tp.eback -> Allows use of the /eback command.
- essentials.tp.down -> Allows use of the /down command.
- essentials.tp.left -> Allows use of the /left command.
- essentials.tp.right -> Allows use of the /right command.
- essentials.tp.up -> Allows use of the /up command.

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
