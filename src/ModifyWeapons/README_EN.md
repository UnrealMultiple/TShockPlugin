# ModifyWeapons

- Authors: 羽学
- Source: Tshock QQ Group 816771079
- This is a Tshock server plugin, mainly used for：
- modifying and storing player weapon data and automatically reloading. You can use the /mw command to give players items with specified attributes.

## Update Log

```
v1.2.7
Fixed a bug where all custom weapons modified by players were being overwritten by public weapons after using the /reload command.
Corrected the "Public Weapons Updated" notification to only broadcast when data synchronization has not occurred.
Added a delayed execution command after giving items (supports the 'up all give' subcommand): This avoids synchronization issues when creating RPG content and using the command: /mw g {0}, ensuring item data is synchronized promptly.
In cases where the ["Create data for admins only on join"] option is not enabled: The 'up all give' command needs to be entered only once for data synchronization.
This update ensures better synchronization of item data and improves the user experience by minimizing conflicts between player-customized weapons and public weapons.

v1.2.6
The following features were custom-made for server admin "556":
To facilitate RPG configuration writing, placeholders {0} for the player's name have been added to the subcommands: up, del, give, all.
Added an option to only give items with specified names when using the give and all subcommands. When enabled, it will only provide the named items; otherwise, it gives all modified items stored in the player's database.
Introduced public weapons, which are item data written into new players' accounts based on the configuration file. When players pick up these items during exploration, they can manually reload to get the new data. Public weapon data has higher priority than item data given by commands. Data updates occur only when players select the public weapons.
Modified the /reload reloading logic (effective only when public weapons are enabled): announces differences between player data and public weapon configurations. If a weapon is not found in the configuration after reloading, it will be removed from all players' data.
If the "Public Weapon Broadcast Title" configuration contains any content, it will announce additional information such as xxxxx Public weapons have been updated.
Added new command /mw pw (empty input provides format instructions):
Format 1: /mw pw ItemName da 100 — modifies or adds specified public weapons.
Format 2: /mw pw on & off enables/disables public weapons.
Format 3: /mw pw del ItemName removes the specified weapon from the configuration and all player data.

v1.2.5
1. Added automatic cleanup functionality and its corresponding toggle command: `/mw clear`
   - This command controls the configuration option `Clear Modified Weapons`.
   - It also includes a `Clear Exemption List` configuration item for filtering.
   - Players with `mw.admin` permissions are immune to this cleanup.
   - Modified weapons will be cleaned up when players intentionally drop items or place them in chests (items dropped during reload will not be cleared).

2. Added logic for automatically recognizing economy commands to trigger a reload (effective for admins as well):
   - When a player sends a string starting with `/` or `.`, followed by keywords from the `Trigger Reload Command Detection List`,
   - It checks if the player's inventory contains modified weapons, and if so, triggers a reload. This prevents players from maliciously refreshing item values through purchases.

v1.2.4
Optimized broadcast and message sending phrases, added instructions for modifying parameters.
Removed the automatic update damage detection logic (had bugs).
Fixed a bug where the /mw all command would repeatedly give items to players who already had them.
Added functionality for /mw read to display the names and quantities of items reloaded onto the player.

v1.2.3
Added automatic update judgment: checks if the player is currently using an item.
Added automatic update judgment: checks if the item has modified ammunition properties.
Added item re-read judgment: will look for corresponding modified items in the player's inventory before updating.
Added item color property: format is hexadecimal without the # symbol, e.g., `/mw s hc CDEEEB`.
Offline modification logic added for `/mw all`, `g`, and `up`.
Whether the player is online or offline: data is automatically created if none exists, otherwise updated; if online, it re-reads and directly gives the item (except for `/mw up`).
Note: Prefixes are only updated when the player is not holding the modified item,
If the player is online and holding the modified item, it only writes the prefix on the held item.
To enhance the efficiency and clarity for administrators, we have optimized the /mw reads command. Now, this command offers two distinct functionalities based on the parameter provided:
reads 1: When an administrator uses /mw reads 1, it will immediately reload the modified weapon data for all online players. This action does not affect the players' auto-reload settings.
reads 2: Using /mw reads 2 will toggle the join auto-reload state for all players. When enabled, the system automatically checks and reloads modified weapon data whenever a player joins the server.

v1.2.2
Refactored and optimized code, improved feedback messages.
Supports modification of prefix and item quantity.
Added auto-reload function (beta): `/mw auto`.
When enabled, it disables the player reload count mechanism (uses the player's own reload cooldown time).
Triggers only when holding a modified item and damage exceeds the modified value + misjudgment value, or the item prefix does not match.
Moved data structure from Config to tshock.sqlite storage.
`mw.admin` permission holders can ignore reload counts.
Fixed a bug where `mw.admin` permissions could not use certain management commands:
It was written as `cmd.admin` but forgot to change it back.

v1.2.1
- Added the /mw all command to give a specified item to all online players and establish data.
- When a player receives an item from an admin, they will be prompted with the exact modified values and a reminder to manually refresh.
- Fixed the broadcast logic for the /mw read command.
- Detailed explanation of the up sub-command:
  The commands /mw s or g or all will first reset other values before modifying the specified values.
  The command /mw up <player name> <item name> <property> <value> (e.g., /mw up John Sword ua 20) will retain previous values and only modify the specified value.
- Item property parameters (can also use Chinese names):
  Damage: d, da
  Scale: c, sc
  Knockback: k, kb
  Use Time: t, ut
  Use Animation: a, ua
  Shoot: h, sh
  Shoot Speed: s, ss
  Ammo: m, am
  Use Ammo: aa, uaa

v1.2.0
- Refactored code to support customization of multiple weapon items.
- Removed database logic, switched to using configuration files for data storage.
- When the "Admin-only data creation on join" configuration option is enabled:
- Player data will only be created after an item is given to that player using the /mw g command.
- Added more commands:
  When inputting /mw s or g or up without values, it will prompt the corresponding tutorial.
  (Successful input will ignore the player's own reload count and immediately update the in-game item status.)
- Using /mw list supports paging through all modified weapon values (1 per page).
- The /mw up command will only update specific parameters of the designated player's item, provided the item has been modified before.

v1.1.0
- Added the ability to synchronize data within the configuration using /reload to the database.
- If the data table in the configuration is accidentally deleted, restarting the server will write it back from the database to the configuration file.

- Added logic for combining attribute modifications:
  /mw set d 100 ss 20
  /mw g PlayerName ItemName d 100 ss 20

- Added a cooldown mechanism and reload count (immune permission: mw.cd).
  By default, when a player logs in for the first time, they have 2 reload counts, and the login reload is turned off by default.
  When the cooldown reaches the preset seconds in the Config, 1 reload count is added automatically.
  According to the reload count, you can directly use /mw read to manually reload the weapon values.
  If the cooldown is not reached or there are no reload counts, take a screenshot of your /mw menu page and send it to the admin.
  The admin can also help the player manually reload in the background:
  /mw g PlayerName WeaponName Value...
  (This command will also actively enable the player's login reload function)

- Fixed the bug where /mw del would not delete the configuration file.
- Fixed the bug where /mw open would not synchronize the configuration file.
- Fixed the bug where reforging and then reloading the weapon would swallow the weapon prefix.
  /mw g will check if the player is holding a custom weapon to get the accurate prefix.
  (It is recommended that the player selects the custom weapon before the admin uses /mw g)


v1.0.0
- Yuxue's version of custom weapons with a database.
- Refer to the bottom line of the /mw command menu for the value status to modify.
- Modifying your held item: /mw s 200 1 4 20 12 938 10
- Modifying and giving a specific player an item: /mw g Yuxue Copper Shortsword 200 1 4 20 12 938 10
- About projectile weapons:
  Sometimes, reducing speed actually makes the projectile frequency denser.
```

