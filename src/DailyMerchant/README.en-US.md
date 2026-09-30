# DailyMerchant — Daily Travelling Merchant

- Author: 不是现在
- Target: .NET 9 / TShock 6.x (tested on 6.1.0 and 6.2.1)
- Server-side only, no client mod required

## How it works

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

There is no configuration file: the behaviour is fixed at "100% daily arrival".

## Commands

| Syntax | Permission | Description |
|--------|:----:|:----:|
| /merchant summon | tshock.admin | Summon the merchant right away (/merchant 召唤) |
| /merchant despawn | tshock.admin | Send him away via vanilla `UnspawnTravelNPC` (/merchant 离开) |
| /merchant check | tshock.admin | Run one arrival roll manually (/merchant 检查) |
| /merchant status | tshock.admin | Chance, in-game clock, count/position, town housing diagnostics (/merchant 状态) |

Aliases: `/merchant`, `/旅商`, `/dailymerchant`.

## Troubleshooting

`/merchant status` tells you exactly why he did not show up:

```
[旅商] 城镇：2 位 NPC（已入住 0，待搬入空房 0）
[旅商] 判定：城镇里有 2 位 NPC，但都没住进房子，原版没有落脚点（静默等待中）
[旅商] 原版生成：原版拒绝：既没有已入住的城镇 NPC，也没有可搬入的空房
```

- `已入住 0` → no town NPC owns a house, so vanilla refuses to spawn him; wait until the town NPCs are housed.
- Sundial / Moondial active → vanilla does not roll at all while that is the case.
- Night or past the window → try again the next morning.

**Empty server note**: a Terraria dedicated server does not advance the world with nobody online,
so the once-per-second automatic roll does not run. As soon as someone joins it resumes on its own.
From the console you can always force one roll with `/merchant check` (this is also what the
automated test loop uses).

## Changelog

### v1.0
- First release: raises the vanilla 22.12% daily arrival chance to 100%
- Skips silently when the conditions are not met, never force-places the merchant
- Adds `/merchant check` plus vanilla rejection-reason diagnostics
