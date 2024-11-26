# AutoAirItem 

- Authors:  羽学
- Source: [AutoAirItem](https://github.com/1242509682/AutoAirItem)
- TShock version of the Auto Trash Can, a small plugin to help players clean up their trash.
- A plugin that is independently run by players using commands, without the need for server administrators to write configurations.
- It automatically creates the configuration structure when a player joins the server.
- A fully automated plugin that interacts with players through commands.

## Update Log
```
v1.2.3
- Move database tables to tshock.sqlite

v1.2.2
- Cleaned up unused code.
- Fixed a bug where items already in the trash bin would not be automatically removed upon reinsertion.
- Added feedback broadcast for item quantity when removing items.

v1.2.1
- Added an [Exclusion List] configuration option.
- Fixed a bug where the first removed item did not return the correct quantity.
- Fixed a bug where money was also considered as trash and removed.
- Fixed a bug where /air ck could not check the number of recently removed trash.
- Added logic to remove and record the quantity of items placed in the trash bin for the first time.
- Fixed a bug where PE received two prompts when using /air del to return items.

v1.2.0
- Added database storage logic for the automatic trash bin to prevent data loss after server restarts.
- Note: When resetting the server, use the command to clear the data: /airreset

v1.1.7
- Fixed the bug where using `/air del` would return double the items on Mobile Games

v1.1.6
- Added the `/air ck <amount>` command to filter out players with more than the specified amount of items
- Fixed the bug where returned items exceeded the stack limit

v1.1.5
- Changed the removal logic to trigger on player movement (performance optimization)
- Removed the `/air sd` command to modify cleanup speed
- Added the item return logic to the `/air del` command

v1.1.4
- Removed the automatic player data cleanup logic and related configuration options
- Changed the `/air reset` command to:
  - A separate `/airreset` command for easier loop reset of the server

v1.1.3
- Improved the command menu
- Added the `/air sd` command to modify garbage cleanup speed
- Added the `/air reset` command to clear all player data (used to reset the server)
- The `/air mess` command can control the hiding of item addition notifications to the trash bin

v1.1.2
- Fixed the null reference when a player leaves the server
- Moved the data cleanup function to the join server event

v1.1.1
- Fixed English translation (open-source version)

v1.1.0
- Moved the data table content from Config to the internal MyData class to avoid frequent writes to the configuration file, which could cause issues with a large number of players
- Automatically enabled the trash bin feature on first login, with a command prompt when items are placed in the trash bin
- Added online status checks to prevent innocent data cleanup due to 24-hour play sessions
- Added offline announcements when cleaning specific player data

v1.0.2
- Added the `/air yes` command to add held items to the trash bin
- Items will only be cleaned if not selected; even if the wrong input is entered, `/air del <item name>` can be used to remove them
- Added the `/air mess` command to enable/disable cleanup messages
- Added a listener for the trash bin slot to automatically add items to the `Trash Bin Table` when they are placed in the trash bin

v1.0.1
- Removed the plugin activation status notification on every `/air add` or `/air del`
- Added the "Data Cleanup Cycle" logic:
  - The "Record Time" is updated every time a player logs in or out
  - If the offline time of Player A and the login time of Player B differ by more than the set "Cleanup Cycle" time, Player A's data will be automatically cleaned
  - Setting the "Data Cleanup Cycle" to over 9999999999 hours effectively means never cleaning data

v1.0.0
- Implemented an automatic trash bin for Tshock based on the idea from the Mod "Better Experience"
- The garbage cleanup speed unit is frames per second; the smaller the value, the faster the cleanup
- Use the `/air on` command to enable the plugin
- Use `/air add <item name>` or `<item id>` or `Alt + Left Click` to select an item, which will automatically write the item name to the configuration file
- Automatically creates a "Player Data Table" based on player login events
- The "Trash Bin Items" will trigger the cleanup logic if they exist and the "Trash Bin Switch" is enabled
- Equipped with "Login Time" for server owners to reference whether they need to manually remove a player's data
```

## Commands

| Syntax                             | Alias  |       Permission       |                   Description                   |
| ---------------------------------- | :----: | :-------------------: | :---------------------------------------------: |
| /air                               | /垃圾  |     AutoAir.use       |                Command menu                 |
| /air on                            | /垃圾 on |     AutoAir.use       |         Enable or disable personal trash can feature        |
| /air list                          | /垃圾 list |     AutoAir.use       |           List items in personal trash can            |
| /air clear                         | /垃圾 clear |     AutoAir.use       |          Clear personal trash can list          |
| /air yes                           | /垃圾 yes |     AutoAir.use       |          Add held item to personal trash can list          |
| /air auto                          | /垃圾 auto |     AutoAir.use       |         Automatically add items to trash can when placed in the designated slot         |
| /air mess                          | /垃圾 mess |     AutoAir.use       |         Enable or disable cleanup messages         |
| /air add or del id                 | /垃圾 add or del 物品名 |     AutoAir.use       |          Add or remove items from personal trash can list          |

## Config
> Configuration file location：tshock/自动垃圾桶.json
```json
{
  "Plugin Command Permission": "Command menu: /air or /trash, permission name [AutoAir.use], give players permission: /group addperm default AutoAir.use",
  "Plugin Switch": true,
  "Do Not Delete Data on Server Restart": true,
  "Exclusion ItemID": [
    71,
    72,
    73,
    74
  ]
}
```
## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love