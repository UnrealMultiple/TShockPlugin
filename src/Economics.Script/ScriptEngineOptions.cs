using Economics.Script.Preprocessing;
using Economics.Script.Sources;
using System.Linq.Expressions;
using System.Reflection;

namespace Economics.Script;

/// <summary>
/// 脚本引擎的配置（fluent/builder 模式）。用链式方法设置完毕后，
/// 交给 <see cref="ScriptManager"/> 使用。可被多个运行时共享。
/// </summary>
public sealed class ScriptEngineOptions
{
    private readonly List<Assembly> _clrAssemblies = [];
    private readonly List<Type> _extensionMethods = [];
    private readonly List<ScriptFunctionRegistration> _functions = [];
    private readonly List<IScriptPreprocessor> _preprocessors = [];
    private readonly Dictionary<string, object?> _globals = new(StringComparer.Ordinal);

    /// <summary>是否允许脚本访问 CLR 类型（<c>importNamespace</c> / <c>new</c> 等）。</summary>
    public bool AllowClr { get; private set; } = true;

    /// <summary>允许脚本访问 CLR 的程序集。</summary>
    public IReadOnlyList<Assembly> ClrAssemblies => this._clrAssemblies;

    /// <summary>为脚本注册的扩展方法所属类型。</summary>
    public IReadOnlyList<Type> ExtensionMethodTypes => this._extensionMethods;

    /// <summary>已注册的宿主函数。</summary>
    public IReadOnlyList<ScriptFunctionRegistration> Functions => this._functions;

    /// <summary>脚本全局变量。</summary>
    public IReadOnlyDictionary<string, object?> Globals => this._globals;

    /// <summary>脚本预处理器。</summary>
    public IReadOnlyList<IScriptPreprocessor> Preprocessors => this._preprocessors;

    /// <summary>单次调用的最大执行时长。为 <c>null</c> 时不限制。</summary>
    public TimeSpan? Timeout { get; private set; }

    /// <summary>单次调用允许执行的语句数量上限。为 <c>null</c> 时不限制。</summary>
    public int? MaxStatements { get; private set; }

    /// <summary>单次调用允许分配的内存上限（字节）。为 <c>null</c> 时不限制。</summary>
    public long? MemoryLimit { get; private set; }

    /// <summary>是否启用原生栈溢出保护（防递归爆栈把进程带崩）。</summary>
    public bool StackOverflowGuard { get; private set; }

    /// <summary>是否以严格模式编译脚本（性能更好，但会禁止部分宽松写法）。</summary>
    public bool StrictMode { get; private set; }

    /// <summary>执行模式：<see cref="ExecutionMode.DefineOnce"/> 或 <see cref="ExecutionMode.SnapshotRestore"/>。</summary>
    public ExecutionMode Mode { get; private set; } = ExecutionMode.DefineOnce;

    /// <summary>默认入口函数名。</summary>
    public string EntryFunction { get; private set; } = "main";

    /// <summary>源码来源。</summary>
    public IScriptSourceProvider SourceProvider { get; private set; } = new FileScriptSourceProvider(Environment.CurrentDirectory);

    /// <summary>出错时的回调（location, 阶段/入口名, 异常）。为 <c>null</c> 时静默。</summary>
    public Action<string, string?, Exception?>? ErrorHandler { get; private set; }

    // ---- fluent 配置 ------------------------------------------------

    /// <summary>允许 CLR 并限定到指定程序集。</summary>
    public ScriptEngineOptions AllowClrWith(params Assembly[] assemblies)
    {
        this.AllowClr = true;
        this._clrAssemblies.AddRange(assemblies);
        return this;
    }

    /// <summary>禁用 CLR 访问。</summary>
    public ScriptEngineOptions DisableClr()
    {
        this.AllowClr = false;
        this._clrAssemblies.Clear();
        return this;
    }

