using Economics.Skill.DB;
using Economics.Skill.Events;
using Economics.Skill.Internal;
using Economics.Skill.JSInterpreter;
using Economics.Skill.Setting;
using EconomicsAPI.Configured;
using EconomicsAPI.EventArgs.PlayerEventArgs;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace Economics.Skill;

[ApiVersion(2, 1)]
public class Skill : TerrariaPlugin
{
    public override string Author => "少司命";

    public override string Description => GetString("让玩家拥有技能!");

    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(2, 0, 1, 1);

    internal static string PATH = Path.Combine(EconomicsAPI.Economics.SaveDirPath, "Skill.json");

    public long TimerCount;

    public static Config Config { get; set; } = new();

    public static PlayerSKillManager PlayerSKillManager { get; set; } = null!;

    public Skill(Main game) : base(game)
    {
        AppDomain.CurrentDomain.AssemblyResolve += this.CurrentDomain_AssemblyResolve;
    }

    private Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
    {
        var resourceName = $"{Assembly.GetExecutingAssembly().GetName().Name}.lib.{new AssemblyName(args.Name).Name}.dll";
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        if (stream != null)
        {
            var assemblyData = new byte[stream.Length];
            stream.Read(assemblyData, 0, assemblyData.Length);
            return Assembly.Load(assemblyData);
        }
        return null;
    }

    public override void Initialize()
    {
        LoadConfig();
        PlayerSKillManager = new();
        ServerApi.Hooks.GamePostInitialize.Register(this, this.OnPost);
        ServerApi.Hooks.NpcStrike.Register(this, this.OnStrike);
        ServerApi.Hooks.GameUpdate.Register(this, this.OnUpdate);
        GetDataHandlers.PlayerUpdate.Register(this.OnPlayerUpdate);
        GetDataHandlers.PlayerHP.Register(this.OnHP);
        GetDataHandlers.PlayerMana.Register(this.OnMP);
        GetDataHandlers.KillMe.Register(this.KillMe);
        GetDataHandlers.NewProjectile.Register(this.OnNewProj);
        GetDataHandlers.PlayerDamage.Register(this.OnPlayerDamage);
        EconomicsAPI.Events.PlayerHandler.OnPlayerKillNpc += this.OnKillNpc;
        EconomicsAPI.Events.PlayerHandler.OnPlayerCountertop += this.OnPlayerCountertop;
        GeneralHooks.ReloadEvent += LoadConfig;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            EconomicsAPI.Economics.RemoveAssemblyCommands(Assembly.GetExecutingAssembly());
            EconomicsAPI.Economics.RemoveAssemblyRest(Assembly.GetExecutingAssembly());
            ServerApi.Hooks.GamePostInitialize.Deregister(this, this.OnPost);
            ServerApi.Hooks.NpcStrike.Deregister(this, this.OnStrike);
            ServerApi.Hooks.GameUpdate.Deregister(this, this.OnUpdate);
            GetDataHandlers.PlayerUpdate.UnRegister(this.OnPlayerUpdate);
            GetDataHandlers.PlayerHP.UnRegister(this.OnHP);
            GetDataHandlers.PlayerMana.UnRegister(this.OnMP);
            GetDataHandlers.KillMe.UnRegister(this.KillMe);
            GetDataHandlers.NewProjectile.UnRegister(this.OnNewProj);
            GetDataHandlers.PlayerDamage.UnRegister(this.OnPlayerDamage);
            EconomicsAPI.Events.PlayerHandler.OnPlayerKillNpc -= this.OnKillNpc;
            EconomicsAPI.Events.PlayerHandler.OnPlayerCountertop -= this.OnPlayerCountertop;
            GeneralHooks.ReloadEvent -= LoadConfig;
        }
        base.Dispose(disposing);
    }
    private void OnPost(EventArgs args)
    {
        Interpreter.LoadFunction();
    }

    private void OnPlayerDamage(object? sender, GetDataHandlers.PlayerDamageEventArgs e)
    {
        PlayerSparkSkillHandler.Adapter(e.Player, Enumerates.SkillSparkType.Struck);
    }


    private void KillMe(object? sender, GetDataHandlers.KillMeEventArgs e)
    {
        PlayerSparkSkillHandler.Adapter(e.Player, Enumerates.SkillSparkType.Death);
    }

    private void OnPlayerCountertop(PlayerCountertopArgs args)
    {
        var skill = PlayerSKillManager.QuerySkill(args.Player!.Name);
        var msg = skill.Any()
            ? string.Join(",", skill.Select(x =>
                x.Skill == null
                    ? GetString("无效技能")
                    : x.Skill.Name))
            : GetString("无");
        args.Messages.Add(new (GetString($"绑定技能: {msg}"), 12));
    }

    private void OnUpdate(EventArgs args)
    {
        this.TimerCount++;
        JobjManager.FrameUpdate();
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

    private static void LoadConfig(ReloadEventArgs? args = null)
    {
        if (File.Exists(PATH))
        {
            Config = ConfigHelper.LoadConfig<Config>(PATH);
        }
        else
        {
            Config = new Config()
            {
                SkillContexts = new()
                {
                    new Model.SkillContext()
                    {
                       LoopEvent = new()
                       {
                            ProjectileLoops = new()
                            
                       }
                    }
                }
            };
            Config = ConfigHelper.LoadConfig(PATH, Config);
        }
    }
}