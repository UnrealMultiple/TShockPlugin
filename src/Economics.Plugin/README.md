# Economics.Plugin

一个**基于 [Economics.Script](../Economics.Script/README.md) 的 JS 脚本宿主插件**。C# 端只负责**加载/卸载**脚本，
并把真实 TShock API（`ServerApi.Hooks` 和插件实例）交给脚本；其余功能（命令、事件钩子、交互）全部由 JS 脚本「直接」实现。

渲染 JS 的是 [Jint](https://github.com/sebastienros/jint)。

## 脚本生命周期

| 阶段 | 函数 | 说明 |
| --- | --- | --- |
| 加载 | `init()` | 插件启动 / `TShock /reload` / `/escript reload` 时，逐个执行，用于注册命令与钩子 |
| 卸载 | `unload()` | 重载或插件停用时执行一次，建议在这里反注册钩子（命令由宿主自动移除） |

## 脚本目录与加载约定

```
tshock 保存目录/EconomicsPlugin/Scripts/
```

**只有文件名以 `plugin-` 开头、以 `.js` 结尾的脚本（即 `plugin-*.js`）会被加载**，其余放在该目录里的 `.js`
不会被当作插件执行——可用来放 `@require` 的辅助/库文件。

可以查看 `TShockPlugin\src\Economics.Plugin\Scripts\plugin-demo.js` 中的示例脚本。

> 想改约定？改 `Economics.Plugin/Scripting/ScriptHost.cs` 里的 `ScriptFilePattern` 常量即可（默认 `plugin-*.js`）。

## 脚本能用的全局

| 全局 | 说明 |
| --- | --- |
| `ServerHooks` | `ServerApi.Hooks`，脚本可 `ServerHooks.ServerJoin.Register(Plugin, onXxx)` 钩**任意** ServerApi 事件 |
| `Plugin` | 插件实例，作为 `Register(TerrariaPlugin, handler, priority)` 的 registrator |
| `Commands` | `Commands.ChatCommands`，脚本用 `command(...)` 构建命令后自行 `Add/Remove` |

## 脚本能用的宿主函数

| JS 函数 | 作用 |
| --- | --- |
| `log(消息)` | 控制台/日志输出 |
| `broadcast(消息)` | 服务器全服广播 |
| `sendMessage(玩家名, 消息)` / `sendError(玩家名, 消息)` | 给指定玩家发消息 |
| `giveItem(玩家名, 物品ID, 数量)` | 给玩家物品 |
| `spawnNpc(npcID, 格子X, 格子Y)` | 生成一个 NPC |
| `getOnlinePlayers()` | 返回在线玩家名字（逗号分隔） |
| `playerName(序号)` | 按玩家序号取名字 |
| `command(命令名, 权限, 帮助, 回调)` | 构建一个命令对象（用 `Commands.Add/Remove` 注册/移除） |

## 脚本示例

命令用 `command(...)` 构建（Jint 无法直接 `new Command`），然后用全局 `Commands` 自行 `Add/Remove`。
事件钩子用 `ServerHooks.<事件>.Register(Plugin, 回调)` **直接**钩真实 TShock 事件，回调参数是 TSAPI 的事件参数对象：

```javascript
var cmdHello;

function init() {
    cmdHello = command("hello", "", "打个招呼: /hello <名字>", onHello);
    if (cmdHello) Commands.Add(cmdHello);
    ServerHooks.ServerJoin.Register(Plugin, onServerJoin);
    ServerHooks.ServerChat.Register(Plugin, onServerChat);
}

function unload() {
    if (cmdHello) Commands.Remove(cmdHello);
    ServerHooks.ServerJoin.Deregister(Plugin, onServerJoin);
    ServerHooks.ServerChat.Deregister(Plugin, onServerChat);
}

function onHello(args) {
    sendMessage(args.PlayerName, "你好, " + (args.Parameters[0] || args.PlayerName) + "!");
}

function onServerJoin(args) {
    broadcast(playerName(args.Who) + " 加入了服务器!");
}

function onServerChat(args) {
    if (args.Text.indexOf("hello") >= 0) {
        sendMessage(playerName(args.Who), "你好呀!");
    }
}
```

## 重要说明

- **不要**用 `importNamespace("TerrariaApi.Server")`：Jint 4.16 反射 TerrariaServer.dll 会抛
  `Invalid generic type parameter on TerrariaApi.Server.Symbol(Symbol.toPrimitive)`。要访问 ServerApi 请用宿主暴露的 `ServerHooks` / `Plugin` 全局。
- 脚本**不设置超时**（是长期驻留的插件），只保留语句数上限与栈溢出保护两类安全阀。
- 钩子回调收到的是原始 TSAPI 事件对象（如 `args.Who`、`args.Text`、`args.npc`）。Jint 首次反射这类对象可能较慢，建议脚本里避免大量访问，用 `playerName(args.Who)` 等宿主函数取需要的信息。
- 钩子是脚本**直接**注册到真实 TShock 钩子的，宿主不追踪它们。因此：
  - 插件**停用**时，TShock 会按 registrator（`Plugin`）自动移除该插件注册的所有钩子；
  - 脚本**重载**时请务必在 `unload()` 里用同一 JS 函数反注册（如上例），否则旧钩子会残留。

## 进阶：事件订阅（Jint 里正确的写法）

**✅ 能用：`ServerHooks.X.Register(Plugin, fn)` / `Deregister`**（TSAPI 钩子事件，参数是 TSAPI 事件对象）：

```javascript
function init() {
    ServerHooks.ServerJoin.Register(Plugin, onServerJoin);
    ServerHooks.NetGetData.Register(Plugin, onNetGetData);
}
function unload() {
    ServerHooks.ServerJoin.Deregister(Plugin, onServerJoin);
    ServerHooks.NetGetData.Deregister(Plugin, onNetGetData);
}
function onServerJoin(args) { broadcast(playerName(args.Who) + " 加入了!"); }
function onNetGetData(args) { log("收包: " + args.MsgID + " 玩家=" + playerName(args.Index)); }
```

**❌ 做不了（Jint 的固有限制）**：

1. **`X += JS函数`** 不行。Jint 不支持对 .NET 事件/委托字段用 `+=` 挂一个 JS 函数：
   - 对**委托字段** `field += jsFunc` → 抛 `InvalidCastException`（且逃出 JS try/catch）。
   - 对**真正的事件** `event += jsFunc` → 不报错但**静默无效**。
   - 正确做法是用事件访问器：`add_事件名(fn)` / `remove_事件名(fn)` —— 详见下一条的"但"。

2. **`On.Terraria.*` 这类 MonoMod detour 在纯 JS 里挂不了**。会报
   `System.ArgumentException: Target method is static, but a target object was provided`。
   原因：`Player.AddBuff` 是**静态方法**，MonoMod 要求传入的 hook 委托 **Target==null**；而 Jint 把 JS 函数转成委托时**一定会带一个非空 Target**（指向引擎/JsValue），两者矛盾。所以 `On.Terraria.Player.add_AddBuff(onPlayerAddBuff)` 在真机上**必然失败**（这是我在 mock 里测通过、真机却失败的根因）。
   → 若要钩 `On.*` detour，只能加 **C# 桥**（C# 注册一个静态方法、Target==null，再由它转调 JS）。

3. **`out` / `ref` 参数** Jint 处理不了：
   - JS 调用带 `out`/`ref` 的 CLR 方法 → `JavaScriptException: No public methods with the specified arguments were found.`
   - 给带 `out`/`ref` 参数的委托/事件挂 JS 回调，回调能被调，但**写不回 out/ref**（C# 侧原值不变）。
   → 因此带 `out`/`ref` 的 detour（如 `On.Terraria.MessageBuffer.GetData` 的 `out int messageType`）在纯 JS 里也做不了，需 C# 桥处理。

**小结**：订阅事件优先用 `ServerHooks.X.Register(Plugin, fn)`；涉及 `On.*` detour 或 `out`/`ref` 的钩子需要 C# 桥。

## 管理命令

- `/escript reload` — 重新加载全部脚本（先卸载、再读取并编译变化的脚本、最后重新注册）。
- `/escript list` — 列出已加载的脚本。

> 权限：`economics.plugin.admin`。

## 构建产物提醒

构建后请把以下文件一起放进 TShock 的 `ServerPlugins`：`Economics.Plugin.dll`、`Economics.Script.dll`、
`Jint.dll`、`Acornima.dll`（后两者来自 NuGet，需作为独立程序集拷贝，**不要内嵌**，见 [Economics.Script](../Economics.Script/README.md)）。
