# Economics.NPC 插件 自定义怪物奖励

- 作者: 少司命，千亦
- 出处: 无
- 配置 NPC 专属奖励

> [!NOTE]  
> 需要安装前置插件：EconomicsAPI(本仓库) 

## 指令

- `/npcreload` —— 重读怪物 AI 脚本（权限 `economics.npc.admin`）。<br/>
  新增脚本文件在对应怪下次生成时即时生效；**修改已有脚本**后执行此命令（或 `/reload`）重新编译。
- `/reload` —— TShock 全局 reload，本插件也会借此重读怪物 AI 脚本（与 `/npcreload` 效果相同）。

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
## JS 怪物 AI（新增）

基于 `Economics.Script` 实现，用 JS 自定义怪物 AI 行为。无需复杂配置：
在 `tshock/Economics/NPCJSScripts/` 放一个以怪物 netID 命名的 `.js` 文件即生效。

```jsonc
// NPC.json 中新增开关（可选，默认 true）
{
  "启用JS怪物AI": true
}
```

脚本目录（运行期自动创建）：

```
tshock/Economics/NPCJSScripts/
   123.js      <-- 怪物 netID 为 123 的 AI
```

可定义的事件钩子（都可省略）：

```
onSpawn(npc)            怪物生成时
ai(npc, index, time, struck)   每帧调用，核心：用来修改 AI
onStrike(npc, damage)   被玩家命中时
onKill(npc)             被击杀时
```

- `npc` —— `Terraria.NPC`，可直接读写 `npc.ai[0..3]`、`npc.velocity`、`npc.position`、`npc.aiStyle`、`npc.life` 等。
- `index` —— `npc.whoAmI`；`time` —— 存活秒数；`struck` —— 被命中次数。
- 脚本还能通过 CLR 访问 `Terraria` / `TShockAPI` / `Economics` API，并调用宿主函数：

| 宿主函数 | 说明 |
|---|---|
| `Say(msg)` | 向控制台输出 |
| `Broadcast(msg, r, g, b)` | 向所有玩家广播（RGB 默认 255） |
| `ActivePlayerCount()` | 当前活跃玩家数 |
| `SpawnNpc(netId, x, y)` | 在指定像素坐标召唤 1 只怪物 |
| `SpawnProjectile(npcIndex, x, y, vx, vy, type, damage, knockback, ai0?, ai1?, ai2?)` | 发射弹幕（Owner 固定为 `Main.myPlayer`）。发射速度 `vx/vy` 由脚本自行计算，可实现“出生时朝锁定玩家射过去”（参考插件里 96/44 弹幕就是锁定玩家：在脚本里找出范围内最近玩家，用方向×锁定速度作为 `vx/vy`） |

示例（`123.js`）：

```javascript
function onSpawn(npc) {
    Say("自定义怪物 " + npc.FullName + " 出现了");
    npc.aiStyle = -1;   // 关闭原版 AI（可选）
}

function ai(npc, index, time, struck) {
    // 修改 npc.ai[] / npc.velocity / npc.position 即可改变 AI 行为（写法由使用者自定）
    npc.ai[0] += 0.1;   // 示例
}
```

> 完整示例：`src/Economics.NPC/Examples/50.js`（史莱姆王）与 `src/Economics.NPC/Examples/4.js`（克苏鲁的右眼）复刻了原「自定义怪物血量」配置里两个 Boss 的逻辑：
> - `50.js`：血量随人数缩放、防御 12、范围内 buff 137、每 10/20 秒按配置发射贴图弹幕并喊话、死亡广播。
> - `4.js`：血量随人数缩放、双重光环(毒/火)、三档定时弹幕(15s/1.5s/0.2s 连射)、血线事件(四向弹雨+喊话)、死亡召唤 10 只小怪。
> 把它们复制为 `tshock/Economics/NPCJSScripts/50.js`、`4.js` 即可使用。

> 注意：`ai` 每帧调用（60/s），请保持轻量。脚本有 2 秒超时 / 20 万条语句上限 / 栈溢出保护，防止拖垮服务器。

### v2.1.0.0
- **破坏性变更**："转换率更改"的"转换率"字段语义修正为"默认击杀奖励的加成系数"，填 `1.5` 表示"拿到默认奖励的 150%"。此前实现把它当成奖励池总量用，导致设置 `1.5` 的 Boss 实际只到账 1-5 块钱。升级后请根据新语义重新评估配置值
- 修复同时配置"转换率更改"和"额外奖励列表"时，额外奖励的系统提示不触发的问题，两段配置现在可以对同一只怪共存生效
- 修复"转换率更改"分支下货币飘字不受全局"悬浮文本.启用"开关和"指定ID"过滤控制的问题，行为与默认击杀结算保持一致
- 当玩家不满足"转换率更改"里配置的进度条件时，改为放行默认击杀奖励（以及"额外奖励列表"），而不是一分钱也拿不到

### v2.0.0.0
- 适配多货币

## 反馈

- 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 国内社区 trhub.cn 或 TShock 官方群等
