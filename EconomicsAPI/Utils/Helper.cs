using EconomicsAPI.Attributes;
using EconomicsAPI.Extensions;
using Rests;
using System.Reflection;
using TerrariaApi.Server;
using TShockAPI;

namespace EconomicsAPI.Utils;

public class Helper
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T">特性</typeparam>
    /// <param name="assembly">程序集</param>
    /// <param name="result">返回值类型</param>
    /// <param name="paramType">参数类型</param>
    /// <returns></returns>
    public static Dictionary<MethodInfo, (object, T)> MatchAssemblyMethodByAttribute<T>(Assembly assembly, Type result, params Type[] paramType) where T : Attribute
    {
        var methods = new Dictionary<MethodInfo, (object, T)>();
        Dictionary<Type, object> types = new();
        assembly.GetTypes().ForEach(x =>
        {
            if (!x.IsAbstract && !x.IsInterface)
            {
                var flag = BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public;
                x.GetMethods(flag).ForEach(m =>
                {
                    if (m.ParamsMatch(paramType) && m.ResultMatch(result))
                    {
                        var attribute = m.GetCustomAttribute<T>();
                        if (attribute != null)
                        {
                            if (!m.IsStatic)
                            {
                                var instance = types.TryGetValue(x, out var obj) && obj != null ? obj : Activator.CreateInstance(x);
                                types[x] = instance;
                                var method = instance?.GetType().GetMethod(m.Name, flag);
                                if (method != null)
                                {
                                    methods.Add(method, (instance, attribute));
                                }
                            }
                            else
                            {
                                methods.Add(m, (null, attribute));
                            }
                        }
                    }
                });
            }
        });
        return methods;
    }

    internal static void InitPluginAttributes()
    {
        var obj = typeof(ServerApi).GetField("loadedAssemblies", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null);
        if (obj is Dictionary<string, Assembly> assemblys)
        {
            foreach (var assembly in assemblys.Values)
            {
                MapingCommand(assembly);
                MapingRest(assembly);
            }
        }
    }

    /// <summary>
    /// 加载指令
    /// </summary>
    internal static void MapingCommand(Assembly assembly)
    {
        var methods = MatchAssemblyMethodByAttribute<CommandMap>(assembly, typeof(void), typeof(CommandArgs));
        foreach (var (method, tuple) in methods)
        {
            (object Instance, CommandMap attr) = tuple;
            Commands.ChatCommands.Add(new(attr.Permission, method.CreateDelegate<CommandDelegate>(Instance), attr.Name));
        }
    }

    /// <summary>
    /// 加载rest API
    /// </summary>
    internal static void MapingRest(Assembly assembly)
    {
        var methods = MatchAssemblyMethodByAttribute<RestMap>(assembly, typeof(object), typeof(RestRequestArgs));
        foreach (var (method, tuple) in methods)
        {
            (object Instance, RestMap attr) = tuple;
            TShock.RestApi.Register(new(attr.ApiPath, method.CreateDelegate<RestCommandD>(Instance)));
        }
    }
}
