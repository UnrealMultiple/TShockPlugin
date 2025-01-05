# Economics.NPC 插件 自定义怪物奖励

- 作者: 少司命
- 出处: 无
- 配置 NPC 专属奖励

> [!NOTE]\
> 需要安装前置插件：EconomicsAPI(本仓库)

## 指令

无

## 配置

> 配置文件位置：tshock/Economics/NPC.json

```json5
{
  "开启提示": true,
  "提示内容": "你因击杀{0},获得额外奖励{1}{2}个",
  "额外奖励列表": [
    {
      "怪物ID": 390,
      "怪物名称": "猪鲨",
      "奖励货币": 100000,
      "按输出瓜分": true // false 时每个人发10000奖励
    }
  ],
  "转换率更改": {
    "50": 1.3 //id 和 转换率
  }
}
```

## 更新日志

```
V2.0.0.0
适配多货币

```

## 反馈

- 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 国内社区 trhub.cn 或 TShock 官方群等
