using Economics.Core.Attributes;
using Economics.Core.Enumerates;
using System.Reflection;
using Terraria;
using Terraria.GameContent.Events;
using TShockAPI;

namespace Economics.Core.Extensions;

public static class GameProgress
{
    public static List<string> Names = typeof(ProgressType).GetFields()
        .Where(f => f.FieldType == typeof(ProgressType))
        .Select(f => f.GetCustomAttribute<ProgressName>()!.Names.First()).ToList();

    public static List<string> _names = typeof(ProgressType).GetFields()
        .Where(f => f.FieldType == typeof(ProgressType))
        .SelectMany(f => f.GetCustomAttribute<ProgressName>()!.Names).ToList();


    public static Dictionary<string, bool> GetProgress(this TSPlayer Player)
    {
        var progress = new Dictionary<string, bool>();
        foreach (var field in typeof(ProgressType).GetFields().Where(f => f.FieldType == typeof(ProgressType)))
        {
            var code = false;
            field.GetCustomAttributes<ProgressMap>().ForEach(x =>
            {
                var targetValue = typeof(NPC).GetField(x.Filed, BindingFlags.Public | BindingFlags.Static)?.GetValue(null) ??
                typeof(Main).GetField(x.Filed, BindingFlags.Public | BindingFlags.Static)?.GetValue(null) ??
                typeof(DD2Event).GetField(x.Filed, BindingFlags.Public | BindingFlags.Static)?.GetValue(null) ??
                typeof(Main).GetProperty(x.Filed)?.GetValue(Main.instance) ??
                typeof(BirthdayParty).GetProperty(x.Filed, BindingFlags.Public | BindingFlags.Static)?.GetValue(null) ??
                typeof(Player).GetField(x.Filed, BindingFlags.Public | BindingFlags.Instance)?.GetValue(Player.TPlayer) ??
                typeof(Player).GetProperty(x.Filed, BindingFlags.Public | BindingFlags.Instance)?.GetValue(Player.TPlayer)!;
                code = targetValue.Equals(x.value);
            });
            field.GetCustomAttributes<ProgressName>().ForEach(x => x.Names.ForEach(v => progress.Add(v, code)));
        }
        return progress;
    }

    public static bool InProgress(this TSPlayer Player, IEnumerable<string> names)
    {
        var gameProgress = Player.GetProgress();
        foreach (var name in names)
        {
            var anti = false;
            var pn = name;
            if (name.StartsWith('!'))
            {
                anti = true;
                pn = name[1..];
            }
            if (gameProgress.TryGetValue(pn, out var code))
            {
                if (code == anti)
                {
                    return false;
                }
            }
        }
        return true;
    }
}