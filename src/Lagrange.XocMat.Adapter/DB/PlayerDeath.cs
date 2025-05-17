using MySql.Data.MySqlClient;
using System.Data;
using TShockAPI;
using TShockAPI.DB;

namespace Lagrange.XocMat.Adapter.DB;

public class PlayerDeath : Dictionary<string, int>
{
    private new int this[string key]
    {
        get
        {
            if (this.TryGetValue(key, out int result))
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
        this.database = TShock.DB;
        var Skeleton = new SqlTable("BotDeath",
            new SqlColumn("Count", MySqlDbType.Int32) { Length = 255 },
            new SqlColumn("Name", MySqlDbType.VarChar) { Length = 255, Unique = true }
              );
        var List = new SqlTableCreator(this.database, this.database.GetSqlQueryBuilder());
        List.EnsureTableStructure(Skeleton);
        this.ReadAll();
    }

    private void ReadAll()
    {
        using var reader = this.database.QueryReader("SELECT * FROM BotDeath");
        while (reader.Read())
        {
            string Name = reader.Get<string>("Name");
            int Count = reader.Get<int>("Count");
            this[Name] = Count;
        }
    }

    public void Add(string name)
    {
        if (this.ContainsKey(name))
        {
            this[name] += 1;
            this.database.Query("UPDATE BotDeath SET Count = @0 WHERE Name = @1", this[name], name);
        }
        else
        {
            this[name] = 1;
            this.database.Query("INSERT INTO `BotDeath` (`Name`, `Count`) VALUES (@0, @1)", name, 1);
        }
    }
}
