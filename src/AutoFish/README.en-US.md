# AutoFish

- Authors: 羽学 & 少司命
- Source: 无
- AutoFish is an automatic fishing plugin designed for Tshock servers. 
- It allows players to automate their fishing activities, enhancing the gameplay experience by adding extra items to the fishing pool and providing various customization options.
- Extra Items: In addition to items that already exist in the environment, you can configure extra items to be caught through fishing.
- Multi-Hook Fishing: Adjust the number of hooks per cast via the configuration file to increase fishing efficiency.
- Consumption Mode: Consume specific items to exchange for plugin usage duration.
- Fishing Buffs: Players can enable or disable fishing buffs that trigger when a catch is made.
- Blood Moon Restriction: No NPCs can be fished during a Blood Moon. A relevant prompt will appear when using the /af command during a Blood Moon.
- Complete Command System: Some features will not display related commands if they are not enabled or the user does not have permission.

## Commands

| Command Syntax	     |           Alias           |    Permission     |                        Description                        |
|---------------------|:-------------------------:|:-----------------:|:---------------------------------------------------------:|
| /af                 |         /autofish         |     autofish      | Command menu (check remaining automatic fishing duration) |
| /af on              |       /autofish on        |     autofish      |          Enable automatic fishing for the player          |
| /af off             |       /autofish off       |     autofish      |         Disable automatic fishing for the player          |
| /af list            |      /autofish list       |     autofish      |         List items specified for consumption mode         |
| /af loot            |      /autofish loot       |     autofish      |                  List extra catch items                   |
| /af buff            |      /autofish buff       |     autofish      |             Toggle the player's fishing buff              |
| /af more            |      /autofish more       |  autofish.admin   |             Enable or disable multi-hook mode             |
| /af duo <number>    |       /autofish duo       |  autofish.admin   |        Set the number of hooks for multi-hook mode        |
| /af + <item name>   |  /autofish + <item name>  |  autofish.admin   |            Add an item to the extra catch list            |
| /af - <item name>   |  /autofish - <item name>  |  autofish.admin   |         Remove an item from the extra catch list          |
| /af mod             |       /autofish mod       |  autofish.admin   |            Enable or disable consumption mode             |
| /af set <number>    |  /autofish set <number>   |  autofish.admin   |  Set the required quantity of items for consumption mode  |
| /af time <number>   |  /autofish time <minute>  |  autofish.admin   |     Set the duration of automatic fishing in minutes      |
| /af add <item name> | /autofish add <item name> |  autofish.admin   |         Add a specified item for consumption mode         |
| /af del <item name> | /autofish del <item name> |  autofish.admin   |       Remove a specified item from consumption mode       |
| /reload             |             无             | tshock.cfg.reload |               Reload the configuration file               |

## Configuration
> Configuration file location： tshock/AutoFish.en-US.json
```json5
{
  "AdditionalCatches": [
    29,
    3093,
    4345
  ], // Extra catch items in addition to those that exist in the environment
  "Enable": true, // Global plugin switch
  "MultipleFishFloats": true, // Enable multi-hook mode for automatic fishing to increase efficiency
  "RandCatches": false, // Randomly fish out any item
  "MultipleFishFloatsLimit": 5, // Define the maximum number of hooks that can fish simultaneously
  "SetBuffs": {
    "80": 10, // 80 is the buff ID, 10 is the duration in frames (60 frames = 1 second)
    "122": 240
  },
  "ConsumeBait": false, // Consume items to exchange for automatic fishing usage duration
  "ConsumeBaitNum": 10, // Item quantity requirement in consumption mode (e.g., 6 of item A + 4 of item B = 10)
  "Time": 24, // Duration of automatic fishing granted to players in consumption mode, in minutes
  "ConsumeItem": [
    2002, // Specified items for consumption mode
    2675,
    2676,
    3191,
    3194
  ]
}
```

## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
