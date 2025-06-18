using MonoMod.RuntimeDetour;
using System.Reflection;

namespace ServerTools;
public class HookManager
{
    private static readonly Dictionary<MethodBase, Hook> _hooks = [];

    public static void Add(MethodInfo method, Delegate to)
    {
        _hooks.Add(method, new Hook(method, to));
    }

    public static void Add(ConstructorInfo method, Delegate to)
    {
        _hooks.Add(method, new Hook(method, to));
    }

    public static void Remove(MethodInfo method)
    {
        if (_hooks.TryGetValue(method, out var hook))
        {
            hook.Dispose();
            _hooks.Remove(method);
        }
    }

    public static void Remove(ConstructorInfo method)
    {
        if (_hooks.TryGetValue(method, out var hook))
        {
            hook.Dispose();
            _hooks.Remove(method);
        }
    }

    public static void Clear()
    {
        foreach (var hook in _hooks.Values)
        {
            hook.Dispose();
        }
        _hooks.Clear();
    }
}
