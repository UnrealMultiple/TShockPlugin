using LazyAPI;
using LazyAPI.ConfigFiles;
using Newtonsoft.Json;
namespace BanNpc;


[Config]
public class Config : JsonConfigBase<Config>
{
    protected override string Filename => "BanNpc";

    [LocalizedPropertyName(CultureType.Chinese, "阻止怪物生成表")]
    [LocalizedPropertyName(CultureType.English, "banNpcs")]
    public HashSet<int> Npcs { get; set; } = new();

}