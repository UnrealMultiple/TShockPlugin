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

    public override string Description => GetString("修改NPC掉落货币!");

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(2, 0, 0, 1);

    internal static string PATH = Path.Combine(EconomicsAPI.Economics.SaveDirPath, "NPC.json");

    private static Config Config = new();

    public Plugin(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        this.LoadConfig();
        EconomicsAPI.Events.PlayerHandler.OnPlayerKillNpc += this.OnPlayerKillNpc;
        GeneralHooks.ReloadEvent += this.LoadConfig;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            EconomicsAPI.Economics.RemoveAssemblyCommands(Assembly.GetExecutingAssembly());
            EconomicsAPI.Economics.RemoveAssemblyRest(Assembly.GetExecutingAssembly());
            EconomicsAPI.Events.PlayerHandler.OnPlayerKillNpc -= this.OnPlayerKillNpc;
            GeneralHooks.ReloadEvent -= this.LoadConfig;
        }
        base.Dispose(disposing);
    }

    private void LoadConfig(ReloadEventArgs? args = null)
    {
        if (!File.Exists(PATH))
        {
            Config.NPCS.Add(new());
        }
        Config = ConfigHelper.LoadConfig(PATH, Config);
    }

    private void OnPlayerKillNpc(EconomicsAPI.EventArgs.PlayerEventArgs.PlayerKillNpcArgs args)
    {
        if (args.Npc == null || args.Player == null)
        {
            return;
        }

        if (Config.AllocationRatio.TryGetValue(args.Npc.netID, out var ra) && ra != null)
        {
            if (!args.Player.InProgress(ra.Progress))
            {
                return;
            }

            double rw = args.Damage / args.Npc.lifeMax;
            foreach (var option in EconomicsAPI.Economics.Setting.CustomizeCurrencys)
            {
                if (option.CurrencyObtain.CurrencyObtainType == EconomicsAPI.Enumerates.CurrencyObtainType.KillNpc)
                {
                    var Curr = Convert.ToInt64(rw * ra.AllocationRatio);
                    EconomicsAPI.Economics.CurrencyManager.AddUserCurrency(args.Player.Name, Curr, option.Name);
                    args.Player.SendCombatMsg($"+{Curr}$", new Color(option.CombatMsgOption.Color[0], option.CombatMsgOption.Color[1], option.CombatMsgOption.Color[2]));
                }
            }
            args.Handler = true;
            return;
        }

        var cfg = Config.NPCS.Find(f => f.ID == args.Npc.netID);
        if (cfg != null)
        {
            if (cfg.DynamicPartition)
            {
                var rw = args.Damage / args.Npc.lifeMax;
                foreach (var option in cfg.ExtraReward)
                {
                    var Curr = Convert.ToInt64(Math.Round(rw * option.Number));
                    EconomicsAPI.Economics.CurrencyManager.AddUserCurrency(args.Player.Name, Curr, option.CurrencyType);
                    if (Config.Prompt)
                    {
                        args.Player.SendInfoMessage(Config.PromptText, args.Npc.GetFullNetName(), option.CurrencyType, Curr);
                    }
                }
            }
            else
            {
                foreach (var option in cfg.ExtraReward)
                {
                    EconomicsAPI.Economics.CurrencyManager.AddUserCurrency(args.Player.Name, option);
                    if (Config.Prompt)
                    {
                        args.Player.SendInfoMessage(Config.PromptText, args.Npc.GetFullNetName(), option.CurrencyType, option.Number);
                    }
                }

            }
        }
    }
}