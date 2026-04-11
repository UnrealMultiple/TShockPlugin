using LazyAPI.Database;
using LinqToDB;
using LinqToDB.Mapping;
using TShockAPI;

namespace DeltaForce.Core.Database;

[Table("deltaforce_marketplace")]
public class MarketplaceItem : RecordBase<MarketplaceItem>
{
    [PrimaryKey, Identity]
    [Column("id")]
    public int Id { get; set; }

    [Column("seller_name")]
    public string SellerName { get; set; } = string.Empty;

    [Column("item_type")]
    public int ItemType { get; set; }

    [Column("item_name")]
    public string ItemName { get; set; } = string.Empty;

    [Column("stack")]
    public int Stack { get; set; } = 1;

    [Column("prefix")]
    public byte Prefix { get; set; } = 0;

    [Column("price")]
    public long Price { get; set; } = 0;

    [Column("is_sold")]
    public bool IsSold { get; set; } = false;

    [Column("buyer_name")]
    public string? BuyerName { get; set; }

    [Column("listed_at")]
    public DateTime ListedAt { get; set; } = DateTime.Now;

    [Column("sold_at")]
    public DateTime? SoldAt { get; set; }

    public static List<MarketplaceItem> GetActiveListings()
    {
        try
        {
            using var db = Db.Context<MarketplaceItem>();
            return db.Records.Where(m => !m.IsSold).OrderByDescending(m => m.ListedAt).ToList();
        }
        catch (Exception ex)
        {
            TShock.Log.Error($"[三角洲交易行] 获取上架物品时发生错误: {ex}");
            return [];
        }
    }

    public static List<MarketplaceItem> GetPlayerListings(string playerName)
    {
        try
        {
            using var db = Db.Context<MarketplaceItem>();
            return db.Records.Where(m => m.SellerName == playerName && !m.IsSold).OrderByDescending(m => m.ListedAt).ToList();
        }
        catch (Exception ex)
        {
            TShock.Log.Error($"[三角洲交易行] 获取玩家 {playerName} 的上架物品时发生错误: {ex}");
            return [];
        }
    }

    public static MarketplaceItem? GetListingById(int id)
    {
        try
        {
            using var db = Db.Context<MarketplaceItem>();
            return db.Records.FirstOrDefault(m => m.Id == id && !m.IsSold);
        }
        catch (Exception ex)
        {
            TShock.Log.Error($"[三角洲交易行] 获取物品 {id} 时发生错误: {ex}");
            return null;
        }
    }

    public static bool ListItem(string sellerName, int itemType, string itemName, int stack, byte prefix, long price)
    {
        try
        {
            using var db = Db.Context<MarketplaceItem>();
            var listing = new MarketplaceItem
            {
                SellerName = sellerName,
                ItemType = itemType,
                ItemName = itemName,
                Stack = stack,
                Prefix = prefix,
                Price = price,
                IsSold = false,
                ListedAt = DateTime.Now
            };
            db.Insert(listing);
            return true;
        }
        catch (Exception ex)
        {
            TShock.Log.Error($"[三角洲交易行] 上架物品时发生错误: {ex}");
            return false;
        }
    }

    public static bool BuyItem(int itemId, string buyerName)
    {
        try
        {
            using var db = Db.Context<MarketplaceItem>();
            var item = db.Records.FirstOrDefault(m => m.Id == itemId && !m.IsSold);

            if (item == null)
            {
                return false;
            }

            if (item.SellerName == buyerName)
            {
                return false;
            }

            if (!PlayerCurrency.DeductHavco(buyerName, item.Price))
            {
                return false;
            }

            if (!PlayerCurrency.AddHavco(item.SellerName, item.Price))
            {
                PlayerCurrency.AddHavco(buyerName, item.Price);
                return false;
            }

            item.IsSold = true;
            item.BuyerName = buyerName;
            item.SoldAt = DateTime.Now;
            db.Update(item);

            TShock.Log.ConsoleInfo($"[三角洲交易行] {buyerName} 购买了 {item.SellerName} 的 {item.ItemName} x{item.Stack}，价格: {item.Price} 哈夫币");
            return true;
        }
        catch (Exception ex)
        {
            TShock.Log.Error($"[三角洲交易行] 购买物品 {itemId} 时发生错误: {ex}");
            return false;
        }
    }

    public static bool CancelListing(int itemId, string sellerName)
    {
        try
        {
            using var db = Db.Context<MarketplaceItem>();
            var item = db.Records.FirstOrDefault(m => m.Id == itemId && !m.IsSold && m.SellerName == sellerName);

            if (item == null)
            {
                return false;
            }

            db.Delete(item);
            TShock.Log.ConsoleInfo($"[三角洲交易行] {sellerName} 取消了 {item.ItemName} x{item.Stack} 的上架");
            return true;
        }
        catch (Exception ex)
        {
            TShock.Log.Error($"[三角洲交易行] 取消上架 {itemId} 时发生错误: {ex}");
            return false;
        }
    }

    public static List<MarketplaceItem> SearchItems(string keyword)
    {
        try
        {
            using var db = Db.Context<MarketplaceItem>();
            return db.Records.Where(m => !m.IsSold && m.ItemName.ToLower().Contains(keyword.ToLower()))
                         .OrderByDescending(m => m.ListedAt)
                         .ToList();
        }
        catch (Exception ex)
        {
            TShock.Log.Error($"[三角洲交易行] 搜索物品时发生错误: {ex}");
            return [];
        }
    }
}
