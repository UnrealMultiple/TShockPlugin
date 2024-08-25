# PluginManager 自动更新插件

- 作者: 少司命，Cai
- 出处: 本仓库
- 使用指令自动更新服务器的插件(仅本仓库)

## 更新日志

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

## 指令

| 语法           |        权限         |   说明   |
| -------------- | :-----------------: | :------: |
| /apm -c | AutoUpdatePlugin   | 检查插件更新|
| /apm -u [插件名] | AutoUpdatePlugin   | 一键升级插件，需要重启服务器，插件名可多选`英文逗号隔开`|
| /apm -l | AutoUpdatePlugin   | 查看仓库插件列表 |
| /apm -i [插件序号] | AutoUpdatePlugin   | 安装插件，需重启服务器，插件序号多选`英文逗号隔开`配合`/apm -i`指令使用 |
## 配置

```json
暂无
```
## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/Controllerdestiny/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
