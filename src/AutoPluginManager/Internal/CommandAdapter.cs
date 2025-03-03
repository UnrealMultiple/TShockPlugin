using System.Reflection;
using TerrariaApi.Server;
using TShockAPI;

namespace AutoPluginManager.Internal;
internal class CommandAdapter
{
    private static readonly Dictionary<string, CommandDelegate> _actions = new Dictionary<string, CommandDelegate>()
    {
        { "c", CheckUpdate },
        { "u", Update },
        { "l", CloudPlugins },
        { "b", SkipUpdate },
        { "r", Repeat },
        { "i", InstallPlugin },
        { "rb", RemoveSkipPlugin },
        { "lb", SkipUpdateList },
        { "on", EnablePlugin },
        { "off", OffPlugin },
        { "il", LocalPlugins },
    };

    public static void Adapter(CommandArgs args)
    {
        if (args.Parameters.Count >= 1)
        {
            var text = args.Parameters[0].ToLower();
            var subcmd = text.StartsWith("-") ? text.TrimStart('-') : text;
            if (_actions.TryGetValue(subcmd, out var action))
            {
                action(args);
                return;
            }
        }
        args.Player.SendInfoMessage(GetString("apm c 检测已安装插件更新"));
        args.Player.SendInfoMessage(GetString("apm u [插件名] 更新所有插件或指定插件"));
        args.Player.SendInfoMessage(GetString("apm i [序号] 安装指定插件"));
        args.Player.SendInfoMessage(GetString("apm l 查看可安装插件表"));
        args.Player.SendInfoMessage(GetString("apm b [插件名字] 更新时跳过指定插件"));
        args.Player.SendInfoMessage(GetString("apm rb [插件名字] 取消更新排除"));
        args.Player.SendInfoMessage(GetString("apm lb 查看更新排除列表"));
        args.Player.SendInfoMessage(GetString("apm ib 查看本地插件列表与启用状态"));
        args.Player.SendInfoMessage(GetString("apm on 启用某个插件"));
        args.Player.SendInfoMessage(GetString("apm off 关闭某个插件"));
    }

    /// <summary>
    /// 本地已安装插件
    /// </summary>
    /// <param name="args"></param>
    /// <exception cref="NotImplementedException"></exception>
    private static void LocalPlugins(CommandArgs args)
    {
        var i = 1;
        var plugins = ((List<PluginContainer>) typeof(ServerApi)
            .GetField("plugins", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)?
            .GetValue(null)!).ToDictionary(i => i.Plugin.GetType().Assembly.GetName().Name!);
        foreach (var (assemblyName, plugin) in PluginManagementContext.Instance.LocalPluginManifests)
        {
            if (!PluginManagementContext.Instance.ClouldPluginManifests.ContainsKey(assemblyName))
            {
                args.Player.SendInfoMessage(GetString($"{i}.{plugin.Name} v{plugin.Version} (by {plugin.Author}) {(PluginManagementContext.Instance.ClouldPluginManifests.ContainsKey(assemblyName) ? "" : "- 此插件无法被开启或关闭!")}"));
            }
            else
            {
                args.Player.SendInfoMessage(GetString($"{i}.{plugin.Name} v{plugin.Version} (by {plugin.Author}) - 状态: {(plugins[assemblyName].Initialized ? "开启" : "关闭")}"));
            }
            i++;
        }
    }

    private static void OffPlugin(CommandArgs args)
    {
        if (args.Parameters.Count != 2)
        {
            args.Player.SendInfoMessage(GetString("语法错误，正确语法:/apm off [序号]"));
            return;
        }
        var indices = args.Parameters[1]
                .Split(",")
                .Select(x => int.TryParse(x, out var index) ? index : -1)
                .ToArray();

        var availablePlugins = PluginManagementContext.Instance.LocalPluginManifests.Values.ToArray();
        var cloudAssemblys = PluginManagementContext.Instance.ClouldPluginManifests.Keys.ToArray();
        var pendingPlugins = indices
            .Where(i => i > 0 && i <= availablePlugins.Length && cloudAssemblys.Contains(availablePlugins[i - 1].AssemblyName))
            .Select(i => availablePlugins[i - 1])
            .ToList();
        if (!pendingPlugins.Any())
        {
            args.Player.SendErrorMessage(GetString("序号无效，请附带需要关闭插件的选择项!"));
            return;
        }

        var plugins = (List<PluginContainer>) typeof(ServerApi)
            .GetField("plugins", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)?
            .GetValue(null)!;
        foreach (var plugin in plugins)
        {
            if (pendingPlugins.Any(i => i.AssemblyName == plugin.Plugin.GetType().Assembly.GetName().Name))
            {
                if (plugin.Initialized)
                {
                    plugin.Dispose();
                    plugin.DeInitialize();
                    args.Player.SendSuccessMessage(GetString($"{plugin.Plugin.Name} v{plugin.Plugin.Version} (by {plugin.Plugin.Author}) 关闭成功!"));
                }
                else
                {
                    args.Player.SendErrorMessage(GetString($"{plugin.Plugin.Name} v{plugin.Plugin.Version} (by {plugin.Plugin.Author}) 被关闭过了，无法重复关闭!"));
                }
            }
        }
    }

