# Economics.Deal 交易插件

- 作者: 少司命
- 出处: 无
- 交易物品插件

> [!NOTE]  
> 需要安装前置插件：EconomicsAPI (本仓库)  

## 指令

| 语法                |          权限          |    说明    |
|-------------------|:--------------------:|:--------:|
| /deal push [价格]   |  economics.deal.use  |  发布手持物品  |
| /deal list [页码]   |  economics.deal.use  | 查看交易物品列表 |
| /deal buy [ID]    |  economics.deal.use  |  购买交易物品  |
| /deal recall [ID] |  economics.deal.use  |  撤回发布物品  |
| /deal reset       | economics.deal.reset |   重置交易   |

## 配置
> 配置文件位置：tshock/Economics/Deal.json
```json
{
  "最大显示页": 10,
  "交易列表": []
}
```

## 更新日志

### v2.0.0.0
- 适配多货币

### v1.0.0.3
- 适配新 EconomicsAPI

### v1.0.0.2
- 支持多语言

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
