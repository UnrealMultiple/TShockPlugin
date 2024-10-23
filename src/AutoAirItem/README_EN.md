# AutoAirItem 

- Authors:  羽学
- Source:  无
- TShock version of the Auto Trash Can, a small plugin to help players clean up their trash.
- A plugin that is independently run by players using commands, without the need for server administrators to write configurations.
- It automatically creates the configuration structure when a player joins the server.
- A fully automated plugin that interacts with players through commands.


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


---
Notes
---

1. To give players permission, use this command in the console: `/group addperm default AutoAir.use`.

2. The "Trash Bin Toggle" and "Trash Bin Items" are both triggered by in-game player commands!

3. The "Player Data Table" is automatically updated when a player joins the server. If a player with the same name exists, their "record time" will be updated (it will also update once when they are offline).

4. The unit for "Trash Cleanup Speed" is the frame rate. The smaller the value, the "faster" the cleanup speed.

5. "Data Cleanup Cycle": If Player A logs out at 00:00 and Player B logs in at 00:10, with a "cleanup cycle of 10 hours," if Player A does not log back in by 10:10, Player A's auto-trash data will be cleared "when Player B logs off."

6. If the "Data Cleanup Cycle" is set to more than "9999999999 hours," it is equivalent to never clearing the data.

7. To "Monitor Trash Bin," use the command `/air auto` to enable auto-monitoring. Items placed in the trash bin slot will automatically be added to the trash bin table.

## Config
> Configuration file location：tshock/自动垃圾桶.json
```json
{
  "插件指令权限": "指令菜单：/air 或 /垃圾，权限名【AutoAir.use】，给玩家权限：/group addperm default AutoAir.use", // Command permissions for the plugin, allowing players to use /air or /trash commands with the AutoAir.use permission
  "使用说明": "玩家每次进出服都会更新【记录时间】，玩家A离线时间与玩家B登录时间相差超过【清理周期】所设定的时间，则自动清理该玩家A的数据", // Description of how the plugin works: player record time is updated on join/leave, and data is cleared if the time between Player A logging out and Player B logging in exceeds the set cleanup cycle
  "插件开关": true, // Toggle to enable or disable the plugin
  "清理垃圾速度": 60, // Speed of trash cleanup, lower values mean faster cleanup
  "广告开关": true, // Toggle to enable or disable advertisements
  "广告内容": "[i:3456][C/F2F2C7:插件开发] [C/BFDFEA:by] [c/00FFFF:羽学][i:3459]", // Content of the advertisement message
  "是否清理数据": true, // Whether or not to clean up player data
  "清理数据周期/小时": 24 // The data cleanup cycle in hours
}

```
## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love