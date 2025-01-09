using GetText;
using Localizer;
using Newtonsoft.Json;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace DumpPluginsList;

[ApiVersion(2, 1)]
public class DumpPluginsList : TerrariaPlugin
{
    public override string Author => "SGKoishi";

    public override string Description => GetString("Dump plugin list and exit");

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 0, 1, 7);

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
        try
        {
            var manifestsDir = Terraria.Program.LaunchParameters.GetValueOrDefault("-dump-plugins-list-only");
            var dict = ((Dictionary<string, Assembly>?) typeof(ServerApi)
                    .GetField("loadedAssemblies", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)?
                    .GetValue(null))!
                .ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
            var des = this.GetDescriptions();
            File.WriteAllText("Plugins.json", JsonConvert.SerializeObject(ServerApi.Plugins
                .OrderBy(p => p.Plugin.Order).ThenBy(p => p.Plugin.Name)
                .Where(p => p.Plugin.GetType().Assembly != typeof(TShockAPI.TShock).Assembly && p.Plugin.GetType().Assembly != this.GetType().Assembly)
                .Select(p =>
                {
                    var manifestFilePath = !string.IsNullOrEmpty(manifestsDir) ? Path.Combine(manifestsDir, $"{p.Plugin.GetType().Assembly.GetName().Name!}.json") : null;
                    var manifest = File.Exists(manifestFilePath) ? JsonConvert.DeserializeObject<ManifestModel>(File.ReadAllText(manifestFilePath)) : null;

                    return new
                    {
                        Name = manifest?.Name ?? p.Plugin.Name,
                        Version = manifest?.Version ?? p.Plugin.Version,
                        Author = manifest?.Author ?? p.Plugin.Author,
                        Description = des[p.Plugin],
                        AssemblyName = p.Plugin.GetType().Assembly.GetName().Name,
                        Path = dict.TryGetValue(p.Plugin.GetType().Assembly, out var name) ? name + ".dll" : null,
                        Dependencies = manifest?.Dependencies ?? p.Plugin.GetType().Assembly.GetReferencedAssemblies()
                            .IntersectBy(dict.Values, n => n.Name)
                            .Where(n => n.Name != "TShockAPI")
                            .Select(n => n.Name),
                        HotReload = manifest?.HotReload ?? true
                    };
                }), Formatting.Indented));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{nameof(DumpPluginsList)}] {ex}");
            Environment.Exit(-1);
        }
        finally
        {
            Environment.Exit(0);
        }
    }

    public Dictionary<TerrariaPlugin, Dictionary<string, string>> GetDescriptions()
    {
        var dic = new Dictionary<TerrariaPlugin, Dictionary<string, string>>();
        var flag = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        var func = typeof(I18n).GetMethod("GetCatalog", flag)!.CreateDelegate<Func<Catalog>>();
        Terraria.Localization.GameCulture._legacyCultures.Keys.ForEach(i =>
        {
            Terraria.Localization.LanguageManager.Instance.SetLanguage(i);
            var c = func();
            var langName = c.CultureInfo.Name == "zh-Hans" ? "zh-CN" : c.CultureInfo.Name;
            ServerApi.Plugins.ForEach(p =>
            {
                if (!dic.TryGetValue(p.Plugin, out var des) || des == null)
                {
                    dic[p.Plugin] = new Dictionary<string, string>();
                }
                dic[p.Plugin][langName] = c.GetString(p.Plugin.Description);
            });
        });
        return dic;
    }
}
internal record ManifestModel(string? Name, Version? Version, string? Author, string? Description, string[]? Dependencies, bool? HotReload);