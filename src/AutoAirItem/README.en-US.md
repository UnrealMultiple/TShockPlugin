# AutoAirItem 

- Authors:  羽学
- Source: [AutoAirItem](https://github.com/1242509682/AutoAirItem)
- TShock version of the Auto Trash Can, a small plugin to help players clean up their trash.
- A plugin that is independently run by players using commands, without the need for server administrators to write configurations.
- It automatically creates the configuration structure when a player joins the server.
- A fully automated plugin that interacts with players through commands.


## Commands

| Syntax             |       Alias        | Permission  |                               Description                               |
|--------------------|:------------------:|:-----------:|:-----------------------------------------------------------------------:|
| /air               |        /垃圾         | AutoAir.use |                              Command menu                               |
| /air on            |       /垃圾 on       | AutoAir.use |              Enable or disable personal trash can feature               |
| /air list          |      /垃圾 list      | AutoAir.use |                    List items in personal trash can                     |
| /air clear         |     /垃圾 clear      | AutoAir.use |                      Clear personal trash can list                      |
| /air yes           |      /垃圾 yes       | AutoAir.use |                Add held item to personal trash can list                 |
| /air auto          |      /垃圾 auto      | AutoAir.use | Automatically add items to trash can when placed in the designated slot |
| /air mess          |      /垃圾 mess      | AutoAir.use |                   Enable or disable cleanup messages                    |
| /air add or del id | /垃圾 add or del 物品名 | AutoAir.use |            Add or remove items from personal trash can list             |

## Config
> Configuration file location：tshock/自动垃圾桶.json
```json5
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