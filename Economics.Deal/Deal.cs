using EconomicsAPI.Attributes;
using EconomicsAPI.Configured;
using Microsoft.Xna.Framework;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace Economics.Deal;

[ApiVersion(2, 1)]
public class Deal : TerrariaPlugin
{
    public override string Author => "少司命";

    public override string Description => Assembly.GetExecutingAssembly().GetName().Name!;

    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;

    public override Version Version => new Version(1, 0, 0, 2);

    internal static string PATH = Path.Combine(EconomicsAPI.Economics.SaveDirPath, "Deal.json");

    internal static Config Config { get; set; } = new();

    public Deal(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        LoadConfig();
        GeneralHooks.ReloadEvent += (e) => Config = ConfigHelper.LoadConfig(PATH, Config);
    }

    private void LoadConfig(ReloadEventArgs? args = null) => Config = ConfigHelper.LoadConfig(PATH, Config);

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            EconomicsAPI.Economics.RemoveAssemblyCommands(Assembly.GetExecutingAssembly());
            EconomicsAPI.Economics.RemoveAssemblyRest(Assembly.GetExecutingAssembly());
            
            GeneralHooks.ReloadEvent -= LoadConfig;
        }
        base.Dispose(disposing);
    }
}
