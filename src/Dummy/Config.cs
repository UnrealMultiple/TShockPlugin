using LazyAPI;
using LazyAPI.ConfigFiles;
using TrProtocol.Packets;

namespace Dummy;

[Config]
public class Config : JsonConfigBase<Config>
{

    [LocalizedPropertyName(CultureType.Chinese, "假人")]
    [LocalizedPropertyName(CultureType.English, "Dummys")]
    public SyncPlayer[] Dummys = Array.Empty<SyncPlayer>();

    protected override void SetDefault()
    {
        this.Dummys = new SyncPlayer[1];
        this.Dummys[0] = new SyncPlayer() { Name = "熙恩" };
    }
}