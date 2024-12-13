using MySql.Data.MySqlClient;
using TShockAPI;
using TShockAPI.DB;

namespace SurfaceBlock;

public class Database
{
    #region 数据的结构体
    public class PlayerData
    {
        //玩家名字
        public string Name { get; set; }
        //销毁开关
        public bool Enabled { get; set; }
        //销毁时间
        public DateTime Time { get; set; }
        internal PlayerData(string name = "",bool enabled = true, DateTime time = default)
        {
            this.Name = name ?? "";
            this.Enabled = enabled;
            this.Time = time;
        }
    }
    #endregion

    #region 数据库表结构（使用Tshock自带的数据库作为存储）
    public Database()
    {
        var sql = new SqlTableCreator(TShock.DB, new SqliteQueryCreator());
        sql.EnsureTableStructure(new SqlTable("SurfaceBlock", //表名
            new SqlColumn("ID", MySqlDbType.Int32) { Primary = true, Unique = true, AutoIncrement = true }, // 主键列
            new SqlColumn("Name", MySqlDbType.TinyText) { NotNull = true }, // 非空字符串列
            new SqlColumn("Enabled", MySqlDbType.Int32) { DefaultValue = "0" }, // 销毁开关 bool标识
            new SqlColumn("Time", MySqlDbType.DateTime) // 销毁时间
        ));
    }
    #endregion

    #region 为玩家创建数据方法
    public bool AddData(PlayerData data)
    {
        return TShock.DB.Query("INSERT INTO SurfaceBlock (Name, Enabled,Time) VALUES (@0, @1, @2)",
            data.Name, data.Enabled ? 1 : 0, data.Time) != 0;
    }
    #endregion

    #region 更新数据方法
    public bool UpdateData(PlayerData data)
    {
        return TShock.DB.Query("UPDATE SurfaceBlock SET Enabled = @0, Time = @1 WHERE Name = @2",
            data.Enabled ? 1 : 0, data.Time, data.Name) != 0;
    }
    #endregion

    #region 获取数据方法
    public PlayerData? GetData(string name)
    {
        using var reader = TShock.DB.QueryReader("SELECT * FROM SurfaceBlock WHERE Name = @0", name);

        return reader.Read()
            ? new PlayerData(
                name: reader.Get<string>("Name"),
                enabled: reader.Get<int>("Enabled") == 1,
                time: reader.Get<DateTime>("Time")
            )
            : null;
    }
    #endregion

    #region 清理所有数据方法
    public bool ClearData()
    {
        return TShock.DB.Query("DELETE FROM SurfaceBlock") != 0;
    }
    #endregion
}