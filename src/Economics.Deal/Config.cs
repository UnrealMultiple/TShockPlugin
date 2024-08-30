using EconomicsAPI.Configured;
using Newtonsoft.Json;
using TShockAPI;

namespace Economics.Deal;

public class Config
{
    [JsonProperty("最大显示页")]
    public int PageMax { get; set; } = 10;

    [JsonProperty("交易列表")]
    public List<DealContext> DealContexts { get; set; } = new();

    public void PushItem(TSPlayer player, long Cost)
    {
        var DealContext = new DealContext()
        {
            Publisher = player.Name,
            Cost = Cost,
            Item = new()
            {
                netID = player.SelectedItem.netID,
                Stack = player.SelectedItem.stack,
                Prefix = player.SelectedItem.prefix
            }
        };
        DealContexts.Add(DealContext);
        ConfigHelper.Write(Deal.PATH, this);
    }

    public void RemoveItem(int index)
    {
        DealContexts.RemoveAt(index);
        ConfigHelper.Write(Deal.PATH, this);
    }

    public DealContext? GetDealContext(int index)
    {
        if (index > DealContexts.Count || index < 1)
            return null;
        return DealContexts[index - 1];
    }
}
