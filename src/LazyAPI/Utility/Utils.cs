using Rests;
using System.Reflection;
using System.Reflection.Emit;
using Terraria;

namespace LazyAPI.Utility;

public static class Utils
{
    private readonly static Func<Rest, List<RestCommand>> getRestCommandsFunc;
    static Utils()
    {
        var method = new DynamicMethod("get_commands", typeof(List<RestCommand>), new Type[] { typeof(Rest) });
        var il = method.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldfld, typeof(Rest).GetField("commands", BindingFlags.NonPublic | BindingFlags.Instance)!);
        il.Emit(OpCodes.Ret);
        getRestCommandsFunc = method.CreateDelegate<Func<Rest, List<RestCommand>>>();
    }
    public static Func<T> Eval<T>(string expression)
    {
        var idx = expression.LastIndexOf('.');
        var @class = expression[..idx];
        var name = expression[(idx + 1)..];
        var field = typeof(Main).Assembly.GetType(@class)!.GetField(name, BindingFlags.Static | BindingFlags.Public)!;
        return () => (T) field.GetValue(null)!;
    }

    public static Func<bool> Eval(IEnumerable<string> include, IEnumerable<string> exclude)
    {
        var exps1 = include.Select(e => Eval<bool>(e));
        var exps2 = exclude.Select(e => Eval<bool>(e));
        return () => exps1.All(e => e()) && exps2.All(e => !e());
    }

    public static bool Unregister(Rest rest, RestCommand command)
    {
        return getRestCommandsFunc(rest).Remove(command);
    }
    internal static Tuple<int, int>? FindItemRef(object item)
    {
        for (var i = 0; i < Main.maxPlayers; ++i)
        {
            var plr = Main.player[i];
            if (plr?.inventory == null)
            {
                continue;
            }

            var n = plr.inventory.Length;
            for (var j = 0; j < n; ++j)
            {
                if (plr.inventory[j] == item)
                {
                    return new Tuple<int, int>(i, j);
                }
            }
        }
        return null;
    }
}