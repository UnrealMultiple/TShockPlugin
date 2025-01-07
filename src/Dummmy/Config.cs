using LazyAPI;
using LazyAPI.ConfigFiles;
using TrProtocol.Packets;

namespace Dummmy;

[Config]
public class Config : JsonConfigBase<Config>
{

    [LocalizedPropertyName(CultureType.Chinese, "假人")]
    [LocalizedPropertyName(CultureType.English, "Dummmys")]
    public SyncPlayer[] Dummmys = Array.Empty<SyncPlayer>();

    protected override void SetDefault()
    {
        this.Dummmys = new SyncPlayer[1];
        this.Dummmys[0] = new SyncPlayer() { Name = "熙恩" };
    }
}