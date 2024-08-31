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
            var num = Projectile.NewProjectile(null, this.npc.Center, Vector2.Zero, 501, 13, 0f, -1, 0f, 0f, 0f);
            Main.projectile[num].timeLeft = 1;
            CProjectile.Update(num);
            for (var i = 0; i < 6; i++)
            {
                Projectile.NewProjectile(null, this.npc.Center, Terraria.Utils.RotatedBy(Vector2.UnitY, Math.PI / 3.0 * i, default) * 5f, 909, 14, 0f, -1, 0f, 0f, 0f);
            }
        }
    }

    public override void OnHurtPlayers(GetDataHandlers.PlayerDamageEventArgs e)
    {
        if (global::Challenger.Challenger.config.EnableConsumptionMode)
        {
            switch (Main.rand.Next(1, 4))
            {
                case 1:
                    global::Challenger.Challenger.SendPlayerText(GetString("毒牙咬击"), new Color(177, 94, 255), this.npc.Center + new Vector2(0f, -30f));
                    break;
                case 2:
                    global::Challenger.Challenger.SendPlayerText(GetString("创死你"), new Color(177, 94, 255), this.npc.Center + new Vector2(0f, -30f));
                    break;
                default:
                    global::Challenger.Challenger.SendPlayerText(GetString("呜哇哇"), new Color(177, 94, 255), this.npc.Center + new Vector2(0f, -30f));
                    break;
            }
        }
    }
}