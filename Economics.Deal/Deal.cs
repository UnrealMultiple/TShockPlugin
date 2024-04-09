using EconomicsAPI.Attributes;
using EconomicsAPI.Configured;
using Microsoft.Xna.Framework;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace Economics.Deal;

[ApiVersion(2, 1)]
public class Deal : TerrariaPlugin
{
    public override string Author => "少司命";

    public override string Description => Assembly.GetExecutingAssembly().GetName().Name!;

    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;

    public override Version Version => Assembly.GetExecutingAssembly().GetName().Version!;

    internal static string PATH = Path.Combine(EconomicsAPI.Economics.SaveDirPath, "Deal.json");

    internal static Config Config { get; set; } 

    public Deal(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        Config = ConfigHelper.LoadConfig<Config>(PATH);
        GeneralHooks.ReloadEvent += (e) => Config = ConfigHelper.LoadConfig(PATH, Config);
    }

    [CommandMap("deal", "economics.deal.use")]
    public static void CDeal(CommandArgs args)
    {
        void Show(List<string> line)
        {
            if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out int pageNumber))
                return;

            PaginationTools.SendPage(
                    args.Player,
                    pageNumber,
                    line,
                    new PaginationTools.Settings
                    {
                        MaxLinesPerPage = Config.PageMax,
                        NothingToDisplayString = "当前没有交易物品",
                        HeaderFormat = "交易列表 ({0}/{1})：",
                        FooterFormat = "输入 {0}deal list {{0}} 查看更多".SFormat(Commands.Specifier)
                    }
                );
        }
        if (args.Parameters.Count == 2 && args.Parameters[0].ToLower() == "push")
        {
            if (args.Player.SelectedItem.stack == 0)
            {
                args.Player.SendErrorMessage("你手持物品为空!");
                return;
            }
            if (!long.TryParse(args.Parameters[1], out long cost) || cost < 0)
            {
                args.Player.SendErrorMessage("请输入一个正确的价格!");
                return;
            }
            Config.PushItem(args.Player, cost);
            args.Player.SendSuccessMessage("发布成功");
            TShock.Utils.Broadcast($"玩家`{args.Player.Name}`发布了一个交易物品: [i/s{args.Player.SelectedItem.stack}:{args.Player.SelectedItem.netID}] 价格: {cost}", Color.DarkGreen);
            args.Player.SelectedItem.stack = 0;
            args.Player.SendData(PacketTypes.PlayerSlot, "", args.Player.Index, args.Player.TPlayer.selectedItem);
        }
        else if (args.Parameters.Count >= 1 && args.Parameters[0].ToLower() == "list")
        {
            var lines = new List<string>();
            var index = 1;
            foreach (var DealContext in Config.DealContexts)
            {
                lines.Add(string.Format("{0}：{1} 发布者：{2} 价格{3}"
                    , index.Color(TShockAPI.Utils.RedHighlight)
                    , DealContext.Item.ToString()
                    , DealContext.Publisher.Color(TShockAPI.Utils.PinkHighlight)
                    , DealContext.Cost.Color(TShockAPI.Utils.GreenHighlight)));
                index++;
            }
            
            Show(lines);
        }
        else if (args.Parameters.Count == 2 && args.Parameters[0].ToLower() == "buy")
        {
            if (!int.TryParse(args.Parameters[1], out var index) || index <= 0)
            {
                args.Player.SendErrorMessage("请输入一个正确的ID");
                return;
            }
            var context = Config.GetDealContext(index);
            if (context == null)
            {
                args.Player.SendErrorMessage("不存在此交易!");
                return;
            }
            if (!EconomicsAPI.Economics.CurrencyManager.DelUserCurrency(args.Player.Name, context.Cost))
            {
                args.Player.SendErrorMessage($"你的{EconomicsAPI.Economics.Setting.CurrencyName}不足，无法购买!");
                return;
            }
            EconomicsAPI.Economics.CurrencyManager.AddUserCurrency(context.Publisher, context.Cost);
            args.Player.SendSuccessMessage("交易成功!");
            TShock.Utils.Broadcast(string.Format("玩家{0}购买了{1}发布的物品{2}!", args.Player.Name, context.Publisher, context.Item.ToString()), Color.OrangeRed);
            Config.RemoveItem(index - 1);
        }
        else if (args.Parameters.Count == 2 && args.Parameters[0].ToLower() == "recall")
        {
            if (!int.TryParse(args.Parameters[1], out var index) || index <= 0)
            {
                args.Player.SendErrorMessage("请输入一个正确的ID");
                return;
            }
            var context = Config.GetDealContext(index);
            if (context == null)
            {
                args.Player.SendErrorMessage("不存在此交易!");
                return;
            }
            if (context.Publisher != args.Player.Name)
            {
                args.Player.SendErrorMessage("该交易不是你发布的无法撤回!");
                return;
            }
            args.Player.SendSuccessMessage("撤回成功!");
            TShock.Utils.Broadcast(string.Format("玩家{0}撤回了发布的物品{1}!", args.Player.Name, context.Item.ToString()), Color.OrangeRed);
            args.Player.GiveItem(context.Item.netID, context.Item.Stack, context.Item.Prefix);
            Config.RemoveItem(index - 1);
        }
        else if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "help")
        {
            args.Player.SendInfoMessage("/deal push [价格]");
            args.Player.SendInfoMessage("/deal buy [ID]");
            args.Player.SendInfoMessage("/deal recall [ID]");
            args.Player.SendInfoMessage("/deal list");
        }
        else if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "reset")
        {
            if (!args.Player.HasPermission("economics.deal.reset"))
            {
                args.Player.SendErrorMessage("你无权使用此命令!");
                return;
            }
            Config.DealContexts.Clear();
            ConfigHelper.Write(PATH, Config);
            args.Player.SendInfoMessage("交易已重置!");
        }
        else
        {
            args.Player.SendInfoMessage("输入/deal help 查看命令使用方法");
        }
    }
 }
