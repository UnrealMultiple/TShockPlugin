using Economics.Core.ConfigFiles;
using Economics.Core.Extensions;
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
    public override Version Version => new Version(2, 0, 0, 4);

    public Plugin(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        Config.Load();
        Core.Events.PlayerHandler.OnPlayerKillNpc += this.OnPlayerKillNpc;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Core.Economics.RemoveAssemblyCommands(Assembly.GetExecutingAssembly());
            Core.Economics.RemoveAssemblyRest(Assembly.GetExecutingAssembly());
            Core.Events.PlayerHandler.OnPlayerKillNpc -= this.OnPlayerKillNpc;
            Config.UnLoad();
        }
        base.Dispose(disposing);
    }

    private void OnPlayerKillNpc(Core.EventArgs.PlayerEventArgs.PlayerKillNpcArgs args)
    {
        if (args.Npc == null || args.Player == null)
        {
            return;
        }

        if (Config.Instance.AllocationRatio.TryGetValue(args.Npc.netID, out var ra) && ra != null)
        {
            if (!args.Player.InProgress(ra.Progress))
            {
                return;
            }

            double rw = args.Damage / args.Npc.lifeMax;
            foreach (var option in Core.ConfigFiles.Setting.Instance.CustomizeCurrencys)
            {
                if (option.CurrencyObtain.CurrencyObtainType == Core.Enumerates.CurrencyObtainType.KillNpc)
                {
                    var Curr = Convert.ToInt64(rw * ra.AllocationRatio);
                    Core.Economics.CurrencyManager.AddUserCurrency(args.Player.Name, Curr, option.Name);
                    args.Player.SendCombatMsg($"+{Curr}$", new Color(option.CombatMsgOption.Color[0], option.CombatMsgOption.Color[1], option.CombatMsgOption.Color[2]));
                }
            }
            args.Handler = true;
            return;
        }

        var cfg = Config.Instance.NPCS.Find(f => f.ID == args.Npc.netID);
        if (cfg != null)
        {
            if (cfg.DynamicPartition)
            {
                var rw = args.Damage / args.Npc.lifeMax;
                foreach (var option in cfg.ExtraReward)
                {
                    var Curr = Convert.ToInt64(Math.Round(rw * option.Number));
                    Core.Economics.CurrencyManager.AddUserCurrency(args.Player.Name, Curr, option.CurrencyType);
                    if (Config.Instance.Prompt)
                    {
                        args.Player.SendInfoMessage(Config.Instance.PromptText, args.Npc.GetFullNetName(), option.CurrencyType, Curr);
                    }
                }
            }
            else
            {
                foreach (var option in cfg.ExtraReward)
                {
                    Core.Economics.CurrencyManager.AddUserCurrency(args.Player.Name, option);
                    if (Config.Instance.Prompt)
                    {
                        args.Player.SendInfoMessage(Config.Instance.PromptText, args.Npc.GetFullNetName(), option.CurrencyType, option.Number);
                    }
                }

            }
        }
    }
}