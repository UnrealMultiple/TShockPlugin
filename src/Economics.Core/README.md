# Economics.Core 插件[经济套件核心]

- 作者: 少司命，千亦(修复 bug)
- 出处: 无
- EconomicsAPI是延续POBC设计思路的新经济系统，他本身并无特别的功
能，只提供货币系统，以及一些API。

## 进度限制
- 进度限制值在Economics系插件通用
- 进度判断支持取反，例如`["!骷髅王"]`表示在未击杀骷髅王 (`!`是英文感叹号)

<Details>

<Summary>进度值</Summary>

| 主名称    | 别名                   |
|--------|----------------------|
| 无限制    | -                    |
| 克眼     | 克苏鲁之眼                |
| 史莱姆王   | 史莱姆之王、史王             |
| 克脑     | 克苏鲁之脑、世界吞噬者、世界吞噬怪、世吞 |
| 骷髅王    | -                    |
| 蜂王     | -                    |
| 鹿角怪    | 独眼巨鹿                 |
| 血肉墙    | 肉山、肉后、困难模式           |
| 一王后    | -                    |
| 双子魔眼   | -                    |
| 毁灭者    | 铁长直                  |
| 机械骷髅王  | -                    |
| 世纪之花   | 世花                   |
| 石巨人    | -                    |
| 猪鲨     | 猪龙鱼公爵                |
| 拜月教邪教徒 | 拜月                   |
| 月球领主   | 月亮领主、月总              |
| 光之女皇   | 光女                   |
| 史莱姆皇后  | 史后                   |
| 哀木     | -                    |
| 南瓜王    | -                    |
| 长绿尖叫怪  | -                    |
| 冰雪女皇   | -                    |
| 圣诞坦克   | -                    |
| 火星飞碟   | 飞碟                   |
| 小丑     | -                    |
| 日耀柱    | -                    |
| 星旋柱    | -                    |
| 星云柱    | -                    |
| 星尘柱    | -                    |
| 哥布林入侵  | 哥布林                  |
| 海盗入侵   | 海盗                   |
| 霜月     | -                    |
| 血月     | -                    |
| 旧日一    | 黑暗法师                 |
| 旧日二    | 巨魔                   |
| 旧日三    | 贝蒂斯、双足翼龙             |
| 雨天     | -                    |
| 白天     | -                    |
| 夜晚     | -                    |
| 大风天    | -                    |
| 万圣节    | -                    |
| 派对     | -                    |
| 2020   | 醉酒世界                 |
| 2021   | 十周年                  |
| ftw    | For The Worthy       |
| 混乱世界   | 颠倒世界                 |
| ntb    | 蜂蜜世界                 |
| 永恒领域   | 饥荒                   |
| 天顶世界   | 天顶                   |
| 危机世界   | -                    |
| 森林     | -                    |
| 丛林     | -                    |
| 沙漠     | -                    |
| 雪原     | -                    |
| 洞穴     | -                    |
| 海洋     | -                    |
| 神圣     | -                    |
| 蘑菇     | -                    |
| 微光     | -                    |
| 腐化之地   | 腐化                   |
| 猩红     | 猩红之地                 |
| 地牢     | -                    |
| 墓地     | -                    |
| 神庙     | -                    |
| 蜂巢     | -                    |
| 沙尘暴    | -                    |
| 天空     | -                    |
| 岩层     | -                    |
| 土层     | -                    |
| 地狱     | -                    |
| 地下沙漠   | -                    |
| 满月     | -                    |
| 亏凸月    | -                    |
| 下弦月    | -                    |
| 残月     | -                    |
| 新月     | -                    |
| 娥眉月    | -                    |
| 上弦月    | -                    |
| 盈凸月    | -                    |
</Details>

## 指令

