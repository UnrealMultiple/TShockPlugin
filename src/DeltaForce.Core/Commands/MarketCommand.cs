using DeltaForce.Core.Database;
using LazyAPI.Attributes;
using LazyAPI.Utility;
using Microsoft.Xna.Framework;
using TShockAPI;
using TShockAPI.DB;

namespace DeltaForce.Core.Commands;

[Command("market")]
[Permissions("deltaforce.market")]
public class MarketCommand
{
    [Alias("balance", "bal", "money")]
    [RealPlayer]
    public static void CheckBalance(CommandArgs args)
    {
        var player = args.Player;
        var havco = PlayerCurrency.GetHavco(player.Name);
        player.SendSuccessMessage(GetString($"[哈夫币] 你当前拥有 {havco:N0} 哈夫币"));
    }

    [Alias("list", "ls")]
    [RealPlayer]
    public static void ListItems(CommandArgs args)
    {
        var player = args.Player;
        var page = 1;

        if (args.Parameters.Count > 1 && int.TryParse(args.Parameters[1], out var parsedPage))
        {
            page = Math.Max(1, parsedPage);
        }

        var items = MarketplaceItem.GetActiveListings();
        const int itemsPerPage = 10;
        var totalPages = (int)Math.Ceiling(items.Count / (double)itemsPerPage);

        if (items.Count == 0)
        {
            player.SendInfoMessage(GetString("[交易行] 当前没有上架的物品"));
            return;
        }

        var pageItems = items.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

        player.SendInfoMessage(GetString($"[交易行] 上架物品列表 (第 {page}/{totalPages} 页，共 {items.Count} 件)"));
        player.SendInfoMessage(GetString("使用 /market buy <ID> 购买物品"));
        player.SendMessage("", Color.White);

        foreach (var item in pageItems)
        {
            var prefixText = item.Prefix > 0 ? $"[{GetPrefixName(item.Prefix)}] " : "";
            player.SendInfoMessage(GetString($"  ID:{item.Id} | {prefixText}{item.ItemName} x{item.Stack} | 价格: {item.Price:N0} 哈夫币 | 卖家: {item.SellerName}"));
        }
    }

    [Alias("sell", "s")]
    [RealPlayer]
    public static void SellItem(CommandArgs args)
    {
        var player = args.Player;

        if (args.Parameters.Count < 2)
        {
            player.SendErrorMessage(GetString("用法: /market sell <价格> [数量]"));
            player.SendInfoMessage(GetString("将手持物品上架到交易行"));
            return;
        }

        if (!long.TryParse(args.Parameters[1], out var price) || price <= 0)
        {
            player.SendErrorMessage(GetString("价格必须是正整数！"));
            return;
        }

        var stack = 1;
        if (args.Parameters.Count > 2 && int.TryParse(args.Parameters[2], out var parsedStack))
        {
            stack = Math.Max(1, parsedStack);
        }

        var heldItem = player.TPlayer.inventory[player.TPlayer.selectedItem];
        if (heldItem.IsAir)
        {
            player.SendErrorMessage(GetString("你必须手持一个物品才能上架！"));
            return;
        }

        if (heldItem.stack < stack)
        {
            player.SendErrorMessage(GetString($"手持物品数量不足！当前: {heldItem.stack}，需要: {stack}"));
            return;
        }

        if (MarketplaceItem.ListItem(player.Name, heldItem.type, heldItem.Name, stack, heldItem.prefix, price))
        {
            heldItem.stack -= stack;
            if (heldItem.stack <= 0)
            {
                heldItem.TurnToAir();
            }

            player.SendData(PacketTypes.PlayerSlot, "", player.Index, player.TPlayer.selectedItem);
            player.SendSuccessMessage(GetString($"[交易行] 成功上架 {heldItem.Name} x{stack}，价格: {price:N0} 哈夫币"));
            TShock.Utils.Broadcast(GetString($"[交易行] {player.Name} 上架了 {heldItem.Name} x{stack}，价格: {price:N0} 哈夫币"), Color.LightGreen);
        }
        else
        {
            player.SendErrorMessage(GetString("上架失败，请稍后重试！"));
        }
    }

