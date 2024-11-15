using Newtonsoft.Json;
using System.Text;
using Terraria.ID;
using TShockAPI;

namespace Plugin;

internal class Configuration
{
    public static readonly string FilePath = Path.Combine(TShock.SavePath, "服主特权.json");

    #region 实例变量
    [JsonProperty("使用说明", Order = -20)]
    public string text = "根据名单进服或复活自动执行：无敌、BUFF、物品、命令、设置背包，用户组名无视所有开关，开关只对特权名单有效，需关用户组权限直接删组名";

    [JsonProperty("进服给无敌", Order = -19)]
    public bool GodMode { get; set; } = true;
    [JsonProperty("进服用命令", Order = -18)]
    public bool Cmd { get; set; } = false;
    [JsonProperty("进服给物品", Order = -17)]
    public bool item { get; set; } = false;
    [JsonProperty("进服给BUFF", Order = -16)]
    public bool Buff { get; set; } = true;
    [JsonProperty("设置SSC背包", Order = -16)]
    public bool SSC { get; set; } = true;
    [JsonProperty("特权名单/受以上开关影响", Order = -15)]
    public HashSet<string> PlayersList { get; set; } = new HashSet<string>();

    [JsonProperty("所有特权的用户组名", Order = -14)]
    public string AllPerm { get; set; } = "admin,owner";
    [JsonProperty("无敌权限的用户组名", Order = -13)]
    public string GodPerm { get; set; } = "admin,owner";
    [JsonProperty("物品权限的用户组名", Order = -12)]
    public string BuffPerm { get; set; } = "admin,owner";
    [JsonProperty("BUFF权限的用户组名", Order = -11)]
    public string ItemPerm { get; set; } = "admin,owner";
    [JsonProperty("命令权限的用户组名", Order = -10)]
    public string CmdPerm { get; set; } = "admin,owner";
    [JsonProperty("特权背包的用户组名", Order = -9)]
    public string SSCPerm { get; set; } = "admin,owner";

    [JsonProperty("命令表", Order = -8)]
    public string[] CmdList { get; set; } = new string[] { "/clear i 9999", ".clear p 9999" };

    [JsonProperty("物品表（ID:数量）", Order = -7)]
    [JsonConverter(typeof(ItemListConverter))]
    public List<ItemData> ItemList { get; set; } = new List<ItemData>();

    [JsonProperty("Buff表（ID:分钟）", Order = -6)]
    [JsonConverter(typeof(BuffListConverter))]
    public List<BuffData> BuffList { get; set; } = new List<BuffData>();

    [JsonProperty("特权背包表", Order = -5)]
    public List<SSCData> SSCLists { get; set; } = new List<SSCData>();

    [JsonProperty("非特权背包表", Order = -4)]
    public List<SSCData2> SSCLists2 { get; set; } = new List<SSCData2>();



    #endregion

    #region 读取与创建配置文件方法
    public void Write()
    {
        using (var fs = new FileStream(FilePath, FileMode.Create, FileAccess.Write, FileShare.Write))
        using (var sw = new StreamWriter(fs, new UTF8Encoding(false)))
        {
            sw.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
        }
    }

    public static Configuration Read()
    {
        if (!File.Exists(FilePath))
        {
            var NewConfig = new Configuration();
            NewConfig.BuffList.Add(new BuffData(1, 10));
            NewConfig.BuffList.Add(new BuffData(BuffID.Shine, -1));
            NewConfig.BuffList.Add(new BuffData(BuffID.NightOwl, -1));
            NewConfig.BuffList.Add(new BuffData(BuffID.Swiftness, -1));
            NewConfig.BuffList.Add(new BuffData(BuffID.SugarRush, -1));
            NewConfig.BuffList.Add(new BuffData(BuffID.DryadsWard, -1));
            NewConfig.BuffList.Add(new BuffData(BuffID.WellFed3, -1));
            NewConfig.BuffList.Add(new BuffData(BuffID.VoltBunny, -1));
            NewConfig.BuffList.Add(new BuffData(BuffID.Panic, -1));
            NewConfig.BuffList.Add(new BuffData(BuffID.Lucky, -1));
            NewConfig.ItemList.Add(new ItemData(ItemID.EncumberingStone, 1));
            NewConfig.SSCLists.Add(new SSCData(200, 100, Items()));
            NewConfig.SSCLists2.Add(new SSCData2(100, 20, Items2()));
            NewConfig.PlayersList = new HashSet<string> { "羽学", "灵乐" };
            new Configuration().Write();
            return NewConfig;
        }
        else
        {
            var jsonContent = File.ReadAllText(FilePath);
            return JsonConvert.DeserializeObject<Configuration>(jsonContent)!;
        }
    }
    #endregion

