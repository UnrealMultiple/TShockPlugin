# EssentialsPlus

- Authors: WhiteX等人，Average,Cjx，肝帝熙恩适配与修改,Cai更新
- Source: [github](https://github.com/QuiCM/EssentialsPlus)
- Essentials+ is a combination of things from Essentials and things from MoreAdminCommands made better. All commands run asynchronously. It does not include Sign Commands.


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
- **/kickall** `<flag> <reason>` -> Kicks every player for `<reason>`. Valid flag: `-nosave` -> The kick doesn't save SSC inventory.
- **/=** -> Repeats your last entered command (not including other instances of /=).
- **/more** or **/stack** -> Maximizes the stack of the held item (including items with affixes). Includes subcommands:
    - **all**  -> Maximizes all items in the player's inventory (affix-free items only).
    - **fall** -> Maximizes all items in the player's inventory (including items with affixes).
- **/mute** -> Overwrites TShock's /mute. Includes subcommands:
    - **add** `<name> <time>` -> Mutes the player `<name>` for the duration `<time>`.
    - **delete** `<name>` -> Removes the mute for the player `<name>`.
    - **help** -> Displays command info.
- **/tpallow** -> Improved /tpallow command.
- **/pvpget** -> Enables or disables your PvP status.
- **/ruler** `[1|2]` -> Measures the distance between points 1 and 2.
- **/sudo** `[flag] <player> <command>` -> Makes `<player>` execute `<command>`. Valid flag: `-force` -> Forces the player to run the command, ignoring permission checks. Players with the `essentials.sudo.super` permission can use /sudo on anyone.
- **/timecmd** `[flag] <time> <command>` -> Executes `<command>` after `<time>`. Valid flag: `-repeat` -> Repeats `<command>` every `<time>`.
- **/eback** `[steps]` -> Returns you to a previous location. If `[steps]` is provided, it takes you back to your position `[steps]` steps ago.
- **/down** `[levels]` -> Moves you down on the map. If `[levels]` is specified, it attempts to move you down `[levels]` levels.
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
- `essentials.tpallow` -> Grants access to the improved `/tpallow` command.
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
## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
