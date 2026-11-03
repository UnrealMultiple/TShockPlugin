# AdditionalPylons

- Authors: Stealownz, 肝帝熙恩优化1449
- Source: [github](https://github.com/Adventure-Terraria-Server-Project/AdditionalPylons-Plugin)
- Configure independent placement limits for 11 pylon types (minimum 1), with optional NPC and biome check bypasses for teleportation, plus an option to teleport from anywhere.
- The v1.1.0 update code and documentation are AI-generated; original authors and source attribution are retained.

## Commands

| Command |    Permission    |             Details             |
|---------|:----------------:|:-------------------------------:|
| None    | AdditionalPylons | Permission to place more Pylons |

## Config
> Configuration file location：tshock/AdditionalPylons.en-US.json
```json5
{
  "MaxJunglePylons": 2,
  "MaxForestPylons": 2,
  "MaxHallowPylons": 2,
  "MaxCavernPylons": 2,
  "MaxOceanPylons": 2,
  "MaxDesertPylons": 2,
  "MaxSnowPylons": 2,
  "MaxMushroomPylons": 2,
  "MaxUniversalPylons": 2,
  "MaxUnderworldPylons": 2,
  "MaxShimmerPylons": 2,
  "NoTownEnvironment": false,
  "TeleportFromAnywhere": false
}
```

`NoTownEnvironment` defaults to `false`. When enabled, it bypasses only the server-side town NPC and biome checks, not other teleportation conditions. Changes take effect with `/reload` and are not scoped to individual player permissions. Limits below 1 are treated as 1.

`TeleportFromAnywhere` defaults to `false`, keeping the vanilla "must stand near a pylon" rule. When enabled, the server temporarily skips both proximity checks (nearby pylon tile and tile-entity interaction range) for that player while a teleport request is being processed, so players farther than 60 tiles from any pylon can still teleport by clicking a destination pylon on the full-screen map. Every other interaction keeps its vanilla reach check, and the remaining teleport conditions are unchanged.

## Upgrade notes

- Targets TShock 6.1.0 / Terraria 1.4.5.6 / .NET 9. The `LazyAPI` dependency and `AdditionalPylons` permission remain unchanged.
- Back up and retain existing configuration values; add the three new fields as needed. Missing fields use the defaults shown above.
- Stop the server and remove a previously installed `FreePylons.dll` to avoid conflicting condition hooks.
- MonoMod runtime dependencies are embedded; `LazyAPI` must still be provided by the server.
- A `FAILED` hook-installation log means the environment option is unavailable; placement logic remains enabled.
- Placement limits retain the upstream client-assisted synchronization approach, not independent server-authoritative quotas.

## Changelog

### v1.2.0 (AI-generated)

- Add the `TeleportFromAnywhere` option (default: off). While a teleport request is being handled, the server skips the "must be near a pylon" and tile-entity interaction range checks for that player only, so players can teleport to another pylon from anywhere via the full-screen map. Existing cleanup on pylon switching, disconnects and unload is unaffected.
- The option only relaxes reach checks during teleport requests; chests, doors and other interactions keep their vanilla reach, and it is independent of `NoTownEnvironment`.
- `/reload` writes the config back to its file so options added by an upgrade appear automatically with their defaults; the startup log prints the effective values of both toggles prefixed with `[AdditionalPylons]` to make mismatched config files obvious.

### v1.1.0 (AI-generated)

- Add Underworld and Shimmer pylons, bringing support to 11 types.
- Correct Jungle/Forest item mappings using `ItemID` constants.
- Check quotas only for the held type and restore client lists regardless of quotas.
- Handle direct pylon switches, permission changes, disconnects and unload cleanup.
- Add reloadable NPC/biome bypass configuration, hook failure rollback and disposal.
- Embed MonoMod runtime dependencies; retain original authorship and attribution.

## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
