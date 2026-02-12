using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using TShockAPI;

namespace Challenger;

public class CProjectile
{
    public Projectile proj;

    private readonly int type;

    private readonly int index;

    private readonly int owner;

    public float[] ai;

    public int lable;

    private bool _isActive;

    public bool isActive
    {
        get => this.proj != null && this.proj.type == this.type && this.proj.whoAmI == this.index && this.proj.owner == this.owner && Main.player[this.owner].active && this.proj.active && this._isActive;
        set => this._isActive = value;
    }

    protected CProjectile()
    {
        this.proj = null!;
        this.type = 0;
        this.index = 0;
        this.owner = 0;
        this.ai = new float[8];
        this.lable = 0;
        this.isActive = false;
    }

    protected CProjectile(Projectile? proj)
    {
        if (proj == null)
        {
            this.proj = null!;
            this.type = 0;
            this.index = 0;
            this.owner = 0;
            this.ai = new float[8];
            this.lable = 0;
            this.isActive = false;
        }
        else
        {
            this.proj = proj;
            this.type = proj.type;
            this.index = proj.whoAmI;
            this.owner = proj.owner;
            this.ai = new float[8];
            this.lable = 0;
            this.isActive = proj.active;
        }
    }

    protected CProjectile(Projectile? proj, float[] ai, int lable)
    {
        if (proj == null)
        {
            this.proj = null!;
            this.type = 0;
            this.index = 0;
            this.owner = 0;
            this.ai = new float[8];
            this.lable = 0;
            this.isActive = false;
        }
        else
        {
            this.proj = proj;
            this.type = proj.type;
            this.index = proj.whoAmI;
            this.owner = proj.owner;
            this.ai = ai;
            this.lable = lable;
            this.isActive = proj.active;
        }
    }

    public static void CKill(int index)
    {
        if (Main.projectile[index] != null && Main.projectile[index].active)
        {
            Main.projectileIdentity[Main.projectile[index].owner, Main.projectile[index].identity] = -1;
            Main.projectile[index].timeLeft = 0;
            if (Main.getGoodWorld && Main.projectile[index].aiStyle == 16)
            {
                Main.projectile[index].PrepareBombToBlow();
                var projRectangle = Main.projectile[index].Damage_GetHitbox();
    
                if (Main.projectile[index].hostile)
                {
                    Main.projectile[index].Damage_EVP(projRectangle);
                }
                else if (Main.projectile[index].friendly && !Main.projectile[index].npcProj && !ProjectileID.Sets.RocketsSkipDamageForPlayers[Main.projectile[index].type] 
                         && (Main.projectile[index].owner == Main.myPlayer || Main.getGoodWorld))
                {
                    Main.projectile[index].BombsHurtPlayers(projRectangle); 
                }
            }
            Main.projectile[index].active = false;
            TSPlayer.All.SendData((PacketTypes) 29, "", Main.projectile[index].identity, Main.projectile[index].owner, 0f, 0f, 0);
            if (Collect.cprojs[index] != null)
            {
                Collect.cprojs[index].isActive = false;
            }
        }
    }

    public void CKill()
    {
        if (this.proj != null && this.proj.active)
        {
            Main.projectileIdentity[this.proj.owner, this.proj.identity] = -1;
            this.proj.timeLeft = 0;
            if (Main.getGoodWorld && Main.projectile[index].aiStyle == 16)
            {
                Main.projectile[index].PrepareBombToBlow();
                var projRectangle = Main.projectile[index].Damage_GetHitbox();
    
                if (Main.projectile[index].hostile)
                {
                    Main.projectile[index].Damage_EVP(projRectangle);
                }
                else if (Main.projectile[index].friendly && !Main.projectile[index].npcProj && !ProjectileID.Sets.RocketsSkipDamageForPlayers[type] 
                         && (Main.projectile[index].owner == Main.myPlayer || Main.getGoodWorld))
                {
                    Main.projectile[index].BombsHurtPlayers(projRectangle); 
                }
            }
            this.proj.active = false;
            TSPlayer.All.SendData((PacketTypes) 29, "", this.proj.identity, this.proj.owner, 0f, 0f, 0);
            if (Collect.cprojs[this.proj.whoAmI] != null)
            {
                Collect.cprojs[this.proj.whoAmI].isActive = false;
            }
        }
    }

    public static void Update(int index)
    {
        TSPlayer.All.SendData((PacketTypes) 27, null, index, 0f, 0f, 0f, 0);
    }

    public void Update()
    {
        TSPlayer.All.SendData((PacketTypes) 27, null, this.index, 0f, 0f, 0f, 0);
    }

    public virtual void ProjectileAI()
    {
    }

    public virtual void PreProjectileKilled()
    {
    }

    public virtual void MyEffect()
    {
    }
}