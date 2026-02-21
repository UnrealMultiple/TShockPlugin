# AutoFishR 自动钓鱼重制版

## 作者
- **少司命**: 提供核心代码
- **羽学**: 提供创意与初版实现
- **ksqeib**: 当前维护者

## 重要说明
- 本插件出现任何问题，请联系 **ksqeib**
- 唯一联系QQ：**2388990095**
- **请勿在群里艾特开发者**，有事请添加QQ私聊
- **警告：在群内艾特其他开发者造成打扰，后果自负！**

## 说明
Tshock 服务器自动钓鱼插件，支持自动收杆、多钩、Buff、额外渔获、消耗模式等，可按权限与全局开关动态显隐指令。

**使用前提：必须开启 SSC（ServerSideCharacter），否则无法正常代扣鱼饵。**

- 历史仓库: https://github.com/ksqeib/AutoFish-old

## 权限模型（重要）

- 管理员全通: `autofish.admin`。
- 通用白名单: `autofish.common`，拥有它即可使用全部玩家指令（仍受全局开关与负权限影响）。
- 功能权限: `autofish.<feature>`，示例 `autofish.fish`、`autofish.multihook` 等。
- 负权限: `autofish.no.<feature>`，拥有该权限即强制无权限（除 admin 外），示例 `autofish.no.fish`。
- `/af` 命令本身需要 `autofish`；拥有 `autofish.common` 等同可用全部玩家指令。

示例：
- 想让默认组能用除钓鱼外的所有功能，可给 default 组添加 `autofish.common`，再添加 `autofish.no.fish`，这样普通玩家可用 BUFF/多钩等，但无法开启自动钓鱼。

## 玩家指令（/af, /autofish）

| 命令 | 说明 | 所需权限 | 其他前置 |
| --- | --- | --- | --- |
| /af | 查看菜单/帮助 | autofish | 插件开启 |
| /af status | 查看个人状态 | autofish |  |
| /af fish | 开/关自动钓鱼 | autofish.fish | 全局自动钓鱼开启 |
| /af buff | 开/关钓鱼 Buff | autofish.buff | 全局 Buff 开启 |
| /af multi | 开/关多钩 | autofish.multihook | 全局多钩开启 |
| /af hook 数字 | 设置个人钩子上限 | autofish.multihook | 全局多钩开启，数值 ≤ 全局上限 |
| /af monster | 开/关不钓怪物 | autofish.filter.monster | 全局不钓怪开启 |
| /af anim | 开/关跳过上鱼动画 | autofish.skipanimation | 全局动画跳过开启 |
| /af list | 查看消耗模式指定物品 | autofish | 全局消耗模式开启 |
| /af loot | 查看额外渔获表 | autofish | 需配置额外渔获列表非空 |
| /af bait | 开/关保护贵重鱼饵 | autofish.bait.protect | 全局保护贵重鱼饵开启 |
| /af baitlist | 查看贵重鱼饵列表 | autofish.bait.protect | 同上 |

> 负权限优先：拥有 `autofish.no.<feature>` 时，除 admin 外一律视为无权。

## 管理员指令（/afa, /autofishadmin）

全部指令需 `autofish.admin`。

| 命令 | 说明 |
| --- | --- |
| /afa | 查看管理员帮助菜单 |
| /afa buff | 全局开/关钓鱼 Buff |
| /afa multi | 全局开/关多线模式 |
| /afa duo 数字 | 设置全局多钩上限 |
| /afa mod | 全局开/关消耗模式 |
| /afa set 数量 | 设置消耗物品数量（消耗模式开启时生效） |
| /afa time 数字 | 设置奖励时长（分钟，消耗模式开启时生效） |
| /afa add 物品名 | 添加指定鱼饵（消耗模式开启时可见） |
| /afa del 物品名 | 移除指定鱼饵（消耗模式开启时可见） |
| /afa addloot 物品名 | 添加额外渔获 |
| /afa delloot 物品名 | 移除额外渔获 |
| /afa monster | 全局开/关不钓怪物 |
| /afa anim | 全局开/关跳过上鱼动画 |

其他：`/reload`（tshock.cfg.reload）可重载配置。

## 配置
配置说明参见 [resource/config/zh-cn.yml](resource/config/zh-cn.yml) 或 [resource/config/en-us.yml](resource/config/en-us.yml)（缺失时插件会根据系统语言自动写入默认模板）。
## 注意事项

