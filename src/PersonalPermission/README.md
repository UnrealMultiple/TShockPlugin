# PersonalPermission 玩家单独权限

- 作者: Megghy，肝帝熙恩更新1449
- 出处: [github](https://github.com/Megghy/PersonalPermission)
- 以给玩家给予一个单独的权限而不是添加给所在用户组
- 对于开发者:可使用`<玩家对象(TSPlayer)>.GetData<Group>("PersonalPermission");`来获取一个包含独立权限的用户组对象, 同时直接调用`.HasPermission`也会自动判断权限.

## 指令

| 语法  |            权限            |      说明      |
|-----|:------------------------:|:------------:|
| /pp | personalpermission.admin | 为玩家添/删加权限等指令 |

## 配置

```
暂无
```

## 更新日志

### v1.1.0.1
- 完善卸载函数

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love