# Economics.NPC 插件 自定义怪物奖励

- 作者: 少司命，千亦
- 出处: 无
- 配置 NPC 专属奖励

> [!NOTE]  
> 需要安装前置插件：EconomicsAPI(本仓库) 

## 指令

无

## 进度限制
Economics文档：[进度限制值](../Economics.Core/README.md)

## 配置
> 配置文件位置：tshock/Economics/NPC.json
```json5
{
  "开启提示": true,
  "提示内容": "你因击杀{0},获得额外奖励{1}{2}个",
  "额外奖励列表": [
    {
      "怪物ID": 4,
      "怪物名称": "克苏鲁之眼",
      "奖励货币": [
        {
          "数量": 250000,
          "货币类型": "战利品"
        }
      ],
      "按输出瓜分": true  //为true时安玩家输出分配货币
    }
  ],
  "转换率更改": {
    "4": { //怪物ID
      "转换率": 1.5, //Core 默认 KillNpc 奖励的加成系数，1.5 = 默认奖励的 150%
      "进度条件": [
        "克脑",
        "世吞"
      ]
    }
  }
}
```
## 更新日志

### v2.1.0.0
- 修复"转换率更改"的字段语义：改为在 Core 默认奖励上按 AllocationRatio 加成（damage × ConversionRate × AllocationRatio），此前实现将其误用为奖励池总量导致只能拿到 1-5 块
- 修复同时配置"转换率更改"与"额外奖励列表"时，额外奖励提示不触发的问题（转换率分支末尾不再 return，两段配置可共存）
- 修复转换率分支无视 `CombatMsgOption.Enable` 与 `ContainsID` 过滤的问题，行为与 Core 默认结算保持一致
- 转换率分支在进度不达标时改为 fallback 到 Core 默认结算，而不是吞掉所有奖励

### v2.0.0.0
- 适配多货币

## 反馈

- 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 国内社区 trhub.cn 或 TShock 官方群等
