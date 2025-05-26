
using Economics.Core.ConfigFiles;
using Economics.Shop.Model;
using Newtonsoft.Json;

namespace Economics.Shop;

public class Config : JsonConfigBase<Config>
{
    protected override string Filename => "Shop.json";

    [JsonProperty("最大显示页")]
    public int PageMax { get; set; } = 10;

    [JsonProperty("商品列表")]
    public List<Product> Products { get; set; } = [];

    protected override void SetDefault()
    {
        this.Products.Add(new()
        {
            Commamds = ["/gbuff {0} 114"],
            ItemTerm = [new()],
            Items = [new()],
            LevelLimit = ["萌新"],
            Name = "Example",
            ProgressLimit = ["肉山"],
            RedemptionRelationshipsOption = [new()],
        });
    }

    public Product? GetProduct(int index)
    {
        return this.Products.Count < index || index < 1 ? null : this.Products[index - 1];
    }
}