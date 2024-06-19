using Newtonsoft.Json.Linq;
using System.IO.Compression;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace AutoUpdatePlugin;

public class Plugin : TerrariaPlugin
{
    private const string ReleaseUrl = "https://github.com/Controllerdestiny/TShockPlugin/releases/download/V1.0.0.0/Plugins.zip";

    private const string PUrl = "https://gitdl.cn/";

    private const string ZipName = "TempPlugin.zip";

    private const string SaveDir = ".temp";

    private static HttpClient _httpClient = new();


    public Plugin(Main game) : base(game)
    {

    }

    public override void Initialize()
    {
        Commands.ChatCommands.Add(new("", PCmd, "uplugin"));
    }

    private void PCmd(CommandArgs args)
    {
        throw new NotImplementedException();
    }

    private static async Task ExtractDirectoryZip(string folderPath, string zipPath)
    {
        if(!File.Exists(zipPath))
            throw new FileNotFoundException("找不到压缩包" + zipPath);
        DirectoryInfo directoryInfo = new(folderPath);
        if (!directoryInfo.Exists)
            directoryInfo.Create();
        var zipBytes = await _httpClient.GetByteArrayAsync(PUrl + ReleaseUrl);
        File.WriteAllBytes(zipPath, zipBytes);
        ZipFile.ExtractToDirectory(zipPath, folderPath);
        var pluginJson = Path.Combine(folderPath, "Plugin.json");
        if(!File.Exists(pluginJson))
            throw new FileNotFoundException("无法找到Plugin.json");
        var data = File.ReadAllText(pluginJson);
        var plugins = (JArray.Parse(data)
            .ToObject<List<PluginVersionInfo>>()?
            .ToDictionary(x => x.Name, x => x)) ?? 
            throw new NullReferenceException("无法正常读取Plugin.json 获取插件信息!");
        foreach (var plugin in ServerApi.Plugins)
        {
            if (plugin.Plugin.Name != "Note" 
                && plugins.TryGetValue(plugin.Plugin.Name, out var pluginInfo) 
                && pluginInfo != null)
            {
                if (pluginInfo.Version != plugin.Plugin.Version.ToString())
                {
                    var fullName = plugin.Plugin.GetType().Assembly.GetName().FullName;
                    if (!string.IsNullOrEmpty(fullName))
                    { 
                    
                    }
                }
            }
        }
    }
}
