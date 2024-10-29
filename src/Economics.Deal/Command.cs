using EconomicsAPI.Attributes;
using EconomicsAPI.Configured;
using Microsoft.Xna.Framework;
using TShockAPI;

namespace Economics.Deal;

[RegisterSeries]
public class Command
{
    [CommandMap("deal", "economics.deal.use")]
    public static void CDeal(CommandArgs args)
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
                        MaxLinesPerPage = Deal.Config.PageMax,
                        NothingToDisplayString = GetString("当前没有交易物品"),
                        HeaderFormat = GetString("交易列表 ({0}/{1})："),
                        FooterFormat = GetString("输入 {0}deal list {{0}} 查看更多").SFormat(Commands.Specifier)
                    }
                );
        }
        if (args.Parameters.Count == 2 && args.Parameters[0].ToLower() == "push")
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
            Deal.Config.PushItem(args.Player, cost);
            args.Player.SendSuccessMessage(GetString("发布成功"));
            TShock.Utils.Broadcast(GetString($"玩家`{args.Player.Name}`发布了一个交易物品: [i/s{args.Player.SelectedItem.stack}:{args.Player.SelectedItem.netID}] 价格: {cost}"), Color.DarkGreen);
            args.Player.SelectedItem.stack = 0;
            args.Player.SendData(PacketTypes.PlayerSlot, "", args.Player.Index, args.Player.TPlayer.selectedItem);
        }
        else if (args.Parameters.Count >= 1 && args.Parameters[0].ToLower() == "list")
        {
            var lines = new List<string>();
            var index = 1;
            foreach (var DealContext in Deal.Config.DealContexts)
            {
                lines.Add(string.Format(GetString("{0}：{1} 发布者：{2} 价格{3}")
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
                args.Player.SendErrorMessage(GetString("请输入一个正确的ID"));
                return;
            }
            var context = Deal.Config.GetDealContext(index);
            if (context == null)
            {
                args.Player.SendErrorMessage(GetString("不存在此交易!"));
                return;
            }
            if (!EconomicsAPI.Economics.CurrencyManager.DeductUserCurrency(args.Player.Name, context.Cost))
            {
                args.Player.SendErrorMessage(GetString($"你的{EconomicsAPI.Economics.Setting.CurrencyName}不足，无法购买!"));
                return;
            }
            EconomicsAPI.Economics.CurrencyManager.AddUserCurrency(context.Publisher, context.Cost);
            args.Player.GiveItem(context.Item.netID, context.Item.Stack, context.Item.Prefix);
            args.Player.SendSuccessMessage(GetString("交易成功!"));
            TShock.Utils.Broadcast(string.Format(GetString("玩家{0}购买了{1}发布的物品{2}!"), args.Player.Name, context.Publisher, context.Item.ToString()), Color.OrangeRed);
            Deal.Config.RemoveItem(index - 1);
        }
        else if (args.Parameters.Count == 2 && args.Parameters[0].ToLower() == "recall")
        {
            if (!int.TryParse(args.Parameters[1], out var index) || index <= 0)
            {
                args.Player.SendErrorMessage(GetString("请输入一个正确的ID"));
                return;
            }
            var context = Deal.Config.GetDealContext(index);
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
            Deal.Config.RemoveItem(index - 1);
        }
        else if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "help")
        {
            args.Player.SendInfoMessage(GetString("/deal push [价格]"));
            args.Player.SendInfoMessage(GetString("/deal buy [ID]"));
            args.Player.SendInfoMessage(GetString("/deal recall [ID]"));
            args.Player.SendInfoMessage(GetString("/deal list"));
        }
        else if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "reset")
        {
            if (!args.Player.HasPermission("economics.deal.reset"))
            {
                args.Player.SendErrorMessage(GetString("你无权使用此命令!"));
                return;
            }
            Deal.Config.DealContexts.Clear();
            ConfigHelper.Write(Deal.PATH, Deal.Config);
            args.Player.SendInfoMessage(GetString("交易已重置!"));
        }
        else
        {
            args.Player.SendInfoMessage(GetString("输入/deal help 查看命令使用方法"));
        }
    }
}