    [Alias("sellnpc", "sn", "vendor", "v")]
    [RealPlayer]
    public static void SellToNPC(CommandArgs args)
    {
        var player = args.Player;

        var stack = 1;
        if (args.Parameters.Count > 1 && int.TryParse(args.Parameters[1], out var parsedStack))
        {
            stack = Math.Max(1, parsedStack);
        }

        var heldItem = player.TPlayer.inventory[player.TPlayer.selectedItem];
        if (heldItem.IsAir)
        {
            player.SendErrorMessage(GetString("你必须手持一个物品才能出售！"));
            return;
        }

        if (heldItem.stack < stack)
        {
            player.SendErrorMessage(GetString($"手持物品数量不足！当前: {heldItem.stack}，需要: {stack}"));
            return;
        }

        var configItem = Config.Instance.Items.FirstOrDefault(i => i.Type == heldItem.type);
        if (configItem.Value <= 0)
        {
            player.SendErrorMessage(GetString("该物品无法出售给系统！"));
            return;
        }

        var totalValue = configItem.Value * stack;

        heldItem.stack -= stack;
        if (heldItem.stack <= 0)
        {
            heldItem.TurnToAir();
        }
        player.SendData(PacketTypes.PlayerSlot, "", player.Index, player.TPlayer.selectedItem);

        if (PlayerCurrency.AddHavco(player.Name, totalValue))
        {
            player.SendSuccessMessage(GetString($"[交易行] 成功出售 {heldItem.Name} x{stack}，获得 {totalValue:N0} 哈夫币"));
            TShock.Log.ConsoleInfo(GetString($"[交易行] {player.Name} 出售了 {heldItem.Name} x{stack}，获得 {totalValue} 哈夫币"));
        }
        else
        {
            player.SendErrorMessage(GetString("出售失败，请稍后重试！"));
        }
    }

    [Alias("buy", "b")]
    [RealPlayer]
    public static void BuyItem(CommandArgs args)
    {
        var player = args.Player;

        if (args.Parameters.Count < 2 || !int.TryParse(args.Parameters[1], out var itemId))
        {
            player.SendErrorMessage(GetString("用法: /market buy <物品ID>"));
            player.SendInfoMessage(GetString("使用 /market list 查看物品ID"));
            return;
        }

        var item = MarketplaceItem.GetListingById(itemId);
        if (item == null)
        {
            player.SendErrorMessage(GetString("该物品不存在或已被购买！"));
            return;
        }

        if (item.SellerName == player.Name)
        {
            player.SendErrorMessage(GetString("你不能购买自己上架的物品！使用 /market cancel <ID> 取消上架。"));
            return;
        }

        var balance = PlayerCurrency.GetHavco(player.Name);
        if (balance < item.Price)
        {
            player.SendErrorMessage(GetString($"哈夫币不足！需要: {item.Price:N0}，你拥有: {balance:N0}"));
            return;
        }

        if (MarketplaceItem.BuyItem(itemId, player.Name))
        {
            var prefixText = item.Prefix > 0 ? $"[{GetPrefixName(item.Prefix)}] " : "";
            player.SendSuccessMessage(GetString($"[交易行] 成功购买 {prefixText}{item.ItemName} x{item.Stack}，花费: {item.Price:N0} 哈夫币"));

            var seller = TShock.Players.FirstOrDefault(p => p?.Name == item.SellerName && p?.Active == true);
            seller?.SendSuccessMessage(GetString($"[交易行] 你的 {item.ItemName} x{item.Stack} 已被 {player.Name} 购买，获得: {item.Price:N0} 哈夫币"));

            TShock.Log.ConsoleInfo(GetString($"[交易行] {player.Name} 购买了 {item.SellerName} 的 {item.ItemName}"));
        }
        else
        {
            player.SendErrorMessage(GetString("购买失败！可能是哈夫币不足或物品已被购买。"));
        }
    }

