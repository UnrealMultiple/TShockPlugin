# 更新日志 / Changelog

## 2.5.0

- 优化整体代码架构，按职责拆分文件（入口、命令、配置、配置校验、默认渔获表、进度、任务鱼、NPC 池、网络、模型、抽取池）
- 配置文件自动生成：首次启动自动写入 `tshock/RandomFishingLoot.json`，包含完整默认渔获表与说明
- 重构默认渔获表数据，支持 /fishrand sample 预览与 /fishrand stage 详情
- Restructured code by concern: entry, commands, config, validation, default tables, progression, quest fish, NPC pool, network, models, roll pools
- Config auto-generation: `tshock/RandomFishingLoot.json` is written on first start with full default tables and notes
