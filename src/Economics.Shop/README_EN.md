# Economics.Shop Shop Plugin

- Author: Shao Shi Ming
- Source: None
- Store plugin

> [!NOTE]
>  requires pre-installed plugins: EconomicsAPI, Economics.RPG (this repository)

##  changelog

```
V1.0.0.3
适配新 EconomicsAPI
```

##  directive

| Syntax | Privileges | Description |
| ----------------- | :------------: | :----------: |
| /shop buy [serial number] | economics.shop | buy item |
| /shop list [page number] | economics.shop | View Product List |

##  configuration
>  configuration file location: tshock/Economics/Shop.json
```json
{
  "最大显示页": 10,
  "商品列表": [
    {
      "商品名称": "test",
      "商品价格": 1,
      "等级限制": [],
      "进度限制": [],
      "执行命令": [],
      "商品内容": [
        {
          "物品ID": 1327,
          "数量": 0,
          "前缀": 0
        }
      ]
    }
  ]
}
```
Feedback for ## 
- Priority sends ISSUED -> co-maintained plugin repository: https://github.com/UnrealMultiple/TShockPlugin
- Sub-priority: TShock official group: 816771079
- Most likely not visible but possible: domestic community trhub.cn , bbstr.net , tr.monika.love