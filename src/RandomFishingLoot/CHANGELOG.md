# 更新日志 / Changelog

## 2.5.0

- 优化整体代码架构，按职责拆分文件（入口、命令、配置、配置校验、默认渔获表、进度、任务鱼、NPC 池、网络、模型、抽取池）
- 配置文件自动生成：首次启动自动写入 `tshock/RandomFishingLoot.json`，包含完整默认渔获表与说明
- 重构默认渔获表数据，支持 /fishrand sample 预览与 /fishrand stage 详情
- 钓鱼浮标与宝匣 ID 改用 Terraria 命名常量（ProjectileID / ItemID），不再硬编码魔法数字，并修正了旧硬编码区间误把钓鱼武器（CrystalSerpent/Toxikarp/Bladetongue）当作宝匣放行的问题
- 配置了不支持的 Mode 时不再静默切换：回退为 progression_items 并在控制台日志、/fishrand 状态和重载提示中显示警告
- 旧版配置升级时保留用户配置（Enabled、AnnounceToPlayer、Mode、BlockedItemIds、RandomNpc）
- Restructured code by concern: entry, commands, config, validation, default tables, progression, quest fish, NPC pool, network, models, roll pools
- Config auto-generation: `tshock/RandomFishingLoot.json` is written on first start with full default tables and notes
- Fishing bobber and crate IDs now use named Terraria constants (ProjectileID / ItemID) instead of magic numbers; fixed the old hardcoded ranges that treated fishing weapons (CrystalSerpent/Toxikarp/Bladetongue) as crates
- Unsupported Mode values no longer switch silently: falls back to progression_items and logs a warning to console, /fishrand status and reload output
- Legacy config upgrade now preserves user settings (Enabled, AnnounceToPlayer, Mode, BlockedItemIds, RandomNpc)
