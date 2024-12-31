# Economics.Shop Shop Plugin

- Author: Shao Shi Ming
- Source: None
- Store plugin

> [!NOTE]
>  requires pre-installed plugins: EconomicsAPI, Economics.RPG (this repository)

## Commands

| Syntax                    |   Permission   |    Description    |
|---------------------------|:--------------:|:-----------------:|
| /shop buy [serial number] | economics.shop |     buy item      |
| /shop list [page number]  | economics.shop | View Product List |

## Configuration
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
## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love