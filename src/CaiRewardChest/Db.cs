using MySql.Data.MySqlClient;
using System.Data;
using Terraria;
using TShockAPI;
using TShockAPI.DB;

namespace CaiRewardChest;

public static class Db
{
    private static IDbConnection DbConnection => TShock.DB;

    public static void Init()
    {
        SqlTableCreator sqlTableCreator = new(DbConnection,
            DbConnection.GetSqlType() == SqlType.Sqlite ? new SqliteQueryCreator() : new MysqlQueryCreator());

        sqlTableCreator.EnsureTableStructure(new SqlTable("CaiRewardChest",
            new SqlColumn("ChestID", MySqlDbType.Int32) { Length = 8 },
            new SqlColumn("X", MySqlDbType.Int32) { Length = 255 },
            new SqlColumn("Y", MySqlDbType.Int32) { Length = 255 },
            new SqlColumn("HasOpened", MySqlDbType.String) { Length = 25555 })
        );
    }


    public static RewardChest? GetChestByPos(int x, int y)
    {
        var chestId = Chest.FindChest(x, y);
        return GetChestById(chestId);
    }

    public static RewardChest? GetChestById(int chestId)
    {
        using var result = DbConnection.QueryReader("SELECT * FROM CaiRewardChest WHERE ChestID=@0;", chestId);
        if (result.Read())
        {
            RewardChest? chest = new()
            {
                ChestId = chestId,
                HasOpenPlayer = result.Get<string>("HasOpened")
                    .Split(',')
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Select(int.Parse)
                    .ToList()
            };
            return chest;
        }

        return null;
    }


    public static void AddChest(int chestId, int x, int y)
    {
        DbConnection.Query("INSERT INTO CaiRewardChest (ChestID, X, Y, HasOpened) VALUES (@0, @1, @2,@3);", chestId, x,
            y, "");
    }

    public static void UpdateChest(RewardChest chest)
    {
        DbConnection.Query("UPDATE CaiRewardChest SET HasOpened=@0 WHERE ChestID=@1",
            string.Join(',', chest.HasOpenPlayer), chest.ChestId);
    }

    public static void DelChest(int chestId)
    {
        DbConnection.Query("DELETE FROM CaiRewardChest WHERE ChestID=@0;", chestId);
    }

    public static void ClearDb()
    {
        DbConnection.Query("DELETE FROM CaiRewardChest;");
    }
}