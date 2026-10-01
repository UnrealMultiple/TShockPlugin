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
edit it and restart the server to apply it.

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

Vanilla only spawns the Skeleton Merchant randomly inside dungeons - there is no "he visits once a day" in
the base game. The plugin's only job is to **bring him to the spawn point**; after that it stays out of the way.

### Spawn rule

Once per second, **all four** of these must hold (in the order the code checks them):

1. the switch `启用骷髅商人每日到访` is on;
2. it is daytime (`Main.dayTime`);
3. no Skeleton Merchant is on the field (dungeon spawns, other plugins' NPCs and yesterday's leftover all count);
4. there is a player within **2000 pixels (125 tiles)** of the anchor - the fixed spot near the spawn point.

If all four hold, one is spawned at the anchor, and from then on his stock, behaviour and eventual
disappearance are entirely vanilla. **If any of them fails, nothing is spawned, and nothing is queued
"to try again later" either.**

| Time | Players near the spawn | One already there | Result |
|---|:---:|:---:|---|
| Day | someone | no | spawns one at the anchor |
| Day | someone | yes | does not spawn a second one |
| Day | nobody | - | does not spawn (nobody would see him) |
| Night | someone | yes | no action, he stays put until vanilla clears him |
| Night | someone | no | does not spawn |
| Night | nobody | - | no action |

### What happens when you walk away

**He disappears, and the plugin does not interfere.** Vanilla clears NPCs farther than 2000 pixels
(125 tiles) from every player, and the Skeleton Merchant is not a town NPC, so the same rule applies to him.
A typical day therefore looks like this: a player is near the spawn point -> he appears; the player walks to
the beach -> vanilla clears him; the player walks back -> the very next scan spawns one again. Being killed
works the same way: the next one appears once someone walks near the spawn point during the day.

> The "someone nearby" radius is also 2000 pixels for the same reason: with a player that close, vanilla
> will not clear a freshly spawned merchant, so he never appears and vanishes within the same second.
> The only way to make him immune to the vanilla unload is to make him a town NPC (`npc.townNPC = true`,
> which is what the Travelling Merchant is), and the plugin does not do that - it would also change how
> vanilla counts town NPCs for the Travelling Merchant's arrival roll.

### Where he stands

- **Fixed spot**: the anchor is found by a deterministic ring search around the spawn point (solid floor
  below, the two body tiles and both sides free, no liquid, not a dungeon / blue brick), so the same spawn
  point always yields the same tile.
- **Team-based spawn seeds**: seeds that hand out different spawns per team (`Main.teamBasedSpawnsSeed`)
  offer several candidates, and the plugin picks **one at random**.
- **Awkward spawn fallback**: world generation often leaves the spawn point unusable (inside a wall, in
  water, on a floating platform), so each candidate is searched within 40 tiles first and, if nothing
  stands, again within 120 tiles.
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
` summon`/`despawn` only affect the Travelling Merchant; the Skeleton Merchant is only spawned by the rule above and has no summon/despawn.

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
- Skeleton Merchant `出生点附近没人` → nobody is near the spawn point, so nothing is spawned; walk over and he appears.
- Switched off → status reports `已在配置里关闭`.

**Empty server note**: a Terraria dedicated server does not advance the world with nobody online,
so the once-per-second automatic roll does not run. As soon as someone joins it resumes on its own.
From the console you can always force one roll with `/merchant check` (this is also what the
automated test loop uses).

## Changelog

### v1.1

- Merges the former DailySkeletonMerchant plugin: the Skeleton Merchant now shows up near the spawn
  point while a player is nearby during the day; his comings and goings are left to vanilla
- Adds the `DailyMerchant.json` config file with one switch per merchant, both on by default,
  applied on the next server restart; the plugin also listens for TShock's config-reload event, so servers running TSConfig can hot-reload it
- The automatic roll now uses a real-time interval (1 second) instead of a frame count
- Adds the invasion case to the Travelling Merchant diagnostics

### v1.0
- First release: raises the vanilla 22.12% daily arrival chance to 100%
- Skips silently when the conditions are not met, never force-places the merchant
- Adds `/merchant check` plus vanilla rejection-reason diagnostics