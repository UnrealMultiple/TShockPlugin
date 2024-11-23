using Economics.RPG.Extensions;
using EconomicsAPI.Attributes;
using EconomicsAPI.Extensions;
using TShockAPI;

namespace Economics.Shop;

[RegisterSeries]
public class Command
{
    [CommandMap("shop", "economics.shop")]
    public static void CShop(CommandArgs args)
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
                        MaxLinesPerPage = Shop.Config.PageMax,
                        NothingToDisplayString = GetString("当前商店没有商品"),
                        HeaderFormat = GetString("商品列表 ({0}/{1})："),
                        FooterFormat = GetString("输入 {0}shop list {{0}} 查看更多").SFormat(Commands.Specifier)
                    }
                );
        }
        if (args.Parameters.Count >= 1 && args.Parameters[0].ToLower() == "list")
        {
            var lines = new List<string>();
            var index = 1;
            foreach (var product in Shop.Config.Products)
            {
                lines.Add(GetString($"{index}:[{product.Name}]  {string.Join(" ", product.Items.Select(x => x.ToString()))} 价格:{string.Join(" ", product.RedemptionRelationshipsOption.Select(x => $"{x.CurrencyType}x{x.Number}"))})"));
                index++;
            }
            Show(lines);
        }
        else if (args.Parameters.Count >= 2 && args.Parameters[0].ToLower() == "buy")
        {
            if (!int.TryParse(args.Parameters[1], out var index))
            {
                args.Player.SendErrorMessage(GetString("请输入正确的序号!"));
                return;
            }
            var count = 1;
            if (args.Parameters.Count > 2 && !int.TryParse(args.Parameters[2], out count) && count <= 0)
            {
                args.Player.SendErrorMessage(GetString("你输入的购买数量不正确!"));
                return;
            }
            var product = Shop.Config.GetProduct(index);
            if (product == null)
            {
                args.Player.SendErrorMessage(GetString("此商品不存在，请检查序号后重新输入!"));
                return;
            }
            if (!args.Player.InProgress(product.ProgressLimit))
            {
                args.Player.SendErrorMessage(GetString($"购买此商品需满足进度条件: {string.Join(",", product.ProgressLimit)}"));
                return;
            }
            if (!args.Player.InLevel(product.LevelLimit))
            {
                args.Player.SendErrorMessage(GetString($"购买此商品需达到以下等级之一: {string.Join(",", product.LevelLimit)}"));
                return;
            }
            if (!Shop.HasItem(args.Player, product.ItemTerm))
            {
                args.Player.SendErrorMessage(GetString($"请满足物品条件: {string.Join(",", product.ItemTerm.Select(x => x.ToString()))}"));
                return;
            }
            if (!EconomicsAPI.Economics.CurrencyManager.DeductUserCurrency(args.Player.Name, product.RedemptionRelationshipsOption, count))
            {
                args.Player.SendErrorMessage(GetString($"你的货币不足!"));
                return;
            }
            for (var i = 0; i < count; i++)
            {
                args.Player.GiveItems(product.Items);
            }

            args.Player.ExecCommand(product.Commamds);
            args.Player.SendSuccessMessage(GetString("购买成功!"));
        }
        else if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "help")
        {
            args.Player.SendInfoMessage(GetString("/shop buy [序号] [数量]"));
            args.Player.SendInfoMessage(GetString("/shop list [序号]"));
        }
        else
        {
            args.Player.SendInfoMessage(GetString("输入/shop help 查看指令帮助"));
        }
    }
}