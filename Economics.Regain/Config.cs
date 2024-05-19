using Newtonsoft.Json;
using TShockAPI;

namespace Economics.Regain;

public class Config
{
    public class RegainInfo
    {
        [JsonProperty("物品ID")]
        public int ID { get; set; }

        [JsonProperty("回收价格")]
        public int Cost { get; set; }

        public override string ToString()
        {
            var item = TShock.Utils.GetItemById(ID);
            return $"[i:{ID}] {item.Name} 价格:{Cost}";
        }
    }

    [JsonProperty("最大显示页")]
    public int PageMax { get; set; } = 20;

    [JsonProperty("回收物品表")]
    public List<RegainInfo> Regains { get; set; } = new();

    public bool TryGetRegain(int id, out RegainInfo? value)
    {
        value = Regains.Find(x => x.ID == id);
        return value != null;
    }
}
