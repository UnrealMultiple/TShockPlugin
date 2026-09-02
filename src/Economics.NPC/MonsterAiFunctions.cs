using Economics.Script;
using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace Economics.NPC;

/// <summary>
/// 暴露给怪物 AI 脚本的宿主函数。脚本里除了可用这些便捷函数外，
/// 还能通过 CLR 访问（<c>Terraria</c> / <c>TShockAPI</c> / <c>Economics</c>）直接操作
/// <c>Terraria.NPC</c>、<c>Terraria.Main</c> 等对象来修改怪物 AI（ai[]、velocity、position、aiStyle…）。
/// </summary>
public class MonsterAiFunctions
{
    /// <summary>向控制台输出一段文本。</summary>
    [ScriptFunction("Say")]
    public static void Say(object message)
    {
        Console.WriteLine(message);
    }

    /// <summary>向所有玩家广播一条消息。</summary>
    [ScriptFunction("Broadcast")]
    public static void Broadcast(string message, int r = 255, int g = 255, int b = 255)
    {
        TShock.Utils.Broadcast(message, (byte)r, (byte)g, (byte)b);
    }

    /// <summary>在指定像素坐标处召唤一个怪物。</summary>
    [ScriptFunction("SpawnNpc")]
    public static void SpawnNpc(int netId, float x, float y)
    {
        var t = Terraria.Utils.ToTileCoordinates(new Vector2(x, y));
        TSPlayer.Server.SpawnNPC(netId, netId.ToString(), 1, t.X, t.Y);
    }

    /// <summary>当前在线/活跃玩家数。</summary>
    [ScriptFunction("ActivePlayerCount")]
    public static int ActivePlayerCount()
    {
        return TShock.Utils.GetActivePlayerCount();
    }

    /// <summary>
    /// 用原版 <see cref="Terraria.Projectile.NewProjectile"/> 生成弹幕，来源用
    /// <see cref="Terraria.Projectile.GetNoneSource"/>，Owner 固定为 <see cref="Main.myPlayer"/>。
    /// 发射方向/速度由脚本自行计算后传入 <paramref name="vx"/>/<paramref name="vy"/>。
    /// 返回弹幕索引（失败为 -1）。
    /// </summary>
    [ScriptFunction("SpawnProjectile")]
    public static int SpawnProjectile(int npcIndex, float x, float y, float vx, float vy, int type, int damage, float knockback, float ai0 = 0, float ai1 = 0, float ai2 = 0)
    {
        return Terraria.Projectile.NewProjectile(
            Terraria.Projectile.GetNoneSource(),
            x, y, vx, vy, type, damage, knockback, Main.myPlayer, ai0, ai1, ai2);
    }
}
