using Economics.Script.Sources;
using Jint;
using Jint.Native;

namespace Economics.Script;

/// <summary>
/// 一个脚本的长期存活运行时：持有单个 <see cref="Engine"/> 与预编译脚本，
/// 负责按需读取源码、编译、执行顶层（<see cref="ExecutionMode.DefineOnce"/> 或
/// <see cref="ExecutionMode.SnapshotRestore"/>），并保证进入引擎的调用是线程安全的
/// （Engine 不是线程安全的，这里用 <see cref="_sync"/> 串行化）。
/// </summary>
public sealed class ScriptRuntime : IDisposable
{
    private readonly Lock _sync = new();
    private readonly ScriptEngineOptions _options;
    private readonly IScriptSourceProvider _provider;

    private Engine? _engine;
    private Prepared<Acornima.Ast.Script>? _prepared;
    private GlobalSnapshot? _snapshot;
    private string? _sourceFingerprint;
    private bool _loaded;
    private bool _buildFailed;
    private volatile bool _dirty = true;

    internal ScriptRuntime(string location, ExecutionMode mode, ScriptEngineOptions options)
    {
        this.Location = location;
        this.Mode = mode;
        this._options = options;
        this._provider = options.SourceProvider;
    }

    /// <summary>脚本的稳定 location（例如绝对路径）。</summary>
    public string Location { get; }

    /// <summary>该运行时的执行模式。</summary>
    public ExecutionMode Mode { get; }

    /// <summary>最近一次构建/执行的错误信息（用于排查），成功时为 <c>null</c>。</summary>
    public string? LastError { get; private set; }

    /// <summary>调用默认入口函数。</summary>
    public object? Invoke(params object?[] args)
    {
        return this.Invoke(this._options.EntryFunction, args);
    }

    /// <summary>调用指定入口函数，并把结果转换为 CLR 对象。函数不存在或调用失败时返回 <c>null</c> 并记录错误。</summary>
    public object? Invoke(string entryFunction, params object?[] args)
    {
        lock (this._sync)
        {
            this.EnsureBuilt();
            if (!this._loaded || this._engine is null)
            {
                return null;
            }

            try
            {
                return this.InvokeCore(entryFunction, args)?.ToObject();
            }
            catch (Exception ex)
            {
                this.LastError = ex.Message;
                this._options.ErrorHandler?.Invoke(this.Location, entryFunction, ex);
                return null;
            }
        }
    }

    /// <summary>
    /// 调用指定入口函数（用于可选的事件钩子）。若入口函数不存在、不可调用、未加载或调用失败，
    /// 返回 <c>false</c> 而不会抛出/刷屏。成功时返回 <c>true</c>。
    /// </summary>
    public bool TryInvoke(string entryFunction, params object?[] args)
    {
        lock (this._sync)
        {
            this.EnsureBuilt();
            if (!this._loaded || this._engine is null)
            {
                return false;
            }

            if (!this._engine.GetValue(entryFunction).IsCallable())
            {
                return false;
            }

            try
            {
                this.InvokeCore(entryFunction, args);
                return true;
            }
            catch (Exception ex)
            {
                this.LastError = ex.Message;
                this._options.ErrorHandler?.Invoke(this.Location, entryFunction, ex);
                return false;
            }
        }
    }

    private JsValue? InvokeCore(string entryFunction, object?[] args)
    {
        if (this.Mode == ExecutionMode.DefineOnce)
        {
            return this._engine!.Invoke(entryFunction, args);
        }

        if (this._snapshot is null)
        {
            return null;
        }

        var prepared = this._prepared ?? throw new InvalidOperationException("脚本尚未编译。");
        JsValue? result = null;
        this._engine!.Advanced.WithRestoredGlobals(this._snapshot, () =>
        {
            this._engine!.Execute(prepared);
            result = this._engine!.Invoke(entryFunction, args);
        });
        return result;
    }

    /// <summary>供 reload 使用：若源码版本发生变化则标记为“脏”，下次调用重新构建。</summary>
    internal void MarkDirtyIfChanged()
    {
        var current = this._provider.GetVersion(this.Location);
        if (this._sourceFingerprint is null || current != this._sourceFingerprint)
        {
            this._dirty = true;
        }
    }

