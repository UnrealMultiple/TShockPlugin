
using Economics.Core.Extensions;
using Microsoft.Xna.Framework;
using System.Collections.Concurrent;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace Economics.Projectile;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    public override string Author => "少司命";

    public override string Description => GetString("增强玩家武器弹幕!");

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 0, 0, 9);

    private readonly int[] useCD = new int[Main.maxPlayers];

    private readonly int[] MiniCD = new int[Main.maxProjectiles];

    private readonly ConcurrentDictionary<string, Terraria.Projectile> FollowProj = new();


    public Plugin(Main game) : base(game)
    {
        this.Order = 6;
    }

    public override void Initialize()
    {
        Config.Load();
        GetDataHandlers.NewProjectile.Register(this.OnProj);
        GetDataHandlers.PlayerUpdate.Register(this.OnDate);
        ServerApi.Hooks.GameUpdate.Register(this, this.Onupdate);
        On.Terraria.Projectile.Minion_FindTargetInRange += this.Projectile_Minion_FindTargetInRange;
        On.Terraria.Projectile.Update += this.Projectile_Update;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Core.Economics.RemoveAssemblyCommands(Assembly.GetExecutingAssembly());
            Core.Economics.RemoveAssemblyRest(Assembly.GetExecutingAssembly());
            GetDataHandlers.NewProjectile.UnRegister(this.OnProj);
            GetDataHandlers.PlayerUpdate.UnRegister(this.OnDate);
            ServerApi.Hooks.GameUpdate.Deregister(this, this.Onupdate);
            On.Terraria.Projectile.Minion_FindTargetInRange -= this.Projectile_Minion_FindTargetInRange;
            On.Terraria.Projectile.Update -= this.Projectile_Update;
            Config.UnLoad();
        }
        base.Dispose(disposing);
    }

    private void Projectile_Update(On.Terraria.Projectile.orig_Update orig, Terraria.Projectile self, int i)
    {
        if (self.active && self.timeLeft > 0 && !string.IsNullOrEmpty(self.miscText) && this.FollowProj.TryGetValue(self.miscText, out var porj))
        {
            var target = self.position.FindRangeNPC(60 * 16f);
            if (target != null)
            {
                var speed = self.DirectionTo(target.Center).SafeNormalize(-Vector2.UnitY);
                self.velocity = speed.ToLenOf(15f);
                TSPlayer.All.SendData(PacketTypes.ProjectileNew, "", self.whoAmI);
                if (Math.Abs(target.Distance(self.Center)) <= 2 * 16f)
                {
                    this.FollowProj.Remove(self.miscText, out var _);
                }
            }
        }
        orig(self, i);
    }

    private void Projectile_Minion_FindTargetInRange(On.Terraria.Projectile.orig_Minion_FindTargetInRange orig, Terraria.Projectile self, int startAttackRange, ref int attackTarget, bool skipIfCannotHitWithOwnBody, Func<Entity, int, bool> customEliminationCheck, bool respectOwnerTarget)
    {
        if (Config.Instance.ProjectileReplace.TryGetValue(self.type, out var data) && data != null)
        {
            float num = 1500;
            var num2 = num;
            var num3 = num;
            var ownerMinionAttackTargetNPC = self.OwnerMinionAttackTargetNPC;
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
                for (var i = 0; i < 200; i++)
                {
                    var nPC = Main.npc[i];
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
            if (attackTarget >= 0 && this.MiniCD[self.whoAmI] == 0)
            {
                for (var i = 0; i < data.ProjData.Count; i++)
                {
                    var proj = data.ProjData[i];
                    if (RPG.RPG.InLevel(Main.player[self.owner].name, proj.Limit))
                    {
                        //伤害
                        var damage = proj.Damage;
                        //击退
                        var knockback = proj.KnockBack;
                        //速度
                        var guid = Guid.NewGuid().ToString();
                        var npc = Main.npc[attackTarget];
                        var speed = self.DirectionTo(npc.Center).SafeNormalize(-Vector2.UnitY) * self.velocity.Length();
                        var index = Core.Utils.SpawnProjectile.NewProjectile(Terraria.Projectile.GetNoneSource(), self.Center, speed.ToLenOf(proj.Speed), proj.ID, (int) damage, knockback, self.owner, proj.AI[0], proj.AI[1], proj.AI[2], proj.TimeLeft, guid);
                        TSPlayer.All.SendData(PacketTypes.ProjectileNew, "", index);
                        if (proj.AutoFollow)
                        {
                            this.FollowProj[guid] = Main.projectile[index];
                        }

                        this.MiniCD[self.whoAmI] += data.CD;
                    }
                }
            }
        }

        orig(self, startAttackRange, ref attackTarget, skipIfCannotHitWithOwnBody, customEliminationCheck, respectOwnerTarget);
    }

    private void Onupdate(EventArgs args)
    {
        for (var i = 0; i < Main.maxPlayers; i++)
        {
            if (this.useCD[i] > 0)
            {
                this.useCD[i]--;
            }
        }

        for (var i = 0; i < Main.maxProjectiles; i++)
        {
            if (this.MiniCD[i] > 0)
            {
                this.MiniCD[i]--;
            }
        }
        if (Main.time % 600 == 0)
        {
            foreach (var (proj, _) in this.FollowProj.Where(x => x.Value == null || !x.Value.active).ToList())
            {
                this.FollowProj.Remove(proj, out var _);
            }
        }
    }

    private void OnDate(object? sender, GetDataHandlers.PlayerUpdateEventArgs e)
    {
        if (e.Player.IsLoggedIn
            && !e.Player.Dead
            && e.Player.TPlayer.controlUseItem
            && this.useCD[e.PlayerId] == 0
            && Config.Instance.ItemReplace.TryGetValue(e.Player.TPlayer.HeldItem.type, out var data)
            && data != null)
        {
            if (data.UseAmmo)
            {
                return;
            }

            for (var i = 0; i < data.ProjData.Count; i++)
            {
                var proj = data.ProjData[i];
                if (RPG.RPG.InLevel(e.Player.Name, proj.Limit))
                {
                    //伤害
                    var damage = proj.Damage;
                    //击退
                    var knockback = proj.KnockBack;
                    //速度
                    var speed = e.Player.TPlayer.ItemOffSet().ToLenOf(proj.Speed);
                    var guid = Guid.NewGuid().ToString();
                    var index = Core.Utils.SpawnProjectile.NewProjectile(e.Player.TPlayer.GetItemSource_OpenItem(e.Player.SelectedItem.type), e.Player.TPlayer.position, speed, proj.ID, (int) damage, knockback, e.PlayerId, proj.AI[0], proj.AI[1], proj.AI[2], proj.TimeLeft, guid);
                    TSPlayer.All.SendData(PacketTypes.ProjectileNew, null, index);
                    this.useCD[e.Player.Index] += e.Player.SelectedItem.useTime;
                    if (proj.AutoFollow)
                    {
                        this.FollowProj[guid] = Main.projectile[index];
                    }
                }
            }
        }
    }

    private void OnProj(object? sender, GetDataHandlers.NewProjectileEventArgs e)
    {
        var projectile = Main.projectile[e.Index];
        if (!string.IsNullOrEmpty(projectile.miscText))
        {
            return;
        }

        if (e.Player.TPlayer.controlUseItem && e.Player.SelectedItem.useAmmo != 0)
        {
            if (Config.Instance.ItemReplace.TryGetValue(e.Player.SelectedItem.type, out var pr) && pr != null)
            {
                if (pr.UseAmmo)
                {
                    for (var i = 0; i < pr.ProjData.Count; i++)
                    {
                        var proj = pr.ProjData[i];
                        if (RPG.RPG.InLevel(e.Player.Name, proj.Limit))
                        {
                            //伤害
                            var damage = proj.DamageFollow ? (e.Damage - e.Player.SelectedItem.damage) / e.Damage * proj.Damage : proj.Damage;
                            //击退
                            var knockback = proj.KnockBackFollow ? (e.Knockback - e.Player.SelectedItem.knockBack) / e.Knockback * proj.KnockBack : proj.KnockBack;
                            //速度
                            var speed = proj.Speed > 0f ? e.Velocity.ToLenOf(proj.Speed) : e.Velocity;
                            var guid = Guid.NewGuid().ToString();
                            var index = Core.Utils.SpawnProjectile.NewProjectile(Main.projectile[e.Index].GetProjectileSource_FromThis(), e.Position, speed, proj.ID, (int) damage, knockback, e.Owner, proj.AI[0], proj.AI[1], proj.AI[2], proj.TimeLeft, guid);
                            e.Player.SendData(PacketTypes.ProjectileNew, "", index);
                            if (proj.AutoFollow)
                            {
                                this.FollowProj[guid] = Main.projectile[index];
                            }
                        }
                    }
                }
            }
        }
        else
        {
            if (e.Player.IsLoggedIn && Config.Instance.ProjectileReplace.TryGetValue(e.Type, out var data) && data != null)
            {
                if (!data.useItem || e.Player.TPlayer.controlUseItem)
                {
                    if ((e.Player.SelectedItem.shoot == e.Type && !data.IsMinion) || data.fromMinion)
                    {
                        for (var i = 0; i < data.ProjData.Count; i++)
                        {
                            var proj = data.ProjData[i];
                            if (RPG.RPG.InLevel(e.Player.Name, proj.Limit))
                            {
                                //伤害
                                var damage = proj.DamageFollow ? (e.Damage - e.Player.SelectedItem.damage) / e.Damage * proj.Damage : proj.Damage;
                                //击退
                                var knockback = proj.KnockBackFollow ? (e.Knockback - e.Player.SelectedItem.knockBack) / e.Knockback * proj.KnockBack : proj.KnockBack;
                                //速度

                                var speed = proj.Speed > 0f ? e.Velocity.ToLenOf(proj.Speed) : e.Velocity;

                                var guid = Guid.NewGuid().ToString();

                                var index = Core.Utils.SpawnProjectile.NewProjectile(e.Player.TPlayer.GetItemSource_OpenItem(e.Player.SelectedItem.type), e.Position, speed, proj.ID, (int) damage, knockback, e.Owner, proj.AI[0], proj.AI[1], proj.AI[2], proj.TimeLeft, guid);

                                e.Player.SendData(PacketTypes.ProjectileNew, "", index);

                                if (proj.AutoFollow)
                                {
                                    this.FollowProj[guid] = Main.projectile[index];
                                }
                            }
                        }
                    }
                }
            }
        }

    }
}