- `/af` 对普通玩家最简做法：给组添加 `autofish.common` 即可；若要禁用某功能，额外赋予对应 `autofish.no.<feature>`。
- 启用消耗模式后，个人需要拥有消耗时长；插件会在缺少鱼饵时直接返回。
- 多钩/不钓怪/跳过动画等均受“全局开关 + 个人开关 + 权限”共同约束。

## 排错指南

1. **检查插件总开关**：确认配置中 `pluginEnabled` 为 `true`。
2. **检查全局功能开关**：确认 `globalAutoFishFeatureEnabled` 为 `true`。
3. **检查权限**：玩家需要 `autofish` 且具备对应功能权限（或 `autofish.common`）。如有 `autofish.no.<feature>` 会强制无权限。
4. **检查消耗模式**：若启用消耗模式，确保已添加兑换规则且玩家有剩余时长。
5. **检查贵重鱼饵保护**：启用后会自动换位保护鱼饵。如出现异常可尝试关闭：`/af bait`。
6. **检查鱼饵**：无鱼饵会停止自动钓鱼，请补充后重新抛竿。
7. **确认 SSC 已开启**：本插件依赖 SSC（ServerSideCharacter），未开启将无法正常工作。
8. **启用调试**：管理员使用 `/afa debug` 开启调试模式。
9. **复现问题并截图**：让玩家复现钓鱼过程，保留聊天提示与控制台输出截图，用于问题反馈。

## 原理（功能行为与关键逻辑）

- 自动钓鱼：在浮漂 AI 更新时检测 `bobber.ai[1] < 0`（已上钩），扣除鱼饵、调用原版收杆逻辑，再重发弹幕；若配置开启额外渔获/过滤怪物，会在生成掉落前执行筛选与替换。
- 多钩：在生成鱼线弹幕事件中统计当前浮漂数量，未超出上限时为玩家复制一枚新的鱼线弹幕，实现并行钓鱼；同时受消耗模式与玩家多钩开关限制。
- 跳过上鱼动画：收杆后直接向客户端发送 `ProjectileDestroy`，省略动画。
- 不钓怪物：当判定结果为怪物（catchId < 0）且开启该功能时，丢弃结果重新尝试。
- 保护贵重鱼饵：检测当前使用的鱼饵是否在贵重列表中，如是则尝试与背包尾部的鱼饵位置交换，并同步槽位，避免被扣除。
- 消耗模式：全局开启时，玩家需个人开启且有剩余时长才执行自动钓鱼/多钩；剩余时长通过玩家消耗指定物品兑换（指令与逻辑同前述配置字段）。
- Buff：当检测到玩家鱼线存在且全局/个人 Buff 开启时，为玩家施加 `Buff表` 配置的 Buff（ID+持续时间）。
- 提示与首钓：玩家第一次抛竿时会提示可使用 `/af fish` 开启自动钓鱼（仅提示一次）。

## 更新日志

```
v1.4.9.1
修正多竿钓鱼下会吞物品

v1.4.9
拆分额外渔获到独立项目

v1.4.8.1
添加防呆代码
添加排错说明
添加用于debug的代码
修复反复交换两个贵重鱼饵的bug

v1.4.8
修复钓鱼代码
修正刷浮漂解决方案

v1.4.7
添加自动打包
使用类原版系统添加额外渔获
添加屏蔽任务鱼功能
优化Status命令

v1.4.6
修复消耗模式不可用
添加更多提示
简化消耗模式代码
简化溅射物生成代码
消耗模式可配置不同鱼饵时间

v1.4.5
简化代码
修复刷浮漂
修复保护不住贵重鱼饵
修复不上额外渔获
简化部分代码逻辑

v1.4.4
兼容1.4.5

v1.4.3
改进钓鱼逻辑性能开销
预设一些简单渔获
让钓鱼逻辑合理化，使得视觉效果与原版几乎一致
对玩家命令和管理员命令进行拆分
添加贵重鱼饵保护功能和对应的命令
命令对控制台添加特判
重排配置顺序
引入负权限系统

更久之前请查找前仓库
```

## 反馈

- issue: https://github.com/UnrealMultiple/TShockPlugin
- QQ：816771079
- 社区：trhub.cn / bbstr.net / tr.monika.love

## 更新日志

- 见 CHANGELOG.md

## 特别鸣谢
TShock群友 羯淩(2372652098) 赞助十元用于开发者开发时购买雪碧