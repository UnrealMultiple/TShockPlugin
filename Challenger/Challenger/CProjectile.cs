using Terraria;
using TShockAPI;

namespace Challenger
{
    public class CProjectile
    {
        public Projectile? proj;

        private int type;

        private int index;

        private int owner;

        public float[] ai;

        public int lable;

        private bool _isActive;

        public bool isActive
        {
            get
            {
                return proj != null && proj.type == type && proj.whoAmI == index && proj.owner == owner && Main.player[owner].active && proj.active && _isActive;
            }
            set
            {
                _isActive = value;
            }
        }

        protected CProjectile()
        {
            proj = null;
            type = 0;
            index = 0;
            owner = 0;
            ai = new float[8];
            lable = 0;
            isActive = false;
        }

        protected CProjectile(Projectile? proj)
        {
            if (proj == null)
            {
                this.proj = null;
                type = 0;
                index = 0;
                owner = 0;
                ai = new float[8];
                lable = 0;
                isActive = false;
            }
            else
            {
                this.proj = proj;
                type = proj.type;
                index = ((Entity)proj).whoAmI;
                owner = proj.owner;
                ai = new float[8];
                lable = 0;
                isActive = ((Entity)proj).active;
            }
        }

        protected CProjectile(Projectile? proj, float[] ai, int lable)
        {
            if (proj == null)
            {
                this.proj = null;
                type = 0;
                index = 0;
                owner = 0;
                this.ai = new float[8];
                this.lable = 0;
                isActive = false;
            }
            else
            {
                this.proj = proj;
                type = proj.type;
                index = ((Entity)proj).whoAmI;
                owner = proj.owner;
                this.ai = ai;
                this.lable = lable;
                isActive = ((Entity)proj).active;
            }
        }

        public static void CKill(int index)
        {
            if (Main.projectile[index] != null && ((Entity)Main.projectile[index]).active)
            {
                Main.projectileIdentity[Main.projectile[index].owner, Main.projectile[index].identity] = -1;
                Main.projectile[index].timeLeft = 0;
                if (Main.getGoodWorld && Main.projectile[index].aiStyle == 16)
                {
                    Main.projectile[index].TryGettingHitByOtherPlayersExplosives();
                }
                ((Entity)Main.projectile[index]).active = false;
                TSPlayer.All.SendData((PacketTypes)29, "", Main.projectile[index].identity, (float)Main.projectile[index].owner, 0f, 0f, 0);
                if (Collect.cprojs[index] != null)
                {
                    Collect.cprojs[index].isActive = false;
                }
            }
        }

        public void CKill()
        {
            if (proj != null && ((Entity)proj).active)
            {
                Main.projectileIdentity[proj.owner, proj.identity] = -1;
                proj.timeLeft = 0;
                if (Main.getGoodWorld && proj.aiStyle == 16)
                {
                    proj.TryGettingHitByOtherPlayersExplosives();
                }
                ((Entity)proj).active = false;
                TSPlayer.All.SendData((PacketTypes)29, "", proj.identity, (float)proj.owner, 0f, 0f, 0);
                if (Collect.cprojs[((Entity)proj).whoAmI] != null)
                {
                    Collect.cprojs[((Entity)proj).whoAmI].isActive = false;
                }
            }
        }

        public static void Update(int index)
        {
            TSPlayer.All.SendData((PacketTypes)27, (string)null, index, 0f, 0f, 0f, 0);
        }

        public void Update()
        {
            TSPlayer.All.SendData((PacketTypes)27, (string)null, index, 0f, 0f, 0f, 0);
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
}