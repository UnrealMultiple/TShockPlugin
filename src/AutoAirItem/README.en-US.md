# AutoAirItem 

- Authors:  羽学
- Source: [AutoAirItem](https://github.com/1242509682/AutoAirItem)
- TShock version of the Auto Trash Can, a small plugin to help players clean up their trash.
- A plugin that is independently run by players using commands, without the need for server administrators to write configurations.
- It automatically creates the configuration structure when a player joins the server.
- A fully automated plugin that interacts with players through commands(requires SSC enabled).


## Commands

| Syntax             |       Alias        | Permission  |                               Description                               |
|--------------------|:------------------:|:-----------:|:-----------------------------------------------------------------------:|
| /air               |        /垃圾         | AutoAir.use |                              Command menu                               |
| /air on            |       /垃圾 on       | AutoAir.use |              Enable or disable personal trash can feature               |
| /air list          |      /垃圾 list      | AutoAir.use |                    List items in personal trash can                     |
| /air clear         |     /垃圾 clear      | AutoAir.use |                      Clear personal trash can list                      |
| /air mess          |      /垃圾 mess      | AutoAir.use |                   Enable or disable cleanup messages                    |
| /air ck Nuber        |     /垃圾 ck 数量     |    AutoAir.use    |    Filter out players with this amount of item    |
| /air del id | /垃圾 del 物品名 | AutoAir.use |            Retrieve items from the trash can                    |
| /air reset         |      None       |   AutoAir.admin   |   Empty the player data table (for resetting the server)   |
| /reload           |         None         | tshock.cfg.reload |       Reload configuration file        |

## Config
> Configuration file location：tshock/自动垃圾桶.json
```json5
{
  "Plugin Command Permission": "Command menu: /air or /trash, permission name [AutoAir.use], give players permission: /group addperm default AutoAir.use",
  "Plugin Switch": true,
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