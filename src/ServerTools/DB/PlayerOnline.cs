using MySql.Data.MySqlClient;
using System.Data;
using TShockAPI;
using TShockAPI.DB;

namespace ServerTools.DB;

public class PlayerOnline : Dictionary<string, int>
{
    private readonly HashSet<string> _players = new();
    public new int this[string key]
    {
        get => this.TryGetValue(key, out var result) ? result : 0;

        set => base[key] = value;
    }
    private readonly IDbConnection database;
    public PlayerOnline()
    {
        this.database = TShock.DB;
        var Skeleton = new SqlTable("OnlineDuration",
            new SqlColumn("username", MySqlDbType.Text) { Length = 500 },
            new SqlColumn("duration", MySqlDbType.Int32) { Length = 255 }
              );
        var List = new SqlTableCreator(this.database, this.database.GetSqlType() == SqlType.Sqlite ? new SqliteQueryCreator() : new MysqlQueryCreator());
        List.EnsureTableStructure(Skeleton);
        this.ReadAll();
    }

    public void ReadAll()
    {
        using var reader = this.database.QueryReader("SELECT * FROM OnlineDuration");
        while (reader.Read())
        {
            var username = reader.Get<string>("username");
            var duration = reader.Get<int>("duration");
            this[username] = duration;
            this._players.Add(username);
        }
    }

    public bool Read(string name, out int duration)
    {
        using var reader = this.database.QueryReader("SELECT * FROM `OnlineDuration` WHERE `username` LIKE @0", name);
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
        return 1 == this.database.Query("UPDATE `OnlineDuration` SET `duration` = @0 WHERE `OnlineDuration`.`username` = @1", duration, Name);

    }
    public bool Insert(string Name, int duration)
    {
        this._players.Add(Name);
        return 1 == this.database.Query("INSERT INTO `OnlineDuration` (`username`, `duration`) VALUES (@0, @1)", Name, duration);
    }




    public void AddOrUpdate(string name, int duration)
    {
        if (this._players.Contains(name))
        {
            this.Update(name, duration);
        }
        else
        {
            this.Insert(name, duration);
        }
    }

    public void UpdateAll()
    {
        this.ForEach(x => this.AddOrUpdate(x.Key, x.Value));
    }
}