using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Utilities;
using static Terraria.UI.ItemSlot;

namespace Challenger
{
    internal static class Collect
    {
        public static CProjectile[] cprojs = new CProjectile[1000];

        public static CNPC[] cnpcs = new CNPC[200];

        public static CPlayer[] cplayers = new CPlayer[255];

        public static int worldevent = 0;

        public static HashSet<int> noneedlifeNPC = new HashSet<int> { 115, 116, 488 };

        public static int MyNewProjectile(IEntitySource spawnSource, float X, float Y, float SpeedX, float SpeedY, int Type, int Damage, float KnockBack, int Owner = 255, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f)
        {
            if (Owner == -1)
            {
                Owner = Main.myPlayer;
            }
            int num = -1;
            for (int num2 = 899; num2 >= 0; num2--)
            {
                if (!((Entity)Main.projectile[num2]).active)
                {
                    num = num2;
                    break;
                }
            }
            if (num == -1)
            {
                num = Projectile.FindOldestProjectile();
            }
            Projectile val = Main.projectile[num];
            val.SetDefaults(Type);
            ((Entity)val).position.X = X - (float)((Entity)val).width * 0.5f;
            ((Entity)val).position.Y = Y - (float)((Entity)val).height * 0.5f;
            val.owner = Owner;
            ((Entity)val).velocity.X = SpeedX;
            ((Entity)val).velocity.Y = SpeedY;
            val.damage = Damage;
            val.knockBack = KnockBack;
            val.identity = num;
            val.gfxOffY = 0f;
            val.stepSpeed = 1f;
            ((Entity)val).wet = Collision.WetCollision(((Entity)val).position, ((Entity)val).width, ((Entity)val).height);
            if (val.ignoreWater)
            {
                ((Entity)val).wet = false;
            }
            ((Entity)val).honeyWet = Collision.honey;
            ((Entity)val).shimmerWet = Collision.shimmer;
            Main.projectileIdentity[Owner, num] = num;
            Projectile.FindBannerToAssociateTo(spawnSource, val);
            if (val.aiStyle == 1)
            {
                while (((Entity)val).velocity.X >= 16f || ((Entity)val).velocity.X <= -16f || ((Entity)val).velocity.Y >= 16f || ((Entity)val).velocity.Y < -16f)
                {
                    Projectile val2 = val;
                    ((Entity)val2).velocity.X = ((Entity)val2).velocity.X * 0.97f;
                    Projectile val3 = val;
                    ((Entity)val3).velocity.Y = ((Entity)val3).velocity.Y * 0.97f;
                }
            }
            if (Type == 434)
            {
                val.ai[0] = ((Entity)val).position.X;
                val.ai[1] = ((Entity)val).position.Y;
            }
            if (Type > 0 && Type < ProjectileID.Count)
            {
                if (ProjectileID.Sets.NeedsUUID[Type])
                {
                    val.projUUID = val.identity;
                }

                // Use a guard clause to avoid accessing Main.projectile when not needed
                if (ProjectileID.Sets.StardustDragon[Type] && val.ai[0] >= 0)
                {
                    int projUUID = Main.projectile[(int)val.ai[0]].projUUID;
                    if (projUUID >= 0)
                    {
                        val.ai[0] = projUUID;
                    }
                }
            }
            if (Type == 249)
            {
                val.frame = Main.rand.Next(5);
            }
            val.ai[0] = ai0;
            val.ai[1] = ai1;
            val.ai[2] = ai2;
            return num;
        }

        public static int MyNewProjectile(IEntitySource spawnSource, Vector2 postion, Vector2 velocity, int Type, int Damage, float KnockBack, int Owner = -1, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f)
        {
            return MyNewProjectile(spawnSource, postion.X, postion.Y, velocity.X, velocity.Y, Type, Damage, KnockBack, Owner, ai0, ai1, ai2);
        }

        public static int MyNewItem(IEntitySource source, Vector2 pos, Vector2 randomBox, int Type, int Stack = 1, bool noBroadcast = false, int prefixGiven = 0, bool noGrabDelay = false, bool reverseLookup = false)
        {
            return MyNewItem(source, (int)pos.X, (int)pos.Y, (int)randomBox.X, (int)randomBox.Y, Type, Stack, noBroadcast, prefixGiven, noGrabDelay, reverseLookup);
        }

        public static int MyNewItem(IEntitySource source, int X, int Y, int Width, int Height, int Type, int Stack = 1, bool noBroadcast = false, int pfix = 0, bool noGrabDelay = false, bool reverseLookup = false)
        {
            if (WorldGen.gen)
            {
                return 0;
            }
            if (Main.rand == null)
            {
                Main.rand = new UnifiedRandom();
            }
            if (Main.tenthAnniversaryWorld)
            {
                if (Type == 58)
                {
                    Type = Utils.NextFromList<short>(Main.rand, new short[3] { 1734, 1867, 58 });
                }
                if (Type == 184)
                {
                    Type = Utils.NextFromList<short>(Main.rand, new short[3] { 1735, 1868, 184 });
                }
            }
            if (Main.halloween)
            {
                if (Type == 58)
                {
                    Type = 1734;
                }
                if (Type == 184)
                {
                    Type = 1735;
                }
            }
            if (Main.xMas)
            {
                if (Type == 58)
                {
                    Type = 1867;
                }
                if (Type == 184)
                {
                    Type = 1868;
                }
            }
            if (Type > 0 && Item.cachedItemSpawnsByType[Type] != -1)
            {
                Item.cachedItemSpawnsByType[Type] += Stack;
                return 400;
            }
            Main.item[400] = new Item();
            int num = 400;
            if (Main.netMode != 1)
            {
                num = Item.PickAnItemSlotToSpawnItemOn(reverseLookup, num);
            }
            Main.timeItemSlotCannotBeReusedFor[num] = 0;
            Main.item[num] = new Item();
            Item val = Main.item[num];
            val.SetDefaults(Type);
            val.Prefix(pfix);
            val.stack = Stack;
            ((Entity)val).position.X = X + Width / 2 - ((Entity)val).width / 2;
            ((Entity)val).position.Y = Y + Height / 2 - ((Entity)val).height / 2;
            ((Entity)val).wet = Collision.WetCollision(((Entity)val).position, ((Entity)val).width, ((Entity)val).height);
            ((Entity)val).velocity.X = (float)Main.rand.Next(-30, 31) * 0.1f;
            ((Entity)val).velocity.Y = (float)Main.rand.Next(-40, -15) * 0.1f;
            if (Type == 859 || Type == 4743)
            {
                ((Entity)val).velocity = ((Entity)val).velocity * 0f;
            }
            if (Type == 520 || Type == 521 || (val.type >= 0 && ItemID.Sets.NebulaPickup[val.type]))
            {
                ((Entity)val).velocity.X = (float)Main.rand.Next(-30, 31) * 0.1f;
                ((Entity)val).velocity.Y = (float)Main.rand.Next(-30, 31) * 0.1f;
            }
            val.active = true;
            val.timeSinceItemSpawned = ItemID.Sets.OverflowProtectionTimeOffset[val.type];
            Item.numberOfNewItems++;
            if (Options.HighlightNewItems && val.type >= 0 && !ItemID.Sets.NeverAppearsAsNewInInventory[val.type])
            {
                val.newAndShiny = true;
            }
            else if (Main.netMode == 0)
            {
                val.playerIndexTheItemIsReservedFor = Main.myPlayer;
            }
            return num;
        }
    }
}