    private static void EnablePlugin(CommandArgs args)
    {
        if (args.Parameters.Count != 2)
        {
            args.Player.SendInfoMessage(GetString("语法错误，正确语法:/apm on [序号]"));
            return;
        }
        var indices = args.Parameters[1]
                .Split(",")
                .Select(x => int.TryParse(x, out var index) ? index : -1)
                .ToArray();

        var availablePlugins = PluginManagementContext.Instance.LocalPluginManifests.Values.ToArray();
        var cloudAssemblys = PluginManagementContext.Instance.ClouldPluginManifests.Keys.ToArray();
        var pendingPlugins = indices
            .Where(i => i > 0 && i <= availablePlugins.Length && cloudAssemblys.Contains(availablePlugins[i - 1].AssemblyName))
            .Select(i => availablePlugins[i - 1])
            .ToList();
        if (!pendingPlugins.Any())
        {
            args.Player.SendErrorMessage(GetString("序号无效，请附带需要开启插件的选择项!"));
            return;
        }

        var plugins = (List<PluginContainer>) typeof(ServerApi)
            .GetField("plugins", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)?
            .GetValue(null)!;
        foreach (var plugin in plugins)
        {
            if (pendingPlugins.Any(i => i.AssemblyName == plugin.Plugin.GetType().Assembly.GetName().Name))
            {
                if (!plugin.Initialized)
                {
                    plugin.Initialize();
                    args.Player.SendSuccessMessage(GetString($"{plugin.Plugin.Name} v{plugin.Plugin.Version} (by {plugin.Plugin.Author}) 启用成功!"));
                }
                else
                {
                    args.Player.SendErrorMessage(GetString($"{plugin.Plugin.Name} v{plugin.Plugin.Version} (by {plugin.Plugin.Author}) 已开启，无法重复开启!"));
                }
            }
        }
    }

    /// <summary>
    /// 下载
    /// </summary>
    /// <param name="args"></param>
    private static void InstallPlugin(CommandArgs args)
    {
        if (args.Parameters.Count != 2)
        {
            args.Player.SendInfoMessage(GetString("语法错误，正确语法:/apm i [序号]"));
            return;
        }
        var indices = args.Parameters[1]
                .Split(",")
                .Select(x => int.TryParse(x, out var index) ? index : -1)
                .ToArray();

        if (!indices.Any())
        {
            args.Player.SendErrorMessage(GetString("无效参数，请附带需要安装插件的选择项!"));
            return;
        }

        try
        {
            var availablePlugins = PluginManagementContext.Instance.ClouldPluginManifests.Values.ToArray();
            var pendingPlugins = indices
                .Where(i => i > 0 && i <= availablePlugins.Length)
                .Select(i => availablePlugins[i - 1])
                .ToList();
            if (!pendingPlugins.Any())
            {
                args.Player.SendErrorMessage(GetString("序号无效，请附带需要安装插件的选择项!"));
                return;
            }

            var success = PluginManagementContext.Instance.InstallOrUpdatePlugins(pendingPlugins.Select(x => x.AssemblyName));

            if (!success.plugins.Any())
            {
                args.Player.SendSuccessMessage(GetString("安装了个寂寞~"));
                return;
            }
            // A bit weird, might be refactored in the next version
            // FIXME: inconsistency in return values of `Utils.UnLoadPlugins` and `Utils.LoadPlugins`
            var failedUnload = new List<string>(); // AssemblyName of Plugins which failed to unload
            var failedLoad = new List<string>(); // Type.FullName of Plugin Classes which failed to load
            if (Config.Instance.HotReloadPlugin)
            {

                failedUnload = Utils.UnLoadPlugins(success.plugins
                    .Where(s => s.Current is not null && s.Latest.HotReload)
                    .Select(s => s.Current!.FileName));
                failedLoad = Utils.LoadPlugins(success.plugins
                    .Where(s => s.Latest.HotReload && !failedUnload.Contains(s.Latest.AssemblyName))
                    .Select(s => s.Current?.FileName ?? s.Latest.FileName));
            }
            args.Player.SendFormattedServerPluginsModifications(success);

            if (Config.Instance.HotReloadPlugin)
            {
                if (failedUnload.Any())
                {
                    args.Player.SendWarningMessage(GetString($"*卸载失败: {string.Join(',', failedUnload)}"));
                }
                if (failedLoad.Any())
                {
                    args.Player.SendWarningMessage(GetString($"*加载失败: {string.Join(',', failedLoad)}"));
                }
                if (failedLoad.Any() || failedLoad.Any())
                {
                    args.Player.SendWarningMessage(GetString("*热加载失败的插件需要重启服务器后才会生效!"));
                }
                else
                {
                    args.Player.SendSuccessMessage(GetString("*热重载已启用,安装的插件已生效!"));
                }

            }
            else
            {
                args.Player.SendSuccessMessage(GetString("*热重载已关闭,插件需要重启服务器后才会生效!"));
            }


        }
        catch (Exception ex)
        {
            args.Player.SendErrorMessage(GetString("安装插件出现错误:") + ex);
        }
    }

