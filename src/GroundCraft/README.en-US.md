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
- Consumed world items are cleared and synchronized to avoid stale client-side ghost drops.

## Changelog

### v1.0.0

- Initial version: JSON recipes, JSON conditions, layer/biome/liquid checks, boss progress checks, hot reload, recipe audit and consumed drop cleanup.
