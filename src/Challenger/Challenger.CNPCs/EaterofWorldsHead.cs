using ChalleAnger;
using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace Challenger;

public class EaterofWorldsHead : CNPC
{
    public EaterofWorldsHead(NPC npc)
        : base(npc)
    {
    }

    public override void OnKilled()
    {
        if (EaterofWorldsBody.State == 0 || EaterofWorldsBody.State == 2)
        {
            var num = Projectile.NewProjectile(null, this.npc.Center, Vector2.Zero, 501, 13, 0f);
            Main.projectile[num].timeLeft = 1;
            CProjectile.Update(num);
            for (var i = 0; i < 6; i++)
            {
                Projectile.NewProjectile(null, this.npc.Center, Vector2.UnitY.RotatedBy(Math.PI / 3.0 * i) * 5f, 909, 14, 0f);
            }
        }
    }

    public override void OnHurtPlayers(GetDataHandlers.PlayerDamageEventArgs e)
    {
        if (Challenger.Config.EnableConsumptionMode)
        {
            switch (Main.rand.Next(1, 4))
            {
                case 1:
                    Challenger.SendPlayerText(GetString("毒牙咬击"), new (177, 94, 255), this.npc.Center + new Vector2(0f, -30f));
                    break;
                case 2:
                    Challenger.SendPlayerText(GetString("创死你"), new (177, 94, 255), this.npc.Center + new Vector2(0f, -30f));
                    break;
                default:
                    Challenger.SendPlayerText(GetString("呜哇哇"), new (177, 94, 255), this.npc.Center + new Vector2(0f, -30f));
                    break;
            }
        }
    }
}