using MySql.Data.MySqlClient;
using System.Text.Json;
using TShockAPI;
using TShockAPI.DB;

namespace AutoAirItem;

public class Database
{
    #region 垃圾桶数据表结构
    public Database()
    {
        var sql = new SqlTableCreator(TShock.DB, TShock.DB.GetSqlQueryBuilder());

        // 定义并确保 AutoTrash 表的结构
        sql.EnsureTableStructure(new SqlTable("AutoTrash", //表名
            new SqlColumn("ID", MySqlDbType.Int32) { Primary = true, Unique = true, AutoIncrement = true }, // 主键列
            new SqlColumn("Name", MySqlDbType.TinyText) { NotNull = true }, // 非空字符串列
            new SqlColumn("Enabled", MySqlDbType.Int32) { DefaultValue = "0" }, // bool值列
            new SqlColumn("Auto", MySqlDbType.Int32) { DefaultValue = "0" }, // bool值列
            new SqlColumn("Mess", MySqlDbType.Int32) { DefaultValue = "1" }, // bool值列
            new SqlColumn("ItemType", MySqlDbType.Text), // 文本列，用于存储序列化的物品类型列表
            new SqlColumn("DelItem", MySqlDbType.Text) // 文本列，用于存储序列化的移除物品字典
        ));
    }
    #endregion

    #region 更新数据
    public bool UpdateData(MyData.PlayerData data)
    {
        var itemType = JsonSerializer.Serialize(data.ItemType);
        var delItem = JsonSerializer.Serialize(data.DelItem);

        // 更新现有记录
        if (TShock.DB.Query("UPDATE AutoTrash SET Enabled = @0, Auto = @1, Mess = @2, ItemType = @3, DelItem = @4 WHERE Name = @5",
            data.Enabled ? 1 : 0, data.Auto ? 1 : 0, data.Mess ? 1 : 0, itemType, delItem, data.Name) != 0)
        {
            return true;
        }

        // 如果没有更新到任何记录，则插入新记录
        return TShock.DB.Query("INSERT INTO AutoTrash (Name, Enabled, Auto, Mess, ItemType, DelItem) VALUES (@0, @1, @2, @3, @4, @5)",
            data.Name, data.Enabled ? 1 : 0, data.Auto ? 1 : 0, data.Mess ? 1 : 0, itemType, delItem) != 0;
    }
    #endregion

    #region 加载所有数据（每次重启服务器时用于读取之前存下的数据）主要用于解决：内存清空时数据丢失的方法
    public List<MyData.PlayerData> LoadData()
    {
        var data = new List<MyData.PlayerData>();

        using var reader = TShock.DB.QueryReader("SELECT * FROM AutoTrash");

        while (reader.Read())
        {
            var ItemType = reader.Get<string>("ItemType");
            var DelItem = reader.Get<string>("DelItem");

            var ItemList = JsonSerializer.Deserialize<List<int>>(ItemType);
            var DelList = JsonSerializer.Deserialize<Dictionary<int, int>>(DelItem);

            data.Add(new MyData.PlayerData(
                name: reader.Get<string>("Name"),
                enabled: reader.Get<int>("Enabled") == 1,
                auto: reader.Get<int>("Auto") == 1,
                mess: reader.Get<int>("Mess") == 1,
                item: ItemList!,
                DelItem: DelList!
            ));
        }

        return data;
    }
    #endregion

    #region 清理所有数据方法
    public bool ClearData()
    {
        return TShock.DB.Query("DELETE FROM AutoTrash") != 0;
    }
    #endregion
}