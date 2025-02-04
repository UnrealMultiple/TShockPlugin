using Economics.Skill.Model.Loop;
using Microsoft.Xna.Framework;
using EconomicsAPI.Extensions;
using Terraria;
using TShockAPI;

namespace Economics.Skill.Internal;
internal class PlayerSkill
{
    public LoopEvent LoopEvent;

    public TSPlayer Player;

    public ProjLoopComm[] ProjLoopComms;

    public int MarkCount;

    public PlayerSkill(LoopEvent loop, TSPlayer ply)
    {
        this.LoopEvent = loop;
        this.Player = ply;
        this.ProjLoopComms = new ProjLoopComm[loop.ProjectileLoops.Count];
        foreach (var i in Enumerable.Range(0, loop.ProjectileLoops.Count))
        {
            this.ProjLoopComms[i] = new(loop.ProjectileLoops[i]);
        }
    }

    internal void Update(int count, Vector2 vel)
    {
        this.MarkCount++;
        this.Project(count, vel);
        this.PlayerEvent(count);
        this.RangeEvent(count);
        if (count >= this.LoopEvent.LoopCount)
        {
            foreach (var proj in this.ProjLoopComms)
            {
                proj.Projectile.miscText = "";
            }
        }
    }

    private void RangeEvent(int count)
    {
        foreach (var loop in this.LoopEvent.RegionLoops)
        {
            if (loop.Mark >= count && loop.EndMark <= count && this.MarkCount % loop.Interval == 0)
            { 
                loop.Spark(this.Player);
            }
            
        }
    }

    private void PlayerEvent(int count)
    {
        foreach (var loop in this.LoopEvent.PlayerLoops)
        {
            if (loop.Mark >= count && loop.EndMark <= count && this.MarkCount % loop.Interval == 0)
            {
                loop.Spark(this.Player);
            }
        }
    }

    public void Mark(int count, ProjLoopComm projLoop)
    {
        foreach (var j in Enumerable.Range(0, projLoop.loop.Marks.Count))
        {
            var mark = projLoop.loop.Marks[j];
            NPC? lockNpc = null;
            if (count >= mark.Mark && count <= mark.EndMark && projLoop.index > 0 && this.MarkCount % mark.Interval == 0)
            {
                #region 锁敌
                if (mark.Lock)
                {
                    lockNpc = mark.LockMinHp ? this.Player.TPlayer.GetNpcInRangeByHp(mark.LockRange) : this.Player.TPlayer.GetNpcInRangeByDis(mark.LockRange);
                    if (mark.FollowNpc && lockNpc != null)
                    {
                        projLoop.Projectile.position.Distance(lockNpc.Center);
                        projLoop.Projectile.velocity = (projLoop.Projectile.position.DirectionTo(lockNpc.Center).SafeNormalize(-Vector2.UnitY) * lockNpc.velocity.Length()).ToLenOf(mark.Speed);
                    }
                }
                #endregion
                if (lockNpc != null && mark.LockNpcCenter)
                {
                    projLoop.Projectile.Center = lockNpc.Center;
                }
                projLoop.Projectile.Center += new Vector2(mark.X, mark.Y);
                var vb = Math.Abs((this.Player.TPlayer.Center + new Vector2(projLoop.loop.X, projLoop.loop.Y) + new Vector2(mark.X, mark.Y)).Distance(this.Player.TPlayer.Center));

                if (mark.Rotating != 0)
                {
                    var angle = (float) (count * mark.Rotating); // 角度随时间递增

                    // 计算弹幕相对玩家的偏移
                    var offsetX = (float) (Math.Cos(angle) * vb);
                    var offsetY = (float) (Math.Sin(angle) * vb);
                    // 弹幕的实际世界坐标
                    projLoop.Projectile.Center = lockNpc != null && mark.LockNpcCenter
                        ? lockNpc.Center + new Vector2(offsetX, offsetY)
                        : this.Player.TPlayer.Center + new Vector2(offsetX, offsetY);
                }
                if (mark.FollowPlayer)
                {
                    var val = (projLoop.Projectile.Center - this.Player.TPlayer.Center).SafeNormalize(Vector2.UnitY * vb);
                    var one = val * vb;
                    projLoop.Projectile.Center = this.Player.TPlayer.Center + one;
                }

                projLoop.Projectile.velocity = projLoop.Projectile.velocity.RotationAngle(mark.Angle);
                projLoop.Projectile.velocity += new Vector2(mark.SpeedX, mark.SpeedY);
                projLoop.Projectile.ai[0] = mark.AI[0];
                projLoop.Projectile.ai[1] = mark.AI[1];
                projLoop.Projectile.ai[2] = mark.AI[2];
                TSPlayer.All.SendData(PacketTypes.ProjectileNew, "", projLoop.index);
            }
        }
    }

