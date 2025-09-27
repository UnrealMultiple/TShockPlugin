using TerrariaApi.Server;

namespace LazyAPI.Hooks;

public sealed class ServerApiHook<ArgsType> : HookBase where ArgsType : EventArgs
{
    public TerrariaPlugin Plugin { get; private set; }
    public HandlerCollection<ArgsType> Collection { get; private set; }
    public HookHandler<ArgsType> Handler { get; private set; }
    public ServerApiHook(TerrariaPlugin plugin, HandlerCollection<ArgsType> collection, HookHandler<ArgsType> handler, HookLoadType loadType = HookLoadType.Auto) : base(loadType)
    {
        this.Plugin = plugin;
        this.Collection = collection;
        this.Handler = handler;
    }
    protected override void DoRegister()
    {
        this.Collection.Register(this.Plugin, this.Handler);
    }

    protected override void DoUnregister()
    {
        this.Collection.Deregister(this.Plugin, this.Handler);
    }
}
