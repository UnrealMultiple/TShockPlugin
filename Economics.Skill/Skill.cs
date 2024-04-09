using Economics.Skill.Events;
using Economics.Skill.Model;
using Economics.Skill.Setting;
using EconomicsAPI.Configured;
using EconomicsAPI.EventArgs;
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

    internal static ESPlayer[] ESPlayers = new ESPlayer[Main.maxPlayers];

    public long TimerCount;

    internal static Config Config { get; set; }

    public Skill(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        LoadConfig();
        ServerApi.Hooks.NetGreetPlayer.Register(this, OnGreetPlayer);
        ServerApi.Hooks.NpcStrike.Register(this, OnStrike);
        ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
        GetDataHandlers.PlayerUpdate.Register(OnPlayerUpdate);
        GetDataHandlers.PlayerHP.Register(OnHP);
        GetDataHandlers.PlayerMana.Register(OnMP);
        GetDataHandlers.NewProjectile.Register(OnNewProj);
        EconomicsAPI.Events.PlayerHandler.OnPlayerKillNpc += OnKillNpc;
        GeneralHooks.ReloadEvent += e => LoadConfig();

        //Commands.ChatCommands.Add(new(e => PlayerSparkSkillHandler.Adapter(Config.SkillContexts[0], Enumerates.SkillSparkType.Take), "skill")); 
    }

    private void OnGreetPlayer(GreetPlayerEventArgs args)
    {
        ESPlayers[args.Who] = new ESPlayer(args.Who);
    }

    private void OnUpdate(EventArgs args)
    {
        TimerCount++;
        if ((TimerCount % 6) == 0)
        { 
           
        }
    }

    private void OnNewProj(object? sender, GetDataHandlers.NewProjectileEventArgs e)
    {
        PlayerSparkSkillHandler.Adapter(e, Config.SkillContexts[0], Enumerates.SkillSparkType.Take);
    }

    private void OnConnent(ConnectEventArgs args)
    {
        
    }

    private void OnMP(object? sender, GetDataHandlers.PlayerManaEventArgs e)
    {
        PlayerSparkSkillHandler.Adapter(e.Player, Config.SkillContexts[0], Enumerates.SkillSparkType.MP);
    }

    private void OnHP(object? sender, GetDataHandlers.PlayerHPEventArgs e)
    {
        PlayerSparkSkillHandler.Adapter(e.Player, Config.SkillContexts[0], Enumerates.SkillSparkType.HP);
    }

    private void OnStrike(NpcStrikeEventArgs args)
    {
        var esplayer = ESPlayers[args.Player.whoAmI];
        esplayer.StrikeNpc.TrySetResult(args.Npc);
        //PlayerSparkSkillHandler.Adapter(TShock.Players[args.Player.whoAmI], Config.SkillContexts[0], Enumerates.SkillSparkType.Strike);
    }

    private void OnKillNpc(PlayerKillNpcArgs args)
    {
        var esplayer = ESPlayers[args.Player.Index];
        esplayer.KillNpc.TrySetResult(args.Npc);
        //PlayerSparkSkillHandler.Adapter(args.Player, Config.SkillContexts[0], Enumerates.SkillSparkType.Kill);
    }

    private void OnPlayerUpdate(object? sender, GetDataHandlers.PlayerUpdateEventArgs e)
    {
        if (e.Player.TPlayer.controlUseItem && e.Player.TPlayer.HeldItem.shoot == 0)
        {
            PlayerSparkSkillHandler.Adapter(e.Player, Config.SkillContexts[0], Enumerates.SkillSparkType.Take);
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
