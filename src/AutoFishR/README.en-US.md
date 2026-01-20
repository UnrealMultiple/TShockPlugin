# AutoFishR (Auto Fishing Remastered)

- Authors: ksqeib / 羽学 / 少司命
- Description: Auto fishing plugin for TShock servers. Supports auto reel-in, multi-hook, Buffs, extra loot, consumption mode, etc. Commands can be shown/hidden dynamically via permissions and global switches.
- Legacy repo: https://github.com/ksqeib/AutoFish-old

## Permission Model (Important)

- Admin bypass: `autofish.admin`.
- Common whitelist: `autofish.common`; owning it grants all player commands (still affected by global toggles and negative permissions).
- Feature permissions: `autofish.<feature>`; examples: `autofish.fish`, `autofish.multihook`, `autofish.filter.unstackable`, etc.
- Negative permissions: `autofish.no.<feature>`; owning it forces denial (except admin). Examples: `autofish.no.fish`.
- `/af` itself needs `autofish`; `autofish.common` is equivalent to all player commands.

Example:
- If you want the default group to use everything except auto fishing, give group `autofish.common` and also `autofish.no.fish`. Players can use Buff/Multi-hook, etc., but cannot enable auto fishing.

## Player Commands (/af, /autofish)

| Command | Description | Permission | Prerequisite |
| --- | --- | --- | --- |
| /af | Show menu/help | autofish | Plugin enabled |
| /af status | Show personal status | autofish |  |
| /af fish | Toggle auto fishing | autofish.fish | Global auto fishing enabled |
| /af buff | Toggle fishing Buffs | autofish.buff | Global Buff enabled |
| /af multi | Toggle multi-hook | autofish.multihook | Global multi-hook enabled |
| /af hook <number> | Set personal hook cap | autofish.multihook | Global multi-hook enabled; value ≤ global cap |
| /af stack | Toggle filtering unstackable loot | autofish.filter.unstackable | Global filter enabled |
| /af monster | Toggle avoid fishing monsters | autofish.filter.monster | Global anti-monster enabled |
| /af anim | Toggle skip catch animation | autofish.skipanimation | Global animation skip enabled |
| /af list | View consumption-mode items | autofish | Global consumption mode enabled |
| /af loot | View extra loot table | autofish | Extra loot list configured and non-empty |
| /af bait | Toggle precious bait protection | autofish.bait.protect | Global precious bait protection enabled |
| /af baitlist | View precious bait list | autofish.bait.protect | Same as above |

> Negative permissions win: with `autofish.no.<feature>`, everyone except admin is treated as no-permission.

## Admin Commands (/afa, /autofishadmin)

All require `autofish.admin`.

| Command | Description |
| --- | --- |
| /afa | Show admin help menu |
| /afa buff | Toggle global fishing Buff |
| /afa multi | Toggle global multi-line mode |
| /afa duo <number> | Set global multi-hook cap |
| /afa mod | Toggle global consumption mode |
| /afa set <amount> | Set consumption item quantity (when consumption mode is on) |
| /afa time <minutes> | Set reward duration in minutes (when consumption mode is on) |
| /afa add <item> | Add allowed bait (visible when consumption mode is on) |
| /afa del <item> | Remove allowed bait (visible when consumption mode is on) |
| /afa addloot <item> | Add extra loot |
| /afa delloot <item> | Remove extra loot |
| /afa stack | Toggle global filtering unstackable loot |
| /afa monster | Toggle global avoid fishing monsters |
| /afa anim | Toggle global skip catch animation |

Others: `/reload` (tshock.cfg.reload) to reload config.

## Config
See [resource/config/zh-cn.yml](resource/config/zh-cn.yml) or [resource/config/en-us.yml](resource/config/en-us.yml). When missing, the plugin writes a default template based on system language.

## Notes

- Simplest setup for `/af` for regular players: give group `autofish.common`. To disable a specific feature, additionally grant `autofish.no.<feature>`.
- With consumption mode on, players must have personal duration; the plugin returns early if bait is missing.
- Multi-hook/filter/anti-monster/skip-animation all honor "global switch + personal switch + permission" simultaneously.

## Mechanics (Behavior and Key Logic)

- Auto fishing: during bobber AI update, detect `bobber.ai[1] < 0` (caught), consume bait, call vanilla reel logic, then re-send projectile. If extra loot/monster filtering/unstackable filtering is enabled, filter/replace before drops spawn.
- Multi-hook: when spawning fishing line projectiles, count current bobbers; if under cap, duplicate a fishing line projectile for the player, enabling parallel fishing. Also gated by consumption mode and player multi-hook toggle.
- Skip catch animation: after reeling, send `ProjectileDestroy` to the client to skip the animation.
- Avoid fishing monsters: if result is a monster (catchId < 0) and feature is on, discard and retry.
- Filter unstackable: if drop `maxStack == 1` and filtering is on, discard and retry.
- Protect precious bait: check current bait against precious list; if matched, swap with bait at the end of inventory and sync slots to prevent consumption.
- Consumption mode: when globally on, player must enable personally and have remaining duration to run auto fishing/multi-hook. Duration is exchanged via consuming specified items (commands and logic follow config fields).
- Buffs: when player has a fishing line and global/personal Buff is on, apply configured Buff list (ID + duration).
- Hint and first-fish: on first cast, prompt player that `/af fish` can enable auto fishing (once only).

## Feedback

- Issues: https://github.com/UnrealMultiple/TShockPlugin
- QQ: 816771079
- Community: trhub.cn / bbstr.net / tr.monika.love

## Changelog

- See CHANGELOG.md
