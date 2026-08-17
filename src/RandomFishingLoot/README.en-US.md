# RandomFishingLoot

- Author: 愚蠢
- Version: 2.5.0
- Description: Replace caught fishing loot with progression-based stage loot

## Features

- **progression_items mode**: Fishing loot tables unlock by boss/progression stage; the further you progress, the better the catch, stacked on top of an always-available base pool
- **random_npcs mode**: Fishing randomly spawns a usable NPC (bosses, friendly NPCs, town NPCs are optional) with a configurable replacement chance
- Auto-generated config: `tshock/RandomFishingLoot.json` is created on first start with the full default loot tables and notes
- Hot reload: `/fishrand reload` applies config changes immediately
- Vanilla angler quest fish and crates are preserved by default
- Entries are validated on load: invalid item IDs, blocked items, and direct combat weapons are skipped

## Commands

| Syntax | Alias | Permission | Description |
|--------|-------|------------|-------------|
| `/fishrand` | | None | Show current mode, stage and status |
| `/fishrand reload` | `/fishrand r` | fishrand.admin | Reload the config file |
| `/fishrand sample <count>` | `/fishrand preview` | None | Preview current stage loot (default 8, max 20) |
| `/fishrand stages` | | None | List all stages and their unlock status |
| `/fishrand stage <stageId>` | | None | Show the items of a specific stage |

## Configuration

> Config file: `tshock/RandomFishingLoot.json`, auto-generated on first start.

| Key | Default | Description |
|-----|---------|-------------|
| Enabled | true | Master switch |
| Mode | progression_items | `progression_items` or `random_npcs` |
| AnnounceToPlayer | false | Show a message when something is caught |
| AllowQuestFish | true | Keep vanilla angler quest fish |
| AllowCrates | true | Keep vanilla crates |
| BlockedItemIds | [] | Blocked item IDs (skipped on load) |
| RandomNpc.ReplaceChancePercent | 100 | Replacement chance in random_npcs mode |
| RandomNpc.IncludeBosses | true | Include bosses |
| RandomNpc.IncludeFriendlyNPCs | true | Include friendly NPCs |
| RandomNpc.IncludeTownNPCs | false | Include town NPCs |
| RandomNpc.BlockedNpcIds | [] | Blocked NPC IDs |
| AlwaysAvailable | Base material/potion pool | Always-active base pool |
| Stages | 10 progression stages | Loot tables unlocked by boss progress |

### Stage unlock conditions (Stages[].Unlock)

| Key | Description |
|-----|-------------|
| HardMode | Require hardmode (true/false/null) |
| GameMode | Game mode (0 Classic / 1 Expert / 2 Master / 3 Journey) |
| Defeated | Bosses that must be defeated |
| NotDefeated | Bosses that must NOT be defeated |

Boss keywords: `eye`, `evilboss`, `skeletron`, `queenbee`, `wof`, `anymech`, `twins`, `destroyer`, `skeletronprime`, `allmechs`, `plantera`, `golem`, `cultist`, `moonlord`

### Loot entry fields (LootEntry)

| Field | Description |
|-------|-------------|
| Name | Display name (leave empty for the vanilla item name) |
| ItemId | Terraria ItemID |
| Weight | Roll weight |
| MinStack / MaxStack | Stack range per catch |

## Changelog

- **2.5.0**
  - Split code architecture into per-concern files
  - Config file is auto-generated with full default loot tables
