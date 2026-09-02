using Economics.Skill.DB;
using Economics.Skill.Events;
using Economics.Skill.Internal;
using Economics.Skill.Scripting;
using Economics.Skill.Setting;
using Economics.Core.EventArgs.PlayerEventArgs;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using Economics.Core.Utils;

namespace Economics.Skill;

[ApiVersion(2, 1)]
public class Skill : TerrariaPlugin
{
    public override string Author => "少司命";

    public override string Description => GetString("让玩家拥有技能!");

    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(3, 1, 0, 0);


    public long TimerCount;

    public static PlayerSKillManager PlayerSKillManager { get; set; } = null!;

    public Skill(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        SkillScripts.EnsureCreated();   // 加载时即创建技能脚本目录
        GeneralHooks.ReloadEvent += this.OnReload;
        Config.Load();
        PlayerSKillManager = new();
        ServerApi.Hooks.NpcStrike.Register(this, this.OnStrike);
        ServerApi.Hooks.GameUpdate.Register(this, this.OnUpdate);
        GetDataHandlers.PlayerUpdate.Register(this.OnPlayerUpdate);
        GetDataHandlers.PlayerHP.Register(this.OnHP);
        GetDataHandlers.PlayerMana.Register(this.OnMP);
        GetDataHandlers.KillMe.Register(this.KillMe);
        GetDataHandlers.NewProjectile.Register(this.OnNewProj);
        GetDataHandlers.PlayerDamage.Register(this.OnPlayerDamage);
        Core.Events.PlayerHandler.OnPlayerKillNpc += this.OnKillNpc;
        PlaceholderManager.Register("skill", p =>
        {
            var skill = PlayerSKillManager.QuerySkill(p.Name);
            var msg = skill.Count != 0
                ? string.Join(",", skill.Select(x =>
                    x.Skill == null
                        ? GetString("无效技能")
                        : x.Skill.Name))
                : GetString("无");
            return msg;
        });
    }

    // TShock /reload 也触发：重新读取并编译变化的技能脚本
    private void OnReload(ReloadEventArgs args)
    {
        SkillScripts.Reload();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GeneralHooks.ReloadEvent -= this.OnReload;
            Config.UnLoad();
            SkillScripts.Dispose();
            Core.Economics.RemoveAssemblyCommands(Assembly.GetExecutingAssembly());
            Core.Economics.RemoveAssemblyRest(Assembly.GetExecutingAssembly());
            ServerApi.Hooks.NpcStrike.Deregister(this, this.OnStrike);
            ServerApi.Hooks.GameUpdate.Deregister(this, this.OnUpdate);
            GetDataHandlers.PlayerUpdate.UnRegister(this.OnPlayerUpdate);
            GetDataHandlers.PlayerHP.UnRegister(this.OnHP);
            GetDataHandlers.PlayerMana.UnRegister(this.OnMP);
            GetDataHandlers.KillMe.UnRegister(this.KillMe);
            GetDataHandlers.NewProjectile.UnRegister(this.OnNewProj);
            GetDataHandlers.PlayerDamage.UnRegister(this.OnPlayerDamage);
            Core.Events.PlayerHandler.OnPlayerKillNpc -= this.OnKillNpc;
        }
        base.Dispose(disposing);
    }

    private void OnPlayerDamage(object? sender, GetDataHandlers.PlayerDamageEventArgs e)
    {
        PlayerSparkSkillHandler.Adapter(e.Player, Enumerates.SkillSparkType.Struck);
    }


    private void KillMe(object? sender, GetDataHandlers.KillMeEventArgs e)
    {
        PlayerSparkSkillHandler.Adapter(e.Player, Enumerates.SkillSparkType.Death);
    }

    private void OnUpdate(EventArgs args)
    {
        this.TimerCount++;
        if ((this.TimerCount % 6) == 0)
        {
            SkillCD.SendGodPacket();
            SkillCD.Updata();
        }
    }

    private void OnNewProj(object? sender, GetDataHandlers.NewProjectileEventArgs e)
    {
        if (e.Player.TPlayer.controlUseItem)
        {
            PlayerSparkSkillHandler.Adapter(e, Enumerates.SkillSparkType.Take);
        }
    }

    private void OnMP(object? sender, GetDataHandlers.PlayerManaEventArgs e)
    {
        PlayerSparkSkillHandler.Adapter(e.Player, Enumerates.SkillSparkType.MP);
    }

    private void OnHP(object? sender, GetDataHandlers.PlayerHPEventArgs e)
    {
        PlayerSparkSkillHandler.Adapter(e.Player, Enumerates.SkillSparkType.HP);
    }

    private void OnStrike(NpcStrikeEventArgs args)
    {
        var tsply = TShock.Players[args.Player.whoAmI];
        if (tsply != null)
        {
            PlayerSparkSkillHandler.Adapter(tsply, Enumerates.SkillSparkType.Strike);
        }
    }

    private void OnKillNpc(PlayerKillNpcArgs args)
    {
        if (args.Player != null)
        {
            PlayerSparkSkillHandler.Adapter(args.Player, Enumerates.SkillSparkType.Kill);
        }
    }

    private void OnPlayerUpdate(object? sender, GetDataHandlers.PlayerUpdateEventArgs e)
    {
        if (e.Player.TPlayer.ItemTimeIsZero && e.Player.TPlayer.controlUseItem && e.Player.TPlayer.HeldItem.shoot == 0)
        {
            PlayerSparkSkillHandler.Adapter(e.Player, Enumerates.SkillSparkType.Take);
        }

        if (e.Player.TPlayer.dashDelay == -1)
        {
            PlayerSparkSkillHandler.Adapter(e.Player, Enumerates.SkillSparkType.Dash);
        }
        if (e.Player.TPlayer.jump > 0)
        {
            PlayerSparkSkillHandler.Adapter(e.Player, Enumerates.SkillSparkType.Jump);
        }
    }
}