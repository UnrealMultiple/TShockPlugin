# EconomicsAPI 插件[经济套件前置]

- 作者: 少司命
- 出处: 无
- 经济套件前置插件

## 指令

| 语法                                                                                                       |                            权限                            |                   说明                  |
| -------------------------------------------------------------------------------------------------------- | :------------------------------------------------------: | :-----------------------------------: |
| /bank add <玩家名称> <数量> <货币>    |              economics.bank              |                  增加货币                 |
| /bank deduct <玩家名称> <数量> <货币> |              economics.bank              |                  扣除货币                 |
| /bank pay <玩家名称> <数量> <货币>    |    economics.bank.pay    |                  转账货币                 |
| /bank query [玩家名称]                                   |   economics.bank.query   |                  查询货币                 |
| /bank clear <玩家名称>                                                              |              economics.bank              |                  清除货币                 |
| /bank reset                                                                                              |              economics.bank              |                 全局重置货币                |
| /bank cash <原货币> <数量> <目标货币>  |    economics.bank.cash   |                  转换货币                 |
| /查询                                                                                                      | economics.currency.query | 查询货币(废弃, 将在未来版本移除) |

## 配置

> 配置文件位置：tshock/Economics/Economics.json

```json5
{
  "货币名称": "魂力",
  "货币转换率": 1.0,
  "保存时间间隔": 30,
  "显示收益": true,
  "禁用雕像": false,
  "死亡掉落率": 0.0,
  "显示信息": true,
  "显示信息左移": 60,
  "显示信息下移": 0,
  "查询提示": "[c/FFA500:你当前拥有{0}{1}个]",
  "渐变颜色": [
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
