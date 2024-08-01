using System.Data;
using Economics.RPG.Model;
using MySql.Data.MySqlClient;
using TShockAPI;
using TShockAPI.DB;

namespace Economics.RPG;

public class PlayerLevelManager
{
    private readonly IDbConnection database;

    private readonly Dictionary<string, string> Levels;

    internal PlayerLevelManager()
    {
        database = TShock.DB;
        var Skeleton = new SqlTable("RPG",
            new SqlColumn("ID", MySqlDbType.Int32) { Length = 8, Primary = true, Unique = true, AutoIncrement = true },
            new SqlColumn("UserName", MySqlDbType.Text) { Length = 500 },
            new SqlColumn("Level", MySqlDbType.Text) { Length = 255 }
              );
        var List = new SqlTableCreator(database, database.GetSqlType() == SqlType.Sqlite ? new SqliteQueryCreator() : new MysqlQueryCreator());
        List.EnsureTableStructure(Skeleton);
        Levels = GetPlayerLevel();
    }

    private Dictionary<string, string> GetPlayerLevel()
    {
        Dictionary<string, string> levels = new();
        using var read = database.QueryReader("select * from `RPG`");
        while (read.Read())
        {
            var Name = read.Get<string>("UserName");
            var Level = read.Get<string>("Level");
            levels[Name] = Level;
        }
        return levels;
    }

    public Level GetLevel(string userName)
    {
        if (Levels.TryGetValue(userName, out var level) && level != null)
        {
            return RPG.Config.GetLevel(level) ?? RPG.Config.DefaultLevel;
        }
        return RPG.Config.DefaultLevel;
    }

    public void ResetPlayerLevel(string userName)
    {
        Update(userName, RPG.Config.DefaultLevel);
    }

    public void Update(string userName, Level level)
    {
        if (Levels.ContainsKey(userName))
        {
            database.Query("UPDATE `RPG` SET `Level` = @0 WHERE `RPG`.`UserName` = @1", level.Name, userName);
        }
        else
        {
            database.Query("INSERT INTO `RPG` (`UserName`, `Level`) VALUES (@0, @1)", userName, level.Name);
        }
        Levels[userName] = level.Name;
    }

    public void Remove(string userName)
    {
        if (Levels.ContainsKey(userName))
            database.Query("DELETE FROM `RPG` WHERE `RPG`.`UserName` = @0", userName);
        Levels.Remove(userName);
    }

    public void RemoveAll()
    {
        database.Query("delete from RPG");
        Levels.Clear();
    }
}
