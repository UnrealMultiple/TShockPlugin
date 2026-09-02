using TShockAPI;

namespace Economics.Core.Utils;

public static class PlaceholderManager
{
    private static readonly Dictionary<string, Func<TSPlayer, string>> _placeholders = [];
    public static void Register(string key, Func<TSPlayer, string> resolver)
    {
        _placeholders[key] = resolver;
    }

    public static string Resolve(string text, TSPlayer? player)
    {
        if (string.IsNullOrEmpty(text) || player == null)
        {
            return text;
        }

        foreach (var kv in _placeholders)
        {
            text = text.Replace($"{{{kv.Key}}}", kv.Value(player) ?? "");
        }
        return text;
    }
}