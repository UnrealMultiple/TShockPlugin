using LazyAPI.Utility;
using Terraria;
using TerrariaApi.Server;

namespace LazyAPI;

[ApiVersion(2, 1)]
// ReSharper disable once UnusedType.Global
public class PluginContainer(Main game) : LazyPlugin(game)
{
    public override string Author => "cc004 & members of UnrealMultiple";

    public override Version Version => new Version(1, 0, 3, 0);

    public override void Initialize()
    {
        ServerApi.Hooks.GamePostUpdate.Register(this, PostUpdate);
    }

    private static void PostUpdate(EventArgs _)
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