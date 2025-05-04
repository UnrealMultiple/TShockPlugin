using MySql.Data.MySqlClient;
using System.Data;
using Terraria;
using TShockAPI;
using TShockAPI.DB;

namespace WeaponPlus;

public class WeaponPlusDB
{
    #region 创建数据库表
    private readonly IDbConnection database;

    private readonly string tableName;

    public WeaponPlusDB(IDbConnection database)
    {
        this.database = database;
        this.tableName = "WeaponPlusDBcostCoin";
        var table = new SqlTable(this.tableName,
            new SqlColumn("owner", (MySqlDbType) 752),
            new SqlColumn("itemID", (MySqlDbType) 3),
            new SqlColumn("itemName", (MySqlDbType) 752),
            new SqlColumn("lable", (MySqlDbType) 3),
            new SqlColumn("level", (MySqlDbType) 3),
            new SqlColumn("damage_level", (MySqlDbType) 3),
            new SqlColumn("scale_level", (MySqlDbType) 3),
            new SqlColumn("knockBack_level", (MySqlDbType) 3),
            new SqlColumn("useSpeed_level", (MySqlDbType) 3),
            new SqlColumn("shootSpeed_level", (MySqlDbType) 3),
            new SqlColumn("allCost", (MySqlDbType) 8));
        var queryBuilder = database.GetSqlQueryBuilder();
        queryBuilder.CreateTable(table);
        var sqlTableCreator = new SqlTableCreator(database, queryBuilder);
        sqlTableCreator.EnsureTableStructure(table);
    }
    #endregion

    #region 读取数据库从拥有者处获取物品数据
    public WItem[] ReadDBGetWItemsFromOwner(string owner, int ID = 0)
    {
        var list = new List<WItem>();
        var text = (ID == 0) ? "'" : ("' And itemID = " + ID);
        try
        {
            using (var queryResult = this.database.QueryReader("SELECT * FROM " + this.tableName + " WHERE owner = '" + owner + text))
            {
                while (queryResult.Read())
                {
                    var wItem = new WItem(queryResult.Get<int>("itemID"), owner)
                    {
                        lable = queryResult.Get<int>("lable"),
                        damage_level = queryResult.Get<int>("damage_level"),
                        scale_level = queryResult.Get<int>("scale_level"),
                        knockBack_level = queryResult.Get<int>("knockBack_level"),
                        useSpeed_level = queryResult.Get<int>("useSpeed_level"),
                        shootSpeed_level = queryResult.Get<int>("shootSpeed_level"),
                        allCost = queryResult.Get<long>("allCost")
                    };
                    list.Add(wItem);
                }
            }
            return list.ToArray();
        }
        catch (Exception ex)
        {
            TShock.Log.Error("错误：ReadDBGetWItemsFromOwner " + ex.ToString());
            TSPlayer.All.SendErrorMessage("错误：ReadDBGetWItemsFromOwner " + ex.ToString());
            Console.WriteLine("错误：ReadDBGetWItemsFromOwner " + ex.ToString());
            return list.ToArray();
        }
    }
    #endregion

