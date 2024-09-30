# PerPlayerLoot 

- Authors: Codian,肝帝熙恩汉化1449
- Source: [github](https://github.com/xxcodianxx/PerPlayerLoot)
- A TShock server plugin which makes naturally spawned loot chests have a separate inventory for each player on your server. Every player that finds a chest can loot it for themselves, even if it has been looted by someone else before.
- you need to have the plugin installed from the very beginning of your server. If you install it halfway through a playthrough, all chests in the world will be treated as if they were generated and will duplicate their inventory contents for each player.
- SQLite file：in tshock/`perplayerloot.sqlite`


## Commands

| Command | Permission |             Details             |
| -------------- | :-----------------: | :------: |
| /ppltoggle | perplayerloot.toggle   | Toggle the plugin packet hooks globally|
- **warning**：Debug use only! When in a disabled state, any chests you place will become loot chests, and any chest inventory accessed will be its real inventory, not a per-player instanced one!


## Config

```
None
```

## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
