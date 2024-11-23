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

## Update Log

```
v1.3.2
- Attempted to fix the server crash bug that occurred when the bait quantity was 1.

v1.3.1
- Fixed the bug where in multi-hook fishing mode, individual summons would spawn additional quantities.
- Added a configuration option to disable "spawned bullet hell" from these summons.
- Introduced a random item configuration option; when enabled, players can randomly catch any item while fishing.

v1.3.0
Fixed: Preferred bait would not disappear, turning it into infinite bait or causing thread locking bugs.
Modified: Extra catches can now be hooked along with existing environmental items.
Refactored: Consumption mode code logic to optimize performance and fix bugs.
Added: Command to set the number of multi-hooks: /af duo <number>.
Added: Commands related to the extra catch table.
Changed: Made the /af buff command player-accessible to toggle personal buffs.

v1.2.0
Fixed: Fishing did not consume bait.
Fixed: Truffle Worm could not catch a Pigron.
Fixed: Thread locking issue when the bait count was 1.
Added: Configuration option for consumption mode.
Added: Configuration option for fishing buffs (triggered only when a catch is made).
Improved: Enhanced the automatic fishing command system and adjusted the content displayed based on different permissions and modes.

v1.1.0
Completed: Initial release of the Tshock version of AutoFish.
Added: Logic to exchange automatic fishing usage duration by consuming bait quantities.
Added: When Truffle Worm is in the player's inventory, it will automatically catch an Iron Pickaxe and attempt to turn off the player's automatic fishing.
Added: Players can use the /af on command to re-enable the plugin without resetting their automatic fishing duration.

v1.0.0
Attempted: Initial development of the Tshock version of AutoFish, which failed due to the inability to modify client-side player actions and the lack of data packets to handle the hook state.
Attempted: Triggering the reel-in effect by changing AI[0] to 1, but unable to obtain actual catches.

```

## Commands

| Command Syntax	                             | Alias  |       Permission       |                   Description                   |
| -------------------------------- | :---: | :--------------: | :--------------------------------------: |
| /af  |  /autofish  |   autofish    |    Command menu (check remaining automatic fishing duration)    |
| /af on  |  /autofish on  |   autofish    |    Enable automatic fishing for the player    |
| /af off  |  /autofish off  |   autofish    |    Disable automatic fishing for the player    |
| /af list  |  /autofish list  |   autofish    |    List items specified for consumption mode    |
| /af loot  |  /autofish loot  |   autofish    |    List extra catch items   |
| /af buff  |  /autofish buff  |   autofish    |    Toggle the player's fishing buff    |
| /af more  |  /autofish more  |   autofish.admin    |    Enable or disable multi-hook mode   |
| /af duo <number>  | /autofish duo |   autofish.admin    |    Set the number of hooks for multi-hook mode   |
| /af + <item name>  | /autofish + <item name> |   autofish.admin    |    Add an item to the extra catch list   |
| /af - <item name>  |  /autofish - <item name>  |   autofish.admin    |    Remove an item from the extra catch list   |
| /af mod  |  /autofish mod |   autofish.admin    |    Enable or disable consumption mode   |
| /af set <number> |  /autofish set <number> |   autofish.admin    |    Set the required quantity of items for consumption mode    |
| /af time <number>  |  /autofish time <minute> |   autofish.admin    |    Set the duration of automatic fishing in minutes    |
| /af add <item name>  |  /autofish add <item name> |   autofish.admin    |    Add a specified item for consumption mode    |
| /af del <item name>  |  /autofish del <item name> |   autofish.admin    |    Remove a specified item from consumption mode    |
| /reload  | 无 |   tshock.cfg.reload    |    Reload the configuration file    |

## Configuration
> Configuration file location： tshock/自动钓鱼.json
```json
{
  "插件开关": true,  // Global plugin switch
  "多钩钓鱼": true,  // Enable multi-hook mode for automatic fishing to increase efficiency
  "随机物品": false, // Randomly fish out any item
  "多钩上限": 5,     // Define the maximum number of hooks that can fish simultaneously
  "广告开关": true,  // Switch for the following string
  "广告内容": "\n[i:3456][C/F2F2C7:Plugin Development] [C/BFDFEA:by] [c/00FFFF:Yu Xue] | [c/7CAEDD:Shao Siming][i:3459]", // Customizable string content
  "Buff": {
    "80": 10,       // 80 is the buff ID, 10 is the duration in frames (60 frames = 1 second)
    "122": 240
  },
  "消耗模式": false, // Consume items to exchange for automatic fishing usage duration
  "消耗数量": 10,     // Item quantity requirement in consumption mode (e.g., 6 of item A + 4 of item B = 10)
  "自动时长": 24,     // Duration of automatic fishing granted to players in consumption mode, in minutes
  "消耗物品": [
    2002,           // Specified items for consumption mode
    2675,
    2676,
    3191,
    3194
  ],
  "额外渔获": [
    75,             // Extra catch items in addition to those that exist in the environment
    29,
    3093,
    4345
  ],
  "禁止衍生弹幕": [
    623, // To solve the BUG where single summon creatures spawn more numbers under multi-hook mode
    625,
    626,
    627,
    628,
    831,
    832,
    833,
    834,
    835,
    963,
    970
  ]

}
```

## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
