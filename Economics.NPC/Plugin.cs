using EconomicsAPI.Configured;
using EconomicsAPI.Extensions;
using Microsoft.Xna.Framework;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI.Hooks;

namespace Economics.NPC;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    public override string Author => "少司命";

    public override string Description => Assembly.GetExecutingAssembly().GetName().Name!;

    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;

    public override Version Version => Assembly.GetExecutingAssembly().GetName().Version!;

    internal static string PATH = Path.Combine(EconomicsAPI.Economics.SaveDirPath, "NPC.json");

    private static Config Config = new();

    public Plugin(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        LoadConfig();
        EconomicsAPI.Events.PlayerHandler.OnPlayerKillNpc += OnPlayerKillNpc;
        GeneralHooks.ReloadEvent += (_) => LoadConfig();
    }

    private void LoadConfig()
    {
        if(!File.Exists(PATH))
        {
            Config.NPCS.Add(new());
        }
        Config = ConfigHelper.LoadConfig(PATH, Config);
    }

    private void OnPlayerKillNpc(EconomicsAPI.EventArgs.PlayerEventArgs.PlayerKillNpcArgs args)
    {
        if (args.Npc == null || args.Player == null) return;
        if (Config.AllocationRatio.TryGetValue(args.Npc.netID, out double ra))
        {
            double rw = args.Damage / args.Npc.lifeMax;
            long Curr = Convert.ToInt64(rw * ra);
            EconomicsAPI.Economics.CurrencyManager.AddUserCurrency(args.Player.Name, Curr);
            args.Player.SendCombatMsg($"+{Curr}$", Color.AliceBlue);
            args.Handler = true;
            return;
        }

        var cfg = Config.NPCS.Find(f => f.ID == args.Npc.netID);
        if (cfg != null)
        {
            if (cfg.DynamicPartition)
            {
                float rw = args.Damage / args.Npc.lifeMax;
                long Curr = Convert.ToInt64(Math.Round(rw * cfg.ExtraReward));
                EconomicsAPI.Economics.CurrencyManager.AddUserCurrency(args.Player.Name, Curr);
                if (Config.Prompt)
                    args.Player.SendInfoMessage(Config.PromptText, args.Npc.GetFullNetName(), EconomicsAPI.Economics.Setting.CurrencyName, Curr);
            }
            else
            {
                EconomicsAPI.Economics.CurrencyManager.AddUserCurrency(args.Player.Name, cfg.ExtraReward);
                if (Config.Prompt)
                    args.Player.SendInfoMessage(Config.PromptText, args.Npc.GetFullNetName(), EconomicsAPI.Economics.Setting.CurrencyName, cfg.ExtraReward);
            }
        }
    }
}
