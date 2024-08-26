# PluginManager automatic update plug-in

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Authors: Shao Siming, Cai,
- Source: this warehouse
- Use the instruction to automatically update the plug-in of the server (this warehouse only)

## Update log

```
v2.0.1.1
更新: apm u支持排除插件，支持自动更新插件，apm l优化显示 & 修复: 插件更新回旧版本，更新插件后不重启仍提示更新
v2.0.0.3
修复: 使用插件包最新目录结构
v2.0.0.2
更新: 插件仓库链接
v2.0.0.1
补全卸载函数
V2.0.0.0
1.正式更名为AutoPluginManager
2.添加安装插件功能
3.更改指令使用方式
```

## instruction

|grammar|limit of authority|explain|
| -------------- |:-----------------:|:------:|
|/apm -c|AutoUpdatePlugin|Check plug-in updates|
|/apm -u [plug-in name]|AutoUpdatePlugin|To upgrade the plug-in with one click, the server needs to be restarted, and the plug-in name can be multiple.`English comma separation`|
|/apm -l|AutoUpdatePlugin|View the list of warehouse plug-ins|
|/apm -i [plug-in serial number]|AutoUpdatePlugin|To install the plug-in, you need to restart the server, and the serial number of the plug-in is multiple.`English comma separation` cooperate `/apm -i` instruction use|
|/apm -b [plug-in name]|AutoUpdatePlugin|Exclude plug-ins from updates|
|/apm -rb [plug-in name]|AutoUpdatePlugin|Remove exclude updates|
|/apm -lb|AutoUpdatePlugin|List plug-ins that exclude updates|
## deploy

```json
暂无
```
## feedback
- Give priority to issued-> jointly maintained plug-in library: https://github.com/Controllerdestiny/TShockPlugin.
- Second priority: TShock official group: 816771079
- You can't see it with a high probability, but you can: domestic communities trhub.cn, bbstr.net, tr. monika.love.