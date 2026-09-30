# TerraNews 泰拉新闻

- 作者: MiMo
- 出处: TShock插件库
- 每天 04:30 全服播报今日渔夫任务鱼与今日月相；沙尘暴、旅商到访时同步播报
- 任务鱼和旅商货架用原版可交互物品图标展示，**鼠标悬停即可查看详情**

## 说明

- 播报时刻是**游戏内时间**的 04:30，触发窗口默认 04:30–05:00，**下午不会补播**
- 任务鱼的名称、产地、深度、钓法提示**都在图标的悬停提示里**，聊天框中不再重复一遍
- 每日任务与换任务是同一帧触发的（`Main.mfwh_AnglerQuestSwap`），所以播报的一定是新任务
- 服务器**无玩家在线时世界不推进**，此时不会触发播报；玩家进服后可用 `/terranews` 手动查看
- 每日播报效果：

```
========== 泰拉新闻 · 今日渔夫任务 ==========
任务鱼 [i:2466]
今日月相 娥眉月
（游戏时间 04:30）输入 /terranews 可随时重新查看今日任务
```

## 指令

| 语法 | 别名 | 权限 | 说明 |
|-----|:----:|:----:|:----:|
| /terranews | /新闻 /泰拉新闻 /news | 所有人 | 查看今日渔夫任务 + 月相 |
| /terranews broadcast | daily 日常 | tshock.admin | 立即全服播报今日任务 |
| /terranews storm | sandstorm weather 天气 | tshock.admin | 立即播报天气预警 |
| /terranews merchant | shop 旅商 | tshock.admin | 立即播报旅商货架 |
| /terranews reload | 重载 reloadconfig | tshock.admin | 重载配置并重置事件检测状态 |

> 指令别名可在配置的 `CommandAliases` 中修改。

## 配置
> 配置文件位置：tshock/TerraNews.json
```json5
{
  // 总开关。关闭后除 reload 外所有指令都会提示已关闭
  "Enabled": true,
  // 每项功能单独开关，默认为全开
  "Features": {
    "DailyQuestBoard": true,      // 04:30 的整条每日播报
    "QuestFishIcon": true,        // 任务鱼的可交互图标
    "FishingLocation": true,      // 钓鱼地点、深度、Y 区间、提示
    "AnglerStatus": true,         // 渔夫当前状态
    "MoonPhase": true,            // 今日月相
    "Sandstorm": true,            // 沙尘暴 / 暴风雪开始时的预警
    "SandstormPeak": true,        // 风暴达到峰值时的补报
    "TravelingMerchant": true,    // 旅商货架播报
    "ServerLog": true             // 同时写入 TShock 日志
  },
  "BroadcastHour": 4,             // 每日播报小时（游戏内）
  "BroadcastMinute": 30,          // 每日播报分钟（游戏内）
  "TriggerWindowSeconds": 30,     // 触发窗口，单位是游戏内分钟
  "StartupDelaySeconds": 5,       // 插件加载后的静默期
  "SandstormPeakSeverity": 0.95,  // 风暴峰值阈值
  "MerchantItemsPerLine": 5,      // 货架每行图标数，0 = 不换行
  "NameSource": "both",           // 鱼名来源：zh | vanilla | both
  "Diagnostics": false,           // 每秒一行诊断日志
  "CommandPermission": "",        // 留空 = 所有人
  "AdminPermission": "",          // 留空 = tshock.admin
  "CommandAliases": ["新闻", "泰拉新闻", "news"],
  "DailyLines": [
    "[c/4FC3F7:========== 泰拉新闻 · 今日渔夫任务 ==========]",
    "[c/FFD966:任务鱼] [c/FFFFFF:{icon}]",
    "[c/B39DDB:今日月相] [c/FFFFFF:{moon}]",
    "[c/888888:（游戏时间 {time}）输入 /terranews 可随时重新查看今日任务]"
  ],
  "SandstormLines": [
    "[c/E0A458:========== 泰拉新闻 · 天气预警 ==========]",
    "[c/FFD966:{storm}]",
    "[c/FFFFFF:强度 {severity} · 预计持续 {remaining}]",
    "[c/888888:沙漠起黄沙，雪原飞暴雪，出行注意（游戏时间 {time}）]"
  ],
  "SandstormPeakLines": [
    "[c/E0A458:========== 泰拉新闻 · 天气预警 ==========]",
    "[c/FF6B6B:{storm} 已达最强]",
    "[c/FFFFFF:强度 {severity} · 预计持续 {remaining}]",
    "[c/888888:能见度极差，建议尽快返回城镇（游戏时间 {time}）]"
  ],
  "MerchantLines": [
    "[c/4FC3F7:========== 泰拉新闻 · 旅商到访 ==========]",
    "[c/FFD966:今日货架（悬停查看详情）]",
    "[c/FFFFFF:{items}]",
    "[c/888888:共 {count} 件 · 售完即止（游戏时间 {time}）]"
  ]
}
```

### 占位符

- 每日：`{icon}` `{name}` `{name_zh}` `{name_en}` `{name_vanilla}` `{biome}` `{depth}`
  `{yrange}` `{tip}` `{angler}` `{time}` `{id}` `{moon}` `{moon_bonus}`
- 天气：`{storm}` `{severity}` `{remaining}` `{time}` `{moon}`
- 旅商：`{items}` `{count}` `{time}` `{moon}`

> 默认模板故意很简短，任务鱼的名称与钓法都在 `{icon}` 的悬停提示里。所有占位符都**依然可用**，
> 想换回完整看板只需改写 `DailyLines`，例如：
> ```json5
> "DailyLines": [
>   "[c/4FC3F7:========== 泰拉新闻 · 今日渔夫任务 ==========]",
>   "[c/FFD966:任务鱼] [c/FFFFFF:{icon} {name}]",
>   "[c/FFD966:钓鱼地点] [c/FFFFFF:{biome} · {depth}{yrange}]",
>   "[c/FFD966:提示] [c/FFFFFF:{tip}]",
>   "[c/B39DDB:今日月相] [c/FFFFFF:{moon} · {moon_bonus}]",
>   "[c/7FD4FF:{angler}]",
>   "[c/888888:（游戏时间 {time}）输入 /terranews 可随时重新查看今日任务]"
> ]
> ```
> 注意 `{angler}` 只能输出纯文本，不能自带颜色标签。

### 开关行为

- 关掉某项后，**依赖它的整行会消失**，不会留下没有内容的标签
- 默认模板不使用 `{biome}` `{depth}` `{tip}` `{angler}` `{moon_bonus}`，所以默认配置下
  `FishingLocation` 与 `AnglerStatus` 两个开关不影响输出，改写 `DailyLines` 后才会生效
- 手动调用已关闭的功能会明确告知原因

## 更新日志

### v1.3.0
- 改用仓库统一的 `GetString` 国际化机制，补充 `i18n` 翻译模板
- 精简插件结构，移除仅为多端共享而存在的抽象层
- 合并四处重复的权限与功能开关检查

### v1.2.1
- 每日看板从 7 行精简为 4 行：任务鱼只留可交互图标，名称与钓法在悬停提示里
- 未被改动的 1.2.0 默认模板在升级时自动替换，手工改过的模板保持不变

### v1.2.0
- 新增 `Features` 逐项开关，取代 1.0 的平铺布尔项
- 新增 `MerchantItemsPerLine` 货架换行

## 反馈

- 问题反馈：https://github.com/UnrealMultiple/TShockPlugin/issues
- TShock 交流群：816771079
- TRHub：https://trhub.cn ；BBSTR：https://bbstr.net ；TR 主页：https://tr.monika.love