    public void Generate(int count, ProjLoopComm projLoop)
    {
        foreach (var generate in projLoop.loop.Generate)
        {
            if (count >= generate.Mark && count <= generate.EndMark && projLoop.index > 0 && this.MarkCount % generate.Interval == 0)
            {
                NPC? lockNpc = null;
                Vector2 vel = default;
                Vector2 pos = default;
                if (generate.Lock)
                {
                    lockNpc = generate.LockMinHp ? this.Player.TPlayer.GetNpcInRangeByHp(generate.LockRange) : this.Player.TPlayer.GetNpcInRangeByDis(generate.LockRange);
                    if (lockNpc != null)
                    {
                        projLoop.Projectile.position.Distance(lockNpc.Center);
                        vel = (projLoop.Projectile.position.DirectionTo(lockNpc.Center).SafeNormalize(-Vector2.UnitY) * lockNpc.velocity.Length()).ToLenOf(generate.Speed);
                    }
                    else
                    {
                        if (generate.TargetAttack)
                        {
                            continue;
                        }
                        vel = Vector2.UnitY.SafeNormalize(default).RotationAngle(generate.Angel).ToLenOf(generate.Speed);
                    }
                }
                else
                {
                    vel = Vector2.UnitY.SafeNormalize(default).RotationAngle(generate.Angel).ToLenOf(generate.Speed);
                }
                
                pos = projLoop.Projectile.position + new Vector2(generate.X, generate.Y);
                var index = EconomicsAPI.Utils.SpawnProjectile.NewProjectile(
                            this.Player.TPlayer.GetProjectileSource_Item(this.Player.TPlayer.HeldItem),
                            pos,
                            vel,
                            generate.ID,
                            generate.Damage,
                            generate.Knockback,
                            this.Player.Index,
                            generate.AI[0] == -1f ? this.Player.Index : generate.AI[0],
                            generate.AI[1] == -1f ? this.Player.Index : generate.AI[1],
                            generate.AI[2] == -1f ? this.Player.Index : generate.AI[2]);
                TSPlayer.All.SendData(PacketTypes.ProjectileNew, "", index);
            }
        }
    }

    public void Project(int count, Vector2 sourcevel)
    {
        foreach (var i in Enumerable.Range(0, this.ProjLoopComms.Length))
        {
            var projLoop = this.ProjLoopComms[i];
            if (count == projLoop.loop.Mark)
            {
                var pos = this.Player.TPlayer.Center + new Vector2(projLoop.loop.X, projLoop.loop.Y);
                var vel = new Vector2(projLoop.loop.SpeedX, projLoop.loop.SpeedY);
                if (projLoop.loop.Speed > 0)
                {
                    vel = vel.SafeNormalize(Vector2.UnitX).ToLenOf(projLoop.loop.Speed);
                }
                if (projLoop.loop.AutoDirection)
                {
                    vel = sourcevel;
                }
                var index = EconomicsAPI.Utils.SpawnProjectile.NewProjectile(
                            this.Player.TPlayer.GetProjectileSource_Item(this.Player.TPlayer.HeldItem),
                            pos,
                            vel,
                            projLoop.loop.ID,
                            Convert.ToInt32(projLoop.loop.Damage),
                            projLoop.loop.Knockback,
                            this.Player.Index,
                            projLoop.loop.AI[0] == -1f ? this.Player.Index : projLoop.loop.AI[0],
                            projLoop.loop.AI[1] == -1f ? this.Player.Index : projLoop.loop.AI[1],
                            projLoop.loop.AI[2] == -1f ? this.Player.Index : projLoop.loop.AI[2],
                            projLoop.loop.TimeLeft);
                projLoop.index = index;
                TSPlayer.All.SendData(PacketTypes.ProjectileNew, "", index);
            }
            this.Mark(count, projLoop);
            this.Generate(count, projLoop);
        }
    }
}



internal class ProjLoopComm
{
    public int index = -1;

    public ProjectileLoop loop;

    public Projectile Projectile => Main.projectile[this.index];

    public ProjLoopComm(ProjectileLoop loop)
    {
        this.loop = loop;
    }
}
