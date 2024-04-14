using System.Reflection;
using Terraria;
using TShockAPI;

namespace ProgressBag;

public static class ProgressUtil
{
    public static List<string> Names { get; } = typeof(ProgressType).GetFields()
        .Where(f => f.FieldType == typeof(ProgressType))
        .Select(f => f.GetCustomAttribute<ProgressNameAttribute>()!.Names.First()).ToList();

    public static List<string> AllName { get; } = typeof(ProgressType).GetFields()
        .Where(f => f.FieldType == typeof(ProgressType))
        .SelectMany(f => f.GetCustomAttribute<ProgressNameAttribute>()!.Names).ToList();


    public static bool InBestiaryDB(int ID)
    {
        var value = Main.BestiaryDB.FindEntryByNPCID(ID).UIInfoProvider.GetEntryUICollectionInfo().UnlockState;
        return value == Terraria.GameContent.Bestiary.BestiaryEntryUnlockState.CanShowDropsWithDropRates_4;
    }

    public static bool CompareVlaue(ProgressMapAttribute? map, object? obj)
    {
        var flag = BindingFlags.Public | BindingFlags.Static;
        var value = map?.Target.GetField(map.Filed, flag)?.GetValue(obj)
                    ?? map?.Target.GetProperty(map.Filed, flag)?.GetValue(obj);
        if (value == null && map != null)
            return map.Value.Equals(value);
        return false;
    }

    public static Dictionary<string, bool> GetProgress(this TSPlayer Player)
    {
        var progress = new Dictionary<string, bool>();
        foreach (var field in typeof(ProgressType).GetFields().Where(f => f.FieldType == typeof(ProgressType)))
        {
            var state = false;
            var map = field.GetCustomAttribute<ProgressMapAttribute>();
            var progName = field.GetCustomAttribute<ProgressNameAttribute>();
            if (map?.Target == typeof(NPC))
            {
                state = (ProgressType)field.GetValue(-1)! switch
                {
                    ProgressType.EvilBoss => InBestiaryDB(Terraria.ID.NPCID.EaterofWorldsHead) && CompareVlaue(map, null),
                    ProgressType.Brainof => InBestiaryDB(Terraria.ID.NPCID.BrainofCthulhu) && CompareVlaue(map, null),
                    _ => CompareVlaue(map, null),
                };
            }
            else if (map?.Target == typeof(Player))
            {
                state = CompareVlaue(map, Player.TPlayer);
            }
            else
            {
                state = CompareVlaue(map, null);
            }
            foreach (var name in progName!.Names)
            {
                progress[name] = state;
            }
        }
        return progress;
    }

    public static bool InProgress(this TSPlayer Player, IEnumerable<string> names)
    {
        var gameProgress = Player.GetProgress();
        foreach (var name in names)
        {
            return gameProgress.TryGetValue(name, out var code) && code;
        }
        return true;
    }
}
