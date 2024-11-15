using EconomicsAPI.Configured;
using IL.Terraria.GameContent.Creative;
using Newtonsoft.Json;
using TShockAPI;

namespace Economics.Deal;

public class Config
{
    [JsonProperty("最大显示页")]
    public int PageMax { get; set; } = 10;

    [JsonProperty("交易列表")]
    public List<DealContext> DealContexts { get; set; } = new();

    public void PushItem(TSPlayer player, long Cost, string type)
    {
        var DealContext = new DealContext()
        {
            Publisher = player.Name,
            RedemptionRelationships = new() { CurrencyType = type, Number = Cost},
            Item = new()
            {
                netID = player.SelectedItem.netID,
                Stack = player.SelectedItem.stack,
                Prefix = player.SelectedItem.prefix
            }
        };
        this.DealContexts.Add(DealContext);
        ConfigHelper.Write(Deal.PATH, this);
    }

    public void RemoveItem(int index)
    {
        this.DealContexts.RemoveAt(index);
        ConfigHelper.Write(Deal.PATH, this);
    }

    public DealContext? GetDealContext(int index)
    {
        return index > this.DealContexts.Count || index < 1 ? null : this.DealContexts[index - 1];
    }
}