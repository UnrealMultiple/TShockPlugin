# EconomicsAPI 插件[经济套件前置]

- 作者: 少司命
- 出处: 无
- EconomicsAPI是延续POBC设计思路的新经济系统，他本身并无特别的功能，只提供货币系统，以及一些API。

## 指令

| 语法                            |            权限            |         说明         |
|-------------------------------|:------------------------:|:------------------:|
| /bank add <玩家名称> <数量> <货币>    |      economics.bank      |        增加货币        |
| /bank deduct <玩家名称> <数量> <货币> |      economics.bank      |        扣除货币        |
| /bank pay <玩家名称> <数量> <货币>    |    economics.bank.pay    |        转账货币        |
| /bank query [玩家名称]            |   economics.bank.query   |        查询货币        |
| /bank clear <玩家名称>            |      economics.bank      |        清除货币        |
| /bank reset                   |      economics.bank      |       全局重置货币       |
| /bank cash <原货币> <数量> <目标货币>  |   economics.bank.cash    |        转换货币        |
| /查询                           | economics.currency.query | 查询货币(废弃, 将在未来版本移除) |

## 配置
> 配置文件位置：tshock/Economics/Economics.json
```json5
{
  "保存时间间隔": 30,
  "显示收益": true,
  "禁用雕像": false,
  "显示信息": true,
  "显示信息左移": 60,
  "显示信息下移": 0,
  "渐变颜色": [
    "[c/00ffbf:{0}]",
    "[c/1aecb8:{0}]",
    "[c/33d9b1:{0}]",
    "[c/A6D5EA:{0}]",
    "[c/A6BBEA:{0}]",
    "[c/B7A6EA:{0}]",
    "[c/A6EAB3:{0}]",
    "[c/D5F0AA:{0}]",
    "[c/F5F7AF:{0}]",
    "[c/F8ECB0:{0}]",
    "[c/F8DEB0:{0}]",
    "[c/F8D0B0:{0}]",
    "[c/F8B6B0:{0}]",
    "[c/EFA9C6:{0}]",
    "[c/00ffbf:{0}]",
    "[c/1aecb8:{0}]"
  ],
  "货币配置": [
    {
      "查询提示": "[c/FFA500: 当前拥有{0}{1}个]",
      "货币名称": "魂力",
      "兑换关系": [   //使用/bank cash 指令兑换货币
       {
          "数量": 0, //兑换此货币需要目标货币数量
          "货币类型": "魂力" //目标货币
        }
      ],
      "获取关系": {
        "获取方式": 0, //0 无法获取 1击杀怪物获取
        "给予数量": 0,
        "比例": 1.0
      },
      "死亡掉落": {
        "启用": false,
        "掉落比例": 0.1
      },
      "悬浮文本": {
        "启用": false,
        "提示文本": "+{0}$",
        "Color": [
          255,
          255,
          255
        ]
      }
    }
  ]
}
```

## 更新日志

```
V2.0.0.0
新增多货币实现

V1.0.2.0
新增 /bank query 指令以替代 /查询
BREAKING CHANGE: CurrencyManager.DelUserCurrency 重命名为 DelUserCurrency.DeductUserCurrency

V1.0.0.0
- 添加扩展函数
- 添加显示消息API
- 添加渐变色消息API
- 自定义渐变色消息颜色
- 修复死亡掉落货币
- 修复玩家伤害计算失准
```

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
