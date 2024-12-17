## SurfaceBlock

- Authors: 羽学
- Source: [SurfaceBlock](https://github.com/1242509682/SurfaceBlock)
- This is a Tshock server plugin primarily used to:
- prevent the generation of spam that occurs when players on the server surface of the world use explosives maliciously, damaging the server map.

## Update Log

```
- 2.0.0
  - Refactored all code and removed command methods.
  - Added database storage logic to identify players who need to have their projectiles destroyed.
  - Implemented item drop and tile restoration methods.
  - Supports all seed types and different map sizes.
- 1.0.0.6.0
  - Prepared for i18n (internationalization).
- 1.0.6
  - Completed uninstall function.
- 1.0.5
  - Removed timers, using the OnWorldLoad method to create configuration files after loading the map, facilitating accurate calculation of the Main.worldSurface value.
- 1.0.4
  - Added a timer to create the configuration file with the calculated Main.worldSurface value after 20 seconds.
  - The configuration file supports normal maps and inverted worlds and synchronizes commands to the main switch in the config file.
- 1.0.3
  - Added a command toggle and permissions, obtaining a list of IDs with names when enabled, with incomplete names changed to "Unknown".
  - Command name: `/禁地表弹幕` (same permission name)
- 1.0.2
  - Preset more projectile types in the config, covering major means of map destruction.
- 1.0.1
  - Introduced Config configuration file allowing players to set blocked projectile IDs via Config.
```
## Commands

| Command Syntax	                             | Alias  |       Permission       |                   Description                   |
| -------------------------------- | :---: | :--------------: | :--------------------------------------: |
| None  | None  |   SurfaceBlock    |    Neglecting violations of surface projectiles   |
| /reload  | None  |   tshock.cfg.reload    |    Reload the configuration file    |

## Configuration
> Configuration file location： tshock/禁地表弹幕.json
```json
{
  "Enabled": true,
  "DestructionSeconds": 5,
  "ItemDrop": false,
  "TileRestoration": true,
  "Broadcast": true,
  "BlockedProjectiles": [
    28, 29, 37, 65, 68, 99, 108, 136, 137, 138, 139, 142, 143, 144, 146, 147, 149, 164, 339, 341, 354, 453, 516, 519, 637, 716, 718, 727, 773, 780, 781, 782, 783, 784, 785, 786, 787, 788, 789, 790, 791, 792, 796, 797, 798, 799, 800, 801, 804, 805, 806, 807, 809, 810, 863, 868, 869, 904, 905, 906, 910, 911, 949, 1013, 1014
  ]
}
```
## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
