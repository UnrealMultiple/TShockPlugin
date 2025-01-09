using Newtonsoft.Json;
using TShockAPI;

public class Configuration
{
    public static readonly string FilePath = Path.Combine(TShock.SavePath, "EndureBoost.json");

    [JsonProperty("自动更新频率(s)")]
    public int UpdateFrequency { get; set; } = -1;
    [JsonProperty("猪猪储钱罐")]
    public bool bank = true;
    [JsonProperty("保险箱")]
    public bool bank2 = true;
    [JsonProperty("护卫熔炉")]
    public bool bank3 = false;
    [JsonProperty("虚空宝藏袋")]
    public bool bank4 = false;
    [JsonProperty("持续时间(s)")]
    public int duration = 3600;

    public class Potion
    {
        [JsonProperty("药水id")]
        public int[] PotionID { get; set; } = Array.Empty<int>();
        [JsonProperty("药水数量")]
        public int RequiredStack { get; set; }
    }

    public class Inventory
    {
        [JsonProperty("同时拥有时触发")]
        public bool Trigger { get; set; }
        [JsonProperty("物品id")]
        public int[] ItemID { get; set; } = Array.Empty<int>();
        [JsonProperty("物品数量")]
        public int RequiredStack { get; set; }
        [JsonProperty("给buff的id")]
        public int[]? BuffType { get; set; }
    }

    public class Accessorie
    {
        [JsonProperty("同时拥有时触发")]
        public bool Trigger { get; set; }

        [JsonProperty("配饰id")]
        public int[] AccessorieID { get; set; } = Array.Empty<int>();

        [JsonProperty("物品数量")]
        public int RequiredStack { get; set; }

        [JsonProperty("给buff的id")]
        public int[]? BuffType { get; set; }

        [JsonProperty("非本装备栏也触发")]
        public bool AllowOtherLoadouts { get; set; }

        [JsonProperty("社交栏也触发")]
        public bool AllowSocialSlot { get; set; }
    }


    public class Dye
    {
        [JsonProperty("同时拥有时触发")]
        public bool Trigger { get; set; }
        [JsonProperty("染料id")]
        public int[] DyeID { get; set; } = Array.Empty<int>(); // 包含染料的ID
        [JsonProperty("物品数量")]
        public int RequiredStack { get; set; }
        [JsonProperty("给buff的id")]
        public int[]? BuffType { get; set; }
        [JsonProperty("非本装备栏也触发")]
        public bool AllowOtherLoadouts { get; set; }
    }

    public class Custom
    {
        [JsonProperty("同时拥有时触发")]
        public bool Trigger { get; set; }
        [JsonProperty("物品id")]
        public int[] CustomItemID { get; set; } = Array.Empty<int>();
        [JsonProperty("物品数量")]
        public int RequiredStack { get; set; }
        [JsonProperty("给buff的id")]
        public int[]? BuffType { get; set; }
    }

    [JsonProperty("药水")]
    public List<Potion> Potions { get; set; } = new List<Potion>();
    [JsonProperty("背包")]
    public List<Inventory> Inventorys { get; set; } = new List<Inventory>();
    [JsonProperty("配饰")]
    public List<Accessorie> Accessories { get; set; } = new List<Accessorie>();
    [JsonProperty("染料")]
    public List<Dye> Dyes { get; set; } = new List<Dye>();
    [JsonProperty("全部物品")]
    public List<Custom> Customs { get; set; } = new List<Custom>();

    public void Write(string path)
    {
        using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
        {
            var str = JsonConvert.SerializeObject(this, Formatting.Indented);
            using (var sw = new StreamWriter(fs))
            {
                sw.Write(str);
            }
        }
    }

    public static Configuration Read(string path)
    {
        if (!File.Exists(path))
        {
            return new Configuration();
        }

        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var sr = new StreamReader(fs);
        return JsonConvert.DeserializeObject<Configuration>(sr.ReadToEnd()) ?? new();
    }
}