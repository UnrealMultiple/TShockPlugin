# Economics.Deal deals plugin

- Author: Shaosimin
- Origin: None
- Deal plugin

> [!NOTE]
>  needs to be installed before the plugin: EconomicsAPI (this repository)

##  changelog

```
V1.0.0.3
适配新 EconomicsAPI

V1.0.0.2
支持多语言
```

##  directive

| Syntax | Privileges | Description |
| ----------------- | :------------------: | :--------------: |
| /deal push [price] | economics.deal.use | Publish Handheld Items |
| /deal list [page number] | economics.deal.use | View list of traded items |
| /deal buy [ID] | economics.deal.use | Buy trade items |
| /deal recall [ID] | economics.deal.use | Withdraw a posted item |
| /deal reset | economics.deal.reset | reset deal |

##  configuration
>  configuration file location: tshock/Economics/Deal.json
```json
{
  "最大显示页": 10,
  "交易列表": []
}
```
Feedback from ## 
- Priority sends ISSUED -> co-maintained plugin repository: https://github.com/UnrealMultiple/TShockPlugin
- Sub-priority: TShock official group: 816771079
- Most likely not visible but possible: domestic community trhub.cn , bbstr.net , tr.monika.love











