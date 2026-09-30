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
  // 每项功能单独开关，缺省全部为开
  "Features": {
    "DailyQuestBoard": true,    // 04:30 整条每日播报
    "QuestFishIcon": true,      // 任务鱼可交互图标
    "FishingLocation": true,    // 钓鱼地点 / 深度 / 提示
    "AnglerStatus": true,       // 渔夫状态
    "MoonPhase": true,          // 今日月相
    "Sandstorm": true,          // 沙尘暴 / 暴风雪预警
    "SandstormPeak": true,      // 风暴达到最强时的补报
    "TravelingMerchant": true,  // 旅商货架
    "ServerLog": true           // 同时写 TShock 日志
  },
  "BroadcastHour": 4,           // 每日播报小时（游戏内）
  "BroadcastMinute": 30,        // 每日播报分钟（游戏内）
  "TriggerWindowSeconds": 30,   // 触发窗口，秒=游戏内分钟。30 即 04:30–05:00
  "StartupDelaySeconds": 5,     // 插件加载后的静默期
  "SandstormPeakSeverity": 0.95,
  "MerchantItemsPerLine": 5,    // 货架每行图标数，0 = 不换行
  "NameSource": "both",         // zh / en / both
  "Diagnostics": false,
  "CommandPermission": "",      // /terranews 权限，空 = 所有人
  "AdminPermission": "",        // 管理子命令权限，空 = tshock.admin
  "CommandAliases": ["新闻", "泰拉新闻", "news"]
}
```

### 播报模板

四组模板都可自定义，支持整行颜色标签 `[c/4FC3F7:内容]`，也支持行内多段
`[c/FFD966:标签] [c/FFFFFF:内容]`。

- `DailyLines` —— 可用 `{icon}` `{name}` `{name_zh}` `{name_en}` `{name_vanilla}` `{biome}` `{depth}` `{yrange}` `{tip}` `{angler}` `{moon}` `{moon_bonus}` `{time}` `{id}`
- `SandstormLines` / `SandstormPeakLines` —— `{storm}` `{severity}` `{remaining}` `{time}` `{moon}`
- `MerchantLines` —— `{items}` `{count}` `{time}` `{moon}`

> 默认模板只用了 `{icon}` `{moon}` `{time}`，因为其余信息都在图标悬停提示里。
> 想换回长版看板，把 `DailyLines` 改成下面这样即可（`FishingLocation`、`AnglerStatus`
> 两个开关随之重新生效）：
>
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
>
> `{angler}` 只能输出纯文本，不能自带颜色标签——外层标签被剥掉后嵌套标签会变成字面文本。

关掉某项功能后，**依赖它的整行会一起消失**，不会留下一个没有内容的标签。
已被手动关闭的功能，指令会明确告知原因，例如
`该功能已在配置中关闭（Features.TravelingMerchant=false）。`

## 更新日志
### v1.2.1
- 每日看板精简：任务鱼只留可交互图标，不再重复输出名称、钓鱼地点、深度、提示与渔夫状态
- 月相不再显示钓鱼力加成
- 新增 `weather` 指令别名，与 tModLoader 版统一用词
- 旧版默认模板自动迁移为精简版（逐行比对，只有完全未改动才会替换）

### v1.2.0
- 新增总开关 + 每项功能单独开关（`Enabled` 与 `Features`）
- 1.1 的平铺布尔配置自动迁移到 `Features` 块
- 支持旅商货架图标按行数换行

### v1.1.0
- 天气播报拆分为「预警」与「最强补报」两套模板
- 任务鱼目录扩充到 41 条，覆盖全部进度阶段

### v1.0.0
- 添加插件

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
