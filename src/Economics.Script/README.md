# Economics.Script

一个基于 [Jint](https://github.com/sebastienros/jint) 的、独立、可复用的 JavaScript 脚本执行库。
它只负责“怎么把 JS 跑起来”，不依赖 Terraria / TShock / Economics API，宿主（如 Economics.Skill）通过配置把
自己的程序集、扩展方法、宿主函数与脚本来源注入进来。

> **依赖说明**：本库依赖 `Jint` + `Acornima`，它们以**普通程序集文件**（拷贝到输出目录）的形式提供，
> 而不是内嵌进本库——因为 TShock 的插件加载器在加载时会枚举本库的类型，若把 Jint 内嵌则无法在那一刻解析到，
> 会导致插件加载失败（`Could not load file or assembly 'Jint...'`）。使用方引用本库后，
> 确保输出目录里有 `Jint.dll` 与 `Acornima.dll` 即可（项目默认会拷贝）。

## 特性

- **引擎复用 + 预编译**：每个脚本一个长期存活的 `Engine`，用 `Engine.PrepareScript` 只编译一次；
  每次调用只 `Invoke` 入口，避免了“每次触发都新建引擎、重新解析源码”。
- **线程安全**：`Engine` 不是线程安全的，库用锁把每个脚本的调用串行化。
- **可切换执行语义**：
  - `DefineOnce`：顶层代码只执行一次，顶层全局在多次调用间保留。
  - `SnapshotRestore`：每次调用前恢复干净全局（`CaptureGlobalSnapshot`/`WithRestoredGlobals`），
    顶层状态每次归零（不重新解析）。
- **宽松 / 严格模式**、**超时 / 语句上限 / 内存上限 / 原生栈溢出保护** 均可配置。
- **插件式扩展**：
  - 宿主函数：`AddFunction(name, delegate)` 或 `RegisterFunctions<THost>()`（扫描 `[ScriptFunction]`）。
  - 源码来源：`IScriptSourceProvider`（默认文件系统，可换成资源/数据库/远程）。
  - 预处理：`IScriptPreprocessor`（默认去除 `@require` 指令）。
- **懒加载 + reload 感知**：首次调用才读取并编译；`Reload(key)` 只在源码版本变化时重建。

## 快速上手

```csharp
var options = new ScriptEngineOptions()
    .AllowClrWith(typeof(MyApi).Assembly)                 // 允许脚本访问 CLR 类型
    .AddExtensionMethods(typeof(MyExtensions))            // 暴露扩展方法
    .RegisterFunctions<MyHostFunctions>()                 // 扫描 [ScriptFunction] 静态方法
    .AddFunction("log", new Action<object>(Console.WriteLine))
    .UseFileSource(@"D:\scripts")                          // 脚本目录
    .SetExecutionMode(ExecutionMode.DefineOnce)           // 默认即 DefineOnce
    .SetTimeout(TimeSpan.FromSeconds(10))
    .SetMaxStatements(1_000_000)
    .EnableStackOverflowGuard()
    .SetErrorHandler((location, phase, ex) => Console.Error.WriteLine($"[{location}] {phase}: {ex}"));

using var manager = new ScriptManager(options);

// 运行脚本并调用默认入口（main）
manager.Execute("my-script.js", arg1, arg2);

// 指定入口 + 指定执行模式（变量是否重置按模式区分，不同模式各自独立缓存）
manager.Execute("my-script.js", ExecutionMode.SnapshotRestore, "setup", arg1);

// reload：仅当文件变化时才重新编译
manager.Reload("my-script.js");

// 手动获取运行时（可按 location + 模式）
var runtime = manager.GetOrCreate("D:\\scripts\\my-script.js");
runtime.Invoke("main", arg1);
```

## 宿主函数

用 `[ScriptFunction("名字")]` 标记静态方法，再 `RegisterFunctions<THost>()`：

```csharp
public static class MyHostFunctions
{
    [ScriptFunction("log")]
    public static void Log(object message) => Console.WriteLine(message);
}
```

## 脚本格式

约定默认入口为 `main`（可用 `SetEntryFunction` 改）：

```javascript
function main(a, b) {
    return a + b;
}
```

`@require "xxx";` 等指令会被默认预处理器注释掉。
