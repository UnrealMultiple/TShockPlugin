# TerraNews 泰拉新闻

- 作者: 不是现在

## 说明

- 播报时刻是**游戏内时间**的 04:30，触发窗口默认 04:30–05:00
- 沙尘暴与暴风雪是同一个原版事件，按地形扫描分开播报，可用 `StormType` 固定
- 每日任务与换任务是同一帧触发的（`Main.mfwh_AnglerQuestSwap`），所以播报的一定是新任务
- 服务器**无玩家在线时世界不推进**，此时不会触发播报
- 每日播报效果：

```
========== 泰拉新闻 · 今日渔夫任务 ==========
任务鱼 [i:2466]
今日月相 娥眉月
```

> 自定义模板时请注意：**含物品图标的行只能用一个颜色标签包住整行**，
> 否则图标会显示成 `[i:2466]` 这样的字符串。原因见下方
> [颜色标签与物品图标不能嵌套](#️-颜色标签与物品图标不能嵌套)。

## 配置
> 配置文件位置：tshock/TerraNews.json
```json5
{
  // 总开关。关闭后插件完全静默，改完需重启服务器
  "Enabled": true,
  // 每项功能单独开关，默认为全开
  "Features": {
    "DailyQuestBoard": true,      // 04:30 的整条每日播报
    "QuestFishIcon": true,        // 任务鱼的可交互图标
    "AnglerStatus": true,         // 渔夫当前状态（仅在自定义模板里用到）
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
  "StormType": "auto",            // auto | sandstorm | blizzard
  "MerchantItemsPerLine": 5,      // 货架每行图标数，0 = 不换行
  "Diagnostics": false,           // 每秒一行诊断日志
  "DailyLines": [
    "[c/4FC3F7:========== 泰拉新闻 · 今日渔夫任务 ==========]",
    "[c/FFD966:任务鱼 {icon}]",
    "[c/FFFFFF:今日月相 {moon}]"
  ],
  "SandstormLines": [
    "[c/E0A458:========== 泰拉新闻 · 天气预警 ==========]",
    "[c/FFD966:{storm}已登陆]",
    "[c/FFFFFF:强度 {severity}]"
  ],
  "BlizzardLines": [
    "[c/E0A458:========== 泰拉新闻 · 天气预警 ==========]",
    "[c/FFD966:{storm}已登陆]",
    "[c/FFFFFF:强度 {severity}]"
  ],
  "SandstormPeakLines": [
    "[c/E0A458:========== 泰拉新闻 · 天气预警 ==========]",
    "[c/FF6B6B:{storm}已达最强]",
    "[c/FFFFFF:强度 {severity}]"
  ],
  "BlizzardPeakLines": [
    "[c/E0A458:========== 泰拉新闻 · 天气预警 ==========]",
    "[c/FF6B6B:{storm}已达最强]",
    "[c/FFFFFF:强度 {severity}]"
  ],
  "MerchantLines": [
    "[c/4FC3F7:========== 泰拉新闻 · 旅商到访 ==========]",
    "[c/FFD966:今日货架（悬停查看详情）]",
    "[c/FFFFFF:{items}]"
  ]
}
```

### 占位符

- 每日：`{icon}` `{angler}` `{id}` `{moon}` `{moon_bonus}` `{time}`
- 天气：`{storm}` `{severity}` `{remaining}` `{time}` `{moon}`
- 旅商：`{items}` `{count}` `{time}` `{moon}`

> 默认模板只用到其中一部分，想加就自己写进 `DailyLines`。
> `{angler}` 输出纯文本，不能自带颜色标签；`{icon}` 所在行不能嵌套颜色标签（见下）。

> **任务鱼只有 ID**：名称、产地、深度、钓法全部来自原版物品标签 `{icon}` 的悬停提示，
> 插件不再自带鱼的介绍数据。想自定义看板只需改写 `DailyLines`，例如：
> ```json5
> "DailyLines": [
>   "[c/4FC3F7:========== 泰拉新闻 · 今日渔夫任务 ==========]",
>   "[c/FFD966:任务鱼 {icon}]",
>   "[c/FFFFFF:今日月相 {moon} · {moon_bonus}]",
>   "[c/7FD4FF:{angler}]",
>   "[c/888888:（游戏时间 {time}）鱼 ID {id}]"
> ]
> ```

### ⚠ 颜色标签与物品图标不能嵌套

这是最容易踩的坑：**含 `{icon}` 或 `{items}` 的行，只能用「一个颜色标签包住整行」**。

原版客户端用一条正则从头扫到尾地解析聊天文本，其中文字部分是非贪婪匹配：

```
\[(?<tag>[a-zA-Z]{1,10})(\/(?<options>[^:]+))?:(?<text>.+?)(?<!\\)\]
```

所以颜色标签只会吞到它后面的**第一个** `]`。写成两段颜色标签时：

```json5
// 错误：图标会变成一串 [i:2453] 字符，不会变成图标
"[c/FFD966:任务鱼] [c/FFFFFF:{icon}]"
```

`[c/FFFFFF:` 的正文字段会把 `[i:2453` 整个当成自己的内容，客户端再也不会把它
渲染成图标。

```json5
// 正确：整行一个颜色标签
"[c/FFD966:任务鱼 {icon}]"
```

这种写法插件会在服务端剥掉标签、改用真正的 RGB 发送，客户端收到的是不含任何标签
的纯文本，图标正常渲染。不含物品标签的行则可以用多段颜色，如
`"[c/B39DDB:今日月相] [c/FFFFFF:{moon}]"`。

> 写到嵌套写法时插件会在 TShock 日志里警告一次，并指出正确写法。

### 沙尘暴与暴风雪

原版的沙尘暴和暴风雪是**同一个 `Sandstorm` 事件**，游戏按玩家所在生物群系决定看起来
是黄沙还是暴雪。它在客户端判定为 `player.ZoneSnow && player.ZoneRain`，但**这两个标志
在专用服务器上不可靠**：`ZoneRain = Main.raining && Y <= worldSurface` 是全局的、准确，
而 `ZoneSnow` 来自客户端算的 `Main.SceneMetrics`，服务器上读到的是过期值。搭配
"一直下雨"（`rain`）种子，`ZoneRain` 恒为真，判据就退化成只看 `ZoneSnow`，容易把
沙尘暴误报成暴风雪。

因此插件不问玩家，改**抽样扫描地表明层**，数沙块(32)与雪块(51)谁多。这个结果只与世界
地形有关，扫描一次后缓存，重启服务器时重新扫描。

> 实测：默认世界与"一直下雨"种子的沙漠地表都约为雪原的 4 倍（沙 8830 / 雪 2378、
> 沙 7516 / 雪 1600），所以 `auto` 的判定结果都是沙尘暴。

沙尘暴与暴风雪无法在同一个世界里两全——同一个事件对沙漠玩家是黄沙、对雪原玩家是暴雪。
如果你要固定用某个名字，或者你的世界地形让 `auto` 判反了，可以直接指定：

| `StormType` | 行为 |
|:--|:--|
| `auto`（默认） | 扫描地表明层，雪多报暴风雪，否则报沙尘暴 |
| `sandstorm` | 一律播报沙尘暴 |
| `blizzard` | 一律播报暴风雪 |

四套模板对应四种情况：

| 模板 | 用途 |
|:--|:--|
| `SandstormLines` | 沙尘暴开始 |
| `BlizzardLines` | 暴风雪开始 |
| `SandstormPeakLines` | 沙尘暴达到最强 |
| `BlizzardPeakLines` | 暴风雪达到最强 |

#### 实机验证记录

沙尘暴与暴风雪两侧都已在真实客户端环境下验证通过：起播时的名称、模板与图标都正确。

验证时值得注意的地方：

- **`auto` 依赖世界地形**。需要在雪原为主地表的世界才能判成暴风雪，否则用
  `"StormType": "blizzard"` 强制。启动日志里有一行判定结果，便于确认：
  `[TerraNews] 地形判定：雪块 N 格，沙块 M 格 → 这场按暴风雪播报。`
- **必须先有客户端连上**。专用服务器无玩家在线时世界不推进，自动播报不会发生。
- **控制台复现不出暴风雪场景**。`/worldevent` 的合法事件类型里没有 `blizzard`，
  只能用 `/worldevent sandstorm` 制造，再靠地形决定播报成哪一种。
- **峰值补报要等强度自然爬升**。只有 `Sandstorm.Severity` 越过
  `SandstormPeakSeverity`（默认 0.95）才会触发 `*PeakLines`，短时间反复开关风暴
  是到不了这个阈值的。

### 开关行为

- 关掉某项后，**依赖它的整行会消失**，不会留下没有内容的标签
- 默认模板不使用 `{angler}` `{moon_bonus}`，所以默认配置下 `AnglerStatus` 不影响输出，
  改写 `DailyLines` 后才会生效

## 更新日志

### v1.0.0
首个发布版本。功能：

- 每天游戏内 04:30 全服播报今日渔夫任务鱼与今日月相，任务鱼用可悬停的原版物品图标
- 沙尘暴 / 暴风雪开始与达到最强时播报预警，沙漠与雪原分开播报，可用 `StormType` 固定
- 旅商到访时播报今日货架（纯图标，可悬停）
- 全部自动播报，不注册任何指令

## 反馈

- 问题反馈：https://github.com/UnrealMultiple/TShockPlugin/issues
