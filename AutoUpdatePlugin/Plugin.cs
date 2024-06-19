using System.IO.Compression;
using System.Reflection;
using Newtonsoft.Json;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace AutoUpdatePlugin;

[ApiVersion(2,1)]
public class Plugin : TerrariaPlugin
{
    public override string Name => "AutoUpdatePlugin";
    public override Version Version => new Version(1, 6, 0, 2);
    public override string Author => "少司命，Cai";
    public override string Description => "自动更新你的插件！";

    private const string ReleaseUrl = "https://github.com/Controllerdestiny/TShockPlugin/releases/download/V1.0.0.0/Plugins.zip";

    private const string PUrl = "https://github.moeyy.xyz/";

    private const string PluginsUrl = "https://raw.githubusercontent.com/Controllerdestiny/TShockPlugin/master/Plugins.json";

    private const string ZipName = "TempPlugin.zip";

    private const string SaveDir = ".temp";

    private static HttpClient _httpClient = new();


    public Plugin(Main game) : base(game)
    {

    }

    public override void Initialize()
    {
        Commands.ChatCommands.Add(new("AutoUpdatePlugin", CheckCmd, "cplugin"));
        Commands.ChatCommands.Add(new("AutoUpdatePlugin", UpdateCmd, "uplugin"));
    }

    private void UpdateCmd(CommandArgs args)
    {
        List<PluginUpdateInfo> updates;
        try
        {
            updates = GetUpdate();
        }
        catch (Exception ex)
        {
            args.Player.SendErrorMessage("无法获取更新:" + ex.Message);
            return;
        }
        if (updates.Count == 0)
        {
            args.Player.SendSuccessMessage("你的插件全是最新版本，无需更新哦~");
            return;
        }
        args.Player.SendInfoMessage("正在下载最新插件包...");
        DownLoadPlugin();
        args.Player.SendInfoMessage("正在解压插件包...");
        ExtractDirectoryZip();
        args.Player.SendInfoMessage("正在升级插件...");
        UpdatePlugin(updates);
        args.Player.SendSuccessMessage("[更新完成]\n" + string.Join("\n", updates.Select(i => $"{i.Name} v{i.OldVersion}  =>  {i.Name} v{i.NewVersion}")));
        args.Player.SendSuccessMessage("重启服务器后插件生效!");

    }

    private void CheckCmd(CommandArgs args)
    {
        List<PluginUpdateInfo> updates;
        try
        {
            updates = GetUpdate(); 
        }
        catch (Exception ex)
        {
            args.Player.SendErrorMessage("无法获取更新:"+ex.Message);
            return;
        }
        if (updates.Count == 0)
        {
            args.Player.SendSuccessMessage("你的插件全是最新版本，无需更新哦~");
            return;
        }
        args.Player.SendInfoMessage("[插件更新列表]\n" + string.Join("\n", updates.Select(i => $"{i.Name} v{i.OldVersion}  =>  {i.Name} v{i.NewVersion}")));

    }

    #region 工具方法
    private static List<PluginUpdateInfo> GetUpdate()
    {
        var plugins = GetPlugins();
        HttpClient httpClient = new();
        var response = httpClient.GetAsync(PUrl+PluginsUrl).Result;

        if (!response.IsSuccessStatusCode)
            throw new Exception("无法连接服务器");
        var json = response.Content.ReadAsStringAsync().Result;
        var latestPluginList = JsonConvert.DeserializeObject<List<PluginVersionInfo>>(json)!;
        List<PluginUpdateInfo> pluginUpdateList = new();
        foreach (var latestPluginInfo in latestPluginList)
            foreach (var plugin in plugins)
                if (plugin.Name == latestPluginInfo.Name && plugin.Version != latestPluginInfo.Version)
                    pluginUpdateList.Add(new PluginUpdateInfo(plugin.Name, plugin.Author, latestPluginInfo.Version, plugin.Version, plugin.Path, latestPluginInfo.Path));
        return pluginUpdateList;
         // Console.WriteLine($"{i.Name} v{i.OldVersion}({i.LocalPath}) => {i.Name} v{i.NewVersion} ({i.RemotePath})");
    }

    private static List<PluginVersionInfo> GetPlugins()
    {
        List<PluginVersionInfo> plugins = new();
        foreach (var plugin in ServerApi.Plugins)
        {
            plugins.Add(new PluginVersionInfo()
            {
                AssemblyName = plugin.Plugin.GetType().Assembly.GetName().Name,
                Author = plugin.Plugin.Author,
                Name = plugin.Plugin.Name,
                Description = plugin.Plugin.Description,
                Version = plugin.Plugin.Version.ToString()
            });
        }
        List<FileInfo> fileInfos = new DirectoryInfo(ServerApi.ServerPluginsDirectoryPath).GetFiles("*.dll").ToList();
        fileInfos.AddRange(new DirectoryInfo(ServerApi.ServerPluginsDirectoryPath).GetFiles("*.dll-plugin"));
        foreach (FileInfo fileInfo in fileInfos)
        {
            try
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.Name);
                Assembly assembly;
                byte[] pe = null;
                assembly = Assembly.Load(pe = File.ReadAllBytes(fileInfo.FullName));
                for (int i =0;i<plugins.Count;i++)
                {
                    if (plugins[i].AssemblyName==assembly.GetName().Name)
                    {
                        plugins[i].Path = fileInfo.Name;
                    }
                }
            }
            catch (Exception)
            {
                continue;
            }
        }
        return plugins;
    }


    private static void DownLoadPlugin()
    {
        DirectoryInfo directoryInfo = new("TempFile");
        if (!directoryInfo.Exists)
            directoryInfo.Create();
        HttpClient httpClient = new();
        var zipBytes = httpClient.GetByteArrayAsync(PUrl + ReleaseUrl).Result;
        File.WriteAllBytes(Path.Combine(directoryInfo.FullName,"Plugins.zip"), zipBytes);
    }
    private static void ExtractDirectoryZip()
    {
        DirectoryInfo directoryInfo = new("TempFile");
        ZipFile.ExtractToDirectory(Path.Combine(directoryInfo.FullName, "Plugins.zip"), Path.Combine(directoryInfo.FullName, "Plugins"),true);
    }

    private static void UpdatePlugin(List<PluginUpdateInfo> pluginUpdateInfos)
    {
        foreach (var pluginUpdateInfo in pluginUpdateInfos)
        {
            string sourcePath = Path.Combine("TempFile", "Plugins", pluginUpdateInfo.RemotePath);
            string destinationPath = Path.Combine("ServerPlugins", pluginUpdateInfo.LocalPath);
            // 确保目标目录存在
            string destinationDirectory = Path.GetDirectoryName(destinationPath);
            // 复制并覆盖文件
            File.Copy(sourcePath, destinationPath, true);
        }
    }
    #endregion
}
