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
    public PlayerOnline()
    {
        database = TShock.DB;
        var Skeleton = new SqlTable("OnlineDuration",
            new SqlColumn("username", MySqlDbType.Text) { Length = 500 },
            new SqlColumn("duration", MySqlDbType.Int32) { Length = 255 }
              );
        var List = new SqlTableCreator(database, database.GetSqlType() == SqlType.Sqlite ? new SqliteQueryCreator() : new MysqlQueryCreator());
        List.EnsureTableStructure(Skeleton);
        ReadAll();
    }

    public void ReadAll()
    {
        using var reader = database.QueryReader("SELECT * FROM OnlineDuration");
        while (reader.Read())
        {
            string username = reader.Get<string>("username");
            int duration = reader.Get<int>("duration");
            this[username] = duration;
            _players.Add(username);
        }
    }

    public bool Read(string name, out int duration)
    {
        using var reader = database.QueryReader("SELECT * FROM `OnlineDuration` WHERE `username` LIKE @0", name);
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
        return 1 == database.Query("UPDATE `OnlineDuration` SET `duration` = @0 WHERE `OnlineDuration`.`username` = @1", duration, Name);

    }
    public bool Insert(string Name, int duration)
    {
        _players.Add(Name);
        return 1 == database.Query("INSERT INTO `OnlineDuration` (`username`, `duration`) VALUES (@0, @1)", Name, duration);
    }




    public void AddOrUpdate(string name, int duration)
    {
        if (_players.Contains(name))
            Update(name, duration);
        else
            Insert(name, duration);
    }

    public void UpdateAll()
    {
        this.ForEach(x => AddOrUpdate(x.Key, x.Value));
    }
}
