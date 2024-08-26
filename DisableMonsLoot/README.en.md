# DisablemonsLoot Monster drop

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: Yu Xue
- - 出处: [github](https://gitee.com/Crafty/bean-points) 
- Magic reform <Monster does not drop money plug -in> The original intention of development is to cooperate with random drops to open up wasteland
- This plug -in cleans up the current progress through the monster death incident,
- If you defeat [Monster ID], any monster will automatically close [Cleaning function]
- If [Whether to kill] is turned on, you need to defeat [Monster ID] all monsters will automatically close
- Enter/kdm reset to turn on all [cleaning up] switches to restore the server

## Update log
```
v1.3.1
补全卸载函数

v1.3.0
加入指令功能方便重置、控制配置项
默认配置调整清理范围35
克脑 世吞 骷髅王 加入：
魔矿/暗影鳞片/猩红矿/组织样本/骨头/金钥匙

v1.2.0
预设一堆进度物品ID
将怪物ID也改为数组，支持更多ID
加入【是否全杀】配置项
怪物名支持多ID转换中文名
为贴合自动转换中文名避免空引用报错，
在空处输入{},然后reload会预设个示例格式如：

    {
     怪物名 ":蓝史莱姆",
     清理": false,
     怪物ID": [1],
     物品ID": []
    },

v1.1.0
魔改怪物不掉钱插件
掉落物可用数组控制，击败指定怪物ID，自动关闭清理功能
使用/reload 可取怪物ID赋值给怪物名，自动转换中文名。
```
## instruction

|grammar|Authority|illustrate|
| -------------- |: --------------------:|: -------:|
|/kdm|killitem.admin|Check the ban instruction menu|

## Configuration
> Configuration file location: TSHOCK/Forbidden Monster drop .json
```json
{
   "使用说明": "你只管改‘ID’就行，‘清理’和‘怪物名’不用动，击杀其中1个怪物ID自动关闭清理功能。指令：/kdm (权限killitem.admin)",
   "插件开关": true,
   "是否全杀": false,
   "清理范围": 35,
   "清理列表": [
    {
       "怪物名": "蓝史莱姆, 恶魔眼, 僵尸",
       "清理": true,
       "怪物ID": [
        1,
        2,
        3
      ],
       "物品ID": [
        71,
        72,
        73,
        74
      ]
    }
  ]
}
```
## feedback
- Priority to ISSUED-> Commonly maintained plug -in library: https://github.com/Controllerdestiny/tshockPlugin
- Two priority: TSHOCK official group: 816771079
- It may not be seen in a high probability, but it can also be: domestic community trhub.cn, bbstr.net, tr.monika.love