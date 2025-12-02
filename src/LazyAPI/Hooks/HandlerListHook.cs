using TShockAPI;

namespace LazyAPI.Hooks;

public sealed class HandlerListHook<T> : HookBase where T : EventArgs
{
    public HandlerList<T> HandlerList { get; private set; }
    public HandlerList<T>.HandlerItem HandlerItem { get; private set; }
    public HandlerListHook(HandlerList<T> handlerList, EventHandler<T> eventHandler, HandlerPriority priority = HandlerPriority.Normal, bool gethandled = false, HookLoadType loadType = HookLoadType.Auto) : base(loadType)
    {
        this.HandlerList = handlerList;
        this.HandlerItem = HandlerList<T>.Create(eventHandler, priority, gethandled);
    }
    protected override void DoRegister()
    {
        this.HandlerList.Register(this.HandlerItem);
    }

    protected override void DoUnregister()
    {
        this.HandlerList.UnRegister(this.HandlerItem.Handler);
    }
}
