using Newtonsoft.Json;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;

namespace DumpPluginsList;

[ApiVersion(2, 1)]
public class DumpPluginsList : TerrariaPlugin
{
    public override string Author => "SGKoishi";

    public override string Description => "Dump plugin list and exit";

    public override string Name => "DumpPluginsList";

    public override Version Version => new(1, 0, 1, 3);

    public DumpPluginsList(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        if (Terraria.Program.LaunchParameters.ContainsKey("-dump-plugins-list-only"))
        {
            ServerApi.Hooks.GameInitialize.Register(this, this.Dump);
        }
    }

    private void Dump(EventArgs args)
    {
        var dict = ((Dictionary<string, Assembly>?) typeof(ServerApi)
            .GetField("loadedAssemblies", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)?
            .GetValue(null))!
            .ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        File.WriteAllText("Plugins.json", JsonConvert.SerializeObject(ServerApi.Plugins
            .OrderBy(p => p.Plugin.Order).ThenBy(p => p.Plugin.Name)
            .Where(p => p.Plugin.GetType().Assembly != typeof(TShockAPI.TShock).Assembly)
            .Select(p => new
            {
                p.Plugin.Name,
                p.Plugin.Version,
                p.Plugin.Author,
                p.Plugin.Description,
                AssemblyName = p.Plugin.GetType().Assembly.GetName().Name,
                Path = dict.TryGetValue(p.Plugin.GetType().Assembly, out var name) ? name + ".dll" : null,
                Dependencies = p.Plugin.GetType().Assembly.GetReferencedAssemblies()
                    .IntersectBy(dict.Values, n => n.Name)
                    .Where(n => n.Name != "TShockAPI")
                    .Select(n => n.Name)
            }), Formatting.Indented));
        Environment.Exit(0);
    }
}