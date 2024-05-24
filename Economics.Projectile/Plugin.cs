using EconomicsAPI.Extensions;
using Microsoft.Xna.Framework;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Configuration;

namespace Economics.Projectile;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    public override string Author => "少司命";

    public override string Description => Assembly.GetExecutingAssembly().GetName().Name!;

    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;

    public override Version Version => Assembly.GetExecutingAssembly().GetName().Version!;

    internal static string PATH = Path.Combine(EconomicsAPI.Economics.SaveDirPath, "Projectile.json");

    private ProjConfig Config = new();

    private readonly int[] useCD = new int[Main.maxPlayers];

    private readonly int[] MiniCD = new int[Main.maxProjectiles];


    public Plugin(Main game) : base(game)
    {
        Order = 6;
    }

    public override void Initialize()
    {
        LoadConfig();
        TShockAPI.Hooks.GeneralHooks.ReloadEvent += (_) => LoadConfig();
        GetDataHandlers.NewProjectile.Register(OnProj);
        GetDataHandlers.PlayerUpdate.Register(OnDate);
        ServerApi.Hooks.GameUpdate.Register(this, Onupdate);
        On.Terraria.Projectile.Damage += Projectile_Damage;
        On.Terraria.Projectile.Minion_FindTargetInRange += Projectile_Minion_FindTargetInRange;
    }

    private void Projectile_Damage(On.Terraria.Projectile.orig_Damage orig, Terraria.Projectile self)
    {
        if (self.minion && Config.ProjectileReplace.TryGetValue(self.type, out ProjectileData? data) && data != null && data.IsMinion)
        {
            if (MiniCD[self.identity] == 0)
            {
                int id = -1;
                self.Minion_FindTargetInRange(1500, ref id, false);
            }
        }
    }

    private void Projectile_Minion_FindTargetInRange(On.Terraria.Projectile.orig_Minion_FindTargetInRange orig, Terraria.Projectile self, int startAttackRange, ref int attackTarget, bool skipIfCannotHitWithOwnBody, Func<Entity, int, bool> customEliminationCheck)
    {
        if (Config.ProjectileReplace.TryGetValue(self.type, out ProjectileData? data) && data != null)
        {
            float num = 1500;
            float num2 = num;
            float num3 = num;
            NPC ownerMinionAttackTargetNPC = self.OwnerMinionAttackTargetNPC;
            if (ownerMinionAttackTargetNPC != null && ownerMinionAttackTargetNPC.CanBeChasedBy(self) && self.IsInRangeOfMeOrMyOwner(ownerMinionAttackTargetNPC, num, out var _, out var _, out var _))
            {
                attackTarget = ownerMinionAttackTargetNPC.whoAmI;
            }
            else
            {
                if (attackTarget >= 0)
                {
                    return;
                }
                for (int i = 0; i < 200; i++)
                {
                    NPC nPC = Main.npc[i];
                    if (nPC.damage > 0 && nPC.CanBeChasedBy(self) && self.IsInRangeOfMeOrMyOwner(nPC, num, out var myDistance2, out var playerDistance2, out var closerIsMe2) && (!skipIfCannotHitWithOwnBody || self.CanHitWithOwnBody(nPC)) && (customEliminationCheck == null || customEliminationCheck(nPC, attackTarget)))
                    {
                        attackTarget = i;
                        num = closerIsMe2 ? myDistance2 : playerDistance2;
                        if (num2 > myDistance2)
                        {
                            num2 = myDistance2;
                        }
                        if (num3 > playerDistance2)
                        {
                            num3 = playerDistance2;
                        }
                        num = Math.Max(num2, num3);
                    }
                }
            }
            if (attackTarget >= 0 && MiniCD[self.identity]== 0)
            {
                for (int i = 0; i < data.ProjData.Count; i++)
                {
                    var proj = data.ProjData[i];
                    if (RPG.RPG.InLevel(Main.player[self.owner].name, proj.Limit))
                    {
                        //伤害
                        float damage = proj.Damage;
                        //击退
                        float knockback = proj.KnockBack;
                        //速度
                        
                        NPC npc = Main.npc[attackTarget];
						self.Distance(npc.Center);
                        var speed = self.DirectionTo(npc.Center).SafeNormalize(-Vector2.UnitY) * self.velocity.Length();
                        int index = EconomicsAPI.Utils.SpawnProjectile.NewProjectile(Terraria.Projectile.GetNoneSource(), self.Center, speed.ToLenOf(proj.speed), proj.ID, (int)damage, knockback, self.owner);
                        TSPlayer.All.SendData(PacketTypes.ProjectileNew, "", index);
                        MiniCD[self.identity] += data.CD;
                    }
                }
            }
        }
    }

    private void Onupdate(EventArgs args)
    {
        for (int i = 0; i < Main.maxPlayers; i++)
        {
            if (useCD[i] > 0)
            {
                useCD[i]--;
            }
        }

        for(int i = 0; i<Main.maxProjectiles; i++)
        {
            if (MiniCD[i] > 0)
            {
                MiniCD[i]--;
            }
        }
    }

    private void OnDate(object? sender, GetDataHandlers.PlayerUpdateEventArgs e)
    {
        if (e.Player.IsLoggedIn && !e.Player.Dead && e.Player.TPlayer.controlUseItem && useCD[e.PlayerId] == 0 && Config.ItemReplace.TryGetValue(e.Player.TPlayer.HeldItem.netID, out ItemData? data) && data != null)
        {
            if (data.UseAmmo)
                return;
            for (int i = 0; i < data.ProjData.Count; i++)
            {
                var proj = data.ProjData[i];
                if (RPG.RPG.InLevel(e.Player.Name, proj.Limit))
                {
                    //伤害
                    float damage = proj.Damage;
                    //击退
                    float knockback = proj.KnockBack;
                    //速度
                    var speed = e.Player.TPlayer.ItemOffSet().ToLenOf(proj.speed);

                    int index = EconomicsAPI.Utils.SpawnProjectile.NewProjectile(e.Player.TPlayer.GetItemSource_OpenItem(e.Player.SelectedItem.netID), e.Player.TPlayer.position, speed, proj.ID, (int)damage, knockback, e.PlayerId);
                    Main.projectile[index].active = true;
                    Main.projectile[index].type = proj.ID;
                    TSPlayer.All.SendData(PacketTypes.ProjectileNew, null, index);
                    useCD[e.Player.Index] += e.Player.SelectedItem.useTime;
                }
            }
        }
    }

    private void OnProj(object? sender, GetDataHandlers.NewProjectileEventArgs e)
    {
        Main.projectile[e.Identity].active = true;
        Main.projectile[e.Identity].owner = e.Player.Index;
        if (e.Player.TPlayer.controlUseItem && e.Player.SelectedItem.useAmmo != 0)
        {
            if (Config.ItemReplace.TryGetValue(e.Player.SelectedItem.netID, out var pr) && pr != null)
            {
                if (pr.UseAmmo)
                {
                    for (int i = 0; i < pr.ProjData.Count; i++)
                    {
                        var proj = pr.ProjData[i];
                        if (RPG.RPG.InLevel(e.Player.Name, proj.Limit))
                        {
                            //伤害
                            float damage = proj.DamageFollow ? (e.Damage - e.Player.SelectedItem.damage) / e.Damage * proj.Damage : proj.Damage;
                            //击退
                            float knockback = proj.KnockBackFollow ? (e.Knockback - e.Player.SelectedItem.knockBack) / e.Knockback * proj.KnockBack : proj.KnockBack;
                            //速度
                            var speed = proj.speed > 0f ? e.Velocity.ToLenOf(proj.speed) : e.Velocity;

                            int index = EconomicsAPI.Utils.SpawnProjectile.NewProjectile(e.Player.TPlayer.GetItemSource_OpenItem(e.Player.SelectedItem.netID), e.Position, speed, proj.ID, (int)damage, knockback, e.Owner);

                            e.Player.SendData(PacketTypes.ProjectileNew, "", index);

                        }
                    }
                }
            }
        }
        else
        {
            if (e.Player.IsLoggedIn && Config.ProjectileReplace.TryGetValue(e.Type, out ProjectileData? data) && data != null)
            {
                if (!data.useItem || e.Player.TPlayer.controlUseItem)
                {
                    if ((e.Player.SelectedItem.shoot == e.Type && !data.IsMinion) || data.fromMinion)
                    {
                        for (int i = 0; i < data.ProjData.Count; i++)
                        {
                            var proj = data.ProjData[i];
                            if (RPG.RPG.InLevel(e.Player.Name, proj.Limit))
                            {
                                //伤害
                                float damage = proj.DamageFollow ? (e.Damage - e.Player.SelectedItem.damage) / e.Damage * proj.Damage : proj.Damage;
                                //击退
                                float knockback = proj.KnockBackFollow ? (e.Knockback - e.Player.SelectedItem.knockBack) / e.Knockback * proj.KnockBack : proj.KnockBack;
                                //速度

                                var speed = proj.speed > 0f ? e.Velocity.ToLenOf(proj.speed) : e.Velocity;

                                int index = EconomicsAPI.Utils.SpawnProjectile.NewProjectile(e.Player.TPlayer.GetItemSource_OpenItem(e.Player.SelectedItem.netID), e.Position, speed, proj.ID, (int)damage, knockback, e.Owner);

                                e.Player.SendData(PacketTypes.ProjectileNew, "", index);

                            }
                        }
                    }
                }
            }
        }

    }



    public void LoadConfig()
    {
        if (!File.Exists(PATH))
        {
            Config.ProjectileReplace.Add(274, new ProjectileData()
            {
                ProjData = new List<ProjectileReplace>()
                {
                    new()
                    {
                        ID = 132,
                        speed = 15,
                        Damage=80,
                        KnockBack = 10,
                        Limit = new()
                    }
                }
            });

            Config.ItemReplace.Add(1327, new()
            {
                ProjData = new List<ProjectileReplace>()
                {
                    new()
                    {
                        ID = 132,
                        speed = 15,
                        Damage=80,
                        KnockBack = 10,
                        Limit = new()
                    }
                }
            });
        }
        Config = EconomicsAPI.Configured.ConfigHelper.LoadConfig(PATH, Config);
    }

}