## Commands

| Syntax                             | Alias  |       Permission       |                   Description                   |
| -------------------------------- | :---: | :--------------: | :--------------------------------------: |
| /mw  | None |   mw.use    |    Command    |
| /mw hand | /mw h |   mw.use    |    Toggle switch for getting held item info    |
| /mw join | /mw j |   mw.use    |    Toggle switch for login reload    |
| /mw list | /mw l |   mw.use    |    List all modified items    |
| /mw read | None |   mw.use    |    Manually reload all modified items    |
| /mw auto | /mw at |   mw.admn    |    Toggle the automatic reload feature    |
| /mw clear | /mw cr |   mw.admin    |    Toggle the automatic cleanup feature    |
| /mw open PlayerName | /mw op |   mw.admin    |    Switch another player's login reload state    |
| /mw add PlayerName Count | None |   mw.admin    |    Add reload counts    |
| /mw del PlayerName | None |   mw.admin    |    Delete specified player's data    |
| /mw up | None |   mw.admin    |    Modify specific attributes of a player's existing "modified item"    |
| /mw set | /mw s |   mw.admin    |    Modify held item attributes    |
| /mw give | /mw g |   mw.admin    |    Give a player a modified item and create data    |
| /mw all | None |   mw.admin    |    Give a modified item to all players and create data    |
| /mw pw | /mw p |   mw.admin    |    Reload for all or enable join reload    |
| /mw reads | /mw rds |   mw.admin    |    Help everyone's reload "modified item"    |
| /mw reset | /mw rs |   mw.admin    |    Reset all player data    |
| /reload  | None |   tshock.cfg.reload    |    Reload configuration file    |
| None  | None |   mw.cd    |    Ignore cooldown and count for reloading weapons    |

