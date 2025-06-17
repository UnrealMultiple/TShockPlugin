using MySql.Data.MySqlClient;
using TShockAPI;
using TShockAPI.DB;

namespace CGive;

public class Data
{
    public static void Init()
    {
        var cgiveTable = new SqlTable("CGive",
            new SqlColumn("id", MySqlDbType.Int32) { Primary = true, AutoIncrement = true },
            new SqlColumn("executer", MySqlDbType.Text),
            new SqlColumn("cmd", MySqlDbType.Text),
            new SqlColumn("who", MySqlDbType.Text)
        );

        var givenTable = new SqlTable("Given",
            new SqlColumn("id", MySqlDbType.Int32) { Primary = true, AutoIncrement = true },
            new SqlColumn("name", MySqlDbType.Text));


        SqlTableCreator creator = new(TShock.DB, TShock.DB.GetSqlQueryBuilder());
        creator.EnsureTableStructure(cgiveTable);
        creator.EnsureTableStructure(givenTable);
    }

    public static void Command(string cmd, params object[] args)
    {
        TShock.DB.Query(cmd, args);
    }
}