# SurfaceBlock

- Authors: 羽学
- Source: [SurfaceBlock](https://github.com/1242509682/SurfaceBlock)
- This is a Tshock server plugin primarily used to:
- prevent the generation of spam that occurs when players on the server surface of the world use explosives maliciously, damaging the server map.

## Commands

| Command Syntax	 | Alias |    Permission     |                 Description                  |
|-----------------|:-----:|:-----------------:|:--------------------------------------------:|
| None            | None  |   SurfaceBlock    | Neglecting violations of surface projectiles |
| /reload         | None  | tshock.cfg.reload |        Reload the configuration file         |

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
