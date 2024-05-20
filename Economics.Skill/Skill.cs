using Economics.Skill.DB;
using Economics.Skill.Events;
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

    public override string Description => Assembly.GetExecutingAssembly().GetName().Name!;

    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;

    public override Version Version => Assembly.GetExecutingAssembly().GetName().Version!;

    internal static string PATH = Path.Combine(EconomicsAPI.Economics.SaveDirPath, "Skill.json");

    public long TimerCount;

    internal static Config Config { get; set; } = new();

    internal static PlayerSKillManager PlayerSKillManager { get; set; }

    public Skill(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        LoadConfig();
        PlayerSKillManager = new();
        ServerApi.Hooks.NpcStrike.Register(this, OnStrike);
        ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
        GetDataHandlers.PlayerUpdate.Register(OnPlayerUpdate);
        GetDataHandlers.PlayerHP.Register(OnHP);
        GetDataHandlers.PlayerMana.Register(OnMP);
        GetDataHandlers.NewProjectile.Register(OnNewProj);
        EconomicsAPI.Events.PlayerHandler.OnPlayerKillNpc += OnKillNpc;
        GeneralHooks.ReloadEvent += e => LoadConfig();
    }
    private void OnUpdate(EventArgs args)
    {
        TimerCount++;
        if ((TimerCount % 6) == 0)
        {
            SkillCD.Updata();
        }
    }

    private void OnNewProj(object? sender, GetDataHandlers.NewProjectileEventArgs e)
    {
        if (e.Player.TPlayer.controlUseItem)
            PlayerSparkSkillHandler.Adapter(e, Enumerates.SkillSparkType.Take);
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
            PlayerSparkSkillHandler.Adapter(tsply, Enumerates.SkillSparkType.Strike);
    }

    private void OnKillNpc(PlayerKillNpcArgs args)
    {
        PlayerSparkSkillHandler.Adapter(args.Player, Enumerates.SkillSparkType.Kill);
    }

    private void OnPlayerUpdate(object? sender, GetDataHandlers.PlayerUpdateEventArgs e)
    {
        if (e.Player.TPlayer.ItemTimeIsZero && e.Player.TPlayer.controlUseItem && e.Player.TPlayer.HeldItem.shoot == 0)
        {
            PlayerSparkSkillHandler.Adapter(e.Player, Enumerates.SkillSparkType.Take);
        }
    }

    private static void LoadConfig()
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
                        Projectiles = new()
                        {
                            new()
                            {
                                CircleProjectiles = new()
                                {
                                    new()
                                },
                                ProjectileCycle = new()
                                {
                                    ProjectileCycles = new()
                                    {
                                        new()
                                    }
                                }
                            }
                        }
                    }
                }
            };
            Config = ConfigHelper.LoadConfig(PATH, Config);
        }
    }
}