    /// <summary>强制下次调用重新构建。</summary>
    internal void Invalidate()
    {
        this._dirty = true;
    }

    private void EnsureBuilt()
    {
        if (this._loaded && !this._dirty)
        {
            return;
        }

        // 上次构建失败且源码未变化时，不每次调用都重试；等待 reload 标记“脏”后再重建。
        if (this._buildFailed && !this._dirty)
        {
            return;
        }

        this.Build();
    }

    private void Build()
    {
        // 在锁内释放旧引擎，避免旧引擎在被并发使用时就被释放。
        this._engine?.Dispose();
        this._engine = null;
        this._prepared = null;
        this._snapshot = null;

        string raw;
        try
        {
            raw = this._provider.Read(this.Location);
        }
        catch (Exception ex)
        {
            this._loaded = false;
            this._buildFailed = true;
            this._dirty = false;
            this.LastError = $"{this.Location} 读取失败: {ex.Message}";
            this._options.ErrorHandler?.Invoke(this.Location, "read", ex);
            return;
        }

        this._sourceFingerprint = this._provider.GetVersion(this.Location);
        var source = this.Preprocess(raw);

        if (string.IsNullOrWhiteSpace(source))
        {
            this._loaded = false;
            this._buildFailed = true;
            this._dirty = false;
            this.LastError = $"{this.Location} 脚本内容为空";
            this._options.ErrorHandler?.Invoke(this.Location, "empty-script", null);
            return;
        }

        Engine? engine = null;
        try
        {
            engine = this.CreateEngine();
            this.RegisterHost(engine);
            var prepared = Engine.PrepareScript(source, this.Location, this._options.StrictMode, null);
            this._prepared = prepared;

            if (this.Mode == ExecutionMode.SnapshotRestore)
            {
                // 在脚本运行前捕获干净全局（只含宿主函数），之后每次调用都先恢复再重跑。
                this._snapshot = engine.Advanced.CaptureGlobalSnapshot();
            }
            else
            {
                engine.Execute(prepared); // 只执行一次顶层，定义入口与全局环境。
            }

            this._engine = engine;
            this._loaded = true;
            this._buildFailed = false;
            this.LastError = null;
        }
        catch (Exception ex)
        {
            engine?.Dispose();
            this._engine = null;
            this._prepared = null;
            this._snapshot = null;
            this._loaded = false;
            this._buildFailed = true;
            this.LastError = ex.Message;
            this._options.ErrorHandler?.Invoke(this.Location, "build", ex);
        }
        finally
        {
            this._dirty = false;
        }
    }

    private string Preprocess(string source)
    {
        foreach (var preprocessor in this._options.Preprocessors)
        {
            source = preprocessor.Preprocess(source);
        }
        return source;
    }

    private Engine CreateEngine()
    {
        return new Engine(options =>
        {
            if (this._options.AllowClr)
            {
                if (this._options.ClrAssemblies.Count > 0)
                {
                    options.AllowClr(this._options.ClrAssemblies.ToArray());
                }
                else
                {
                    options.AllowClr();
                }
            }

            if (this._options.ExtensionMethodTypes.Count > 0)
            {
                options.AddExtensionMethods(this._options.ExtensionMethodTypes.ToArray());
            }

            if (this._options.Timeout is { } timeout)
            {
                options.TimeoutInterval(timeout);
            }

            if (this._options.MaxStatements is { } maxStatements)
            {
                options.MaxStatements(maxStatements);
            }

            if (this._options.MemoryLimit is { } memoryLimit)
            {
                options.LimitMemory(memoryLimit);
            }

            if (this._options.StackOverflowGuard)
            {
                options.Constraints.StackOverflowGuard = true;
            }
        });
    }

    private void RegisterHost(Engine engine)
    {
        foreach (var function in this._options.Functions)
        {
            engine.SetValue(function.Name, function.Handler);
        }

        foreach (var global in this._options.Globals)
        {
            engine.SetValue(global.Key, global.Value);
        }
    }

    public void Dispose()
    {
        lock (this._sync)
        {
            this._engine?.Dispose();
            this._engine = null;
            this._prepared = null;
            this._snapshot = null;
            this._loaded = false;
        }
    }
}
