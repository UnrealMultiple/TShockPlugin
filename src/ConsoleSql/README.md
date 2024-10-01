# ConsoleSql

- 作者: Cai
- 出处: 本仓库
- 允许你使用在控制台和BOT中执行SQL语句
 
> [!CAUTION]
> 执行危险SQL语句(DELETE、DROP等)时，请仔细检查(条件等)，否则可能造成严重不可逆后果

## 示例
```
v1.0.0
i18n预备

列出Tshock的数据表名：  
sql select name from sqlite_master where type='table'  
查询“用户数据表”有什么：  
sql select * from users  
修改ID为2的用户角色名字为熙恩（帮强制开荒玩家换角色名）：  
sql update users set username='熙恩' where id=2   
```

## 配置

```json
无
```
## 命令

| 命令名           |        权限         |        说明         |
| -------------- | :----------------- | :-----------------: |
| /sql <SQL语句>|ConsoleSql.Use |执行SQL|


## 更新日志

```
暂无
```

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
