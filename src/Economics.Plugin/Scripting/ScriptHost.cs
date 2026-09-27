using Economics.Script;
using Economics.Script.Preprocessing;
using TerrariaApi.Server;
using TShockAPI;

namespace Economics.Plugin.Scripting;

public sealed class ScriptHost : IDisposable
{
    public const string EntryFunction = "init";
    public const string ExitFunction = "unload";
    public static readonly string ScriptsDir = Path.Combine(TShock.SavePath, "EconomicsPlugin", "Scripts");
    public const string ScriptFilePattern = "plugin-*.js";

    internal static ScriptHost? CurrentLoadingHost { get; private set; }
    internal static ScriptRuntime? CurrentLoadingRuntime { get; private set; }

    private readonly TerrariaPlugin _plugin;
    private readonly ScriptManager _manager;
    private readonly List<string> _locations = [];

    public ScriptHost(TerrariaPlugin plugin)
    {
        this._plugin = plugin ?? throw new ArgumentNullException(nameof(plugin));
        this._manager = new ScriptManager(this.CreateOptions());
    }

    public IReadOnlyList<string> LoadedScripts => this._locations;

    private ScriptEngineOptions CreateOptions()
    {
        return new ScriptEngineOptions()
            .AllowClrWith(
                typeof(TShock).Assembly,
                typeof(Terraria.Main).Assembly,
                typeof(On.Terraria.Main).Assembly,
                typeof(ServerApi).Assembly,
                typeof(Enumerable).Assembly,
                typeof(List<>).Assembly)
            .AddExtensionMethods(typeof(Enumerable))
            .RegisterFunctions<JSFunctions>()
            .AddPreprocessor(RequireDirectivePreprocessor.Instance)
            .UseFileSource(ScriptsDir)
            .SetEntryFunction(EntryFunction)
            .SetExecutionMode(ExecutionMode.DefineOnce)
            .SetMaxStatements(1_000_000)
            .EnableStackOverflowGuard()
            .SetGlobal("Plugin", this._plugin)
            .SetGlobal("ServerHooks", ServerApi.Hooks)
            .SetGlobal("Commands", Commands.ChatCommands)
            .SetErrorHandler(ReportError);
    }

    public void LoadAll()
    {
        Directory.CreateDirectory(ScriptsDir);
        foreach (var file in Directory.GetFiles(ScriptsDir, ScriptFilePattern))
        {
            this.Load(file);
        }
    }

    private void Load(string location)
    {
        var runtime = this._manager.GetOrCreate(location);
        CurrentLoadingHost = this;
        CurrentLoadingRuntime = runtime;
        try
        {
            runtime.Invoke(EntryFunction);
        }
        finally
        {
            CurrentLoadingHost = null;
            CurrentLoadingRuntime = null;
        }

        if (!this._locations.Any(x => string.Equals(x, location, StringComparison.OrdinalIgnoreCase)))
        {
            this._locations.Add(location);
        }
    }

    public static Command BuildCommand(string name, string permission, string helpText, ScriptRuntime runtime, string functionName)
    {
        var commandName = name.TrimStart('/');
        var permissions = string.IsNullOrWhiteSpace(permission)
            ? []
            : new List<string> { permission };

        return new Command(permissions, args => runtime.Invoke(functionName, new ScriptCommandArgs(args)), commandName)
        {
            HelpText = helpText
        };
    }

    public void Reload()
    {
        this.UnloadAll();
        this._locations.Clear();
        this._manager.ReloadAll();
        this.LoadAll();
    }

    private void UnloadAll()
    {
        foreach (var location in this._locations)
        {
            this._manager.GetOrCreate(location).TryInvoke(ExitFunction);
        }
    }

    public void Dispose()
    {
        this.UnloadAll();
        this._manager.Dispose();
    }

    private static void ReportError(string location, string? phase, Exception? ex)
    {
        if (ex is null)
        {
            TShock.Log.Error($"[Economics.Plugin] ({location}) {phase}: 脚本未加载或内容为空。");
        }
        else
        {
            TShock.Log.ConsoleError($"[Economics.Plugin] ({location}) {phase} 错误: " + ex);
        }
    }
}
