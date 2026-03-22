# ItemPreserver 物品不消耗

- 作者: 肝帝熙恩，少司命
- 出处: [github](https://github.com/THEXN/ItemPreserver)
- 可在配置文件添加自定义物品，还可以配置需要物品达到什么值时才会使该物品使用不消耗
- 通过快捷键使用的物品可能会消耗，比如快捷喝药


## 指令

```
暂无
```

## 配置
> 配置文件位置：tshock/ItemPreserverConfig.json
```json5
{
  "不消耗物品": {
	"25": -1  //"物品ID"： 达成物品不消耗所需要的物品数量，-1代表不需要达成数量
    "1145": 30
  }
}
```

## 更新日志

### v1.0.7
- 完善卸载函数
### v1.0.6
- 添加数量条件

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
