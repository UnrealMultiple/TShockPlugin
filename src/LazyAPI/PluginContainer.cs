using LazyAPI.Utility;
using Terraria;
using TerrariaApi.Server;

namespace LazyAPI;

[ApiVersion(2, 1)]
// ReSharper disable once UnusedType.Global
public class PluginContainer : LazyPlugin
{
    public override string Author => "cc004 & members of UnrealMultiple";

    public override Version Version => new Version(1, 0, 2, 1);

    public PluginContainer(Main game) : base(game) { }
    public override void Initialize()
    {
        ServerApi.Hooks.GamePostUpdate.Register(this, this.PostUpdate);
        base.Initialize();
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.GamePostUpdate.Deregister(this, this.PostUpdate);
        }
        base.Dispose(disposing);
    }
    private void PostUpdate(EventArgs _)
    {
        ++TimingUtils.Timer;

        while (TimingUtils.scheduled.TryPeek(out var action, out var time))
        {
            if (time > TimingUtils.Timer)
            {
                break;
            }
            action();
            TimingUtils.scheduled.Dequeue();
        }
    }
}