## Configuration
> Configuration file location：tshock/修改武器.json
```json
{
  "Plugin Enabled": true,
  "Initial Reload Attempts": 2,
  "Give Only Specified Items by Name": true,
  "Enable Delayed Execution Command After Giving Items": true,
  "Delayed Command Milliseconds": 500.0,
  "Delayed Command List": [
    "/mw read"
  ],
  "Auto Reload": 1,
  "Trigger Reload Command Check List": [
    "deal",
    "shop",
    "fishshop",
    "fs"
  ],
  "Clean Modified Weapons (Dropped or Placed in Chests Will Disappear)": true,
  "Exempt from Cleaning List": [
    1
  ],
  "Create Data for Admins Only on Join": false,
  "Increase Reload Attempts Cooldown Seconds": 1800.0,
  "Enable Public Weapons": true,
  "Data Sync Interval Seconds": 15,
  "Public Weapons Broadcast Title": "YuXue Kaifang Server ",
  "Public Weapons List": [
    {
      "Name": "Firearm",
      "ID": 96,
      "Quantity": 1,
      "Prefix": 82,
      "Damage": 30,
      "Size": 1.0,
      "Knockback": 5.5,
      "Use Speed": 10,
      "Attack Speed": 15,
      "Projectile Type": 10,
      "Projectile Speed": 9.0,
      "Ammo": 0,
      "Launcher": 97,
      "Color": {
        "packedValue": 0,
        "R": 0,
        "G": 0,
        "B": 0,
        "A": 0,
        "PackedValue": 0
      }
    },
    {
      "Name": "Deadly Gun",
      "ID": 800,
      "Quantity": 1,
      "Prefix": 82,
      "Damage": 35,
      "Size": 1.0,
      "Knockback": 2.5,
      "Use Speed": 10,
      "Attack Speed": 15,
      "Projectile Type": 5,
      "Projectile Speed": 6.0,
      "Ammo": 0,
      "Launcher": 97,
      "Color": {
        "packedValue": 0,
        "R": 0,
        "G": 0,
        "B": 0,
        "A": 0,
        "PackedValue": 0
      }
    }
  ]
}
```
## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love