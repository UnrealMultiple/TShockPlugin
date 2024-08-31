
using Economics.Shop.Model;
using Newtonsoft.Json;

namespace Economics.Shop;

public class Config
{
    [JsonProperty("最大显示页")]
    public int PageMax { get; set; } = 10;

    [JsonProperty("商品列表")]
    public List<Product> Products { get; set; }

    public Product? GetProduct(int index)
    {
        return this.Products.Count < index || index < 1 ? null : this.Products[index - 1];
    }
}