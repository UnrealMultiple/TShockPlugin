# ConsoleSql

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: Cai
- Source: this warehouse
- Allows you to execute SQL statements in the console and BOT.
 
> [! CAUTION]
> When executing dangerous SQL statements (DELETE, DROP, etc.), please carefully check (conditions, etc.), otherwise it may cause serious irreversible consequences.

## example
```
列出Tshock的数据表名：  
sql select name from sqlite_master where type='table'  
查询“用户数据表”有什么：  
sql select * from users  
修改ID为2的用户角色名字为熙恩（帮强制开荒玩家换角色名）：  
sql update users set username='熙恩' where id=2   
```

## deploy

```json
无
```
## order

|Command name|limit of authority|explain|
| -------------- |:-----------------|:-----------------:
|/sql <SQL statement >|ConsoleSql.Use|Execute SQL


## Update log

```
暂无
```

## feedback
- Give priority to issued-> jointly maintained plug-in library: https://github.com/Controllerdestiny/TShockPlugin.
- Second priority: TShock official group: 816771079
- You can't see it with a high probability, but you can: domestic communities trhub.cn, bbstr.net, tr. monika.love.