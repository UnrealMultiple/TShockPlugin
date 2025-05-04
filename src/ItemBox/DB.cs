using MySql.Data.MySqlClient;
using System.Data;
using Terraria;
using TShockAPI;
using TShockAPI.DB;

namespace ItemBox;

public static class DB
{
    private static IDbConnection db => TShock.DB;

    public static void Connect()
    {
        var dbConnection = db;

        var sqlTableCreator = new SqlTableCreator(dbConnection, dbConnection.GetSqlQueryBuilder());
        sqlTableCreator.EnsureTableStructure(new SqlTable("item_box", new SqlColumn("UserID", MySqlDbType.Int32) { Primary = true, Unique = true, Length = 8 }, new SqlColumn("Inventory", MySqlDbType.Text) { Length = int.MaxValue }));
    }

    public static List<Item> GetUserInfo(int userid)
    {
        using var queryResult = db.QueryReader("SELECT * FROM item_box WHERE UserID=@0;", userid);
        if (queryResult.Read())
        {
            return PlayerInventory.Prase(queryResult.Get<string>("Inventory"));
        }

        AddNewUser(userid);
        return new List<Item>();
    }

    public static void AddNewUser(int userid)
    {
        db.Query("INSERT INTO item_box (UserId, Inventory) VALUES (@0, @1);", userid, string.Empty);
    }

    public static void UpdataInentory(int userid, List<Item> items)
    {
        db.Query("UPDATE item_box SET Inventory=@0 WHERE UserID=@1;", PlayerInventory.ToString(items), userid);
    }

    public static void ClearDB()
    {
        db.Query("DELETE FROM item_box;");
    }

    public static void ClearPlayerInventory(int userid)
    {
        db.Query("DELETE FROM item_box WHERE UserID=@0;", userid);
    }
}