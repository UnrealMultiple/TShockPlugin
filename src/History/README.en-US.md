# History

- author: Cracker64 & Zaicon & Cai
- source: none
- Record each player's operations on the tiles
- This allows you to restore the tile using the command
- Or reproduce the player's actions
- Avoid some cases from the intention of the player's intentional damage that the game cannot be performed normally

## Instruction

| Command                        |  Alias   |        Permissions        |          Description          |
|---------------------------|:-----:|:----------------:|:--------------------:|
| /history [Account name] [Time] [Range]  |  /历史  |   history.get    |  Restore a user's behavior within the time span and radius   |
| /prunehist [Time]           |  /删除  |  history.prune   |   Delete history over a time span    |
| /reenact [Account name] [Time] [Range]  |  /复现  | history.reenact  | Create all the actions of the player in time span and radius |
| /rollback [Account name] [Time] [Range] |  /回溯  | history.rollback |  Restore all player actions within time span and radius  |
| /rollback [Account name] [Time] [Range] |  /撤销  | history.rollback |  Restore all player actions within time span and radius  |
| /hreset                   | /重置历史 |  history.admin   |        Reset the database table        |

---
Note
---
1.`The format of time is `dd:hh:mm:ss`[day/hour/minute/second]    
  
2.`Range` is the number of radius grids of your location`    

For example, to restore the tiles modified by Yuxue `2 hours ago` within the range of `200 blocks` at the current position: /rollback Yuxue 2h 200    

## Configuration

```json5
none
```

## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
