using CaiBotLite.Models;
using LinqToDB;
using LinqToDB.Common;
using LinqToDB.Data;
using Newtonsoft.Json;
using TShockAPI;
using TShockAPI.DB;
using SqlType = TShockAPI.DB.SqlType;

namespace CaiBotLite;

public static class Database
{
    public static DataConnection Db => new (GetProvider(), TShock.DB.ConnectionString.Replace(",Version=3", ""));

    public static void Init()
    {
        Db.CreateTable<BossKillInfo>(tableOptions: TableOptions.CreateIfNotExists);
        Db.CreateTable<CaiCharacterInfo>(tableOptions: TableOptions.CreateIfNotExists);
        Db.CreateTable<Mail>(tableOptions: TableOptions.CreateIfNotExists);
    }

    private static string GetProvider()
    {
        return TShock.DB.GetSqlType() switch
        {
            SqlType.Mysql => ProviderName.MySql,
            SqlType.Sqlite => ProviderName.SQLiteMS,
            SqlType.Postgres => ProviderName.PostgreSQL,
            _ => ""
        };
    }
}

public class JsonConverter<T>() : ValueConverter<List<T>, string>(v => JsonConvert.SerializeObject(v),
    s => JsonConvert.DeserializeObject<List<T>>(s) ?? new List<T>(),
    true);