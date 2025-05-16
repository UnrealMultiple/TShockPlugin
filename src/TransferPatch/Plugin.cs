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
        this.Order = int.MinValue;
    }

    public override void Initialize()
    {
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
        t.SetProperty += this.SetProperty;
        t.SetField += this.SetField;
        t.Patch(Config.Instance.TargetClassName);
        t.SetProperty -= this.SetProperty;
        t.SetField -= this.SetField;
    }

    private string? SetField(Mono.Cecil.FieldDefinition arg, string className)
    {
        return Config.Instance.Transfers.GetValueOrDefault($"{className}.{arg.Name}");
    }

    private string? SetProperty(Mono.Cecil.PropertyDefinition arg, string className)
    {
        return Config.Instance.Transfers.GetValueOrDefault($"{className}.{arg.Name}");
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }
}
