using Newtonsoft.Json;
using System.IO.Compression;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace AutoUpdatePlugin;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    public override string Name => "AutoUpdatePlugin";

    public override Version Version => new(2024, 6, 20, 2);

    public override string Author => "少司命，Cai";

    public override string Description => "自动更新你的插件！";

    private const string ReleaseUrl = "https://github.com/Controllerdestiny/TShockPlugin/releases/download/V1.0.0.0/Plugins.zip";

    private const string PUrl = "https://github.moeyy.xyz/";

    private const string PluginsUrl = "https://raw.githubusercontent.com/Controllerdestiny/TShockPlugin/master/Plugins.json";

    private static readonly HttpClient _httpClient = new();

    private const string TempSaveDir = "TempFile";

    private const string TempZipName = "Plugins.zip";

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
        try
        {
            var updates = GetUpdate();
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
            args.Player.SendSuccessMessage("[更新完成]\n" + string.Join("\n", updates.Select(i => $"[{i.Name}] V{i.OldVersion} >>> V{i.NewVersion}")));
            args.Player.SendSuccessMessage("重启服务器后插件生效!");
        }
        catch (Exception ex)
        {
            args.Player.SendErrorMessage("自动更新出现错误:" + ex.Message);
            return;
        }
    }

    private void CheckCmd(CommandArgs args)
    {
        try
        {
            var updates = GetUpdate();
            if (updates.Count == 0)
            {
                args.Player.SendSuccessMessage("你的插件全是最新版本，无需更新哦~");
                return;
            }
            args.Player.SendInfoMessage("[以下插件有新的版本更新]\n" + string.Join("\n", updates.Select(i => $"[{i.Name}] V{i.OldVersion} >>> V{i.NewVersion}")));
        }
        catch (Exception ex)
        {
            args.Player.SendErrorMessage("无法获取更新:" + ex.Message);
            return;
        }
    }

    #region 工具方法
    private static List<PluginUpdateInfo> GetUpdate()
    {
        var plugins = GetPlugins();
        HttpClient httpClient = new();
        var response = httpClient.GetAsync(PUrl + PluginsUrl).Result;

        if (!response.IsSuccessStatusCode)
            throw new Exception("无法连接服务器");
        var json = response.Content.ReadAsStringAsync().Result;
        var latestPluginList = JsonConvert.DeserializeObject<List<PluginVersionInfo>>(json) ?? new();
        List<PluginUpdateInfo> pluginUpdateList = new();
        foreach (var latestPluginInfo in latestPluginList)
            foreach (var plugin in plugins)
                if (plugin.Name == latestPluginInfo.Name && plugin.Version != latestPluginInfo.Version)
                    pluginUpdateList.Add(new PluginUpdateInfo(plugin.Name, plugin.Author, latestPluginInfo.Version, plugin.Version, plugin.Path, latestPluginInfo.Path));
        return pluginUpdateList;
    }

    private static List<PluginVersionInfo> GetPlugins()
    {
        List<PluginVersionInfo> plugins = new();
        //获取已安装的插件，并且读取插件信息和AssemblyName
        foreach (var plugin in ServerApi.Plugins) 
        {
            plugins.Add(new PluginVersionInfo()
            {
                AssemblyName = plugin.Plugin.GetType().Assembly.GetName().Name!,
                Author = plugin.Plugin.Author,
                Name = plugin.Plugin.Name,
                Description = plugin.Plugin.Description,
                Version = plugin.Plugin.Version.ToString()
            });
        }
        //从插件文件夹中读取插件路径(通过对比AssemblyName)
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
                for (int i = 0; i < plugins.Count; i++)
                {
                    if (plugins[i].AssemblyName == assembly.GetName().Name)
                    {
                        plugins[i].Path = fileInfo.Name!;
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
        DirectoryInfo directoryInfo = new(TempSaveDir);
        if (!directoryInfo.Exists)
            directoryInfo.Create();
        HttpClient httpClient = new();
        var zipBytes = httpClient.GetByteArrayAsync(PUrl + ReleaseUrl).Result;
        File.WriteAllBytes(Path.Combine(directoryInfo.FullName, TempZipName), zipBytes);
    }

    private static void ExtractDirectoryZip()
    {
        DirectoryInfo directoryInfo = new(TempSaveDir);
        ZipFile.ExtractToDirectory(Path.Combine(directoryInfo.FullName, TempZipName), Path.Combine(directoryInfo.FullName, "Plugins"), true);
    }

    private static void UpdatePlugin(List<PluginUpdateInfo> pluginUpdateInfos)
    {
        foreach (var pluginUpdateInfo in pluginUpdateInfos)
        {
            string sourcePath = Path.Combine(TempSaveDir, "Plugins", pluginUpdateInfo.RemotePath);
            string destinationPath = Path.Combine(ServerApi.ServerPluginsDirectoryPath, pluginUpdateInfo.LocalPath);
            // 确保目标目录存在
            string destinationDirectory = Path.GetDirectoryName(destinationPath)!;
            // 复制并覆盖文件
            File.Copy(sourcePath, destinationPath, true);
        }
        if (Directory.Exists(TempSaveDir))
            Directory.Delete(TempSaveDir, true);
    }
    #endregion
}
