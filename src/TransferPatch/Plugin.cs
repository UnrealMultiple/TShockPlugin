using Terraria;
using TerrariaApi.Server;

namespace TransferPatch;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    public override string Author => "少司命";

    public override string Description => GetString("翻译任意插件");

    public override string Name => "TransferPatch";

    public override Version Version => new Version(1, 0, 0, 0);

    public Plugin(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        TShockAPI.Hooks.GeneralHooks.ReloadEvent += Config.OnReload;
        this.Run();
    }

    private void Run()
    {
        if (!Config.Instance.Enable || !File.Exists(Config.Instance.TargetAssembly))
        {
            return;
        }
        using var stream = new MemoryStream(File.ReadAllBytes(Config.Instance.TargetAssembly));
        var t = new ILTranslate(stream, Config.Instance.TargetAssembly);
        t.SetProperty += this.T_SetProperty;
        t.SetField += this.T_SetField;
        t.Patch(Config.Instance.TargetClassName);
        t.SetProperty -= this.T_SetProperty;
        t.SetField -= this.T_SetField;
    }

    private string T_SetField(Mono.Cecil.FieldDefinition arg, string className)
    {
        TShockAPI.TShock.Log.Debug(GetString($"[翻译]:{arg.FullName}..."));
        return Config.Instance.Transfers.GetValueOrDefault($"{className}.{arg.Name}", arg.Name);
    }

    private string T_SetProperty(Mono.Cecil.PropertyDefinition arg, string className)
    {
        TShockAPI.TShock.Log.Debug(GetString($"[翻译]:{arg.FullName}..."));
        return Config.Instance.Transfers.GetValueOrDefault($"{className}.{arg.Name}", arg.Name);
    }

    protected override void Dispose(bool disposing)
    {
        TShockAPI.Hooks.GeneralHooks.ReloadEvent -= Config.OnReload;
        base.Dispose(disposing);
    }
}
