using Microsoft.Xna.Framework;
using MySql.Data.MySqlClient;
using System.Text.Json;
using System.Text.Json.Serialization;
using TShockAPI;
using TShockAPI.DB;

namespace ModifyWeapons;

public class Database
{
    #region 数据的结构体
    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
    {
        WriteIndented = true, //缩进
        //统一编码（避免Dict的键名为中文时变成乱码用）
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All)
    };

    public class PlayerData
    {
        public string Name { get; set; }
        public int ReadCount { get; set; }
        public int Process { get; set; }
        public bool Hand { get; set; }
        public bool Join { get; set; }
        public bool Alone { get; set; }
        public DateTime ReadTime { get; set; }
        public DateTime SyncTime { get; set; }
        public DateTime AloneTime { get; set; }
        public Dictionary<string, List<ItemData>> Dict { get; set; } = new Dictionary<string, List<ItemData>>();
        internal PlayerData(string name = "", int readCount = 0, bool hand = false, bool join = true, DateTime? readTime = null, DateTime? syncTime = null, Dictionary<string, List<ItemData>>? dict = null, int process = 0, bool alone = false, DateTime? aloneTime = default)
        {
            this.Name = name ?? "";
            this.ReadCount = readCount;
            this.Hand = hand;
            this.Join = join;
            this.ReadTime = readTime ?? DateTime.UtcNow;
            this.SyncTime = syncTime ?? DateTime.UtcNow;
            this.Dict = dict ?? new Dictionary<string, List<ItemData>>();
            this.Process = process;
            this.Alone = alone;
            this.AloneTime = aloneTime ?? DateTime.UtcNow;
        }

        public class ItemData
        {
            public int type { get; set; }
            public int stack { get; set; }
            public byte prefix { get; set; }
            public int damage { get; set; }
            public float scale { get; set; }
            public float knockBack { get; set; }
            public int useTime { get; set; }
            public int useAnimation { get; set; }
            public int shoot { get; set; }
            public float shootSpeed { get; set; }
            public int ammo { get; set; }
            public int useAmmo { get; set; }
            public Color color { get; set; }
            public ItemData() { }

            [JsonConstructor]
            public ItemData(int type, int stack, byte prefix, int damage, float scale, float knockBack,
                int useTime, int useAnimation, int shoot, float shootSpeed, int ammo, int useAmmo, Color color)
            {
                this.type = type;
                this.stack = stack;
                this.prefix = prefix;
                this.damage = damage;
                this.scale = scale;
                this.knockBack = knockBack;
                this.useTime = useTime;
                this.useAnimation = useAnimation;
                this.shoot = shoot;
                this.shootSpeed = shootSpeed;
                this.ammo = ammo;
                this.useAmmo = useAmmo;
                this.color = color;
            }
        }
    }
    #endregion

    #region 数据库表结构（使用Tshock自带的数据库作为存储）
    public Database()
    {
        var sql = new SqlTableCreator(TShock.DB, TShock.DB.GetSqlQueryBuilder());

        sql.EnsureTableStructure(new SqlTable("ModifyWeapons", //表名
            new SqlColumn("ID", MySqlDbType.Int32) { Primary = true, Unique = true, AutoIncrement = true }, // 主键列
            new SqlColumn("Name", MySqlDbType.TinyText) { NotNull = true }, // 非空字符串列
            new SqlColumn("ReadCount", MySqlDbType.Int32) { DefaultValue = "0" },
            new SqlColumn("IsProcess", MySqlDbType.Int32) { DefaultValue = "0" },
            new SqlColumn("Hand", MySqlDbType.Int32) { DefaultValue = "0" },
            new SqlColumn("IsJoin", MySqlDbType.Int32) { DefaultValue = "0" },
            new SqlColumn("Alone", MySqlDbType.Int32) { DefaultValue = "0" },
            new SqlColumn("ReadTime", MySqlDbType.DateTime) { DefaultValue = "CURRENT_TIMESTAMP" },
            new SqlColumn("SyncTime", MySqlDbType.DateTime) { DefaultValue = "CURRENT_TIMESTAMP" },
            new SqlColumn("AloneTime", MySqlDbType.DateTime) { DefaultValue = "CURRENT_TIMESTAMP" },
            new SqlColumn("Dict", MySqlDbType.LongText)  // 文本列，用于存储序列化的物品ID列表
        ));
    }
    #endregion

    #region 为玩家创建数据方法
    public bool AddData(PlayerData data)
    {
        var dictJson = JsonSerializer.Serialize(data.Dict, Options);
        return TShock.DB.Query("INSERT INTO ModifyWeapons (Name, ReadCount,IsProcess, Hand, IsJoin,Alone, ReadTime, SyncTime,AloneTime, Dict) VALUES (@0, @1, @2, @3, @4, @5,@6, @7,@8,@9)",
            data.Name, data.ReadCount, data.Process, data.Hand ? 1 : 0, data.Join ? 1 : 0, data.Alone ? 1 : 0, data.ReadTime, data.SyncTime, data.AloneTime, dictJson) != 0;
    }
    #endregion

    #region 更新数据内容方法
    public bool UpdateData(PlayerData data)
    {
        var dictJson = JsonSerializer.Serialize(data.Dict, Options);

        return TShock.DB.Query("UPDATE ModifyWeapons SET ReadCount = @0,IsProcess = @1, Hand = @2, IsJoin = @3, Alone = @4 ,ReadTime = @5, SyncTime = @6, AloneTime = @7, Dict = @8 WHERE Name = @9",
            data.ReadCount, data.Process, data.Hand ? 1 : 0, data.Join ? 1 : 0, data.Alone ? 1 : 0, data.ReadTime, data.SyncTime, data.AloneTime, dictJson, data.Name) != 0;
    }
    #endregion

    #region 增加指定玩家重读次数方法
    public bool UpReadCount(string name, int num)
    {
        return TShock.DB.Query("UPDATE ModifyWeapons SET ReadCount = ReadCount + @0 WHERE Name = @1", num, name) != 0;
    }
    #endregion

    #region 删除指定玩家数据方法
    public bool DeleteData(string name)
    {
        return TShock.DB.Query("DELETE FROM ModifyWeapons WHERE Name = @0", name) != 0;
    }
    #endregion

    #region 获取指定玩家数据方法
    public PlayerData? GetData(string name)
    {
        using var reader = TShock.DB.QueryReader("SELECT * FROM ModifyWeapons WHERE Name = @0", name);

        if (reader.Read())
        {
            var dictJson = reader.Get<string>("Dict");
            var dict = JsonSerializer.Deserialize<Dictionary<string, List<PlayerData.ItemData>>>(dictJson, Options);
            return new PlayerData(
                name: reader.Get<string>("Name"),
                readCount: reader.Get<int>("ReadCount"),
                hand: reader.Get<int>("Hand") == 1,
                process: reader.Get<int>("IsProcess"),
                join: reader.Get<int>("IsJoin") == 1,
                alone: reader.Get<int>("Alone") == 1,
                aloneTime: reader.Get<DateTime>("AloneTime"),
                readTime: reader.Get<DateTime>("ReadTime"),
                syncTime: reader.Get<DateTime>("SyncTime"),
                dict: dict
            );
        }

        return null;
    }
    #endregion

    #region 移除所有玩家的指定物品方法
    public bool RemovePwData(int type)
    {
        var AllData = this.GetAll();
        var flag = false;

        foreach (var data in AllData)
        {
            if (data.Dict != null && data.Dict.Values.Any(list => list.Any(item => item.type == type)))
            {
                // 移除指定类型的物品
                foreach (var dict in data.Dict.ToList())
                {
                    dict.Value.RemoveAll(item => item.type == type);
                }

                // 更新玩家数据
                if (this.UpdateData(data))
                {
                    flag = true;
                }
            }
        }

        return flag;
    }
    #endregion

    #region 获取所有玩家数据方法
    public List<PlayerData> GetAll()
    {
        var data = new List<PlayerData>();
        using var reader = TShock.DB.QueryReader("SELECT * FROM ModifyWeapons");
        while (reader.Read())
        {
            var dictJson = reader.Get<string>("Dict");
            var dict = JsonSerializer.Deserialize<Dictionary<string, List<PlayerData.ItemData>>>(dictJson, Options);

            data.Add(new PlayerData(
                name: reader.Get<string>("Name"),
                readCount: reader.Get<int>("ReadCount"),
                hand: reader.Get<int>("Hand") == 1,
                process: reader.Get<int>("IsProcess"),
                join: reader.Get<int>("IsJoin") == 1,
                alone: reader.Get<int>("Alone") == 1,
                aloneTime: reader.Get<DateTime>("AloneTime"),
                readTime: reader.Get<DateTime>("ReadTime"),
                syncTime: reader.Get<DateTime>("SyncTime"),
                dict: dict
            ));
        }

        return data;
    }
    #endregion

    #region 清理所有数据方法
    public bool ClearData()
    {
        return TShock.DB.Query("DELETE FROM ModifyWeapons") != 0;
    }
    #endregion
}