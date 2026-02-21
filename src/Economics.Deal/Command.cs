using Economics.Core.Command;
using Economics.Core.ConfigFiles;
using Microsoft.Xna.Framework;
using TShockAPI;

namespace Economics.Deal;

public class Command : BaseCommand
{
    public override string[] Alias => ["deal"];

    public override List<string> Permissions => ["economics.deal.use"];

    public override string HelpText => GetString("此命令可以交易物品!");

    public override string ErrorText => GetString("语法错误，请输入/deal help查看正确使用方法!");

    [SubCommand("list")]
    public static void DealList(CommandArgs args)
    {
        void Show(List<string> line)
        {
            if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out var pageNumber))
            {
                return;
            }

            PaginationTools.SendPage(
                    args.Player,
                    pageNumber,
                    line,
                    new PaginationTools.Settings
                    {
                        MaxLinesPerPage = Config.Instance.PageMax,
                        NothingToDisplayString = GetString("当前没有交易物品"),
                        HeaderFormat = GetString("交易列表 ({0}/{1})："),
                        FooterFormat = GetString("输入 {0}deal list {{0}} 查看更多").SFormat(Commands.Specifier)
                    }
                );
        }
        var lines = new List<string>();
        var index = 1;
        foreach (var DealContext in Config.Instance.DealContexts)
        {
            lines.Add(string.Format(GetString("{0}：{1} 发布者：{2} 价格{3}")
                , index.Color(Utils.RedHighlight)
                , DealContext.Item.ToString()
                , DealContext.Publisher.Color(Utils.PinkHighlight)
                , DealContext.RedemptionRelationships.ToString().Color(Utils.GreenHighlight)));
            index++;
        }
        Show(lines);
    }

    [SubCommand("help")]
    public static void DealHelp(CommandArgs args)
    {
        args.Player.SendInfoMessage(GetString("/deal命令帮助"));
        args.Player.SendInfoMessage(GetString("/deal push [价格] [货币类型]"));
        args.Player.SendInfoMessage(GetString("/deal buy [ID]"));
        args.Player.SendInfoMessage(GetString("/deal recall [ID]"));
        args.Player.SendInfoMessage(GetString("/deal list"));
    }

    [SubCommand("push", 3)]
    [OnlyPlayer]
    [HelpText("/deal push <num> <currency>")]
    public static void DealPush(CommandArgs args)
    {
        if (args.Player.SelectedItem.stack == 0)
        {
            args.Player.SendErrorMessage(GetString("你手持物品为空!"));
            return;
        }
        if (!long.TryParse(args.Parameters[1], out var cost) || cost < 0)
        {
            args.Player.SendErrorMessage(GetString("请输入一个正确的价格!"));
            return;
        }
        if (!Core.ConfigFiles.Setting.Instance.HasCustomizeCurrency(args.Parameters[2]))
        {
            args.Player.SendErrorMessage(GetString($"货币类型{args.Parameters[2]}不存在!"));
            return;
        }
        Config.Instance.PushItem(args.Player, cost, args.Parameters[2]);
        args.Player.SendSuccessMessage(GetString("发布成功"));
        TShock.Utils.Broadcast(GetString($"玩家`{args.Player.Name}`发布了一个交易物品: [i/s{args.Player.SelectedItem.stack}:{args.Player.SelectedItem.type}] 价格: {args.Parameters[2]}x{cost}"), Color.DarkGreen);
        args.Player.SelectedItem.stack = 0;
        args.Player.SendData(PacketTypes.PlayerSlot, "", args.Player.Index, args.Player.TPlayer.selectedItem);
    }

    [SubCommand("buy", 2)]
    [HelpText("/bank buy <id>")]
    [OnlyPlayer]
    public static void DealBuy(CommandArgs args)
    {
        if (!int.TryParse(args.Parameters[1], out var index) || index <= 0)
        {
            args.Player.SendErrorMessage(GetString("请输入一个正确的ID"));
            return;
        }
        var context = Config.Instance.GetDealContext(index);
        if (context == null)
        {
            args.Player.SendErrorMessage(GetString("不存在此交易!"));
            return;
        }
        if (!Core.Economics.CurrencyManager.DeductUserCurrency(args.Player.Name, context.RedemptionRelationships))
        {
            args.Player.SendErrorMessage(GetString($"你的{context.RedemptionRelationships.CurrencyType}不足，无法购买!"));
            return;
        }
        Core.Economics.CurrencyManager.AddUserCurrency(context.Publisher, context.RedemptionRelationships);
        args.Player.GiveItem(context.Item.netID, context.Item.Stack, context.Item.Prefix);
        args.Player.SendSuccessMessage(GetString("交易成功!"));
        TShock.Utils.Broadcast(string.Format(GetString("玩家{0}购买了{1}发布的物品{2}!"), args.Player.Name, context.Publisher, context.Item.ToString()), Color.OrangeRed);
        Config.Instance.RemoveItem(index - 1);
    }

    [SubCommand("recall", 2)]
    [HelpText("/bank recall <id>")]
    [OnlyPlayer]
    public static void DealRecall(CommandArgs args)
    {
        if (!int.TryParse(args.Parameters[1], out var index) || index <= 0)
        {
            args.Player.SendErrorMessage(GetString("请输入一个正确的ID"));
            return;
        }
        var context = Config.Instance.GetDealContext(index);
        if (context == null)
        {
            args.Player.SendErrorMessage(GetString("不存在此交易!"));
            return;
        }
        if (context.Publisher != args.Player.Name)
        {
            args.Player.SendErrorMessage(GetString("该交易不是你发布的无法撤回!"));
            return;
        }
        args.Player.SendSuccessMessage(GetString("撤回成功!"));
        TShock.Utils.Broadcast(string.Format(GetString("玩家{0}撤回了发布的物品{1}!"), args.Player.Name, context.Item.ToString()), Color.OrangeRed);
        args.Player.GiveItem(context.Item.netID, context.Item.Stack, context.Item.Prefix);
        Config.Instance.RemoveItem(index - 1);
    }

    [SubCommand("reset")]
    [CommandPermission("economics.deal.reset")]
    public static void DealReset(CommandArgs args)
    {
        Config.Instance.DealContexts.Clear();
        Config.Save();
        args.Player.SendInfoMessage(GetString("交易已重置!"));
    }
}