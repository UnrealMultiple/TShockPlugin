using Economics.Skill.Attributes;
using Economics.Skill.Internal;
using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace Economics.Skill.JSInterpreter;

public class JSFunctions
{
    [JavaScriptFunction("print")]
    public static void JSPrint(string message)
    {
        Console.WriteLine(message);
    }

    [JavaScriptFunction("SpawnProjtile")]
    public static int JSProj(TSPlayer ply, Vector2 pos, Vector2 vel, int type, int Damage, int KnockBack, int Owner, float ai0 = 0, float ai1 = 0, float ai2 = 0, int timeLeft = -1, string uuid = "")
    {
        return EconomicsAPI.Utils.SpawnProjectile.NewProjectile(
                           //发射原无期
                           ply.TPlayer.GetProjectileSource_Item(ply.TPlayer.HeldItem),
                           //发射位置
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


    [JavaScriptFunction("SendProjectilePacket")]
    public static void SendProjectilePacket(int index)
    {
        TSPlayer.All.SendData(PacketTypes.ProjectileNew, "", index);
    }


    [JavaScriptFunction("SendPacket")]
    public static void SendPacket(int packetid, int num, int num2, int num3, int num4, int num5, int num6, int num7)
    {
        NetMessage.SendData(packetid, -1, -1, null, num, num2, num3, num4, num5, num6, num7);
    }

    [JavaScriptFunction("Schedule")]
    public static void Schedule(Action<object> action, int interval, object obj)
    {
        JobjManager.Delayed(interval, action, obj);
    }
}