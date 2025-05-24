using System.Text.Json;
using MySql.Data.MySqlClient;
using TShockAPI;
using TShockAPI.DB;

namespace AutoAirItem;

public class Database
{
    #region 垃圾桶数据表结构
    public Database()
    {
        // Tshock 6 建数据：
         var sql = new SqlTableCreator(TShock.DB, TShock.DB.GetSqlQueryBuilder());

        // Tshock 5 建数据：
        // var sql = new SqlTableCreator(TShock.DB, new SqliteQueryCreator());

        // 定义并确保 AutoTrash 表的结构
        sql.EnsureTableStructure(new SqlTable("AutoTrash", //表名
            new SqlColumn("ID", MySqlDbType.Int32) { Primary = true, Unique = true, AutoIncrement = true }, // 主键列
            new SqlColumn("Name", MySqlDbType.TinyText) { NotNull = true }, // 非空字符串列
            new SqlColumn("Enabled", MySqlDbType.Int32) { DefaultValue = "0" }, // bool值列
            new SqlColumn("Mess", MySqlDbType.Int32) { DefaultValue = "1" }, // bool值列
            new SqlColumn("TrashList", MySqlDbType.Text) // 文本列，用于存储序列化的移除物品字典
        ));
    }
    #endregion

    #region 创建数据方法
    public bool AddData(MyData.PlayerData data)
    {
        var trashList = JsonSerializer.Serialize(data.TrashList);

        return TShock.DB.Query("INSERT INTO AutoTrash (Name, Enabled, Mess, TrashList) VALUES (@0, @1, @2, @3)",
            data.Name, data.Enabled ? 1 : 0, data.Mess ? 1 : 0, trashList) != 0;
    }
    #endregion

    #region 更新数据
    public bool UpdateData(MyData.PlayerData data)
    {
        var trashList = JsonSerializer.Serialize(data.TrashList);

        // 更新现有记录
        if (TShock.DB.Query("UPDATE AutoTrash SET Enabled = @0, Mess = @1, TrashList = @2 WHERE Name = @3",
            data.Enabled ? 1 : 0, data.Mess ? 1 : 0, trashList, data.Name) != 0)
        {
            return true;
        }

        // 如果没有更新到任何记录，则插入新记录
        return TShock.DB.Query("INSERT INTO AutoTrash (Name, Enabled, Mess, TrashList) VALUES (@0, @1, @2, @3)",
            data.Name, data.Enabled ? 1 : 0, data.Mess ? 1 : 0, trashList) != 0;
    }
    #endregion

    #region 获取玩家数据
    public MyData.PlayerData? GetData(string name)
    {
        using var reader = TShock.DB.QueryReader("SELECT * FROM AutoTrash WHERE Name = @0", name);

        return reader.Read()
            ? new MyData.PlayerData
            (
                name: reader.Get<string>("Name"),
                enabled: reader.Get<int>("Enabled") == 1,
                mess: reader.Get<int>("Mess") == 1,
                trashList: JsonSerializer.Deserialize<Dictionary<int, int>>(reader.Get<string>("TrashList"))! // 反序列化的字典
            ):  null;
    }
    #endregion

    #region 获取所有数据
    public List<MyData.PlayerData> GetAllData()
    {
        var data = new List<MyData.PlayerData>();

        using var reader = TShock.DB.QueryReader("SELECT * FROM AutoTrash");

        while (reader.Read())
        {
            data.Add(new MyData.PlayerData(
                name: reader.Get<string>("Name"),
                enabled: reader.Get<int>("Enabled") == 1,
                mess: reader.Get<int>("Mess") == 1,
                trashList: JsonSerializer.Deserialize<Dictionary<int, int>>(reader.Get<string>("TrashList"))!
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