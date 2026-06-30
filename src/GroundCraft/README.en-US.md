# GroundCraft

- Author: 愚蠢
- Source: Generic TShock plugin packaged in the `UnrealMultiple/TShockPlugin` repository style
- Keywords: GroundCraft, DropCraft, Ground Item Craft, dropped-item crafting
- Description: craft items by dropping ingredients together. Recipes and conditions are loaded from JSON. Vanilla Terraria recipes are not scanned or triggered.

## Commands

| Command | Alias | Permission | Description |
|---|---|---|---|
| `/groundcraft` | `/gc` | `tshock.canchat` | Show plugin status |
| `/gcrecipes [page/search]` | `/gcr` | `tshock.canchat` | List active recipes |
| `/gcenv` | none | `tshock.canchat` | Show the current layer, biome and liquid tags |
| `/gcreload` | none | `groundcraft.admin` | Hot-reload config and recipe JSON files |
| `/gcaudit` | none | `groundcraft.admin` | Show recipe audit and runtime stats |

## Configuration

Files are generated under `tshock/GroundCraft/`.

- `config.json`: scan interval, radius, notifications, client ghost cleanup, safety rules and permissions
- `recipes.json`: custom ground crafting recipes

Use `/gcreload` to reload both JSON files without restarting the server.

`requireExactIngredientTypes` is enabled by default. Extra dropped item types in the same cluster will prevent a similar recipe from firing accidentally. `animateConsumedItems` is also enabled by default: consumed drops are locked as unpickable visual items, spiral upward, and only then turn into the output. Zenith recipes use a taller and wider dedicated animation.

## Recipe Conditions

Supported condition groups:

- `layers`: `Sky`, `Surface`, `Underground`, `Cavern`, `Underworld`
- `biomes`: `Forest`, `Snow`, `Desert`, `Jungle`, `Corruption`, `Crimson`, `Hallow`, `Mushroom`, `Graveyard`, `Ocean`, `Shimmer`, `Dungeon`, `Hive`, `Temple`
- `liquids` / `anyLiquids`: `Water`, `Lava`, `Honey`, `Shimmer`
- `bossProgress.hardmode`: `true` or `false`
- `bossProgress.allDowned` / `anyDowned` / `noneDowned`: `KingSlime`, `EyeOfCthulhu`, `EaterOfWorldsOrBrain`, `QueenBee`, `Deerclops`, `Skeletron`, `WallOfFlesh`, `QueenSlime`, `MechBossAny`, `Destroyer`, `Twins`, `SkeletronPrime`, `MechBossAll`, `Plantera`, `Golem`, `DukeFishron`, `EmpressOfLight`, `LunaticCultist`, `MoonLord`

## Safety

- At least two ingredient types are required by default.
- Coin recipes are rejected by default.
- Input-output self loops are rejected by default.
- Ambiguous recipes with the same inputs, stations and conditions but different outputs are rejected.
- Exact ingredient type matching is enabled by default to avoid accidental similar-recipe crafts.
- Consumed world items are cleared and synchronized to avoid stale client-side ghost drops.

## Changelog

### v1.1.0

- Added a spiral fusion animation. Consumed drops are locked while animating and the output is spawned at completion.
- Added a taller, wider dedicated Zenith animation around a Mythril/Orichalcum Anvil.
- Added default recipe examples for Water Candle, Life Crystal, Wormhole Potion, Bloody Tear and Zenith.
- Enabled exact ingredient type matching by default to reduce similar-recipe mistakes.
- Increased the default batch limit to 25 crafts per cluster per scan.

### v1.0.0

- Initial version: JSON recipes, JSON conditions, layer/biome/liquid checks, boss progress checks, hot reload, recipe audit and consumed drop cleanup.
