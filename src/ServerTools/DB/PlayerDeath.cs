using MySql.Data.MySqlClient;
using System.Data;
using TShockAPI;
using TShockAPI.DB;

namespace ServerTools.DB;

public class PlayerDeath : Dictionary<string, int>
{
    private new int this[string key]
    {
        get
        {
            if (TryGetValue(key, out int result))
                return result;
            return 0;
        }

        set
        {
            base[key] = value;
        }
    }
    private readonly IDbConnection database;
    public PlayerDeath()
    {
        database = TShock.DB;
        var Skeleton = new SqlTable("Death",
            new SqlColumn("Count", MySqlDbType.Int32) { Length = 255 },
            new SqlColumn("Name", MySqlDbType.VarChar) { Length = 255, Unique = true }
              );
        var List = new SqlTableCreator(database, database.GetSqlType() == SqlType.Sqlite ? new SqliteQueryCreator() : new MysqlQueryCreator());
        List.EnsureTableStructure(Skeleton);
        ReadAll();
    }

    private void ReadAll()
    {
        using var reader = database.QueryReader("SELECT * FROM Death");
        while (reader.Read())
        {
            string Name = reader.Get<string>("Name");
            int Count = reader.Get<int>("Count");
            this[Name] = Count;
        }
    }

    public void Add(string name)
    {
        if (ContainsKey(name))
        {
            this[name] += 1;
            database.Query("UPDATE Death SET Count = @0 WHERE Name = @1", this[name], name);
        }
        else
        {
            this[name] = 1;
            database.Query("INSERT INTO `Death` (`Name`, `Count`) VALUES (@0, @1)", name, 1);
        }
    }
}
