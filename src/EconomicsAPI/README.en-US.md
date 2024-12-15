以下是README.md文件的英文翻译，以及对应的Markdown原始文本：
# EconomicsAPI Plugin [Economic Suite Prequel]

- Author: Shao Siming
- Source: None
- Economic Suite Prequel Plugin

## Update Log

```
V1.0.2.0 Added /bank query command to replace /query BREAKING CHANGE: CurrencyManager.DelUserCurrency renamed to DelUserCurrency.DeductUserCurrency
V1.0.0.0
Added extension functions
Added display message API
Added gradient message API
Custom gradient message colors
Fixed currency drop on death
Fixed inaccurate player damage calculation
```

## Commands

| Syntax                           |           Permission           |               Description               |
| ------------------------------ | :----------------------: | :------------------------------: |
| /bank add <player name> <amount> |      economics.bank      |             Increase currency            |
| /bank deduct <player name> <amount> |      economics.bank      |             Deduct currency            |
| /bank pay <player name> <amount> |    economics.bank.pay    |             Transfer currency           |
| /bank query [player name]        |   economics.bank.query   |             Query currency             |
| /bank clear <player name>        |      economics.bank      |             Clear currency            |
| /bank reset                     |      economics.bank      |            Global currency reset         |
| /query                          | economics.currency.query | Query currency (Deprecated, will be removed in future versions) |

## Configuration
> Configuration file location: tshock/Economics/Economics.json
```json
{
  "Currency Name": "Soul Power",
  "Currency Exchange Rate": 1.0,
  "Save Time Interval": 30,
  "Display Profits": true,
  "Disable Statues": false,
  "Death Drop Rate": 0.0,
  "Display Information": true,
  "Display Information Left Shift": 60,
  "Display Information Down Shift": 0,
  "Query Prompt": "[c/FFA500:You currently have {0}{1}]",
  "Gradient Colors": [
    "[c/00ffbf:{0}]",
    "[c/1aecb8:{0}]",
    "[c/33d9b1:{0}]",
    "[c/80a09c:{0}]",
    "[c/998c95:{0}]",
    "[c/b3798e:{0}]",
    "[c/cc6687:{0}]",
    "[c/e65380:{0}]",
    "[c/ff4079:{0}]",
    "[c/ed4086:{0}]",
    "[c/db4094:{0}]",
    "[c/9440c9:{0}]",
    "[c/8240d7:{0}]",
    "[c/7040e4:{0}]",
    "[c/5e40f2:{0}]",
    "[c/944eaa:{0}]",
    "[c/b75680:{0}]",
    "[c/db5d55:{0}]",
    "[c/ed6040:{0}]",
    "[c/ff642b:{0}]"
  ]
}
```
## Feedback
Priority: Issued -> Shared Plugin Repository: https://github.com/UnrealMultiple/TShockPlugin
Secondary Priority: TShock Official Group: 816771079
Likely not seen but also possible: Domestic community trhub.cn, bbstr.net, tr.monika.love
