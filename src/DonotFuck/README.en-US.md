# DonotFuck 禁止脏话

- Authors: 羽学 & Cai
- Source: [禁止脏话(内嵌版)](https://github.com/1242509682/DonotFuck)
- This is a Tshock server plugin primarily used for: detecting civilized speech among players on the server.

## Commands

| 语法             |  别名  |                     Permission                    |                      Description                      |
| -------------- | :--: | :-----------------------------------------------: | :---------------------------------------------------: |
| /af            | None |                     DonotFuck                     |                      Command menu                     |
| /af list       | None |                     DonotFuck                     | List sensitive words (for players) |
| /af log        | None |          DonotFuck.admin          |        Enable or disable sensitive word logging       |
| /af add <word> | None |          DonotFuck.admin          |                  Add a sensitive word                 |
| /af del <word> | None |          DonotFuck.admin          |                Remove a sensitive word                |
| /reload        | None | tshock.cfg.reload |             Reload the configuration file             |

## Configuration

> Configuration file location： tshock/禁止脏话/禁止脏话.json

```json
{
  "RowsPerPage": 30,
  "LogEnabled": true,
  "SensitiveWords": [
    "6",
    "六"
  ]
}
```

## 更新日志

```
v3.1
补充了i18n 英文翻译
加入了发言是否为指令的判断处理
加入了记录敏感词到配置文件目录下的《脏话纪录》
加入了/df 系列指令
将指令权限划分
DonotFuck：用于给玩家查询敏感词
DonotFuck.admin：用于给管理员添加或者移除敏感词

v3.0
重构代码逻辑，移除了封禁逻辑，当检测玩家有脏话时用*号代替
将免检的权限名更改为：DonotFuck

v2.0
1.修复了玩家被封禁后不会清空计数问题
2.修复了玩家随意发送字符1次都会导致封禁问题
3.修复了空引用，引起的聊天黄码刷屏问题
4.修复了玩家每次发脏话不会广播问题，
并列出所触发的敏感词，输出给所有玩家与控制台
5.添加了日志记录
6.移除了在Config中的【启用】选项

v1.0:  
1.可以用配置文件自定义：启用开关、词、封禁时长、是否封禁、检查次数
2.给配置文件加了Reload重载方法

```

## FeedBack

- - Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
