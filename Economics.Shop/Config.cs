
using Economics.Shop.Model;
using Newtonsoft.Json;
using System.Reflection.Metadata.Ecma335;

namespace Economics.Shop;

public class Config
{
    [JsonProperty("最大显示页")]
    public int PageMax { get; set; } = 10;

    [JsonProperty("商品列表")]
    public List<Product> Products { get; set; }

    public Product? GetProduct(int index)
    {
        if (Products.Count < index || index < 1)
            return null;
        return Products[index - 1];
    }
}
