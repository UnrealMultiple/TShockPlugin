# ShortCommand short instructions

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: GK, Shaosh Life, Yu Xue
- Source: QQ group (232109072)
- This is a TSHOCK server plug -in mainly used to map the plug -in or native TSHOCK command into a command you want
- And can completely disable a command through the [prevent primitive] in the configuration file, add RELOAD heavy load configuration file method
- And equipped with a permissions [Exemption instruction] to prevent the specific user group from being influenced by [Prevent Original]

## Update log

```
v1.3.0.1
完善卸载函数

v1.3.0
修复了Dispose方法中关服引发的超堆栈问题
加了个免疫阻止原始的权限名
```

## instruction

|grammar|Authority|illustrate|
| -------------- |: --------------------:|: -------:|
|none|Free instruction|The plug -in is not prohibited from using the original command|

## Configuration
> Configuration file location: TSHOCK/Short instruction.json
```json
{
   "命令表": [
    {
       "原始命令": "执行",
       "新的命令": "重置玩家数据",
       "余段补充": true,
       "阻止原始": true,
       "限制条件": 0,
       "冷却秒数": 0,
       "冷却共享": false
    },
    {
       "原始命令": "pskill list",
       "新的命令": "被动书店",
       "余段补充": true,
       "阻止原始": false,
       "限制条件": 0,
       "冷却秒数": 0,
       "冷却共享": false
    }
  ]
}
```
## feedback
- Commonly maintained plug -in library: https://github.com/thexn/tshockplugin/
- The official group of Trhub.cn or Tshock in the domestic community