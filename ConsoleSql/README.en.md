# Consolesql

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: CAI
- Source: This warehouse
- Allow you to use SQL statements in the console and bot
 
> [! Caution]
> When performing a dangerous SQL statement (delete, drop, etc.), please check (condition, etc.) carefully, otherwise it may cause serious irreversible consequences

## Exemplary example
```
列出Tshock的数据表名：  
sql select name from sqlite_master where type='table'  
查询“用户数据表”有什么：  
sql select * from users  
修改ID为2的用户角色名字为熙恩（帮强制开荒玩家换角色名）：  
sql update users set username='熙恩' where id=2   
```

## Configuration

```json
无
```
## Order

|Order name|Authority|illustrate|
| -------------- |: -----------------------|: --------------------:
|/sql <sql statement>|Consolesql.use|Execute SQL


## Update log

```
暂无
```

## feedback
- Priority to ISSUED-> Commonly maintained plug -in library: https://github.com/Controllerdestiny/tshockPlugin
- Two priority: TSHOCK official group: 816771079
- It may not be seen in a high probability, but it can also be: domestic community trhub.cn, bbstr.net, tr.monika.love