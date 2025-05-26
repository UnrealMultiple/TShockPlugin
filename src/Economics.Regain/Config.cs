using Economics.Core.ConfigFiles;
using Newtonsoft.Json;
using TShockAPI;

namespace Economics.Regain;

public class Config : JsonConfigBase<Config>
{
    protected override string Filename => "Regain.json";
    public class RegainInfo
    {
        [JsonProperty("物品ID")]
        public int ID { get; set; }

        [JsonProperty("回收价格")]
        public List<RedemptionRelationshipsOption> RedemptionRelationshipsOption { get; set; } = new();

        public override string ToString()
        {
            var item = TShock.Utils.GetItemById(this.ID);
            return GetString($"[i:{this.ID}] {item.Name} 价格:{string.Join(" ", this.RedemptionRelationshipsOption.Select(r => $"{r.CurrencyType}=>{r.Number}"))}");
        }
    }

    [JsonProperty("最大显示页")]
    public int PageMax { get; set; } = 20;

    [JsonProperty("回收物品表")]
    public List<RegainInfo> Regains { get; set; } = new();

    public bool TryGetRegain(int id, out RegainInfo? value)
    {
        value = this.Regains.Find(x => x.ID == id);
        return value != null;
    }
}