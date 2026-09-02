using System.Collections.Concurrent;
using Economics.Script.Sources;

namespace Economics.Script;

/// <summary>
/// 脚本运行时的高层门面：按 <see cref="ScriptKey"/>（location + 执行模式）缓存
/// <see cref="ScriptRuntime"/>、懒加载、解析脚本 key、处理 reload 变更，并在停用时释放所有引擎。
/// 同一个 location 可以用不同的 <see cref="ExecutionMode"/> 各建一个运行时，互不影响。
/// </summary>
public sealed class ScriptManager : IDisposable
{
    private readonly ConcurrentDictionary<ScriptKey, ScriptRuntime> _runtimes = new();

    /// <summary>创建管理器时的配置。</summary>
    public ScriptEngineOptions Options { get; }

    public ScriptManager(ScriptEngineOptions options)
    {
        this.Options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>把一个逻辑 key（脚本文件名）解析为稳定 location。失败返回 <c>false</c>。</summary>
    public bool TryResolve(string key, out string location)
    {
        return this.Options.SourceProvider.TryResolve(key, out location);
    }

    /// <summary>取回（或按需创建）使用默认执行模式的运行时。</summary>
    public ScriptRuntime GetOrCreate(string location)
    {
        return this.GetOrCreate(location, this.Options.Mode);
    }

    /// <summary>
    /// 取回（或按需创建）某个 location + 指定执行模式的运行时。
    /// 源码的读取与编译是懒加载的，发生在第一次 <c>Invoke</c> 时。
    /// </summary>
    public ScriptRuntime GetOrCreate(string location, ExecutionMode mode)
    {
        return this._runtimes.GetOrAdd(new ScriptKey(location, mode), key => new ScriptRuntime(key.Location, key.Mode, this.Options));
    }

    /// <summary>解析 key 并调用默认入口函数、使用默认执行模式。解析失败返回 <c>null</c>。</summary>
    public object? Execute(string key, params object?[] args)
    {
        return this.Execute(key, this.Options.Mode, this.Options.EntryFunction, args);
    }

    /// <summary>解析 key 并调用默认入口函数、指定执行模式。解析失败返回 <c>null</c>。</summary>
    public object? Execute(string key, ExecutionMode mode, params object?[] args)
    {
        return this.Execute(key, mode, this.Options.EntryFunction, args);
    }

    /// <summary>解析 key 并调用指定入口函数、指定执行模式。解析失败返回 <c>null</c>。</summary>
    public object? Execute(string key, ExecutionMode mode, string entryFunction, params object?[] args)
    {
        return !this.TryResolve(key, out var location) ? null : this.GetOrCreate(location, mode).Invoke(entryFunction, args);
    }

    /// <summary>
    /// 解析 key 并用 <see cref="ScriptRuntime.TryInvoke"/> 调用一个可选事件钩子：
    /// 函数不存在、不可调用或调用失败时返回 <c>false</c>，不抛出、不刷屏。
    /// </summary>
    public bool TryInvoke(string key, string entryFunction, params object?[] args)
    {
        return this.TryResolve(key, out var location)
            && this.GetOrCreate(location).TryInvoke(entryFunction, args);
    }

    /// <summary>reload 时把全部已缓存运行时的源码变更标记为“脏”，下次调用重建。</summary>
    public void ReloadAll()
    {
        foreach (var runtime in this._runtimes.Values)
        {
            runtime.MarkDirtyIfChanged();
        }
    }

    /// <summary>reload 时调用：仅当源码版本变化时才把对应 location 的所有模式运行时标记为“脏”。</summary>
    public void Reload(string key)
    {
        if (!this.TryResolve(key, out var location))
        {
            return;
        }

        foreach (var runtime in this._runtimes.Values)
        {
            if (string.Equals(runtime.Location, location, StringComparison.OrdinalIgnoreCase))
            {
                runtime.MarkDirtyIfChanged();
            }
        }
    }

    /// <summary>reload 时调用：仅当源码版本变化时才把指定模式的运行时标记为“脏”。</summary>
    public void Reload(string key, ExecutionMode mode)
    {
        if (this.TryResolve(key, out var location) && this._runtimes.TryGetValue(new ScriptKey(location, mode), out var runtime))
        {
            runtime.MarkDirtyIfChanged();
        }
    }

    /// <summary>强制某个 key 的所有模式运行时下次调用时重新构建。</summary>
    public void Invalidate(string key)
    {
        if (!this.TryResolve(key, out var location))
        {
            return;
        }

        foreach (var runtime in this._runtimes.Values)
        {
            if (string.Equals(runtime.Location, location, StringComparison.OrdinalIgnoreCase))
            {
                runtime.Invalidate();
            }
        }
    }

    /// <summary>强制某个 key 的指定模式运行时下次调用时重新构建。</summary>
    public void Invalidate(string key, ExecutionMode mode)
    {
        if (this.TryResolve(key, out var location) && this._runtimes.TryGetValue(new ScriptKey(location, mode), out var runtime))
        {
            runtime.Invalidate();
        }
    }

    /// <summary>释放所有脚本引擎并清空缓存。</summary>
    public void Dispose()
    {
        foreach (var runtime in this._runtimes.Values)
        {
            runtime.Dispose();
        }
        this._runtimes.Clear();
    }

    /// <summary>缓存键：location + 执行模式。</summary>
    private readonly record struct ScriptKey(string Location, ExecutionMode Mode);
}
