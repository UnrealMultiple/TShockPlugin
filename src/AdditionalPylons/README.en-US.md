# AdditionalPylons

- Authors: Stealownz, 肝帝熙恩优化1449
- Source: [github](https://github.com/Adventure-Terraria-Server-Project/AdditionalPylons-Plugin)
- Configure independent placement limits for 11 pylon types (minimum 1), with optional NPC and biome check bypasses for teleportation.
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
  "NoTownEnvironment": false
}
```

`NoTownEnvironment` defaults to `false`. When enabled, it bypasses only the server-side town NPC and biome checks, not other teleportation conditions. Changes take effect with `/reload` and are not scoped to individual player permissions. Limits below 1 are treated as 1.

## Upgrade notes

- Targets TShock 6.1.0 / Terraria 1.4.5.6 / .NET 9. The `LazyAPI` dependency and `AdditionalPylons` permission remain unchanged.
- Back up and retain existing configuration values; add the three new fields as needed. Missing fields use the defaults shown above.
- Stop the server and remove a previously installed `FreePylons.dll` to avoid conflicting condition hooks.
- MonoMod runtime dependencies are embedded; `LazyAPI` must still be provided by the server.
- A `FAILED` hook-installation log means the environment option is unavailable; placement logic remains enabled.
- Placement limits retain the upstream client-assisted synchronization approach, not independent server-authoritative quotas.

## Changelog

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