    [Alias("cancel", "c")]
    [RealPlayer]
    public static void CancelListing(CommandArgs args)
    {
        var player = args.Player;

        if (args.Parameters.Count < 2 || !int.TryParse(args.Parameters[1], out var itemId))
        {
            player.SendErrorMessage(GetString("用法: /market cancel <物品ID>"));
            player.SendInfoMessage(GetString("使用 /market my 查看你上架的物品ID"));
            return;
        }

        var item = MarketplaceItem.GetListingById(itemId);
        if (item == null)
        {
            player.SendErrorMessage(GetString("该物品不存在或已被购买！"));
            return;
        }

        if (item.SellerName != player.Name)
        {
            player.SendErrorMessage(GetString("你只能取消自己上架的物品！"));
            return;
        }

        if (MarketplaceItem.CancelListing(itemId, player.Name))
        {
            var newItem = TShock.Utils.GetItemById(item.ItemType);
            newItem.stack = item.Stack;
            newItem.prefix = item.Prefix;

            player.GiveItem(newItem.type, newItem.stack, newItem.prefix);
            player.SendSuccessMessage(GetString($"[交易行] 成功取消上架 {item.ItemName} x{item.Stack}"));
        }
        else
        {
            player.SendErrorMessage(GetString("取消上架失败，请稍后重试！"));
        }
    }

    [Alias("my", "m")]
    [RealPlayer]
    public static void MyListings(CommandArgs args)
    {
        var player = args.Player;
        var items = MarketplaceItem.GetPlayerListings(player.Name);

        if (items.Count == 0)
        {
            player.SendInfoMessage(GetString("[交易行] 你没有上架任何物品"));
            return;
        }

        player.SendInfoMessage(GetString($"[交易行] 你上架的物品 ({items.Count} 件):"));
        foreach (var item in items)
        {
            var prefixText = item.Prefix > 0 ? $"[{GetPrefixName(item.Prefix)}] " : "";
            player.SendInfoMessage(GetString($"  ID:{item.Id} | {prefixText}{item.ItemName} x{item.Stack} | 价格: {item.Price:N0} 哈夫币"));
        }
    }

    [Alias("search", "find", "f")]
    [RealPlayer]
    public static void SearchItems(CommandArgs args)
    {
        var player = args.Player;

        if (args.Parameters.Count < 2)
        {
            player.SendErrorMessage(GetString("用法: /market search <关键词>"));
            return;
        }

        var keyword = args.Parameters[1];
        var items = MarketplaceItem.SearchItems(keyword);

        if (items.Count == 0)
        {
            player.SendInfoMessage(GetString($"[交易行] 没有找到包含 \"{keyword}\" 的物品"));
            return;
        }

        player.SendInfoMessage(GetString($"[交易行] 搜索 \"{keyword}\" 的结果 ({items.Count} 件):"));
        foreach (var item in items.Take(10))
        {
            var prefixText = item.Prefix > 0 ? $"[{GetPrefixName(item.Prefix)}] " : "";
            player.SendInfoMessage(GetString($"  ID:{item.Id} | {prefixText}{item.ItemName} x{item.Stack} | 价格: {item.Price:N0} 哈夫币 | 卖家: {item.SellerName}"));
        }

        if (items.Count > 10)
        {
            player.SendInfoMessage(GetString($"  ... 还有 {items.Count - 10} 件物品"));
        }
    }

    [Alias("pay", "transfer", "t")]
    [RealPlayer]
    public static void TransferHavco(CommandArgs args)
    {
        var player = args.Player;

        if (args.Parameters.Count < 3)
        {
            player.SendErrorMessage(GetString("用法: /market pay <玩家名> <金额>"));
            return;
        }

        var targetName = args.Parameters[1];
        if (!long.TryParse(args.Parameters[2], out var amount) || amount <= 0)
        {
            player.SendErrorMessage(GetString("金额必须是正整数！"));
            return;
        }

        if (targetName == player.Name)
        {
            player.SendErrorMessage(GetString("你不能给自己转账！"));
            return;
        }

        var target = TShock.UserAccounts.GetUserAccountByName(targetName);
        if (target == null)
        {
            player.SendErrorMessage(GetString($"玩家 {targetName} 不存在！"));
            return;
        }

        var balance = PlayerCurrency.GetHavco(player.Name);
        if (balance < amount)
        {
            player.SendErrorMessage(GetString($"哈夫币不足！需要: {amount:N0}，你拥有: {balance:N0}"));
            return;
        }

        if (PlayerCurrency.TransferHavco(player.Name, targetName, amount))
        {
            player.SendSuccessMessage(GetString($"[哈夫币] 成功转账 {amount:N0} 哈夫币给 {targetName}"));

            var targetPlayer = TShock.Players.FirstOrDefault(p => p?.Name == targetName && p?.Active == true);
            targetPlayer?.SendSuccessMessage(GetString($"[哈夫币] 你收到了来自 {player.Name} 的 {amount:N0} 哈夫币"));

            TShock.Log.ConsoleInfo(GetString($"[哈夫币] {player.Name} 转账 {amount} 哈夫币给 {targetName}"));
        }
        else
        {
            player.SendErrorMessage(GetString("转账失败！请检查哈夫币余额。"));
        }
    }

