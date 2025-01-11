# BuildMaster Bean Paste Game·Building Master
- author: [Bean_Paste]https://gitee.com/Crafty/projects,羽学  
- source: [bbstr]https://www.bbstr.net/r/117/  
- This is a brand new mini-game, developed based on the MiniGamesAPI framework. Its gameplay is similar to mini-games in MC (Build Battle Competition).
- Players will need to complete the corresponding theme building within a certain period of time. After the time is up, each player will be scored.  
- The scores will be ranked after the game. Come use your imagination and start building!

> [!NOTE]  
> You need to install the pre-plugin: MiniGamesAPI



## Instruction

| Command                 |    Permissions    |      Description      |
|--------------------|:--------:|:------------:|
| /bm list           | bm.user  |    View room list    |
| /bm join [RoomID]      | bm.user  |     Join room     |
| /bm leave          | bm.user  |     Leave the room     |
| /bm ready          | bm.user  |    Ready/not ready    |
| /bm vote [theme]        | bm.user  |     Voting theme     |
| /bma list          | bm.admin |    List all rooms    |
| /bma create [Room Name]    | bm.admin |     Create room     |
| /bma remove [RoomID]   | bm.admin |    Remove specified room    |
| /bma start [RoomID]    | bm.admin |    Open designated room    |
| /bma stop [RoomID]     | bm.admin |    Close designated room    |
| /bma smp [RoomID] [number of player]   | bm.admin |  Set the maximum number of players in the room   |
| /bma sdp [RoomID] [number of player]   | bm.admin |  Set the minimum number of players in the room   |
| /bma swt [RoomID] [time]   | bm.admin | Set waiting time (unit: seconds) |
| /bma sgt [RoomID] [time]   | bm.admin | Set game time (unit: seconds) |
| /bma sst [RoomID] [time]   | bm.admin | Set the scoring time (unit: seconds) |
| /bma sp 1/2        | bm.admin |    Select point 1/2    |
| /bma sr [RoomID] [theme]    | bm.admin |  Set up the game area of ​​the room   |
| /bma addt [RoomID] [theme name] | bm.admin |     Add theme     |
| /bma sh [RoomID] [Height]    | bm.admin |    Set small area height    |
| /bma sw [RoomID] [Width]    | bm.admin |    Set small area width    |
| /bma sg [RoomID] [Interval]    | bm.admin |   Set small area intervals    |
| /bma dp [player name]        | bm.admin | Set the player's base build backpack  |
| /bma ep            | bm.admin |    Set up a scoring package    |
| /bma reload        | bm.admin | Reload configuration file non-room file  |

## Configuration
> Configuration file location: tshock/BuildMaster

> Config.json
```json    
{
  "UnlockAll": true,
  "Range": {},
  "BanItem": []
}
```
> default.json
```json5
{
  "Name": "基础套",
  "ID": 2,
  "UnlockedBiomeTorches": 0,
  "HappyFunTorchTime": 0,
  "UsingBiomeTorches": 0,
  "QuestsCompleted": 0,
  "HideVisuals": [
    false,
    false,
    false,
    false,
    false,
    false,
    false,
    false,
    false,
    false
  ],
  "EyeColor": {
    "packedValue": 4283128425,
    "R": 105,
    "G": 90,
    "B": 75,
    "A": 255,
    "PackedValue": 4283128425
  },
  "SkinColor": {
    "packedValue": 4284120575,
    "R": 255,
    "G": 125,
    "B": 90,
    "A": 255,
    "PackedValue": 4284120575
  },
  "ShoeColor": {
    "packedValue": 4282149280,
    "R": 160,
    "G": 105,
    "B": 60,
    "A": 255,
    "PackedValue": 4282149280
  },
  "UnderShirtColor": {
    "packedValue": 4292326560,
    "R": 160,
    "G": 180,
    "B": 215,
    "A": 255,
    "PackedValue": 4292326560
  },
  "ShirtColor": {
    "packedValue": 4287407535,
    "R": 175,
    "G": 165,
    "B": 140,
    "A": 255,
    "PackedValue": 4287407535
  },
  "HairColor": {
    "packedValue": 4287407535,
    "R": 175,
    "G": 165,
    "B": 140,
    "A": 255,
    "PackedValue": 4287407535
  },
  "PantsColor": {
    "packedValue": 4287407535,
    "R": 175,
    "G": 165,
    "B": 140,
    "A": 255,
    "PackedValue": 4287407535
  },
  "Hair": 0,
  "SkinVariant": 0,
  "ExtraSlots": 0,
  "SpawnY": -1,
  "SpawnX": -1,
  "Exists": true,
  "MaxMana": 20,
  "Mana": 20,
  "MaxHP": 100,
  "HP": 100,
  "HairDye": 0,
  "Items": []
}
```
> eva.json
```json
{
  "Name": "评分套",
  "ID": 3,
  "UnlockedBiomeTorches": 0,
  "HappyFunTorchTime": 0,
  "UsingBiomeTorches": 0,
  "QuestsCompleted": 0,
  "HideVisuals": [
    false,
    false,
    false,
    false,
    false,
    false,
    false,
    false,
    false,
    false
  ],
  "EyeColor": {
    "packedValue": 4283128425,
    "R": 105,
    "G": 90,
    "B": 75,
    "A": 255,
    "PackedValue": 4283128425
  },
  "SkinColor": {
    "packedValue": 4284120575,
    "R": 255,
    "G": 125,
    "B": 90,
    "A": 255,
    "PackedValue": 4284120575
  },
  "ShoeColor": {
    "packedValue": 4282149280,
    "R": 160,
    "G": 105,
    "B": 60,
    "A": 255,
    "PackedValue": 4282149280
  },
  "UnderShirtColor": {
    "packedValue": 4292326560,
    "R": 160,
    "G": 180,
    "B": 215,
    "A": 255,
    "PackedValue": 4292326560
  },
  "ShirtColor": {
    "packedValue": 4287407535,
    "R": 175,
    "G": 165,
    "B": 140,
    "A": 255,
    "PackedValue": 4287407535
  },
  "HairColor": {
    "packedValue": 4287407535,
    "R": 175,
    "G": 165,
    "B": 140,
    "A": 255,
    "PackedValue": 4287407535
  },
  "PantsColor": {
    "packedValue": 4287407535,
    "R": 175,
    "G": 165,
    "B": 140,
    "A": 255,
    "PackedValue": 4287407535
  },
  "Hair": 0,
  "SkinVariant": 0,
  "ExtraSlots": 0,
  "SpawnY": -1,
  "SpawnX": -1,
  "Exists": true,
  "MaxMana": 20,
  "Mana": 20,
  "MaxHP": 100,
  "HP": 100,
  "HairDye": 0,
  "Items": []
}
```
> rooms.json
```json5
[]
```

## Change log

```
1.0.5
Fix incorrect GetString
1.0.3
i18n reservation (there are so many, I don’t want to write about them)
1.0.2
Improve the uninstall function
1.0.1
Complete and correct uninstall function
1.0.0
Remember to turn on SSC, this plug-in runs in the travel mode server (because this plug-in uses the creative mode of travel)
After setting up the basic building set (it doesn’t need to be too complicated, because there is a creative mode, you can just go and get things from there), and then you can set up the scoring set (the default is built into the plug-in, no redundant configuration is required, you only need to enter a command)
The player's separate building area must be calculated carefully, and the total should not exceed the width of the total area (the player's building area is created horizontally by default), otherwise a separate building area cannot be generated.
```


## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
