# Economics.Shop 商店插件

- 作者: 少司命
- 出处: 无
- 商店插件

> [!NOTE]  
> 需要安装前置插件：EconomicsAPI、Economics.RPG (本仓库) 

## 更新日志

```
暂无
```

## 指令

| 语法              |      权限      |     说明     |
| ----------------- | :------------: | :----------: |
| /shop buy [序号]  | economics.shop |   购买商品   |
| /shop list [页码] | economics.shop | 查看商品列表 |

## 配置
> 配置文件位置：tshock/Economics/Shop.json
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
## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/Controllerdestiny/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
