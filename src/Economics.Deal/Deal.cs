using EconomicsAPI.Configured;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI.Hooks;

namespace Economics.Deal;

[ApiVersion(2, 1)]
public class Deal : TerrariaPlugin
{
    public override string Author => "少司命";

    public override string Description => GetString("玩家可以进行交易");

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(2, 0, 0, 4);

    internal static string PATH = Path.Combine(EconomicsAPI.Economics.SaveDirPath, "Deal.json");

    internal static Config Config { get; set; } = new();

    public Deal(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        this.LoadConfig();
        GeneralHooks.ReloadEvent += (e) => Config = ConfigHelper.LoadConfig(PATH, Config);
    }

    private void LoadConfig(ReloadEventArgs? args = null)
    {
        Config = ConfigHelper.LoadConfig(PATH, Config);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            EconomicsAPI.Economics.RemoveAssemblyCommands(Assembly.GetExecutingAssembly());
            EconomicsAPI.Economics.RemoveAssemblyRest(Assembly.GetExecutingAssembly());

            GeneralHooks.ReloadEvent -= this.LoadConfig;
        }
        base.Dispose(disposing);
    }
}