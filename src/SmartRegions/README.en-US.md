# Smart Regions

- **Author**: GameRoom, 肝帝熙恩fix
- **Source**: [Github](https://github.com/ZakFahey/SmartRegions)  
- **Description**:  
  - Executes custom commands when players enter a region.  
  - Use cases: Assign teams, heal players, grant items, or any custom action. Endless possibilities!  
  - Placeholder `[PLAYERNAME]` is replaced with the player's name in the region.  
  - Backward compatible (SQLite or JSON, mutually exclusive).  
  - Regions ending with `--` execute commands only if they have the highest Z-axis value among overlapping regions.  
  - **Cooldown**: Minimum interval (in milliseconds) between activations.  
  - Supports single commands or command files (place multi-line command files in `tshock/SmartRegions/`).  



## Commands

| Command                                 | Permissions             | Description                                   |
|-----------------------------------------|-------------------------|-----------------------------------------------|
| `/smartregion add <region> <cooldown> <command>` | `SmartRegions.manage` | Bind commands to a region (supports commands or file references). |
| `/smartregion remove <region>`          | `SmartRegions.manage` | Remove command bindings from a region.        |
| `/smartregion check <region>`           | `SmartRegions.manage` | View region details (cooldown, commands).     |
| `/smartregion list [page] [distance]`   | `SmartRegions.manage` | List all smart regions (pagination/distance filtering). |
| `/replace <parameters>`                 | `SmartRegions.manage` | Replace region configurations or override old regions. |



## Configuration  
> Configuration file location：tshock/SmartRegions/SmartRegions.sqlite


## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
