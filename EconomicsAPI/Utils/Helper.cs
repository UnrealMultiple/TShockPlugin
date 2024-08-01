using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using EconomicsAPI.Attributes;
using EconomicsAPI.EventArgs.PlayerEventArgs;
using EconomicsAPI.Extensions;
using Microsoft.Xna.Framework;
using Rests;
using TerrariaApi.Server;
using TShockAPI;

namespace EconomicsAPI.Utils;

public class Helper
{
    private static readonly Regex Regex = new(@"\[(?<type>[^\]]+):(?<id>\d+)\]");
    public static string GetGradientText(string text)
    {
        string result = "";
        //匹配物品消息
        var matchs = Regex.Matches(text);
        var chat = matchs.Select(x => x.Groups).ToDictionary(x => x[1].Index, x => x);
        var info = Terraria.UI.Chat.ChatManager.ParseMessage(text, Color.White);
        var colors = Economics.Setting.GradientColor;
        var fullIndex = 1;
        var index = 0;
        foreach (var item in info)
        {
            for (int i = 0; i < item.Text.Length; i++)
            {
                fullIndex++;
                if (chat.TryGetValue(fullIndex - 1, out var group) && group != null)
                {
                    result += item.TextOriginal;
                    fullIndex += item.Text.Length + 1;
                    break;
                }
                else
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
    /// <typeparam name="T">特性类型</typeparam>
    /// <param name="assembly">程序集实例</param>
    /// <param name="result">返回参数类型</param>
    /// <param name="paramType">参数类型</param>
    /// <returns></returns>
    public static Dictionary<MethodInfo, (object?, T)> MatchAssemblyMethodByAttribute<T>(Assembly assembly, Type result, params Type[] paramType) where T : Attribute
    {
        var Modules = new Dictionary<MethodInfo, (object?, T)>();
        var flag = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
        Dictionary<Type, MethodInfo[]> mapping = assembly.GetExportedTypes()
            .Where(x => x.IsDefined(typeof(RegisterSeries)))
            .Select(type => (type, type.GetMethods(flag)
            .Where(m => m.IsDefined(typeof(T)) && m.ParamsMatch(paramType))
            .ToArray()))
            .ToDictionary(method => method.type, method => method.Item2);
        foreach (var (cls, methods) in mapping)
        {
            var instance = Activator.CreateInstance(cls);
            if (instance == null)
                continue;
            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute<T>()!;
                if (method.IsStatic)
                {
                    Modules[method] = (null, attr);
                    continue;
                }
                var _method = instance.GetType().GetMethod(method.Name, flag)!;
                Modules[method] = (instance, attr);
            }
        }
        return Modules;
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
            (object? Instance, CommandMap attr) = tuple;
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
            (object? Instance, RestMap attr) = tuple;
            TShock.RestApi.Register(new(attr.ApiPath, method.CreateDelegate<RestCommandD>(Instance)));
        }
    }
}
