# PluginManager automatic update plug -in

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: Shao Si Ming, CAI
- Source: This warehouse
- Use the instruction to automatically update the server's plug -in (only the warehouse)

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

|grammar|Authority|illustrate|
| -------------- |: --------------------:|: -------:|
|/APM -C|Autopdateplugin|Check the plug -in update|
|/APM -U [Plug -in Name]|Autopdateplugin|One -click upgrade plug -in, you need to restart the server, the plug -in name can be selected more `English comma separate`|
|/APM -L|Autopdateplugin|View warehouse plug -in list|
|/APM -i [Plug -in serial number]|Autopdateplugin|Install the plug -in, you need to restart the server, the plug -in serial number is multiple choice `English comma separate` cooperate `/apm -i` instruction|
|/apm -b [plug -in name]|Autopdateplugin|Exclude the plug -in update|
|/APM -RB [Plug -in name]|Autopdateplugin|Removal and update|
|/APM -LB|Autopdateplugin|List the plug -in to exclude the update|
## Configuration

```json
暂无
```
## feedback
- Priority to ISSUED-> Commonly maintained plug -in library: https://github.com/Controllerdestiny/tshockPlugin
- Two priority: TSHOCK official group: 816771079
- It may not be seen in a high probability, but it can also be: domestic community trhub.cn, bbstr.net, tr.monika.love