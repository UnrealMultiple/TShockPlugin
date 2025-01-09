using LazyAPI.Attributes;
using LazyAPI.Enums;
using System.Reflection;
using Terraria;

namespace LazyAPI.Utility;
public class GameProgressHelper
{
    public static List<string> Names { get; } = typeof(ProgressType).GetFields()
        .Where(f => f.FieldType == typeof(ProgressType))
        .Select(f => f.GetCustomAttribute<ProgressNameAttribute>()!.Names.First()).ToList();

    public static List<string> AllName { get; } = typeof(ProgressType).GetFields()
        .Where(f => f.FieldType == typeof(ProgressType))
        .SelectMany(f => f.GetCustomAttribute<ProgressNameAttribute>()!.Names).ToList();

    public static Dictionary<ProgressType, List<string>> DefaultProgressTypes { get; } = typeof(ProgressType)
        .GetFields()
        .Where(f => f.FieldType == typeof(ProgressType))
        .ToDictionary(f => (ProgressType) f.GetValue(null)!, f => f.GetCustomAttribute<ProgressNameAttribute>()!.Names.ToList());

    public static readonly Dictionary<string, ProgressType> DefaultProgressNames = typeof(ProgressType)
        .GetFields()
        .Where(f => f.FieldType == typeof(ProgressType))
        .SelectMany(field => field.GetCustomAttribute<ProgressNameAttribute>()!.Names.Select(a => (field, a)))
        .ToDictionary(t => t.a, t => (ProgressType) t.field.GetValue(null)!);


    public static bool InBestiaryDB(int ID)
    {
        var value = Main.BestiaryDB.FindEntryByNPCID(ID).UIInfoProvider.GetEntryUICollectionInfo().UnlockState;
        return value == Terraria.GameContent.Bestiary.BestiaryEntryUnlockState.CanShowDropsWithDropRates_4;
    }

    public static bool CompareVlaue(ProgressMapAttribute? map, object? obj)
    {
        var flag = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;
        var value = map?.Target.GetField(map.Filed, flag)?.GetValue(obj)
                    ?? map?.Target.GetProperty(map.Filed, flag)?.GetValue(obj);
        return value != null && map != null ? map.Value.Equals(value) : false;
    }
}
