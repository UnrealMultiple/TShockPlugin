using Terraria;
using TerrariaApi.Server;

namespace LocalizerTestingPlugin;

[ApiVersion(2, 1)]
// ReSharper disable once UnusedType.Global
public class Plugin : TerrariaPlugin
{
    public override string Author => "LaoSparrow";
    public override string Description => "Test";
    public override string Name => "I18nTestingPlugin";
    public override Version Version => new Version(1, 0, 0);
    
    public Plugin(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        Console.WriteLine(GetString("No locale"));
    }
}