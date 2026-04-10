using System.Reflection;
using Newtonsoft.Json;

namespace Economics.Deal;

public static class ItemNameMapper
{
    private static readonly Dictionary<int, string> IdToName = new();
    private static readonly Dictionary<string, List<int>> NameIndex = new();

    public static void Load()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith("item.json"));

        if (resourceName == null)
        {
            return;
        }

        using var stream = assembly.GetManifestResourceStream(resourceName)!;
        using var reader = new StreamReader(stream);
        var items = JsonConvert.DeserializeObject<List<ItemEntry>>(reader.ReadToEnd());
        if (items == null)
        {
            return;
        }

        foreach (var item in items)
        {
            IdToName[item.Id] = item.Name;
        }
    }

    public static string? GetName(int id)
    {
        return IdToName.TryGetValue(id, out var name) ? name : null;
    }

    public static HashSet<int> SearchIds(string keyword)
    {
        var result = new HashSet<int>();
        foreach (var (id, name) in IdToName)
        {
            if (name.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            {
                result.Add(id);
            }
        }

        return result;
    }

    private class ItemEntry
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
    }
}
