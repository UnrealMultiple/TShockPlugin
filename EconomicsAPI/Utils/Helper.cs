using EconomicsAPI.Attributes;
using EconomicsAPI.EventArgs.PlayerEventArgs;
using EconomicsAPI.Extensions;
using Microsoft.Xna.Framework;
using Rests;
using System.Reflection;
using System.Text;
using TerrariaApi.Server;
using TShockAPI;

namespace EconomicsAPI.Utils;

public class Helper
{
    //代码来自于CaiLib
    public static string GetGradientText(string text)
    {
        string result = "";
        var info = Terraria.UI.Chat.ChatManager.ParseMessage(text, Color.White);
        var colors = Economics.Setting.GradientColor;
        foreach (var item in info)
        {
            var index = 0;
            for(int i = 0; i< item.Text.Length; i++)
            {
                if (index >= colors.Count)
                {
                    result += item.Text[i];
                    index = 0;
                }
                else
                {
                    result += Economics.Setting.GradientColor[index].SFormat(item.Text[i]);
                }
                index++;
            }
        }
        return result;
    }
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

    public static void CountertopUpdate(PlayerCountertopArgs args)
    {
        StringBuilder sb = new();
        string down = new('\n', Economics.Setting.StatusTextShiftDown);
        string Left = new(' ', Economics.Setting.StatusTextShiftLeft);
        sb.AppendLine(down);
        args.Messages.OrderBy(x => x.Order).ForEach(x => sb.AppendLine(GetGradientText(x.Message) + Left));
        args.Player?.SendData(PacketTypes.Status, sb.ToString(), 0, 1);
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
