using Economics.Core.Model;
using MySql.Data.MySqlClient;
using System.Data;
using TShockAPI;
using TShockAPI.DB;

namespace Economics.Core.DB;

internal class CurrencyManager
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
        using var reader = this._database.QueryReader("SELECT * FROM economics");
        while (reader.Read())
        {
            this._currencies.Add(new PlayerCurrencyInfo
            {
                Number = reader.Get<long>("currency"),
                PlayerName = reader.Get<string>("username"),
                CurrencyType = reader.Get<string>("type")
            });
        }
    }

    public List<PlayerCurrencyInfo> GetCurrencies()
    {
        return this._currencies;
    }

    public PlayerCurrencyInfo[] GetPlayerCurrencies(string name)
    {
        return [.. this._currencies.FindAll(x => x.PlayerName == name)];
    }

    public PlayerCurrencyInfo GetUserCurrency(string name, string type)
    {
        return this._currencies.Find(x => x.PlayerName == name && x.CurrencyType == type)
        ?? new PlayerCurrencyInfo { Number = 0, PlayerName = name, CurrencyType = type };
    }

    public void AddUserCurrency(string name, long amount, string type)
    {
        var currency = this._currencies.Find(x => x.PlayerName == name && x.CurrencyType == type);
        if (currency is null)
        {
            currency = new PlayerCurrencyInfo { Number = amount, PlayerName = name, CurrencyType = type };
            this._currencies.Add(currency);
        }
        else
        {
            currency.Number += amount;
        }
    }

    public bool DeductUserCurrency(string name, long amount, string type)
    {
        if (amount == 0)
        {
            return true;
        }

        var currency = this.GetUserCurrency(name, type);
        if (currency.Number >= amount)
        {
            currency.Number -= amount;
            return true;
        }
        return false;
    }

    public void ClearUserCurrency(string name, string type)
    {
        GetUserCurrency(name, type).Number = 0;
    }

    public void Reset()
    {
        _database.Query("DELETE FROM economics");
        _currencies.Clear();
    }
}
