using Terraria;
using TerrariaApi.Server;
using System.Reflection;
using TShockAPI;

namespace PluginMutex;

[ApiVersion(2,1)]
public class Plugin : TerrariaPlugin
{
    public Plugin(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        Commands.ChatCommands.Add(new(this.test, "check"));
    }

    private void test(CommandArgs args)
    {
        if (typeof(ServerApi)
            .GetField("loadedAssemblies", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance)
            !.GetValue(null) is Dictionary<string, Assembly> loadassemblys)
        {
            var mutexs = loadassemblys
                .GroupBy(x => x.Value.GetName().FullName)
                .Where(x => x.Count() > 1)
                .SelectMany(x => x);
            args.Player.SendErrorMessage("[插件重复安装]" + string.Join(" >>> ", mutexs.Select(x => x.Key + ".dll")));
        }
    }
}
