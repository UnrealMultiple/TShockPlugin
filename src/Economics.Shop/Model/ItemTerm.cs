using Economics.Core.Model;
using Newtonsoft.Json;

namespace Economics.Shop.Model;

public class ItemTerm : Item
{
    [JsonProperty("是否消耗")]
    public bool Consume { get; set; } = false;
}