using LazyAPI.Database;
using LinqToDB;
using LinqToDB.Mapping;
using TShockAPI;

namespace DeltaForce.Core.Database;

[Table("deltaforce_currency")]
public class PlayerCurrency : PlayerRecordBase<PlayerCurrency>
{
    [Column("account_id")]
    public int AccountID { get; set; }

    [Column("havco")]
    public long Havco { get; set; } = 0;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public static PlayerCurrency? GetPlayerCurrency(string playerName)
    {
        try
        {
            using var context = Db.PlayerContext<PlayerCurrency>();
            return context.Get(playerName).FirstOrDefault();
        }
        catch (Exception ex)
        {
            TShock.Log.Error($"[三角洲货币] 获取玩家 {playerName} 哈夫币时发生错误: {ex}");
            return null;
        }
    }

    public static long GetHavco(string playerName)
    {
        var currency = GetPlayerCurrency(playerName);
        return currency?.Havco ?? 0;
    }

    public static bool AddHavco(string playerName, long amount)
    {
        try
        {
            using var context = Db.PlayerContext<PlayerCurrency>();
            var existing = context.Get(playerName).FirstOrDefault();

            if (existing == null)
            {
                var account = TShock.UserAccounts.GetUserAccountByName(playerName);
                if (account == null) return false;

                var newCurrency = new PlayerCurrency
                {
                    Name = playerName,
                    AccountID = account.ID,
                    Havco = amount,
                    UpdatedAt = DateTime.Now
                };
                context.Insert(newCurrency);
            }
            else
            {
                existing.Havco += amount;
                existing.UpdatedAt = DateTime.Now;
                context.Update(existing);
            }

            return true;
        }
        catch (Exception ex)
        {
            TShock.Log.Error($"[三角洲货币] 给玩家 {playerName} 添加哈夫币时发生错误: {ex}");
            return false;
        }
    }

    public static bool DeductHavco(string playerName, long amount)
    {
        try
        {
            using var context = Db.PlayerContext<PlayerCurrency>();
            var existing = context.Get(playerName).FirstOrDefault();

            if (existing == null || existing.Havco < amount)
            {
                return false;
            }

            existing.Havco -= amount;
            existing.UpdatedAt = DateTime.Now;
            context.Update(existing);

            return true;
        }
        catch (Exception ex)
        {
            TShock.Log.Error($"[三角洲货币] 扣除玩家 {playerName} 哈夫币时发生错误: {ex}");
            return false;
        }
    }

    public static bool TransferHavco(string fromPlayer, string toPlayer, long amount)
    {
        if (GetHavco(fromPlayer) < amount)
        {
            return false;
        }

        if (!DeductHavco(fromPlayer, amount))
        {
            return false;
        }

        if (!AddHavco(toPlayer, amount))
        {
            AddHavco(fromPlayer, amount);
            return false;
        }

        return true;
    }

    public static void InitializePlayer(string playerName)
    {
        try
        {
            using var context = Db.PlayerContext<PlayerCurrency>();
            var existing = context.Get(playerName).FirstOrDefault();

            if (existing != null)
            {
                return;
            }

            var account = TShock.UserAccounts.GetUserAccountByName(playerName);
            if (account == null) return;

            var newCurrency = new PlayerCurrency
            {
                Name = playerName,
                AccountID = account.ID,
                Havco = 1000,
                UpdatedAt = DateTime.Now
            };
            context.Insert(newCurrency);
            TShock.Log.ConsoleInfo($"[三角洲货币] 已初始化玩家 {playerName} 的哈夫币，初始金额: 1000");
        }
        catch (Exception ex)
        {
            TShock.Log.Error($"[三角洲货币] 初始化玩家 {playerName} 哈夫币时发生错误: {ex}");
        }
    }
}
