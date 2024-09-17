using Newtonsoft.Json;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace ChestRestore;

[ApiVersion(2, 1)]
public class MainPlugin : TerrariaPlugin
{
    public MainPlugin(Main game) : base(game)
    {
    }
    public override string Name => "ChestRestore";
    public override Version Version => new Version(1, 0, 2);
    public override string Author => "Cjx重构 | 肝帝熙恩简单修改";
    public override string Description => "无限宝箱插件";

    public override void Initialize()
    {
        ServerApi.Hooks.NetGetData.Register(this, OnGetData);
    }
    private void OnGetData(GetDataEventArgs args)
    {
        if (args.MsgID == PacketTypes.ChestItem)
        {
            args.Handled = true;
        }
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
        }
        base.Dispose(disposing);
    }
}