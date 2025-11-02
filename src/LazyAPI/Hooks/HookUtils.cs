using TerrariaApi.Server;
using TShockAPI;

namespace LazyAPI.Hooks;

public static class HookUtils
{
    public static ServerApiHook<ArgsType> GetHook<ArgsType>(this HandlerCollection<ArgsType> handlerCollection, TerrariaPlugin hookPlugin, HookHandler<ArgsType> hookHandler, HookLoadType loadType = HookLoadType.Auto) where ArgsType : EventArgs
    {
        return new ServerApiHook<ArgsType>(hookPlugin, handlerCollection, hookHandler, loadType);
    }

    public static HandlerListHook<ArgsType> GetHook<ArgsType>(this HandlerList<ArgsType> handlerList, EventHandler<ArgsType> hookHandler, HandlerPriority priority = HandlerPriority.Normal, bool gethandled = false, HookLoadType loadType = HookLoadType.Auto) where ArgsType : EventArgs
    {
        return new HandlerListHook<ArgsType>(handlerList, hookHandler, priority, gethandled, loadType);
    }
}
