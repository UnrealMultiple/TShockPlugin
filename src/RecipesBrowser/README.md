# RecipesBrowser 合成表

- 作者: 棱镜、羽学、Cai
- 出处: [github](https://github.com/1242509682/RecipesBrowser)
- 由于PE的Terraria的向导存在一些恶性bug (远古时期)  
- 导致在大多数服务器中向导被禁用，这样一来想要查合成表就非常麻烦，  
- 所以写了这样一个插件，支持查找“此物品的配方”和“此物品可以合成什么”

## 指令

| 语法        |       权限       |            说明            |
|-----------|:--------------:|:------------------------:|
| /fd、/find | RecipesBrowser | /fd <物品ID> 查询合成所需材料与工作站  |
| /查        | RecipesBrowser | /fd <物品ID> r 查询该材料可合成的物品 |

## 配置

```json
暂无
```

## 更新日志

### v1.1.1
- 修复无参数报错问题
- 优化代码
### v1.0.6
- 完善卸载函数
### v0.5
- 修复未释放钩子导致关闭服务器时的报错
### v0.4
- 适配.NET 6.0
- 添加中文命令
- 添加一个权限名

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love