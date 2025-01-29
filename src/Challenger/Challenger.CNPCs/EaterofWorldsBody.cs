using Challenger;
using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace ChalleAnger;

public class EaterofWorldsBody : CNPC
{
    public static int State;

    public EaterofWorldsBody(NPC npc)
        : base(npc)
    {
    }

    public override void NPCAI()
    {
        this.SetState();
        var targetData = this.npc.GetTargetData();
        switch (State)
        {
            case 1:
                if (Vector2.DistanceSquared(targetData.Center, this.npc.Center) <= 250000f && Main.rand.Next(1200) == 0)
                {
                    Projectile.NewProjectile(null, this.npc.Center, -Vector2.UnitY * 6f, 671, 8, 0f);
                }
                break;
            case 2:
                if (Vector2.DistanceSquared(targetData.Center, this.npc.Center) <= 490000f && Main.rand.Next(800) == 0)
                {
                    Projectile.NewProjectile(null, this.npc.Center, -Vector2.UnitY * 12f, 671, 14, 0f);
                }
                break;
            case 3:
                if (Vector2.DistanceSquared(targetData.Center, this.npc.Center) <= 640000f && Main.rand.Next(300) == 0)
                {
                    if (Main.rand.Next(2) == 0)
                    {
                        Projectile.NewProjectile(null, this.npc.Center, this.npc.Center.DirectionTo(targetData.Center).RotatedByRandom(0.1) * 12f, 671, 17, 0f);
                    }
                    else
                    {
                        Projectile.NewProjectile(null, this.npc.Center, -Vector2.UnitY.RotatedByRandom(0.2) * 10f, 671, 17, 0f);
                    }
                }
                break;
        }
    }

    public override int SetState()
    {
        var num = 0;
        var array = Main.npc;
        foreach (var val in array)
        {
            if ((val.type == 13 || val.type == 14 || val.type == 15) && val.active)
            {
                num++;
            }
        }
        if (num > 66)
        {
            if (State == 0)
            {
                State = 1;
                if (Challenger.Challenger.Config.EnableBroadcastConsumptionMode)
                {
                    TSPlayer.All.SendMessage(GetString("邪恶的蠕虫寻找新的受害者"), new (177, 94, 255));
                }
            }
            return State;
        }
        if (num > 40)
        {
            if (State == 1)
            {
                State = 2;
                if (Challenger.Challenger.Config.EnableBroadcastConsumptionMode)
                {
                    TSPlayer.All.SendMessage(GetString("有毒的鳞甲炸裂开来"), new (177, 94, 255));
                }
            }
            return State;
        }
        if (State == 2)
        {
            State = 3;
            if (Challenger.Challenger.Config.EnableBroadcastConsumptionMode)
            {
                TSPlayer.All.SendMessage(GetString("怒不可遏"), new (177, 94, 255));
            }
        }
        return State;
    }

    public override void OnKilled()
    {
        if (State == 0 || State == 2)
        {
            var num = Projectile.NewProjectile(null, this.npc.Center, Vector2.Zero, 501, 13, 0f);
            Main.npc[num].timeLeft = 1;
            CProjectile.Update(num);
            for (var i = 0; i < 6; i++)
            {
                Projectile.NewProjectile(null, this.npc.Center, Vector2.UnitY.RotatedBy(Math.PI / 3.0 * i) * 5f, 909, 14, 0f);
            }
        }
    }

    public override void OnHurtPlayers(GetDataHandlers.PlayerDamageEventArgs e)
    {
        if (Challenger.Challenger.Config.EnableConsumptionMode)
        {
            var num = Main.rand.Next(1, 3);
            if (num == 1)
            {
                Challenger.Challenger.SendPlayerText(GetString("刺啦"), new (177, 94, 255), this.npc.Center + new Vector2(0f, -30f));
            }
            else
            {
                Challenger.Challenger.SendPlayerText(GetString("小心我爆炸的鳞甲"), new (177, 94, 255), this.npc.Center + new Vector2(0f, -30f));
            }
        }
    }
}