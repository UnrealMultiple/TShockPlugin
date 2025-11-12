using System.Diagnostics.CodeAnalysis;
using TShockAPI;
using TShockAPI.DB;

namespace LazyAPI.Commands;

internal static class CommandParser
{
    public delegate bool Parser(string arg, [MaybeNullWhen(false)] out object obj);

    private static bool TryParseBool(string arg, out object obj)
    {
        var result = bool.TryParse(arg, out var t);
        obj = t;
        return result;
    }

    private static bool TryParseInt(string arg, out object obj)
    {
        var result = int.TryParse(arg, out var t);
        obj = t;
        return result;
    }

    private static bool TryParseLong(string arg, out object obj)
    {
        var result = long.TryParse(arg, out var t);
        obj = t;
        return result;
    }

    private static bool TryParseString(string arg, out object obj)
    {
        obj = arg;
        return true;
    }

    private static bool TryParseDateTime(string arg, out object obj)
    {
        var result = DateTime.TryParse(arg, out var t);
        obj = t;
        return result;
    }

    private static bool TryParseTSPlayer(string arg, [MaybeNullWhen(false)] out object obj)
    {
        var plr = TSPlayer.FindByNameOrID(arg);
        if (plr.Count == 1)
        {
            obj = plr.Single();
            return true;
        }

        obj = null;
        return false;
    }

    private static bool TryParseAccount(string arg, out object obj)
    {
        var account = TryParseInt(arg, out var t) 
            ? TShock.UserAccounts.GetUserAccountByID((int)t) 
            : TShock.UserAccounts.GetUserAccountByName(arg);
        obj = account;
        return account != null;
    }

    private static readonly Dictionary<Type, Parser> parsers = new()
    {
        [typeof(bool)] = TryParseBool,
        [typeof(int)] = TryParseInt,
        [typeof(long)] = TryParseLong,
        [typeof(string)] = TryParseString,
        [typeof(DateTime)] = TryParseDateTime,
        [typeof(TSPlayer)] = TryParseTSPlayer,
        [typeof(UserAccount)] = TryParseAccount
    };

    private static readonly Dictionary<Type, string> friendlyName = new()
    {
        [typeof(bool)] = GetString("bool"),
        [typeof(int)] = GetString("int"),
        [typeof(long)] = GetString("long"),
        [typeof(string)] = GetString("str"),
        [typeof(DateTime)] = GetString("date"),
        [typeof(TSPlayer)] = GetString("player"),
        [typeof(UserAccount)] = GetString("account")
    };

    public static Parser GetParser(Type type)
    {
        return parsers[type];
    }

    public static string GetFriendlyName(Type type)
    {
        return friendlyName[type];
    }
}