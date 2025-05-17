# StatusTextManager
- Author: LaoSparrow
- Source: https://github.com/CNS-Team/StatusTxtMgr
- Manage template text in the upper right corner of the PC side


## Instruction

| Command          | Permissions |    Description    |
|-------------|:--:|:--------:|
| /statustext | None  | Switch template text display |
| /st         | None  | Switch template text display |

## DynamicText placeholder list

| PlaceHolder                        | PlaceHolder Content              |
|---------------------------|-------------------|
| {PlayerName}              | Player name              |
| {PlayerGroupName}         | Player group           |
| {PlayerLife}              | Player health             |
| {PlayerMana}              | Player mana             |
| {PlayerLifeMax}           | Player's maximum health           |
| {PlayerManaMax}           | Player's maximum mana           |
| {PlayerLuck}              | Player luck             |
| {PlayerCoordinateX}       | Player X coordinate             |
| {PlayerCoordinateY}       | Player Y coordinates             |
| {PlayerCurrentRegion}     | Current TShock region of the player   |
| {IsPlayerAlive}           | Alive Player            |
| {RespawnTimer}            | Cooldown player spawn (Unknown reasons do not take effect) |
| {OnlinePlayersCount}      | Number of online players            |
| {OnlinePlayersList}       | Online player list            |
| {AnglerQuestFishName}     | Angler quest fish name           |
| {AnglerQuestFishID}       | Angler quest fish ID           |
| {AnglerQuestFishingBiome} | Angler quest biome location          |
| {AnglerQuestCompleted}    | Angler quest completed         |
| {CurrentTime}             | In-game time             |
| {RealWorldTime}           | Real world time            |
| {WorldName}               | World name              |
| {CurrentBiomes}           | Curent biome          |

## Configuration
> Configuration file location: tshock/StatusTextManager.json
```json5
{
  "Settings": {
    "LogLevel": "INFO",
    "StatusTextSettings": [
      {
        "TypeName": "StaticText",
        "Text": "Helloooooooooooooooooooooooooo\n"
      },
      {
        "TypeName": "HandlerInfoOverride", //Plugin template text configuration override
        "PluginName": "STMTest2", //AssemblyName of plug-in, usually plug-in dll removes the extension
        "Enabled": true, //Enable plugin template text
        "OverrideInterval": true, //Overwrite the text update interval of the plug-in template
        "UpdateInterval": 1200 //Update interval, in frame units, 60=1s, for example, here 1200=20s 
      },
      {
        "TypeName": "DynamicText", //Dynamic text types
        "Text": "\nWorld Name: {WorldName}, {Player Name: {PlayerName}}, Field: {{PlayerName}}\n", //Dynamic text content, the interpolation wrapped in curly braces {} will be dynamically replaced with the corresponding interpolation content, and the interpolation is skipped with double curly braces.
        // If you want to do Player Name: {Sparrow}, just use two StaticTexts to wrap it. .
        "UpdateInterval": 60 //Update interval, similar
      },
      {
        "TypeName": "HandlerInfoOverride", //Much the same
        "PluginName": "STMTest1",
        "Enabled": true,
        "OverrideInterval": true,
        "UpdateInterval": 600
      }
    ]
  }
}
```
Final display effect
```
Helloooooooooooooooooooooooooo
Sparrow Hello from STMTest2 9
World Name: 1449World, {Player Name: Sparrow}, Field: {PlayerName}
Sparrow Hello from STMTest1 16
```
Sample configuration file
```json5
{
  "Settings": {
    "LogLevel": "INFO",
    "StatusTextSettings": [
      {
        "TypeName": "DynamicText",
        "Text": "\n\n\n\n\n\n\n\n\n\n--[提[i:29]瓦[i:29]特]--\n[i:1503]Player name: {PlayerName}\n[i:346]Current group: {PlayerGroupName}\n[i:893]World name: {WorldName}\n[i:855]Lucky value: {PlayerLuck}\n[i:889]Game time: {CurrentTime}\n[i:{AnglerQuestFishID}]Angler quest fish: {AnglerQuestFishName}\n[i:3036]Angler quest fishing spot: {AnglerQuestFishingBiome}[i:1307]\n[i:267]Online players: {OnlinePlayersList}\n[i:3122]Current biome: {CurrentBiomes}",
        "UpdateInterval": 60
      }
    ]
  }
}

```

## Adaptation sample code

```csharp
using Terraria;
using TerrariaApi.Server;

namespace STMTest1;

[ApiVersion(2, 1)]
// ReSharper disable once UnusedType.Global
public class Plugin : TerrariaPlugin
{
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public Plugin(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        StatusTextManager.Hooks.OnStatusTextUpdate.Register(this.OnStatusTextUpdate, 60);
    }

    private int _counter;
    private void OnStatusTextUpdate(StatusTextManager.StatusTextUpdateEventArgs args)
    {
        args.StatusTextBuilder.AppendFormat($"{args.TSPlayer.Name} Hello from STMTest1 {this._counter++}");
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            StatusTextManager.Hooks.OnStatusTextUpdate.Deregister(this.OnStatusTextUpdate);
        }
        base.Dispose(disposing);
    }
}
```

## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
