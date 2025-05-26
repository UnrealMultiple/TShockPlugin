using Economics.Core.ConfigFiles;
using Newtonsoft.Json;

namespace Economics.Projectile;

public class Config : JsonConfigBase<Config>
{
    protected override string Filename => "Projectile.json";

    [JsonProperty("弹幕触发")]
    public Dictionary<int, ProjectileSpark> ProjectileReplace { get; set; } = [];

    [JsonProperty("物品触发 ")]
    public Dictionary<int, ItemSpark> ItemReplace { get; set; } = [];

    protected override void SetDefault()
    {
        this.ProjectileReplace.Add(274, new ProjectileSpark()
        {
            ProjData =
            [
                new()
                {
                    ID = 132,
                    Speed = 15,
                    Damage=80,
                    KnockBack = 10,
                    Limit = []
                }
            ]
        });

        this.ItemReplace.Add(1327, new()
        {
            ProjData =
            [
                new()
                {
                    ID = 132,
                    Speed = 15,
                    Damage=80,
                    KnockBack = 10,
                    Limit = []
                }
            ]
        });
    }

}