| 语法                                       |            权限            |         说明         |
|------------------------------------------|:------------------------:|:------------------:|
| /bank add <玩家名称> <数量> <货币>             |      economics.bank      |        增加货币        |
| /bank deduct <玩家名称> <数量> <货币>          |      economics.bank      |        扣除货币        |
| /bank pay <玩家名称> <数量> <货币>             |    economics.bank.pay    |        转账货币        |
| /bank query [玩家名称]                       |   economics.bank.query   |        查询货币        |
| /bank clear <玩家名称>                       |      economics.bank      |        清除货币        |
| /bank reset                              |      economics.bank      |       全局重置货币       |
| /bank exchange <源货币> <目标货币> <数量>      |   economics.bank.cash    |        兑换货币        |
| /bank preview <源货币> <目标货币> <数量>       |   economics.bank.cash    |       预览兑换结果       |
| /bank rates [货币]                         |   economics.bank.query   |       查看汇率关系       |
| /bank cycles                             |   economics.bank.admin   |      查看兑换循环检测      |
| /bank lb <货币> <数量>                       |   economics.bank.query   |      查看货币排行榜       |
| /查询                                      | economics.currency.query | 查询货币(废弃, 将在未来版本移除) |

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
      "货币名称": "金币",
      "货币描述": "基础货币，通过击杀怪物获得",
      // 可兑换为: 配置此货币可以兑换成哪些其他货币
      // 例如: "银币": 100 表示 1金币 = 100银币
      "可兑换为": {
        "银币": 100,
        "铜币": 10000
      },
      "获取关系": {
        "获取方式": 1, // 0: 无法获取 1: 击杀怪物获取 2: 挖掘图格获取
        "给予数量": 0,
        "比例": 0.5,  // 每点伤害获得0.5个货币
        "指定ID": []  // 指定的NPC或图格ID，空数组表示所有
      },
      "死亡掉落": {
        "启用": false,
        "掉落比例": 0.1
      },
      "悬浮文本": {
        "启用": true,
        "提示文本": "+{0}$",
        "Color": [255, 255, 255]
      }
    },
    {
      "查询提示": "[c/C0C0C0: 银币: {1}]",
      "货币名称": "银币",
      "货币描述": "次级货币，可由金币兑换",
      "可兑换为": {
        "铜币": 100
      },
      "获取关系": {
        "获取方式": 0  // 无法通过游戏行为获取，只能兑换
      }
    },
    {
      "查询提示": "[c/B87333: 铜币: {1}]",
      "货币名称": "铜币",
      "货币描述": "最低级货币，无法继续兑换",
      "可兑换为": {},  // 空对象表示无法兑换为其他货币
      "获取关系": {
        "获取方式": 0
      }
    }
  ]
}
```

## 其他插件API调用示例

```csharp
// 查询余额
var balanceResult = Core.Economics.CurrencyService.GetBalance("玩家名", "金币");
if (balanceResult.IsSuccess)
{
    long balance = balanceResult.Value;
}

// 增加货币
Core.Economics.CurrencyService.AddCurrency("玩家名", "金币", 100);

// 扣除货币（带结果检查）
var result = Core.Economics.CurrencyService.DeductCurrency("玩家名", "金币", 50);
if (result.IsSuccess) { /* 成功 */ }

// 兑换货币
var exchangeResult = Core.Economics.ExchangeService.ExecuteExchange(
    "玩家名", "金币", "钻石", 10);

// 重置所有货币
var resetResult = Core.Economics.CurrencyService.ResetAllCurrencies();
```

## 更新日志

### v3.0.0.0
- **重大架构重构**: 重新设计货币系统，提供更清晰的 API 接口
- **新增货币兑换系统**: 支持任意货币之间的兑换，自动检测并阻止套利循环
- **新增服务层**: 引入 `ICurrencyService` 和 `IExchangeService` 接口，便于其他插件调用
- **新增兑换命令**:
  - `/bank exchange <源货币> <目标货币> <数量>` - 执行货币兑换
  - `/bank preview <源货币> <目标货币> <数量>` - 预览兑换结果
  - `/bank rates [货币]` - 查看汇率关系
  - `/bank cycles` - 查看检测到的兑换循环
- **移除废弃 API**: `CurrencyManager` 不再对外暴露，请使用 `Economics.CurrencyService`
- **配置变更**: `CustomizeCurrencys` 更名为 `Currencies`，配置结构更加清晰
- **代码优化**: 精简 CurrencyManager，统一使用 PlayerCurrencyInfo 模型

### v2.1.0.0
- 修复按输出瓜分的货币奖励在击杀 Boss 时拿不到满额的问题：此前受最后一击伤害丢失、武器伤害与实际扣血不一致、以及中毒 / 灼烧等持续伤害未被统计等因素影响，实际到账常少于配置值；现在 solo 击杀能稳定拿到完整奖励，多人击杀按各自对 Boss 造成的伤害占比瓜分

### v2.0.0.13
- 修复击杀低血怪时按武器原始伤害结算货币的问题，现在溢出伤害会被裁剪到 NPC 实际剩余血量

### v2.0.0.12
- 修复/bank lb无法显示

### v2.0.0.11
- 修复/bank query指定玩家无输出问题

### v2.0.0.10
- 修复CombatMsg无法正常显示击杀后获得货币数的问题

### v2.0.0.8
- 更名为Economics.Core
- 更新命令系统
- 更新配置文件系统

### v2.0.0.0
- 新增多货币实现

### v1.0.2.0
- 新增 /bank query 指令以替代 /查询
- BREAKING CHANGE: CurrencyManager.DelUserCurrency 重命名为 DelUserCurrency.DeductUserCurrency

### v1.0.0.0
- 添加扩展函数
- 添加显示消息API
- 添加渐变色消息API
- 自定义渐变色消息颜色
- 修复死亡掉落货币
- 修复玩家伤害计算失准

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