    /// <summary>
    /// 排除更新列表
    /// </summary>
    /// <param name="args"></param>
    private static void SkipUpdateList(CommandArgs args)
    {
        if (!Config.Instance.UpdateBlackList.Any())
        {
            args.Player.SendSuccessMessage(GetString("当前没有排除任何一个插件哦~"));
            return;
        }

        args.Player.SendErrorMessage(GetString("插件更新排除列表:\n") + string.Join('\n', Config.Instance.UpdateBlackList));
    }

    /// <summary>
    /// 移除排除更新插件
    /// </summary>
    /// <param name="args"></param>
    private static void RemoveSkipPlugin(CommandArgs args)
    {
        if (args.Parameters.Count != 2)
        {
            args.Player.SendInfoMessage(GetString("语法错误，正确语法:/apm rb [插件名称]"));
            return;
        }
        if (!Config.Instance.UpdateBlackList.Contains(args.Parameters[1]))
        {
            args.Player.SendErrorMessage(GetString("删除失败, 没有在你的插件列表里找到这个插件呢~"));
            return;
        }

        Config.Instance.UpdateBlackList.Remove(args.Parameters[1]);
        Config.Instance.Write();
        args.Player.SendSuccessMessage(GetString("删除成功, 此插件将会被检查更新~"));
    }

    /// <summary>
    /// 重复安装
    /// </summary>
    /// <param name="args"></param>
    private static void Repeat(CommandArgs args)
    {
        var duplicates = Utils.CheckDuplicatePlugins();
        if (duplicates.Any())
        {
            args.Player.SendErrorMessage(GetString("[插件重复安装]") + string.Join(" >>> ", duplicates.Select(x => x.Key + ".dll")));
        }
    }

    /// <summary>
    /// 跳过插件更新
    /// </summary>
    /// <param name="args"></param>
    private static void SkipUpdate(CommandArgs args)
    {
        if (args.Parameters.Count != 2)
        {
            args.Player.SendInfoMessage(GetString("语法错误，正确语法:/apm b [插件名称]"));
            return;
        }
        var plugins = PluginManagementContext.Instance.LocalPluginManifests.Values;
        if (plugins.All(p => p.Name != args.Parameters[1]))
        {
            args.Player.SendErrorMessage(GetString("排除失败, 没有在你的插件列表里找到这个插件呢~"));
            return;
        }

        if (Config.Instance.UpdateBlackList.Contains(args.Parameters[1]))
        {
            args.Player.SendErrorMessage(GetString("排除失败, 已经排除过这个插件了呢~"));
            return;
        }

        Config.Instance.UpdateBlackList.Add(args.Parameters[1]);
        Config.Instance.Write();
        args.Player.SendSuccessMessage(GetString("排除成功, 已跳过此插件的更新检查~"));
    }

