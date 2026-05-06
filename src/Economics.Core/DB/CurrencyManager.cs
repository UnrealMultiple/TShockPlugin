using Economics.Core.Model;
using MySql.Data.MySqlClient;
using System.Data;
using TShockAPI;
using TShockAPI.DB;

namespace Economics.Core.DB;

internal sealed class CurrencyManager
{
    private readonly List<PlayerCurrencyInfo> _currencies = [];
    private readonly IDbConnection _database;

    internal CurrencyManager()
    {
        this._database = TShock.DB;
        this.InitializeDatabase();
        this.LoadCurrencies();
    }

    private void InitializeDatabase()
    {
        var table = new SqlTable("economics",
            new SqlColumn("id", MySqlDbType.Int32) { Length = 8, Primary = true, Unique = true, AutoIncrement = true },
            new SqlColumn("username", MySqlDbType.Text) { Length = 500 },
            new SqlColumn("currency", MySqlDbType.Int64) { Length = 255 },
            new SqlColumn("type", MySqlDbType.Text) { Length = 255 });
        var creator = new SqlTableCreator(this._database, this._database.GetSqlQueryBuilder());
        creator.EnsureTableStructure(table);
    }

    private void LoadCurrencies()
    {
        try
        {
            using var reader = this._database.QueryReader("SELECT * FROM economics");
            while (reader.Read())
            {
                this._currencies.Add(new PlayerCurrencyInfo
                {
                    Id = reader.Get<int>("id"),
                    Number = reader.Get<long>("currency"),
                    PlayerName = reader.Get<string>("username"),
                    CurrencyType = reader.Get<string>("type")
                });
            }
        }
        catch (Exception ex)
        {
            TShock.Log.Error(GetString($"加载货币数据失败: {ex.Message}"));
            throw;
        }
    }

    internal IReadOnlyList<PlayerCurrencyInfo> GetAllCurrencies()
    {
        return this._currencies.AsReadOnly();
    }

    internal PlayerCurrencyInfo[] GetPlayerCurrencies(string playerName)
    {
        ArgumentException.ThrowIfNullOrEmpty(playerName);
        return [.. this._currencies.FindAll(x => x.PlayerName == playerName)];
    }

    internal long GetBalance(string playerName, string currencyType)
    {
        ArgumentException.ThrowIfNullOrEmpty(playerName);
        ArgumentException.ThrowIfNullOrEmpty(currencyType);

        var currency = this._currencies.Find(x => x.PlayerName == playerName && x.CurrencyType == currencyType);
        return currency?.Number ?? 0;
    }

    internal void AddCurrency(string playerName, long amount, string currencyType)
    {
        ArgumentException.ThrowIfNullOrEmpty(playerName);
        ArgumentException.ThrowIfNullOrEmpty(currencyType);
        ArgumentOutOfRangeException.ThrowIfNegative(amount);

        if (amount == 0) return;

        var currency = this._currencies.Find(x => x.PlayerName == playerName && x.CurrencyType == currencyType);
        if (currency == null)
        {
            currency = new PlayerCurrencyInfo
            {
                Number = amount,
                PlayerName = playerName,
                CurrencyType = currencyType
            };
            this._currencies.Add(currency);
            this.InsertCurrency(currency);
        }
        else
        {
            currency.Number += amount;
            this.UpdateCurrency(currency);
        }
    }

    internal bool DeductCurrency(string playerName, long amount, string currencyType)
    {
        var result = this.TryDeductCurrency(playerName, amount, currencyType);
        return result.IsSuccess;
    }

    internal DeductResult TryDeductCurrency(string playerName, long amount, string currencyType)
    {
        ArgumentException.ThrowIfNullOrEmpty(playerName);
        ArgumentException.ThrowIfNullOrEmpty(currencyType);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);

        var currency = this._currencies.Find(x => x.PlayerName == playerName && x.CurrencyType == currencyType);
        if (currency == null)
        {
            return DeductResult.Failure(0);
        }

        if (currency.Number < amount)
        {
            return DeductResult.Failure(currency.Number);
        }

        currency.Number -= amount;
        this.UpdateCurrency(currency);
        return DeductResult.Success(currency.Number);
    }

    internal void ClearCurrency(string playerName, string currencyType)
    {
        ArgumentException.ThrowIfNullOrEmpty(playerName);
        ArgumentException.ThrowIfNullOrEmpty(currencyType);

        var currency = this._currencies.Find(x => x.PlayerName == playerName && x.CurrencyType == currencyType);
        if (currency == null)
        {
            return;
        }

        currency.Number = 0;
        this.UpdateCurrency(currency);
    }

    internal void ClearAllCurrencies(string playerName)
    {
        ArgumentException.ThrowIfNullOrEmpty(playerName);

        var playerCurrencies = this._currencies.FindAll(x => x.PlayerName == playerName);
        foreach (var currency in playerCurrencies)
        {
            currency.Number = 0;
            this.UpdateCurrency(currency);
        }
    }

    internal void Reset()
    {
        try
        {
            this._database.Query("DELETE FROM economics");
            this._currencies.Clear();
        }
        catch (Exception ex)
        {
            TShock.Log.Error(GetString($"重置货币数据失败: {ex.Message}"));
            throw;
        }
    }

    internal void SaveAll()
    {
        using var transaction = this._database.BeginTransaction();
        try
        {
            foreach (var currency in this._currencies)
            {
                this.UpsertCurrency(currency);
            }
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    internal void SavePlayerCurrencies(string playerName)
    {
        ArgumentException.ThrowIfNullOrEmpty(playerName);

        var playerCurrencies = this._currencies.FindAll(x => x.PlayerName == playerName);
        using var transaction = this._database.BeginTransaction();
        try
        {
            foreach (var currency in playerCurrencies)
            {
                this.UpsertCurrency(currency);
            }
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private void UpsertCurrency(PlayerCurrencyInfo currency)
    {
        ArgumentNullException.ThrowIfNull(currency);

        if (currency.Id > 0)
        {
            this.UpdateCurrency(currency);
        }
        else
        {
            this.InsertCurrency(currency);
        }
    }

    private void InsertCurrency(PlayerCurrencyInfo currency)
    {
        ArgumentNullException.ThrowIfNull(currency);

        try
        {
            this._database.Query(
                "INSERT INTO economics (username, currency, type) VALUES (@0, @1, @2)",
                currency.PlayerName, currency.Number, currency.CurrencyType);

            currency.Id = this._database.QueryScalar<int>("SELECT last_insert_rowid()");
        }
        catch (Exception ex)
        {
            TShock.Log.Error(GetString($"插入货币数据失败 [{currency.PlayerName}, {currency.CurrencyType}]: {ex.Message}"));
            throw;
        }
    }

    private void UpdateCurrency(PlayerCurrencyInfo currency)
    {
        ArgumentNullException.ThrowIfNull(currency);

        if (currency.Id <= 0)
        {
            throw new InvalidOperationException(GetString($"无法更新货币数据：Id 无效 [{currency.PlayerName}, {currency.CurrencyType}]"));
        }

        try
        {
            this._database.Query(
                "UPDATE economics SET currency = @0 WHERE id = @1",
                currency.Number, currency.Id);
        }
        catch (Exception ex)
        {
            TShock.Log.Error(GetString($"更新货币数据失败 [{currency.PlayerName}, {currency.CurrencyType}]: {ex.Message}"));
            throw;
        }
    }
}
