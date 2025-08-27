using LazyAPI.Attributes;
using System.Reflection;
using TShockAPI;

namespace LazyAPI.Commands;

internal static class CommandHelper
{
    private static IEnumerable<string> GetAlias(MemberInfo info)
    {
        var alias = info.GetCustomAttributes<AliasAttribute>().SelectMany(a => a.alias);
        var flag = false;

        foreach (var a in alias)
        {
            flag = true;
            yield return a;
        }

        if (flag)
        {
            yield break;
        }

        yield return info.Name.ToLower();
    }

    private static string AliasToString(string[] alias)
    {
        return alias.Length == 1 ? alias[0] + " " : $"({string.Join('|', alias)}) ";
    }

    private static Command BuildTree(Type type, string prefix)
    {
        var result = new Command(type, prefix);

        foreach (var t in type.GetNestedTypes(BindingFlags.Public | BindingFlags.Static))
        {
            var al = GetAlias(t).ToArray();
            var sub = BuildTree(t, prefix + AliasToString(al));
            foreach (var alias in al)
            {
                result.Add(alias, sub);
            }
        }

        foreach (var func in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
        {
            if (func.GetCustomAttribute<MainAttribute>() != null)
            {
                result.Add(null, new SingleCommand(func, prefix));
            }
            else
            {
                var al = GetAlias(func).ToArray();
                CommandBase sub = func.GetCustomAttribute<FlexibleAttribute>() != null ? new FlexibleCommand(func, prefix + AliasToString(al)) : new SingleCommand(func, prefix + AliasToString(al));
                foreach (var alias in al)
                {
                    result.Add(alias, sub);
                }
            }
        }

        result.PostBuildTree();

        return result;
    }

    private static void ParseCommand(Command tree, CommandArgs args)
    {
        var result = tree.TryParse(args, 0);
        if (result.unmatched == 0)
        {
            return;
        }

        args.Player.SendInfoMessage(GetString($"most match: {result.current}"));
        args.Player.SendInfoMessage(GetString("use subcommand `help` for more usage"));
    }

    private static IEnumerable<string> GetCommandAlias(MemberInfo info)
    {
        var alias = info.GetCustomAttributes<CommandAttribute>().SelectMany(a => a.alias);
        var flag = false;

        foreach (var a in alias)
        {
            flag = true;
            yield return a;
        }

        if (flag)
        {
            yield break;
        }

        yield return info.Name.ToLower();
    }
    
    private static string GetCommandHelpText(MemberInfo info)
    {
        return info.GetCustomAttributes<HelpTextAttribute>().Select(a => a.helpText).FirstOrDefault() ?? GetString("No help available.");
    }


    internal static string[] Register(Type type)
    {
        if (!(type.IsAbstract && type.IsSealed))
        {
            Console.WriteLine($"Command `{type.FullName}` should be static");
        }
        var names = GetCommandAlias(type).ToArray();
        var helpText = GetCommandHelpText(type);
        var tree = BuildTree(type, AliasToString(names));
        
        TShockAPI.Commands.ChatCommands.Add(new TShockAPI.Command(args => ParseCommand(tree, args),
            names)
        {
            HelpText = helpText
        });

        return names;
    }
}