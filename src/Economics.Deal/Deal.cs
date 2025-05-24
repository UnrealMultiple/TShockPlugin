using Economics.Core.ConfigFiles;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI.Hooks;

namespace Economics.Deal;

[ApiVersion(2, 1)]
public class Deal(Main game) : TerrariaPlugin(game)
{
    public override string Author => "少司命";

    public override string Description => GetString("玩家可以进行交易");

    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(2, 0, 0, 5);

    public override void Initialize()
    {
        Config.Load();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Core.Economics.RemoveAssemblyCommands(Assembly.GetExecutingAssembly());
            Core.Economics.RemoveAssemblyRest(Assembly.GetExecutingAssembly());

            Config.UnLoad();
        }
        base.Dispose(disposing);
    }
}