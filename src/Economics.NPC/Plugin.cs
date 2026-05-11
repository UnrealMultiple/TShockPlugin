using Economics.Core;
using Economics.Core.Extensions;
using Microsoft.Xna.Framework;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;

namespace Economics.NPC;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    public override string Author => "少司命，千亦";

    public override string Description => GetString("修改NPC掉落货币!");

    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(3, 0, 0, 0);

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

        // 转换率更改分支：作为 Core 默认 KillNpc 奖励的加成系数（damage × ConversionRate × AllocationRatio）。
        // 命中且玩家进度达标时接管默认结算；其它情况（无条目 / ra 为空 / 进度不达标）不置 Handler，
        // 放行 Core 的默认结算，让玩家至少能拿到基础奖励。
        if (Config.Instance.AllocationRatio.TryGetValue(args.Npc.netID, out var ra)
            && ra != null
            && args.Player.InProgress(ra.Progress))
        {
            foreach (var currency in Core.ConfigFiles.Setting.Instance.Currencies)
            {
                if (currency.CurrencyObtain.CurrencyObtainType != Core.Enumerates.CurrencyObtainType.KillNpc)
                {
                    continue;
                }
                if (currency.CurrencyObtain.ContainsID.Count > 0
                    && !currency.CurrencyObtain.ContainsID.Contains(args.Npc.type))
                {
                    continue;
                }

                var num = Convert.ToInt64(args.Damage * currency.CurrencyObtain.ConversionRate * ra.AllocationRatio);
                Core.Economics.CurrencyService.AddCurrency(args.Player.Name, currency.Name, num);

                if (currency.CombatMsgOption.Enable)
                {
                    args.Player.SendCombatMsg(
                        string.Format(currency.CombatMsgOption.CombatMsg, num),
                        new Color(currency.CombatMsgOption.Color[0], currency.CombatMsgOption.Color[1], currency.CombatMsgOption.Color[2]));
                }
            }
            args.Handler = true;
        }

        // 额外奖励列表分支：不论是否走过转换率分支都追加执行，保证两段配置可以共存。
        var cfg = Config.Instance.NPCS.Find(f => f.ID == args.Npc.netID);
        if (cfg != null)
        {
            if (cfg.DynamicPartition)
            {
                var rw = args.Damage / args.Npc.lifeMax;
                foreach (var option in cfg.ExtraReward)
                {
                    var Curr = Convert.ToInt64(Math.Round(rw * option.Number));
                    Core.Economics.CurrencyService.AddCurrency(args.Player.Name, option.CurrencyType, Curr);
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
                    Core.Economics.CurrencyService.AddCurrency(args.Player.Name, option.CurrencyType, option.Number);
                    if (Config.Instance.Prompt)
                    {
                        args.Player.SendInfoMessage(Config.Instance.PromptText, args.Npc.GetFullNetName(), option.CurrencyType, option.Number);
                    }
                }

            }
        }
    }
}