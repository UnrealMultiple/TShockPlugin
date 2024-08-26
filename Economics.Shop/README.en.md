# Economics.Shop store plug-in

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: Shao Siming
- Source: None
- Store plug-in

> [! NOTE]
> Pre-plug-ins need to be installed: EconomicsAPI, Economics.RPG (this warehouse).

## Update log

```
暂无
```

## instruction

|grammar|limit of authority|explain|
| ----------------- |:------------:|:----------:|
|/shop buy [serial number]|economics.shop|Buy goods|
|/shop list [page number]|economics.shop|View the list of goods|

## deploy
> Configuration file location: tshock/Economics/Shop.json
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
## feedback
- Give priority to issued-> jointly maintained plug-in library: https://github.com/Controllerdestiny/TShockPlugin.
- Second priority: TShock official group: 816771079
- You can't see it with a high probability, but you can: domestic communities trhub.cn, bbstr.net, tr. monika.love.