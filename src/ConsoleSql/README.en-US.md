# ConsoleSql

- Author: Cai
- Source: This repository
- Allows you to execute SQL statements in the console and BOT
 
> [!CAUTION]
> When executing dangerous SQL statements (DELETE, DROP, etc.), please check carefully (conditions, etc.), otherwise it may cause serious irreversible consequences.

## Example
```
List the data table name of Tshock:  
sql select name from sqlite_master where type='table'  
What is there to query "user data table":  
sql select * from users  
Modify the user's character name with ID 2 to Xien (help the player who forces the landing to change the character name):
sql update users set username='Xien' where id=2
```

## Configuration

```json
None
```
## Instruction

| Command           |        Permissions         |        Description         |
| -------------- | :----------------- | :-----------------: |
| /sql <SQL Statements>|ConsoleSql.Use |Execute SQL|


## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
