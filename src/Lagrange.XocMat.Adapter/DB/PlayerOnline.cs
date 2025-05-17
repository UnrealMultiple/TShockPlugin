using MySql.Data.MySqlClient;
using System.Data;
using TShockAPI;
using TShockAPI.DB;

namespace Lagrange.XocMat.Adapter.DB;

public class PlayerOnline : Dictionary<string, int>
{
    private readonly HashSet<string> _players = new();
    public new int this[string key]
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
    public PlayerOnline()
    {
        this.database = TShock.DB;
        var Skeleton = new SqlTable("BotOnlineDuration",
            new SqlColumn("username", MySqlDbType.Text) { Length = 500 },
            new SqlColumn("duration", MySqlDbType.Int32) { Length = 255 }
              );
        var List = new SqlTableCreator(this.database, this.database.GetSqlQueryBuilder());
        List.EnsureTableStructure(Skeleton);
        this.ReadAll();
    }

    public void ReadAll()
    {
        using var reader = this.database.QueryReader("SELECT * FROM BotOnlineDuration");
        while (reader.Read())
        {
            string username = reader.Get<string>("username");
            int duration = reader.Get<int>("duration");
            this[username] = duration;
            this._players.Add(username);
        }
    }

    public bool Read(string name, out int duration)
    {
        using var reader = this.database.QueryReader("SELECT * FROM `BotOnlineDuration` WHERE `username` LIKE @0", name);
        if (reader.Read())
        {
            duration = reader.Get<int>("duration");
            return true;
        }
        else
        {
            duration = 0;
            return false;
        }
    }

    public bool Update(string Name, int duration)
    {
        return 1 == this.database.Query("UPDATE `BotOnlineDuration` SET `duration` = @0 WHERE `BotOnlineDuration`.`username` = @1", duration, Name);

    }
    public bool Insert(string Name, int duration)
    {
        this._players.Add(Name);
        return 1 == this.database.Query("INSERT INTO `BotOnlineDuration` (`username`, `duration`) VALUES (@0, @1)", Name, duration);
    }




    public void AddOrUpdate(string name, int duration)
    {
        if (this._players.Contains(name))
            this.Update(name, duration);
        else
            this.Insert(name, duration);
    }

    public void UpdateAll()
    {
        this.ForEach(x => this.AddOrUpdate(x.Key, x.Value));
    }
}