    /// <summary>
    /// 插件列表
    /// </summary>
    /// <param name="args"></param>
    /// <exception cref="NotImplementedException"></exception>
    private static void CloudPlugins(CommandArgs args)
    {
        var manifest = PluginManagementContext.Instance.ClouldPluginManifests.Values.ToArray();
        args.Player.SendInfoMessage(GetString("可安装插件列表:"));
        for (var i = 0; i < manifest.Length; i++)
        {
            var cultureName = PluginManagementContext.Instance.CultureInfo.Name;
            if (!manifest[i].Description.ContainsKey(cultureName))
            {
                cultureName = "zh-CN";
            }
            args.Player.SendInfoMessage($"{i + 1}.{manifest[i].Name.Color("1E90FF")} v{manifest[i].Version} - {manifest[i].Description[cultureName].Color("32CD32")} (by {manifest[i].Author})");
        }
        args.Player.SendInfoMessage(GetString("*使用/apm -i <序号> 即可安装哦~"));
    }

    public static void UpdatePlugin(TSPlayer Player, params string[] targets)
    {
        try
        {
            var updates = PluginManagementContext.Instance.GetAvailableUpdates();
            if (!updates.Any())
            {
                Player.SendSuccessMessage(GetString("你的插件全是最新版本，无需更新哦~"));
                return;
            }

            if (targets.Any())
            {
                updates = updates
                    .Where(i => targets.Contains(i.Current!.Name))
                    .ToArray();
                if (!updates.Any())
                {
                    Player.SendErrorMessage($"{string.Join(",", targets)} 无需更新!");
                    return;
                }
            }

            var success = PluginManagementContext.Instance.InstallOrUpdatePlugins(updates.Select(x => x.Latest.AssemblyName));
            if (!success.plugins.Any())
            {
                Player.SendSuccessMessage(GetString("更新了个寂寞~"));
                return;
            }

            // A bit weird, might be refactored in the next version
            // FIXME: inconsistency in return values of `Utils.UnLoadPlugins` and `Utils.LoadPlugins`
            var failedUnload = new List<string>(); // AssemblyName of Plugins which failed to unload
            var failedLoad = new List<string>(); // Type.FullName of Plugin Classes which failed to load
            if (Config.Instance.HotReloadPlugin)
            {

                failedUnload = Utils.UnLoadPlugins(success.plugins
                    .Where(s => s.Current is not null && s.Latest.HotReload)
                    .Select(s => s.Current!.FileName));
                failedLoad = Utils.LoadPlugins(success.plugins
                    .Where(s => s.Latest.HotReload && !failedUnload.Contains(s.Latest.AssemblyName))
                    .Select(s => s.Current?.FileName ?? s.Latest.FileName));
            }
            Player.SendFormattedServerPluginsModifications(success);

            if (Config.Instance.HotReloadPlugin)
            {
                if (failedUnload.Any())
                {
                    Player.SendWarningMessage(GetString($"*卸载失败: {string.Join(',', failedUnload)}"));
                }
                if (failedLoad.Any())
                {
                    Player.SendWarningMessage(GetString($"*加载失败: {string.Join(',', failedLoad)}"));
                }
                if (failedLoad.Any() || failedLoad.Any())
                {
                    Player.SendWarningMessage(GetString("*热加载失败的插件需要重启服务器后才会生效!"));
                }
                else
                {
                    Player.SendSuccessMessage(GetString("*热重载已启用,安装的插件已生效!"));
                }

            }
            else
            {
                Player.SendSuccessMessage(GetString("*热重载已关闭,插件需要重启服务器后才会生效!"));
            }
        }
        catch (Exception ex)
        {
            Player.SendErrorMessage(GetString("自动更新出现错误:") + ex);
        }
    }
    /// <summary>
    /// 更新插件
    /// </summary>
    /// <param name="args"></param>
    private static void Update(CommandArgs args)
    {
        var targets = Array.Empty<string>();
        if (args.Parameters.Count > 1)
        {
            targets = args.Parameters[1].Split(",");
        }
        UpdatePlugin(args.Player, targets);
    }

    /// <summary>
    /// 检查插件更新
    /// </summary>
    /// <param name="args"></param>
    private static void CheckUpdate(CommandArgs args)
    {
        try
        {
            var updates = PluginManagementContext.Instance.GetAvailableUpdates();
            if (updates.Length == 0)
            {
                args.Player.SendSuccessMessage(GetString("你的插件全是最新版本，无需更新哦~"));
                return;
            }

            args.Player.SendInfoMessage(GetString("[以下插件有新的版本更新]\n") + string.Join("\n", updates.Select(i => $"[{i.Current?.Name ?? i.Latest.Name}] V{i.Current?.Version} >>> V{i.Latest.Version}")));
        }
        catch (Exception ex)
        {
            args.Player.SendErrorMessage(GetString("无法获取更新:") + ex);
        }
    }
}
