# ModifyWeapons

- Authors: 羽学
- Source: Tshock QQ Group 816771079
- This is a Tshock server plugin, mainly used for：
- modifying and storing player weapon data and automatically reloading. You can use the /mw command to give players items with specified attributes.

## Commands

| Syntax                   |  Alias  |    Permission     |                            Description                            |
|--------------------------|:-------:|:-----------------:|:-----------------------------------------------------------------:|
| /mw                      |  None   |      mw.use       |                              Command                              |
| /mw hand                 |  /mw h  |      mw.use       |             Toggle switch for getting held item info              |
| /mw join                 |  /mw j  |      mw.use       |                  Toggle switch for login reload                   |
| /mw list                 |  /mw l  |      mw.use       |                      List all modified items                      |
| /mw read                 |  None   |      mw.use       |                Manually reload all modified items                 |
| /mw auto                 | /mw at  |      mw.admn      |                Toggle the automatic reload feature                |
| /mw clear                | /mw cr  |     mw.admin      |               Toggle the automatic cleanup feature                |
| /mw open PlayerName      | /mw op  |     mw.admin      |            Switch another player's login reload state             |
| /mw add PlayerName Count |  None   |     mw.admin      |                         Add reload counts                         |
| /mw del PlayerName       |  None   |     mw.admin      |                  Delete specified player's data                   |
| /mw up                   |  None   |     mw.admin      | Modify specific attributes of a player's existing "modified item" |
| /mw set                  |  /mw s  |     mw.admin      |                    Modify held item attributes                    |
| /mw give                 |  /mw g  |     mw.admin      |           Give a player a modified item and create data           |
| /mw all                  |  None   |     mw.admin      |        Give a modified item to all players and create data        |
| /mw pw                   |  /mw p  |     mw.admin      |               Reload for all or enable join reload                |
| /mw reads                | /mw rds |     mw.admin      |              Help everyone's reload "modified item"               |
| /mw reset                | /mw rs  |     mw.admin      |                       Reset all player data                       |
| /reload                  |  None   | tshock.cfg.reload |                     Reload configuration file                     |
| None                     |  None   |       mw.cd       |          Ignore cooldown and count for reloading weapons          |

## Configuration
> Configuration file location：tshock/修改武器.json
```json5
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