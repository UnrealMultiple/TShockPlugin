using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;
namespace BanNpc;


[Config]
public class Config : JsonConfigBase<Config>
{
    protected override string Filename => "BanNpc";

    [LocalizedPropertyName(CultureType.Chinese, "阻止怪物生成表")]
    [LocalizedPropertyName(CultureType.English, "BanNpcs")]
    public HashSet<int> Npcs { get; set; } = new();

}