    [Alias("help", "h")]
    public static void ShowHelp(CommandArgs args)
    {
        var player = args.Player;
        player.SendInfoMessage(GetString("[交易行] 指令列表:"));
        player.SendInfoMessage(GetString("  /market balance - 查看哈夫币余额"));
        player.SendInfoMessage(GetString("  /market list [页码] - 查看上架物品列表"));
        player.SendInfoMessage(GetString("  /market sell <价格> [数量] - 上架手持物品到交易行"));
        player.SendInfoMessage(GetString("  /market sellnpc [数量] - 直接出售手持物品给系统（根据配置价值）"));
        player.SendInfoMessage(GetString("  /market buy <ID> - 购买物品"));
        player.SendInfoMessage(GetString("  /market cancel <ID> - 取消自己上架的物品"));
        player.SendInfoMessage(GetString("  /market my - 查看自己上架的物品"));
        player.SendInfoMessage(GetString("  /market search <关键词> - 搜索物品"));
        player.SendInfoMessage(GetString("  /market pay <玩家> <金额> - 转账哈夫币给其他玩家"));
    }

    private static string GetPrefixName(byte prefixId)
    {
        var prefixNames = new Dictionary<byte, string>
        {
            { 1, "大" }, { 2, "巨大" }, { 3, "危险" }, { 4, "狂野" }, { 5, "鲁莽" },
            { 6, "强力" }, { 7, "神秘" }, { 8, "精巧" }, { 9, "快速" }, { 10, "致命" },
            { 11, "敏捷" }, { 12, "暴怒" }, { 13, "锐利" }, { 14, "优越" }, { 15, "强力" },
            { 16, "迅捷" }, { 17, "致命" }, { 18, "灵活" }, { 19, "残暴" }, { 20, "神级" },
            { 21, "恶魔" }, { 22, "狂热" }, { 23, "坚硬" }, { 24, "守护" }, { 25, "奥术" },
            { 26, "精确" }, { 27, "幸运" }, { 28, "锯齿" }, { 29, "尖刺" }, { 30, "愤怒" },
            { 31, "凶险" }, { 32, "沉重" }, { 33, "稳重" }, { 34, "暴力" }, { 35, "迅捷" },
            { 36, "狂野" }, { 37, "鲁莽" }, { 38, "大胆" }, { 39, "疯狂" }, { 40, "无情" },
            { 41, "神话" }, { 42, "传奇" }, { 43, "虚幻" }, { 44, "稀有" }, { 45, "完整" },
            { 46, "愉悦" }, { 47, "惊人" }, { 48, "可怕" }, { 49, "可怕" }, { 50, "恐怖" },
            { 51, "危险" }, { 52, "锋利" }, { 53, "尖锐" }, { 54, "微小" }, { 55, "可怕" },
            { 56, "小" }, { 57, "普通" }, { 58, "大" }, { 59, "巨大" }, { 60, "危险" },
            { 61, "狂野" }, { 62, "鲁莽" }, { 63, "强力" }, { 64, "神秘" }, { 65, "精巧" },
            { 66, "快速" }, { 67, "致命" }, { 68, "敏捷" }, { 69, "暴怒" }, { 70, "锐利" },
            { 71, "优越" }, { 72, "强力" }, { 73, "迅捷" }, { 74, "致命" }, { 75, "灵活" },
            { 76, "残暴" }, { 77, "神级" }, { 78, "恶魔" }, { 79, "狂热" }, { 80, "坚硬" },
            { 81, "守护" }, { 82, "奥术" }
        };

        return prefixNames.TryGetValue(prefixId, out var name) ? name : GetString($"前缀{prefixId}");
    }
}