    #region 写入数据
    public bool WriteDB(WItem[] WItem)
    {
        if (WItem.Length == 0)
        {
            return false;
        }
        var result = true;
        foreach (var wItem in WItem)
        {
            if (wItem == null || wItem.Level == 0 || string.IsNullOrWhiteSpace(wItem.owner))
            {
                continue;
            }
            try
            {
                if (this.ReadDBGetWItemsFromOwner(wItem.owner, wItem.id).Length == 0)
                {
                    this.database.Query("INSERT INTO " + this.tableName + " (owner, itemName, itemID, lable, level, damage_level, scale_level, knockBack_level, useSpeed_level, shootSpeed_level, allCost) VALUES (@0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10);", wItem.owner, Lang.GetItemNameValue(wItem.id), wItem.id, wItem.lable, wItem.Level, wItem.damage_level, wItem.scale_level, wItem.knockBack_level, wItem.useSpeed_level, wItem.shootSpeed_level, wItem.allCost);
                }
                else
                {
                    this.database.Query("UPDATE " + this.tableName + " SET lable = @0, level = @1, damage_level = @4, scale_level = @5, knockBack_level = @6, useSpeed_level = @7, shootSpeed_level = @8, allCost = @9 WHERE owner = @2 And itemID = @3;", wItem.lable, wItem.Level, wItem.owner, wItem.id, wItem.damage_level, wItem.scale_level, wItem.knockBack_level, wItem.useSpeed_level, wItem.shootSpeed_level, wItem.allCost);
                }
            }
            catch (Exception ex)
            {
                TShock.Log.Error("错误：WriteDB " + ex.ToString());
                TSPlayer.All.SendErrorMessage("错误：WriteDB " + ex.ToString());
                Console.WriteLine("错误：WriteDB " + ex.ToString());
                result = false;
            }
        }
        return result;
    }

    public bool WriteDB(WItem? WItem)
    {
        if (WItem == null || WItem.Level <= 0 || string.IsNullOrWhiteSpace(WItem.owner))
        {
            return false;
        }
        try
        {
            if (this.ReadDBGetWItemsFromOwner(WItem.owner, WItem.id).Length == 0)
            {
                this.database.Query("INSERT INTO " + this.tableName + " (owner, itemName, itemID, lable, level, damage_level, scale_level, knockBack_level, useSpeed_level, shootSpeed_level, allCost) VALUES (@0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10);", WItem.owner, Lang.GetItemNameValue(WItem.id), WItem.id, WItem.lable, WItem.Level, WItem.damage_level, WItem.scale_level, WItem.knockBack_level, WItem.useSpeed_level, WItem.shootSpeed_level, WItem.allCost);
                return true;
            }
            this.database.Query("UPDATE " + this.tableName + " SET lable = @0, level = @1, damage_level = @4, scale_level = @5, knockBack_level = @6, useSpeed_level = @7, shootSpeed_level = @8, allCost = @9 WHERE owner = @2 And itemID = @3;", WItem.lable, WItem.Level, WItem.owner, WItem.id, WItem.damage_level, WItem.scale_level, WItem.knockBack_level, WItem.useSpeed_level, WItem.shootSpeed_level, WItem.allCost);
            return true;
        }
        catch (Exception ex)
        {
            TShock.Log.Error("错误：WriteDB2 " + ex.ToString());
            TSPlayer.All.SendErrorMessage("错误：WriteDB2 " + ex.ToString());
            Console.WriteLine("错误：WriteDB2 " + ex.ToString());
            return false;
        }
    }
    #endregion

    #region 删除指定数据表
    public bool DeleteDB(string owner, int ID = 0)
    {
        try
        {
            var text = (ID == 0) ? "'" : ("' And itemID = " + ID);
            this.database.Query("DELETE FROM " + this.tableName + " WHERE owner = '" + owner + text);
            return true;
        }
        catch (Exception ex)
        {
            TShock.Log.Error("错误：DeleteDB " + ex.ToString());
            TSPlayer.All.SendErrorMessage("错误：DeleteDB " + ex.ToString());
            Console.WriteLine("错误：DeleteDB " + ex.ToString());
            return false;
        }
    }
    #endregion

    #region 删除所有数据表
    public bool DeleteDBAll()
    {
        try
        {
            this.database.Query("DROP TABLE " + this.tableName);
            WeaponPlus.DB = new WeaponPlusDB(TShock.DB);
            return true;
        }
        catch (Exception ex)
        {
            TShock.Log.Error("错误：DeleteDBAll " + ex.ToString());
            TSPlayer.All.SendErrorMessage("错误：DeleteDBAll " + ex.ToString());
            Console.WriteLine("错误：DeleteDBAll " + ex.ToString());
            return false;
        }
    }
    #endregion
}