    #region Buff数据
    public class BuffListConverter : JsonConverter<List<BuffData>>
    {
        public override void WriteJson(JsonWriter writer, List<BuffData>? value, JsonSerializer serializer)
        {
            var BuffDict = value!.ToDictionary(Buff => Buff.ID, Buff => Buff.Min);
            serializer.Serialize(writer, BuffDict);
        }

        public override List<BuffData> ReadJson(JsonReader reader, Type objectType, List<BuffData>? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var buffDict = serializer.Deserialize<Dictionary<int, int>>(reader);
            return buffDict?.Select(kv => new BuffData(kv.Key, kv.Value)).ToList() ?? new List<BuffData>();
        }
    }

    public class BuffData
    {
        public int ID { get; set; }
        public int Min { get; set; }

        public BuffData(int id, int minute)
        {
            this.ID = id;
            this.Min = minute;
        }
    }
    #endregion

    #region 物品数据
    public class ItemListConverter : JsonConverter<List<ItemData>>
    {
        public override void WriteJson(JsonWriter writer, List<ItemData>? value, JsonSerializer serializer)
        {
            var itemDict = value!.ToDictionary(item => item.ID, item => item.Stack);
            serializer.Serialize(writer, itemDict);
        }

        public override List<ItemData> ReadJson(JsonReader reader, Type objectType, List<ItemData>? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var itemDict = serializer.Deserialize<Dictionary<int, int>>(reader);
            return itemDict?.Select(kv => new ItemData(kv.Key, kv.Value)).ToList() ?? new List<ItemData>();
        }
    }

    public class ItemData
    {
        public int ID { get; set; }
        public int Stack { get; set; }
        public ItemData(int id, int stack)
        {
            this.ID = id;
            this.Stack = stack;
        }
    }
    #endregion

    #region 特权背包数据
    public class SSCData
    {
        [JsonProperty("初始血量")]
        public int Health { get; set; }
        [JsonProperty("初始魔力")]
        public int Mana { get; set; }
        [JsonProperty("初始物品")]
        public List<NetItem> Inventory { get; set; }
        public SSCData(int health, int mana, List<NetItem> inventory)
        {
            this.Health = health;
            this.Mana = mana;
            this.Inventory = inventory;
        }
    }

    private static List<NetItem> Items()
    {
        var Inventory = new List<NetItem>
        {
            new NetItem(4956, 1, 81),
            new NetItem(4346, 1, 0),
            new NetItem(5437, 1, 0)
        };
        return Inventory;
    }
    #endregion

    #region 非特权背包数据
    public class SSCData2
    {
        [JsonProperty("初始血量")]
        public int Health { get; set; }
        [JsonProperty("初始魔力")]
        public int Mana { get; set; }
        [JsonProperty("初始物品")]
        public List<NetItem> Inventory { get; set; }
        public SSCData2(int health, int mana, List<NetItem> inventory)
        {
            this.Health = health;
            this.Mana = mana;
            this.Inventory = inventory;
        }
    }

    private static List<NetItem> Items2()
    {
        var Inventory = new List<NetItem>
        {
            new NetItem(-15, 1, 0),
            new NetItem(-13, 1, 0),
            new NetItem(-16, 1, 0)
        };
        return Inventory;
    }
    #endregion

}