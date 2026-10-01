# DailySkeletonMerchant — the Skeleton Merchant shows up every day

- Author: 不是现在
- Target framework: .NET 9 / TShock 6.x (tested on 6.1.0 and 6.2.1)

## Behaviour

- **One visit per game day**: on the first second after 4:30 AM each game day, a Skeleton Merchant (NPC 453) spawns at a fixed spot near the world spawn point.
- **Fixed position**: the anchor is found by walking outwards from the spawn tile in a fixed order (solid floor, two empty tiles of headroom plus both sides empty, no liquid, not a dungeon brick), so the same spawn point always lands on the same tile.
- **Seeds with several spawn points**: some map seeds hand out a different spawn point per team. The plugin then picks one of those **at random** each morning.
- **Awkward spawn points are handled**: world generation only guarantees a spawn point is "roughly standable" — it can sit in a wall, in water, on dungeon bricks or on a floating platform. The plugin searches 40 tiles around each spawn point, and widens that to 120 tiles if nothing works.
- **He leaves when the players leave**: as soon as no online player is within 50 tiles (the vanilla NPC despawn radius of 800 pixels), he is removed exactly like vanilla `UnspawnTravelNPC` does it (`active`/`life` cleared plus the sync packet broadcast).
- **No second visit the same game day**: once he has been around, he will not come back that day — whether he left on his own or a player killed him. The next attempt is at 4:30 AM the following day.
- **Death is not patched up**: he only has 250 life, so if players kill him, he waits for the next day.
- **An existing Skeleton Merchant is left alone**: if any 453 is already on the field — one that spawned in a cave by itself, one left over from yesterday, or one summoned by an admin — the plugin does not spawn a second one and does not touch it that day.
- **Idle servers do not burn the day's visit**: a Terraria world does not advance without players, so the visit is simply made as soon as somebody joins during the morning window.
- **No configuration file, no commands, no broadcasts**; the behaviour above is fixed.

## The stock stays vanilla

The Skeleton Merchant's items and prices have always been computed **client side from the moon phase**:

- item list: `Chest.SetupShop(453)`, generated from `Main.moonPhase` / `Main.dayTime` every time the shop is opened;
- price multiplier: `ShopHelper.GetSkeletonMerchantPrices` (up to 1.4 on a full moon, +0.1 during the day);
- `Main.moonPhase` is incremented once per day by `Main.UpdateTime` at **4:30 AM**.

So this plugin caches, broadcasts and rewrites nothing: as long as he is around, the stock rolls over automatically the next morning. That is exactly how "keep the vanilla stock refreshing every morning" is satisfied.

One vanilla detail worth knowing: those area checks use the position of the **player who opened the shop**, not the merchant's. Park him at the world spawn and a player standing in a graveyard gets the graveyard variant of his stock.

## Changelog

### v1.0

- First release: the Skeleton Merchant shows up once a day at a fixed spot near the world spawn
- Leaves when the players leave, does not return the same day, is not respawned after death
- Stays out of the way when a Skeleton Merchant is already on the field
- Seeds with per-team spawn points get one of them at random; awkward spawn points are searched outwards
- Stock stays vanilla and rolls over every morning at 4:30 AM with the moon phase

## Feedback

- Open an issue first