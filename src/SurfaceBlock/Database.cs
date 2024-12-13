using MySql.Data.MySqlClient;
using TShockAPI;
using TShockAPI.DB;

namespace SurfaceBlock;

public class Database
{
    #region ���ݵĽṹ��
    public class PlayerData
    {
        //�������
        public string Name { get; set; }
        //���ٿ���
        public bool Enabled { get; set; }
        //����ʱ��
        public DateTime Time { get; set; }
        internal PlayerData(string name = "",bool enabled = true, DateTime time = default)
        {
            this.Name = name ?? "";
            this.Enabled = enabled;
            this.Time = time;
        }
    }
    #endregion

    #region ���ݿ��ṹ��ʹ��Tshock�Դ������ݿ���Ϊ�洢��
    public Database()
    {
        var sql = new SqlTableCreator(TShock.DB, new SqliteQueryCreator());
        sql.EnsureTableStructure(new SqlTable("SurfaceBlock", //����
            new SqlColumn("ID", MySqlDbType.Int32) { Primary = true, Unique = true, AutoIncrement = true }, // ������
            new SqlColumn("Name", MySqlDbType.TinyText) { NotNull = true }, // �ǿ��ַ�����
            new SqlColumn("Enabled", MySqlDbType.Int32) { DefaultValue = "0" }, // ���ٿ��� bool��ʶ
            new SqlColumn("Time", MySqlDbType.DateTime) // ����ʱ��
        ));
    }
    #endregion

    #region Ϊ��Ҵ������ݷ���
    public bool AddData(PlayerData data)
    {
        return TShock.DB.Query("INSERT INTO SurfaceBlock (Name, Enabled,Time) VALUES (@0, @1, @2)",
            data.Name, data.Enabled ? 1 : 0, data.Time) != 0;
    }
    #endregion

    #region �������ݷ���
    public bool UpdateData(PlayerData data)
    {
        return TShock.DB.Query("UPDATE SurfaceBlock SET Enabled = @0, Time = @1 WHERE Name = @2",
            data.Enabled ? 1 : 0, data.Time, data.Name) != 0;
    }
    #endregion

    #region ��ȡ���ݷ���
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

    #region �����������ݷ���
    public bool ClearData()
    {
        return TShock.DB.Query("DELETE FROM SurfaceBlock") != 0;
    }
    #endregion
}