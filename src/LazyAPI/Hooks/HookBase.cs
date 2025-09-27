namespace LazyAPI.Hooks;

public abstract class HookBase
{
    public bool Registered { get; protected set; }
    public HookLoadType LoadType { get; }
    protected HookBase(HookLoadType loadType)
    {
        this.Registered = false;
    }
    public bool Register()
    {
        if (!this.Registered)
        {
            this.DoRegister();
            this.Registered = true;
            return true;
        }
        return false;
    }
    protected abstract void DoRegister();
    public bool Unregister()
    {
        if (this.Registered)
        {
            this.DoUnregister();
            this.Registered = false;
            return true;
        }
        return false;
    }
    protected abstract void DoUnregister();
}
