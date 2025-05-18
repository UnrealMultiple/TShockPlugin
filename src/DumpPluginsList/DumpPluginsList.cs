using GetText;
using Localizer;
using Newtonsoft.Json;
using System.Globalization;
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
    public override Version Version => new Version(1, 0, 1, 9);

    private CultureInfo CultureInfo => (CultureInfo) typeof(TShock).Assembly.GetType("TShockAPI.I18n")!.GetProperty(
            "TranslationCultureInfo",
            BindingFlags.NonPublic | BindingFlags.Static)!.GetValue(null)!;
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
                        Description = des.GetValueOrDefault(p.Plugin) ?? manifest?.Description,
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

    public Catalog? CreateCatalog(string lang, TerrariaPlugin plugin)
    {
        var asm = plugin.GetType().Assembly;
        var moFilePath = $"i18n.{lang}.mo";
        return asm.GetManifestResourceInfo(moFilePath) == null ? null : new Catalog(asm.GetManifestResourceStream(moFilePath));
    }

    public Dictionary<TerrariaPlugin, Dictionary<string, string>> GetDescriptions()
    {
        var dic = new Dictionary<TerrariaPlugin, Dictionary<string, string>>();
        var pluginDefaultDes = new Dictionary<TerrariaPlugin, string>();
        var currentCulture = this.CultureInfo;
        foreach(var p in ServerApi.Plugins)
        {
            var catalog = this.CreateCatalog(currentCulture.Name, p.Plugin);
            if (catalog == null)
            {
                pluginDefaultDes[p.Plugin] = p.Plugin.Description;
                continue;
            }
            else
            {
                if (catalog.Translations.TryGetValue(p.Plugin.Description, out var translation))
                {
                    pluginDefaultDes[p.Plugin] = translation.Length > 0 ? translation[0] : p.Plugin.Description;
                }
                else
                {
                    var (msgid, msg) = catalog.Translations.Where(i => i.Value.Contains(p.Plugin.Description)).FirstOrDefault();
                    pluginDefaultDes[p.Plugin] = msgid ?? p.Plugin.Description;
                }
                
            }
        }

        foreach (var p in ServerApi.Plugins)
        {
            foreach (var l in Terraria.Localization.GameCulture._legacyCultures)
            {
                var langName = l.Value.Name == "zh-Hans" ? "zh-CN" : l.Value.Name;
                var c = this.CreateCatalog(langName, p.Plugin);
                if (!dic.ContainsKey(p.Plugin))
                {
                    dic[p.Plugin] = new();
                }
                dic[p.Plugin][langName] = c == null ? pluginDefaultDes[p.Plugin] : c.GetString(pluginDefaultDes[p.Plugin]);
            }
        }
        return dic;
    }
}
internal record ManifestModel(string? Name, Version? Version, string? Author, Dictionary<string, string>? Description, string[]? Dependencies, bool? HotReload);