using Economics.Script;
using Economics.Script.Preprocessing;
using Economics.Skill.Model;
using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace Economics.Skill.Scripting;

/// <summary>
/// 技能脚本运行时的宿主入口：安装 <see cref="Economics.Script"/> 库，
/// 注入 Skill 插件自己的程序集、扩展方法与宿主函数（<see cref="JSFunctions"/>）。
/// 根据技能的 <see cref="SkillContext.ResetVariables"/>（是否重置变量）为每个技能选择执行模式：
/// <see cref="ExecutionMode.DefineOnce"/>（不重置，变量保留）或
/// <see cref="ExecutionMode.SnapshotRestore"/>（重置，调用前恢复干净全局快照）。
/// 单个 <see cref="ScriptManager"/> 会按（location + 模式）缓存各自的运行时，互不影响。
/// </summary>
public static class SkillScripts
{
    public static readonly string ScriptsDir = Path.Combine(Core.Economics.SaveDirPath, "SkillScripts");
    public static readonly ScriptManager Manager = CreateManager();

    static SkillScripts()
    {
        if (!Directory.Exists(ScriptsDir))
        {
            Directory.CreateDirectory(ScriptsDir);
        }
    }

    /// <summary>确保脚本目录存在（幂等，可在插件 Initialize 时调用）。</summary>
    public static void EnsureCreated()
    {
        Directory.CreateDirectory(ScriptsDir);
    }

    private static ScriptManager CreateManager()
    {
        var options = new ScriptEngineOptions()
            .AllowClrWith(
                typeof(Core.Economics).Assembly,
                typeof(TShock).Assembly,
                typeof(Task).Assembly,
                typeof(List<>).Assembly,
                typeof(Main).Assembly)
            .AddExtensionMethods(
                typeof(Core.Extensions.Vector2Extension),
                typeof(Core.Extensions.GameProgress),
                typeof(Terraria.Utils),
                typeof(Core.Extensions.PlayerExtension),
                typeof(Core.Extensions.NpcExtension),
                typeof(Enumerable),
                typeof(Core.Extensions.TSPlayerExtension))
            .RegisterFunctions<JSFunctions>()
            .AddPreprocessor(RequireDirectivePreprocessor.Instance)
            .UseFileSource(ScriptsDir)
            .SetExecutionMode(ExecutionMode.DefineOnce)
            .SetTimeout(TimeSpan.FromSeconds(10))
            .SetMaxStatements(1_000_000)
            .EnableStackOverflowGuard()
            .SetErrorHandler(ReportError);

        return new ScriptManager(options);
    }

    private static ExecutionMode ModeFor(SkillContext skill)
    {
        return skill.ResetVariables ? ExecutionMode.SnapshotRestore : ExecutionMode.DefineOnce;
    }

    /// <summary>释放技能对应的脚本。</summary>
    public static void Execute(SkillContext skill, TSPlayer player, Vector2 pos, Vector2 vel, int level, int index = -1)
    {
        var location = skill.ScriptLocation;
        if (string.IsNullOrEmpty(location))
        {
            return;
        }

        Manager.GetOrCreate(location, ModeFor(skill)).Invoke("main", skill, player, pos, vel, level, index);
    }

    /// <summary>reload 时通知该脚本所有模式的运行时：仅当文件变化时才重新构建。</summary>
    public static void Reload(string key)
    {
        Manager.Reload(key);
    }

    /// <summary>reload：标记所有已加载的脚本为“脏”，下次触发时重新读取并编译变化的部分。</summary>
    public static void Reload()
    {
        Manager.ReloadAll();
    }

    /// <summary>插件停用时释放所有脚本引擎。</summary>
    public static void Dispose()
    {
        Manager.Dispose();
    }

    private static void ReportError(string location, string? phase, Exception? ex)
    {
        if (ex is null)
        {
            TShock.Log.Error($"[{location}] {phase}: 脚本内容为空或未加载。");
        }
        else
        {
            TShock.Log.ConsoleError($"[{location}] {phase} 错误：" + ex);
        }
    }
}
