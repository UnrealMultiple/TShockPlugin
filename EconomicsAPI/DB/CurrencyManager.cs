using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TShockAPI.DB;

namespace EconomicsAPI.DB;

public class CurrencyManager
{
    private readonly Dictionary<string, long> Currency = new();

    public IDbConnection database;

    internal CurrencyManager()
    {
        database = TShock.DB;
        var Skeleton = new SqlTable("Economics",
            new SqlColumn("ID", MySqlDbType.Int32) { Length = 8, Primary = true, Unique = true, AutoIncrement = true },
            new SqlColumn("UserName", MySqlDbType.Text) { Length = 500 },
            new SqlColumn("Currency", MySqlDbType.Int64) { Length = 255 }
              );
        var List = new SqlTableCreator(database, database.GetSqlType() == SqlType.Sqlite ? new SqliteQueryCreator() : new MysqlQueryCreator());
        List.EnsureTableStructure(Skeleton);
        using (var reader = database.QueryReader("SELECT * FROM Economics"))
        {
            while (reader.Read())
            {
                string username = reader.Get<string>("UserName");
                long currency = reader.Get<long>("Currency");
                Currency[username] = currency;
            }
        }
    }

    public void UpdataAll()
    {
        foreach (var name in Currency.Keys)
        { 
            if(!Update(name))
                Write(name);
        }
    }

    public bool Update(string UserName)
    {
        return database.Query("UPDATE `economics` SET `Currency` = @0 WHERE `economics`.`UserName` = @1", GetUserCurrency(UserName), UserName) == 1;
    }
    public bool Write(string UserName)
    {
        return database.Query("INSERT INTO `economics` (`ID`, `UserName`, `Currency`) VALUES (NULL, @0, @1)", UserName, GetUserCurrency(UserName)) == 1;
    }

    public long GetUserCurrency(string name)
    {
        if(Currency.TryGetValue(name, out var currency))
            return currency;
        return 0;
    }

    public void AddUserCurrency(string name, long num)
    {
        if (Currency.ContainsKey(name))
            Currency[name] += num;
        else
            Currency[name] = num;
    }

    public void ClearUserCurrency(string name)
    {
        Currency[name] = 0;
    }

    public bool DelUserCurrency(string name, long num)
    {
        if (GetUserCurrency(name) >= num)
        {
            Currency[name] -= num;
            return true;
        }
        return false;
    }

    public void Reset()
    {
        database.Query("delete from Economics");
        Currency.Clear();
    }
}
