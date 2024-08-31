# DeathDrop 死亡掉落

- 作者: 大豆子，肝帝熙恩更新优化
- 出处: TShock中文官方群
- 允许自定义怪物死亡时的掉落物。
- 随机or自定义，互不影响

## 更新日志

```
- v1.0.3 使用reload来重载配置，原先是杀一只怪就重载一次，感觉会爆
```

## 指令

```
暂无
```

## 配置
> 配置文件位置：tshock/死亡掉落配置表.json
```json
{
  "是否开启随机掉落": false,//随机掉落的总开关，必须设置这个为true能设置除了自定义以外的内容
  "完全随机掉落": false,//完全随机掉落，从1-5452里面选一个或多个物品
  "完全随机掉落排除物品ID": [],//不会选择这里面的物品
  "普通随机掉落物": [],//如果完全随机掉落为false，你可以在这里面自定义所有怪物一起的随机掉落物，随机掉落物从这里面选取
  "随机掉落概率": 100,//概率，同时影响完全随机掉落和普通随机掉落
  "随机掉落数量最小值": 1,//随机掉落数量最小值，同时影响完全随机掉落和普通随机掉落
  "随机掉落数量最大值": 1,//随机掉落数量最小值，同时影响完全随机掉落和普通随机掉落
  "是否开启自定义掉落": false,//自定义掉落，不受上面所有设置的影响，独立作用
  "自定义掉落设置": [
    {
      "生物id": 0,
      "完全随机掉落": false,
      "完全随机掉落排除物品ID": [],
      "普通随机掉落物": [],
      "随机掉落数量最小值": 1,
      "随机掉落数量最大值": 1,
      "掉落概率": 100
    }
  ]
}
```
## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love