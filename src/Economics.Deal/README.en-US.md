# Economics.Deal

- Author: Shaosimin
- Origin: None
- Deal plugin

> [!NOTE]
> needs to be installed before the plugin: EconomicsAPI (this repository)

## Commands

| Syntax                                                                       |                      Permission                      |        Description        |
| ---------------------------------------------------------------------------- | :--------------------------------------------------: | :-----------------------: |
| /deal push [price]       |  economics.deal.use  |   Publish Handheld Items  |
| /deal list [page number] |  economics.deal.use  | View list of traded items |
| /deal buy [ID]           |  economics.deal.use  |      Buy trade items      |
| /deal recall [ID]        |  economics.deal.use  |   Withdraw a posted item  |
| /deal reset                                                                  | economics.deal.reset |         reset deal        |

## Configuration

> configuration file location: tshock/Economics/Deal.json

```json
{
  "最大显示页": 10,
  "交易列表": []
}
```

## 更新日志

```
V2.0.0.0
适配多货币

V1.0.0.3
适配新 EconomicsAPI

V1.0.0.2
支持多语言
```

## FeedBack

- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
