using Newtonsoft.Json;
using TShockAPI;

namespace ConvertWorld
{
    internal class Configuration
    {
        [JsonProperty("使用说明", Order = -8)]
        public string Text { get; set; } = "击败指定NPC将世界所有指定图格与箱子内物品对比1:1转换";
        [JsonProperty("插件开关", Order = -8)]
        public bool NpcKilled { get; set; } = true;
        [JsonProperty("击败所有", Order = -7)]
        public bool KillAll { get; set; } = false;

        [JsonProperty("击杀转换表", Order = 0)]
        public List<ItemData> BossList { get; set; } = new List<ItemData>();

        #region 数据结构
        public class ItemData
        {
            [JsonProperty("怪物名", Order = -4)]
            public string Name { get; set; }
            [JsonProperty("怪物ID", Order = -3)]
            public int[] ID { get; set; }

            [JsonProperty("世界图格替换表", Order = -2)]
            public Dictionary<ushort, ushort> TileID { get; set; }

            [JsonProperty("箱子物品替换表", Order = -1)]
            public Dictionary<int, int> ItemID { get; set; }

            public ItemData(string name, int[] id, Dictionary<ushort, ushort> tileID, Dictionary<int, int> itemID)
            {
                Name = name ?? "";
                ID = id ?? new int[] { 113 };
                TileID = tileID ?? new Dictionary<ushort, ushort>();
                ItemID = itemID ?? new Dictionary<int, int>();
            }

        }
        #endregion


        #region 预设参数方法
        public void Ints()
        {
            BossList =
            new List<ItemData>
            {
                #region 血肉之墙
                new ItemData("", new int[] { 113 },
                new Dictionary<ushort, ushort>
                {
                    {7, 58},                // 铜矿 换 狱石
                    {166, 58},            // 锡矿 换 狱石
                    {6, 107},              // 铁矿 换 钴矿
                    {167, 221},          // 铅矿 换 钯金
                    {9, 108},              // 银矿 换 秘银
                    {168, 222},          // 钨矿 换 山铜
                    {8, 111},              // 金矿 换 精金
                    {169, 223},          // 铂金矿 换 钛金
                },

                new Dictionary<int, int>
                {
                    {9,621},              // 木头 换 珍珠木
                    {188,499},          // 治疗药水 换 强效治疗药水
                    {189,500},          // 魔力药水 换 强效魔力药水
                    {964,534},          // 三发猎枪 换 霰弹枪
                    {848, 857}          // 法老面具 换 沙暴瓶
                }), 
	            #endregion

                #region 世纪之花
                new ItemData("", new int[] { 262 },
                new Dictionary<ushort, ushort>
                {
                    {12, 236},          // 生命水晶 换 生命果
                    {22, 211},          // 魔矿 换 叶绿矿
                    {204, 211},        // 猩红矿 换 叶绿矿
                },

                new Dictionary<int, int>
                {
                    {953,976},          // 攀爬爪 换 猛虎攀爬装备
                    {975,976},          // 鞋钉 换 猛虎攀爬装备
                    {29,1291},          // 生命水晶 换 生命果
                }) 
	            #endregion
           };
        }
        #endregion

        #region 读取与创建配置文件方法
        public static readonly string FilePath = Path.Combine(TShock.SavePath, "击败怪物替换世界物品.json");

        public void Write()
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(FilePath, json);
        }

        public static Configuration Read()
        {
            if (!File.Exists(FilePath))
            {
                var NewConfig = new Configuration();
                NewConfig.Ints();
                new Configuration().Write();
                return NewConfig;
            }
            else
            {
                string jsonContent = File.ReadAllText(FilePath);
                return JsonConvert.DeserializeObject<Configuration>(jsonContent)!;
            }
        }
        #endregion

    }
}