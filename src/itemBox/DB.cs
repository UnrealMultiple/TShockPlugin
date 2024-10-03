using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Terraria;
using TShockAPI;
using TShockAPI.DB;

namespace itemBox;

public static class DB
{
	private static IDbConnection db => TShock.DB;

	public static void Connect()
	{
		IDbConnection dbConnection = db;
		IQueryBuilder provider;
		if (db.GetSqlType() != SqlType.Sqlite)
		{
			IQueryBuilder queryBuilder = new MysqlQueryCreator();
			provider = queryBuilder;
		}
		else
		{
			IQueryBuilder queryBuilder = new SqliteQueryCreator();
			provider = queryBuilder;
		}
		SqlTableCreator sqlTableCreator = new SqlTableCreator(dbConnection, provider);
		sqlTableCreator.EnsureTableStructure(new SqlTable("itemBox", new SqlColumn("UserID", MySqlDbType.Int32)
		{
			Primary = true,
			Unique = true,
			Length = 8
		}, new SqlColumn("Inventory", MySqlDbType.Text)
		{
			Length = int.MaxValue
		}));
	}

	public static List<Item> GetUserInfo(int userid)
	{
		using QueryResult queryResult = db.QueryReader("SELECT * FROM itemBox WHERE UserID=@0;", userid);
		if (queryResult.Read())
		{
			return PlayerInventory.Prase(queryResult.Get<string>("Inventory"));
		}
		AddNewUser(userid);
		return new List<Item>();
	}

	public static void AddNewUser(int userid)
	{
		db.Query("INSERT INTO itemBox (UserId, Inventory) VALUES (@0, @1);", userid, string.Empty);
	}

	public static void UpdataInentory(int userid, List<Item> items)
	{
		db.Query("UPDATE itemBox SET Inventory=@0 WHERE UserID=@1;", PlayerInventory.ToString(items), userid);
	}

	public static void ClearDB()
	{
		db.Query("DELETE FROM itemBox;");
	}

	public static void ClearPlayerInventory(int userid)
	{
		db.Query("DELETE FROM itemBox WHERE UserID=@0;", userid);
	}
}
