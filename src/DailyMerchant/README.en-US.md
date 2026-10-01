# DailyMerchant — Daily Travelling Merchant and Skeleton Merchant

- Author: 不是现在
- Target: .NET 9 / TShock 6.x (tested on 6.1.0 and 6.2.1)
- Server-side only, no client mod required

One plugin, two merchants, one switch each, both enabled by default:

| Switch (config file `DailyMerchant.json`) | Default | Effect |
|---|:---:|:---|
| `启用旅商每日到访` | on | The Travelling Merchant (NPC 368) shows up every day |
| `启用骷髅商人每日到访` | on | The Skeleton Merchant (NPC 453) appears near the world spawn every morning |

The file is created on first start in the TShock config directory (`TShock.SavePath`, usually `tshock/DailyMerchant.json` on the server);
edit it and run `/tconfig reload` to apply it without a restart.

## Travelling Merchant (NPC 368)

Vanilla does **not** roll once per day for the Travelling Merchant (NPC 368): it rolls **every tick**
between 4:30 AM and 12:00 PM (27000 ticks) with a `1/108000` chance, which works out to only
`1 - (107999/108000)^27000 ≈ 22.12%` per day. The chance is multiplied by 5 while a player sleeps,
and the merchant never appears while a Sundial / Moondial is active.

This plugin **never spawns the NPC itself**; it only makes the roll happen every day:

- It scans once per second and mirrors the vanilla conditions one by one (daytime, inside the
  4:30 AM + 450 minute window, at least 2 town NPCs, no Sundial / Moondial, no eclipse or invasion);
- when they are met it calls the game's own `WorldGen.SpawnTravelNPC()`; the shop stock
  (`Chest.SetupTravelShop` + `NetMessage.SendTravelShop`), client sync and the arrival/departure
  broadcasts are all vanilla;
- when they are not met (for example no town NPC has a house yet) it **skips silently** and waits
  for the next scan — it never force-places the merchant.

Vanilla already handles the evening departure, so the plugin does not duplicate that.
Killing the merchant in the morning re-rolls him the next day, keeping the vanilla reroll trick.

## Skeleton Merchant (NPC 453)

- **Once every morning**: the first second after 4:30 AM of each game day, while it is daytime, inside
  the 4:30 AM – 12:00 PM window, and no Sundial / Moondial is active.
- **Fixed spot**: the anchor is found by a deterministic ring search around the spawn point (solid floor
  below, the two body tiles and both sides free, no liquid, not a dungeon / blue brick), so the same spawn
  point always yields the same tile.
- **Team-based spawn seeds**: seeds that hand out different spawns per team (`Main.teamBasedSpawnsSeed`)
  offer several candidates, and the plugin picks **one at random**.
- **Awkward spawn fallback**: world generation often leaves the spawn point unusable (inside a wall, in
  water, on a floating platform), so each candidate is searched within 40 tiles first and, if nothing
  stands, again within 120 tiles.
- **He leaves at nightfall**: the leave condition matches the Travelling Merchant's — it follows the
  **in-game clock, not player distance**. The clear-out is the vanilla `UnspawnTravelNPC` recipe (reset
  `active`/`life` + send packet 23) and takes every Skeleton Merchant on the field, whoever spawned it,
  so that "one arrives every morning" always holds.
- **Once per game day**, killing him does not queue a replacement; he returns the next morning.
- **Never interferes**: if a Skeleton Merchant is already on the field (dungeon spawn, another plugin),
  no second one is created that day.

**Stock is entirely vanilla**: the shop table is computed client-side from `Main.moonPhase`
(`Chest.SetupShop(453)` + `ShopHelper.GetSkeletonMerchantPrices`), and `Main.moonPhase` is incremented
by `Main.UpdateTime` at 4:30 AM every day. So the plugin caches, broadcasts and rewrites nothing: as long
as someone is in the world, the stock refreshes by itself the next morning.

> One vanilla detail: those area checks use **the position of the player who opens the shop**, not the
> merchant's position.

## Commands

| Syntax | Permission | Description |
|--------|:----:|:----:|
| /merchant summon | tshock.admin | Summon the travelling merchant right away (/merchant 召唤) |
| /merchant despawn | tshock.admin | Send him away via vanilla `UnspawnTravelNPC` (/merchant 离开) |
| /merchant check | tshock.admin | Run one arrival roll manually (both merchants) |
| /merchant status | tshock.admin | Clock, moon phase, counts, town housing diagnostics, switches (/merchant 状态) |

Aliases: `/merchant`, `/旅商`, `/dailymerchant`.
` summon`/`despawn` only affect the Travelling Merchant; the Skeleton Merchant follows the rules above.

## Troubleshooting

`/merchant status` tells you exactly why he did not show up:

```
[旅商] 城镇：2 位 NPC（已入住 0，待搬入空房 0）
[旅商] 判定：城镇里有 2 位 NPC，但都没住进房子，原版没有落脚点（静默等待中）
[旅商] 原版生成：原版拒绝：既没有已入住的城镇 NPC，也没有可搬入的空房
[骷髅商人] 当前：白天 04:31，月相 1，场上骷髅商人 0 位，位置 当前不在场
[骷髅商人] 出生点：(3568, 1837)，判定：条件已满足，将出现在出生点附近
```

- `已入住 0` → no town NPC owns a house, so vanilla refuses to spawn him; wait until the town NPCs are housed.
- Sundial / Moondial active → vanilla does not roll at all while that is the case.
- Night or past the window → try again the next morning.
- Skeleton Merchant `暂无玩家在线` → nobody is online, so the dedicated server is not advancing the world.
- Switched off → status reports `已在配置里关闭`.

**Empty server note**: a Terraria dedicated server does not advance the world with nobody online,
so the once-per-second automatic roll does not run. As soon as someone joins it resumes on its own.
From the console you can always force one roll with `/merchant check` (this is also what the
automated test loop uses).

## Changelog

### v1.1

- Merges the former DailySkeletonMerchant plugin: the Skeleton Merchant now shows up near the spawn
  point every morning from 4:30 AM and leaves at nightfall
- Adds the `DailyMerchant.json` config file with one switch per merchant, both on by default,
  reloadable with `/tconfig reload`
- The automatic roll now uses a real-time interval (1 second) instead of a frame count
- Adds the invasion case to the Travelling Merchant diagnostics

### v1.0
- First release: raises the vanilla 22.12% daily arrival chance to 100%
- Skips silently when the conditions are not met, never force-places the merchant
- Adds `/merchant check` plus vanilla rejection-reason diagnostics