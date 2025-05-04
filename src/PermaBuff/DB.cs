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
        var Skeleton = new SqlTable("Permabuff",
            new SqlColumn("buffid", MySqlDbType.Int32) { Length = 255, Unique = true },
            new SqlColumn("Name", MySqlDbType.VarChar) { Length = 255, Unique = true }
              );
        var List = new SqlTableCreator(database, database.GetSqlQueryBuilder());
        List.EnsureTableStructure(Skeleton);
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
}