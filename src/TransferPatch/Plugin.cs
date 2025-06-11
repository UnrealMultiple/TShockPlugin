using Mono.Cecil;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace TransferPatch;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    public override string Author => "少司命";

    public override string Description => GetString("翻译任意插件");

    public override string Name => "TransferPatch";

    public override Version Version => new Version(1, 0, 0, 2);

    public Plugin(Main game) : base(game)
    {
        this.Order = int.MinValue;
    }

    public override void Initialize()
    {
        Commands.ChatCommands.Add(new Command("transferpatch.dump", this.DumpConfig, "dump-config"));
        if (!Config.Instance.Enable)
        {
            return;
        }
        foreach (var target in Config.Instance.TransferTargets)
        {
            using var stream = new MemoryStream(File.ReadAllBytes(target.TargetAssembly));
            var t = new ILTranslate(stream, target.TargetAssembly);
            t.SetMember += (member, className) => target.Transfers.GetValueOrDefault($"{className}.{member.Name}");
            t.Patch(target.TargetClassName);
            t.Dispose();
        }
    }

    private void DumpConfig(CommandArgs args)
    {
        if(args.Parameters.Count < 2)
        {
            args.Player.SendErrorMessage(GetString("用法: /dump-config <pluginName> <class>"));
            return;
        }
        var assemblyName = args.Parameters[0];
        var className = args.Parameters[1];
        foreach (var plugin in ServerApi.Plugins)
        { 
            if(plugin.Plugin.GetType().Assembly.GetName().Name == assemblyName)
            {
                using var dumper = new Dumper($"{className}.dump");
                var type = plugin.Plugin.GetType().Assembly.GetType(className) ?? throw new NullReferenceException(GetString("无法获取类型"));
                dumper.DumpType(type);
                args.Player.SendSuccessMessage(GetString("已导出类型 {0} 的字段和属性到文件 {1}.dump", className, assemblyName));
                return;
            }
        }
        args.Player.SendErrorMessage(GetString("未找到插件 {0}", assemblyName));
    }
    
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }
}
