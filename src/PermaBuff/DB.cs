using MySql.Data.MySqlClient;
using System.Data;
using TShockAPI;
using TShockAPI.DB;

namespace PermaBuff;
public class DB
{
    private static IDbConnection database = null!;
    public static void Init()
    {
        database = TShock.DB;
        string sql = @"
        CREATE TABLE IF NOT EXISTS Permabuff (
            Name TEXT NOT NULL,
            buffid INTEGER NOT NULL,
            UNIQUE(Name, buffid)
        )";
        database.Query(sql);
    }

    public static void ReadAll()
    {
        using (var reader = database.QueryReader("SELECT * FROM Permabuff"))
        {
            while (reader.Read())
            {
                var username = reader.Get<string>("Name");
                var buffid = reader.Get<int>("buffid");
                Playerbuffs.AddBuff(username, buffid, false);
            }
        }
    }

    public static void AddBuff(string Name, int buffid)
    {
        database.Query("INSERT INTO `Permabuff` (`Name`, `buffid`) VALUES (@0, @1)", Name, buffid);
    }

    public static void Delbuff(string Name, int buffid)
    {
        database.Query("DELETE FROM Permabuff WHERE Name = @0 and buffid = @1", Name, buffid);
    }

    public static void ClearTable()
    {
        database.Query("DELETE FROM Permabuff");
    }
    public static void ClearPlayerBuffs(string name)
    {
        database.Query("DELETE FROM Permabuff WHERE Name = @0", name);
    }
}