    /// <summary>注册扩展方法类型。</summary>
    public ScriptEngineOptions AddExtensionMethods(params Type[] types)
    {
        if (types is not null)
        {
            this._extensionMethods.AddRange(types);
        }
        return this;
    }

    /// <summary>直接注册一个宿主函数（委托）。</summary>
    public ScriptEngineOptions AddFunction(string name, Delegate handler)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("函数名不能为空。", nameof(name));
        }

        this._functions.Add(new ScriptFunctionRegistration(name, handler ?? throw new ArgumentNullException(nameof(handler))));
        return this;
    }

    /// <summary>
    /// 扫描 <typeparamref name="THost"/> 中所有标记了 <see cref="ScriptFunctionAttribute"/>
    /// 的静态方法并注册为宿主函数。
    /// </summary>
    public ScriptEngineOptions RegisterFunctions<THost>() where THost : class
    {
        foreach (var method in typeof(THost).GetMethods())
        {
            var attribute = method.GetCustomAttribute<ScriptFunctionAttribute>();
            if (attribute is null || !method.IsStatic)
            {
                continue;
            }

            var delegateType = Expression.GetDelegateType(
                [.. method.GetParameters().Select(x => x.ParameterType), method.ReturnType]);
            this._functions.Add(new ScriptFunctionRegistration(attribute.Name, method.CreateDelegate(delegateType, null)));
        }
        return this;
    }

    /// <summary>设置一个脚本全局变量。</summary>
    public ScriptEngineOptions SetGlobal(string name, object? value)
    {
        this._globals[name] = value;
        return this;
    }

    /// <summary>设置单次调用超时。</summary>
    public ScriptEngineOptions SetTimeout(TimeSpan timeout)
    {
        this.Timeout = timeout;
        return this;
    }

    /// <summary>设置单次调用语句上限。</summary>
    public ScriptEngineOptions SetMaxStatements(int maxStatements)
    {
        this.MaxStatements = maxStatements;
        return this;
    }

    /// <summary>设置单次调用内存上限（字节）。</summary>
    public ScriptEngineOptions SetMemoryLimit(long bytes)
    {
        this.MemoryLimit = bytes;
        return this;
    }

    /// <summary>启用/停用原生栈溢出保护。</summary>
    public ScriptEngineOptions EnableStackOverflowGuard(bool enabled = true)
    {
        this.StackOverflowGuard = enabled;
        return this;
    }

    /// <summary>设置严格模式。</summary>
    public ScriptEngineOptions SetStrictMode(bool strict = true)
    {
        this.StrictMode = strict;
        return this;
    }

    /// <summary>设置执行模式。</summary>
    public ScriptEngineOptions SetExecutionMode(ExecutionMode mode)
    {
        this.Mode = mode;
        return this;
    }

    /// <summary>设置默认入口函数名。</summary>
    public ScriptEngineOptions SetEntryFunction(string name)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            this.EntryFunction = name;
        }
        return this;
    }

    /// <summary>使用文件系统作为源码来源。</summary>
    public ScriptEngineOptions UseFileSource(string rootDirectory)
    {
        this.SourceProvider = new FileScriptSourceProvider(rootDirectory);
        return this;
    }

    /// <summary>使用自定义源码来源。</summary>
    public ScriptEngineOptions UseSourceProvider(IScriptSourceProvider provider)
    {
        this.SourceProvider = provider ?? throw new ArgumentNullException(nameof(provider));
        return this;
    }

    /// <summary>添加一个预处理器。</summary>
    public ScriptEngineOptions AddPreprocessor(IScriptPreprocessor preprocessor)
    {
        this._preprocessors.Add(preprocessor ?? throw new ArgumentNullException(nameof(preprocessor)));
        return this;
    }

    /// <summary>设置出错回调。</summary>
    public ScriptEngineOptions SetErrorHandler(Action<string, string?, Exception?> handler)
    {
        this.ErrorHandler = handler;
        return this;
    }
}
