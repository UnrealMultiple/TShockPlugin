using Economics.Core.Utility;
using Economics.Core.Utils;
using Economics.Script;
using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace Economics.Skill.Scripting;

/// <summary>
/// 暴露给技能脚本的宿主函数。这些函数依赖 Terraria / TShock / Economics API，
/// 因此放在 Skill 插件内，通过 <see cref="ScriptFunctionAttribute"/> 标记后
/// 由 <see cref="Economics.Script.ScriptEngineOptions.RegisterFunctions{THost}"/> 注册进引擎。
/// </summary>
public class JSFunctions
{
    [ScriptFunction("log")]
    public static void JSPrint(object message)
    {
        Console.WriteLine(message);
    }

    [ScriptFunction("SpawnProjtile")]
    public static int JSProj(TSPlayer ply, Vector2 pos, Vector2 vel, int type, int Damage, int KnockBack, int Owner, float ai0 = 0, float ai1 = 0, float ai2 = 0, int timeLeft = -1, string uuid = "")
    {
        return SpawnProjectile.NewProjectile(
            ply.TPlayer.GetProjectileSource_Item(ply.TPlayer.HeldItem),
            pos,
            vel,
            type,
            Damage,
            KnockBack,
            Owner,
            ai0,
            ai1,
            ai2,
            timeLeft,
            uuid);
    }

    [ScriptFunction("SendProjectilePacket")]
    public static void SendProjectilePacket(int index)
    {
        TSPlayer.All.SendData(PacketTypes.ProjectileNew, "", index);
    }

    [ScriptFunction("range")]
    public static IEnumerable<int> GenerateRange(int start, int end)
    {
        return Enumerable.Range(start, end);
    }

    [ScriptFunction("SendPacket")]
    public static void SendPacket(int packetid, int num, int num2, int num3, int num4, int num5, int num6, int num7)
    {
        NetMessage.SendData(packetid, -1, -1, null, num, num2, num3, num4, num5, num6, num7);
    }

    [ScriptFunction("Schedule")]
    public static void Schedule(Action action, int interval)
    {
        TimingUtils.Delayed(interval, action);
    }

    [ScriptFunction("FrameTimer")]
    public static FrameTimer CreateFrameTimer(int intervalFrames, bool triggerOnFirstTick = false)
    {
        return new FrameTimer(intervalFrames, triggerOnFirstTick);
    }
}
