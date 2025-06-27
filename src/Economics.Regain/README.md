# Economics.Regain 回收物品插件

- 作者: 少司命
- 出处: 无
- 可自定义回收物品插件

> [!NOTE]  
> 需要安装前置插件：EconomicsAPI (本仓库)  

## 指令

| 语法                |        权限        |   说明    |
|-------------------|:----------------:|:-------:|
| /regain [数量]      | economics.regain | 兑换手持物品  |
| /regain list [页码] | economics.regain | 查看可兑换物品 |
| /regain           | economics.regain | 兑换手持物品  |

## 配置
> 配置文件位置：tshock/Economics/Regain.json
```json
{
  "回收物品列表": [
    {
      "物品ID": 2990,
      "回收价格": [
        {
          "数量": 25,
          "货币类型": "升级书"
        }
      ]
    },
    {
      "物品ID": 5341,
      "回收价格": [
        {
          "数量": 250,
          "货币类型": "灵魂"
        }
      ]
    },
    {
      "物品ID": 2991,
      "回收价格": [
        {
          "数量": 1,
          "货币类型": "魂力"
        }
      ]
    }
  ]
}
```
## 更新日志

### v2.0.0.0
- 适配多货币

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
