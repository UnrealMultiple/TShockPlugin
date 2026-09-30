# TerraNews 泰拉新闻

- 作者: 不是现在
- 出处: TShock插件库
- 每天 04:30 全服播报今日渔夫任务鱼与今日月相；沙尘暴、暴风雪、旅商到访时同步播报
- 任务鱼和旅商货架用原版可交互物品图标展示，**鼠标悬停即可查看详情**

## 说明

- 播报时刻是**游戏内时间**的 04:30，触发窗口默认 04:30–05:00，**下午不会补播**
- 任务鱼的名称、产地、深度、钓法提示**都在图标的悬停提示里**，聊天框中不再重复一遍
- 沙尘暴与暴风雪是同一个原版事件，按玩家所在生物群系区分，插件分开播报
- 每日任务与换任务是同一帧触发的（`Main.mfwh_AnglerQuestSwap`），所以播报的一定是新任务
- 服务器**无玩家在线时世界不推进**，此时不会触发播报；玩家进服后可用 `/terranews` 手动查看
- 每日播报效果：

```
========== 泰拉新闻 · 今日渔夫任务 ==========
任务鱼 [i:2466]
今日月相 娥眉月
（游戏时间 04:30）输入 /terranews 可随时重新查看今日任务
```

> 自定义模板时请注意：**含物品图标的行只能用一个颜色标签包住整行**，
> 否则图标会显示成 `[i:2466]` 这样的字符串。原因见下方
> [颜色标签与物品图标不能嵌套](#️-颜色标签与物品图标不能嵌套)。

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
  "MerchantItemsPerLine": 5,      // 货架每行图标数，0 = 不换行
  "Diagnostics": false,           // 每秒一行诊断日志
  "CommandPermission": "",        // 留空 = 所有人
  "AdminPermission": "",          // 留空 = tshock.admin
  "CommandAliases": ["新闻", "泰拉新闻", "news"],
  "DailyLines": [
    "[c/4FC3F7:========== 泰拉新闻 · 今日渔夫任务 ==========]",
    "[c/FFD966:任务鱼 {icon}]",
    "[c/FFFFFF:今日月相 {moon}]",
    "[c/888888:（游戏时间 {time}）输入 /terranews 可随时重新查看今日任务]"
  ],
  "SandstormLines": [
    "[c/E0A458:========== 泰拉新闻 · 天气预警 ==========]",
    "[c/FFD966:{storm}]",
    "[c/FFFFFF:强度 {severity} · 预计持续 {remaining}（游戏时间 {time}）]"
  ],
  "BlizzardLines": [
    "[c/E0A458:========== 泰拉新闻 · 天气预警 ==========]",
    "[c/FFD966:{storm}]",
    "[c/FFFFFF:强度 {severity} · 预计持续 {remaining}（游戏时间 {time}）]"
  ],
  "SandstormPeakLines": [
    "[c/E0A458:========== 泰拉新闻 · 天气预警 ==========]",
    "[c/FF6B6B:{storm}已达最强]",
    "[c/FFFFFF:强度 {severity} · 预计持续 {remaining}（游戏时间 {time}）]"
  ],
  "BlizzardPeakLines": [
    "[c/E0A458:========== 泰拉新闻 · 天气预警 ==========]",
    "[c/FF6B6B:{storm}已达最强]",
    "[c/FFFFFF:强度 {severity} · 预计持续 {remaining}（游戏时间 {time}）]"
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

- 每日：`{icon}` `{angler}` `{time}` `{id}` `{moon}` `{moon_bonus}`
- 天气：`{storm}` `{severity}` `{remaining}` `{time}` `{moon}`
- 旅商：`{items}` `{count}` `{time}` `{moon}`

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
是黄沙还是暴雪（客户端判定为 `ZoneSnow && ZoneRain`）。插件用同样的办法区分：在线的
玩家里有人在雪地就报暴风雪，有人在沙漠就报沙尘暴，都不在时退回抽样扫描世界地表。

四套模板对应四种情况：

| 模板 | 用途 |
|:--|:--|
| `SandstormLines` | 沙尘暴开始 |
| `BlizzardLines` | 暴风雪开始 |
| `SandstormPeakLines` | 沙尘暴达到最强 |
| `BlizzardPeakLines` | 暴风雪达到最强 |

### 开关行为

- 关掉某项后，**依赖它的整行会消失**，不会留下没有内容的标签
- 默认模板不使用 `{angler}` `{moon_bonus}`，所以默认配置下 `AnglerStatus` 不影响输出，
  改写 `DailyLines` 后才会生效
- 手动调用已关闭的功能会明确告知原因

## 更新日志

### v1.0.0
首个发布版本。功能：

- 每天游戏内 04:30 全服播报今日渔夫任务鱼与今日月相，任务鱼用可悬停的原版物品图标
- 沙尘暴 / 暴风雪开始与达到最强时播报预警，沙漠与雪原分开播报
- 旅商到访时播报今日货架（纯图标，可悬停）
- 总开关加 8 项逐项功能开关，支持 `/terranews reload` 热重载
- 兼容 1.0 的平铺布尔配置，老配置自动迁移

## 反馈

- 问题反馈：https://github.com/UnrealMultiple/TShockPlugin/issues
