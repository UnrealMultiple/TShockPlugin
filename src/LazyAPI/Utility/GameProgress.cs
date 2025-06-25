using LazyAPI.Attributes;
using LazyAPI.Enums;
using ProgressHelper;
using System.Reflection;
using Terraria;

namespace LazyAPI.Utility;
public class GameProgress
{
    public static Dictionary<string, IProgressMap> DefaultProgressNames { get; } = typeof(ProgressType)
        .Assembly
        .GetTypes()
        .Where(f => f.IsDefined(typeof(ProgressMatchAttribute)))
        .Select(t => (match : t.GetCustomAttribute<ProgressMatchAttribute>(), Instance: Activator.CreateInstance(t) ))
        .SelectMany(t => t.match!.Names.Select(a => (t.Instance, a)))
        .ToDictionary(t => t.a, t => (IProgressMap)t.Instance!);

    public static Dictionary<ProgressType, IProgressMap> DefaultProgressTypes { get; } = DefaultProgressNames.Values
        .GroupBy(p => p.GetType().GetCustomAttribute<ProgressMatchAttribute>()!.Type)
        .ToDictionary(g => g.Key, g => g.First());


    public static bool InBestiaryDB(int ID)
    {
        var value = Main.BestiaryDB.FindEntryByNPCID(ID).UIInfoProvider.GetEntryUICollectionInfo().UnlockState;
        return value == Terraria.GameContent.Bestiary.BestiaryEntryUnlockState.CanShowDropsWithDropRates_4;
    }
}
