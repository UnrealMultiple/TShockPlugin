using System.Text.RegularExpressions;
using TShockAPI;
using TShockAPI.Hooks;

namespace ServerTools;

public partial class CommandPlaceholderHandle
{
    private static readonly List<PlaceholderResolver> _resolvers = [];

    public static void Register()
    {
        // *all*
        _resolvers.Add(new PlaceholderResolver(
            placeholder => placeholder == "*all*",
            _ => Plugin.ActivePlayers.Select(p => p.Name)
        ));

        // *组名*
        _resolvers.Add(new PlaceholderResolver(
            placeholder =>
            {
                var match = GroupRegex().Match(placeholder);
                return match.Success && TShock.Groups.Any(g => g.Name == match.Groups[1].Value);
            },
            placeholder =>
            {
                var groupName = GroupRegex().Match(placeholder).Groups[1].Value;
                return Plugin.ActivePlayers
                    .Where(p => p.Group.Name == groupName)
                    .Select(p => p.Name);
            }
        ));
    }

    public static void Handle(PlayerCommandEventArgs args)
    {
        var commandText = args.CommandText;

        var matches = PlaceholderRegex().Matches(commandText);
        if (matches.Count == 0)
        {
            return;
        }

        var placeholders = matches.Select(m => m.Value).Distinct().ToList();

        Dictionary<string, List<string>> expansions = [];
        foreach (var placeholder in placeholders)
        {
            var resolver = _resolvers.FirstOrDefault(r => r.CanResolve(placeholder));
            if (resolver == null)
            {
                return;
            }

            var values = resolver.Resolve(placeholder).ToList();
            if (values.Count == 0)
            {
                return;
            }

            expansions[placeholder] = values;
        }

        args.Handled = true;
        ExpandAndExecute(commandText, placeholders, expansions, 0, [], args.Player);
    }

    private static void ExpandAndExecute(
        string commandText,
        List<string> placeholders,
        Dictionary<string, List<string>> expansions,
        int index,
        Dictionary<string, string> selected,
        TSPlayer player)
    {
        if (index >= placeholders.Count)
        {
            var resolved = commandText;
            foreach (var (ph, value) in selected)
            {
                resolved = resolved.Replace(ph, value);
            }
            Commands.HandleCommand(player, "/" + resolved);
            return;
        }

        var current = placeholders[index];
        foreach (var value in expansions[current])
        {
            selected[current] = value;
            ExpandAndExecute(commandText, placeholders, expansions, index + 1, selected, player);
        }
    }

    [GeneratedRegex(@"\*[^*]+\*")]
    private static partial Regex PlaceholderRegex();

    [GeneratedRegex(@"\*([^*]+)\*")]
    private static partial Regex GroupRegex();

    private sealed class PlaceholderResolver(
        Func<string, bool> canResolve,
        Func<string, IEnumerable<string>> resolve)
    {
        public bool CanResolve(string placeholder)
        {
            return canResolve(placeholder);
        }

        public IEnumerable<string> Resolve(string placeholder)
        {
            return resolve(placeholder);
        }
    }
}