using Economics.Plugin.Scripting;
using Terraria;
using TerrariaApi.Server;
using TShockAPI.Hooks;

namespace Economics.Plugin;

[ApiVersion(2, 1)]
public class EconomicsPlugin(Main game) : TerrariaPlugin(game)
{
    private ScriptHost _scripts = null!;

    public override string Name => "Economics.Plugin";
    public override string Author => "少司命";
    public override string Description => "JavaScript 脚本插件实现";
    public override Version Version => new(1, 0, 0, 0);

    public override void Initialize()
    {
        Directory.CreateDirectory(ScriptHost.ScriptsDir);
        this._scripts = new ScriptHost(this);
        this._scripts.LoadAll();
        GeneralHooks.ReloadEvent += this.OnReload;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GeneralHooks.ReloadEvent -= this.OnReload;
            this._scripts.Dispose();
        }
        base.Dispose(disposing);
    }

    private void OnReload(ReloadEventArgs args)
    {
        this._scripts.Reload();
    }
}
