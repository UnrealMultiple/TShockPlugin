# PluginManager

- Authors: 少司命,Cai
- 出处: 本仓库
- Use the command to automatically update the server’s plugins (only from this repository).
- 提供了热重载插件的可能，但此热重载并不算是真正意义上的热重载，已加载的程序集无法卸载，而是依然存在于应用域
  中，只是不再生效!

## Commands

| 语法                                                                          |    Permission    |                                                                         说明                                                                         |
| --------------------------------------------------------------------------- | :--------------: | :------------------------------------------------------------------------------------------------------------------------------------------------: |
| /apm -c                                                                     | AutoUpdatePlugin |                                                              Check for plugin updates                                                              |
| /apm -u [plugin name]   | AutoUpdatePlugin |               One-click upgrade plugins, requires server restart. Multiple plugin names can be separated by `commas`               |
| /apm -l                                                                     | AutoUpdatePlugin |                                                         View the list of repository plugins                                                        |
| /apm -i [plugin number] | AutoUpdatePlugin | Install plugins, requires server restart. Multiple plugin numbers can be separated by `commas` and used with the `/apm -i` command |
| /apm -b [plugin name]   | AutoUpdatePlugin |                                                             Exclude plugin from updates                                                            |
| /apm -r                                                                     | AutoUpdatePlugin |                                                        Check for duplicate installed plugins                                                       |
| /apm -rb [plugin name]  | AutoUpdatePlugin |                                                               Remove update exclusion                                                              |
| /apm -lb                                                                    | AutoUpdatePlugin |                                                         List plugins excluded from updates                                                         |
| /apm -ib                                                                    | AutoUpdatePlugin |                                                                   列出已安装插件列表与启用状态                                                                   |
| /apm -on [序号]           | AutoUpdatePlugin |                                                                       启用某个插件                                                                       |
| /apm -off [序号]          | AutoUpdatePlugin |                                                                       关闭某个插件                                                                       |

## Config

> Configuration file location：tshock/AutoPluginManager.json

```json5
{
  "允许自动更新插件": false, // Allow automatic plugin updates
  "使用Github源": true, // Use GitHub source
  "使用自定义源": false, // Use custom source
  "自定义源清单地址": "", // Custom source plugin manifest url
  "自定义源压缩文件地址": "", // Custom source plugin archive url
  "插件排除列表": [], // Plugin exclusion list
  "热重载升级插件": true, // Hot reload upgrade plugin
  "热重载出错时继续": true // continue when hot reload error occurred
}
```

## 更新日志

```
v2.0.3.1
更新英文翻译
v2.0.2.9
添加指令
/apm il 查看本地插件列表
/apm on 启用某个插件
/apm off 关闭某个插件
v2.0.2.8
调整apm指令管理
v2.0.2.7
完全适配自建API
v2.0.2.6
使用流缓存插件包,抛弃Gitee，自建API
v2.0.2.4
回滚到 v2.0.2.2
v2.0.2.3
添加 LazyAPI 依赖
配置文件本地化
v2.0.2.2
默认启用热重载
新增配置项热重载出错时跳过报错插件
新增`HotReload`字段以跳过指定插件热重载
细化一些提示
v2.0.2.1
修复在未开启热重载时，插件更新后版本号未更新
修复英文翻译错误
v2.0.2.0
重构部分逻辑
实现自动拉取依赖
实现自定义源
v2.0.1.7
热重载升级插件
v2.0.1.4
添加英文翻译
v2.0.1.3
添加配置项可切换到Github源
v2.0.1.2
将源更换为Gitee
v2.0.1.1
更新: apm u支持排除插件,支持自动更新插件,apm l优化显示 & 修复: 插件更新回旧版本,更新插件后不重启仍提示更新
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

## FeedBack

- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
