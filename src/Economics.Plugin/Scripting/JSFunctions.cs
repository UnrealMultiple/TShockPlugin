using Economics.Script;
using Jint.Native;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using TShockAPI;

namespace Economics.Plugin.Scripting;

/// <summary>
/// 暴露给 JS 脚本的宿主函数（脚本可用的 API）。
/// 这些是对服务端能力的最小封装，脚本用它们来实现插件的具体功能。
/// </summary>
public class JSFunctions
{
    [ScriptFunction("log")]
    public static void Log(object? message)
    {
        Console.WriteLine(message);
    }

    [ScriptFunction("broadcast")]
    public static void Broadcast(string message)
    {
        TShock.Utils.Broadcast(message, Color.Yellow);
    }

    [ScriptFunction("sendMessage")]
    public static void SendMessage(string playerName, string message)
    {
        FindPlayer(playerName)?.SendInfoMessage(message);
    }

    [ScriptFunction("sendError")]
    public static void SendError(string playerName, string message)
    {
        FindPlayer(playerName)?.SendErrorMessage(message);
    }

    [ScriptFunction("giveItem")]
    public static void GiveItem(string playerName, int itemId, int count = 1)
    {
        FindPlayer(playerName)?.GiveItem(itemId, count);
    }

    [ScriptFunction("spawnNpc")]
    public static int SpawnNpc(int npcId, float tileX, float tileY)
    {
        var index = NPC.NewNPC(new EntitySource_DebugCommand(), (int) (tileX * 16f), (int) (tileY * 16f), npcId);
        if (Main.netMode == 2)
        {
            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, index);
        }
        return index;
    }

    [ScriptFunction("getOnlinePlayers")]
    public static string GetOnlinePlayers()
    {
        return string.Join(", ", TShock.Players.Where(p => p is { Active: true }).Select(p => p.Name));
    }

    [ScriptFunction("playerName")]
    public static string PlayerName(int who)
    {
        Console.WriteLine(who);
        return (who >= 0 && who < Main.maxPlayers) ? TShock.Players[who]?.Name ?? string.Empty : string.Empty;
    }

    [ScriptFunction("command")]
    public static Command? Command(string name, string permission, string helpText, JsValue callback)
    {
        var host = ScriptHost.CurrentLoadingHost;
        var runtime = ScriptHost.CurrentLoadingRuntime;
        if (host is null || runtime is null)
        {
            Log("[Economics.Plugin] command(...) 只能在脚本加载阶段（init）里调用。");
            return null;
        }

        var functionName = callback.Get("name").ToString();
        if (string.IsNullOrEmpty(functionName) || functionName == "undefined")
        {
            Log($"[Economics.Plugin] 命令 '{name}' 的回调必须是命名函数。");
            return null;
        }

        return ScriptHost.BuildCommand(name, permission, helpText, runtime, functionName);
    }

    private static TSPlayer? FindPlayer(string name)
    {
        return string.IsNullOrEmpty(name) ? null : TShock.Players.FirstOrDefault(p => p is { } && p.Name == name);
    }
}
