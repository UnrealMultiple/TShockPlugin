namespace DamageRuleLoot;

public static class CritTracker
{
    public static Dictionary<string, int> CritCounts = new Dictionary<string, int>();
    public static void UpdateCritCount(string name)
    {
        if (!CritCounts.ContainsKey(name))
        {
            CritCounts[name] = 0;
        }
        CritCounts[name]++;
    }

    public static int GetCritCount(string name)
    {
        return CritCounts.TryGetValue(name, out var count) ? count : 0;
    }
}
