# Cgive offline command

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: Leader
- Source: None
- Offline execution commands, players automatically execute commands when logging in to the game.

## Update log

```
V1.0.0.1
- 优化简化部分代码，完善卸载函数
```

## REST API

|Path|Authority|illustrate|
| ------------- |:-:|: -----------:|
|/getwarehouse|none|Query/GIVE command specific information|

## instruction

|grammar|Authority|illustrate|
| ----------------------------- |: ----------:|: ---------------------:|
|/CGive Personal [Command] [target]|cgive.admin|Add a command to a player|
|/cgive all [executor] [command]|cgive.admin|All players offline command|
|/cgive list|cgive.admin|Offline command list|
|/CGIVE DEL [ID]|cgive.admin|Delete offline command|
|/cgive reset|cgive.admin|Reset the offline command|

## Configuration

```json
暂无
```
## feedback
- Priority to ISSUED-> Commonly maintained plug -in library: https://github.com/Controllerdestiny/tshockPlugin
- Two priority: TSHOCK official group: 816771079
- It may not be seen in a high probability, but it can also be: domestic community trhub.cn, bbstr.net, tr.monika.love