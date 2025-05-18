using Terraria;
using TerrariaApi.Server;

namespace TransferPatch;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    public override string Author => "少司命";

    public override string Description => GetString("翻译任意插件");

    public override string Name => "TransferPatch";

    public override Version Version => new Version(1, 0, 0, 1);

    public Plugin(Main game) : base(game)
    {
        this.Order = int.MinValue;
    }

    public override void Initialize()
    {
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

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }
}
