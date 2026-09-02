using Economics.Script;
using Economics.Script.Preprocessing;
using System.Collections.Concurrent;
using Terraria;
using TShockAPI;

namespace Economics.NPC;

/// <summary>
/// 用 JS 修改怪物 AI 行为的宿主。
/// <para>
/// 目录：<see cref="ScriptsDir"/>。使用者只需放一个以怪物 netID 命名的脚本文件，
/// 例如怪物 netID 为 123 就放 <c>123.js</c>。脚本内可定义（都可省略）：
/// </para>
/// <list type="bullet">
/// <item><c>onSpawn(npc)</c> —— 该怪物生成时。</item>
/// <item><c>ai(npc, index, time, struck)</c> —— 每帧调用，用于修改 AI（改 npc.ai[]/velocity/position/aiStyle 等）。</item>
/// <item><c>onStrike(npc, damage)</c> —— 被玩家命中时。</item>
/// <item><c>onKill(npc)</c> —— 被击杀时。</item>
/// </list>
/// <para>
/// 脚本通过 <see cref="MonsterAiFunctions"/> 与 CLR 访问（Terraria/TShock/Economics）来执行任意逻辑。
/// </para>
/// </summary>
public static class MonsterScripts
{
    /// <summary>怪物 AI 脚本目录。</summary>
    public static readonly string ScriptsDir = Path.Combine(Core.Economics.SaveDirPath, "NPCJSScripts");

    private static readonly ScriptManager Manager = CreateManager();
    private static readonly ConcurrentDictionary<int, NpcAiState> Tracked = new();

    static MonsterScripts()
    {
        if (!Directory.Exists(ScriptsDir))
        {
            Directory.CreateDirectory(ScriptsDir);
        }
    }

    /// <summary>是否启用 JS 怪物 AI。</summary>
    public static bool Enabled { get; set; } = true;

    /// <summary>确保脚本目录存在（幂等，可在插件 Initialize 时调用）。</summary>
    public static void EnsureCreated() => Directory.CreateDirectory(ScriptsDir);

    /// <summary>插件停用时释放所有脚本引擎。</summary>
    public static void Dispose() => Manager.Dispose();

    /// <summary>reload 所有怪物脚本（下次帧生效，仅重编译变化的文件）。</summary>
    public static void Reload() => Manager.ReloadAll();

    private static ScriptManager CreateManager()
    {
        var options = new ScriptEngineOptions()
            .AllowClrWith(
                typeof(Main).Assembly,
                typeof(TShock).Assembly,
                typeof(Core.Economics).Assembly,
                typeof(List<>).Assembly,
                typeof(Task).Assembly)
            .AddExtensionMethods(
                typeof(Core.Extensions.Vector2Extension),
                typeof(Core.Extensions.NpcExtension),
                typeof(Core.Extensions.PlayerExtension),
                typeof(Enumerable))
            .RegisterFunctions<MonsterAiFunctions>()
            .AddPreprocessor(RequireDirectivePreprocessor.Instance)
            .UseFileSource(ScriptsDir)
            .SetExecutionMode(ExecutionMode.DefineOnce)
            .SetTimeout(TimeSpan.FromSeconds(2))
            .SetMaxStatements(200_000)
            .EnableStackOverflowGuard()
            .SetErrorHandler((location, phase, ex) =>
                TShock.Log.ConsoleError($"[{location}] {phase} 错误：" + ex));

        return new ScriptManager(options);
    }

    private static string KeyFor(int netId) => netId + ".js";

    private static bool HasScript(int netId) => File.Exists(Path.Combine(ScriptsDir, KeyFor(netId)));

    /// <summary>怪物生成时调用。</summary>
    public static void OnNpcSpawn(int npcId)
    {
        if (!Enabled)
        {
            return;
        }

        var npc = Main.npc[npcId];
        if (npc == null || !npc.active || !HasScript(npc.netID))
        {
            return;
        }

        var key = KeyFor(npc.netID);
        if (!Manager.TryResolve(key, out var location))
        {
            return;
        }

        var state = new NpcAiState { Runtime = Manager.GetOrCreate(location) };
        Tracked[npcId] = state;
        state.Runtime.TryInvoke("onSpawn", npc);
    }

    /// <summary>每帧更新：对每个带脚本的存活怪物调用 <c>ai</c>。</summary>
    public static void OnUpdate()
    {
        if (!Enabled)
        {
            return;
        }

        const float dt = 1f / 60f;
        foreach (var pair in Tracked)
        {
            var npc = Main.npc[pair.Key];
            if (npc == null || !npc.active)
            {
                Tracked.TryRemove(pair.Key, out _);
                continue;
            }

            var state = pair.Value;
            state.Time += dt;
            state.Runtime?.TryInvoke("ai", npc, npc.whoAmI, state.Time, state.Struck);
        }
    }

    /// <summary>怪物被命中时调用。</summary>
    public static void OnNpcStrike(Terraria.NPC npc, int damage)
    {
        if (!Enabled || npc == null)
        {
            return;
        }

        if (!Tracked.TryGetValue(npc.whoAmI, out var state))
        {
            return;
        }

        state.Struck++;
        state.Runtime?.TryInvoke("onStrike", npc, damage);
    }

    /// <summary>怪物被击杀时调用。</summary>
    public static void OnNpcKilled(Terraria.NPC npc)
    {
        if (!Enabled || npc == null || !Tracked.TryGetValue(npc.whoAmI, out var state))
        {
            return;
        }

        state.Runtime?.TryInvoke("onKill", npc);
        Tracked.TryRemove(npc.whoAmI, out _);
    }
}

/// <summary>某个怪物实例的运行时状态（供 <c>ai</c> 使用：存活/被击等统计）。</summary>
public sealed class NpcAiState
{
    /// <summary>自生成以来的存活秒数。</summary>
    public float Time { get; set; }

    /// <summary>被玩家命中的次数。</summary>
    public int Struck { get; set; }

    /// <summary>该怪物对应的脚本运行时（内部缓存，避免每帧重复解析）。</summary>
    internal ScriptRuntime? Runtime { get; set; }
}
